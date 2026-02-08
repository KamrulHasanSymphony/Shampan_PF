using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
   public class Appraisal360DetailsVM
    {
        public int Id { get; set; }
        [Display(Name = "Appraisal360 Id")]
        public int Appraisal360Id { get; set; }
        [Display(Name = "Department")]
        public string DepartmentId { get; set; }
        [Display(Name = "FeedBack Year")]
        public int FeedBackYear { get; set; }
        [Display(Name = "FeedBack Month Id")]
        public int FeedBackMonthId { get; set; }
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "Question")]
        public int QuestionId { get; set; }
        [Display(Name = "Question")]
        public string Question { get; set; }
        [Display(Name = "Feedback Value")]
        public int FeedbackValue { get; set; }
        [Display(Name = "Feedback User Type")]
        public string FeedbackUserType { get; set; }

    }
}
