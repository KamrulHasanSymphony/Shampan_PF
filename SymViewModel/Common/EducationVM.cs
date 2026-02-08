using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class EducationVM
    {
        public int Id { get; set; }
        public string ExamTitle { get; set; }
        public string Major { get; set; }
        public string Institute { get; set; }
        public string Result { get; set; }
        public string PassYear { get; set; }
        public string Duration { get; set; }
        public string Achievment { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string ApplicantId { get; set; }
    }
}
