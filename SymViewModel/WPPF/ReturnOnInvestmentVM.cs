using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WPPF
{
    public class ReturnOnInvestmentVM
    {
        public int Id { get; set; }

        [Display(Name = "Investment")]
        public int InvestmentId { get; set; }

        [Display(Name = "Reference No.")]
        public string ReferenceNo { get; set; }


        [Display(Name = "ROI Date")]
        public string ROIDate { get; set; }
        [Display(Name = "ROI Rate (%)")]
        public decimal ROIRate { get; set; }
        [Display(Name = "ROI Total")]
        public decimal ROITotalValue { get; set; }
        [Display(Name = "Net Interest")]
        public decimal TotalInterestValue { get; set; }
        public bool Post { get; set; }


        [Display(Name = "Interest Amount")]
        public decimal ActualInterestAmount { get; set; }
        [Display(Name = "Service Charge")]
        public decimal ServiceChargeAmount { get; set; }


        [Display(Name = "Investment Type")]
        public string InvestmentType { get; set; }
        [Display(Name = "Investment Type")]
        public int InvestmentTypeId { get; set; }

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

        public bool IsTransferPDF { get; set; }


        public bool IsBankDeposited { get; set; }

        [Display(Name = "Fixed")]
        public bool IsFixed { get; set; }

        public int ReferenceId { get; set; }

        public List<ROIDetailVM> detailVMs { get; set; }

        public string TransactionCode { get; set; }

        public string TransactionType { get; set; }
        public string TransType { get; set; }
    }
}
