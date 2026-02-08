using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class Schedule2TaxSlabVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Min Tax")]
        public decimal MinimumTAX { get; set; }
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

         public List<Schedule2TaxSlabDetailVM> schedule2TaxSlabDetailVMs { get; set; }



         public string Operation { get; set; }
    }
}
