using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class BonusStructureVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
        public string Name { get; set; }
        [Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        public string Religion { get; set; }
        [Display(Name = "Bonus Amount")]
        public decimal BonusValue { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "DOJ From")]
        public string DOJFrom { get; set; }
        [Display(Name = "DOJ To")]
        public string DOJTo { get; set; }
        [Display(Name = "Job Age From")]
        public int JobAge { get; set; }

        [Display(Name = "Job Age To")]
        public int JobAgeTo { get; set; }


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
