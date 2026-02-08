using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class Schedule3TaxSlabDetailVM
    {
        public int Id { get; set; }
        public int Schedule3TaxSlabId { get; set; }
        public string SlabName { get; set; }

        public decimal EarningCeilingMin { get; set; }
        public decimal EarningCeilingMax { get; set; }
        public decimal CeilingMin        { get; set; }
        public decimal CeilingMax        { get; set; }


        public decimal Ratio { get; set; }
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

        public decimal ActualAmount { get; set; }
        public decimal RestAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal MonthlyEarningCeilingMin { get; set; }
        public decimal MonthlyEarningCeilingMax { get; set; }
        public decimal MonthlyCeiling { get; set; }

    }
}
