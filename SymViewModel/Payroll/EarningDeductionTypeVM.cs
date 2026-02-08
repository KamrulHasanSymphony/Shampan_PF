using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class EarningDeductionTypeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Earning / Deduction")]
        public bool IsEarning { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }        
		[Display(Name = "GL Account Code")]
        public string GLAccountCode { get; set; }
    }
}
