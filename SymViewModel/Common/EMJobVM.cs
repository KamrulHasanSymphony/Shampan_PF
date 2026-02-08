using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class EMJobVM
    {
        public int Id { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public int Vacancy { get; set; }
        public string RequiruitmentDate { get; set; }
        public string JobDescription { get; set; }
        public string FullName { get; set; }
        public int PresentDistrictId { get; set; }
        public int JobCategoryId { get; set; }
        public string Website { get; set; }
    }
}
