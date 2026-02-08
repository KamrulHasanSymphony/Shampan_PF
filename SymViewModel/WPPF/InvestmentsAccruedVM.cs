using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WPPF
{
    public class InvestmentAccruedVM
    {
        public int Id { get; set; }
        public string InvestmentNameId { get; set; }
        public string PeriodName { get; set; }
        public string FiscalYearDetailId { get; set; }
        [Display(Name = "Transaction Date")]
        public string TransactionDate { get; set; }
         [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }
        public string Remarks { get; set; }
        public decimal InvestmentValue { get; set; }
        public decimal AccruedMonth { get; set; }
        public decimal InterestRate { get; set; }
        public decimal AccruedInterest { get; set; }

        public decimal AitInterest { get; set; }
        public decimal NetInterest { get; set; }
        public bool Post { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }

        public string Operation { get; set; }
        public string TransType { get; set; }

    }
}
