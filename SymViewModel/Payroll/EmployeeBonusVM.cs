using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class EmployeeBonusVM
    {
        public string Id { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Name { get; set; }
        public int BranchId { get; set; }
        public string BonusStructureId { get; set; }
        [Display(Name = "Bonus Structure")]
        public string BonusStructureName { get; set; }        
		[Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }        
		[Display(Name = "Bonus Amount")]
        public decimal BonusValue { get; set; }        
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
        public List<EmployeeBonusDetailVM> employeeBonusDetailVM { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
    }
}
