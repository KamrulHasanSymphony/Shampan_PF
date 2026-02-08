using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.WPPF
{
    public class AutoJournalSetupVM
    {
        public int Id { get; set; }
        [Display(Name = "Journal For")]
        public string JournalFor { get; set; }
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }
        public string Nature { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
        public int COAID { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

        public string BranchId { get; set; }
        public string Operation { get; set; }

        public string Name { get; set; }
        public string COAName { get; set; }
    }
}
