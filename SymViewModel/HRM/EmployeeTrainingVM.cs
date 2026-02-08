using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
  public  class EmployeeTrainingVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        public string Topics { get; set; }
        [Display(Name = "Institute Name")]
        public string InstituteName { get; set; }
        public string Location { get; set; }
        [Display(Name = "Funded By")]
        public string FundedBy { get; set; }
        [Required]
        [Display(Name = "Duration(Mon)")]
        public int DurationMonth { get; set; }
        [Required]
        [Display(Name = "Duration(Days)")]
        public int DurationDay { get; set; }
        public string Achievement { get; set; }
        [Display(Name = "Allowances(Tk)")]
        public decimal AllowancesTotalTk { get; set; }
        [Required]
        [Display(Name = "Training Place")]
        public string TrainingPlace_E { get; set; }
        [Display(Name="Certificate")]
        public string FileName { get; set; }

        [Display(Name = "Training Time")]
        public string TraningTime { get; set; }
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }

        [Display(Name = "Training Course")]
        public string TrainingCourse { get; set; }
        [Display(Name = "Training Status")]
        public string TrainingStatus_E { get; set; }
        [Display(Name = "Date From")]
        public string DateFrom { get; set; }
        [Display(Name = "Date To")]
        public string DateTo { get; set; }
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
