using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Loan
{
   public class EmployeeLoanVM
    {
       public string Operation { get; set; }
       public decimal PFBalance { get; set; }
       public decimal AvailableRate { get; set; }


        public string Id { get; set; }
       [Display(Name="Type")]
        public string LoanType_E { get; set; }
       public string LoanNo { get; set; }
        public string EmployeeId { get; set; }

        [Display(Name = "Installment")]
        public int NumberOfInstallment { get; set; }
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "Application Date")]
        public string ApplicationDate { get; set; }
        [Display(Name = "Approve Date")]
        public string ApprovedDate { get; set; }
        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }        
        [Display(Name = "End Date")]
        public string EndDate { get; set; }
		[Display(Name = "Hold")]
        public bool IsHold { get; set; }
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

        public string Code { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Loan Type")]
        public string LoanType { get; set; }    
        [Display(Name = "Principal Amount")]
        public decimal PrincipalAmount { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        [Display(Name = "Settelment Date")]
        public string EarlySellteDate { get; set; }       

        public string Project { get; set; }
        public string Section { get; set; }


       [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]

        public List<EmployeeLoanDetailVM> employeeLoanDetails { get; set; }


        [Display(Name = "Interest Policy")]
        public string InterestPolicy { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "Rate (%)")]
        public decimal InterestRate { get; set; }
        [Display(Name = "Rate1 (%)")]
        public decimal InterestRate1 { get; set; }
        [Display(Name = "Interest Amount")]
        public decimal InterestAmount { get; set; }
      
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public string BranchId { get; set; }

       // Only for display
        public string Employee { get; set; }
               
	    public int FiscalYearDetailId { get; set; }
               
		[Display(Name = "Refund Amount")]
        public decimal RefundAmount { get; set; }        
		[Display(Name = "Refund Date")]
        public string RefundDate { get; set; }
       
        public bool FromSetting { get; set; }

       [Display(Name = "Principal Amount Due")]
        public decimal TotalDuePrincipalAmount { get; set; }
       [Display(Name = "Interest Amount Due")]
        public decimal TotalDueInterestAmount { get; set; }
       [Display(Name = "Settlement Amount")]
        public decimal SettlementAmount { get; set; }

        public decimal NoofInstallment { get; set; }

        public bool IsEarlySellte { get; set; }
    }
}
