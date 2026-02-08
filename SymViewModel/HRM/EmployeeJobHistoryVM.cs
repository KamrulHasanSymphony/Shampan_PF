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
   public class EmployeeJobHistoryVM
   {
       [Required]
       public int Id { get; set; }
       [Required]
       public string EmployeeId { get; set; }

       [Required]
       public string Company { get; set; }
       [Required]
       [Display(Name = "Job Title")]
       public string JobTitle { get; set; }
       [Required]
       [Display(Name = "Job From")]
       public string JobFrom { get; set; }
       [Required]
       [Display(Name = "Job To")]
       public string JobTo { get; set; }
       [Display(Name = "Service Length")]
       public string ServiceLength { get; set; }
       [Display(Name = "Experience Certificate")]
       public string FileName { get; set; }
       [Display(Name = "Job Description")]
       [StringLength(450, ErrorMessage = "Job Description cannot be longer than 40 characters.")]
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

       [Display(Name = "Reason For Leaving")] 
       public string ReasonForLeaving { get; set; }

    }
}
