using DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class TransactionTypeModel
    {
        public int TransactionTypeId { get; set; }
        public string Type { get; set; }
    }
}
