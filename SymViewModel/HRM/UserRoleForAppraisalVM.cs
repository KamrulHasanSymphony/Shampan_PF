using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class UserRoleForAppraisalVM
    {
        public string Id { get; set; }
     
        public string Logid { get; set; }
        public string FullName { get; set; }
      
        [Display(Name = "Own")]
        public bool IsOwn { get; set; }
        [Display(Name = "Team Lead")]
        public bool IsTeamLead { get; set; }     
    }
}
