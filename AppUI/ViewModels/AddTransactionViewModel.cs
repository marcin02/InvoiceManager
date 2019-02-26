using AppUI.Interfaces;
using AppUI.Models;
using Caliburn.Micro;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUI.ViewModels
{
    public class AddTransactionViewModel : Screen
    {
        #region Constructor

        public AddTransactionViewModel(AddTransactionModel addTransactionModel, IQueries queries)
        {
            _queries = queries;
            _addTransactionModel = addTransactionModel;
            Users = addTransactionModel.Users;
            TransactionType = addTransactionModel.TransactionType;
        }

        #endregion

        #region Private properties

        private decimal _amount;
        private string _company;
        private string _invoice;
        private string _title;
        private TransactionTypeModel _selectedTransactionType;
        private UsersModel _selectedUser;
        private bool _titleIsEnabled = false;
        private bool _amountIsEnabled = false;
        private bool _companyIsEnabled = false;
        private bool _invoiceIsEnabled = false;

        #endregion

        #region Public properties

        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        public bool AmountIsEnabled
        {
            get { return _amountIsEnabled; }
            set { _amountIsEnabled = value; NotifyOfPropertyChange(() => AmountIsEnabled); }
        }
        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }
        public bool CompanyIsEnabled
        {
            get { return _companyIsEnabled; }
            set { _companyIsEnabled = value; NotifyOfPropertyChange(() => CompanyIsEnabled); }
        }
        public string Invoice
        {
            get { return _invoice; }
            set { _invoice = value; }
        }
        public bool InvoiceIsEnabled
        {
            get { return _invoiceIsEnabled; }
            set { _invoiceIsEnabled = value; NotifyOfPropertyChange(() => InvoiceIsEnabled); }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public bool TitleIsEnabled
        {
            get { return _titleIsEnabled; }
            set { _titleIsEnabled = value; NotifyOfPropertyChange(() => TitleIsEnabled); }
        }
        public TransactionTypeModel SelectedTransactionType
        {
            get { return _selectedTransactionType; }
            set
            {
                _selectedTransactionType = value;
                EnableTextBoxes(_selectedTransactionType.Type);                
            }
        }
        public UsersModel SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; }
        }

        #endregion

        #region Models and interfaces

        private AddTransactionModel _addTransactionModel;
        private IQueries _queries;

        #endregion

        #region Collections

        public BindableCollection<UsersModel> Users { get; set; }
        public BindableCollection<TransactionTypeModel> TransactionType { get; set; }

        #endregion

        #region Methods      

        private void EnableTextBoxes(string value)
        {
            if (value != "")
            {
                TitleIsEnabled = true;
                AmountIsEnabled = true;

                if (value == "Wpłata" || value == "Wypłata")
                {
                    CompanyIsEnabled = false;
                    InvoiceIsEnabled = false;
                }
                else
                {
                    CompanyIsEnabled = true;
                    InvoiceIsEnabled = true;
                }
            }
        }        

        private decimal GetAndSetBalance()
        {
            decimal b = 0;

            if(SelectedTransactionType.Type == "Wpłata")
            {
                b = _addTransactionModel.Balance + _amount;
                return b;
            }
            else
            {
                b = _addTransactionModel.Balance - _amount;
                return b;
            }
        }

        public void Insert()
        {
            InsertTransactionModel model = new InsertTransactionModel
            {
                Amount = _amount,
                Balance = GetAndSetBalance(),
                Title = _title,
                TransactionTypeId = _selectedTransactionType.TransactionTypeId,
                UserId = _selectedUser.UserId
            };

            _queries.InsertTransaction(model);

            if(!string.IsNullOrWhiteSpace(Company) || !string.IsNullOrWhiteSpace(Invoice))
            {
                InsertTransactionModelInfo infoModel = new InsertTransactionModelInfo
                {
                    Company = _company,
                    Invoice = _invoice,
                    TransactionFullId = _queries.SelectLastTransactionFullId()
                };

                _queries.InsertTransactionInfo(infoModel);
            }

            TryClose();
        }

        public void Close()
        {
            TryClose();
        }

        #endregion
    }
}
