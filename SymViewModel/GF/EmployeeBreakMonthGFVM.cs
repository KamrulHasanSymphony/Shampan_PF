using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.GF
{
    public class EmployeeBreakMonthGFVM
    {

        public string Id { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Code")]
        public string Code { get; set; }
        public string CodeT { get; set; }
        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }
       
        [Display(Name = "Date")]
        public string OpeningDate { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool Post { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        public string Section { get; set; }
        public string Project { get; set; }
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary { get; set; }
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string ProjectId { get; set; }
        public string Orderby { get; set; }

  [Display(Name = "Employer Contribution")]
  public decimal EmployerContribution { get; set; }
  [Display(Name = "Employer Profit")]
  public decimal EmployerProfit { get; set; }
  public string TransType { get; set; }



    }
}
