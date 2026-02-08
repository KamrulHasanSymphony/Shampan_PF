using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
    public class ForfeitureAccountVM
    {
        public int Id { get; set; }

        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Forfeit Date")]
        public string ForfeitDate { get; set; }
        [Display(Name = "Employee Contribution")]
        public decimal EmployeContributionForfeitValue { get; set; }
        [Display(Name = "Employee Profit")]
        public decimal EmployeProfitForfeitValue { get; set; }
        [Display(Name = "Employer Contribution")]
        public decimal EmployerContributionForfeitValue { get; set; }
        [Display(Name = "Employer Profit")]
        public decimal EmployerProfitForfeitValue { get; set; }

        [Display(Name = "Total Forfeit")]
        public decimal TotalForfeitValue { get; set; }
        


        public bool Post { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
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

        public bool IsTransferPDF { get; set; }

        public int PFSettlementId { get; set; }



        [Display(Name = "Employee Name")]
        public string EmpName { get; set; }

        public string Code { get; set; }

        public string Designation { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Project { get; set; }

        [Display(Name = "Resign Date")]
        public string EmpResignDate { get; set; }

        public string Operation { get; set; }
        public string TransType { get; set; }
    }
}
