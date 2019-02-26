using DataAccess.Models;
using System;
using System.Collections.Generic;

namespace DataAccess.Interfaces
{
    public interface IQueries
    {
        List<TransactionFullModel> SelectTransactionFull(DateTime from, DateTime to,string filters);
        List<TransactionTypeModel> SelectTransactionType();
        List<UsersModel> SelectUsers();
        int SelectLastTransactionFullId();
        decimal SelectLastBalance();
        void InsertTransaction(InsertTransactionModel model);
        void InsertTransactionInfo(InsertTransactionModelInfo model);
    }
}