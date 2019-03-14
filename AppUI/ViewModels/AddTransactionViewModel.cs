using AppUI.Interfaces;
using AppUI.Models;
using Caliburn.Micro;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppUI.ViewModels
{
    public class AddTransactionViewModel : Screen, IDataErrorInfo
    {
        #region Constructor

        public AddTransactionViewModel(AddTransactionModel addTransactionModel, IQueries queries)
        {
            _queries = queries;
            _addTransactionModel = addTransactionModel;
            Users = addTransactionModel.Users;
            TransactionType = addTransactionModel.TransactionType;
            Users.RemoveAt(0);
            TransactionType.RemoveAt(0);
        }

        #endregion

        #region Private properties

        private string _amount;
        private decimal _amountDecimal;
        private bool _canExecute = true;
        private bool _canValidate = false;
        private string _company;
        private string _invoice;
        private string _title;
        private TransactionTypeModel _selectedTransactionType;
        private UsersModel _selectedUser;
        private bool _titleIsEnabled = false;
        private bool _amountIsEnabled = false;
        private bool _companyIsEnabled = false;
        private bool _invoiceIsEnabled = false;
        private string _titleLenght;
        private int _maxCharacters = 50;
        private int _maxCharactersCompany = 30;
        private string _companyLenght;
        private string _invoiceLenght;

        #endregion

        #region Public properties

        public string Amount
        {
            get { return _amount; }
            set { _amount = value; if(!string.IsNullOrWhiteSpace(_amount))_amountDecimal = Convert.ToDecimal(_amount); }
        }
        public bool AmountIsEnabled
        {
            get { return _amountIsEnabled; }
            set { _amountIsEnabled = value; NotifyOfPropertyChange(() => AmountIsEnabled); }
        }
        public string Company
        {
            get { return _company; }
            set { _company = value; GetLenghtCompany(); }
        }
        public bool CompanyIsEnabled
        {
            get { return _companyIsEnabled; }
            set { _companyIsEnabled = value; NotifyOfPropertyChange(() => CompanyIsEnabled); }
        }
        public string CompanyLenght
        {
            get { return _companyLenght; }
            set { _companyLenght = value; NotifyOfPropertyChange(() => CompanyLenght); }
        }
        public string Invoice
        {
            get { return _invoice; }
            set { _invoice = value; GetLenghtInvoice(); }
        }
        public bool InvoiceIsEnabled
        {
            get { return _invoiceIsEnabled; }
            set { _invoiceIsEnabled = value; NotifyOfPropertyChange(() => InvoiceIsEnabled); }
        }
        public string InvoiceLenght
        {
            get { return _invoiceLenght; }
            set { _invoiceLenght = value; NotifyOfPropertyChange(() => InvoiceLenght); }
        }
        public int MaxCharacters
        {
            get { return _maxCharacters; }
            set { _maxCharacters = value; }
        }
        public int MaxCharactersCompany
        {
            get { return _maxCharactersCompany; }
            set { _maxCharactersCompany = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; GetLenghtTitle(); }
        }
        public bool TitleIsEnabled
        {
            get { return _titleIsEnabled; }
            set { _titleIsEnabled = value; NotifyOfPropertyChange(() => TitleIsEnabled); }
        }
        public string TitleLenght
        {
            get { return _titleLenght; }
            set { _titleLenght = value; NotifyOfPropertyChange(() => TitleLenght); }
        }
        public TransactionTypeModel SelectedTransactionType
        {
            get { return _selectedTransactionType; }
            set
            {
                _selectedTransactionType = value;
                if(_selectedTransactionType != null) EnableTextBoxes(_selectedTransactionType.Type);                
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
       
        private void GetLenghtTitle()
        {
            if (_title != null)
            {
                int lenght = _title.Length;
                TitleLenght = lenght.ToString();
            }
        }

        private void GetLenghtCompany()
        {
            if (_company != null)
            {
                int lenght = _company.Length;
                CompanyLenght = lenght.ToString();
            }
        }

        private void GetLenghtInvoice()
        {
            if (_title != null)
            {
                int lenght = _invoice.Length;
                InvoiceLenght = lenght.ToString();
            }
        }

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
                b = _addTransactionModel.Balance + _amountDecimal;
                return b;
            }
            else
            {
                b = _addTransactionModel.Balance - _amountDecimal;
                return b;
            }
        }

        public void Insert()
        {
            Validate();
            if(_canExecute)
            {
                InsertHelper();
            }
            else
            {
                MessageBox.Show("Zaznaczone pola nie mogą być puste!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                _canExecute = true;
            }
        }

        private void InsertHelper()
        {
            if (GetAndSetBalance() >= 0)
            {
                InsertTransactionModel model = new InsertTransactionModel
                {
                    Amount = _amountDecimal,
                    Balance = GetAndSetBalance(),
                    Title = _title,
                    TransactionTypeId = _selectedTransactionType.TransactionTypeId,
                    UserId = _selectedUser.UserId
                };

                _queries.InsertTransaction(model);

                if (!string.IsNullOrWhiteSpace(Company) || !string.IsNullOrWhiteSpace(Invoice))
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
            else
            {
                MessageBox.Show("Saldo nie może być ujemne!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Close()
        {
            TryClose();
        }

        private void RefreshProperties()
        {
            NotifyOfPropertyChange(() => SelectedTransactionType);
            NotifyOfPropertyChange(() => SelectedUser);
            NotifyOfPropertyChange(() => Title);
            NotifyOfPropertyChange(() => Amount);
        }

        private void Validate()
        {
            _canValidate = true;
            RefreshProperties();
            _canValidate = false;
        }

        public string Error { get { return null; } }

        public string this[string columnName]
        {
            get
            {
                string resultOK = null;

                if (_canValidate)
                {
                    string resultNOK = "Błąd";

                    switch (columnName)
                    {
                        case "SelectedTransactionType":
                            if (SelectedTransactionType == null)
                            {
                                _canExecute = false;
                                return resultNOK;
                            }
                            break;

                        case "SelectedUser":
                            if (SelectedUser == null)
                            {
                                _canExecute = false;
                                return resultNOK;
                            }
                            break;

                        case "Title":
                            if (string.IsNullOrWhiteSpace(_title))
                            {
                                _canExecute = false;
                                return resultNOK;
                            }
                            break;

                        case "Amount":
                            if (_amountDecimal <= 0)
                            {
                                _canExecute = false;
                                return resultNOK;
                            }
                            break;
                    }
                }
                    return resultOK; 
            }
        }

        #endregion
    }
}
