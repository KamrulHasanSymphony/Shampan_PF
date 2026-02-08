using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SymViewModel.Common
{
    public class CmpanyListVM
    {
        [Display(Name = "Company Sl")]
        public string CompanySl { get; set; }
        [Display(Name = "Company ID")]
        public string CompanyID { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Database Name")]
        public string DatabaseName { get; set; }
        [Display(Name = "Active Status")]
        public string ActiveStatus { get; set; }
        public string Serial { get; set; }
    }

    public class settingVM
    {
        public static DataTable SettingsDT;
    }
    //S123456_
    public class SysDBInfoVMTemp
    {
        public string SysDatabaseName = "SymphonyVATSys";
        public string SysUserName = "sa";// { get; set; }
        public string SysPassword = "S123456_";// { get; set; }
        public string SysdataSource = ".";// { get; set; }

        #region Property

        public string CompanyID { get; set; }
        public string CompanyNameLog { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLegalName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public string TINNo { get; set; }
        public string VatRegistrationNo { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
        public DateTime FMonthStart { get; set; }
        public DateTime FMonthEnd { get; set; }
        public decimal VATAmount { get; set; }
        public string IsWCF { get; set; }
        public string Section { get; set; }
        public int BranchId = 0;
        public string BranchCode = "";
        public string CurrentUser { get; set; }
        public string CurrentUserID { get; set; }
        public bool IsLoading { get; set; }
        public string R_F { get; set; }
        public string fromOpen { get; set; }
        public string SalesType { get; set; }
        public string Trading { get; set; }
        public string DatabaseName { get; set; }
        public string[] PublicRollLines { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime SessionTime { get; set; }
        public int ChangeTime { get; set; }
        public DateTime ServerDateTime { get; set; }
        public DateTime vMinDate = Convert.ToDateTime("1753/01/02");
        public DateTime vMaxDate = Convert.ToDateTime("9998/12/30");
        public bool successLogin = false;
        public string FontSize = "8";
        public string Access { get; set; }
        public string Post { get; set; }
        public DateTime LicenceDate { get; set; }
        public DateTime serverDate { get; set; }
        public bool IsTrial = false;
        public string Trial = "";
        public string TrialComments = "Unregister Version";
        public string ImportFileName { get; set; }
        public string ItemType = "Other";
        public bool IsAlpha = false;
        public string Alpha = "";
        public string AlphaComments = "Alpha Version";
        public bool IsBeta = false;
        public string Beta = "";
        public string BetaComments = "Beta Version";
        public string AppPathForRootLocation = AppDomain.CurrentDomain.BaseDirectory;
        public bool IsBureau = false;
        public string Add { get; set; }
        public string Edit { get; set; }
        public bool IsDHLCrat { get; set; }



        #endregion
        public static bool IsWindowsAuthentication = false;// { get; set; }

    }
}
