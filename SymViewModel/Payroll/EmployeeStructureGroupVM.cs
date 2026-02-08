using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace SymViewModel.Payroll
{
    public class EmployeeStructureGroupVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
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
        public string EmployeeGroupId { get; set; }
        public string LeaveStructureId { get; set; }
        public int EarningDeductionStructureId { get; set; }

        public string SalaryStructureId { get; set; }
        public string PFStructureId { get; set; }
        public string TaxStructureId { get; set; }
        public string BonusStructureId { get; set; }
        public string ProjectAllocationId { get; set; }
        public decimal basic { get; set; }
        public decimal gross { get; set; } 
        
		[Display(Name = "Active")]
        public decimal salaryInput { get; set; }


        public decimal Housing { get; set; }
        public decimal TA { get; set; }
        public decimal Medical { get; set; }

        public decimal ChildAllowance { get; set; }
        public decimal HardshipAllowance { get; set; }
        public decimal Overtime { get; set; }
        public decimal LeaveEncashment { get; set; }
        public decimal FestivalAllowance { get; set; }
   
		[Display(Name = "Grade")]
        public string GradeId { get; set; }
		[Display(Name = "Step")]
        public string StepId { get; set; }
		[Display(Name = "Gross")]
        public bool IsGross { get; set; }
        public int year { get; set; }
        public decimal BankPayAmount { get; set; }

        public decimal TaxPortion { get; set; }
        public decimal EmpTaxValue { get; set; }

        public string SalaryTypeId { get; set; }

        public HttpPostedFileBase File { get; set; }

        public decimal Amount { get; set; }

        public decimal BonusTaxPortion { get; set; }
        public decimal EmpBonusTaxValue { get; set; }
        public decimal BasicPercentage { get; set; }

        public decimal FixedOT { get; set; }
        public bool IsGFApplicable { get; set; }
        public decimal TravelAllowance { get; set; }

    }
}
