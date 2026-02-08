using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SymViewModel.HRM
{
    public class EmployeeJobVM
    {

        public int Id { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        [Display(Name = "Probation Month")]
        public int ProbationMonth { get; set; }

        [Display(Name = "Extention Probation Month")]
        public int ExtendedProbationMonth { get; set; }

        [Display(Name = "Probation End")]
        public string ProbationEnd { get; set; }

        //public string Rank { get; set; }
        [Display(Name = "Force")]
        public string Force { get; set; }
        [Display(Name = "Rank")]
        public string Rank { get; set; }
        [Display(Name = "Duration")]
        public string Duration { get; set; }
        [Display(Name = "Retirement")]
        public string Retirement { get; set; }
        [Display(Name = "Confirm Date")]
        public string DateOfPermanent { get; set; }
        [Display(Name = "Employment Status")]
        public string EmploymentStatus_E { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType_E { get; set; }
        [Display(Name = "Supervisor/Line Manager")]
        public string Supervisor { get; set; }

        [Display(Name = "Retirement Date")]
        public string RetirementDate { get; set; }
        [Display(Name = "1st Extention Date")]
        public string fristExDate { get; set; }
        [Display(Name = "2nd Extention Date")]
        public string secondExDate { get; set; }
        [Display(Name = "Extension Y/N")]
        public string Extentionyn { get; set; }

        [Display(Name = "Contract Extention Date")]
        public string ContrExDate { get; set; }

        [Display(Name = "Confirmed")]
        public bool IsPermanent { get; set; }
        [Display(Name = "IsBuild")]
        public bool IsBuild { get; set; }

        [Display(Name = "PF Applicable")]
        public bool IsPFApplicable { get; set; }

        [Display(Name = "GF Applicable")]
        public bool IsGFApplicable { get; set; }

        [Display(Name = "Inactive/Active")]
        public bool IsInactive{ get; set; }

        [Display(Name = "From Date")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        public string ToDate { get; set; }

        [Display(Name = "Structure Group")]
        public string StructureGroupId { get; set; }
         [Display(Name = "GF Start From")]
        public string GFStartFrom { get; set; }

        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }

        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Doted Line Report")]
        public string DotedLineReport { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public bool IsGross { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        #region DesignationAndDepartment
        [Display(Name = "Designation")]
        public string DesignationId { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [Display(Name = "Section")]
        public string SectionId { get; set; }
        #endregion

        [Display(Name = "Bank Information")]
        public string BankInfo { get; set; }
        [Display(Name = "Bank Account No.")]
        public string BankAccountNo { get; set; }

        [Display(Name = "Grade")]
        public string GradeName { get; set; }

        public string Designation { get; set; }

        public string Project { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }
        [Display(Name = "Employment Status Name")]
        public string EmploymentStatus_EName { get; set; }
        [Display(Name = "Employment Type Name")]

        public string EmploymentType_EName { get; set; }

        [Display(Name = "Job Location/Zone")]
        public string Other1 { get; set; }
        [Display(Name = "Area")]
        public string Other2 { get; set; }
        [Display(Name = "Division")]
        public string Other3 { get; set; }
        [Display(Name = "Job Location")]
        public string Other4 { get; set; }
        public string Other5 { get; set; }

        [Display(Name = "Job Before")]
        public bool IsJobBefore { get; set; }        
        
        [Display(Name = "TAX Applicable")]
        public bool IsTAXApplicable { get; set; }

        [Display(Name = "FG Applicable")]
        public bool IsFGApplicable { get; set; }

        [Display(Name = "Rebate Applicable")]
        public bool IsRebate { get; set; }

        [Display(Name = "Profit Not Applicable")]
        public bool IsNoProfit { get; set; }
        
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }


        public string LabelOther1 { get; set; }
        public string LabelOther2 { get; set; }
        public string LabelOther3 { get; set; }
        public string LabelOther4 { get; set; }
        public string LabelOther5 { get; set; }


        public string BankName { get; set; }


        [Display(Name = "First Holiday")]
        public string FirstHoliday { get; set; }
         [Display(Name = "Second Holiday")]
        public string SecondHoliday { get; set; }

        [Display(Name = "Bank Account Name")]
        public string BankAccountName { get; set; }
        [Display(Name = "Routing No")]
        public string Routing_No { get; set; }

        [Display(Name = "Employee Job Type")]
        public string EmpJobType { get; set; }
        [Display(Name = "Employee Category")]
        public string EmpCategory { get; set; }

        



    }
}
