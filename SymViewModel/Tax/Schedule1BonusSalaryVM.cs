using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Tax
{
    public class Schedule1BonusSalaryVM
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
        public decimal Line1A { get; set; }
        public decimal Line1B { get; set; }
        public decimal Line1C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line1Remarks { get; set; }
        public decimal Line2A { get; set; }
        public decimal Line2B { get; set; }
        public decimal Line2C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line2Remarks { get; set; }
        public decimal Line3A { get; set; }
        public decimal Line3B { get; set; }
        public decimal Line3C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line3Remarks { get; set; }
        public decimal Line4A { get; set; }
        public decimal Line4B { get; set; }
        public decimal Line4C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line4Remarks { get; set; }
        public decimal Line5A { get; set; }
        public decimal Line5B { get; set; }
        public decimal Line5C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line5Remarks { get; set; }
        public decimal Line6A { get; set; }
        public decimal Line6B { get; set; }
        public decimal Line6C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line6Remarks { get; set; }
        public decimal Line7A { get; set; }
        public decimal Line7B { get; set; }
        public decimal Line7C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line7Remarks { get; set; }
        public decimal Line8A { get; set; }
        public decimal Line8B { get; set; }
        public decimal Line8C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line8Remarks { get; set; }
        public decimal Line9A { get; set; }
        public decimal Line9B { get; set; }
        public decimal Line9C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line9Remarks { get; set; }
        public decimal Line10A { get; set; }
        public decimal Line10B { get; set; }
        public decimal Line10C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line10Remarks { get; set; }
        public decimal Line11A { get; set; }
        public decimal Line11B { get; set; }
        public decimal Line11C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line11Remarks { get; set; }
        public decimal Line12A { get; set; }
        public decimal Line12B { get; set; }
        public decimal Line12C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line12Remarks { get; set; }
        public decimal Line13A { get; set; }
        public decimal Line13B { get; set; }
        public decimal Line13C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line13Remarks { get; set; }
        public decimal Line14A { get; set; }
        public decimal Line14B { get; set; }
        public decimal Line14C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line14Remarks { get; set; }
        public decimal Line15A { get; set; }
        public decimal Line15B { get; set; }
        public decimal Line15C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line15Remarks { get; set; }
        public decimal Line16A { get; set; }
        public decimal Line16B { get; set; }
        public decimal Line16C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line16Remarks { get; set; }
        public decimal Line17A { get; set; }
        public decimal Line17B { get; set; }
        public decimal Line17C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line17Remarks { get; set; }
        public decimal Line18A { get; set; }
        public decimal Line18B { get; set; }
        public decimal Line18C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line18Remarks { get; set; }
        public decimal Line19A { get; set; }
        public decimal Line19B { get; set; }
        public decimal Line19C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line19Remarks { get; set; }
        public decimal Line20A { get; set; }
        public decimal Line20B { get; set; }
        public decimal Line20C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line20Remarks { get; set; }
        public decimal Line21A { get; set; }
        public decimal Line21B { get; set; }
        public decimal Line21C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line21Remarks { get; set; }
        public decimal Line22A { get; set; }
        public decimal Line22B { get; set; }
        public decimal Line22C { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Line22Remarks { get; set; }
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


        public string EmployeeName { get; set; }
        public string Project { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string FiscalPeriod { get; set; }
        public string PeriodStart { get; set; }
        public string PeriodEnd { get; set; }

        public decimal TotalIncomeAmount { get; set; }
        public decimal TotalExemptedAmount { get; set; }
        public decimal TotalTaxableAmount { get; set; }
        public decimal TotalTaxPayAmount { get; set; }
        public decimal FinalTaxAmount { get; set; }

        public decimal ProcessedTaxAmount { get; set; }
        public decimal FinalBonusTaxAmount { get; set; }

        
        public string Operation { get; set; }

        public int TaxSlabId { get; set; }

        public List<EmployeeTaxSlabDetailVM> employeeTaxSlabDetailVMs {get; set;}

        public string ScheduleType { get; set; }
        
    }
}
