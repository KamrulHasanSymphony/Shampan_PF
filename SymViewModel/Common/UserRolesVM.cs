using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class UserRolesVM
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Display(Name = "User Info")]
        public string UserInfoId { get; set; }
        [Display(Name = "Role Info")]
        public string RoleInfoId { get; set; }
        public bool IsArchived { get; set; }
    }
}
