using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class PFSettlementVM
    {
        public int Id { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionType { get; set; }

        
        
        
        public bool Post { get; set; }
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


        [Display(Name = "Fiscal Period")] public int FiscalYearDetailId { get; set; }
        [Display(Name = "Project")] public string ProjectId { get; set; }
        [Display(Name = "Department")] public string DepartmentId { get; set; }
        [Display(Name = "Section")] public string SectionId { get; set; }
        [Display(Name = "Designation")] public string DesignationId { get; set; }
        [Display(Name = "Employee")] public string EmployeeId { get; set; }
        [Display(Name = "Employee Profit Value")] public decimal EmployeeProfitValue { get; set; }
        [Display(Name = "Employer Profit Value")] public decimal EmployerProfitValue { get; set; }
        [Display(Name = "Employee Total Contribution")] public decimal EmployeeTotalContribution { get; set; }
        [Display(Name = "Employer TotalContribution")] public decimal EmployerTotalContribution { get; set; }
        [Display(Name = "Employee Date of Join")] public string EmpDOJ { get; set; }
        [Display(Name = "Employee Resign Date")] public string EmpResignDate { get; set; }
        [Display(Name = "Settlement Date")] public string SettlementDate { get; set; }
        [Display(Name = "Settlemen tPolicy")] public int SettlementPolicyId { get; set; }
        [Display(Name = "Job Age in Month")] public decimal JobAgeInMonth { get; set; }
        [Display(Name = "Employee Contribution Ratio")] public decimal EmployeeContributionRatio { get; set; }
        [Display(Name = "Employer Contribution Ratio")] public decimal EmployerContributionRatio { get; set; }


        [Display(Name = "Employee Profit Ratio")] public decimal EmployeeProfitRatio { get; set; }
        [Display(Name = "Employer Profit Ratio")] public decimal EmployerProfitRatio { get; set; }



         [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        public string Code { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }




        public string Operation { get; set; }

        [Display(Name = "Employee Actual Contribution")] public decimal EmployeeActualProfitValue { get; set; }
        [Display(Name = "Employer Actual Contribution")] public decimal EmployeeActualContribution { get; set; }
        [Display(Name = "Employee Actual Profit Value")] public decimal EmployerActualProfitValue { get; set; }
        [Display(Name = "Employer Actual Profit Value")] public decimal EmployerActualContribution { get; set; }


public decimal EmployeeContributionForfeitValue   { get; set; }
public decimal EmployeeProfitForfeitValue         { get; set; }
public decimal EmployerContributionForfeitValue  { get; set; }
public decimal EmployerProfitForfeitValue        { get; set; }
public decimal TotalForfeitValue                 { get; set; }


        //////public int PFContributionMonth { get; set; }

        public decimal TotalPayableAmount { get; set; }

        public decimal AlreadyPaidAmount { get; set; }

        public decimal NetPayAmount { get; set; }

        public string PFStartDate { get; set; }

        public string PFEndDate { get; set; }

        public List<PFSettlementDetailVM> detailVMs { get; set; }


        public decimal ProvidentFundAmount { get; set; }
        public string TransType { get; set; }

        public string BranchId { get; set; }
    }
}
