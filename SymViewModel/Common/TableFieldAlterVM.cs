using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class TableFieldAlterVM
    {
		[Display(Name = "Table Name")]
        public string tableName { get; set; }        
		[Display(Name = "Field Name")]
        public string fieldName { get; set; }
		[Display(Name = "Field Type")]
        public string fieldType { get; set; }
    }
}
