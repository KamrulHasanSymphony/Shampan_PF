using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Attendance
{
   public class EmployeeDailyAbsenceVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string AbsentDate { get; set; }
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

        public bool IsAbsent { get; set; }
        public bool IsAbsentPrevious { get; set; }

        public string SearchDate { get; set; }

        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        public string TransactionType { get; set; }

        public object DayStatus { get; set; }
        public bool IsManual { get; set; }
       
    }
}

