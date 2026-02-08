using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
   public class AttendanceRosterDetailVM
    {
        public string Id { get; set; }

        public string AttendanceRosterId { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Attendance Group")]
        public string AttendanceGroupId { get; set; }
        [Display(Name = "Attendance Group")]
        public string AttendanceGroupName { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")] 
        public string Remarks { get; set; }

       [Display(Name="1 ")]
        public string AttendanceStructureId1 { get; set; }
       [Display(Name="2 ")]
        public string AttendanceStructureId2 { get; set; }
       [Display(Name="3 ")]
        public string AttendanceStructureId3 { get; set; }
       [Display(Name="4 ")]
        public string AttendanceStructureId4 { get; set; }
       [Display(Name="5 ")]
        public string AttendanceStructureId5 { get; set; }
       [Display(Name="6 ")]
        public string AttendanceStructureId6 { get; set; }
       [Display(Name="7 ")]
        public string AttendanceStructureId7 { get; set; }
       [Display(Name="8 ")]
        public string AttendanceStructureId8 { get; set; }
       [Display(Name="9 ")]
        public string AttendanceStructureId9 { get; set; }
       [Display(Name="10")]
        public string AttendanceStructureId10 { get; set; }
       [Display(Name="11")]
        public string AttendanceStructureId11 { get; set; }
       [Display(Name="12")]
        public string AttendanceStructureId12 { get; set; }
       [Display(Name="13")]
        public string AttendanceStructureId13 { get; set; }
       [Display(Name="14")]
        public string AttendanceStructureId14 { get; set; }
       [Display(Name="15")]
        public string AttendanceStructureId15 { get; set; }
       [Display(Name="16")]
        public string AttendanceStructureId16 { get; set; }
       [Display(Name="17")]
        public string AttendanceStructureId17 { get; set; }
       [Display(Name="18")]
        public string AttendanceStructureId18 { get; set; }
       [Display(Name="19")]
        public string AttendanceStructureId19 { get; set; }
       [Display(Name="20")]
        public string AttendanceStructureId20 { get; set; }
       [Display(Name="21")]
        public string AttendanceStructureId21  { get; set; }
       [Display(Name="22")]
        public string AttendanceStructureId22  { get; set; }
       [Display(Name="23")]
        public string AttendanceStructureId23  { get; set; }
       [Display(Name="24")]
        public string AttendanceStructureId24  { get; set; }
       [Display(Name="25")]
        public string AttendanceStructureId25  { get; set; }
       [Display(Name="26")]
        public string AttendanceStructureId26  { get; set; }
       [Display(Name="27")]
        public string AttendanceStructureId27  { get; set; }
       [Display(Name="28")]
        public string AttendanceStructureId28  { get; set; }
       [Display(Name="29")]
        public string AttendanceStructureId29 { get; set; }
       [Display(Name="30")]
        public string AttendanceStructureId30 { get; set; }
       [Display(Name="31")]
       public string AttendanceStructureId31 { get; set; }

    }
}
