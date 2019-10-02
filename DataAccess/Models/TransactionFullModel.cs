using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class TransactionFullModel
    {
        public int TransactionFullId { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string  Date { get { return CreationDate.ToShortDateString(); } }
        public int Deleted { get; set; }
        public UsersModel Users { get; set; }
        public TransactionTypeModel TransactionType { get; set; }
        public TransactionInfoModel TransactionInfo { get; set; }
        public ReportNumberModel ReportNumber { get; set; }
    }
}
