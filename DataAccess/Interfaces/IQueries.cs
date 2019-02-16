using DataAccess.Models;
using System.Collections.Generic;

namespace DataAccess.Interfaces
{
    public interface IQueries
    {
        List<TransactionFullModel> SelectTransactionFull();
        List<TransactionTypeModel> SelectTransactionType();
        List<UsersModel> SelectUsers();
    }
}