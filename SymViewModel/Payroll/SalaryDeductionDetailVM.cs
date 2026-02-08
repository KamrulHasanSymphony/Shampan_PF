using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
    public class SalaryDeductionDetailVM
    {
        public int Id { get; set; }
        public string SalaryDeductionId { get; set; }
        public string EmployeeId { get; set; }
        public string FiscalYearDetailId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }

        public decimal DeductionAmount { get; set; }
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

        public string EmployeeName { get; set; }
        public string FiscalPeriod { get; set; }
    }
}
