using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class EETransactionVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }

        [Display(Name = "Transaction Date")]
        public string TransactionDateTime { get; set; }
        [Display(Name = "Grand Total")]
        public decimal GrandTotal { get; set; }
        [Display(Name = "Reference No.")]
        public string ReferenceNo1 { get; set; }
        public string ReferenceNo2 { get; set; }
        public string ReferenceNo3 { get; set; }
        public bool Post { get; set; }
        [Display(Name = "Active")]
        public string TransactionType { get; set; }

        public bool IsPS { get; set; }


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
        public string TransType { get; set; }

        [Display(Name = "Date From")]
        public string TransactionDateTimeFrom { get; set; }

        [Display(Name = "Date To")]
        public string TransactionDateTimeTo { get; set; }

        public List<EETransactionDetailVM> eeTransactionDetailVMs {get; set;}

        public string PostStatus { get; set; }
    }
}
