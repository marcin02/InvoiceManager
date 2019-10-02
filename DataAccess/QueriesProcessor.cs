using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccess.SqliteDataAccess;

namespace DataAccess
{
    public class QueriesProcessor : IQueries
    {
        public QueriesProcessor(ISqliteDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        private readonly ISqliteDataAccess _dataAccess;

        public void InsertTransaction(InsertTransactionModel model)
        {
            string sql = @"INSERT INTO TransactionFull (Title, Amount, Balance, CreationDate, UserId, TransactionTypeId) 
                              VALUES (@Title, @Amount, @Balance, datetime('now'), @UserId, @TransactionTypeId);";

            _dataAccess.SaveBasicData(sql, model);
        }

        public void InsertTransactionInfo(InsertTransactionModelInfo model)
        {
            string sql = @"INSERT INTO TransactionInfo (Company, Invoice, TransactionFullId)  
                                  VALUES (@Company, @Invoice, @TransactionFullId);";

            _dataAccess.SaveBasicData(sql, model);
        }

        public decimal SelectLastBalance()
        {
            string sql = @"SELECT Balance FROM TransactionFull 
                                                           ORDER BY TransactionFullId DESC LIMIT 1;";

            return _dataAccess.LoadSingleValue<decimal>(sql);
        }

        public int SelectLastTransactionFullId()
        {
            string sql = @"SELECT TransactionFullId FROM TransactionFull 
                                                  ORDER BY TransactionFullId DESC LIMIT 1;";

            return _dataAccess.LoadSingleValue<int>(sql);
        }

        public List<TransactionFullModel> SelectTransactionFull(DateTime from, DateTime to)
        {
            string sql = @"SELECT TransactionFull.TransactionFullId, Title, Amount, Balance, CreationDate, Deleted, 
                                      TransactionFull.UserId, Users.UserId, Users.FirstName, Users.LastName,
                                      TransactionFull.TransactionTypeId, TransactionType.Type, 
                                      TransactionInfo.TransactionInfoId, TransactionInfo.Company, TransactionInfo.Invoice,
                                      TransactionFull.ReportNumberId, ReportNumber.ReportNumberId, ReportNumber.Number
                                FROM  TransactionFull
                                INNER JOIN Users ON Users.UserId = TransactionFull.UserId
                                INNER JOIN TransactionType ON TransactionType.TransactionTypeId = TransactionFull.TransactionTypeId
                                LEFT JOIN TransactionInfo ON TransactionInfo.TransactionFullId = TransactionFull.TransactionFullId
                                LEFT JOIN ReportNumber ON ReportNumber.ReportNumberId = TransactionFull.ReportNumberId 
                                WHERE date(CreationDate) >= date(@From) AND date(CreationDate) <= date(@To)";

            return _dataAccess.LoadTransaction(from, to, sql);
        }

        public List<TransactionTypeModel> SelectTransactionType()
        {
            string sql = @"SELECT TransactionTypeId, Type
                               FROM TransactionType";

            return _dataAccess.LoadBasicData<TransactionTypeModel>(sql);
        }

        public List<UsersModel> SelectUsers()
        {
            string sql = @"SELECT FirstName, LastName, UserId
                               FROM Users";

            return _dataAccess.LoadBasicData<UsersModel>(sql);
        }

        public void UpdateTransaction(int update, int id)
        {
            string sql = @"UPDATE TransactionFull 
                              SET Deleted = @Parameter1
                              WHERE TransactionFullId = @Parameter2";

            _dataAccess.SaveDataWithTwoParameters(sql, update, id);
        }
    }
}
