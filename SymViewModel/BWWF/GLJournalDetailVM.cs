using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class GLJournalDetailVM
    {
        public int Id { get; set; }
        public int GLJournalId { get; set; }

        [Display(Name = "Account Head")]
        public int COAId { get; set; }
        public bool IsDr { get; set; }

        [Display(Name = "Debit")]
        public decimal DrAmount { get; set; }
        [Display(Name = "Credit")]
        public decimal CrAmount { get; set; }
        [Display(Name = "Date")]
        public string TransactionDate { get; set; }
        [Display(Name = "Journal Type")]
        public int JournalType { get; set; }
        [Display(Name = "Transaction Type")]
        public int TransactionType { get; set; }
        public bool Post { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Line Narration")]
        public string Remarks { get; set; }

        public string AccountName { get; set; }
        public string TransType { get; set; }
        public bool IsYearClosing { get; set; }        
        public string COAName { get; set; }
    }
}
