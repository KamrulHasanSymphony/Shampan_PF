using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class EmployeeSchedule2TaxSlabDetailVM
    {
        public int Id { get; set; }
        public int Schedule2Id { get; set; }
        public string EmployeeId { get; set; }
        public string FiscalYearId { get; set; }
        public int Year { get; set; }
        public int FiscalYearDetailId { get; set; }
        public int Schedule2TaxSlabId { get; set; }
        public string SlabName { get; set; }
        public decimal Ceiling { get; set; }
        public decimal Ratio { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal RestAmount { get; set; }
        public decimal TaxAmount { get; set; }
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

        public string EmployeeName { get; set; }

        public string EmployeeCode { get; set; }

        public string Project { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Designation { get; set; }

        public decimal MonthlyCeiling { get; set; }
    }
}
