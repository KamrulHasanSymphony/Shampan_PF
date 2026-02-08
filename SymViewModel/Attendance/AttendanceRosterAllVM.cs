using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class AttendanceRosterAllVM
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "To Date")] 
        public string ToDate { get; set; }
        [Required]
        [Display(Name = "From Date")] 
        public string FromDate { get; set; }
        [Required]
        public string Date { get; set; }
        [Display(Name = "Attendance Group")]
        public string AttendanceGroupId { get; set; }
        [Display(Name = "Attendance Group")]
        public string AttendanceGroup { get; set; }
        [Display(Name = " Attendance Structure")]
        public string AttendanceStructureId { get; set; }
        [Display(Name = " Attendance Structure")]
        public string AttendanceStructure { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")] 
        public string Remarks { get; set; }
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
