using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ParameterVM
    {
        public string FiscalPeriodDetailsId { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "Section")]
        public string SectionId { get; set; }
        [Display(Name = "Project")]
        public string ProjectId { get; set; }
        [Display(Name = "Designation")]
        public string DesignationId { get; set; }
        [Display(Name = "Code From")]
        public string CodeF { get; set; }
        [Display(Name = "Code To")]
        public string CodeT { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
        [Display(Name = "DOJ From")]
        public string dojFrom { get; set; }
        [Display(Name = "DOJ To")]
        public string dojTo { get; set; }

        [Display(Name = "Earning Type")]
        public int EarningTypeId { get; set; }
        [Display(Name = "Deduction Type")]
        public int DeductionTypeId { get; set; }


        [Display(Name = "EmployeeId From")]
        public string EmployeeIdF { get; set; }
        [Display(Name = "EmployeeId To")]
        public string EmployeeIdT { get; set; }




        public string fYear { get; set; }
        public string FiscalPeriodDetailId { get; set; }
        public string FiscalPeriodDetailIdTo { get; set; }
        public string ReportGroup { get; set; }
        public string Orderby { get; set; }
        public string View { get; set; }
        




    }
}
