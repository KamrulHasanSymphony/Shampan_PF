using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class JobCircularVM : BaseEntity
   {
       public string Id { get; set; }
       public int BranchId { get; set; }
       [Display(Name = "Job Title")]
       public string JobTitle { get; set; }
       [Display(Name = "Designation")]
       public string DesignationId { get; set; }
       [Display(Name = "Designation Name")]
       public string DesignationName { get; set; }
       public string Deadline { get; set; }
       [Display(Name = "Experience")]
       public string Expriance	 { get; set; }
       [Display(Name = "Job Responsibilities & Contex")]        
       public string Description { get; set; }

       [Display(Name = "Job Location")]
       public string JobLocation { get; set; }
       public string Vacancy { get; set; }
       [Display(Name = "Employment Status")]
       public string EmploymentStatus { get; set; }
       public string Workplace { get; set; }
       [Display(Name = "Salary Min")]
       public string SalaryMin { get; set; }
       [Display(Name = "Salary Max")]
       public string SalaryMax { get; set; }
       [Display(Name = "Age Min")]
       public string AgeMin { get; set; }
       [Display(Name = "Age Max")]
       public string AgeMax { get; set; }
       [Display(Name = "Degree Name")]
       public string DegreeName { get; set; }
       [Display(Name = "Contact Person")]
       public string ContactPerson { get; set; }
       [Display(Name = "HR Designation")]
       public string HRDesignation { get; set; }
       [Display(Name = "Contact No")]
       public string ContactNo { get; set; }
       [Display(Name = "Email Send To")]
       public string EmailSendTo { get; set; }

        [Display(Name = "Additional Requirement")]
       public string AdditionalRequirement { get; set; }
       [Display(Name = "Should have experience in the following areas")]
       public string ShouldHaveArea { get; set; }
        [Display(Name = "Should have experience in the following business")]
       public string ShouldHaveBusiness { get; set; }
        [Display(Name = "Skill & Experties")]
       public string SkillExperties { get; set; }
       public string Compensation { get; set; }
        [Display(Name = "Read Before Apply")]
       public string ReadBeforeApply { get; set; }
        [Display(Name = "Benifit")]
       public string Benifit { get; set; }
       public ReqEmployeeEducationVM educationVM { get; set; }

       public int TotalApplicant { get; set; }      
       public string CreatedAt { get; set; }
       public string JobId { get; set; }

       [Display(Name = "Applicant Name")]
       public string ApplicantName { get; set; }
       [Display(Name = "Present Address")]
       public string PresentAddress { get; set; }
       [Display(Name = "Last Education")]
       public string LastEducation { get; set; }
       [Display(Name = "Expected Salary")]
       public string ExpectedSalary { get; set; }    
       public string Phone { get; set; }
       public string Experience { get; set; }
       public string Email { get; set; }
       public string RecentCompany { get; set; }
       public string ImageFileName { get; set; }

       public string Confirmed { get; set; }
       public string Shortlisted { get; set; }
       public string Viewed { get; set; }
       public string NotViewed { get; set; }
       public ApplicatTotalMarksVM ApplicatTotalMarksVM { get; set; }
       public List<ApplicatTotalMarksVM> ApplicatTotalMarksVMS { get; set; }
     
   
   }
}