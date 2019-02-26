using System.Collections.Generic;
using DataAccess.Models;

namespace AppUI.Interfaces
{
    public interface IAddTransactionModel
    {
        decimal Balance { get; set; }
        List<TransactionTypeModel> TransactionType { get; set; }
        List<UsersModel> Users { get; set; }
    }
}