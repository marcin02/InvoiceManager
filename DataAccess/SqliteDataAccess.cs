using Dapper;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SqliteDataAccess : ISqliteDataAccess
    {
        public string GetConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public List<T> LoadBasicData<T>(string sql) //For Users, TransactionTypes
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString()))
            {
                return cnn.Query<T>(sql).ToList();
            }
        }

        public T LoadSingleValue<T>(string sql) //For TransactionId, LastBalance
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString()))
            {
                return cnn.ExecuteScalar<T>(sql);
            }
        }

        public List<TransactionFullModel> LoadTransaction(DateTime from, DateTime to, string sql)
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString()))
            {
                var p = new
                {
                    From = from,
                    To = to
                };

                var output = cnn.Query<TransactionFullModel, UsersModel, TransactionTypeModel, TransactionInfoModel, ReportNumberModel, TransactionFullModel>(sql,
                    (transactionFullModel, usersModel, transactionTypeModel, transactionInfoModel, reportNumber) =>
                    {
                        transactionFullModel.Users = usersModel;
                        transactionFullModel.TransactionType = transactionTypeModel;
                        transactionFullModel.TransactionInfo = transactionInfoModel;
                        transactionFullModel.ReportNumber = reportNumber;

                        return transactionFullModel;
                    }, p, splitOn: "UserId,TransactionTypeId,TransactionInfoId,ReportNumberId").ToList();

                return output;
            }
        }

        public void SaveBasicData<T>(string sql, T data) //For InsertTransaction, InsertTransatcionInfo
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString()))
            {
                cnn.Execute(sql, data);
            }
        }

        public void SaveDataWithTwoParameters<T>(string sql, T parameter1, T parameter2)
        {
            var p = new
            {
                Parameter1 = parameter1,
                Parameter2 = parameter2
            };

            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString()))
            {
                cnn.Execute(sql, p);
            }
        }
    }
}
