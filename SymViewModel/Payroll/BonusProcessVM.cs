using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace SymViewModel.Payroll
{
    public class BonusProcessVM
    {
        #region Properties
        public string Id { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }
        [Display(Name = "Bonus Amount")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }
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
        public string BonusNameId { get; set; }
        public string BonusStructureId { get; set; }
        [Display(Name = "Bonus Type")]
        public string BonusType { get; set; }
        [Display(Name = "Bonus Structure")]
        public string BonusStructureName { get; set; }
        [Display(Name = "Total Employee")]
        public string TotalEmployee { get; set; }
        #endregion Properties
        [Display(Name = "Code From")]
        public string CodeF { get; set; }
        [Display(Name = "Code To")]
        public string CodeT { get; set; }
        [Display(Name = "Order By")]
        public string Orderby { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }

        public string SectionOrder { get; set; }
        public string FestivalDate { get; set; }
        public string IssueDate { get; set; }


        public string SheetName { get; set; }


        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string Other3 { get; set; }

        public string MultipleOther3 { get; set; }

        public string MultiOther3 { get; set; }

        public List<string> Other3List { get; set; }



        public int FiscalYearDetailId { get; set; }
        public int FiscalYear { get; set; }

        public string FiscalPeriod { get; set; }

        public string PeriodStart { get; set; }

        public string PeriodEnd { get; set; }
        public decimal TaxValue { get; set; }
        public decimal NetPayAmount { get; set; }
        public decimal NetAmount { get; set; }


        
        public decimal HouseRent { get; set; }
        public decimal Medical { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal Stamp { get; set; }


        [Display(Name = "Effect Date")]
        public string EffectDate { get; set; }



        public bool IsManual { get; set; }

        public HttpPostedFileBase File { get; set; }




        public string BonusStatus { get; set; }


        public string Grade { get; set; }
        public string StepName { get; set; }

        public string PaymentDay { get; set; }
        public string BankAccountNo{ get; set; }
        public string Routing_No { get; set; }
        public string Email { get; set; }
        public string BounsName { get; set; }

    }
}
