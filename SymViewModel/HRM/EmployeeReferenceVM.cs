using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
    public class EmployeeReferenceVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Relation { get; set; }

        //To Show Different (In View) Using the Same Field
        [Display(Name = "Village/House & Road")]
        public string Address { get; set; }

        [Display(Name = "Post Office & Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Police Station")]
        public string City { get; set; }


        public string District { get; set; }
        public string Division { get; set; }
        public string Country { get; set; }

        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string FileName { get; set; }
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

        public string Designation { get; set; }
        [Display(Name = "WorkAddress (Organization)")]
        public string WorkAddress { get; set; }
        [Display(Name = "Year of Acquaintance")]
        public int YearOfAcquaintance { get; set; }


    }
}
