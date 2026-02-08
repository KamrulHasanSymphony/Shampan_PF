using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class WithdrawVM
    {
        public int Id { get; set; }

        public bool IsInvested { get; set; }


        public string Code { get; set; }
        [Display(Name = "Withdraw Date")]
        public string WithdrawDate { get; set; }
        [Display(Name = "Withdraw Amount")]
        public decimal WithdrawAmount { get; set; }
        [Display(Name = "Bank Branch")]
        public int BankBranchId { get; set; }
        [Display(Name = "Bank Branch Name")]
        public string BankBranchName { get; set; }

        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }
        public int TransactionTypeId { get; set; }
         [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }
          [Display(Name = "Transaction Media")]
        public string TransactionMedia { get; set; }

        public string TillDate { get; set; }

        

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

        public decimal TotalDepositAmount { get; set; }

        public decimal TotalWithdrawAmount { get; set; }

        public decimal AvailableBalance { get; set; }

        public string BankName { get; set; }

        public string BranchName { get; set; }

        public string BankAccountType { get; set; }

        public string BankAccountNo { get; set; }

        public int TransactionMediaId { get; set; }

        public string TransType { get; set; }

    }
}
