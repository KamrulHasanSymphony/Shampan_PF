using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class AttendanceMigrationVM
    {
        public int Id { get; set; }

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
        [Display(Name = "Punch Out Time Nextday")]
        public string PunchOutTimeNextday { get; set; }
        [Required]
        [Display(Name = "Punch Next Day")]
        public bool PunchNextDay { get; set; }
        [Required]
        [Display(Name = "Manual")]
        public bool IsManual { get; set; }
        [Required]
        [Display(Name = "In Time")]
        public string PInTime { get; set; }
        [Required]
        [Display(Name = "In Grace")]
        public int PInGrace { get; set; }
        [Required]
        [Display(Name = "In Time Start")]
        public string PInTimeStart { get; set; }
        [Required]
        [Display(Name = "In Time End")]
        public string PInTimeEnd { get; set; }
        [Required]
        [Display(Name = "Out Time")]
        public string POutTime { get; set; }
        [Required]
        [Display(Name = "Lunch Time")]
        public string PLunchTime { get; set; }
        [Required]
        [Display(Name = "Lunch Break")]
        public int PLunchBreak { get; set; }
        [Required]
        [Display(Name = "Working Hour")]
        public decimal PWorkingHour { get; set; }
        [Required]
        [Display(Name = "OT Time")]
        public int POTTime { get; set; }
        [Required]
        [Display(Name = "Tiffin")]
        public bool PIsTiff { get; set; }
        [Required]
        [Display(Name = "Tiffin Start Time")]
        public string PTiffinSTime { get; set; }
        [Required]
        [Display(Name = "Tiffin Next Day")]
        public bool PIsTiffNextDay { get; set; }
        [Required]
        [Display(Name = "Deduct Tiffin Time")]
        public int PDeductTiffTime { get; set; }
        [Required]
        [Display(Name = "Ifter")]
        public bool PIsIfter { get; set; }
        [Required]
        [Display(Name = "Ifter Start Time")]
        public string PIfterSTime { get; set; }
        [Required]
        [Display(Name = "Ifter Next Day")]
        public bool PIsIfterNextDay { get; set; }
        [Required]
        [Display(Name = "Deduct Ifter Time")]
        public int PDeductIfterTime { get; set; }
        public bool PIsDinner { get; set; }
        [Required]
        [Display(Name = "Dinner Start Time")]
        public string PDinnerSTime { get; set; }
        [Required]
        [Display(Name = "Dinner Next Day")]
        public bool PIsDinNextDay { get; set; }
        [Required]
        [Display(Name = "Deduct DinnerTime")]
        public int PDeductDinTime { get; set; }
        [Required]
        [Display(Name = "Deduct Early Out")]
        public bool PIsDeductEarlyOut { get; set; }
        [Required]
        [Display(Name = "Early Out Min")]
        public int PEarlyOutMin { get; set; }
        [Required]
        [Display(Name = "Deduct Late In")]
        public bool PIsDeductLateIn { get; set; }
        [Required]
        [Display(Name = "Late In Min")]
        public int PLateInMin { get; set; }
        [Required]
        [Display(Name = "OT Round Up")]
        public bool PIsOTRoundUp { get; set; }
        [Required]
        [Display(Name = "OT Round Up Min")]
        public int POTRoundUpMin { get; set; }
        public string Remarks { get; set; }
        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }



        public string EmployeeCode { get; set; }

        public string AttendanceTime { get; set; }

        public string AttendanceDate { get; set; }
        public string AttendanceDateFrom { get; set; }
        public string AttendanceDateTo { get; set; }

        public int POutGrace { get; set; }

        public int PWorkingMin { get; set; }
        public int PTiffinMin { get; set; }
        public int PIfterMin { get; set; }
        public int PDinnerMin { get; set; }


        public string AttendanceSystem { get; set; }
        public string ProcessTime { get; set; }

    }
}
