using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class EmployeeOtherEarningVM
    {
        #region Properties
        public string Id { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal EarningAmount { get; set; }

        public int FiscalYearDetailId { get; set; }

        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "Date")]
        public string EarningDate { get; set; }
        [Display(Name = "Earning Type")]
        public string EarningType { get; set; }
        public int EarningTypeId { get; set; }
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
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public string Section { get; set; }
        public string Project { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string ProjectId { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
        [Display (Name="Normal OT (Hours)")]
        public decimal OTHrs { get; set; }
        [Display(Name = "Special OT (Hours)")]
        public decimal OTHrsSpecial { get; set; }
        #endregion Properties
        [Display(Name = "Fiscal Period To")]
        public object fidTo { get; set; }
        [Display(Name = "Code From")]
        public string CodeF { get; set; }
        [Display(Name = "Code To")]
        public string CodeT { get; set; }
        [Display(Name = "Period Start")]
        public string PeriodStart { get; set; }
        [Display(Name = "Order By")]
        public object Orderby { get; set; }

        public string SalaryMonthId { get; set; }
    }
}
