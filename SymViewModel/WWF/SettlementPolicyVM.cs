using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
    public class SettlementPolicyVM
    {
        public int Id { get; set; }

        [Display(Name = "Policy Name")]
        public string PolicyName { get; set; }
        [Display(Name = "Job Age (Month)")]
        public decimal JobAgeInMonth { get; set; }
        [Display(Name = "Employee Contribution Ratio")]
        public decimal EmployeeContributionRatio { get; set; }
        [Display(Name = "Employer Contribution Ratio")]
        public decimal EmployerContributionRatio { get; set; }


        [Display(Name = "Employee Profit Ratio")]public decimal EmployeeProfitRatio { get; set; }
        [Display(Name = "Employer Profit Ratio")]public decimal EmployerProfitRatio { get; set; }


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

        public string Operation { get; set; }
        public string TransType { get; set; }
    }
}
