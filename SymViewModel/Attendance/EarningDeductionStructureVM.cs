using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class EarningDeductionStructureVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

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

        [Required]
        [Display(Name = "Days Count From")]
        public string DaysCountFrom { get; set; }

        [Required]
        [Display(Name = "Slot 1 Absent From")]
        public string FirstSlotAbsentFrom { get; set; }

        [Required]
        [Display(Name = "Slot 1 Absent Days")]
        public int FirstSlotAbsentDays { get; set; }

        [Required]
        [Display(Name = "Slot 2 Absent From")]
        public string SecondSlotAbsentFrom { get; set; }
        [Required]
        [Display(Name = "Slot 2 Absent Days")]
        public int SecondSlotAbsentDays { get; set; }
        [Required]
        [Display(Name = "NP Absent From")]
        public string NPAbsentFrom { get; set; }
        [Required]
        [Display(Name = "LWP From")]
        public string LWPFrom { get; set; }

        [Display(Name = "Weekly OT Rate")]
        public decimal WeeklyOTRate { get; set; }
        [Display(Name = "Govt OT Rate")]
        public decimal GovtOTRate { get; set; }
        [Display(Name = "Festival OT Rate")]
        public decimal FestivalOTRate { get; set; }
        [Display(Name = "Special OT Rate")]
        public decimal SpecialOTRate { get; set; }

        public string EmployeeId { get; set; }
        public string AttendanceGroupId { get; set; }
        public string Operation { get; set; }


        [Display(Name = "Monthly LateIn Deduct")]
        public bool IsMonthlyLateInDeduct { get; set; }
        [Display(Name = "Monthly LateIn Hourly Count")]
        public bool IsMonthlyLateInHourlyCount { get; set; }
        [Display(Name = "Monthly LateIn Count Days")]
        public int MonthlyLateInCountDays { get; set; }
        [Display(Name = "LateIn Absent Days")]
        public int LateInAbsentDays { get; set; }


        [Display(Name = "Monthly EarlyOut Deduct")]
        public bool IsMonthlyEarlyOutDeduct { get; set; }
        [Display(Name = "Monthly EarlyOut Hourly Count")]
        public bool IsMonthlyEarlyOutHourlyCount { get; set; }
        [Display(Name = "Monthly EarlyOut Count Days")]
        public int MonthlyEarlyOutCountDays { get; set; }
        [Display(Name = "EarlyOut Absent Days")]
        public int EarlyOutAbsentDays { get; set; }

        [Display(Name = "Day Rate Count From")]
        public string DayRateCountFrom { get; set; }
        [Display(Name = "Hour Rate Count From")]
        public string HourRateCountFrom { get; set; }
        [Display(Name = "Day Rate Division Factor")]
        public int DayRateDivisionFactor { get; set; }
        [Display(Name = "Hour Rate Division Factor")]
        public int HourRateDivisionFactor { get; set; }
    }
}










