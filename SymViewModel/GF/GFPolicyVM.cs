using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.GF
{
    public class GFPolicyVM
    {
        public int Id { get; set; }
        [Display(Name = "Policy Name")]
        public string PolicyName { get; set; }
        [Display(Name = "Fixed")]
        public bool IsFixed { get; set; }
        [Display(Name = "Last Basic Multiplication ")]
        public decimal LastBasicMultipication { get; set; }
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

        public string Name { get; set; }
        public string Operation { get; set; }

        [Display(Name = "Job Year From")]
        public int JobDurationYearFrom { get; set; }
        [Display(Name = "Job Year To")]
        public int JobDurationYearTo { get; set; }
        [Display(Name = "Multiplication Factor")]
        public decimal MultipicationFactor { get; set; }

    }
}
