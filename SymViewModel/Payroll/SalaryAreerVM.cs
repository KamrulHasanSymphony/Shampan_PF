using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
   public class SalaryAreerVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        public int FiscalYearDetailId { get; set; }
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

        #region
        public string Project { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmpName { get; set; }
        public string Section { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal AreerAmount { get; set; }

        public string PeriodName { get; set; }
        public string JoinDate { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string ProjectId { get; set; }
        public string SectionId { get; set; }
        public string FiscalYearId { get; set; }
        public string Code { get; set; }
        #endregion



     
    }
}
