using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class SettingVM
    {
        public string SettingId { get; set; }
        [Display(Name = "Setting Group")]
        public string SettingGroup { get; set; }
        [Display(Name = "Setting Name")]
        public string SettingName { get; set; }
        [Display(Name = "Setting Value")]
        public string SettingValue { get; set; }
        [Display(Name = "Setting Type")]
        public string SettingType { get; set; }
        [Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

    }
}
