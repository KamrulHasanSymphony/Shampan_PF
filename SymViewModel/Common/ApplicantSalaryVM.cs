using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ApplicantSalaryVM
    {
        public int Id { get; set; }
        public string ApplicantId { get; set; }
        public string Date { get; set; }
        public int Salary { get; set; }
        public string JobId { get; set; }
    }
}
