using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class UserGroupVM
    {
        public int Id { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        public string Remarks { get; set; }
        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public string Name { get; set; }
        [Display(Name = "Super")]
        public bool IsSuper { get; set; }
        public List<SymUserRollVM> SymUserRollVMs { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsHRM { get; set; }

        public bool IsAttendance { get; set; }

        public bool IsPayroll { get; set; }

        public bool IsTAX { get; set; }

        public bool IsPF { get; set; }

        public bool IsGF { get; set; }

        public bool IsESS { get; set; }
    }
}
