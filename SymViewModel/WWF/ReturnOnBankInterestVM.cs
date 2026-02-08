using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
    public class ReturnOnBankInterestVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Bank Branch")] public int BankBranchId { get; set; }
        [Display(Name = "Bank Branch Name")] public string BankBranchName { get; set; }

        [Display(Name = "ROBI Date")]
        public string TransactionDate { get; set; }
        [Display(Name = "Net Interest")]
        public decimal TotalValue { get; set; }
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
        public string TransType { get; set; }

    }
}
