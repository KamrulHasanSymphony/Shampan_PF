using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.Common
{
    public class CompanyVM
    {
        [Required]
        public int Id { get; set; }
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Enter 4 to 20 characters!")]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
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

        [Display(Name = "Tax Id")]
        public string TaxId { get; set; }
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }
        [Display(Name = "Mail")]
        public string Email { get; set; }
        [Display(Name = "Number Of Employees")]
        [Required]
        public int NumberOfEmployees { get; set; }

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

        [Display(Name = "Year")]
        public string Year { get; set; }
        [Display(Name = "Year Start")]
        public string YearStart { get; set; }

        //Only for data pass
        [Display(Name = "Current Branch")]
        public int CurrentBranch { get; set; }
    }
}
