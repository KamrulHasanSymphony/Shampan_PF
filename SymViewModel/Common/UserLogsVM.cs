using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SymViewModel.Common
{
    public class UserLogsVM
    {
        public string Id { get; set; }
          [Display(Name = "Branch Name")] 
        public int BranchId { get; set; }
        [Display(Name="User Id")]
        public string LogID { get; set; }
        [Display(Name = "Computer Name")] 
        public string ComputerName { get; set; }
        [Display(Name = "Computer Login UserName")] 
        public string ComputerLoginUserName { get; set; }
        [Display(Name = "Computer IP Address")] 
        public string ComputerIPAddress { get; set; }
        [Display(Name = "Location Address")]
        public string Location { get; set; }
        [Display(Name = "Software User")] 
        public string SoftwareUserId { get; set; }
        [Display(Name = "Software User")] 
        public string SoftwareUserName { get; set; }
        [Display(Name = "Session Date")] 
        public string SessionDate { get; set; }
        [Display(Name = "LogIn DateTime")] 
        public string LogInDateTime { get; set; }
        [Display(Name = "LogOut DateTime")] 
        public string LogOutDateTime { get; set; }
        [Display(Name="Full Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password required")]
        [CompareAttribute("Password", ErrorMessage = "Password doesn't match.")]
        public string confirmPassword { get; set; }
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        [Display(Name = "Verification Code")] 
        public string VerificationCode { get; set; }
        public string EmployeeId { get; set; }
        [Display(Name = "Employee Code")] 
        public string EmployeeCode { get; set; }

        [Display(Name = "ESS")] 
        public bool IsESS { get; set; }
        [Display(Name = "Admin")] 
        public bool IsAdmin { get; set; }
        [Display(Name = "User Type")] 
        public string UserType { get; set; }
        [Display(Name = "Active")] 
        public bool IsActive { get; set; }       
        public bool IsVerified { get; set; }
        public bool IsArchived { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedFrom { get; set; }
        public string LastUpdateBy { get; set; }
        public string LastUpdateAt { get; set; }
        public string LastUpdateFrom { get; set; }


        ////////
        [Display(Name = "Return Url")] 
        public string ReturnUrl  { get; set; }
        [Display(Name = "DataBase Name")] 
        public string DataBaseName { get; set; }
      
        [Display(Name = "Sym Area")] 
        public string symArea { get; set; }
        [Display(Name = "Group Name")] 
        public int GroupId { get; set; }

        [Display(Name = "Photo Name")] 
        public string PhotoName { get; set; }

        public bool IsHRM { get; set; }

        public bool IsAttenance { get; set; }

        public bool IsPayroll { get; set; }

        public bool IsTAX { get; set; }

        public bool IsPF { get; set; }

        public bool IsGF { get; set; }

        public bool IsApprove { get; set; }
    }
}
