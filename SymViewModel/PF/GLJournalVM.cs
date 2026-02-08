using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class GLJournalVM
    {
        public GLJournalVM()
        {
            GLJournalDetails = new List<GLJournalDetailVM>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        [Display(Name = "Date")]
        public string TransactionDate { get; set; }
        [Display(Name = "Journal Type")]
        public int JournalType { get; set; }
        [Display(Name = "Transaction Type")]
        public int TransactionType { get; set; }
        public bool Post { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        [Display(Name = "Narration")]
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
        public decimal TransactionValue { get; set; }

        public List<GLJournalDetailVM> GLJournalDetails { get; set; }
        public string TransactionTypeName { get; set; }
        public string JournalTypeName { get; set; }
        public string TransType { get; set; }

        public bool IsYearClosing { get; set; }

        public string BranchId { get; set; }
        public int SourceId { get; set; }
        public string Source { get; set; }

        public string CashCOAId { get; set; }

        public bool IsApprove { get; set; }
    }
}
