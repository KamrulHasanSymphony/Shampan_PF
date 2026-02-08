using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class SettingsVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "Setting Group")]
        public string SettingGroup { get; set; }
        [Display(Name = "Setting Name")]
        public string SettingName { get; set; }
        [Display(Name = "Setting Value")]
        public string SettingValue { get; set; }
        [Display(Name = "Setting Type")]
        public string SettingType { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Active")] 
        public bool IsActive { get; set; }       
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
    }

    public class RootApiModel
    {
        public string KeyValue { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }


    }

}
