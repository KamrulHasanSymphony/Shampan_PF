using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace SymViewModel.Common
{
    public class FiscalYearDetailVM
    {
        public int Id { get; set; }
        public string FiscalYearId { get; set; }


        public int Year { get; set; }
        
        [Display(Name="Period")]
        public string PeriodName { get; set; }
        [Display(Name = "Start")]
        public string PeriodStart { get; set; }
        [Display(Name = "End")]
        public string PeriodEnd { get; set; }
        [Display(Name = "Lock")]
        public bool PeriodLock { get; set; }
        public string Remarks { get; set; }
        public string Name { get; set; }
        [Display(Name = "Loan Lock")]
        public bool LoanLock { get; set; }
        [Display(Name = "TAX Lock")]
        public bool TAXLock { get; set; }
        [Display(Name = "PF Lock")]
        public bool PFLock { get; set; }
        [Display(Name = "Payroll Lock")]
        public bool PayrollLock { get; set; }
        [Display(Name = "Sage Post Complete")]
        public bool SagePostComplete { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        public string PeriodId { get; set; }
    }
}
