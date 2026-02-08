using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ApplicantLanguageVM
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Reading { get; set; }
        public string Writing { get; set; }
        public string Speaking { get; set; }
        public string ApplicantId { get; set; }
    }
}
