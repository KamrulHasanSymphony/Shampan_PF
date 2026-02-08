using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
  public class SalaryInformationVM
    {        
		[Display(Name = "SL No")]
        public string SLNo { get; set; }
        public string EmployeeId { get; set; }        
		[Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public string Code { get; set; }
        public string DesignationId { get; set; }
        public string Designation { get; set; }

        public string DepartmentId { get; set; }
        public string Department { get; set; }

        public string FiscalYearDetailId { get; set; }        
		[Display(Name = "Period Name")]
        public string PeriodName { get; set; }

        public string SalaryTypeId { get; set; }        
		[Display(Name = "Salary Type")]
        public string SalaryType { get; set; }

        public string ProjectId { get; set; }
        public string Project { get; set; }

        public string SectionId { get; set; }
        public string Section { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
              
		[Display(Name = "Join Date")]
        public string JoinDate { get; set; }        
		[Display(Name = "Earning / Deduction")]
        public int IsEarning { get; set; }
    }
}
