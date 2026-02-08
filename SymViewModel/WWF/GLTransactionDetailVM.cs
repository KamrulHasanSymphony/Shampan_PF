using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WWF
{
    public class GLTransactionDetailVM
    {
        public int Id { get; set; }
        public int DrAccountIdForCredit { get; set; }

        public string TransactionCode { get; set; }
        public int TransactionMasterId { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public int AccountId { get; set; }

        public bool IsDr { get; set; }
        public bool IsSingle { get; set; }

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public bool Post { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        public string PostedBy { get; set; }
        public string PostedAt { get; set; }
        public string PostedFrom { get; set; }
        public decimal TransactionAmount { get; set; }

        public string AccountHead { get; set; }
        public string AccountType { get; set; }
        public string TransType { get; set; }


    }
}
