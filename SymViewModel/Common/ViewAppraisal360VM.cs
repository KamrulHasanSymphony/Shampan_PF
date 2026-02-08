using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ViewAppraisal360VM
    {
        public int FeedBackYear { get; set; }
        public string PeriodName { get; set; }
        public string FeedbackBy { get; set; }
        public string DepartmentName { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string FeedbackCode { get; set; }
        public string FeedbackName { get; set; }
        public bool   IsFeedbackCompeted { get; set; }
        public string UserId { get; set; }
        public string FeedBackUserId { get; set; }
        public int    FeedBackMonth { get; set; }
        public int Appraisal360Id { get; set; }

    }
}
