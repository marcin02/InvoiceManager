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
    public class ShellViewModel : Screen
    {
        #region Constructor

        public ShellViewModel(IQueries queries)
        {
            _queries = queries;
            GetTransactions();
            GetTransactionType();
            GetUsers();
        }

        #endregion

        #region Interfaces

        private IQueries _queries;

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
            set { _fromDate = value; }
        }

        public TransactionTypeModel SelectedTransaction
        {
            get { return _selectedTransaction; }
            set { _selectedTransaction = value; }
        }

        public UsersModel SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; }
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set { _toDate = value; }
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
                }
                else if(SelectedTransactionFull.Deleted == 1)
                {
                    update = 0;
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

        private void GetTransactions()
        {
            string filters = $"{UseTransactionTypeFilter()} {UseUsersFilter()}";
            
            List<TransactionFullModel> model = new List<TransactionFullModel>(_queries.SelectTransactionFull(FromDate, ToDate, filters));
            PrepareTransactions(model);          
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
           
            WindowManager wm = new WindowManager();
            wm.ShowDialog(new AddTransactionViewModel(model, _queries));            
        }
        
        #endregion
    }
}
