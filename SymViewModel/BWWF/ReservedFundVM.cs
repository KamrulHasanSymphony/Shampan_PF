using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class ReservedFundVM
    {
        public int Id { get; set; }

        [Display(Name = "Reserved Date")]
        public string ReservedDate { get; set; }
        [Display(Name = "Reserved Value")]
        public decimal ReservedValue { get; set; }
        public bool Post { get; set; }

        public bool IsTransferPDF { get; set; }


        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }



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

        public int PDFId { get; set; }

        public string Operation { get; set; }
        public string TransType { get; set; }
    }
}
