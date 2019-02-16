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

        public List<TransactionFullModel> SelectTransactionFull()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                string sql = @"SELECT Title, Amount, Balance, CreationDate, TransactionFull.UserId, Users.UserId, Users.FirstName, Users.LastName,
                                      TransactionFull.TransactionTypeId, TransactionType.Type, TransactionInfo.TransactionInfoId, 
                                      TransactionInfo.Company, TransactionInfo.Invoice
                                FROM  TransactionFull
                                INNER JOIN Users ON Users.UserId = TransactionFull.UserId
                                INNER JOIN TransactionType ON TransactionType.TransactionTypeId = TransactionFull.TransactionTypeId
                                LEFT JOIN TransactionInfo ON TransactionInfo.TransactionFullId = TransactionFull.TransactionFullId";



                var output = cnn.Query<TransactionFullModel, UsersModel, TransactionTypeModel, TransactionInfoModel, TransactionFullModel>(sql,
                    (transactionFullModel, usersModel, transactionTypeModel, transactionInfoModel) => 
                    { transactionFullModel.Users = usersModel;
                      transactionFullModel.TransactionType = transactionTypeModel;
                      transactionFullModel.TransactionInfo = transactionInfoModel ;
                      return transactionFullModel;
                    }, splitOn: "UserId,TransactionTypeId,TransactionInfoId").ToList();

                return output;
            }
        }

        public List<TransactionTypeModel> SelectTransactionType()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                string sql = @"SELECT Type
                               FROM TransactionType";

                var output = cnn.Query<TransactionTypeModel>(sql).ToList();

                return output;
            }
        }

        public List<UsersModel> SelectUsers()
        {
            using (IDbConnection cnn = new SQLiteConnection(_sqlliteDataAcces.GetConnectionString()))
            {
                string sql = @"SELECT FirstName, LastName
                               FROM Users";

                var output = cnn.Query<UsersModel>(sql).ToList();

                return output;
            }
        }
    }
}
