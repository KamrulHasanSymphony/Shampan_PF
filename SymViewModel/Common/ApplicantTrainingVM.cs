using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ApplicantTrainingVM
    {
        public int Id { get; set; }
        public string TrainingTitle { get; set; }
        public string Topic { get; set; }
        public string Institute { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string Year { get; set; }
        public string Duration { get; set; }
        public string ApplicantId { get; set; }
    }
}
