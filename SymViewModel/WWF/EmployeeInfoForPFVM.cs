using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymViewModel.WWF
{
    public class EmployeeInfoForPFVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Project { get; set; }
        public string Section { get; set; }
        [Display(Name = "Date Of Birth")]
        public string DateOfBirth { get; set; }
        [Display(Name = "Join Date")]
        public string JoinDate { get; set; }
        [Display(Name = "Resign Date")]
        public string ResignDate { get; set; }
        public string ResignReason { get; set; }        
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }        
        public bool IsArchive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }
        public string PhotoName { get; set; }
        [Display(Name = "Nominee Name")]
        public string NomineeName { get; set; }
        [Display(Name = "Nominee Date of Birth")]
        public string NomineeDateofBirth { get; set; }
        [Display(Name = "Relation")]
        public string NomineeRelation { get; set; }
        [Display(Name = "Address")]
        public string NomineeAddress { get; set; }
        [Display(Name = "District")]
        public string NomineeDistrict { get; set; }
        [Display(Name = "Division")]
        public string NomineeDivision { get; set; }
        [Display(Name = "Country")]
        public string NomineeCountry { get; set; }
        [Display(Name = "City")]
        public string NomineeCity { get; set; }
        [Display(Name = "Postal Code")]
        public string NomineePostalCode { get; set; }
        [Display(Name = "Phone")]
        public string NomineePhone { get; set; }
        [Display(Name = "Mobile")]
        public string NomineeMobile { get; set; }
        [Display(Name = "Birth Certificate No")]
        public string NomineeBirthCertificateNo { get; set; }
        [Display(Name = "Fax")]
        public string NomineeFax { get; set; }
        [Display(Name = "File Name")]
        public string NomineeFileName { get; set; }
        [Display(Name = "NID")]
        public string NomineeNID { get; set; }
        [Display(Name = "Nominee Remarks")]
        public string NomineeRemarks { get; set; }

        public HttpPostedFileBase File { get; set; }
        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string SectionId { get; set; }
        public string ProjectId { get; set; }
        public string EmployeeId { get; set; }
        public string GradeId { get; set; }
        public string Email { get; set; }
        [Display(Name = "Personal Contact No")]
        public string ContactNo { get; set; }
        public string BranchId { get; set; }

        [Display(Name = "Official Contact No")]
        public string OfficialContactNo { get; set; }
        [Display(Name = "NID")]
        public string EmployeeNID { get; set; }
         [Display(Name = "TIN")]
        public string EmployeeTIN { get; set; }
         [Display(Name = "Father Name")]
        public string FathersName { get; set; }
         [Display(Name = "Mother Name")]
        public string MothersName { get; set; }
         [Display(Name = "Spouse Name")]
        public string SpouseName { get; set; }
        
         [Display(Name = "Bank Account Number")]
        public string EmployeeBankAccountNumber { get; set; }
         [Display(Name = "Present Address")]
        public string PresentAddress { get; set; }
         [Display(Name = "Parmanent Adderss")]
        public string ParmanentAdderss { get; set; }

         [Display(Name = "Bank Account Number")]
        public string NomineeBankAccountNumber { get; set; }
         [Display(Name = "Nominee Share(%)")]
        public string NomineeShare { get; set; }
         [Display(Name = "Bank Name")]
         public int EmployeeBankNameId { get; set; }
         [Display(Name = "Bank Name")]
         public int NomineeBankNameId { get; set; }
    }
}
