using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WPPF
{
    public class InvestmentNameVM
    {
        public InvestmentNameVM()
        {
            InvestmentAccrued = new InvestmentAccruedVM();
        }
        public int Id { get; set; }
        [Display(Name = "Investment Name")]
        public string Name { get; set; }
        [Display(Name = "Investment Code")]
        public string Code { get; set; }
        public string Address { get; set; }
        [Display(Name = "Investment Type")]
        public int InvestmentTypeId { get; set; }
        [Display(Name = "Investment Date")]
        public string InvestmentDate { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Maturity Date")]
        public string MaturityDate { get; set; }

        [Display(Name = "AIT Rate%")]
        public Decimal AitRate { get; set; }

        [Display(Name = "Bank Branch Name")]
        public int BankBranchId { get; set; }
        [Display(Name = "Bank Branch")]
        public int BankNameId { get; set; }

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
        public List<InvestmentNameDetailsVM> InvestmentNameDetails { get; set; }

        public List<InvestmentAccruedVM> InvestmentAccrueds { get; set; }
        public InvestmentAccruedVM InvestmentAccrued { get; set; }


        public string Operation { get; set; }
        public string FiscalYearDetailId { get; set; }
        public string TransType { get; set; }

        public string BranchId { get; set; }
    }
}
