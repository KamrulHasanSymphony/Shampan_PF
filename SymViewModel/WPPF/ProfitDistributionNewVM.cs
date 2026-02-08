using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SymViewModel.WPPF
{
    public class ProfitDistributionNewVM
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

        [Display(Name = "PFDetailFiscalYearDetailIds")]
        public string PFDetailFiscalYearDetailIds { get; set; }

        [Display(Name = "PreDistributionFundIds")]
        public string PreDistributionFundIds { get; set; }
        [Display(Name = "Distribution Date")]
        public string DistributionDate { get; set; }
        [Display(Name = "Fiscal Period")]
        public int FiscalYearDetailId { get; set; }
        [Display(Name = "Total Employee Contribution")]
        public decimal TotalEmployeeContribution { get; set; }
        [Display(Name = "Total Employer Contribution")]
        public decimal TotalEmployerContribution { get; set; }
        [Display(Name = "Total Profit")]
        public decimal TotalProfit { get; set; }
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }


        [Display(Name = "Fiscal Period")]
        public int FiscalYearDetailIdTo { get; set; }


        public int FiscalYear { get; set; }


        public List<ProfitDistributionDetailVM> profitDistributionDetailVMs { get; set; }

        public decimal TotalExpense { get; set; }
        public decimal AvailableDistributionAmount { get; set; }


        public decimal MultiplicationFactor { get; set; }

        public decimal TotalWeightedContribution { get; set; }
        [Display(Name = "Period From")]
        public string PeriodNameFrom { get; set; }

        [Display(Name = "Period To")]
        public string PeriodNameTo { get; set; }

        public bool IsPaid { get; set; }


        public string PreDistributionFundId { get; set; }
        public string EmployeeId { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal EmployeeProfit { get; set; }
        public decimal EmployerProfit { get; set; }
        public decimal EmployeeProfitDistribution { get; set; }
        public decimal EmployeerProfitDistribution { get; set; }

        public PreDistributionFundVM PreDistributionFund { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeCode { get; set; }
        public string TransType { get; set; }

        public string BranchId { get; set; }
    }
}