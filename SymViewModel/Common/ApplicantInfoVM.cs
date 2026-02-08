using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class ApplicantInfoVM
    {
        public string Id { get; set; }
        public string Jobid { get; set; }
        public string JobTitle { get; set; }
        [Display(Name = "Applicant Name")]
        public string ApplicantName { get; set; }
        public string Designation { get; set; }
        [Display(Name = "Present Address")]
        public string PresentAddress { get; set; }
        [Display(Name = "Permanent Address")]
        public string PermanentAddress { get; set; }
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }
        public string Email { get; set; }
        [Display(Name = "Email Text")]
        public string EmailText { get; set; }
        [Display(Name = "Last Education")]
        public string LastEducation { get; set; }
        public string Gender { get; set; }
        public string Experience { get; set; }       
        [Display(Name = "Notice Period")]
        public string NoticePeriod { get; set; }
        [Display(Name = "Are you still studying?")]
        public string Studying { get; set; }
        [Display(Name = "Present Salary")]
        public string PresentSalary { get; set; }
        [Display(Name = "Expected Salary")]
        public string ExpectedSalary { get; set; }
        [Display(Name = "File Attach")]
        public string AttachmentFile { get; set; }
        [Display(Name = "Career Objective")]
        public string CoverLetter { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public bool IsApproved { get; set; }
        [Display(Name = "Interview Date")]
        public string InterviewDate { get; set; }
         [Display(Name = "Interview Time")]
        public string InterviewTime { get; set; }
        [Display(Name = "Written Marks")]
        public string InterviewWrittenMarks { get; set; }
        [Display(Name = "Viva Marks")]
        public string InterviewVivaMarks { get; set; }

        public string HRDesignation { get; set; }
        [Display(Name="Recent Company")]
        public string RecentCompany { get; set; }
        public bool IsConfirmed { get; set; }

        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }

        public bool IsShortlisted { get; set; }
        public string Type { get; set; }

         [Display(Name = "Employment History")]
        public string EmploymentHistory { get; set; }
        [Display(Name = "Academic Qualification")]
        public string AcademicQualification { get; set; }
        [Display(Name = "Professional Qualification")]
        public string ProfessionalQualification { get; set; }
        [Display(Name = "Looking For")]
        public string LookingFor { get; set; }
        [Display(Name = "Available For")]
        public string AvailableFor  { get; set; }
        [Display(Name = "Father Name")]
        public string FatherName { get; set; }
        [Display(Name = "Mother Name")]
        public string MotherName { get; set; }
        [Display(Name = "Date Of Birth")]
        public string DateOfBirth { get; set; }
        [Display(Name = "Marital Status")]
        public string MaritalStatus  { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup  { get; set; }
        public string ImageFileName { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string FaceBook { get; set; }
        public string linkedIn { get; set; }
        public string VideoCv { get; set; }

        public string ExamTitle { get; set; }
        public string Institute { get; set; }
        public string Skill { get; set; }
        public string SkillDescription { get; set; }
        public string Status { get; set; }
      
        public EducationVM educationVM { get; set; }
        public List<EducationVM> educationVMS { get; set; }

        public ProfessionalQualificationVM professionalQualificationVM { get; set; }
        public List<ProfessionalQualificationVM> professionalQualificationVMS { get; set; }

        public ApplicantTrainingVM applicantTrainingVM { get; set; }
        public List<ApplicantTrainingVM> applicantTrainingVMS { get; set; }

        public ApplicantLanguageVM applicantLanguageVM { get; set; }
        public List<ApplicantLanguageVM> applicantLanguageVMS { get; set; }

        public ApplicantEmployeementHistoryVM applicantEmployeementHistoryVM { get; set; }
        public List<ApplicantEmployeementHistoryVM> applicantEmployeementHistoryVMS { get; set; }

        public ApplicantSkillVM applicantSkillVM { get; set; }
        public List<ApplicantSkillVM> applicantSkillVMS { get; set; }

        public ApplicantMarksVM ApplicantMarksVM { get; set; }
        public List<ApplicantMarksVM> ApplicantMarksVMS { get; set; }

        public ApplicantSalaryVM ApplicantSalaryVM { get; set; }
        public List<ApplicantSalaryVM> ApplicantSalaryVMS { get; set; }

        public ApplicatTotalMarksVM ApplicatTotalMarksVM { get; set; }
        public List<ApplicatTotalMarksVM> ApplicatTotalMarksVMS { get; set; }

        public List<MatchPerventsVM> MatchPerventsVMs { get; set; }


        public string Age { get; set; }
    }  
}
