using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class AppraisalEvaluationFor
    {
        public int Id { get; set; }
        [Display(Name = "Evaluation Name")]
        public string EvaluationName { get; set; }

        public bool IsActive { get; set; }
  
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string CreatedFrom { get; set; }     
    }
}
