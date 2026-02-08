using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class DatabaseTableVM
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string TableName { get; set; }
        ////public string SQLQuery { get; set; }
        public string SelectSQLQuery { get; set; }
        public string UpdateSQLQuery { get; set; }

        public string Name { get; set; }
    }
}
