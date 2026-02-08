
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
    public class LoanVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Interest")]
        public decimal InterestAmount { get; set; }
        [Display(Name = "Date")]
        public string TransactionDate { get; set; }
        [Display(Name = "Reference")]
        public string ReferenceNo { get; set; }
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
        public bool Post { get; set; }
        public string TransType { get; set; }

    }
}
