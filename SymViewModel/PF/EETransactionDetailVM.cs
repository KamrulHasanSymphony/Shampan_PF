using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class EETransactionDetailVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int SL { get; set; }
        [Display(Name = "Earning Expense Transaction")]
        public int EETransactionId { get; set; }
        [Display(Name = "Earning Expense Head")]
        public int EEHeadId { get; set; }
        [Display(Name = "EE Head")]
        public string EEHeadName { get; set; }

        [Display(Name = "Transaction Date Time")]
        public string TransactionDateTime { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Reference No. 1")]
        public string ReferenceNo1 { get; set; }
        [Display(Name = "Reference No. 2")]
        public string ReferenceNo2 { get; set; }
        [Display(Name = "Reference No. 3")]
        public string ReferenceNo3 { get; set; }

        public bool Post { get; set; }
        public string TransactionType { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public bool IsPS { get; set; }

        [Display(Name = "Date From")]
        public string TransactionDateTimeFrom { get; set; }
        public string Code { get; set; }
        [Display(Name = "Date To")]
        public string TransactionDateTimeTo { get; set; }
        [Display(Name = "Post Status")]
        public string PostStatus { get; set; }
        public string TransType { get; set; }
    }
}
