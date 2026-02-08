using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
    public class SalaryStructureDetailVM
    {
        public int Id { get; set; }
        [Display(Name = "Salary Head")]
        public string SalaryTypeId { get; set; }
        [Display(Name = "From")]
        public string PortionSalaryType { get; set; }
        public string SalaryType { get; set; }

        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "Amount / % of Basic")]
        public decimal Portion { get; set; }        
		[Display(Name = "Gross")]
        public bool IsGross { get; set; }

        public string EmployeeId { get; set; }
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
}



