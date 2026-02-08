using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class AppraisalQuestionsVM
    {
        [Required]
        public int Id { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "FeedBack Year")]
        public int FeedBackYear { get; set; }
        [Display(Name = "FeedBack Month")]
        public int FeedBackMonthId { get; set; }
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "Question")]
        public string Question { get; set; }
        [Display(Name = "User")]
        public bool IsUser { get; set; }
        [Display(Name = "Supervisor")]
        public bool IsSupervisor { get; set; }
        [Display(Name = "Department Head")]
        public bool IsDepartmentHead { get; set; }
        [Display(Name = "Management")]
        public bool IsManagement { get; set; }
        [Display(Name = "HR")]
        public bool IsHR { get; set; }
    }
}
