using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class PFDetailVM
    {
        public string MyProperty { get; set; }


        public int Id { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string PFHeaderId { get; set; }
        public string PFStructureId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Employee PF Value")] public decimal EmployeePFValue { get; set; }
        [Display(Name = "Employeer PF Value")] public decimal EmployeerPFValue { get; set; }
        [Display(Name = "Basic Salary")] public decimal BasicSalary { get; set; }
        [Display(Name = "Gross Salary")] public decimal GrossSalary { get; set; }
        public bool Post { get; set; }

        public bool IsBankDeposited { get; set; }


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
        public decimal TotalEmployeeValue { get; set; }
        public decimal TotalEmployerValue { get; set; }

        public decimal TotalPF { get; set; }

        public string Operation { get; set; }


        [Display(Name = "Fiscal Period")] public string FiscalPeriod { get; set; }
        [Display(Name = "Period Start")] public string PeriodStart { get; set; }
        [Display(Name = "Period End")] public string PeriodEnd { get; set; }
        [Display(Name = "Employee Name")] public string EmpName { get; set; }

        public string Code { get; set; }
        public string PFHeaderCode { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        [Display(Name = "Join Date")] public string JoinDate { get; set; }

        public bool IsDistribute { get; set; }

        public decimal TotalEmployeeContribution { get; set; }

        public decimal TotalEmployerContribution { get; set; }

        public decimal EmployeeTotalContribution { get; set; }

        public decimal EmployerTotalContribution { get; set; }

        public string DateOfPermanent { get; set; }

        public string PFStartDate { get; set; }

        public string LeftDate { get; set; }

        public string PFEndDate { get; set; }

        public string PFEndPeriodName { get; set; }

        public string PeriodName { get; set; }

        public string ProjectName { get; set; }
        public string TransType { get; set; }

        public decimal EmployeeProfit { get; set; }
        public decimal EmployerProfit { get; set; }
    }


    public class PFHeaderVM
    {


        public int Id { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string Code { get; set; }
        public string ProjectName { get; set; }
        public string DistributionDate { get; set; }
        [Display(Name = "Fiscal Period")]
        public string FiscalPeriod { get; set; }
        public decimal TotalPF { get; set; }
        [Display(Name = "Employee PF Value")]
        public decimal EmployeePFValue { get; set; }
        [Display(Name = "Employeer PF Value")]
        public decimal EmployeerPFValue { get; set; }
        public decimal TotalEmployeeValue { get; set; }
        [Display(Name = "Basic Salary")]
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
       
        public decimal TotalEmployerValue { get; set; }
        public string Operation { get; set; }
        public string ProjectId { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        public string TransType { get; set; }

        public decimal DistributedValue { get; set; }

        public decimal BWWFValue { get; set; }
    }
}
