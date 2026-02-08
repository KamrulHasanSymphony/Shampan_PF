using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class AppraisalEvaluationVM
    {
        public AppraisalEvaluationVM()
        {
            AppraisalEvaluationDetailVM = new List<AppraisalEvaluationDetailVM>();
        }

        public int Id { get; set; }
        [Display(Name = "Question Set Name")]
        public string QuestionSetName { get; set; }

        public string DepartmentId { get; set; }
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        public string AssignToId { get; set; }
        [Display(Name = "AssignTo Name")]
        public string AssignToName { get; set; }

        public string EvaluationForId { get; set; }
        [Display(Name = "Evaluation For")]
        public string EvaluationFor { get; set; }

        public string Year { get; set; }
        public string ExDate { get; set; }
        public string CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string CreateFrom { get; set; }

        public List<AppraisalEvaluationDetailVM> AppraisalEvaluationDetailVM { get; set; }
    }
}
