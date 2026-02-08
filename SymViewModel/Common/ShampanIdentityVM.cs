using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
   public class ShampanIdentityVM
    {
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public string BranchId { get; set; }
        public string UserId { get; set; }
        public string Module { get; set; }
        public string Menu { get; set; }

        [Display(Name = "Index")]
        public string IsIndex { get; set; }
        [Display(Name = "Add")]
        public string IsAdd { get; set; }
        [Display(Name = "Edit")]
        public string IsEdit { get; set; }
        [Display(Name = "Delete")]
        public string IsDelete { get; set; }
        [Display(Name = "Report")]
        public string IsReport { get; set; }
        [Display(Name = "Process")]
        public string IsProcess { get; set; }

    }
}
