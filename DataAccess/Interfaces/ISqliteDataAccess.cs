using DataAccess.Models;
using System;
using System.Collections.Generic;

namespace DataAccess.Interfaces
{
    public interface ISqliteDataAccess
    {
        string GetConnectionString(string id = "Default");
        List<T> LoadBasicData<T>(string sql);
        T LoadSingleValue<T>(string sql);
        List<TransactionFullModel> LoadTransaction(DateTime from, DateTime to, string sql);
        void SaveBasicData<T>(string sql, T data);
        void SaveDataWithTwoParameters<T>(string sql, T parameter1, T parameter2);
    }
}