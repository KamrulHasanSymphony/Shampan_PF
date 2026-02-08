using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
   public class Appraisal360VM
    {
        public int Id { get; set; }
        [Display(Name = "Department Id")]
        public string DepartmentId { get; set; }
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }
        [Display(Name = "FeedBack Year")]
        public int FeedBackYear { get; set; }
        [Display(Name = "FeedBack Month Id")]
        public int FeedBackMonthId { get; set; }
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; }
        [Display(Name = "User Id")]
        public string UserId { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "FeedBack User Id")]
        public string FeedBackUserId { get; set; }
        [Display(Name = "FeedBack User Name")]
        public string FeedBackUserName { get; set; }
        [Display(Name = "Feedback Date")]
        public DateTime FeedbackDate { get; set; }
        [Display(Name = "Feedback Competed")]
        public bool IsFeedbackCompeted { get; set; }
        [Display(Name = "User")]
        public bool IsUser { get; set; }
        [Display(Name = "Supervisor")]
        public bool IsSupervisor { get; set; }
        [Display(Name = "Department Head")]
        public bool IsDepartmentHead { get; set; }
        [Display(Name = "Management")]
        public bool IsManagement { get; set; }
        [Display(Name = "HR")]
        public bool IsHR { get; set; }

    }


   public class Appraisal360FeedBackVM
   {
       public int Id { get; set; }
        [Display(Name = "Department")]
       public string DepartmentId { get; set; }
        [Display(Name = "FeedBack Year")]
       public string FeedBackYear	 { get; set; }
        [Display(Name = "Period Name")]
       public string PeriodName		 { get; set; }
        [Display(Name = "Department Name")]
       public string DepartmentName	 { get; set; }
        [Display(Name = "Designation Name")]
       public string DesignationName { get; set; }
        [Display(Name = "User Code")]
       public string UserCode	 { get; set; }
        [Display(Name = "User Name")]
       public string UserName	 { get; set; }
        [Display(Name = "Feedback Code")]
       public string FeedbackCode	 { get; set; }
        [Display(Name = "Feedback Name")]
       public string FeedbackName	 { get; set; }
        [Display(Name = "Feedback Competed")]
       public string IsFeedbackCompeted { get; set; }
        [Display(Name = "User")]
       public string UserId	 { get; set; }
        [Display(Name = "FeedBack User")]
       public string FeedBackUserId	 { get; set; }
        [Display(Name = "FeedBack Month")]
       public string FeedBackMonth	 { get; set; }
        [Display(Name = "Appraisal360")]
       public string Appraisal360Id  { get; set; }
        [Display(Name = "Feedback By")]
       public string FeedbackBy { get; set; }
       public List<Appraisal360DetailVM> Details { get; set; }
   }

   public class Appraisal360DetailVM
   {
       public int Id { get; set; }
       [Display(Name = "Department")]
       public string DepartmentId { get; set; }
       [Display(Name = "FeedBack Year")]
       public string FeedBackYear { get; set; }
       [Display(Name = "Period Name")]
       public string PeriodName { get; set; }
       [Display(Name = "Department Name")]
       public string DepartmentName { get; set; }
       [Display(Name = "User Code")]
       public string UserCode { get; set; }
       [Display(Name = "User Name")]
       public string UserName { get; set; }
       [Display(Name = "Feedback Code")]
       public string FeedbackCode { get; set; }
       [Display(Name = "Feedback Name")]
       public string FeedbackName { get; set; }
       [Display(Name = "Feedback Competed")]
       public string IsFeedbackCompeted { get; set; }
       [Display(Name = "User Id")]
       public string UserId { get; set; }
       [Display(Name = "FeedBack User Id")]
       public string FeedBackUserId { get; set; }
       [Display(Name = "FeedBack Month")]
       public string FeedBackMonth { get; set; }
       [Display(Name = "Appraisal360 Id")]
       public string Appraisal360Id { get; set; }
       [Display(Name = "Feedback By")]
       public string FeedbackBy { get; set; }
       [Display(Name = "Question")]
       public string Question { get; set; }
       [Display(Name = "Feedback Value")]
       public string FeedbackValue { get; set; }

   }

}
