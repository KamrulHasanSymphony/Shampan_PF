using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SymViewModel.Leave
{
    public class EmployeeLeaveBalanceVM
    {
        public string EmpEmail;
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }
        [Display(Name = "Opening Balance")]
        public string OpeningBalance { get; set; }
        [Display(Name = "Allowed Days")]
        public string Total { get; set; }
        //[Display(Name = "Used Days")]
        [Display(Name = "Availed Leave")]
        public string Used { get; set; }
        [Display(Name = "Current Balance")]
        public string Have { get; set; }

        public string Id { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Active")]
        public DateTime JoinDate { get; set; }
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


        public string EmployeeId { get; set; }
        public int EmployeeLeaveStructureId { get; set; }
        [Display(Name = "Leave Year")]
        public int LeaveYear { get; set; }
        [Display(Name = "Current Leave Type")]
        public string CurrentLeaveType { get; set; }
        [Display(Name = "From Date")]
        public string FromDate { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Total Leave")]
        public decimal TotalLeave { get; set; }
        [Display(Name = "Approved By")]
        public string approvedby { get; set; }
        [Display(Name = "Approve Date")]
        public string ApproveDate { get; set; }
        [Display(Name = "Approved")]
        public bool IsApprove { get; set; }
        [Display(Name = "Reject Date")]
        public string RejectDate { get; set; }
        [Display(Name = "Reject")]
        public bool IsReject { get; set; }
        [Display(Name = "Half Day")]
        public bool IsHalfDay { get; set; }
        [Display(Name = "Leave Purpose")]
        public string LeavePurpose { get; set; }

        [Display(Name = "Gender")]
        public string Gender_E { get; set; }
        public string Religion { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }

        public string Supervisor { get; set; }
    }

    public class EmployeeLeaveStatementVM
    {
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }
        [Display(Name = "Leave Year")]
        public int LeaveYear { get; set; }
        [Display(Name = "EL Type Name")]
        public string ELTypeName { get; set; }
        [Display(Name = "EL Opening")]
        public decimal ELOpening { get; set; }
        [Display(Name = "EL Leave Days")]
        public decimal ELLDays { get; set; }
        [Display(Name = "EL Used")]
        public decimal ELUsed { get; set; }
        [Display(Name = "EL Balance")]
        public decimal ELBalance { get; set; }
        [Display(Name = "SL Type Name")]
        public string SLTypeName { get; set; }
        [Display(Name = "SL Opening")]
        public decimal SLOpening { get; set; }
        [Display(Name = "SL Leave Days")]
        public decimal SLLDays { get; set; }
        [Display(Name = "SL Used")]
        public decimal SLUsed { get; set; }
        [Display(Name = "SL Balance")]
        public decimal SLBalance { get; set; }
        [Display(Name = "CL Type Name")]
        public string CLTypeName { get; set; }
        [Display(Name = "CL Opening")]
        public decimal CLOpening { get; set; }
        [Display(Name = "CL Leave Days")]
        public decimal CLLDays { get; set; }
        [Display(Name = "CL Used")]
        public decimal CLUsed { get; set; }
        [Display(Name = "CL Balance")]
        public decimal CLBalance { get; set; }
        public string Id { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        [Display(Name = "Active")]
        public string Code { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Join Date")]
        public DateTime JoinDate { get; set; }
        [Display(Name = "Probation End")]
        public DateTime ProbationEnd { get; set; }
        [Display(Name = "Date Of Permanent")]
        public DateTime DateOfPermanent { get; set; }
        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }
        public string Project { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        [Display(Name = "Transfer Date")]
        public DateTime TransferDate { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        [Display(Name = "Promotion")]
        public bool IsPromotion { get; set; }
        [Display(Name = "Promotion Date")]
        public DateTime PromotionDate { get; set; }
    }
}
