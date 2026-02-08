using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ApplicantSkillVM
    {
        public int Id { get; set; }
        public string Skill { get; set; }
        [Display(Name = "Skill Description")]
        public string SkillDescription { get; set; }
        [Display(Name = "Extra Curricular")]
        public string ExtraCurricular { get; set; }
        public string ApplicantId { get; set; }
    }
}
