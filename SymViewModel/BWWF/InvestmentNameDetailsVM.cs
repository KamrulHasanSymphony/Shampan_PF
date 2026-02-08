using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.BWWF
{
    public class InvestmentNameDetailsVM
    {
        public int Id { get; set; }
        public int InvestmentNameId { get; set; }
        [Display(Name = "Accrued From")]
        public decimal FromMonth { get; set; }
        [Display(Name = "To Month")]
        public decimal ToMonth { get; set; }
        [Display(Name = "Interest Rate %")]
        public decimal InterestRate { get; set; }
        public string Remarks { get; set; }
        public string TransType { get; set; }
    }
}
