using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SymRepository.Common
{
    public class CommonRepo
    {
        

      
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public CommonRepo()
        {
            connVM = null;
        }
      

        #region Maintenance Restart from 25 June 2018 - Khalid


        #endregion
        #region Before 27 February 2019 - Khalid

        public bool AlreadyExist(string tableName, string fieldName, string value)
        {
            bool Exist = false;
            try
            {
                Exist = new CommonDAL().AlreadyExist(tableName, fieldName, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Exist;
        }
        public ComboBox ComboBoxLoad(ComboBox comboBox, string tableName, string valueMember, string displayMember)
        {
            try
            {
                comboBox = new CommonDAL().ComboBoxLoad(comboBox, tableName, valueMember, displayMember);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return comboBox;
        }
        public DataGridView DataGridViewLoad(DataGridView dgv, string TableName, string[] Columns, string[,] SearchColumn)
        {
            try
            {
                dgv = new CommonDAL().DataGridViewLoad(dgv, TableName, Columns, SearchColumn);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dgv;
        }

        public DataTable SearchTransanctionHistoryNew(string TransactionNo, string TransactionType, string TransactionDateFrom, string TransactionDateTo, string ProductName, string databaseName)
        {
            DataTable dataTable = new DataTable("Search Transaction History");
            try
            {
                dataTable = new CommonDAL().SearchTransanctionHistoryNew(TransactionNo, TransactionType, TransactionDateFrom, TransactionDateTo, ProductName, databaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;

        }

        public bool TestConnection(string userName, string Password, string Datasource)
        {
            bool result = false;
            try
            {
                result = new CommonDAL().TestConnection(userName, Password, Datasource);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return result;

        }

        public bool SuperInformationFileExist()
        {
            try
            {
                return new CommonDAL().SuperInformationFileExist(connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataSet CompanyList(string ActiveStatus)
        {

            DataSet dataTable = new DataSet();
            try
            {
                dataTable = new CommonDAL().CompanyList(ActiveStatus);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return dataTable;
        }
        public DataTable SuperAdministrator()
        {
            DataTable dataTable = new DataTable("SA");
            try
            {
                dataTable = new CommonDAL().SuperAdministrator();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return dataTable;
        }
        public DataSet SuperDBInformation()
        {
            DataSet superDs = new DataSet();
            try
            {
                superDs = new CommonDAL().SuperDBInformation();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return superDs;
        }


        public string settings(string SettingGroup, string SettingName)
        {
            string SettingValue = string.Empty;
            try
            {
                SettingValue = new CommonDAL().settings(SettingGroup, SettingName);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return SettingValue;

        }
        public string SysDBCreate(string Uname, string Pwd, string DBSource)
        {
            string result = string.Empty;
            try
            {
                result = SysDBCreate(Uname, Pwd, DBSource);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;


        }
        public string[] NewDBCreate(CompanyProfileVM companyProfiles, string databaseName, List<FiscalYearVM> fiscalDetails)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            try
            {
                retResults = NewDBCreate(companyProfiles, databaseName, fiscalDetails);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retResults;
        }

        #region Old Methods

        public string[] SuperAdministratorUpdate(string miki, string mouse)
        {
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            try
            {
                retResults = new CommonDAL().SuperAdministratorUpdate(miki, mouse);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return retResults;
        }

        public string[] DatabaseInformationUpdate(string Tom, string jary, string mini)
        {

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            try
            {
                retResults = new CommonDAL().DatabaseInformationUpdate(Tom, jary, mini);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retResults;
        }


        public bool UpdateSystemData(string userName, string password, string source)
        {
            bool success = false;

            try
            {
                success = new CommonDAL().UpdateSystemData(userName, password, source);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }
        #endregion

        public string TransactionCode(string CodeGroup, string CodeName, string tableName, string tableIdField, string tableDateField, string tranDate, SqlConnection currConn, SqlTransaction transaction)
        {
            string newID = "";

            try
            {
                newID = new CommonDAL().TransactionCode(CodeGroup, CodeName, tableName, tableIdField, tableDateField, tranDate, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return newID;

            #endregion

        }
        public int TableAdd(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().TableAdd(TableName, FieldName, DataType, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return transResult;
        }
        public int TableFieldAdd(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().TableFieldAdd(TableName, FieldName, DataType, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return transResult;
        }

        public string settingValue(string settingGroup, string settingName)
        {
            string retResults = string.Empty;

            try
            {
                retResults = new CommonDAL().settingValue(settingGroup, settingName);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return retResults;

        }
        public bool TransactionUsed(string tableName, string tableIdField, string FieldValue, SqlConnection currConn)
        {
            bool sqlResult = false;

            try
            {
                sqlResult = new CommonDAL().TransactionUsed(tableName, tableIdField, FieldValue, currConn);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return sqlResult;

            #endregion

        }
        public int DataAlreadyUsed(string CompareTable, String CompareField, String CompareWith, SqlConnection currConn, SqlTransaction transaction)
        {
            int retResults = 0;

            try
            {
                retResults = new CommonDAL().DataAlreadyUsed(CompareTable, CompareField, CompareWith, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return retResults;

            #endregion

        }

        public decimal FormatingDecimal(string input)
        {
            decimal outPutValue = 0;

            try
            {
                outPutValue = new CommonDAL().FormatingDecimal(input);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outPutValue;
        }

        public int TableFieldAddInSys(string TableName, string FieldName, string DataType, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().TableFieldAddInSys(TableName, FieldName, DataType, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return transResult;
        }

        public int DeleteForeignKey(string TableName, string ForeignKeyName, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().DeleteForeignKey(TableName, ForeignKeyName, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return transResult;
        }

        public int ExecuteUpdateQuery(string SqlText, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().ExecuteUpdateQuery(SqlText, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return transResult;
        }

        public string GetHardwareID()
        {
            string processorId = "";

            try
            {
                processorId = new CommonDAL().GetHardwareID();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return processorId;
        }


        public string GetServerHardwareId()
        {
            string retResults = string.Empty;

            try
            {
                retResults = new CommonDAL().GetServerHardwareId();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #region Results

            return retResults;

            #endregion

        }

        public int NewTableAdd(string TableName, string createQuery, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().NewTableAdd(TableName, createQuery, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return transResult;
        }

        public int AddForeignKey(string TableName, string ForeignKeyName, string query, SqlConnection currConn, SqlTransaction transaction)
        {
            int transResult = 0;

            try
            {
                transResult = new CommonDAL().AddForeignKey(TableName, ForeignKeyName, query, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return transResult;
        }
        #region DB List
        //public ComboBox AllDB(ComboBox cmb)
        //{
        //    try
        //    {
        //        cmb = _commonDAL.AllDB(cmb);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return cmb;
        //}
        #endregion DBList
        public bool FileDelete(string tableName, string field, string id)
        {
            return new CommonDAL().FileDelete(tableName, field, id, null, null);
        }

        public DateTime ServerDateTime()
        {
            DateTime result = DateTime.Now;

            try
            {
                CommonDAL _commonDAL = new CommonDAL();
                result = _commonDAL.ServerDateTime();

            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }

        public DataTable DataTableLoad(string tableName, string[] Condition, string DatabaseName)
        {
            DataTable dataTable = new DataTable("Search Transaction");
            try
            {
                dataTable = new CommonDAL().DataTableLoad(tableName, Condition, DatabaseName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataTable;

        }

        public string[] InsertThreads(string value)
        {
            try
            {
                return new CommonDAL().InsertThreads(value, null, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


    }
}
