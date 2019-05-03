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
            set { _selectedTransaction = value; GetTransactions(); }
        }

        public UsersModel SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; GetTransactions(); }
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; GetTransactions(); }
        }

        public string SearchTitle
        {
            get { return _searchTitle; }
            set { _searchTitle = value; }
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

        private void SearchTransactions()
        {
            string keyword = $"%{_searchTitle}%";
            List<TransactionFullModel> model = new List<TransactionFullModel>(_queries.SearchTransactionFull(keyword));
            PrepareTransactions(model);
        }

        private void StartUp()
        {
            GetTransactions();
            GetTransactionType();
            GetUsers();
        }

        private void GetTransactions()
        {
            string filters = $"{UseTransactionTypeFilter()} {UseUsersFilter()}";
            
            List<TransactionFullModel> model = new List<TransactionFullModel>(_queries.SelectTransactionFull(FromDate, ToDate, filters));
            PrepareTransactions(model);          
        }
        
        

        private string UseTransactionTypeFilter()
        {
            if (_selectedTransaction != null)
            {
                if (_selectedTransaction.Type != "Wszystkie")
                {
                    string filter = $"AND TransactionType.TransactionTypeId = '{SelectedTransaction.TransactionTypeId}'";

                    return filter;
                } 
            }

            return null;
        }

        private string UseUsersFilter()
        {
            if(_selectedUser != null)
            {
                if(_selectedUser.FullName != "Wszyscy użytkownicy")
                {
                    string filter = $"AND Users.UserId = '{_selectedUser.UserId}'";

                    return filter;
                }
            }

            return null;
        }         

        private void GetTransactionType()
        {
            List<TransactionTypeModel> model = new List<TransactionTypeModel>(_queries.SelectTransactionType());
            TransactionType = new BindableCollection<TransactionTypeModel>();
            TransactionType.Add(new TransactionTypeModel { Type = "Wszystkie" });
            foreach (var item in model)
            {
                TransactionType.Add(item);
            }
        }

        private void GetUsers()
        {
            List<UsersModel> model = new List<UsersModel>(_queries.SelectUsers());
            Users = new BindableCollection<UsersModel>();
            Users.Add(new UsersModel { FirstName = "Wszyscy", LastName= "użytkownicy" });
            foreach (var item in model)
            {
                Users.Add(item);
            }
        }

        #endregion

        #region Methods

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
            }
            else
            {
                TransactionFull.Clear();
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
            }
        }

        public void RefreshBtn()
        {
            GetTransactions();
        }

        public void Search()
        {
            SearchTransactions();
        }

        public void ShowAddTransactionWindow()
        {
            AddTransactionModel model = new AddTransactionModel
            {
                Balance = _queries.SelectLastBalance(),
                Users = Users,
                TransactionType = this.TransactionType
            };

            // WindowManager wm = new WindowManager();
            //   _addTransactionViewModel.ActivateWith(model);
            var vm = _addTransactionFactory.Create(model);
            vm.PrepareCollections();
            _wm.ShowDialog(vm);
            GetTransactions();
        }
        
        #endregion
    }
}
