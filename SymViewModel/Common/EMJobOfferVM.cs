using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class EMJobOfferVM
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public int EmployerId { get; set; }
        public int JobId { get; set; }
        public string OfferType { get; set; }
        public string FullName { get; set; }
        public string PhotoName { get; set; }
        public int PresentDistrictId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public int JobCategoryId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Vacancy { get; set; }
        public string RequiruitmentDate { get; set; }
    }
}
