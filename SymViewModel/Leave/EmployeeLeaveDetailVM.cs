using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Leave
{
   public class EmployeeLeaveDetailVM
    {

        public int Id { get; set; }
        public string EmployeeId { get; set; }

        public int EmployeeLeaveStructureId { get; set; }        
		[Display(Name = "Leave Year")]
        public int LeaveYear { get; set; }        
		[Display(Name = "Leave Type")]
        public string LeaveType_E { get; set; }        
		[Display(Name = "Total Leave")]
        public decimal TotalLeave { get; set; }        
		[Display(Name = "Leave Date")]
        public string LeaveDate { get; set; }        
		[Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }        
		[Display(Name = "Approved")]
        public bool IsApprove { get; set; }
        [Display(Name = "Half Day")]
        public bool IsHalfDay { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]

        public string FiscalYearDetailId { get; set; }
      
      
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


        public bool IsLWP { get; set; }
    }
}
