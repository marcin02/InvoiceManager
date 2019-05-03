using AppUI.Interfaces;
using AppUI.Models;
using AppUI.ViewModels;
using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUI.Factories
{
    public class AddTransactionFactory : IAddTransactionFactory
    {
        private readonly IQueries _queries;

        public AddTransactionFactory(IQueries queries)
        {
            _queries = queries;
        }

        public AddTransactionViewModel Create(AddTransactionModel model)
        {
            return new AddTransactionViewModel(_queries, model);
        }
    }
}
