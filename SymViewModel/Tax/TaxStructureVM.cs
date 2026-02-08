using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
   public class TaxStructureVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TXSetupId { get; set; }
        [Display(Name = "TAX Setup")] 
		public string TXSetup { get; set; }
        [Display(Name = "Salary Type Name")] 
        public string SalaryTypeId { get; set; }
        [Display(Name = "% Of Basic")] 
        public string PercentOfBasic { get; set; }
        [Display(Name = "Fixed Exampted")] 
        public string FixedExampted { get; set; }
        [Display(Name = "Tax Value")]
        public decimal TaxValue { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
       
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
    }
}
