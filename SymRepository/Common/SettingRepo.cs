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
    public class SettingRepo
    {
        SettingDAL _settingDAL= new SettingDAL();
        public DataSet SearchSettings()
        {
            DataSet dataSet = new DataSet("SearchSettings");
            try
            {
                dataSet = _settingDAL.SearchSettings();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }
        public string[] SettingsUpdate(List<SettingsVM> settingsVM)
        {
            try
            {
                return _settingDAL.SettingsUpdate(settingsVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string[] settingsDataUpdate(SettingsVM vm)
        {
            string[] result = new string[6];
            try
            {
                result = _settingDAL.settingsDataUpdate(vm, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<SettingsVM> SettingsAll(int branchID = 0)
        {
            List<SettingsVM> SettingsAll ;
            try
            {
                SettingsAll = _settingDAL.SettingsAll(branchID);
            }
            catch (Exception ex)
            {
                throw;
            }
            return SettingsAll;
        }
        public string settingValue(string settingGroup, string settingName)
          {  string retResults = "0";
            try
            {
                retResults = _settingDAL.settingValue(settingGroup, settingName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region Results
            return retResults;
            #endregion
        }

        public string[] settingsDataInsert(SettingsVM vm, string settingGroup, string settingName, string settingType, string settingValue)
        {
            string[] retResults = new string[6];
            try
            {
                retResults = _settingDAL.settingsDataInsert( vm,settingGroup, settingName, settingType, settingValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region Results
            return retResults;
            #endregion
        }

        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            object outPutValue = 0;
            try
            {
                outPutValue=FormatingNumeric( value,  DecPlace);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToDecimal(outPutValue);
        }
        #region unused
        //public void SettingsUpdate(string companyId)
        //{
        //    try
        //    {
        //        _settingDAL.SettingsUpdate(companyId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public string settingsDataInsert(string settingGroup, string settingName, string settingType, string settingValue,SqlConnection currConn, SqlTransaction transaction)
        //{
        //    string retResults = "0";
        //    try
        //    {
        //        retResults = _settingDAL.settingsDataInsert(settingGroup, settingName, settingType, settingValue, currConn, transaction);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    #region Results
        //    return retResults;
        //    #endregion
        //}
        //public string settingsDataDelete(string settingGroup, string settingName, SqlConnection currConn, SqlTransaction transaction)
        //{
        //    string retResults = "0";
        //    try
        //    {
        //        retResults = _settingDAL.settingsDataDelete(settingGroup, settingName, currConn, transaction);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    #region Results
        //    return retResults;
        //    #endregion
        //}
        //public string settingsDataUpdate(string settingGroup, string settingName, string settingGroupNew, string settingNameNew, SqlConnection currConn, SqlTransaction transaction)
        //{
        //    string retResults = "0";
        //    try
        //    {
        //        retResults = _settingDAL.settingsDataUpdate(settingGroup, settingName, settingGroupNew, settingNameNew, currConn, transaction);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    #region Results
        //    return retResults;
        //    #endregion
        //}
        #endregion
    }
}
