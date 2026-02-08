using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymViewModel.HRM
{
   public class EmployeeFilesVM
    {

        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        public string Filepath { get; set; }

       //========Personal Detalies===========

        [Display(Name = "Vaccine File 1")]
        public string EmployeePersonalDetail_VaccineFile1 { get; set; }
        [Display(Name = "Vaccine File 2")]
        public string EmployeePersonalDetail_VaccineFiles2 { get; set; }
        [Display(Name = "Vaccine File 3")]
        public string EmployeePersonalDetail_VaccineFile3 { get; set; }
        [Display(Name = "TIN File")]
        public string EmployeePersonalDetail_TINFiles { get; set; }
        [Display(Name = "Finger Print")]
        public string EmployeePersonalDetail_Fingerprint { get; set; }
        [Display(Name = "Disability")]
        public bool IsDisable { get; set; }
        [Display(Name = "Kinds Of Disability")]
        public string KindsOfDisability { get; set; }
        [Display(Name = "Disability File")]
        public string EmployeePersonalDetail_DisabilityFile { get; set; }
       [Display(Name = "Passport File")]
        public string EmployeePersonalDetail_PassportFile { get; set; }
        [Display(Name = "NID File")]
        public string EmployeePersonalDetail_NIDFile { get; set; }
        public string DisabilityFileName { get; set; }
        public string PassportFileName { get; set; }
        [Display(Name = "Signature File")]
        public string SignatureFiles { get; set; }
        public string FileName { get; set; }

        [StringLength(450, ErrorMessage = "Remarks cannot be longer than 450 characters.")]
        public string Remarks { get; set; }


        //========Employee Education===========
        [Display(Name = "Certificate")]
        public string Education_Certificate { get; set; }
        [Display(Name = "Language")]
        public string Language { get; set; }
        [Display(Name = "Experience")]
        public string Experience { get; set; }
        [Display(Name = "Extra Curriculum Activity")]
        public string ExtraCurriculumActivity { get; set; }


        //========Employee Nominee===========
        [Display(Name = "N.VaccineFile 1")]
        public string EmployeeNominee_VaccineFile1 { get; set; }
        [Display(Name = "N.VaccineFile 2")]
        public string EmployeeNominee_VaccineFile2 { get; set; }
        [Display(Name = "N.VaccineFile 3")]
        public string EmployeeNominee_VaccineFile3 { get; set; }

        //========Employee Dependent===========
        [Display(Name = "Education Certificate")]
        public string edu_Certificate { get; set; }
        [Display(Name = "Language Achivement")]
        public string Lng_Achivement { get; set; }
        [Display(Name = "Experience Certificate")]
        public string Experience_Certificate { get; set; }
        [Display(Name = "Extra FileName")]
        public string Extra_FileName { get; set; }

        //========Employee Dependent===========
        [Display(Name = "D.VaccineFile 1")]
        public string Employeedependent_VaccineFile1 { get; set; }
        [Display(Name = "D.VaccineFile 2")]
        public string Employeedependent_VaccineFile2 { get; set; }
        [Display(Name = "D.VaccineFile 3")]
        public string Employeedependent_VaccineFile3 { get; set; }
        [Display(Name = "Passport Visa")]
        public string PassportVisa { get;set;}
       [Display(Name = "Bill/Voucher")]
        public string BillVoucher { get; set; }
       [Display(Name = "Asset File Name")]
        public string AssetFileName { get; set; }
       [Display(Name = "Certificate")]
        public string Certificate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }

    }
}
