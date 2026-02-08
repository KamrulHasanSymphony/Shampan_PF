using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.PF
{
   public class EmployeeNomineeVM
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

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

       [Display(Name = "Post Office")]
       public string PostOffice{ get; set;}

        [Display(Name = "Police Station")]
        public string City { get; set; }


        public string District { get; set; }
        public string Division { get; set; }
        public string Country { get; set; }





        public string Phone { get; set; }
        [Display(Name = "Date of Birth")]
        public string DateofBirth { get; set; }
        [Display(Name = "Birth Certificate.")]
        public string BirthReg { get; set; }

        [Display(Name = "NID No")]
        public string NID { get; set; }

        [Display(Name = "Passport No.")]
        public string Passport { get; set; }
       [Required]
        public string Mobile { get; set; }
        public string Fax { get; set; }
        [Display(Name = "Image")]
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

       //===IsVacccine INFO===

        public bool IsVaccineDose1Complete { get; set; }
        public string VaccineDose1Date { get; set; }
        public string VaccineDose1Name { get; set; }
        public bool IsVaccineDose2Complete { get; set; }
        public string VaccineDose2Date { get; set; }
        public string VaccineDose2Name { get; set; }
        public bool IsVaccineDose3Complete { get; set; }
        public string VaccineDose3Date { get; set; }
        public string VaccineDose3Name { get; set; }

        [Display(Name = "Vaccine File 1")]
        public string VaccineFile1 { get; set; }
        [Display(Name = "Vaccine File 2")]
        public string VaccineFiles2 { get; set; }
        [Display(Name = "Vaccine File 3")]
        public string VaccineFile3 { get; set; }

    }
}
