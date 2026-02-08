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
    public class EmployeePromotionVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public string DesignationId { get; set; }
        [Required]
        [Display(Name = "Promotion")]
        public bool IsPromotion { get; set; }
        [Required]
        [Display(Name = "Promotion Date")]
        public string PromotionDate { get; set; }
        [Display(Name = "Grade")]
        public string GradeName { get; set; }
        [Display(Name = "Grade")]
        public string GradeId { get; set; }
         [Display(Name = "Step Id")]
        public string StepId { get; set; }
        public bool IsCurrent { get; set; }
         [Display(Name = "File Name")]
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

        [Display(Name = "Designation")]
        public string DesignationName { get; set; }

        public List<string> EmployeeIdList { get; set; }

    }
}
