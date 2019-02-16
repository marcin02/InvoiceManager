using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UsersModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName {get; set;}
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}
