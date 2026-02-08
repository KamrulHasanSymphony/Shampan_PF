using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
    public class SalarySheetVM
    {
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        public string Code { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public string EmployeeId { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Join Date")]
        public DateTime JoinDate { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        [Display(Name = "Salary Head")]
        public string SalaryHead { get; set; }
        public decimal Amount { get; set; }
        //////public string ProjectId { get; set; }
        //////public string DepartmentId { get; set; }
        //////public string SectionId { get; set; }
        //////public string DesignationId { get; set; }
        //////public string FiscalYearDetailId { get; set; }
        [Display(Name = "Earning / Deduction")]
        public int IsEarning { get; set; }
        [Display(Name = "Payment Date")]
        public string PaymentDate { get; set; }
        public string ReportType { get; set; }
        public string EmploymentType_E { get; set; }


        public string PeriodId { get; set; }

        public int FiscalYearDetailId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string CodeFrom { get; set; }
        public string CodeTo { get; set; }
        public string Orderby { get; set; }
        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string Other3 { get; set; }
        public string BankId { get; set; }
        public List<string> ProjectIdList { get; set; }

        public string SheetName { get; set; }

        public List<string> Other3List { get; set; }

        public string DBName { get; set; }

        public string MainTopGroup { get; set; }

        public string HoldStatus { get; set; }

        public string View { get; set; }

        public int FiscalYear { get; set; }

        public string MultipleProjectId { get; set; }

        public string MultiProjectId { get; set; }


        public string MultipleOther2 { get; set; }

        public string MultipleOther3 { get; set; }

        public string MultiOther3 { get; set; }

        public string FullPeriodName { get; set; }

        public int FiscalYearDetailIdTo { get; set; }


        public bool IsMultipleSalary { get; set; }

        public string CompanyName { get; set; }

        public string Gender { get; set; }

    }
}
