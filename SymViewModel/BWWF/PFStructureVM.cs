using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF

{
   public class PFStructureVM
    {

        public string Id { get; set; }
        public int BranchId { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
        public string Name { get; set; }
        [Display(Name = "BWWF Amount")]
        public decimal PFValue { get; set; }
        [Display(Name = "Fixed")]
        public bool IsFixed { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "From")]
        public string PortionSalaryType { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }       
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public string TransType { get; set; }

    }
}
