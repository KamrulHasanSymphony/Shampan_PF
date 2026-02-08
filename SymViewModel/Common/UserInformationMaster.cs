using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class UserRollVM
    {
        [Display(Name = "User ID")]
        public string UserID { get; set; }
        [Display(Name = "Form ID")]
        public string FormID { get; set; }
        public string Access { get; set; }
        [Display(Name = "Last Login DateTime")]
        public string LastLoginDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        [Display(Name = "Line ID")]
        public string LineID { get; set; }
        [Display(Name = "Form Name")]
        public string FormName { get; set; }
        [Display(Name = "Post Access")]
        public string PostAccess { get; set; }
        [Display(Name = "Add Access")]
        public string AddAccess { get; set; }
        [Display(Name = "EditAccess")]
        public string EditAccess { get; set; }
    }
}
