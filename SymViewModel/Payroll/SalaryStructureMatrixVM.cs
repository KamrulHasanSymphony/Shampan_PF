using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Payroll
{
  public class SalaryStructureMatrixVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
        [Display(Name = "Salary Type")]
        public string SalaryTypeName { get; set; }
        [Display(Name = "S-1")]
        public decimal Step1Amount { get; set; }
        [Display(Name = "S-2")]
        public decimal Step2Amount { get; set; }
        [Display(Name = "S-3")]
        public decimal Step3Amount { get; set; }
        [Display(Name = "S-4")]
        public decimal Step4Amount { get; set; }
        [Display(Name = "S-5")]
        public decimal Step5Amount { get; set; }
        [Display(Name = "S-6")]
        public decimal Step6Amount { get; set; }
        [Display(Name = "S-7")]
        public decimal Step7Amount { get; set; }
        [Display(Name = "S-8")]
        public decimal Step8Amount { get; set; }
        [Display(Name = "S-9")]
        public decimal Step9Amount { get; set; }
        [Display(Name = "S-10")]
        public decimal Step10Amount { get; set; }
        [Display(Name = "S-11")]
        public decimal Step11Amount { get; set; }
        [Display(Name = "S-12")]
        public decimal Step12Amount { get; set; }
        [Display(Name = "S-13")]
        public decimal Step13Amount { get; set; }
        [Display(Name = "S-14")]
        public decimal Step14Amount { get; set; }
        [Display(Name = "S-15")]
        public decimal Step15Amount { get; set; }
        [Display(Name = "S-16")]
        public decimal Step16Amount { get; set; }
        [Display(Name = "S-17")]
        public decimal Step17Amount { get; set; }
        [Display(Name = "S-18")]
        public decimal Step18Amount { get; set; }
        [Display(Name = "S-19")]
        public decimal Step19Amount { get; set; }
        [Display(Name = "S-20")]
        public decimal Step20Amount { get; set; }
        [Display(Name = "S-21")]
        public decimal Step21Amount { get; set; }
        [Display(Name = "S-22")]
        public decimal Step22Amount { get; set; }
        [Display(Name = "S-23")]
        public decimal Step23Amount { get; set; }
        [Display(Name = "S-24")]
        public decimal Step24Amount { get; set; }
        [Display(Name = "S-25")]
        public decimal Step25Amount { get; set; }
        [Display(Name = "S-26")]
        public decimal Step26Amount { get; set; }
        [Display(Name = "S-27")]
        public decimal Step27Amount { get; set; }
        [Display(Name = "S-28")]
        public decimal Step28Amount { get; set; }
        [Display(Name = "S-29")]
        public decimal Step29Amount { get; set; }
        [Display(Name = "S-30")]
        public decimal Step30Amount { get; set; }

        [Display(Name = "S-1.5")]
        public decimal Step1_5Amount { get; set; }
        [Display(Name = "S-2.5")]
        public decimal Step2_5Amount { get; set; }
        [Display(Name = "S-3.5")]
        public decimal Step3_5Amount { get; set; }
        [Display(Name = "S-4.5")]
        public decimal Step4_5Amount { get; set; }
        [Display(Name = "S-5.5")]
        public decimal Step5_5Amount { get; set; }
        [Display(Name = "S-6.5")]
        public decimal Step6_5Amount { get; set; }
        [Display(Name = "S-7.5")]
        public decimal Step7_5Amount { get; set; }
        [Display(Name = "S-8.5")]
        public decimal Step8_5Amount { get; set; }
        [Display(Name = "S-9.5")]
        public decimal Step9_5Amount { get; set; }
        [Display(Name = "S-10.5")]
        public decimal Step10_5Amount { get; set; }
        [Display(Name = "S-11.5")]
        public decimal Step11_5Amount { get; set; }
        [Display(Name = "S-12.5")]
        public decimal Step12_5Amount { get; set; }
        [Display(Name = "S-13.5")]
        public decimal Step13_5Amount { get; set; }
        [Display(Name = "S-14.5")]
        public decimal Step14_5Amount { get; set; }
        [Display(Name = "S-15.5")]
        public decimal Step15_5Amount { get; set; }
        [Display(Name = "S-16.5")]
        public decimal Step16_5Amount { get; set; }
        [Display(Name = "S-17.5")]
        public decimal Step17_5Amount { get; set; }
        [Display(Name = "S-18.5")]
        public decimal Step18_5Amount { get; set; }
        [Display(Name = "S-19.5")]
        public decimal Step19_5Amount { get; set; }
        [Display(Name = "S-20.5")]
        public decimal Step20_5Amount { get; set; }
        [Display(Name = "S-30")]
        public decimal Step21_5Amount { get; set; }
        [Display(Name = "S-22.5")]
        public decimal Step22_5Amount { get; set; }
        [Display(Name = "S-30")]
        public decimal Step23_5Amount { get; set; }
        [Display(Name = "S-24.5")]
        public decimal Step24_5Amount { get; set; }
        [Display(Name = "S-25.5")]
        public decimal Step25_5Amount { get; set; }
        [Display(Name = "S-26.5")]
        public decimal Step26_5Amount { get; set; }
        [Display(Name = "S-17.5")]
        public decimal Step27_5Amount { get; set; }
        [Display(Name = "S-28.5")]
        public decimal Step28_5Amount { get; set; }
        [Display(Name = "S-29.5")]
        public decimal Step29_5Amount { get; set; }
        [Display(Name = "S-30.5")]
        public decimal Step30_5Amount { get; set; }
        [Display(Name = "S-31.5")]
        public decimal Step31_5Amount { get; set; }

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

        [Display(Name = "Designation Group")]
        public string DesignationGroupId { get; set; }

        public decimal CurrentAmount { get; set; }
        public string CurrentYear { get; set; }
        public string CurrentYearPart { get; set; }

    }
}
