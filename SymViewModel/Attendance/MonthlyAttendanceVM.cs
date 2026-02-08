using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class MonthlyAttendanceVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }


        public int DOM { get; set; }
        public int HoliDay { get; set; }
        public string HoliDayDetails { get; set; }
        public int OffDay { get; set; }
        public int WorkingDay { get; set; }
        public int NPDay { get; set; }
        public decimal NPAmount { get; set; }
        public decimal LWPDay { get; set; }
        public decimal LWPAmount { get; set; }
        public decimal AbsentDay { get; set; }
        public decimal AbsentAmount { get; set; }
        public decimal TotalLeave { get; set; }
        public string LeaveDetail { get; set; }
        public decimal PresentDay { get; set; }
        public int LateDay { get; set; }
        public decimal LateAmount { get; set; }
        public decimal AttnBonus { get; set; }
        public bool OTAllow { get; set; }
        public bool OTAllowBY { get; set; }
        public bool OTAllowExtra { get; set; }
        public decimal OTRate { get; set; }
        public decimal TotalOTHrs { get; set; }
        public decimal TotalOTHrsBY { get; set; }
        public decimal TotalOTHrsExtra { get; set; }
        public decimal OTAmount { get; set; }
        public decimal OTAmountBY { get; set; }
        public decimal OTAmountExtra { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

         public string EmploymentType { get; set; }
        public decimal LateAbsentDay { get; set; }
        public decimal LateAbsentHour { get; set; }
        public decimal EarlyOutDayCount { get; set; }
        public decimal EarlyOutHourCount { get; set; }
        public decimal EarlyOutDeductAmount { get; set; }
        public decimal LateInDayCount { get; set; }
        public decimal LateInHourCount { get; set; }
        public decimal LateInDeductAmount { get; set; }
        public decimal OtherDeductionDay { get; set; }




        public string OrderBy { get; set; }

        public string CodeF { get; set; }

        public string CodeT { get; set; }

        public int FiscalYear { get; set; }


        public string MultipleCode { get; set; }
        public string MultipleDesignation { get; set; }
        public string MultipleDepartment { get; set; }
        public string MultipleSection { get; set; }
        public string MultipleProject { get; set; }
        public string MultipleOther1 { get; set; }
        public string MultipleOther2 { get; set; }
        public string MultipleOther3 { get; set; }


        public List<string> DesignationList { get; set; }

        public List<string> DepartmentList { get; set; }

        public List<string> SectionList { get; set; }

        public List<string> ProjectList { get; set; }

        public List<string> Other1List { get; set; }

        public List<string> Other2List { get; set; }

        public List<string> Other3List { get; set; }



        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string Other3 { get; set; }
        public string Other4 { get; set; }
        public string Other5 { get; set; }


        public List<string> CodeList { get; set; }




        public bool IsEmployeeChecked { get; set; }

        [Display(Name = "Employee Code")]
        public string EmpCode { get; set; }

        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        public string Designation { get; set; }




        public string PeriodName { get; set; }

        public string PeriodStart { get; set; }

        public string PeriodEnd { get; set; }
    }
}
