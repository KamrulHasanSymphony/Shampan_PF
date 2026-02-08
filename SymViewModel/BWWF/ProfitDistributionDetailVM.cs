using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class ProfitDistributionDetailVM
    {
        public int Id { get; set; }
        
        
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

        public string Operation { get; set; }



        [Display(Name = "Fiscal Period")] public int FiscalYearDetailId { get; set; }

        [Display(Name = "ProfitDistributionId")] public int ProfitDistributionId { get; set; }
        [Display(Name = "Project")] public string ProjectId { get; set; }
        [Display(Name = "Department")] public string DepartmentId { get; set; }
        [Display(Name = "Section")] public string SectionId { get; set; }
        [Display(Name = "Designation")] public string DesignationId { get; set; }
        [Display(Name = "Employee")] public string EmployeeId { get; set; }
        [Display(Name = "Employee ProfitV alue")] public decimal EmployeeProfitValue { get; set; }
        [Display(Name = "Employer Profit Value")] public decimal EmployerProfitValue { get; set; }



        [Display(Name = "Total Employee Contribution")] public decimal EmployeeTotalContribution { get; set; }
        [Display(Name = "Total Employer Contribution")] public decimal EmployerTotalContribution { get; set; }



        public string EmpName { get; set; }

        public string Code { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        public decimal ServiceLengthMonth { get; set; }
        public decimal ServiceLengthMonthWeight { get; set; }



        public string JoinDate { get; set; }

        public decimal IndividualTotalContribution { get; set; }

        public decimal IndividualWeightedContribution { get; set; }

        public decimal IndividualProfitValue { get; set; }

        public decimal MultiplicationFactor { get; set; }

        public int FiscalYearDetailIdTo { get; set; }

        public string DateOfPermanent { get; set; }

        public string PFStartDate { get; set; }

        public bool IsPaid { get; set; }
        public string TransType { get; set; }
    }
}
