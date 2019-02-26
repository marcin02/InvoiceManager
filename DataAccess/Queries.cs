using Dapper;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class Queries : IQueries
    {
        public Queries(ISqliteDataAccess sqliteDataAcces)
        {
            _sqlliteDataAcces = sqliteDataAcces;
        }

        private ISqliteDataAccess _sqlliteDataAcces;

        public List<TransactionFullModel> SelectTransactionFull(DateTime from, DateTime to, string filters)
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                var p = new
                {
                    From = from,
                    To = to
                };


                string sql = @"SELECT TransactionFull.TransactionFullId, Title, Amount, Balance, CreationDate, TransactionFull.UserId, Users.UserId, Users.FirstName, Users.LastName,
                                      TransactionFull.TransactionTypeId, TransactionType.Type, TransactionInfo.TransactionInfoId, 
                                      TransactionInfo.Company, TransactionInfo.Invoice
                                FROM  TransactionFull
                                INNER JOIN Users ON Users.UserId = TransactionFull.UserId
                                INNER JOIN TransactionType ON TransactionType.TransactionTypeId = TransactionFull.TransactionTypeId
                                LEFT JOIN TransactionInfo ON TransactionInfo.TransactionFullId = TransactionFull.TransactionFullId
                                WHERE date(CreationDate) >= date(@From) AND date(CreationDate) <= date(@To)";

                string filteredSql = $"{sql} {filters}";

                var output = cnn.Query<TransactionFullModel, UsersModel, TransactionTypeModel, TransactionInfoModel, TransactionFullModel>(filteredSql,
                    (transactionFullModel, usersModel, transactionTypeModel, transactionInfoModel) => 
                    { transactionFullModel.Users = usersModel;
                      transactionFullModel.TransactionType = transactionTypeModel;
                      transactionFullModel.TransactionInfo = transactionInfoModel;

                      return transactionFullModel;
                    },p, splitOn: "UserId,TransactionTypeId,TransactionInfoId").ToList();

                return output;
            }
        }

        public List<TransactionTypeModel> SelectTransactionType()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                string sql = @"SELECT TransactionTypeId, Type
                               FROM TransactionType";

                var output = cnn.Query<TransactionTypeModel>(sql).ToList();

                return output;
            }
        }

        public List<UsersModel> SelectUsers()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                string sql = @"SELECT FirstName, LastName, UserId
                               FROM Users";

                var output = cnn.Query<UsersModel>(sql).ToList();

                return output;
            }
        }

        public int SelectLastTransactionFullId()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                var id = cnn.ExecuteScalar<int>("SELECT TransactionFullId FROM TransactionFull ORDER BY TransactionFullId DESC LIMIT 1;");

                return id;
            }
        }

        public decimal SelectLastBalance()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                var balance = cnn.ExecuteScalar<decimal>("SELECT Balance FROM TransactionFull ORDER BY TransactionFullId DESC LIMIT 1;");

                return balance;
            }
        }

        public void InsertTransaction(InsertTransactionModel model)
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                cnn.Execute("INSERT INTO TransactionFull (Title, Amount, Balance, CreationDate, UserId, TransactionTypeId) " +
                            "VALUES (@Title, @Amount, @Balance, datetime('now'), @UserId, @TransactionTypeId);", model);           
            }            
        }

        public void InsertTransactionInfo(InsertTransactionModelInfo model)
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                cnn.Execute(@"INSERT INTO TransactionInfo (Company, Invoice, TransactionFullId)  
                                  VALUES (@Company, @Invoice, @TransactionFullId);", model);
            }
        }
    }
}
