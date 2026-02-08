using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
    public class SalaryEmployeeVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string EmployeeStatus { get; set; }
        public string GradeId { get; set; }
        public bool IsHold { get; set; }
        public string HoldBy { get; set; }
        public string HoldAt { get; set; }
        public string UnHoldBy { get; set; }
        public string UnHoldAt { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        public string CodeF { get; set; }

        public string CodeT { get; set; }

        public bool IsEmployeeChecked { get; set; }

        public int FiscalYear { get; set; }


        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }

        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        public string Designation { get; set; }




        public string PeriodName { get; set; }

        public string PeriodStart { get; set; }

        public string PeriodEnd { get; set; }
    }
}
