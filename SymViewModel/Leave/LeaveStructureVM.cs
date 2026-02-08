using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Leave
{
 public   class LeaveStructureVM
 {
     [Required]
     public int Id { get; set; }
     [Required]
     public int BranchId { get; set; }
     [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
     public string Code { get; set; }
     [Required]
     public string Name { get; set; }


     [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
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
     public List<LeaveStructureDetailVM> leaveStructureDetailVMs { get; set; }
    }
}
