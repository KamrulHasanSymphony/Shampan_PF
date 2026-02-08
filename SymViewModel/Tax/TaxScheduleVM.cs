using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class TaxScheduleVM
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string FiscalYearId { get; set; }
        public int Year { get; set; }
        public int FiscalYearDetailId { get; set; }

        public Schedule1SalaryVM schedule1SalaryVM { get; set; }
        public Schedule2HousePropertyVM schedule2HousePropertyVM { get; set; }
        public Schedule3InvestmentVM schedule3InvestmentVM { get; set; }
        //public Schedule3InvestmentYearlyVM schedule3InvestmentYearlyVM { get; set; }
        public ScheduleForm10BBMonthlyVM scheduleForm10BBMonthlyVM { get; set; }
        public ScheduleForm10BBYearlyVM scheduleForm10BBYearlyVM { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string FiscalPeriod { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }
        
        public string Operation { get; set; }
        public string Remarks { get; set; }
        [Display(Name="Employee Code")]
        public string EmployeeCode { get; set; }

        public int EmployeeTaxSlabCount { get; set; }
        public int EmployeeSchedule2TaxSlabCount { get; set; }
        public int EmployeeSchedule3TaxSlabCount { get; set; }
        [Display(Name = "Final Tax Amount")]
        public decimal FinalTaxAmount { get; set; }
        public decimal FinalTaxAmountMonthly { get; set; }
        public decimal FinalBonusTaxAmount { get; set; }
      
        public string TransactionType { get; set; }
    }
}
