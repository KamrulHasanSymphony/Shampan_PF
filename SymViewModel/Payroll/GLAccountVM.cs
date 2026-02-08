using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class GLAccountVM
    {
        public int Id { get; set; }
        [Display(Name = "Project")]
        public string Project { get; set; }
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [Required(ErrorMessage = "GL Account Code is required")]
        [Display(Name = "GL Account Code")]
        public string GLAccountCode { get; set; }
        public string Description { get; set; }
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
		[Display(Name = "Outstanding Liabilities")]
        public bool OutstandingLiabilities { get; set; }
        public string VoucherType { get; set; }
        public string GLAccountType { get; set; }
    }
}
