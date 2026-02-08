using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SymViewModel.Common
{
    public class ApplicantEmployeementHistoryVM
    {
        public int Id { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Company Business")]
        public string CompanyBusiness { get; set; }
        [Display(Name = "Designation")]
        public string ApplicantDesignation { get; set; }
        [Display(Name = "Department")]
        public string ApplicantDepartment { get; set; }
        [Display(Name = "Employment Period")]
        public string EmploymentPeriod { get; set; }
        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Display(Name = "Currently Working")]
        public string CurrentlyWorking { get; set; }
        public string Responsibilities { get; set; }
        [Display(Name = "Area Of Experience")]
        public string AreaOfExperience { get; set; }
        [Display(Name = "Company Location")]
        public string CompanyLocation { get; set; }
        public string ApplicantId { get; set; }
    }
}
