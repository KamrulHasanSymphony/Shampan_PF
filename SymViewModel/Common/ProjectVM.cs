using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Common
{
   public class ProjectVM
   {
       [Required]
       public string Id { get; set; }
       [Required]
       public int BranchId { get; set; }
       [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
       public string Code { get; set; }
       [Required]
       public string Name { get; set; }

       [Display(Name = "Start")]
       public string Startdate { get; set; }
       [Display(Name = "End")]
       public string EndDate { get; set; }
       [Required]
       [Display(Name = "Manpower")]
       public int ManpowerRequired { get; set; }
       [Display(Name = "Contact Name")]
       public string ContactPerson { get; set; }
       [Display(Name = "Designation")]
       public string ContactPersonDesignation { get; set; }
       public string Address { get; set; }
       public string District { get; set; }
       public string Division { get; set; }
       public string Country { get; set; }
       public string City { get; set; }
       [Display(Name = "Postal Code")]
       public string PostalCode { get; set; }
       public string Phone { get; set; }
       public string Mobile { get; set; }
       public string Fax { get; set; }

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
       public HttpPostedFileBase File { get; set; }
    }
}
