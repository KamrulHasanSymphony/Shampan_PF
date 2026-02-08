using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ApplicantMarksVM
    {
        public int Id { get; set; }
        public string ApplicantId { get; set; }
        public string UserName { get; set; }
        public int Marks { get; set; }
        public string JobId { get; set; }
    }
}
