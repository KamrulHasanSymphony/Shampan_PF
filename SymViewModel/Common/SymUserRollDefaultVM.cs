using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class SymUserRollDefaultVM
    {
        public string Id { get; set; }
        public int BranchId { get; set; }
        public string UserId { get; set; }
        public string Module { get; set; }
        public string Menu { get; set; }
        [Display(Name = "Index")]
        public bool IsIndex { get; set; }
        [Display(Name = "Add")]
        public bool IsAdd { get; set; }
        [Display(Name = "Edit")]
        public bool IsEdit { get; set; }
        [Display(Name = "Delete")]
        public bool IsDelete { get; set; }
        [Display(Name = "Report")]
        public bool IsReport { get; set; }
        [Display(Name = "Process")]
        public bool IsProcess { get; set; }
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
        public List<SymUserRollVM> SymUserRollvm { get; set; }
    }
}
