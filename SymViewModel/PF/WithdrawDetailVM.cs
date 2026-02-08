using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class WithdrawDetailVM
    {
        public int Id { get; set; }
        public int WithdrawId { get; set; }
        public string TransactionDate { get; set; }
        public int AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        public string TransactionType { get; set; }
        public bool Post { get; set; }


        public int AccountHead { get; set; }

        public string AccountType { get; set; }
        public string TransType { get; set; }

    }
}
