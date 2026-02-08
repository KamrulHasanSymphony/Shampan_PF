using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{

    public class AdminInfoDashboardVM
    {
        public string Present { get; set; }
        public string Absent { get; set; }
        public string Late { get; set; }
        [Display(Name = "In Miss")]
        public string InMiss { get; set; }
      
        public HrmInfoDashboardVM HrmInfoDashboardVMS { get; set; }
        public PayrollInfoDashboardVM PayrollInfoDashboardVMS { get; set; }
        public TaxInfoDashboardVM TaxInfoDashboardVMS { get; set; }
        public PfInfoDashboardVM PfInfoDashboardVMS { get; set; }
        public GfInfoDashboardVM GfInfoDashboardVMS { get; set; }

        public BranchVM BranchVM { get; set; }
    }
    public class HrmInfoDashboardVM
    {
        [Display(Name = "Total Employee")]
        public decimal TotalEmployee { get; set; }
        [Display(Name = "Active Employee")]
        public decimal ActiveEmployee { get; set; }
        [Display(Name = "Inactive Employee")]
        public decimal InactiveEmployee { get; set; }
        public decimal Male { get; set; }
        public decimal Female { get; set; }
        [Display(Name = "Not Applicable")]
        public decimal NotApplicable { get; set; }   
    }
    public class PayrollInfoDashboardVM
    {

        [Display(Name = "Total Person")]
        public decimal TotalPerson { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }      
    }
    public class TaxInfoDashboardVM
    {
        [Display(Name = "Total Person")]
        public decimal TotalPerson { get; set; }
        [Display(Name = "Tax Value")]
        public decimal TaxValue { get; set; }       
    }
    public class PfInfoDashboardVM
    {
        [Display(Name = "Total Person")]
        public decimal TotalPerson { get; set; }
        [Display(Name = "PF Value")]
        public decimal PFValue { get; set; }
    }
    public class GfInfoDashboardVM
    {
        [Display(Name = "Gf Applicable")]
        public decimal TotalPerson { get; set; }
         [Display(Name = "Gf Amount")]
        public decimal GFValue { get; set; }
         [Display(Name = "Monthly Gf Account")]
         public decimal MonthlyGFValue { get; set; }
    }
}
