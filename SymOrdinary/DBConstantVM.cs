using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
    public static class DBConstant
    {
        public static string PassPhrase = "20120220";
        public static string EnKey = "12345678";
        public const string LineDelimeter = "^";
        public const string FieldDelimeter = "~";
        public const string DBName = "VAT";

        public const string CompanyName = "Symphony Softtech Ltd.";
        public const string CompanyAddress = "Concord Archidea, Block B, 4th Floor, Road 4, Dhanmondi R/A, Dhaka-1205.";
        public const string CompanyContactNumber = "Tel: +88(02)9119812, +88(02)8151714, Fax +88(02)9104352";
    }

    public class DatabaseInfoVM
    {
        public static string DatabaseName { get; set; }
        public static string dbUserName { get; set; }
        public static string dbPassword { get; set; }
        public static string dataSource { get; set; }
    }
    public class SuperAdminInfoVM
    {
        public static string dbUserName { get; set; }
        public static string dbPassword { get; set; }
        public static string dataSource { get; set; }
    }

    public class SysDBInfoVM
    {
       
        public static string SysDatabaseName = "SymphonyVATSys";
        public static string DatabaseName { get; set; }
        public static string SysUserName { get; set; }
        public static string SysPassword { get; set; }
        public static string SysdataSource { get; set; }
        public static bool IsWindowsAuthentication = false;// { get; set; }


    }

    public class UserInfoVM
    {
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string UserId { get; set; }
        public static string UserLogingPcIP { get; set; }
    }
}
