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
    public class EmployeeExtraCurriculumActivityVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Display(Name = "Skill Proficiency")]
        public string Skill { get; set; }
        [Required]
        [Display(Name = " Year Of Experience")]
        public decimal YearsOfExperience { get; set; }

        [Display(Name = "Skill/Quality/Level")]
        public string SkillQuality_E { get; set; }

        [Display(Name = "Experience/Institute")]
        public string Institute   { get; set; }
        public string Address     { get; set; }
        public string Date        { get; set; }

        [Display(Name = "Achievement/Award")]
        public string Achievement { get; set; }
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
    }
}
