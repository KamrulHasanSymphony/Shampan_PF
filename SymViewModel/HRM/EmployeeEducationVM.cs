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
  public  class EmployeeEducationVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Display(Name = "Degree")]
        public string Degree_E { get; set; }
        public string Institute { get; set; }
        [Display(Name = "Major/Group")]
        public string Major { get; set; }
        [Required]
        [Display(Name = "Total Year")]
        public decimal TotalYear { get; set; }
        [Required]
        [Display(Name = "Year Of Passing")]
        public string YearOfPassing { get; set; }
        public string Result { get; set; }
        public decimal CGPA { get; set; }
        public decimal Scale { get; set; }
        [Display(Name="Marks(%)")]
        public decimal Marks { get; set; }

        [Display(Name = "Last")]
        public bool IsLast { get; set; }    

        [Display(Name = "Certificate")]
        public string FileName { get; set; }
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
