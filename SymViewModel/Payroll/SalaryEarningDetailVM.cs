using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class SalaryEarningDetailVM
    {
        public int Id { get; set; }
        public string SalaryEarningId { get; set; }        
		[Display(Name = "Salary Type")]
        public string SalaryTypeId { get; set; }        
		[Display(Name = "Salary Type Name")]
        public string SalaryTypeName { get; set; }        
		[Display(Name = "SalaryName")]
        public string SalaryName { get; set; }
        public int FiscalYearDetailId { get; set; }        
		[Display(Name = "Salary Type")]
        public string SalaryType { get; set; }
        public string EmployeeId { get; set; }        
		[Display(Name = "Fixed / Rate")]
        public bool IsFixed { get; set; }        
        public decimal Portion { get; set; }        
		[Display(Name = "Portion Salary Type")]
        public string PortionSalaryType { get; set; }
        public decimal Amount { get; set; }
        public string EmployeeSalaryStructureId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
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
        public string EmpName { get; set; }        
		[Display(Name = "Period Name")]
        public string PeriodName { get; set; }        
		[Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }        
		[Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public string DesignationId { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Section { get; set; }        
		[Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        #endregion        
		[Display(Name = "Code From")]
        public string CodeF { get; set; }        
		[Display(Name = "Code To")]
        public object CodeT { get; set; }        
		[Display(Name = "Fiscal Period To")]
        public object fidTo { get; set; }        
		[Display(Name = "Period Start")]
        public string PeriodStart { get; set; }        
		[Display(Name = "Order By")]
        public object Orderby { get; set; }
    }
}