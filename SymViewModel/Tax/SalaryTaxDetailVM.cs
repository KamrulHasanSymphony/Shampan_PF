using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class SalaryTaxDetailVM
    {
        public int Id { get; set; }
        public string SalaryTaxId { get; set; }
        public string FiscalYearDetailId { get; set; }
        public string TaxStructureId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string SectionId { get; set; }
        public string EmployeeId { get; set; }        
		[Display(Name = "Tax Amount")]
        public decimal TaxValue { get; set; }        
		[Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }        
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

        #region Only For display        
		[Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }        
		[Display(Name = "Fiscal Period")]
        public string FiscalPeriod { get; set; }
        #endregion

                
		[Display(Name = "Code From")]
        public object CodeF { get; set; }
                
		[Display(Name = "Code To")]
        public object CodeT { get; set; }
                
		[Display(Name = "Fiscal Period To")]
        public object fidTo { get; set; }
                
		[Display(Name = "Active")]
        public string PeriodName { get; set; }        
		[Display(Name = "Active")]
        public string PeriodStart { get; set; }

        public string Department      { get; set; }
        public string Designation     { get; set; }
        public string Project         { get; set; }
        public string Section         { get; set; }        
        [Display(Name = "Employee Name")]
        public string EmpName         { get; set; }
        public string Code            { get; set; }        
        [Display(Name = "Join Date")]
        public string JoinDate   { get; set; }
                
        [Display(Name = "Order By")]
        public string Orderby { get; set; }

    }
}
