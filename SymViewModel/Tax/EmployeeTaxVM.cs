using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
   public class EmployeeTaxVM
    {
        public string Id { get; set; }
        public string TaxStructureId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxValue { get; set; }
        [Display(Name = "Fixed")]
        public bool IsFixed { get; set; }
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

        #region For Display        
		[Display(Name = "Tax Name")]
        public string TaxName { get; set; }        
		[Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
        #endregion For Display




        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Project { get; set; }
        public string Section { get; set; }
        [Display(Name = "Employee Name")] 
		public string EmpName { get; set; }
        public string Code { get; set; }



    }
}
