using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymOrdinary
{
   public static class License
    {
       static string DataBase = "";

       public static string DataBaseName(string CompanyName)
       {
           //kamrul
           if (CompanyName.ToLower()=="KajolBrothersHRM".ToLower() || CompanyName.ToLower()=="AnupamPrintersHRM".ToLower())
           {
               DataBase = "KajolBrothersHRM~AnupamPrintersHRM" ;
           }
           if (CompanyName.ToLower()=="KajolBrothersHRMDemo".ToLower())
           {
               DataBase = "KajolBrothersHRMDemo~AnupamPrintersHRM" ;
           }
           if (CompanyName.ToLower() == "HRM_SSL_DB".ToLower())
           {
               DataBase = "HRM_SSL_DB~HRM_SSL_DB";
           }
           if (CompanyName.ToLower() == "G4S_HRM_DB".ToLower())
           {
               DataBase = "G4S_HRM_DB~G4S_HRM_DB";
           }
           if (CompanyName.ToLower() == "G4S_HRM_Live_DB".ToLower())
           {
               DataBase = "G4S_HRM_Live_DB~G4S_HRM_Live_DB";
           }
           if (CompanyName.ToLower() == "BOLLORE_HRM_DB".ToLower())
           {
               DataBase = "BOLLORE_HRM_DB~BOLLORE_HRM_DB";
           }
           if (CompanyName.ToLower() == "ShampanHRM_DB".ToLower())
           {
               DataBase = "ShampanHRM_DB~ShampanHRM_DB";
           }
           if (CompanyName.ToLower() == "ShampanPFEGCB".ToLower())
           {
               DataBase = "ShampanPFEGCB~ShampanPFEGCB";
           }
           return DataBase;
       }
    }
}
