using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class EmployeeSalaryStructureDetailVM
    {
        public int Id { get; set; }
        public string EmployeeSalaryStructureId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Salary Type")]
        public string SalaryTypeId { get; set; }
        [Display(Name = "Salary Type")]
        public string SalaryType { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        public decimal Portion { get; set; }
        [Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
        public decimal Amount { get; set; }
        [Display(Name = "Earning / Deduction")]
        public bool IsEarning { get; set; }
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

        [Display(Name = "Increment Date")]
        public string IncrementDate { get; set; }

        [Display(Name = "Current")]
        public bool IsCurrent { get; set; }
    }
}
