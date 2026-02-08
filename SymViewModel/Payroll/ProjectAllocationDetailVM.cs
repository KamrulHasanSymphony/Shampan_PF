using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SymViewModel.Payroll
{
    public class ProjectAllocationDetailVM
    {
        public string Id { get; set; }
        [Display(Name = "Project Allocation Id")]
        public string ProjectAllocationId { get; set; }
        public string HeadName { get; set; }
        [Display(Name = "GL Code 1")]
        public string GLCode1 { get; set; }
        [Display(Name = "Portion 1")]
        public decimal Portion1 { get; set; }
        [Display(Name = "GLCode 2")]
        public string GLCode2 { get; set; }
        [Display(Name = "Portion 2")]
        public decimal Portion2 { get; set; }
        [Display(Name = "GLCode 3")]
        public string GLCode3 { get; set; }
        [Display(Name = "Portion 3")]
        public decimal Portion3 { get; set; }
        [Display(Name = "GLCode 4")]
        public string GLCode4 { get; set; }
        [Display(Name = "Portion 4")]
        public decimal Portion4 { get; set; }
        public decimal Total { get; set; }
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
