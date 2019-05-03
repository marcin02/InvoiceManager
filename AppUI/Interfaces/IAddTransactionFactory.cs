using AppUI.Models;
using AppUI.ViewModels;

namespace AppUI.Interfaces
{
    public interface IAddTransactionFactory
    {
        AddTransactionViewModel Create(AddTransactionModel model);
    }
}