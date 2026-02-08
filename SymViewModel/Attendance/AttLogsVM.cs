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
    public class AttLogsVM
    {
        [Display(Name = "SL No")]
        public int SLNo { get; set; }
        public string UserID { get; set; }
        public int AttendanceStructureId { get; set; }
        [Required]
        [Display(Name = "Punch Date")]
        public string PunchDate { get; set; }
        [Required]
        [Display(Name = "Punch Time")]
        public string PunchTime { get; set; }

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

        public string Code { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public string Project { get; set; }
        public string Section { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public bool Self { get; set; }
        [Display(Name = "Attendance Date")]
        public string AttnDate { get; set; }

        [Display(Name = "In Punch Time")]
        public string InPunchTime { get; set; }
        [Display(Name = "Out Punch Time")]
        public string OutPunchTime { get; set; }
        [Display(Name = "Lunch Time")]
        public string LunchTime { get; set; }
        [Display(Name = "Lunch Break")]
        public string LunchBreak { get; set; }
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [Display(Name = "Section")]
        public string SectionId { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "Designation")]
        public string DesignationId { get; set; }
        [Display(Name = "Working Hour")]
        public int WorkingHour { get; set; }
        [Display(Name = "Working Time")]
        public string WorkingTime { get; set; }
        [Display(Name = "Attendance Status")]
        public string AttnStatus { get; set; }
        [Display(Name = "Late Minute")]
        public int LateMin { get; set; }
    }
}
