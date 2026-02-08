using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WPPF
{
    public class ProfitDistributionOfBankInterestVM
    {
        public int Id { get; set; }

        public int ROBIId { get; set; }
        public int FiscalYearDetailId { get; set; }
        public string PFStructureId { get; set; }
        public string ProjectId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string DesignationId { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Distribution Date")] public string DistributionDate { get; set; }
        [Display(Name = "TotalInterest Value")] public decimal TotalInterestValue { get; set; }
        [Display(Name = "SelfInterest Value")] public decimal SelfInterestValue { get; set; }
 

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
    }
}
