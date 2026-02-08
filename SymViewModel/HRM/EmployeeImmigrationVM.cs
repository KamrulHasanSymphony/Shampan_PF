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
    public class EmployeeImmigrationVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Required]

        [Display(Name = "Immigration Type")]
        public string ImmigrationType_E { get; set; }
        [Display(Name = "Immigration Number")]
        [Required]
        public string ImmigrationNumber { get; set; }
        [Required]
        [Display(Name = "Issue Date")]
        public string IssueDate { get; set; }
        [Required]
        [Display(Name = "Expire Date")]
        public string ExpireDate { get; set; }
        [Required]
        [Display(Name = "Issued By")]
        public string IssuedBy_E { get; set; }
        [Required]
        [Display(Name = "Eligible Review Date")]
        public string EligibleReviewDate { get; set; }
        [Display(Name = "Passport/Visa")]
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
