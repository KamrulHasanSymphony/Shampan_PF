using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class AttendanceDailyNewVM
    {

        public int Id { get; set; }
        public int AttendanceMigrationId { get; set; }
        public int AttendanceStructureId { get; set; }
        public string GroupId { get; set; }
        public string EmployeeId { get; set; }
        public string ProxyID { get; set; }
        public string DailyDate { get; set; }
        public string PunchInTime { get; set; }
        public string PunchOutTime { get; set; }
        public string PunchOutTimeNextday { get; set; }
        public bool PunchNextDay { get; set; }
        public bool IsManual { get; set; }
        public string InTime { get; set; }
        public string InTimeBy { get; set; }
        public string OutTime { get; set; }
        public string OutTimeBy { get; set; }

        public int MovementLateInMin { get; set; }
        public int LateInMin { get; set; }

        public bool IsDeductLateIn { get; set; }
        public int LunchBreak { get; set; }
        public bool IsDeductEarlyOut { get; set; }

        public int MovementEarlyOutMin { get; set; }
        public int EarlyOutMin { get; set; }

        public string WorkingHrs { get; set; }
        public string WorkingHrsBy { get; set; }
        public int TotalOT { get; set; }
        public int TotalOTBy { get; set; }
        public string AttnStatus { get; set; }
        public string DayStatus { get; set; }
        public decimal EarlyDeduct { get; set; }
        public decimal LateDeduct { get; set; }
        public bool TiffinAllow { get; set; }
        public bool DinnerAllow { get; set; }
        public bool IfterAllow { get; set; }

        public decimal TiffinAmnt { get; set; }
        public decimal IfterAmnt { get; set; }
        public decimal DinnerAmnt { get; set; }
        public int DeductTiffTime { get; set; }
        public int DeductIfterTime { get; set; }
        public int DeductDinTime { get; set; }

        public decimal GrossAmnt { get; set; }
        public decimal BasicAmnt { get; set; }
        public string Remarks { get; set; }




        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }

        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        public string Designation { get; set; }

        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }

        [Display(Name = "Designation")]
        public string DesignationId { get; set; }

        [Display(Name = "Department")]
        public string DepartmentId { get; set; }

        [Display(Name = "Section")]
        public string SectionId { get; set; }

        [Display(Name = "Project")]
        public string ProjectId { get; set; }

        public bool IsInTimeUpdate { get; set; }
        public bool IsOutTimeUpdate { get; set; }
        public bool IsEmployeeChecked { get; set; }

        public string UpdatedInTime { get; set; }
        public string UpdatedOutTime { get; set; }

        public bool IsNextDay { get; set; }

        [Display(Name = "Code From")]
        public string CodeFrom { get; set; }
        [Display(Name = "Code To")]
        public string CodeTo { get; set; }

        public string DateFrom { get; set; }

        public string DateTo { get; set; }

        public int Fydid { get; set; }
        
    }
}
