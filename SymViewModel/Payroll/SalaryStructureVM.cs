using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
   public class SalaryStructureVM
   {
       [Required]
       public string Id { get; set; }
       [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
       public string Code { get; set; }
       public string Name { get; set; }
       public int BranchId { get; set; }

       public List<SalaryStructureDetailVM> salaryStructureDetailVMs { get; set; }
       public List<SalaryStructureDetailVM> salaryStructureDeductionDetailVMs { get; set; }
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
    }

   public class SingleEmployeeSalaryStructureVM
   {
       public string EmployeeId { get; set; }
       public int FiscalYearDetailId { get; set; }
       public string Name { get; set; }
       public decimal Value { get; set; }
       [Display(Name = "Earning")]
       public bool IsEarning { get; set; }
       [Display(Name = "Editable")]
       public bool IsEditable { get; set; }
       [StringLength(50, ErrorMessage = "Remarks cannot be longer than 50 characters.")]
       public string Remarks { get; set; }
   }
}
