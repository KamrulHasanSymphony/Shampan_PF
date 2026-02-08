using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymRepository.Common
{
   public class SettingRoleRepo
    {

       SettingRoleDAL _settingRoleDAL = new SettingRoleDAL();


        public DataSet SearchSettingsRole()
        {

            DataSet dataSet = new DataSet("SearchSettingsRole");

            try
            {
                dataSet = _settingRoleDAL.SearchSettingsRole();
            }
            catch (Exception ex)
            {
                throw ex;
            }

           
            return dataSet;
        }
        //public string[] SettingsUpdate(List<SettingsVM> settingsVM)
        //{
        //   string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";
        //    retResults[2] = "";
        //    try
        //    {
        //        retResults = _settingRoleDAL.SettingsUpdate(settingsVM);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
             
        //    return retResults;
        //}
        public bool CheckUserAccess()
        {
            bool isAlloweduser = false;

            try
            {
                isAlloweduser = _settingRoleDAL.CheckUserAccess();
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
            return isAlloweduser;
        }
    }
}
