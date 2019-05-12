using AppUI.Interfaces;
using AppUI.Models;
using AppUI.ViewModels;
using DataAccess.Interfaces;

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
