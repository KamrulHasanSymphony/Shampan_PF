using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class EmployeeLeftInformationVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Display(Name = "Left Type")]
        public string LeftType_E { get; set; }

        [Display(Name = "Left/Last Working Date")]
        public string LeftDate { get; set; }
        [Required]
        [Display(Name = "Issue/Resign Date")]
        public string EntryLeftDate { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }

        public string FileName { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Project { get; set; }
        public string JoinDate { get; set; }
        [Display(Name = "Salary Process")]

        public bool IsSalalryProcess { get; set; }
    }
}
