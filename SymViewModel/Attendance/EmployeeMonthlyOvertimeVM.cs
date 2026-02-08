using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public  class EmployeeMonthlyOvertimeVM : RegularVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public decimal TotalOvertime { get; set; }
        public decimal TotalLateInHrs { get; set; }
        public decimal TotalEarlyOutHrs { get; set; }

        public decimal TotalLateInMins { get; set; }
        public decimal TotalEarlyOutMins { get; set; }

        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Project { get; set; }
        public string Section { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        public string Code { get; set; }
        public int SearchFid { get; set; }

        public string FileName { get; set; }

        public string Filepath { get; set; }

        public bool IsExcel { get; set; }

        public decimal OTRate { get; set; }

        public decimal TotalOvertimePrevious { get; set; }

        public decimal TotalLateInHrsPrevious { get; set; }

        public decimal TotalEarlyOutHrsPrevious { get; set; }

        public bool IsManual { get; set; }

        public string OrderBy { get; set; }

        public List<string> Other3List { get; set; }

        public List<string> ProjectIdList { get; set; }

        public string DBName { get; set; }

        public string HoldStatus { get; set; }
    }
}
