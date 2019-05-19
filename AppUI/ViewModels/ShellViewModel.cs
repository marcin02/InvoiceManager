using AppUI.Interfaces;
using AppUI.Models;
using Caliburn.Micro;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AppUI.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        #region Constructor

        public ShellViewModel(IQueries queries, IWindowManager wm, IAddTransactionFactory addTransactionFactory)
        {
            _queries = queries;
            _addTransactionFactory = addTransactionFactory;
            _wm = wm;
            StartUp();
        }

        #endregion

        #region Interfaces and viewmodels

        private readonly IQueries _queries;
        private readonly IWindowManager _wm;
        private readonly IAddTransactionFactory _addTransactionFactory;

        #endregion

        #region Collections

        public BindableCollection<TransactionFullModel> TransactionFull { get; set; }
        public BindableCollection<TransactionFullModel> TransactionFullDisplay { get; set; }
        public BindableCollection<TransactionTypeModel> TransactionType { get; set; }
        public BindableCollection<UsersModel> Users { get; set; }

        #endregion

        #region Private properties

        private DateTime _fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        private DateTime _toDate = DateTime.Now;
        private UsersModel _selectedUser;
        private TransactionTypeModel _selectedTransaction;
        private string _searchTitle;
        private TransactionFullModel _selectedTransactionFull;     

        #endregion

        #region Public properties

        public DateTime FromDate
        {
            get { return _fromDate; }
            set { _fromDate = value; GetTransactions(); }
        }

        public TransactionTypeModel SelectedTransaction
        {
            get { return _selectedTransaction; }
            set { _selectedTransaction = value; Filter(SearchTitle); }
        }

        public UsersModel SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; Filter(SearchTitle); }
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; GetTransactions(); }
        }

        public string SearchTitle
        {
            get { return _searchTitle; }
            set { _searchTitle = value; Filter(SearchTitle); }
        }

        public TransactionFullModel SelectedTransactionFull
        {
            get { return _selectedTransactionFull; }
            set { _selectedTransactionFull = value; }
        }

        #endregion

        #region Methods for data acces

        private void DeleteTransaction()
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz wykonać tę operację?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                int id = SelectedTransactionFull.TransactionFullId;
                int update = 1;
                
                if(SelectedTransactionFull.Deleted == 0)
                {
                    update = 1;
                    SelectedTransactionFull.Deleted = update;
                }
                else if(SelectedTransactionFull.Deleted == 1)
                {
                    update = 0;
                    SelectedTransactionFull.Deleted = update;
                }

                _queries.UpdateTransaction(id, update);
            }
        }       
    
        private void StartUp()
        {
            GetTransactions();
            GetTransactionType();
            GetUsers();
        }

        private void GetTransactions()
        {
            List<TransactionFullModel> model = new List<TransactionFullModel>(_queries.SelectTransactionFull(FromDate, ToDate));
            PrepareTransactions(model);          
        }
                            
        private void GetTransactionType()
        {
            List<TransactionTypeModel> model = new List<TransactionTypeModel>(_queries.SelectTransactionType());
            TransactionType = new BindableCollection<TransactionTypeModel>();
            TransactionType.Add(new TransactionTypeModel { Type = "Wszystkie", TransactionTypeId = 0 });
            foreach (var item in model)
            {
                TransactionType.Add(item);
            }
        }

        private void GetUsers()
        {
            List<UsersModel> model = new List<UsersModel>(_queries.SelectUsers());
            Users = new BindableCollection<UsersModel>();
            Users.Add(new UsersModel { FirstName = "Wszyscy", LastName= "użytkownicy", UserId = 0});
            foreach (var item in model)
            {
                Users.Add(item);
            }
        }

        #endregion

        #region Methods

        private BindableCollection<T> Reload<T>(BindableCollection<T> input, BindableCollection<T> output)
        {
            output.Clear();

            foreach (var item in input)
            {
                output.Add(item);
            }

            return output;
        }

        private void Filter(string searchTextBox)
        {
            if (_selectedTransaction != null && _selectedUser != null)
            {
                List<TransactionFullModel> displayed = new List<TransactionFullModel>(TransactionFull as BindableCollection<TransactionFullModel>);
                if (_selectedUser.UserId != 0 && SelectedTransaction.TransactionTypeId != 0)
                {
                    displayed = displayed.Where(x => x.TransactionType.TransactionTypeId == _selectedTransaction.TransactionTypeId && x.Users.UserId == _selectedUser.UserId).ToList();
                }
                else if (_selectedUser.UserId == 0 && SelectedTransaction.TransactionTypeId == 0)
                {
                    Reload(TransactionFull, TransactionFullDisplay);
                }
                else if (_selectedUser.UserId != 0 && SelectedTransaction.TransactionTypeId == 0)
                {
                    displayed = displayed.Where(x => x.Users.UserId == _selectedUser.UserId).ToList();
                }
                else if (_selectedUser.UserId == 0 && SelectedTransaction.TransactionTypeId != 0)
                {
                    displayed = displayed.Where(x => x.TransactionType.TransactionTypeId == _selectedTransaction.TransactionTypeId).ToList();
                }

                if(!String.IsNullOrWhiteSpace(searchTextBox))
                {
                    displayed = displayed.FindAll(x => x.Title.Contains(SearchTitle)).ToList();
                }

                TransactionFullDisplay.Clear();

                foreach (var item in displayed)
                {
                    TransactionFullDisplay.Add(item);
                }
            }
        }  

        public void DeleteBtn()
        {
            DeleteTransaction();
            TransactionFull.Refresh();
        }

        private void PrepareTransactions(List<TransactionFullModel> model)
        {
            model.Reverse();

            if (TransactionFull == null)
            {
                TransactionFull = new BindableCollection<TransactionFullModel>();
                TransactionFullDisplay = new BindableCollection<TransactionFullModel>();
            }
            else
            {
                TransactionFull.Clear();
                TransactionFullDisplay.Clear();
            }

            foreach (var item in model)
            {
                if (item.TransactionType.Type != "Wpłata")
                {
                    string amount = $"-{item.Amount}";
                    item.Amount = Convert.ToDecimal(amount);
                }
                if (item.TransactionInfo == null) item.TransactionInfo = new TransactionInfoModel();
                TransactionFull.Add(item);
                TransactionFullDisplay.Add(item);
            }      
        }

        public void RefreshBtn()
        {
            GetTransactions();
        }

        public void ShowAddTransactionWindow()
        {
            AddTransactionModel model = new AddTransactionModel
            {
                Balance = _queries.SelectLastBalance(),
                Users = Users,
                TransactionType = this.TransactionType
            };

            var vm = _addTransactionFactory.Create(model);
            vm.PrepareCollections();
            _wm.ShowDialog(vm);
            GetTransactions();
        }
        
        #endregion
    }
}
