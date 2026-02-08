using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class AttendanceDailyVM
    {
        public string Id { get; set; }
        public int AttendanceStructureId { get; set; }
        public string GroupId { get; set; }
        public string EmployeeId { get; set; }
        [Required]
        [Display(Name = "Proxy")]
        public string ProxyID { get; set; }
        [Required]
        [Display(Name = "Daily Date")]
        public string DailyDate { get; set; }
        [Required]
        [Display(Name = "Punch In Time")]
        public string PunchInTime { get; set; }
        [Required]
        [Display(Name = "Punch Out Time")]
        public string PunchOutTime { get; set; }
        [Required]
        [Display(Name = "Punch Out Time Next Day")]
        public string PunchOutTimeNextday { get; set; }
        [Required]
        [Display(Name = "Punch Next Day")]
        public bool PunchNextDay { get; set; }
        [Required]
        [Display(Name = "Manual")]
        public bool IsManual { get; set; }
        [Required]
        [Display(Name = "In Time")]
        public string InTime { get; set; }
        [Required]
        [Display(Name = "In Time By")]
        public string InTimeBy { get; set; }
        [Required]
        [Display(Name = "Out Time")]
        public string OutTime { get; set; }
        [Required]
        [Display(Name = "Out Time By")]
        public string OutTimeBy { get; set; }
        [Required]
        [Display(Name = "Late Min")]
        public int LateMin { get; set; }
        [Required]
        [Display(Name = "Working Hrs")]
        public string WorkingHrs { get; set; }
        [Required]
        [Display(Name = "WorkingHrs By")]
        public string WorkingHrsBy { get; set; }
        [Required]
        [Display(Name = "Working Hrs Rest")]
        public string WorkingHrsRest { get; set; }
        [Required]
        [Display(Name = "Total OT")]
        public decimal TotalOT { get; set; }
        [Required]
        [Display(Name = "Total OT By")]
        public decimal TotalOTBy { get; set; }
        [Required]
        [Display(Name = "Extra OT")]
        public decimal ExtraOT { get; set; }
        [Required]
        [Display(Name = "OT Rest")]
        public decimal OTRest { get; set; }
        [Required]
        [Display(Name = "Bonus Minute")]
        public decimal BonusMinit { get; set; }
        [Required]
        [Display(Name = "Lunch Out Deduct")]
        public decimal LunchOutDeduct { get; set; }
        [Required]
        [Display(Name = "Attn Status")]
        public string AttnStatus { get; set; }
        [Required]
        [Display(Name = "Day Status")]
        public string DayStatus { get; set; }
        [Required]
        [Display(Name = "Early Deduct")]
        public decimal EarlyDeduct { get; set; }
        [Required]
        [Display(Name = "Late Deduct")]
        public decimal LateDeduct { get; set; }
        [Required]
        [Display(Name = "Tiffin Allow")]
        public bool TiffinAllow { get; set; }
        [Required]
        [Display(Name = "Dinner Allow")]
        public bool DinnerAllow { get; set; }
        [Required]
        [Display(Name = "Ifter Allow")]
        public bool IfterAllow { get; set; }
        [Required]
        [Display(Name = "Gross Amount")]
        public decimal GrossAmnt { get; set; }
        [Required]
        [Display(Name = "Basic Amount")]
        public decimal BasicAmnt { get; set; }
        [Required]
        [Display(Name = "Tiffin Amount")]
        public decimal TiffinAmnt { get; set; }
        [Required]
        [Display(Name = "Ifter Amount")]
        public decimal IfterAmnt { get; set; }
        [Required]
        [Display(Name = "Dinner Amount")]
        public decimal DinnerAmnt { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public string CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedFrom { get; set; }

        public string LastUpdateAt { get; set; }

        public string LastUpdateBy { get; set; }

        public string LastUpdateFrom { get; set; }

        public string Remarks { get; set; }


        //Extra Added for ViewAttendanceDaily
        public string Code { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }
        public string Project { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string ProjectId { get; set; }
        public string SectionId { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string GradeId { get; set; }
        //public string BranchId              { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        [Display(Name = "Attendance Stracture")]
        public string AttnStractureName { get; set; }

    }
}
