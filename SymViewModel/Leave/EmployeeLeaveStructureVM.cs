using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Leave
{
   public class EmployeeLeaveStructureVM
    {
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]
        public int LeaveStructureId { get; set; }
        [Required]
        [Display(Name = "Year")]
        public int LeaveYear { get; set; }
        [Required]
        [Display(Name = "Leave Type")]
        public string LeaveType_E { get; set; }
        [Required]
        [Display(Name = "Opening Balance")]
        public decimal OpeningDays { get; set; }
        [Display(Name = "Allowed Days")]
        public decimal LeaveDays { get; set; }
        [Display(Name = "Max Days")]
        public decimal MaxBalance { get; set; }
        [Required]
        [Display(Name = "Earned")]
        public bool IsEarned { get; set; }
        [Required]
        [Display(Name = "Compensation")]
        public bool IsCompensation { get; set; }
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

        #region only for data cary
        public string[] employeeIds { get; set; }
        //public string leaveStructureId { get; set; }
        #endregion

        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Starting Leave Days")]
        public decimal OpeningLeaveDays { get; set; }
        [Display(Name = "Carry Forward")]
        public bool IsCarryForward { get; set; }
    }
}
