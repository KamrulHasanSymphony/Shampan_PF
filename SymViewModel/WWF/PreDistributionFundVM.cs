using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
    public class PreDistributionFundVM
    {
        public int Id { get; set; }
        [Display(Name = "Investment")] public int InvestmentId { get; set; }
        [Display(Name = "Funding Date")] public string FundingDate { get; set; }
        [Display(Name = "Funding Value")] public decimal FundingValue { get; set; }
        public bool Post { get; set; }
        [Display(Name = "Distribute")] public bool IsDistribute { get; set; }

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
        [Display(Name = "Reserved Funding Value")] public decimal ReservedFundingValue { get; set; }
        [Display(Name = "Total Funding Value")] public decimal TotalFundingValue { get; set; }
        [Display(Name = "Funding Reference Ids")] public string FundingReferenceIds { get; set; }
        [Display(Name = "Transaction Type")] public string TransactionType { get; set; }



        public string FundingReferenceNos { get; set; }

        public bool IsPaid { get; set; }
        public string Code { get; set; }
        [Display(Name = "Transaction Date")]
        public string TransactionDate { get; set; }
        [Display(Name = "Total Value")]
        public string TotalValue { get; set; }
        public string TransType { get; set; }

        public string BranchId { get; set; }
    }
    public class ColumnDetails
    {
        public string COLUMN_NAME { get; set; }
    }
}
