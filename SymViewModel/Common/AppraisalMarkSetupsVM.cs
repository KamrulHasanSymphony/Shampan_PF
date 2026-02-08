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
    public class AppraisalMarkSetupsVM
    {    
        public int Id { get; set; }
        public string DepartmentId { get; set; }
         [Display(Name = "FeedBack Year")]
        public int FeedBackYear { get; set; }
        [Display(Name = "FeedBack Month")]
        public int FeedBackMonthId { get; set; }
        [Display(Name = "Period Name")]
        public string PeriodName{ get; set; }
        [Display(Name = "Eatch Question Mark")]
        public int EatchQuestionMark { get; set; }
        [Display(Name = "User Mark")]
        public int UserMark { get; set; }
        [Display(Name = "Supervisor Mark")]
        public int SupervisorMark { get; set; }
        [Display(Name = "Department Head Mark")]
        public int DepartmentHeadMark { get; set; }
        [Display(Name = "Management Mark")]
        public int ManagementMark { get; set; }
        [Display(Name = "HR Mark")]
        public int HRMark { get; set; }

    }
}
