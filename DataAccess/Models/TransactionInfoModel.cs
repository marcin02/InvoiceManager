using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class TransactionInfoModel
    {
        public int TransactionInfoId {get; set;}
        public string Company { get; set; }
        public string Invoice { get; set; }
        public int TransactionFullId { get; set; }
    }
}
