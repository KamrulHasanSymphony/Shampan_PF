using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class AppraisalQuestionBankVM
    {
        
        [Required]
        public int Id { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        public string CategoryId { get; set; }
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        [Display(Name = "Question")]
        public string Question { get; set; }
        [Display(Name = "Mark")]
        public string Mark { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string CreatedFrom { get; set; }
        public string Remark {get;set;}
    }
}
