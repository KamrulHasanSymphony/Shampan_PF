using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class EmployeeEarningLeaveVM
    {

        public string Id { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal DeductionAmount { get; set; }
        public decimal NetDeductionAmount { get; set; }
        public int FiscalYearDetailId { get; set; }
        public int SalaryPreiodId { get; set; }
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "Date")]
        public string DeductionDate { get; set; }
        [Display(Name = "Remarks")]
        //public string DeductionType { get; set; }
        //public int DeductionTypeId { get; set; }
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
        public decimal Medical { get; set; }
        public decimal HouseRent { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal Stamp { get; set; }

        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string ProjectId { get; set; }
        [Display(Name = "Code From")]
        public object CodeF { get; set; }
        [Display(Name = "Code To")]
        public object CodeT { get; set; }
        [Display(Name = "Fiscal Period To")]
        public object fidTo { get; set; }
        [Display(Name = "Period Start")]
        public string PeriodStart { get; set; }
        [Display(Name = "Order By")]
        public object Orderby { get; set; }

        public decimal Days { get; set; }
        public bool IsPunishmentFromBasic { get; set; }

        public string PunishmentFromBasic { get; set; }
        public int DaysOfMonth { get; set; }
        public string SectionOrder { get; set; }
        public string StepName { get; set; }
        public string Grade { get; set; }


        public void CalculateEL()
        {
            try
            {
                DeductionAmount = (BasicSalary / 30) * Days;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string BankAccountNo { get; set; }

        public string Routing_No { get; set; }

        public string Email { get; set; }

        public string SalaryPeriod { get; set; }

        public string SalaryPeriodName { get; set; }
    }
}
