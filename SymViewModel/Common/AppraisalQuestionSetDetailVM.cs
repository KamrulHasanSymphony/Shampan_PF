using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class AppraisalQuestionSetDetailVM
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public string Logid { get; set; }
        public string FullName { get; set; }
        public string QuestionSetId { get; set; }       
        [Display(Name = "Question Name")]
        public string QuestionName { get; set; }
        public string QuestionId { get; set; }
              
        [Display(Name = "Own")]
        public bool IsOwn { get; set; }
        [Display(Name = "Team Lead")]
        public bool IsTeamLead { get; set; }
        [Display(Name = "HR")]
        public bool IsHR { get; set; }
        [Display(Name = "COO")]
        public bool IsCOO { get; set; }
        [Display(Name = "MD")]
        public bool IsMD { get; set; }

        [Display(Name = "P-1")]
        public bool IsP1 { get; set; }
        [Display(Name = "P-2")]
        public bool IsP2 { get; set; }
        [Display(Name = "P-3")]
        public bool IsP3 { get; set; }
        [Display(Name = "P-4")]
        public bool IsP4 { get; set; }
        [Display(Name = "P-5")]
        public bool IsP5 { get; set; }
        public bool IsActive { get; set; }
        public string Marks { get; set; }
    }
}
