using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
  public  class TaxSetupVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
       [Display(Name = "Salary Type")]
        public string SalaryTypeId { get; set; }
       [Display(Name = "Salary Type")]
       public string SalaryTypeName { get; set; }
       [Display(Name = "% Of Basic")]
        public decimal PercentOfBasic { get; set; }
        [Display(Name = "Fixed Exampted")]
        public decimal FixedExampted { get; set; }
        [Display(Name = "Minimum Tax")]
        public decimal MinimumTax { get; set; }
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


        public string Name { get; set; }

        public string Code { get; set; }
    }
}
