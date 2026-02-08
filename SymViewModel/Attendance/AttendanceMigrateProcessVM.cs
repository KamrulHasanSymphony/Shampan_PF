using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
  public  class AttendanceMigrateProcessVM
    {        
		[Display(Name = "Start Date")]
        public string StartDate { get; set; }
		[Display(Name = "End Date")]
        public string EndDate { get; set; }
        public string Remarks { get; set; }
        public bool IsManual { get; set; }
    }
}
