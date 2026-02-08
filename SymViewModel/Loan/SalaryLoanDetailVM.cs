using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Loan
{
   public class SalaryLoanDetailVM
    {
       public string Project;
        public int Id { get; set; }
        public string SalaryLoanId { get; set; }
        public string EmployeeId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string SectionId { get; set; }

        public string Code { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        [Display(Name = "Loan Type")]
        public string LoanTypeName { get; set; }

        [Display(Name = "Principal Amount")]
        public decimal PrincipalAmount { get; set; }
        [Display(Name = "Loan Amount")]
        public decimal LoanAmount { get; set; }
        [Display(Name = "Interest Amount")]
        public decimal InterestAmount { get; set; }
        [Display(Name = "Installment Amount")]
        public decimal InstallmentAmount { get; set; }


        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }        
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }


        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }

       

        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }

        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }

        public string Section { get; set; }

    

    

        [Display(Name = "Loan Type")]
        public string LoanType_E { get; set; }

        [Display(Name = "Code From")]
        public object CodeF { get; set; }

        [Display(Name = "Code To")]
        public object CodeT { get; set; }

        [Display(Name = "Fiscal Period To")]
        public object fidTo { get; set; }

        [Display(Name = "Period Start")]
        public string PeriodStart { get; set; }

        [Display(Name = "Order By")]
        public object Orderby { get; set; }
    }
}
