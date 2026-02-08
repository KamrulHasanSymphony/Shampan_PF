using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF

{
    public class EmployeePFVM
    {

        public int Id { get; set; }
        public string PFStructureId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "BWWF Value")]
        public decimal PFValue { get; set; }
        [Display(Name = "Fixed")]
        public bool IsFixed { get; set; }
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

        #region For Display
        [Display(Name = "BWWF Name")] 
        public string PFName { get; set; }
        [Display(Name = "Portion Salary Type")] 
        public string PortionSalaryType { get; set; }
        #endregion For Display

        public int FiscalYearDetailId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }




        public string Code { get; set; }
        public string TransType { get; set; }
    }
}
