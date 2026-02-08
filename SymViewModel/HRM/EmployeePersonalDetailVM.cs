using System.ComponentModel.DataAnnotations;

namespace SymViewModel.HRM
{
    public class EmployeePersonalDetailVM
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [Display(Name = "Other ID")]
        public string OtherId { get; set; }
        [Display(Name = "Gender")]
        [Required]
        public string Gender_E { get; set; }
        [Display(Name = "Father Name")]
        [Required]
        public string FatherName { get; set; }
        [Display(Name = "Mother Name")]
        [Required]
        public string MotherName { get; set; }
        [Display(Name = "Spouse Name")]
        [Required]
        public string SpouseName { get; set; }
        [Display(Name = "Personal Contact No")]
        [Required]
        public string PersonalContactNo { get; set; }
        [Display(Name = "Corporate Contact No")]
        [Required]
        public string CorporateContactNo { get; set; }
        [Display(Name = "Corp. No. Credit Limit")]
        [Required]
        public decimal CorporateContactLimit { get; set; }
        [Display(Name = "Marital Status")]
        [Required]
        public string MaritalStatus_E { get; set; }
        [Display(Name = "Nationality")]
        [Required]
        public string Nationality_E { get; set; }
        [Display(Name = "Date Of Birth")]
        [Required]
        public string DateOfBirth { get; set; }
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }
        public bool Smoker { get; set; }
        [Display(Name = "NID/Birth Certificate")]
        public string NID { get; set; }
        public string NIDFile { get; set; }
        [Display(Name = "Passport Number")]
        public string PassportNumber { get; set; }
        [Display(Name = "Expiry Date")]
        public string ExpiryDate { get; set; }
        public string Religion { get; set; }
        [Display(Name = "TIN")]
        public string TIN { get; set; }
        [Display(Name = "TIN File")]
        public string TINFile { get; set; }
        [Display(Name = "Finger Print")]
        public string FingerprintFile { get; set; }
        public string Fingerprint { get; set; }
        [Display(Name = "Disability")]
        public bool IsDisable { get; set; }
        [Display(Name = "Kinds Of Disability")]
        public string KindsOfDisability { get; set; }
        [Display(Name = "Disability File")]
        public string DisabilityFile { get; set; }
        public string PassportFile { get; set; }
        public string NIDFD { get; set; }
        public string DisabilityFileName { get; set; }
        public string PassportFileName { get; set; }
        public string NIDFDName { get; set; }
        public string SignatureFile { get; set; }
        public string Signature { get; set; }

        public string Email { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup_E { get; set; }

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


        [Display(Name = "Place Of Birth")]
        public string PlaceOfBirth { get; set; }
        [Display(Name = "Marriage Date")]
        public string MarriageDate { get; set; }
        [Display(Name = "Spouse Profession")]
        public string SpouseProfession { get; set; }
        [Display(Name = "Spouse Date of Birth")]
        public string SpouseDateOfBirth { get; set; }
        [Display(Name = "Spouse Blood Group")]
        public string SpouseBloodGroup { get; set; }

        [Display(Name = "No.Children")]
        public string NoChildren { get; set; }
        [Display(Name = "Height (Feet)")]
        public string Heightft { get; set; }
        [Display(Name = "Height (Inch)")]
        public string HeightIn { get; set; }
        [Display(Name = "Weight (KG)")]
        public string Weight { get; set; }
        [Display(Name = "Chest (Inch)")]
        public string ChestIn { get; set; }
     

        public string HRMSCode { get; set; }
        public string WDCode { get; set; }
        public string TPNCode { get; set; }
        public string PersonalEmail { get; set; }
        public string DisabilityType { get; set; }
        public bool IsVaccineDose1Complete { get; set; }
        [Display (Name="Vaccine File 1")]
        public string VaccineFile1 { get; set; }
        public string VaccineFile2 { get; set; }
        [Display(Name = "Vaccine File 2")]
        public string VaccineFiles2 { get; set; }
        [Display(Name = "Vaccine File 3")]
        public string VaccineFile3 { get; set; }
        public string VaccineDose1Date { get; set; }
        public string VaccineDose1Name { get; set; }
        public bool IsVaccineDose2Complete { get; set; }
        public string VaccineDose2Date { get; set; }
        public string VaccineDose2Name { get; set; }
        public bool IsVaccineDose3Complete { get; set; }
        public string VaccineDose3Date { get; set; }
        public string VaccineDose3Name { get; set; }










    }
}
