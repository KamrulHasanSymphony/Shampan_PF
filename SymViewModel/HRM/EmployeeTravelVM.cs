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
    public class EmployeeTravelVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Display(Name = "Travel Type")]
        public string TravelType_E { get; set; }
        [Required]
        [Display(Name = "Travel From Address")]
        public string TravelFromAddress { get; set; }
        [Required]
        [Display(Name = "Travel To Address")]
        public string TravelToAddress { get; set; }

        [Display(Name = "From Date")]
        public string FromDate { get; set; }

        [Display(Name = "To Date")]
        public string ToDate { get; set; }
        [Required]
        [Display(Name = "From Time")]
        public string FromTime { get; set; }
        [Required]
        [Display(Name = "To Time")]
        public string ToTime { get; set; }
        [Display(Name = "Bill/Voucher")]
        public string FileName { get; set; }
        public string TravleTime { get; set; }
        [Display(Name = "Remarks")]
        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }
        [Display(Name = "Allowances(TK)")]
        public decimal Allowances { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        [Display(Name = "Issue Date")]
        public string IssueDate { get; set; }
        [Display(Name = "Expiry Date")]
        public string ExpiryDate { get; set; }
        public string Country { get; set; }
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }

        [Display(Name = "Embassy Name")]
        public string EmbassyName { get; set; }

        





    }
}
