using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SymViewModel.Common
{
    public class DbCreateVM
    {
        public string Id { get; set; }
        [Display(Name = "Log IN")] 
        public string LogIN { get; set; }
        public string Password { get; set; }
        [Display(Name = "Server Name")] 
        public string ServerName { get; set; }
        [Display(Name = "Database Name")]
        public string DatabaseName { get; set; }
        public string Name { get; set; }
    }
}
