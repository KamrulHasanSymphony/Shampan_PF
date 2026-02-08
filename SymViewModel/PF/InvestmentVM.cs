using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class InvestmentVM
    {
        public int Id { get; set; }
        [Display(Name = "Investment Code")]
        public string TransactionCode { get; set; }
        public string TransactionType { get; set; }
        

        [Display(Name = "Investment Type")]
        public string InvestmentType { get; set; }
        [Display(Name = "Investment Type")]
        public int InvestmentTypeId { get; set; }


        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }


        [Display(Name = "Investment Address")]
        public string InvestmentAddress { get; set; }
        [Display(Name = "Investment Date")]
        public string InvestmentDate { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Maturity Date")]
        public string MaturityDate { get; set; }
        [Display(Name = "Interest rate (%)")]
        public decimal InvestmentRate { get; set; }
        [Display(Name = "Investment Value")]
        public decimal InvestmentValue { get; set; }

        [Display(Name = "Investment Name")]
        public int InvestmentNameId { get; set; }
        [Display(Name = "Investment Name")]
        public string InvestmentName { get; set; }

         [Display(Name = "Investment Months")]
        public string InvestmentMonths { get; set; }
         [Display(Name = "Total Interest")]
        public decimal TotalInterest { get; set; }
         [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

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
        public int ReferenceId { get; set; }
        public bool IsEncashed { get; set; }
        public string TransType { get; set; }

         [Display(Name = "Net Interest")]
        public decimal NetInterest { get; set; }

        public decimal AIT { get; set; }

         [Display(Name = "Bank charge & Excise Duty.")]
        public decimal BankCharge { get; set; }     
        public decimal AitInterest { get; set; }

        public string BranchId { get; set; }
    }
}
