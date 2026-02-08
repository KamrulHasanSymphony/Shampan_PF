using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class InvestmentRenewVM
    {
        public int Id { get; set; }
        public int InvestmentId { get; set; }
        public string TransactionCode { get; set; }
        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }
        [Display(Name = "Investment Date")]
        public string InvestmentDate { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Maturity Date")]
        public string MaturityDate { get; set; }
        [Display(Name = "Investable/Encashable Value")]
        public decimal InvestmentValue { get; set; }
        [Display(Name = "Bank charge & Excise Duty.")]
        public decimal BankCharge { get; set; }
        [Display(Name = "Excise Duty")]
        public decimal BankExciseDuty { get; set; }

        [Display(Name = "New Interest Rate")]
        public decimal InterestRate { get; set; }
        [Display(Name = "Investment Month")]
        public string InvestmentMonth { get; set; }
        [Display(Name = "Source Tax Deduct")]
        public decimal SourceTaxDeduct { get; set; }
        [Display(Name = "Other Charge")]
        public decimal OtherCharge { get; set; }

        [Display(Name = "New Interest")]
        public decimal Interest { get; set; }
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
        public string Name { get; set; }
        public InvestmentVM InvestmentVm { get; set; }

        public bool IsEncashed { get; set; }
        public string TransType { get; set; }
        public decimal AIT { get; set; }
         [Display(Name = "Adition Amount")]
        public decimal AditionAmount { get; set; }
        [Display(Name = "Deduction Amount")]
        public decimal EncashAmount { get; set; }
        public decimal InvestableAmount { get; set; }
    }
}
