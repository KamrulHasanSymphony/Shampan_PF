using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class CodeVM
    {
        public string CodeId { get; set; }
		[Display(Name = "Code Group")]
        public string CodeGroup { get; set; }        
		[Display(Name = "Code Name")]
        public string CodeName { get; set; }
		[Display(Name = "Prefix")]
        public string prefix { get; set; }
		[Display(Name = "Length")]
        public string Lenth { get; set; }
		[Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
    }
}
