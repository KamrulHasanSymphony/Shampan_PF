using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class TaxDepositVM
    {
        public int Id { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string FiscalYearId { get; set; }
        public int Year { get; set; }
        public int FiscalYearDetailId { get; set; }
        [Display(Name = "Challan No")]
        public string ChallanNo { get; set; }
        [Display(Name = "Bank Information")]
        public string BankInformation { get; set; }
         [Display(Name = "Deposit Amount")]
        public decimal DepositAmount { get; set; }
        [Display(Name = "Deposit Date")]
        public string DepositDate { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        public string EmployeeName { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string FiscalPeriod { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }

        public string Particular { get; set; }

        public string Operation { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Code To")]
        public string EmployeeCodeTo { get; set; }
        public string PeriodName { get; set; }

        public bool IsEmployeeChecked { get; set; }

    }
}
