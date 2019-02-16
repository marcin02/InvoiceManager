using Caliburn.Micro;
using DataAccess;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AppUI.ViewModels
{
    public class ShellViewModel : Screen
    {
        public ShellViewModel(IQueries queries)
        {
            _queries = queries;
            GetTransactions();
            GetTransactionType();
            GetUsers();
        }

        private IQueries _queries;

        public BindableCollection<TransactionFullModel> TransactionFull { get; set; }
        public BindableCollection<string> TransactionType { get; set; }
        public BindableCollection<UsersModel> Users { get; set; }

        private string _amountOnScreen;

        public string AmountOnScreen
        {
            get { return _amountOnScreen; }
            set { _amountOnScreen = value; NotifyOfPropertyChange(() => AmountOnScreen); }
        }


        private void GetTransactions()
        {
            List<TransactionFullModel> model = new List<TransactionFullModel>(_queries.SelectTransactionFull());            
            TransactionFull = new BindableCollection<TransactionFullModel>();

            foreach (var item in model)
            {
                if(item.TransactionType.Type != "Wpłata")              
                {
                    string amount = $"-{item.Amount}";
                    item.Amount = Convert.ToDecimal(amount);
                }
                item.TransactionInfo.Company = $"Nazwa firmy: {item.TransactionInfo.Company}";
                item.TransactionInfo.Invoice = $"Numer faktury: {item.TransactionInfo.Invoice}";
                TransactionFull.Add(item);
            }
        }

        private void GetTransactionType()
        {
            List<TransactionTypeModel> model = new List<TransactionTypeModel>(_queries.SelectTransactionType());
            TransactionType = new BindableCollection<string>();

            foreach (var item in model)
            {
                TransactionType.Add(item.Type);
            }
        }

        private void GetUsers()
        {
            List<UsersModel> model = new List<UsersModel>(_queries.SelectUsers());
            Users = new BindableCollection<UsersModel>();

            foreach (var item in model)
            {
                Users.Add(item);
            }
        }
    }
}
