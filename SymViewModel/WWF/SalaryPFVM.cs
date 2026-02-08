using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
   public class SalaryPFVM
    {

        public string Id { get; set; }
        public int BranchId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Active")] 
        public bool   IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        #region Display For
        [Display(Name = "Active")] 
        public string PeriodName { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Employee Name")] 
        public string EmpName { get; set; }
        public string Section { get; set; }
        [Display(Name = "Basic Salary")] 
        public decimal BasicSalary { get; set; }
        [Display(Name = "Gross Salary")] 
        public decimal GrossSalary { get; set; }
        [Display(Name = "Join Date")] 
        public string JoinDate { get; set; }
        [Display(Name = "WWF Amount")] 
        public decimal PFValue { get; set; }
        public string DepartmentId { get; set; }

        public string DesignationId { get; set; }

        public string ProjectId { get; set; }

        public string SectionId { get; set; }

        public string FiscalYearId { get; set; }

        public string Code { get; set; }

        #endregion





        [Display(Name = "Period Start")]
        public string PeriodStart { get; set; }
        public string TransType { get; set; }
    }
}
