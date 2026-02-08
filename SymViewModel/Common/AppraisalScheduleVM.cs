using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class AppraisalScheduleVM
    {
        public int Id { get; set; }      
        [Display(Name = "Schedule Name")]
        public string ScheduleName { get; set; }

        public string QuestionSetId { get; set; }
        [Display(Name = "Question Set Name")]
        public string QuestionSetName { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public string CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string CreateFrom { get; set; }
    }
}
