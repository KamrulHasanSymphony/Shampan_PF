using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class BlockJobSeekerVM
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public int EmployerId { get; set; }
        public string FullName { get; set; }
        public string PhotoName { get; set; }
        public string CVName { get; set; }
        public int PresentDistrictId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public int JobCategoryId { get; set; }
        public string JobLevel { get; set; }
        public string JobNature { get; set; }
        public string CareerObjective { get; set; }
        public string LastSchool { get; set; }
        public string LastDegree { get; set; }
        public string EducationSubject { get; set; }
        public string LastCompany { get; set; }
        public string ExperianceYear { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        public string BlockStatus { get; set; }
    }
}
