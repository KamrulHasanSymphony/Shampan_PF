using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Loan
{
    public class EmployeeLoanDetailVM
    {
        public int Id { get; set; }
        public string EmployeeLoanId { get; set; }
        public string EmployeeId { get; set; }

        [Display(Name = "Installment Amount")]
        public decimal InstallmentAmount { get; set; }
        [Display(Name = "Paid Amount")]
        public decimal InstallmentPaidAmount { get; set; }
        [Display(Name = "Schedule Date")]
        public string PaymentScheduleDate { get; set; }
        [Display(Name = "Payment Date")]
        public string PaymentDate { get; set; }
        [Display(Name = "Hold")]
        public bool IsHold { get; set; }
        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }
        public int InstallmentSLNo { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        [Required]
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
                
		[Display(Name = "Loan Type")]
        public string LoanType_E { get; set; }        
		[Display(Name = "Principal Amount")]
        public decimal PrincipalAmount { get; set; }        
		[Display(Name = "Interest Amount")]
        public decimal InterestAmount { get; set; }

        [Display(Name = "Duplicate")]
        public bool HaveDuplicate { get; set; }
        [Display(Name = "Duplicate")]
        public int DuplicateID { get; set; }




        public string BranchId { get; set; }
        //public string LoanType_E { get; set; }
        //public string EmployeeId { get; set; }
        //public decimal PrincipalAmount { get; set; }

                
		[Display(Name = "Interest Policy")]
        public string InterestPolicy { get; set; }        
		[Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }
        //public decimal InterestAmount { get; set; }        
		[Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }        
		[Display(Name = "N. Of Installment")]
        public decimal NumberOfInstallment { get; set; }        
		[Display(Name = "Approve Date")]
        public string ApprovedDate { get; set; }        
		[Display(Name = "Start Date")]
        public string StartDate { get; set; }        
		[Display(Name = "End Date")]
        public string EndDate { get; set; }        
		[Display(Name = "ApplicationDate")]
        public string ApplicationDate { get; set; }
		[Display(Name = "Approved")]
        public bool IsApproved { get; set; }
        //public bool IsHold { get; set; }
        //public bool HaveDuplicate { get; set; }
        //public string Remarks { get; set; }        
		[Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        //public decimal InstallmentAmount { get; set; }
        //public decimal InstallmentPaidAmount { get; set; }
        //public string PaymentScheduleDate { get; set; }
        //public string PaymentDate { get; set; }
        public string Code { get; set; }        
		[Display(Name = "Employee Name")]
        public string EmpName { get; set; }        
		[Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public string Project { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string ProjectId { get; set; }
        public string SectionId { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }        
		[Display(Name = "Grade")]
        public string GradeId { get; set; }
        public string LoanNo { get; set; }
                
		[Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }        
		[Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        public string triger { get; set; }
                
		[Display(Name = "Install. Intr. Amount")]
        public decimal InstallmentInterestAmount { get; set; }
                
		[Display(Name = "Install. Princ. Amount")]
        public decimal InstallmentPrincipalAmount { get; set; }

        public decimal RefundAmount { get; set; }

        public string RefundDate { get; set; }

        public decimal PrincipalAmountPaid { get; set; }

        public decimal InterestAmountPaid { get; set; }

        public decimal TotalDuePrincipalAmount { get; set; }

        public decimal TotalDueInterestAmount { get; set; }

        public decimal SettlementAmount { get; set; }

        public decimal NoofInstallment { get; set; }
    }
}
