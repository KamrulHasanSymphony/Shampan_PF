using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class AbsentDeductionStructureVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "In Time")]
        public string InTime { get; set; }
        [Required]
        [Display(Name = "Out Time")]
        public string OutTime { get; set; }
        [Display(Name = "Out Next Day")]
        public bool IsOutNextDay { get; set; }

        [Required]
        [Display(Name = "Lunch Break(Min)")]
        public int LunchBreak { get; set; }

        [Required]
        [Display(Name = "Lunch Time")]
        public string LunchTime { get; set; }

        [Required]
        [Display(Name = "In Grace")]
        public int InGrace { get; set; }

        [Required]
        [Display(Name = "In Time End")]
        public string InTimeEnd { get; set; }
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
        [Display(Name = "In Time Start")]
        public string InTimeStart { get; set; }   
        [Required]
        [Display(Name = "Working Hour")]
        public decimal WorkingHour { get; set; }
        [Required]
        [Display(Name = "OT Time")]
        public int OTTime { get; set; }
        
        [Required]
        [Display(Name = "Tiffin")]
        public bool IsTiff { get; set; }
        [Required]
        [Display(Name = "Tiffin Start Time")]
        public string TiffinSTime { get; set; }
        [Required]
        [Display(Name = "Tiffin Next Day")]
        public bool IsTiffNextDay { get; set; }
        
        [Required]
        [Display(Name = "Deduct Tiffin Time")]
        public int DeductTiffTime { get; set; }
        [Required]
        [Display(Name = "Ifter")]
        public bool IsIfter { get; set; }
        [Required]
        [Display(Name = "Ifter Start Time")]
        public string IfterSTime { get; set; }
        [Required]
        [Display(Name = "Ifter Next Day")]
        public bool IsIfterNextDay { get; set; }
         
        [Required]
        [Display(Name = "Deduct Ifter Time")]
        public int DeductIfterTime { get; set; }
        [Required]
        [Display(Name = "Dinner Start Time")]
        public string DinnerSTime { get; set; }
        [Required]
        [Display(Name = "Dinner Next Day")]
        public bool IsDinNextDay { get; set; }
         
        [Required]
        [Display(Name = "Deduct Dinner Time")]
        public int DeductDinTime { get; set; }
        [Required]
        [Display(Name = "Deduct Early Out")]
        public bool IsDeductEarlyOut { get; set; }
        [Required]
        [Display(Name = "Early Out Min")]
        public int EarlyOutMin { get; set; }
        [Required]
        [Display(Name = "Deduct Late In")]
        public bool IsDeductLateIn { get; set; }
        [Required]
        [Display(Name = "Late In Min")]
        public int LateInMin { get; set; }
        
        [Required]   [Display(Name = "OT Round Up")]
        public bool IsOTRoundUp { get; set; }
        [Required]
        [Display(Name = "OT Round Up Min")]
        public int OTRoundUpMin { get; set; }

        [Required]
        [Display(Name = "In OT")]
        public bool IsInOT { get; set; }
        [Required]
        [Display(Name = "Slot 1 Absent Days")]
        public int FirstSlotAbsentDays { get; set; }
        [Required]
        public string FirstSlotAbsentFrom { get; set; }
        [Required]
        public string FirstSlotDaysCount { get; set; }

        [Required]
        [Display(Name = "Slot 1 Absent From Gross")]
        public bool FirstSlotAbsentFromGross { get; set; }
       
       
        [Required]
        [Display(Name = "Slot 2 Absent From Gross")]
        public bool SecondSlotAbsentFromGross { get; set; }
        [Required]
        public string SecondSlotAbsentFrom { get; set; }
        [Required]
        public string SecondSlotDaysCount { get; set; }
        [Required]
        [Display(Name = "Slot 2 Absent Days")]
        public int SecondSlotAbsentDays { get; set; }
        [Required]
        [Display(Name = "NP Absent From Gross")]
        public bool NPAbsentFromGross { get; set; }
        [Required]
        public string NPAbsentFrom { get; set; }
        [Required]
        public string NPDaysCount { get; set; }
        [Required]
        public string LWPFrom { get; set; }
        public string LWPDaysCount { get; set; }
        [Required]
        [Display(Name = "Late In Calculate")]
        public int LateInAllowDays { get; set; }
        [Required]
        [Display(Name = "Late In Absent Days")]
        public int LateInAbsentDays { get; set; }
        [Required]
        [Display(Name = "Early Out Calculate")]
        public int EarlyOutAllowDays { get; set; }
        [Required]
        [Display(Name = "Early Out Absent Days")]
        public int EarlyOutAbsentDays { get; set; }


        public string EmployeeId { get; set; }


        public string AttendanceGroupId { get; set; }


    }
}










