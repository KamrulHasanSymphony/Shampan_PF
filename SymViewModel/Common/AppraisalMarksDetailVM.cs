using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class AppraisalMarksDetailVM
    {
        public int Id { get; set; }

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
        public string IsOwn { get; set; }
        [Display(Name = "Team Lead")]
        public string IsTeamLead { get; set; }
        [Display(Name = "HR")]
        public string IsHR { get; set; }
        [Display(Name = "COO")]
        public string IsCOO { get; set; }
        [Display(Name = "MD")]
        public string IsMD { get; set; }

        [Display(Name = "P-1")]
        public string IsP1 { get; set; }
        [Display(Name = "P-2")]
        public string IsP2 { get; set; }
        [Display(Name = "P-3")]
        public string IsP3 { get; set; }
        [Display(Name = "P-4")]
        public string IsP4 { get; set; }
        [Display(Name = "P-5")]
        public string IsP5 { get; set; }

        public string Marks { get; set; }
    }
}
