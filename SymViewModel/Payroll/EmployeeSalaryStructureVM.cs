using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace SymViewModel.Payroll
{
    public class EmployeeSalaryStructureVM
    {
        public EmployeeSalaryStructureVM()
        {
            EmployeeSalaryStructureVMs = new List<EmployeeSalaryStructureVM>();
        }

        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string SalaryStructureId { get; set; }
        [Display(Name = "Salary Structure")]
        public string SalaryStructureName { get; set; }
        [Display(Name = "Salary Type")]
        public string SalaryTypeId { get; set; }
        [Display(Name = "Salary Type")]
        public string SalaryType { get; set; }
        public decimal Portion { get; set; }
        public decimal EmpTaxValue { get; set; }
        [Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
        [Display(Name = "Salary Type Name")]
        public string SalaryTypeName { get; set; }
        [Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }
        [Display(Name = "Earning / Deduction")]
        public bool IsEarning { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalValue { get; set; }
        [Display(Name = "Increment Date")]
        public string IncrementDate { get; set; }
        public string BranchId { get; set; }
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

        [Display(Name = "Increment Amount")]
        public decimal IncrementValue { get; set; }

        [Display(Name = "After Increment Amount")]
        public decimal AfterIncrementValue { get; set; }


        [Display(Name = "Current")]
        public bool IsCurrent { get; set; }


        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Employee code")]
        public string EmpCode { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Project { get; set; }


        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string ProjectId { get; set; }

        [Display(Name = "Code From")]
        public string CodeFrom { get; set; }
        [Display(Name = "Code To")]
        public string CodeTo { get; set; }

        public bool IsGross { get; set; }
        public bool IsBasic { get; set; }
        public bool IsRate { get; set; }
        [Display(Name = "Current Year")]
        public string CurrentYear { get; set; }
        public string CurrentYearPart { get; set; }
        public HttpPostedFileBase file { get; set; }


        public string StepSL { get; set; }

        public string GradeId { get; set; }
        public List<string> IDs { get; set; }


        public List<EmployeeSalaryStructureVM> EmployeeSalaryStructureVMs { get; set; }

    }
}
