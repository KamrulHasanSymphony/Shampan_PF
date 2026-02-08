using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
    public class EmployeeDailyOvertimeVM : RegularVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string OvertimeDate { get; set; }
        public decimal Overtime { get; set; }
        public decimal LateInHrs { get; set; }
        public decimal EarlyOutHrs { get; set; }

        public decimal LateInMins { get; set; }
        public decimal EarlyOutMins { get; set; }





        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Project { get; set; }
        public string Section { get; set; }
        public string EmpName { get; set; }
        public string Code { get; set; }
        public string SearchDate { get; set; }

        public bool IsOvertime { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public bool IsManual { get; set; }

        public decimal OvertimePrevious { get; set; }
        public decimal LateInHrsPrevious { get; set; }
        public decimal EarlyOutHrsPrevious { get; set; }
    }
}
