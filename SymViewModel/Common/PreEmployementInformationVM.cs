using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class PreEmployementInformationVM
    {
        public int Id { get; set; }

        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; }
        [Display(Name = "Issue Date")]
        public string IssueDate { get; set; }
        [Display(Name = "Salutation")]
        public string Salutation { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Job Grade")]
        public string JobGrade { get; set; }
        [Display(Name = "Job Grade Designation")]
        public string JobGradeDesignation { get; set; }
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        [Display(Name = "House Rent Allowance")]
        public decimal HouseRentAllowance { get; set; }
        [Display(Name = "Medical Allowance")]
        public decimal MedicalAllowance { get; set; }
        [Display(Name = "Conveyance Allowance")]
        public decimal ConveyanceAllowance { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }


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

        public string Operation { get; set; }

        public string Code { get; set; }
    }
}
