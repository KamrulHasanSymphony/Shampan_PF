using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class CompanyProfileVM
    {
        //        
        [Display(Name = "Company ID")]
        public string CompanyID { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Company LegalName")]
        public string CompanyLegalName { get; set; }
        [Display(Name = "Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        [Display(Name = "Telephone No")]
        public string TelephoneNo { get; set; }
        [Display(Name = "Fax No")]
        public string FaxNo { get; set; }
        public string Email { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Display(Name = "ContactPerson Designation")]
        public string ContactPersonDesignation { get; set; }
        [Display(Name = "ContactPerson Telephone")]
        public string ContactPersonTelephone { get; set; }
        [Display(Name = "ContactPerson Email")]
        public string ContactPersonEmail { get; set; }
        [Display(Name = "TIN No.")]
        public string TINNo { get; set; }
        [Display(Name = "VAT Registration No.")]
        public string VatRegistrationNo { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
        [Display(Name = "Start DateTime")]
        public string StartDateTime { get; set; }
        [Display(Name = "FYear Start")]
        public string FYearStart { get; set; }
        [Display(Name = "FYear End")]
        public string FYearEnd { get; set; }
        public string Tom { get; set; }       //encrypted companyName
        public string Jary { get; set; }      //encrypted CompanyLegalName
        public string Miki { get; set; }      //encrypted VatRegistrationNo
        public string Mouse { get; set; }     //encrypted ProcessorId

    }
}
