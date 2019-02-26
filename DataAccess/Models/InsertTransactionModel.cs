using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class InsertTransactionModel
    {
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }        
        public string Title { get; set; }
        public int TransactionTypeId { get; set; }
        public int UserId { get; set; }
    }
}
