using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class StructureGroupVM
    {
        public string Id { get; set; }

        public int BranchId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Leave Structure")]
        public string LeaveStructure { get; set; }
        [Display(Name = "Leave Structure")]
        public string LeaveStructureId { get; set; }

        [Display(Name = "Salary Structure")]
        public string SalaryStructure { get; set; }
        [Display(Name = "Salary Structure")]
        public string SalaryStructureId { get; set; }

        [Display(Name = "PF Structure")]
        public string PFStructure { get; set; }
        [Display(Name = "PF Structure")]
        public string PFStructureId { get; set; }

        [Display(Name = "Employee Group Structure")]
        public string EmployeeGroupId { get; set; }
        [Display(Name = "Employee Group Structure")]
        public string EmployeeGroup { get; set; }

        [Display(Name = "Bonus Structure")]
        public string BonusStructure { get; set; }
        [Display(Name = "Bonus Structure")]
        public string BonusStructureId { get; set; }

        [Display(Name = "Tax Structure")]
        public string TaxStructure { get; set; }
        [Display(Name = "Tax Structure")]
        public string TaxStructureId { get; set; }

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
