using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.Common
{
   public class BranchVM
    {

        [Required]
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
       
        public string Address { get; set; }
        public string District { get; set; }
        public string Division { get; set; }
        public string Country { get; set; }
        public string City { get; set; }        
		[Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        [Required]
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
