using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SymViewModel.Common
{
    public class FiscalYearVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        [Display(Name="Year")]
        public int Year { get; set; }
        [Display(Name = "Year Start")]
        public string YearStart { get; set; }
        [Display(Name = "Year End")]
        public string YearEnd { get; set; }
        [Display(Name = "Year Lock")]
        public bool YearLock { get; set; }
        [Display(Name = "Remark")]
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

        public string Name { get; set; }


        public List<FiscalYearDetailVM> FiscalYearDetailVM { get; set; }
    }
}
