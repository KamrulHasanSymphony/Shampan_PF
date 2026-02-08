using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web;
using System.Web.Security;

namespace SymWebUI.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
    public class EmailSettings
    {
        public string MailToAddress { get; set; }
        public string MailFromAddress { get; set; }
        public bool USsel = true;
        public string Password { get; set; }
        public string UserName { get; set; }
        public string ServerName { get; set; }
        public string Name { get; set; }
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public string FileName { get; set; }
    }

    public class EmailSettingsSym
    {
        public string MailToAddress = "symphonyleave@gmail.com";
        public string MailFromAddress = "symphonyleave@gmail.com";
        public bool USsel = true;
        public string Password = "vdki rypf aump rrvt";
        public string UserName = "symphonyleave@gmail.com";
        public string ServerName = "smtp.gmail.com";
        public string Name { get; set; }
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
    }
    public class EmailSettingsTIB
    {
        public string MailToAddress { get; set; }
        public string MailFromAddress = "payrolltib@ti-bangladesh.info";
        public bool USsel = false;
        public string Password = "stcyjzvwympkjrwb";
        public string UserName = "payrolltib@ti-bangladesh.info";

        public string ServerName = "smtp.office365.com";
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public HttpPostedFileBase fileUploader { get; set; }
        public string FileName { get; set; }
    }

    public class EmailSettingsGmail
    {
        public string MailToAddress { get; set; }
        public string MailFromAddress = "bollorelogisticsbdoffice@gmail.com";
        public bool USsel = true;
        public string Password = "ouov xqhh mgwp nrxy";
        public string UserName = "bollorelogisticsbdoffice@gmail.com";
        public string ServerName = "smtp.gmail.com";
        //public string ServerName = "smtp-mail.outlook.com";
        //public string ServerName = "smtp.mail.yahoo.com";
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public HttpPostedFileBase fileUploader { get; set; }
        public string FileName { get; set; }
    }
      
    public class EmailSettingsBollore
    {
        public string MailToAddress { get; set; }
        public string MailFromAddress = "_svc_bbddac_0004@btl.bollore.com";
        public bool USsel = true;
        public string Password = "x3FJq68PeBb2t7R4Gcn9";
        public string UserName = "_svc_bbddac_0004";
        public string ServerName = @"csi-btl-smtp.bollore-logistics.com";
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public HttpPostedFileBase fileUploader { get; set; }
        public string FileName { get; set; }
    }
    public class EmailSettingsBolloreOld
    {
        public string MailToAddress { get; set; }
        public string MailFromAddress = "BD.HRMS@BOLLORE.COM";
        public bool USsel = true;
        public string Password = "k9M4Ses5J8EtAm27Bwo3D6Cz";
        public string UserName = "BD.HRMS@BOLLORE.COM";
        //csi-btl-smtp.bollore-logistics.com
        public string ServerName = @"smtp.bollore-logistics.com";
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public HttpPostedFileBase fileUploader { get; set; }
        public string FileName { get; set; }
    }
}
