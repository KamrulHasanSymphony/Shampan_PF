using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class EmployeeMonthlyAbsenceVM
    {   
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public decimal AbsentDays { get; set; }
        public decimal LateInDays { get; set; }
        public decimal EarlyOutDays { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string Department { get; set; }

        public string Designation { get; set; }

        public string Project { get; set; }

        public string Section { get; set; }
        [Display(Name = "Employee Name")] 
		public string EmpName { get; set; }
        public string Code { get; set; }
        public int SearchFid { get; set; }


        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        public string TransactionType { get; set; }

        public decimal AbsentDaysPrevious { get; set; }

        public decimal LateInDaysPrevious { get; set; }

        public decimal EarlyOutDaysPrevious { get; set; }

        public bool IsManual { get; set; }
    }
}
