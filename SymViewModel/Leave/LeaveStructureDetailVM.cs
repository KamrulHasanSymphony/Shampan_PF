using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SymViewModel.Leave
{
    public class LeaveStructureDetailVM
    {
        public int Id { get; set; }
        public int LeaveStructureId { get; set; }
        [Display(Name = "Leave Type")]
        public string LeaveType_E { get; set; }
        [Display(Name = "Leave Day")]
        public string LeaveDays { get; set; }
        [Display(Name = "Earned")]
        public bool IsEarned { get; set; }
        [Display(Name = "Compensation")]
        public bool IsCompensation { get; set; }
        [Display(Name = "Carry Forward")]
        public bool IsCarryForward { get; set; }
        [Display(Name = "Max Balance")]
        public int MaxBalance { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
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
    }
}
