using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace SymViewModel.Payroll
{
    public class GradeVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
        public string Name { get; set; }

        public string Area { get; set; }
        [Display(Name = "Grade No")]
        public int GradeNo { get; set; }
        [Display(Name = "Current Basic")]
        public decimal CurrentBasic { get; set; }
        [Display(Name = "Basic Next Year Factor(%)")]
        public decimal BasicNextYearFactor { get; set; }
        [Display(Name = "Basic Next Step Factor")]
        public decimal BasicNextStepFactor { get; set; }
        [Display(Name = "House Rent Factor Basic")]
        public bool IsHouseRentFactorFromBasic { get; set; }
        [Display(Name = "House Rent Factor")]
        public decimal HouseRentFactor { get; set; }
        [Display(Name = "TA Factor Basic")]
        public bool IsTAFactorFromBasic { get; set; }
        [Display(Name = " Travelling Allowance Factor")]
        public decimal TAFactor { get; set; }
        [Display(Name = "Medical Factor Basic")]
        public bool IsMedicalFactorFromBasic { get; set; }
        [Display(Name = "Medical Factor")]
        public decimal MedicalFactor { get; set; }

        [Display(Name = "Min Salary")]
        public decimal MinSalary { get; set; }
        [Display(Name = "Max Salary")]
        public decimal MaxSalary { get; set; }
        [Display(Name = "Lower Limit")]
        public decimal LowerLimit { get; set; }
        [Display(Name = "Median Limit")]
        public decimal MedianLimit { get; set; }
        [Display(Name = "Upper Limit")]
        public decimal UpperLimit { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixedHouseRent { get; set; }
        [Display(Name = "House Rent Allowance")]
        public decimal HouseRentAllowance { get; set; }
        public bool IsFixedSpecialAllowance { get; set; }
         [Display(Name = "Special Allowance")]
        public decimal SpecialAllowance { get; set; }
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
        public HttpPostedFileBase File { get; set; }
        public int SL { get; set; }
    }
}
