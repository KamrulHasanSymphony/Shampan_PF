using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Leave
{
   public class EmployeeCompensatoryLeaveVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        
        [Required]
        [Display(Name = "Leave Year")]
        public int LeaveYear { get; set; }
        [Display(Name = "Leave Type")]
        public string LeaveType_E { get; set; }
        [Required]
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Required]
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Required]
        [Display(Name = "Total Leave")]
        public decimal TotalLeave { get; set; }
        //[Required]
        //[Display(Name = "Leave Date")]
        //public string LeaveDate { get; set; }
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        [Display(Name = "Approve Date")]
        public string ApproveDate { get; set; }
        [Display(Name = "Approved")]
        public bool IsApprove { get; set; }

        [Display(Name = "Rejected By")]
        public string RejectedBy { get; set; }
        [Display(Name = "Reject Date")]
        public string RejectDate { get; set; }
        [Display(Name = "Reject")]
        public bool IsReject { get; set; }

        [Display(Name = "Half Day")]
        public bool IsHalfDay { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        public string Approval { get; set; }
        public string DayType { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public bool Self { get; set; }
       
        public string Supervisor { get; set; }

        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public string Branch { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }

        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string Grade { get; set; }
        public string SaveType { get; set; }

        public bool IsRegular { get; set; }

        public List<string> EmployeeIdList { get; set; }

        public string RedirectPage { get; set; }



        public string Button { get; set; }
    }
}
