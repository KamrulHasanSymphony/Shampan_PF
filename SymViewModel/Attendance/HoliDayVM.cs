using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class HoliDayVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Holiday Date")]
        public string HoliDay { get; set; }
        [Display(Name = "Holiday Type")] 
        public string HoliDayType { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")] public string Remarks { get; set; }
        [Required]
        [Display(Name = "Active")] 
        public bool IsActive { get; set; }       
        [Required]
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
    }
}
