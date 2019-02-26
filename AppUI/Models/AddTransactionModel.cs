using AppUI.Interfaces;
using Caliburn.Micro;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUI.Models
{
    public class AddTransactionModel
    {
        public decimal Balance { get; set; }
        public BindableCollection<UsersModel> Users { get; set; }
        public BindableCollection<TransactionTypeModel> TransactionType {get; set;}
    }
}
