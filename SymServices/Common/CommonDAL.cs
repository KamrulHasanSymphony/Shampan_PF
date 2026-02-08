using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using SymOrdinary;
//using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Newtonsoft.Json;
using SymViewModel.PF;
using SymViewModel.Common;
using SymphonySofttech.Utilities;
using System.Threading;
//using Microsoft.SqlServer.Management.Smo;

//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer.Management.Common;
namespace SymServices.Common
{
    public class CommonDAL
    {
        #region Global Variables

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        public System.Data.DataTable SearchTransanctionHistoryNew(string TransactionNo, string TransactionType,
            string TransactionDateFrom, string TransactionDateTo, string ProductName, string databaseName)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            System.Data.DataTable dataTable = new System.Data.DataTable("Search Transaction History");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                            SELECT     dbo.TransactionHistorys.TransactionNo,
                            dbo.TransactionHistorys.TransactionType,
                            convert(varchar,dbo.TransactionHistorys.TransactionDate,120)TransactionDate,
                            dbo.PhProducts.ProductName, 
                            dbo.TransactionHistorys.PhUOMs,
                            dbo.TransactionHistorys.Quantity,
                            dbo.TransactionHistorys.UPrice, 
                            dbo.TransactionHistorys.TradingMarkup,
                            dbo.TransactionHistorys.SDRate,
                            dbo.TransactionHistorys.VATRate,
                            dbo.TransactionHistorys.TCost, 
                            dbo.TransactionHistorys.CreatedBy,
                            convert (varchar,dbo.TransactionHistorys.CreatedOn,120)CreatedOn,
                            dbo.TransactionHistorys.LastModifiedBy,
                            convert (varchar,dbo.TransactionHistorys.LastModifiedOn,120)LastModifiedOn
                            FROM         dbo.TransactionHistorys LEFT OUTER JOIN
                                                    dbo.PhProducts ON dbo.TransactionHistorys.ItemNo = dbo.PhProducts.ItemNo
                            WHERE 
                                (TransactionNo  =  @TransactionNo OR @TransactionNo IS NULL) 
                            AND (TransactionType = @TransactionType  OR @TransactionType IS NULL)
                            AND (TransactionDate >= @TransactionDateFrom OR @TransactionDateFrom IS NULL)
                            AND (TransactionDate < dateadd(d,1, @TransactionDateTo)  OR @TransactionDateTo IS NULL)
                            AND (ProductName = @ProductName  OR @ProductName IS NULL)
                            and TransactionType is not null
                            order by TransactionDate desc ,TransactionNo asc,ProductName asc
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommTransanctionHistory = new SqlCommand();
                objCommTransanctionHistory.Connection = currConn;
                objCommTransanctionHistory.CommandText = sqlText;
                objCommTransanctionHistory.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (TransactionNo == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionNo"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionNo", System.DBNull.Value);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionNo"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionNo"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionNo", TransactionNo);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionNo"].Value = TransactionNo;
                    }
                }

                if (TransactionType == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", System.DBNull.Value);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionType"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (TransactionType == "DebitNote")
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        {
                            objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", "Debit");
                        }
                        else
                        {
                            objCommTransanctionHistory.Parameters["@TransactionType"].Value = "Debit";
                        }
                    }
                    else if (TransactionType == "CreditNote")
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        {
                            objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", "Credit");
                        }
                        else
                        {
                            objCommTransanctionHistory.Parameters["@TransactionType"].Value = "Credit";
                        }
                    }
                    else if (TransactionType == "Sale")
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        {
                            objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", "New");
                        }
                        else
                        {
                            objCommTransanctionHistory.Parameters["@TransactionType"].Value = "New";
                        }
                    }
                    else
                    {
                        if (!objCommTransanctionHistory.Parameters.Contains("@TransactionType"))
                        {
                            objCommTransanctionHistory.Parameters.AddWithValue("@TransactionType", TransactionType);
                        }
                        else
                        {
                            objCommTransanctionHistory.Parameters["@TransactionType"].Value = TransactionType;
                        }
                    }
                }

                if (TransactionDateFrom == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateFrom"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateFrom", System.DBNull.Value);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionDateFrom"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateFrom"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateFrom", TransactionDateFrom);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionDateFrom"].Value = TransactionDateFrom;
                    }
                    // Common Filed
                }

                if (TransactionDateTo == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateTo"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateTo", System.DBNull.Value);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionDateTo"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@TransactionDateTo"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@TransactionDateTo", TransactionDateTo);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@TransactionDateTo"].Value = TransactionDateTo;
                    }
                }

                if (ProductName == "")
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@ProductName"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@ProductName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@ProductName"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommTransanctionHistory.Parameters.Contains("@ProductName"))
                    {
                        objCommTransanctionHistory.Parameters.AddWithValue("@ProductName", ProductName);
                    }
                    else
                    {
                        objCommTransanctionHistory.Parameters["@ProductName"].Value = ProductName;
                    }
                }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommTransanctionHistory);
                dataAdapter.Fill(dataTable);
            }

            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return dataTable;
        }

        public bool TestConnection(string userName, string Password, string Datasource)
        {
            bool result = false;
            SqlConnection conn = null;
            try
            {
                #region open connection and transaction

                string ConnectionString = "Data Source=" + Datasource + ";" +
                                          "user id=" + userName + ";password=" + Password + ";connect Timeout=120;";
                conn = new SqlConnection(ConnectionString);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    result = true;
                }

                #endregion open connection and transaction
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 
        public DataSet SuperDBInformation(SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            DataSet superDs = new DataSet();

            #endregion

            #region try

            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml";
                if (!File.Exists(filePath))
                {
                    return superDs;
                }

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml");
                superDs.ReadXml(AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml");

            }

            #endregion

            #region catch

            catch (Exception exp)
            {
                FileLogger.Log("CommonDAL", "SuperDBInformation", exp.ToString());

                throw exp;
            }
            #endregion

            return superDs;
        }

        public DataSet SettingInformation(SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            DataSet superDs = new DataSet();

            #endregion

            #region try

            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "/SettingInformation.xml";
                if (!File.Exists(filePath))
                {
                    return superDs;
                }

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "/SettingInformation.xml");
                superDs.ReadXml(AppDomain.CurrentDomain.BaseDirectory + "/SettingInformation.xml");

            }

            #endregion

            #region catch

            catch (Exception exp)
            {
                FileLogger.Log("CommonDAL", "SuperDBInformation", exp.ToString());

                throw exp;
            }
            #endregion

            return superDs;
        }


        public bool SuperInformationFileExist(SysDBInfoVMTemp connVM = null)
        {
            /// firstly check user in settingsrole ,if not exist then check settings table
            #region Objects & Variablessule
            bool result = false;
            string SettingValue = string.Empty;
            string rootDirectory = "";
            #endregion

            #region try

            try
            {
                rootDirectory = AppDomain.CurrentDomain.BaseDirectory + "SuperInformation.xml";
                if (System.IO.File.Exists(rootDirectory))
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    doc.Load(rootDirectory);
                    DataSet ds = new DataSet();

                    ds.ReadXml(rootDirectory);
                    string Tom = ds.Tables[0].Rows[0]["Tom"].ToString();
                    string jery = ds.Tables[0].Rows[0]["jery"].ToString();
                    string mini = ds.Tables[0].Rows[0]["mini"].ToString();
                    if (ds.Tables[0].Columns.Contains("doremon"))
                    {
                        string doremon = ds.Tables[0].Rows[0]["doremon"].ToString();
                        SysDBInfoVM.IsWindowsAuthentication = Converter.DESDecrypt(PassPhrase, EnKey, doremon) == "Y" ? true : false;

                    }

                    SysDBInfoVM.SysUserName = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["Tom"].ToString());
                    SysDBInfoVM.SysPassword = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["jery"].ToString());
                    SysDBInfoVM.SysdataSource = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["mini"].ToString());

                    //doc.Load("");
                    ds.Clear();
                    result = true;
                }

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CommonDAL", "SuperInformationFileExist", ex.ToString());

                return result;
            }

            #endregion


            return result;

        }

        public bool SuperInformationFileExist(string path, SysDBInfoVMTemp connVM = null)
        {
            //// firstly check user in settingsrole ,if not exist then check settings table

            #region Objects & Variablessule
            bool result = false;
            string SettingValue = string.Empty;
            string rootDirectory = "";
            #endregion

            #region try

            try
            {
                rootDirectory = path + "SuperInformation.xml";
                if (System.IO.File.Exists(rootDirectory))
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

                    doc.Load(rootDirectory);
                    DataSet ds = new DataSet();

                    ds.ReadXml(rootDirectory);

                    SysDBInfoVM.SysUserName = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["Tom"].ToString());
                    SysDBInfoVM.SysPassword = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["jery"].ToString());
                    SysDBInfoVM.SysdataSource = Converter.DESDecrypt(PassPhrase, EnKey, ds.Tables[0].Rows[0]["mini"].ToString());
                    //doc.Load("");
                    ds.Clear();
                    result = true;
                }

            }

            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("CommonDAL", "SuperInformationFileExist", ex.ToString());

                return result;
            }

            #endregion

            return result;

        }
        /// </summary>
        /// <param name="ActiveStatus"></param>
        /// <returns></returns>

     

        public DataSet CompanyList(string ActiveStatus)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataSet dataTable = new DataSet();
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = @"
                            SELECT 
                            CompanySl,
                            CompanyID,
                            CompanyName,
                            DatabaseName,
                            ActiveStatus,
                            Serial
                            FROM CompanyInformations
                            where (ActiveStatus = @ActiveStatus)	
                            and (CompanyName<>'NA')
                            ORDER BY ISNULl(serial,CompanySL) asc
                            ";
                SqlCommand objCommCompanyList = new SqlCommand();
                objCommCompanyList.Connection = currConn;
                objCommCompanyList.CommandText = sqlText;
                objCommCompanyList.CommandType = CommandType.Text;
                if (!objCommCompanyList.Parameters.Contains("@ActiveStatus"))
                {
                    objCommCompanyList.Parameters.AddWithValue("@ActiveStatus", ActiveStatus);
                }
                else
                {
                    objCommCompanyList.Parameters["@ActiveStatus"].Value = ActiveStatus;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCompanyList);
                dataAdapter.Fill(dataTable);
            }

            #region Catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            return dataTable;
        }

        public System.Data.DataTable SuperAdministrator()
        {
            #region Objects & Variables

            string Description = "";
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            System.Data.DataTable dataTable = new System.Data.DataTable("SA");

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"
                            SELECT miki as [user],mouse as [pwd]
FROM SuperAdministrator";
                SqlCommand objCommProductType = new SqlCommand();
                objCommProductType.Connection = currConn;
                objCommProductType.CommandText = sqlText;
                objCommProductType.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProductType);
                dataAdapter.Fill(dataTable);

                #endregion
            }

            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return dataTable;
        }

        public DataSet SuperDBInformation()
        {
            DataSet superDs = new DataSet();
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml";
                if (!File.Exists(filePath))
                {
                    return superDs;
                }

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml");
                superDs.ReadXml(AppDomain.CurrentDomain.BaseDirectory + "/SuperInformation.xml");
            }
            catch (Exception exp)
            {
                throw exp;
            }

            return superDs;
        }

        public string settings(string SettingGroup, string SettingName)
        {
            /// firstly check user in settingsrole ,if not exist then check settings table

            #region Objects & Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";
            string SettingValue = string.Empty;
            System.Data.DataTable dataTable = new System.Data.DataTable("SA");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "  ";
                sqlText += " SELECT SettingValue FROM SettingsRole";
                sqlText += " WHERE SettingGroup='" + SettingGroup + "' AND SettingName='" + SettingName +
                           "' AND UserId='" + UserInfoVM.UserId + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    DataRow[] settingRow =
                        SymViewModel.Common.settingVM.SettingsDT.Select("SettingGroup='" + SettingGroup +
                                                                        "' and SettingName='" + SettingName +
                                                                        "'");
                    SettingValue = settingRow[0]["SettingValue"].ToString();
                }
                else
                {
                    SettingValue = objfoundId.ToString();
                }
            }

            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                SettingValue = string.Empty;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                SettingValue = string.Empty;
            }

            #endregion

            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return SettingValue;
        }

        public string SysDBCreate(string Uname, string Pwd, string DBSource)
        {
            string result = string.Empty;
            return result;

            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string nextId = "";
            string newID = "";

            #endregion Initializ

            #region Validation

            //if (string.IsNullOrEmpty(databaseName))
            //{
            //    throw new ArgumentNullException(MessageVM.dbMsgMethodName, MessageVM.dbMsgNoCompanyName);
            //}

            #endregion Validation

            #region open connection and transaction sys / Master

            SysDBInfoVM.SysDatabaseName = "SymphonyVATSys";
            currConn = _dbsqlConnection.GetConnection(); //start
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }

            #endregion open connection and transaction
        }

        #region Old Methods

        public string[] SuperAdministratorUpdate(string miki, string mouse)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(miki))
                {
                    throw new ArgumentNullException("SuperAdministratorUpdate", "unable to find Username");
                }
                else if (string.IsNullOrEmpty(mouse))
                {
                    throw new ArgumentNullException("SuperAdministratorUpdate",
                        "unable to find password");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("SA");
                }

                #endregion open connection and transaction

                #region update existing row to table

                if (miki == "ADMINISTRATOR")
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(miki) from SuperAdministrator ";
                    sqlText += " where miki='zTvrNxNvP08='";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    var exeRes = cmdExistTran.ExecuteScalar();
                    int IDExist = Convert.ToInt32(exeRes);
                    if (IDExist <= 0)
                    {
                        sqlText = "";
                        sqlText += " INSERT INTO SuperAdministrator(	miki,	mouse) VALUES('zTvrNxNvP08=','" + mouse +
                                   "')";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                "Super Administrator information not Updated");
                        }
                    }
                    else
                    {
                        #region sql statement

                        sqlText = "";
                        sqlText += " update SuperAdministrator set mouse='" + mouse + "'";
                        sqlText += " where miki='zTvrNxNvP08='";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                "Super Administrator information not Updated");
                        }

                        #endregion
                    }

                    #endregion Find Transaction Exist
                }
                else if (miki == "SYMPHONY")
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(miki) from SuperAdministrator ";
                    sqlText += " where miki='hV9vFF0OUsptxqpZlnEhrA=='";
                    SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                    cmdExistTran.Transaction = transaction;
                    var exeRes = cmdExistTran.ExecuteScalar();
                    int IDExist = Convert.ToInt32(exeRes);
                    if (IDExist <= 0)
                    {
                        sqlText = "";
                        sqlText += " INSERT INTO SuperAdministrator(	miki,	mouse) VALUES('hV9vFF0OUsptxqpZlnEhrA==','" +
                                   mouse +
                                   "')";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                "Super Administrator information not Updated");
                        }
                    }
                    else
                    {
                        #region sql statement

                        sqlText = "";
                        sqlText += " update SuperAdministrator set mouse='" + mouse + "'";
                        sqlText += " where miki='hV9vFF0OUsptxqpZlnEhrA=='";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SuperAdministratorUpdate",
                                "Super Administrator information not Updated");
                        }

                        #endregion
                    }

                    #endregion Find Transaction Exist
                }

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Requested Information";
                    }
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Requested Informatioe";
                }

                #endregion Commit

                #endregion
            }

            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] DatabaseInformationUpdate(string Tom, string jary, string mini)
        {
            #region Objects & Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            #endregion

            #region try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(Tom))
                {
                    throw new ArgumentNullException("DatabaseInformationUpdate", "unable to find Username");
                }
                else if (string.IsNullOrEmpty(jary))
                {
                    throw new ArgumentNullException("DatabaseInformationUpdate",
                        "unable to find password");
                }
                else if (string.IsNullOrEmpty(mini))
                {
                    throw new ArgumentNullException("DatabaseInformationUpdate", "unable to find Source");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("SA");
                }

                #endregion open connection and transaction

                #region update existing row to table

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(Tom) from DBInformation ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                var exeRes = cmdExistTran.ExecuteScalar();
                int IDExist = Convert.ToInt32(exeRes);
                if (IDExist <= 0)
                {
                    sqlText = "";
                    sqlText += " INSERT INTO DBInformation(	Tom,	jary,mini) VALUES('" + Tom + "','" + jary + "','" +
                               mini + "')";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("DatabaseInformationUpdate", "Database Information not update");
                    }
                }
                else
                {
                    #region sql statement

                    sqlText = "";
                    sqlText += " update DBInformation set Tom= '" + Tom + "',jary='" + jary + "',mini='" + mini + "'";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("DatabaseInformationUpdate", "Database Information not update");
                    }

                    #endregion
                }

                #endregion Find Transaction Exist

                #region Commit

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested Information successfully Updated";
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to Update Requested Information";
                    }
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Requested Informatioe";
                }

                #endregion Commit

                #endregion
            }

            #endregion try

            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }

            #endregion catch

            #region Finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return retResults;
        }

        public bool UpdateSystemData(string userName, string password, string source)
        {
            bool success = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            try
            {
                #region Validation

                //if (string.IsNullOrEmpty(""))
                //{
                //    throw new ArgumentNullException("InsertToBankInformation",
                //                "Could not find requested Bank Id.");
                //}

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("sysdb");
                }

                #endregion open connection and transaction

                sqlText = "delete from DBInformation";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objDel = cmdExist.ExecuteScalar();
                sqlText = "";
                sqlText += "insert into DBInformation";
                sqlText += "(";
                sqlText += "Tom,";
                sqlText += "jary,";
                sqlText += "mini";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "'" + userName + "',";
                sqlText += "'" + password + "',";
                sqlText += "'" + source + "'";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                transResult = (int)cmdInsert.ExecuteNonQuery();
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            return success;
        }

        #endregion

        public string TransactionCode(string CodeGroup, string CodeName, string tableName,
            string tableIdField, string tableDateField, string tranDate, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            decimal retResults = 0;
            int countId = 0;
            string sqlText = "";
            string Prefetch = "";
            int CurrentID = 0;
            int SetupLen = 0;
            string newID = "";
            string n = "";
            int Len = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(CodeGroup))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(CodeName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }
                else if (string.IsNullOrEmpty(tableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                }
                else if (string.IsNullOrEmpty(tableIdField))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                }
                else if (Convert.ToDateTime(tranDate) < DateTime.MinValue ||
                         Convert.ToDateTime(tranDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("TransactionCode", "Transaction Date not Valid");
                }

                #endregion Validation

                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Prefetch

                sqlText = "";
                sqlText = sqlText + " SELECT     prefix FROM Codes";
                sqlText = sqlText + " WHERE     (CodeGroup = '" + CodeGroup + "') AND (CodeName = '" + CodeName + "')";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;
                //Prefetch = (string)cmdPrefetch.ExecuteScalar();
                var exeRes = cmdPrefetch.ExecuteScalar();
                Prefetch = exeRes.ToString();
                if (string.IsNullOrEmpty(Prefetch))
                {
                    throw new ArgumentNullException("TransactionCode", "Could not find prefix.");
                }

                #endregion Prefetch

                #region F year Start Date

                var tranYear = Convert.ToDateTime(tranDate).ToString("yyyy-MM-dd");
                sqlText = "";
                sqlText = sqlText + " SELECT MIN(fy1.PeriodStart) FROM FiscalYear fy1 WHERE fy1.CurrentYear= (";
                ////sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE convert (date, '" + tranDate + "',101)";
                //sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE CONVERT(date,'" + tranDate + "', 101)";
                sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE '" + tranYear + "'";
                sqlText = sqlText + " between fy.PeriodStart AND fy.PeriodEnd) ";
                SqlCommand cmdFsDate = new SqlCommand(sqlText, currConn);
                cmdFsDate.Transaction = transaction;
                object objFsDate = cmdFsDate.ExecuteScalar();
                //DateTime FsDate = Convert.ToDateTime(objFsDate);
                var FsDate = Convert.ToDateTime(objFsDate).ToString("yyyy-MM-dd HH:mm:ss");
                if (objFsDate == null || FsDate == null)
                {
                    throw new ArgumentNullException("TransactionCode", "Fyscal year Stardate not found");
                }

                #endregion F year Start Date

                #region F year End Date

                sqlText = "";
                sqlText = sqlText + " SELECT  MAX(fy1.PeriodEnd) FROM FiscalYear fy1 WHERE fy1.CurrentYear= (";
                ////sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE convert (date, '" + tranDate + "',101)";
                sqlText = sqlText + " SELECT fy.CurrentYear FROM FiscalYear fy WHERE '" + tranYear + "'";
                sqlText = sqlText + " between fy.PeriodStart AND fy.PeriodEnd) ";
                SqlCommand cmdFeDate = new SqlCommand(sqlText, currConn);
                cmdFeDate.Transaction = transaction;
                object objFeDate = cmdFeDate.ExecuteScalar();
                var FeDate = Convert.ToDateTime(objFeDate).ToString("yyyy-MM-dd HH:mm:ss");
                if (FeDate == null || objFeDate == null)
                {
                    throw new ArgumentNullException("TransactionCode", "Fyscal year Enddate not found");
                }

                #endregion F year End Date

                #region CurrentID

                sqlText = "";
                sqlText += "  SELECT isnull(max(cast(SUBSTRING ( ih." + tableIdField + " ,5 , LEN(ih." + tableIdField +
                           ")-9 ) AS INT)),0)+1";
                sqlText += " FROM " + tableName + " ih";
                sqlText += " WHERE SUBSTRING ( ih." + tableIdField + " ,1 , 3 )='" + Prefetch + "'";
                sqlText +=
                    " AND ih." + tableDateField + " >= '" + FsDate + "' and ih." + tableDateField + " <DATEADD(d,1,'" +
                    FeDate + "' )";
                //sqlText += " AND ih." + tableDateField + " BETWEEN '" + FsDate + "' and '" + FeDate + "' ";
                SqlCommand cmdCurrentID = new SqlCommand(sqlText, currConn);
                cmdCurrentID.Transaction = transaction;
                exeRes = cmdCurrentID.ExecuteScalar();
                CurrentID = Convert.ToInt32(exeRes);
                if (CurrentID < 0)
                {
                    throw new ArgumentNullException("Setting", "Unable Table create");
                }

                #endregion CurrentID

                #region SetupLen

                sqlText = "";
                sqlText = sqlText + " SELECT     Lenth FROM Codes";
                sqlText = sqlText + " WHERE     (CodeGroup = '" + CodeGroup + "') AND (CodeName = '" + CodeName + "')";
                SqlCommand cmdSetupLen = new SqlCommand(sqlText, currConn);
                cmdSetupLen.Transaction = transaction;
                exeRes = cmdSetupLen.ExecuteScalar();
                SetupLen = Convert.ToInt32(exeRes);
                if (SetupLen < 0)
                {
                    throw new ArgumentNullException("Code", "Unable to Create");
                }

                #endregion SetupLen

                #region ID Create

                n = "";
                Len = Convert.ToString(CurrentID).Length;
                for (int i = 0; i < SetupLen - Len; i++)
                {
                    n = n + "0";
                }

                var idYear = Convert.ToDateTime(tranDate);
                //newID = Prefetch + "-" + n + Convert.ToString(CurrentID) + "/" +
                //        Convert.ToString(tranDate.ToString("MMyy"));
                newID = Prefetch + "-" + n + Convert.ToString(CurrentID) + "/" +
                        Convert.ToString(idYear.ToString("MMyy"));
                if (string.IsNullOrEmpty(newID))
                {
                    throw new ArgumentNullException("TransactionCodeGenerator", "Unable to Create ID");
                }

                #endregion ID Create
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion Catch and Finall

            #region Results

            return newID;

            #endregion
        }

        public int TableAdd(string TableName, string FieldName, string DataType, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                }

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";
                sqlText += " BEGIN";
                sqlText += " CREATE TABLE " + TableName + "( " + FieldName + " " + DataType + " null) ";
                sqlText += " END";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;
                var exeRes = cmdPrefetch.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            return transResult;
        }

        public int TableFieldAdd(string TableName, string FieldName, string DataType, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                }

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + ";";
                sqlText += " END";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;
                var exeRes = cmdPrefetch.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            return transResult;
        }

        public void TableFieldAlter(string TableName, string FieldName, string DataType, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                }

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " ALTER TABLE " + TableName + " ALTER COLUMN " + FieldName + "   " + DataType + "";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;
                cmdPrefetch.ExecuteScalar();

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion
        }

        public string settingValue(string settingGroup, string settingName)
        {
            #region Initializ

            string retResults = string.Empty;
            string sqlText = "";
            SqlConnection currConn = null;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingValue", "Code system not find");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingValue", "Code system not find");
                }

                #endregion Validation

                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region Stock

                sqlText = "  ";
                sqlText = " SELECT SettingValue FROM Settings ";
                sqlText += " where ";
                sqlText += " SettingGroup='" + settingGroup + "' ";
                sqlText += " and SettingName='" + settingName + "'";
                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {
                    retResults = string.Empty;
                }
                else
                {
                    var exeRes = cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                    retResults = exeRes.ToString();
                    //object objDel = cmdDelete.ExecuteScalar();
                }

                #endregion Stock
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion
        }

        public bool TransactionUsed(string tableName, string tableIdField, string FieldValue, SqlConnection currConn)
        {
            #region Initializ

            bool sqlResult = false;
            string sqlText = "";
            int CurrentID = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(FieldValue))
                {
                    throw new ArgumentNullException("TransactionUsed", "Unable to find FieldValue");
                }
                else if (string.IsNullOrEmpty(tableIdField))
                {
                    throw new ArgumentNullException("TransactionUsed", "Unable to find FieldValue");
                }
                else if (string.IsNullOrEmpty(FieldValue))
                {
                    throw new ArgumentNullException("TransactionUsed", "Unable to find FieldValue");
                }

                #endregion Validation

                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region CurrentID

                sqlText = "";
                sqlText += "  SELECT COUNT(" + tableIdField + ") FROM " + tableName + "  ";
                sqlText += " WHERE " + tableIdField + "='" + FieldValue + "'";
                SqlCommand cmdCurrentID = new SqlCommand(sqlText, currConn);
                var exeRes = cmdCurrentID.ExecuteScalar();
                CurrentID = Convert.ToInt32(exeRes);
                if (CurrentID > 0)
                {
                    sqlResult = true;
                }

                #endregion CurrentID
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            #region Results

            return sqlResult;

            #endregion
        }

        public int DataAlreadyUsed(string CompareTable, String CompareField, String CompareWith, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            int retResults = 0;
            string sqlText = "";

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region AvgPrice

                sqlText = "  ";
                sqlText += "  SELECT isnull(COUNT(" + CompareField + "),0) FROM " + CompareTable + "";
                sqlText += "  WHERE " + CompareField + "='" + CompareWith + "'";
                SqlCommand cmdDAU = new SqlCommand(sqlText, currConn);
                cmdDAU.Transaction = transaction;
                if (cmdDAU.ExecuteScalar() == null)
                {
                    retResults = 0;
                }
                else
                {
                    var exeRes = cmdDAU.ExecuteScalar();
                    retResults = Convert.ToInt32(exeRes);
                    //object objDel = cmdDelete.ExecuteScalar();
                }

                #endregion Stock
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion
        }

        public decimal FormatingDecimal(string input)
        {
            string inputValue = input;
            decimal outPutValue = 0;
            string decPointLen = "";
            int DecPlace = 9;
            try
            {
                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }

                if (Convert.ToDecimal(inputValue) < 1000)
                {
                    var a = "0." + decPointLen + "";
                    //outPutValue = Convert.ToDecimal(Convert.ToDecimal(inpQuantity).ToString("0.0000"));
                    outPutValue = Convert.ToDecimal(inputValue);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    //outPutValue = Convert.ToDecimal(Convert.ToDecimal(inpQuantity).ToString("0,0.0000"));
                    //outPutValue = Convert.ToDecimal(inputValue).ToString(a);
                    outPutValue = Convert.ToDecimal(inputValue);
                }
            }

            #region Catch

            catch (Exception ex)
            {
                string exMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                ex.StackTrace;
                }

                throw new Exception(ex.Message);
            }

            #endregion Catch

            return outPutValue;
        }

        public int TableFieldAddInSys(string TableName, string FieldName, string DataType, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(); //
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region Prefetch

                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + ";";
                sqlText += " END";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                //cmdPrefetch.ExecuteScalar();
                //cmdPrefetch.Transaction = transaction;
                var exeRes = cmdPrefetch.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return transResult;
        }

        public int DeleteForeignKey(string TableName, string ForeignKeyName, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(ForeignKeyName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create Foreign Key");
                }

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " if exists(select * from sys.foreign_keys ";
                sqlText += " where Name = '" + ForeignKeyName + "' )   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE " + TableName + " DROP CONSTRAINT " + ForeignKeyName + " ";
                sqlText += " END";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;
                var exeRes = cmdPrefetch.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            return transResult;
        }

        public int ExecuteUpdateQuery(string SqlText, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = SqlText;
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(SqlText))
                {
                    throw new ArgumentNullException("ExecuteUpdateQuery", "No data found for Update.");
                }

                #endregion Validation

                #region Prefetch

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                var exeRes = cmd.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                //transResult = -1;

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            return transResult;
        }

        public string GetHardwareID()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + "Win32_Processor");
            string processorId = "";
            try
            {
                foreach (ManagementObject share in searcher.Get())
                {
                    if (share.Properties.Count > 0)
                    {
                        foreach (PropertyData PC in share.Properties)
                        {
                            if (PC.Name.ToLower() == "processorid")
                            {
                                processorId = PC.Value.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                //MessageBox.Show("can't get data because of the followeing error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return processorId;
        }

        public void SetSecurity(string companyId)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
            try
            {
                CompanyprofileDAL reportDsdal = new CompanyprofileDAL();
                DataSet ReportResult = reportDsdal.ComapnyProfileSecurity(companyId);
                if (ReportResult.Tables[0].Rows.Count <= 0)
                {
                    return;
                }

                #region Retrive Data

                string cName = ReportResult.Tables[0].Rows[0]["CompanyName"].ToString();
                string cLegalName = ReportResult.Tables[0].Rows[0]["CompanyLegalName"].ToString();
                string vatNo = ReportResult.Tables[0].Rows[0]["VatRegistrationNo"].ToString();
                string hardwareInfo = ReportResult.Tables[0].Rows[0]["Mouse"].ToString();
                string tom = Converter.DESEncrypt(PassPhrase, EnKey, cName);
                string jary = Converter.DESEncrypt(PassPhrase, EnKey, cLegalName);
                string miki = Converter.DESEncrypt(PassPhrase, EnKey, vatNo);
                string mouse = "";
                if (string.IsNullOrEmpty(hardwareInfo))
                {
                    //mouse = GetHardwareID();
                    mouse = GetServerHardwareId();
                    mouse = Converter.DESEncrypt(PassPhrase, EnKey, mouse);
                }
                else
                {
                    return;
                }

                #endregion Retrive Data

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                if (string.IsNullOrEmpty(hardwareInfo))
                {
                    string updateQuer = "";
                    updateQuer += " Update CompanyProfiles set Tom ='" + tom + "',";
                    updateQuer += " Jary ='" + jary + "', Miki ='" + miki + "', Mouse ='" + mouse + "'";
                    updateQuer += "where CompanyID='" + companyId + "'";
                    transResult = ExecuteUpdateQuery(updateQuer, currConn, transaction);
                    if (transResult >= 0)
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw sqlex;
            }
            catch (ArgumentNullException sqlex)
            {
                transaction.Rollback();
                throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public string GetServerHardwareId()
        {
            #region Initializ

            string retResults = string.Empty;
            SqlConnection currConn = null;
            string sqlText = @"EXEC xp_instance_regread
                                'HKEY_LOCAL_MACHINE',
                                'HARDWARE\DESCRIPTION\System\MultifunctionAdapter\0\DiskController\0\DiskPeripheral\0',
                                'Identifier'";

            #endregion Initializ

            #region Try

            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                System.Data.DataTable dt = new System.Data.DataTable("ServerProcessor");
                //SqlCommand getHardware = new SqlCommand(sqlText, currConn);
                SqlDataAdapter adapter = new SqlDataAdapter(sqlText, currConn);
                adapter.Fill(dt);
                if (dt == null)
                {
                    retResults = string.Empty;
                }
                else if (dt.Columns.Count > 0)
                {
                    retResults = dt.Rows[0][1].ToString();
                }
            }

            #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                //if (currConn == null)
                //{
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
                //}
            }

            #endregion

            #region Results

            return retResults;

            #endregion
        }

        public int NewTableAdd(string TableName, string createQuery, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(createQuery))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }

                //else if (string.IsNullOrEmpty(DataType))
                //{
                //    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                //}

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";
                sqlText += " BEGIN";
                sqlText += " " + createQuery;
                sqlText += " END";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;
                var exeRes = cmdPrefetch.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            return transResult;
        }

        public int AddForeignKey(string TableName, string ForeignKeyName, string query, SqlConnection currConn,
            SqlTransaction transaction)
        {
            #region Initializ

            string sqlText = "";
            int transResult = 0;

            #endregion

            #region Try

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");
                }
                else if (string.IsNullOrEmpty(query))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");
                }

                //else if (string.IsNullOrEmpty(DataType))
                //{
                //    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");
                //}

                #endregion Validation

                #region Prefetch

                sqlText = "";
                sqlText = "";
                sqlText += " if NOT exists(select * from sys.foreign_keys ";
                sqlText += " where Name = '" + ForeignKeyName + "' )   ";
                sqlText += " BEGIN";
                sqlText += " " + query;
                sqlText += " END";
                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.ExecuteNonQuery();
                //cmdPrefetch.Transaction = transaction;
                //transResult = (int)cmdPrefetch.ExecuteNonQuery();

                #endregion Prefetch
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }

            #endregion

            return transResult;
        }

        public System.Windows.Forms.ComboBox ComboBoxLoad(System.Windows.Forms.ComboBox comboBox, string tableName,
            string valueMember, string displayMember)
        {
            SqlConnection currConn = null;
            currConn = _dbsqlConnection.GetConnection(); //
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }

            System.Data.DataTable dt = new System.Data.DataTable("ComboDt");
            string sql = "";
            sql = sql + @"select " + valueMember + ", " + displayMember + " from " + tableName + "";
            SqlDataAdapter adp = new SqlDataAdapter(sql, currConn);
            adp.Fill(dt);
            comboBox.DataSource = dt;
            comboBox.DisplayMember = displayMember.Replace("[", "").Replace("]", "");
            comboBox.ValueMember = valueMember;
            return comboBox;
        }

        public System.Windows.Forms.DataGridView DataGridViewLoad(System.Windows.Forms.DataGridView dgv,
            string TableName, string[] Column, string[,] SearchColumn)
        {
            //string[,] SearchColumn = new string[3, 2] 
            //{
            //   // { "Search Field", "Search Value" },
            //    { "BodyColor", txtBodyColor.Text },
            //    { "CuffColor", txtCuffColor.Text },
            //    { "Size", txtSize.Text }
            //};
            SqlConnection currConn = null;
            currConn = _dbsqlConnection.GetConnection(); //
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }

            string sql = "";
            sql = sql + @" Select ";
            foreach (var item in Column)
            {
                sql = sql + @" " + item + ",";
            }

            sql = sql.Substring(0, sql.Length - 1);
            System.Data.DataTable dt = new System.Data.DataTable("DataTable");
            sql = sql + @" from " + TableName + "";
            sql = sql + @" where IsArchive=0";
            if (SearchColumn.GetUpperBound(0) > 0)
            {
                for (int i = 0; i <= SearchColumn.GetUpperBound(0); i++)
                {
                    string SearchField = SearchColumn[i, 0];
                    string SearchValue = SearchColumn[i, 1];
                    sql = sql + @" and " + SearchField + " like '%" + SearchValue + "%' ";
                }
            }

            SqlDataAdapter adp = new SqlDataAdapter(sql, currConn);
            adp.Fill(dt);
            dgv.DataSource = dt;
            return dgv;
        }

        public string GetIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

        #region Load DB

        //public System.Windows.Forms.ComboBox AllDB(System.Windows.Forms.ComboBox cmb)
        //{
        //    List<Database> dbList;
        //    try
        //    {
        //        SqlConnection sqlConn = _dbsqlConnection.GetConnectionMaster();
        //        Server sqlServer = new Server(new ServerConnection(sqlConn));
        //        dbList = new List<Database>();
        //        foreach (Database db in sqlServer.Databases)
        //        {
        //            var tt = db;
        //            dbList.Add(tt);
        //        }

        //        cmb.DataSource = dbList;
        //        cmb.SelectedIndex = -1;
        //    }
        //    catch (Exception exc)
        //    {
        //    }

        //    return cmb;
        //}

        #endregion

        public bool AlreadyExist(string tableName, string fieldName, string value)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT top 1 " + fieldName + " from " + tableName + " where " + fieldName + "= '" + value +
                          "' ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            #endregion

            return Exist;
        }

        public bool FileDelete(string tableName, string field, string id, SqlConnection currConn,
            SqlTransaction transaction)
        {
            bool returnval = false;
            try
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("FileDelete");
                }

                string sqlText = "";
                sqlText = "update " + tableName + " set " + field + "=@field";
                sqlText += " where Id=@Id";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", id);
                cmdUpdate.Parameters.AddWithValue("@field", "");
                cmdUpdate.Transaction = transaction;
                cmdUpdate.ExecuteNonQuery();
                returnval = true;
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            return returnval;
        }

        public DateTime ServerDateTime()
        {
            DateTime result = DateTime.Now;
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                sqlText = "select GETDATE()";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //result = Convert.ToDateTime(cmdIdExist.ExecuteScalar());
                var exeRes = cmdIdExist.ExecuteScalar();
                result = Convert.ToDateTime(exeRes);
            }

            #region Catch

            catch (Exception ex)
            {
                throw new ArgumentNullException("",
                    "SQL:" + sqlText + FieldDelimeter +
                    ex.Message.ToString()); //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }

            return result;
        }

 
        public string SelectSingleRowFromMultipleRows(string tableName, string fieldName,
            string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string ReturnValue = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                sqlText = "";
                sqlText += @" SELECT STUFF((SELECT '~' + CAST( " + fieldName + " AS nvarchar(500))";
                sqlText += @" FROM (SELECT DISTINCT " + fieldName + " FROM " + tableName;
                sqlText += @" WHERE 1=1";
                //////------AND IsDistribute = 0
                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                sqlText += @" ) US ORDER BY " + fieldName;
                sqlText += @" FOR XML PATH('')), 1, 1, '') [ReturnValue]";

                #endregion SqlText

                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                            string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    ReturnValue = Convert.ToString(dr["ReturnValue"]);
                }

                dr.Close();

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return ReturnValue;
        }


        ////==================SelectFieldsByConditionList=================
        public List<string> SelectFieldsByConditionList(string tableName, List<string> selectFields,
            List<string> conditionFields = null, List<string> conditionValues = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<string> returnValues = new List<string>();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "";

                string selectFieldsString = "";
                for (int k = 0; k < selectFields.Count(); k++)
                {
                    selectFieldsString += selectFields[k] + ",";
                }

                selectFieldsString = selectFieldsString.Substring(0, selectFieldsString.Length - 1);

                sqlText = "SELECT " + selectFieldsString + " From " + tableName + " WHERE 1=1";
                int i = 0;
                if (conditionFields != null && conditionValues != null)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) &&
                            conditionFields.Count() == conditionValues.Count())
                        {
                            continue;
                        }

                        sqlText += " AND " + conditionFields[i] + "=@" + conditionFields[i];
                        i++;
                    }
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                i = 0;
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Count() == conditionValues.Count())
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]))
                        {
                            continue;
                        }

                        objComm.Parameters.AddWithValue("@" + conditionFields[i], conditionValues[i]);
                        i++;
                    }
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();

                string selectField = "";
                while (dr.Read())
                {
                    for (int p = 0; p < selectFields.Count(); p++)
                    {
                        selectField = selectFields[p];
                        returnValues.Add(dr[selectField].ToString());
                    }
                }

                dr.Close();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }

            #endregion Try

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + '~' + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + "~" + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return returnValues;
        }


        ////==================SelectFieldsByCondition=================
        public string[] SelectFieldsByCondition(string tableName, string[] selectFields,
            string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string[] returnValues = new string[5];

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "";

                string selectFieldsString = "";
                for (int k = 0; k < selectFields.Length; k++)
                {
                    selectFieldsString += selectFields[k] + ",";
                }

                selectFieldsString = selectFieldsString.Substring(0, selectFieldsString.Length - 1);

                sqlText = "SELECT " + selectFieldsString + " From " + tableName + " WHERE 1=1";
                int i = 0;
                if (conditionFields != null && conditionValues != null)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) &&
                            conditionFields.Length == conditionValues.Length)
                        {
                            continue;
                        }

                        sqlText += " AND " + conditionFields[i] + "=@" + conditionFields[i];
                        i++;
                    }
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                i = 0;
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]))
                        {
                            continue;
                        }

                        objComm.Parameters.AddWithValue("@" + conditionFields[i], conditionValues[i]);
                        i++;
                    }
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();


                while (dr.Read())
                {
                    for (int p = 0; p < selectFields.Length; p++)
                    {
                        returnValues[p] = dr[selectFields[p]].ToString();
                    }
                }

                dr.Close();

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }

            #endregion Try

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + '~' + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + "~" + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return returnValues;
        }


        ////==================SelectByCondition=================
        public DataTable SelectByCondition(string tableName, string conditionField, string conditionValue,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable returnDt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "";
                sqlText = "SELECT *  From " + tableName + " WHERE 1=1";
                if (!string.IsNullOrWhiteSpace(conditionField))
                {
                    sqlText += " AND " + conditionField + "=@" + conditionField;
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                if (!string.IsNullOrWhiteSpace(conditionField))
                {
                    da.SelectCommand.Parameters.AddWithValue("@" + conditionField, conditionValue);
                }

                da.SelectCommand.Transaction = transaction;
                da.Fill(returnDt);

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }

            #endregion Try

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + '~' + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + "~" + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return returnDt;
        }

        ////==================SelectByMultiCondition=================
        public DataTable SelectByMultiCondition(string tableName, string[] conditionFields, string[] conditionValues,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable returnDt = new DataTable();

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "";
                sqlText = "SELECT *  From " + tableName + " WHERE 1=1";
                int i = 0;
                if (conditionFields != null && conditionValues != null)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) &&
                            conditionFields.Length == conditionValues.Length)
                        {
                            continue;
                        }

                        sqlText += " AND " + conditionFields[i] + "=@" + conditionFields[i];
                        i++;
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                i = 0;
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]))
                        {
                            continue;
                        }

                        da.SelectCommand.Parameters.AddWithValue("@" + conditionFields[i], conditionValues[i]);
                        i++;
                    }
                }

                da.SelectCommand.Transaction = transaction;
                da.Fill(returnDt);

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }

            #endregion Try

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + '~' + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + "~" + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return returnDt;
        }

        public bool ExistCheck(string tableName, string[] conditionFields, string[] conditionValues,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" SELECT TOP 1 Id"
                          + " FROM [" + tableName + "]"
                          + " WHERE 1=1  ";
                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                SqlCommand _objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                            string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        _objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                    break;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return Exist;
        }

        public bool ExistCheckOnUpdate(string tableName, string Id, string[] conditionFields, string[] conditionValues,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" SELECT TOP 1 Id"
                          + " FROM [" + tableName + "]"
                          + " WHERE 1=1 and Id <> @Id";
                int i = 0;
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]))
                        {
                            continue;
                        }

                        sqlText += " AND " + conditionFields[i] + "=@" + conditionFields[i];
                        i++;
                    }
                }

                SqlCommand _objComm = new SqlCommand(sqlText, currConn, transaction);
                i = 0;
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]))
                        {
                            continue;
                        }

                        _objComm.Parameters.AddWithValue("@" + conditionFields[i], conditionValues[i]);
                        i++;
                    }
                }

                _objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                    break;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return Exist;
        }


        public bool ExistCheck(string tableName, string conditionField, string conditionValue, SqlConnection VcurrConn,
            SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                int i = 0;
                sqlText = @" SELECT TOP 1 id "
                          + " FROM " + tableName
                          + " WHERE 1=1  ";
                if (!string.IsNullOrWhiteSpace(conditionField) || !string.IsNullOrWhiteSpace(conditionValue))
                {
                    sqlText += " AND " + conditionField + "=@" + conditionField + "";
                }

                SqlCommand _objComm = new SqlCommand(sqlText, currConn, transaction);
                if (!string.IsNullOrWhiteSpace(conditionField) || !string.IsNullOrWhiteSpace(conditionValue))
                {
                    _objComm.Parameters.AddWithValue("@" + conditionField + "", conditionValue);
                }

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                    break;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return Exist;
        }

        /// <summary>
        /// Only for Users Methods
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <param name="VcurrConn"></param>
        /// <param name="Vtransaction"></param>
        /// <returns></returns>
        public bool CheckDuplicateInInsertUser(string tableName, string fieldName, string value,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" SELECT top 1 " + fieldName + " from " + tableName + " where " + fieldName + "= '" + value +
                          "' and IsArchived=0";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                _objComm.Transaction = transaction;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return Exist;
        }

        public bool CheckDuplicateInInsert(string tableName, string fieldName, string value, SqlConnection VcurrConn,
            SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" SELECT top 1 " + fieldName + " from " + tableName + " where " + fieldName + "= '" + value +
                          "' and IsArchive=0";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                _objComm.Transaction = transaction;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return Exist;
        }


        public bool CheckDuplicateInInsertConditionFields(string tableName, string fieldName, SqlConnection VcurrConn,
           SqlTransaction Vtransaction, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" SELECT top 1 " + fieldName + " from " + tableName + " where 1=1 and IsArchive=0 ";
                           
                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                SqlCommand _objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                            string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        _objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                _objComm.Transaction = transaction;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return Exist;
        }

        public bool CheckDuplicateInInsertWithBranch(string tableName, string fieldName, string value, int branchId,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @" SELECT top 1 " + fieldName + " from " + tableName + " where " + fieldName + "= '" + value +
                          "' and IsArchive=0";
                sqlText += @" AND  BranchId=@branchId ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branchId);
                SqlDataReader dr;
                _objComm.Transaction = transaction;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return Exist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Table Name"></param>
        /// <param name="Field Name"></param>
        /// <param name="Field Value"></param>
        /// <param name="Connection Name"></param>
        /// <param name="Transaction Name"></param>
        /// <returns></returns>
        public bool CheckDuplicateInUpdate(string Id, string tableName, string fieldName, string value,
            SqlConnection VcurrConn, SqlTransaction transaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"select distinct " + fieldName + " from " + tableName + " where " + fieldName + "= '" +
                          value + "' and Id<>'" + Id + "' and IsArchive=0";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                _objComm.Transaction = transaction;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return Exist;
        }

        public bool CheckDuplicateInUpdateWithBranch(string Id, string tableName, string fieldName, string value,
            int branchId, SqlConnection VcurrConn, SqlTransaction transaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"select distinct [" + fieldName + "] from [" + tableName + "] where [" + fieldName + "]= '" +
                          value + "' and Id<>'" + Id + "' and IsArchive=0";
                sqlText += @" AND  BranchId=@branchId ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@BranchId", branchId);
                SqlDataReader dr;
                _objComm.Transaction = transaction;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return Exist;
        }


        public int NextCode(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            int nextCode = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

               // sqlText = "select isnull(max(cast(Code as int)),0)+1 Code FROM  " + tableName + "";
                sqlText = "select isnull(max(cast(REPLACE(id, '1_', '') as int)),0)+1 Code FROM  " + tableName + "";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                var exeRes = objComm.ExecuteScalar();
                nextCode = Convert.ToInt32(exeRes);
                if (nextCode <= 0)
                {
                    throw new ArgumentNullException("Unexeptected Error - Unable to create new Customer No", "");
                }

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return nextCode;
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return nextCode;

            #endregion
        }

        public int NextId(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            int nextId = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = "select isnull(max(cast(id as int)),0)+1 FROM  " + tableName + "";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                nextId = Convert.ToInt32(exeRes);
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("InsertToCustomer",
                        "Unable to create new Customer No");
                }

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return nextId;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return nextId;

            #endregion
        }

        public string NextIdWithBranch(string tableName, string BranchId, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Initializ

            string sqlText = "";
            string nextId = "0";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = " ";
                sqlText +=
                    @"Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10)
, id))+1,10))),0) ";
                sqlText += " FROM  " + tableName + "";
                sqlText += " WHERE BranchId=  " + BranchId + "";
                SqlCommand cmdSelect = new SqlCommand(sqlText, currConn, transaction);
                var exeRes = cmdSelect.ExecuteScalar();
                nextId = BranchId + "_" + (Convert.ToInt32(exeRes) + 1);
                //if (Convert.ToInt32(exeRes) <= 0)
                //{
                //    throw new ArgumentNullException("Select From "+ tableName,
                //                                    "Unable Select From "+tableName);
                //}

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return nextId;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return nextId;

            #endregion
        }

        public string[] DeleteTable(string tableName, string[] conditionFields, string[] conditionValues,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = tableName + " Delete"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion
                  

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

             
                    sqlText = "DELETE "
                        + " FROM " + tableName
                        + " WHERE 1=1  ";
                    string cField = "";
                    if (conditionFields != null && conditionValues != null &&
                        conditionFields.Length == conditionValues.Length)
                    {
                        for (int i = 0; i < conditionFields.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                                string.IsNullOrWhiteSpace(conditionValues[i]))
                            {
                                continue;
                            }

                            cField = conditionFields[i].ToString();
                            cField = Ordinary.StringReplacing(cField);
                            sqlText += " AND " + conditionFields[i] + "=@" + cField;
                        }
                    }

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    if (conditionFields != null && conditionValues != null &&
                        conditionFields.Length == conditionValues.Length)
                    {
                        for (int j = 0; j < conditionFields.Length; j++)
                        {
                            if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                                string.IsNullOrWhiteSpace(conditionValues[j]))
                            {
                                continue;
                            }

                            cField = conditionFields[j].ToString();
                            cField = Ordinary.StringReplacing(cField);
                            cmdUpdate.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                        }
                    }

                
                    cmdUpdate.ExecuteNonQuery();
                    retResults[2] = ""; // Return Id
                    retResults[3] = sqlText; //  SQL Query
                          
                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";

                #endregion Commit
            }

            #region catch

            catch (Exception ex)
            {
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }

        public string[] DeleteTable(string tableName, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = tableName + " Delete"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "DELETE "
                          + " FROM " + tableName;
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.ExecuteNonQuery();
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";

                #endregion Commit
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }

        

        public string[] DeleteTableByCondition(string tableName, string conditionField, string conditionValue,
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = tableName + " Delete"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement

                sqlText = "DELETE "
                          + " FROM " + tableName
                          + " WHERE 1=1  ";
                sqlText += " AND " + conditionField + "=@" + conditionField;
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@" + conditionField, conditionValue);
                cmdUpdate.ExecuteNonQuery();
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Delete Successfully.";

                #endregion Commit
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }


        public string[] DeleteTableInformation(string Id, string TableName, string FieldName, SqlConnection VcurrConn,
            SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = FieldName + " from " + TableName + " Delete"; //Method Name
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    if (transaction == null)
                    {
                        transaction = currConn.BeginTransaction("Delete");
                    }
                }

                #endregion open connection and transaction

                #region Check is  it used

                #endregion Check is  it used

                #region Update Settings

                sqlText = "";
                sqlText = "delete from  " + TableName + " ";
                sqlText += " where " + FieldName + "=@Id";
                //sqlText += " AND IsArchive=0";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", Id);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    //throw new ArgumentNullException(FieldName+ " Delete", Id + " could not Delete.");
                }

                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #endregion Update Settings

                #region Commit

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete Project Information.";
                    throw new ArgumentNullException("", "");
                }

                #endregion Commit
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }

        public DataTable DataTableLoad(string tableName, string[] Condition, string DatabaseName)
        {
            DataTable dt = new DataTable("");
            SqlConnection currConn = null;
            try
            {
                #region New open connection and transaction

                #endregion New open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                    currConn.Open();
                if (DatabaseName != null)
                {
                    currConn.ChangeDatabase(DatabaseName);
                }

                string sql = "";
                sql = sql + @" select  * from " + tableName + "";
                if (Condition != null)
                {
                    if (Condition.Count() > 0)
                        sql = sql + @" where ";
                    for (int i = 0; i < Condition.Length; i++)
                    {
                        if (i > 0)
                            sql = sql + @" and ";
                        sql = sql + Condition[i];
                    }
                }

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sql;
                objComm.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dt);
                return dt;

                #region Commit

                #endregion Commit
            }
            catch (Exception)
            {
                return dt;
            }
            finally
            {
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public string SelectFieldValue(string tableName, string fieldName, string conditionField, string conditionValue,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            string retFieldValue = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = "";
                sqlText += " SELECT Top 1 ";
                sqlText += " Id, ";
                sqlText += fieldName;
                sqlText += " From ";
                sqlText += tableName;
                sqlText += " WHERE 1=1";
                sqlText += " AND " + conditionField + "=@conditionValue";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@conditionValue", conditionValue);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    retFieldValue = dr[fieldName].ToString();
                }

                dr.Close();

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retFieldValue;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return retFieldValue;

            #endregion
        }

        public string NextCode(string tableName, string fieldName, string Branch, string TransDateMMyyS,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            string retFieldValue = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = "";
                sqlText += @" select  isnull( max(cast(left(right( " + fieldName + ",len(" + fieldName +
                           ")-2),CHARINDEX('/',right( " + fieldName + ",len(" + fieldName +
                           ")-2))-1) as int)),0)+1   Code";
                sqlText += @" from " + tableName + "";
                sqlText += @" where left( " + fieldName + ",2)='" + Branch.PadLeft(2, '0') + "'";
                sqlText += @" and right( " + fieldName + ",3)='" + TransDateMMyyS.ToString().Substring(2, 3) + "'";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    retFieldValue = dr[fieldName].ToString();
                }

                dr.Close();
                if (string.IsNullOrWhiteSpace(retFieldValue))
                {
                    retFieldValue = "1";
                }

                retFieldValue = Branch.PadLeft(2, '0') + retFieldValue.PadLeft(5, '0') + "/" + TransDateMMyyS;

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retFieldValue;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return retFieldValue;

            #endregion
        }

        public string[] FieldPost(string tableName, string conditionField, string conditionValue,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("PostTo" + tableName);
                }

                #endregion open connection and transaction

                #region Check is  it used

                #endregion Check is  it used

                #region Update Settings

                sqlText = "";
                sqlText = "UPDATE " + tableName + " SET";
                sqlText += " Post=@Post";
                sqlText += " WHERE 1=1 AND";
                sqlText += " " + conditionField + "=@ConditionValue";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Post", true);
                cmdUpdate.Parameters.AddWithValue("@ConditionValue", conditionValue);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    //////throw new ArgumentNullException("Post", conditionValue + " could not Post.");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Posted Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] FieldUpdate(string tableName, string field, string conditionField, string conditionValue,
            SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("PostTo" + tableName);
                }

                #endregion open connection and transaction

                #region Check is  it used

                #endregion Check is  it used

                #region Update Settings

                sqlText = "";
                sqlText = "UPDATE " + tableName + " SET ";
                sqlText += field + "=@Post";
                sqlText += " WHERE 1=1 AND";
                sqlText += " " + conditionField + "=@ConditionValue";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Post", true);
                cmdUpdate.Parameters.AddWithValue("@ConditionValue", conditionValue);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    //////throw new ArgumentNullException("Post", conditionValue + " could not Post.");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Posted Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }


        public string[] FieldUpdate(string tableName, string updateField, string updatedValue, string conditionField,
            string conditionValue, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("UpdateTo" + tableName);
                }

                #endregion open connection and transaction

                #region Update Settings

                sqlText = "";
                sqlText += "UPDATE " + tableName + " SET ";
                sqlText += updateField + " =@updatedValue ";
                sqlText += " WHERE 1=1 AND ";
                sqlText += conditionField + "=@conditionValue";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@updatedValue", updatedValue);
                cmdUpdate.Parameters.AddWithValue("@conditionValue", conditionValue);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Update", conditionValue + " could not Update.");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Updated Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Upadate " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] FieldUpdateByMultiCondition(string tableName, string updateField, string updatedValue,
            string[] conditionFields, string[] conditionValues, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("UpdateTo" + tableName);
                }

                #endregion open connection and transaction

                #region Update Settings

                sqlText = "";
                sqlText += "UPDATE " + tableName + " SET ";
                sqlText += updateField + " =@updatedValue ";
                sqlText += " WHERE 1=1 ";
                int i = 0;
                if (conditionFields != null && conditionValues != null)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        sqlText += " AND " + conditionFields[i] + "=@" + conditionFields[i];
                        i++;
                    }
                }

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                i = 0;
                if (conditionFields != null && conditionValues != null)
                {
                    foreach (string item in conditionFields)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        cmdUpdate.Parameters.AddWithValue("@" + conditionFields[i], conditionValues[i]);
                        i++;
                    }
                }

                cmdUpdate.Parameters.AddWithValue("@updatedValue", updatedValue);
                var exeRes = cmdUpdate.ExecuteNonQuery();


                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Update - could not Update.", "");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Updated Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Update " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] InsertThreads(string value, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region Validation

                #endregion Validation

                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = "  ";
                sqlText += @" INSERT INTO Threads(TValue  ) 
                                VALUES ( @TValue ) 
                                        ";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Parameters.AddWithValue("@TValue", value);
                cmdInsert.Transaction = transaction;
                cmdInsert.ExecuteNonQuery();

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = "1";

                #endregion SuccessResult
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion
        }

        public DataSet DataSetLoadByQuery(string SQLQuery, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            DataSet ds = new DataSet();

            #region Initializ

            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                string sql = SQLQuery;

                SqlDataAdapter sqlAdpt = new SqlDataAdapter(sql, currConn);
                sqlAdpt.SelectCommand.Transaction = transaction;
                sqlAdpt.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                return ds;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }
        }

        public string[] RESEED_Table_IDENTITY(string tableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("PostTo" + tableName);
                }

                #endregion open connection and transaction

                #region Update Settings

                sqlText = "";
                sqlText += @"DECLARE @MaxId as int";

                sqlText += @" SELECT @MaxId = ISNULL(MAX(Id),0)  FROM " + tableName;

                sqlText += @" DBCC CHECKIDENT ('[" + tableName + "]', RESEED, @MaxId)";
                sqlText += @"GO ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    //////throw new ArgumentNullException("Post", conditionValue + " could not Post.");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Table Identity Reseed Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Reseed " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }


        private List<ColumnDetails> ValidateColumnNames(string tableName, DataTable data, SqlConnection currConn,
            SqlTransaction transaction)
        {
            if (tableName.Contains("#"))
            {
                return new List<ColumnDetails>();
            }


            string sqlText = @"SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = N'" + tableName + "'";
            ;

            SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dtcolumnNames = new DataTable();
            adapter.Fill(dtcolumnNames);

            List<ColumnDetails> columnDetails =
                JsonConvert.DeserializeObject<List<ColumnDetails>>(JsonConvert.SerializeObject(dtcolumnNames));

            foreach (DataColumn dataColumn in data.Columns)
            {
                ColumnDetails vm = columnDetails.SingleOrDefault(x => x.COLUMN_NAME == dataColumn.ColumnName);

                if (vm == null)
                {
                    throw new Exception(
                        "'" + dataColumn.ColumnName + "' Column Does Not Match With the Given Table");
                }
            }

            return columnDetails;
        }

        public string CodeGenerationPF(string TransactionTypeGroup, string TransactionType, string Date
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ

            string sqlText = "";
            int Id = 0;
            string retResults = "";
            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query//4 - catch ex//5 - Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                string year = "";
                string GENERATEDCODE = "0";
                DateTime TransactionDate = DateTime.Now;

                #region Make Code

                try
                {
                    int CODEGENID = 0;
                    int transResultCodeGen = 0;
                    TransactionDate = Convert.ToDateTime(Date);
                    //new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(TransactionDate, currConn, transaction).Year;
                    year = new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(Ordinary.DateToString( TransactionDate.ToString()), currConn, transaction).Year.ToString();

                    string getMaxIdSql = "";

                    getMaxIdSql = @"     declare @id as varchar(10);

  Select @id= id  FROM CodeGenerations WHERE 1=1
AND CYear=@CYear 
AND TransactionType=@TransactionType and TransactionTypeGroup=@TransactionTypeGroup
 
if	(@id is NULL) begin 
insert into CodeGenerations(CYear,	TransactionTypeGroup,	TransactionType,	Prefix,	LastNumber,	TransType)
  Select top 1 @CYear,TransactionTypeGroup,TransactionType, Prefix ,0 ,TransactionTypeGroup FROM CodeGenerations WHERE 1=1
AND TransactionType='JournalVoucher' and TransactionTypeGroup='PF'
  end
";

                    getMaxIdSql += "Select id, Prefix Prefix, isnull(LastNumber,0)  +1 INDENTID ";
                    getMaxIdSql += " FROM CodeGenerations";
                    getMaxIdSql +=" WHERE 1=1 AND CYear=@CYear  AND TransactionType=@TransactionType and TransactionTypeGroup=@TransactionTypeGroup";


                    string INDENTID_STR = "1";
                    string PrefixStr = "";
                    //SqlCommand objMaxIdComm = new SqlCommand(getMaxIdSql, currConn, transaction);
                  

                    //SqlDataReader drMaxId;
                    SqlDataAdapter da = new SqlDataAdapter(getMaxIdSql, currConn);
                    da.SelectCommand.Transaction = transaction;
                    da.SelectCommand.Parameters.AddWithValue("@CYear", year);
                    da.SelectCommand.Parameters.AddWithValue("@TransactionType", TransactionType.ToString() ?? Convert.DBNull);
                    da.SelectCommand.Parameters.AddWithValue("@TransactionTypeGroup", TransactionTypeGroup.ToString() ?? Convert.DBNull);                   

                    da.Fill(dt);

                    INDENTID_STR = dt.Rows[0]["INDENTID"].ToString();
                    PrefixStr = dt.Rows[0]["Prefix"].ToString();
                    CODEGENID =Convert.ToInt32(dt.Rows[0]["id"]);
                    
                    //drMaxId = objMaxIdComm.ExecuteReader();
                    //while (drMaxId.Read())
                    //{
                    //    INDENTID_STR = drMaxId["INDENTID"].ToString();

                    //    PrefixStr = drMaxId["Prefix"].ToString();
                    //    CODEGENID = Convert.ToInt32(drMaxId["id"].ToString());
                    //}

                    GENERATEDCODE = PrefixStr + "-" + INDENTID_STR.PadLeft(4, '0') + "/" +
                                    TransactionDate.ToString("MMyyyy");

                    //drMaxId.Close();

                    //UPDATE GENERATE CODE TABLE////
                    sqlText = "";
                    sqlText = "UPDATE CodeGenerations SET";
                    sqlText += "   LastNumber=@LastNumber";

                    sqlText += " WHERE 1=1 AND Id=@Id";

                    #endregion

                    #region SqlExecution

                    SqlCommand cmdCodeGenUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdCodeGenUpdate.Parameters.AddWithValue("@Id", CODEGENID);
                    cmdCodeGenUpdate.Parameters.AddWithValue("@LastNumber", INDENTID_STR);
                    var exeResCode = cmdCodeGenUpdate.ExecuteNonQuery();
                    transResultCodeGen = Convert.ToInt32(exeResCode);
                }
                catch (Exception ex)
                {
                    retResults = "0"; //Success or Fail

                    return retResults;
                }

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult

                retResults = GENERATEDCODE;

                #endregion SuccessResult
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults = "0"; //Success or Fail
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion
        }   
        
       

        #region Unused Methods

        //public string existCheck(string tableName, string code, int Id, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //{
        //    #region Initializ
        //    string sqlText = "";
        //    int objfoundId = 0;
        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    string[] retResults = new string[6];
        //    retResults[0] = "Fail";//Success or Fail
        //    retResults[1] = "Fail";// Success or Fail Message
        //    retResults[2] = Id.ToString();// Return Id
        //    retResults[3] = sqlText; //  SQL Query
        //    retResults[4] = "ex"; //catch ex
        //    retResults[5] = "InsertProducts"; //Method Name
        //    #endregion
        //    #region Try
        //    try
        //    {
        //        #region open connection and transaction
        //        #region New open connection and transaction
        //        if (VcurrConn != null)
        //        {
        //            currConn = VcurrConn;
        //        }
        //        if (Vtransaction != null)
        //        {
        //            transaction = Vtransaction;
        //        }
        //        #endregion New open connection and transaction
        //        if (currConn == null)
        //        {
        //            currConn = _dbsqlConnection.GetConnection();
        //            if (currConn.State != ConnectionState.Open)
        //            {
        //                currConn.Open();
        //            }
        //        }
        //        if (transaction == null)
        //        {
        //            transaction = currConn.BeginTransaction("");
        //        }
        //        #endregion open connection and transaction
        //        #region Save
        //        //genericExist
        //        //string sqlText = "";
        //        sqlText = "  ";
        //        sqlText += " SELECT COUNT(Id)Id FROM " + tableName + "";
        //        sqlText += " WHERE code=@Code  ";
        //        SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
        //        cmdCodeExist.Parameters.AddWithValue("@Code", code);
        //        cmdCodeExist.Transaction = transaction;
        //        objfoundId = (int)cmdCodeExist.ExecuteScalar();
        //        if (objfoundId > 0)
        //        {
        //            retResults[1] = "Code already used!";
        //            retResults[3] = sqlText;
        //            throw new ArgumentNullException(retResults[1], "");
        //        }
        //        //EndgenericExist
        //        #endregion Save
        //        #region Commit
        //        if (Vtransaction == null)
        //        {
        //            if (transaction != null)
        //            {
        //                transaction.Commit();
        //            }
        //        }
        //        #endregion Commit
        //    }
        //    #endregion try
        //    #region Catch and Finall
        //    catch (Exception ex)
        //    {
        //        if (Vtransaction == null) { transaction.Rollback(); }
        //        return retResults[1];
        //    }
        //    finally
        //    {
        //        if (VcurrConn == null)
        //        {
        //            if (currConn != null)
        //            {
        //                if (currConn.State == ConnectionState.Open)
        //                {
        //                    currConn.Close();
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //    #region Results
        //    return "Proceed";
        //    #endregion
        //}

        #endregion Unused Methods

        public int NextOtherId(string EmpCategory, string EmpJobType, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            int nextCode = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    DBSQLConnection _dbsqlConnection = new DBSQLConnection();
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region Save

                sqlText = @"Select isnull(max(cast(Empd.OtherId as int)),0)+1 from EmployeePersonalDetail Empd
                 left outer join EmployeeJob  Empj on Empd.EmployeeId=Empj.EmployeeId where 1=1";
                if (!string.IsNullOrWhiteSpace(EmpJobType))
                {
                    sqlText += " and Empj.EmpJobType= @EmpJobType";
                }
                if (!string.IsNullOrWhiteSpace(EmpCategory))
                {
                    sqlText += " and Empj.EmpCategory=@EmpCategory";
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (!string.IsNullOrWhiteSpace(EmpJobType))
                {
                    objComm.Parameters.AddWithValue("@EmpJobType", EmpJobType);
                }
                if (!string.IsNullOrWhiteSpace(EmpCategory))
                {
                    objComm.Parameters.AddWithValue("@EmpCategory", EmpCategory);
                }

                var exeRes = objComm.ExecuteScalar();
                nextCode = Convert.ToInt32(exeRes);
                if (nextCode <= 0)
                {
                    throw new ArgumentNullException("Unexeptected Error - Unable to create new Customer No", "");
                }

                #endregion Save

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return nextCode;
            }

            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return nextCode;

            #endregion
        }

        public bool OtherIdExistCheck( string[] conditionFields, string[] conditionValues,
    SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            bool Exist = false;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                #region sql statement
                sqlText = @"Select TOP 1 Empd.EmployeeId from EmployeePersonalDetail Empd
                 left outer join EmployeeJob  Empj on Empd.EmployeeId=Empj.EmployeeId where 1=1";
           
                string cField = "";
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) ||
                            string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }

                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                SqlCommand _objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null &&
                    conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) ||
                            string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }

                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        _objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    Exist = true;
                    break;
                }

                dr.Close();

                #endregion
            }

            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return Exist;
        }

        public string[] BulkInsert(string tableName, DataTable data, SqlConnection VcurrConn = null,
             SqlTransaction Vtransaction = null, int batchSize = 0,
             SqlRowsCopiedEventHandler rowsCopiedCallBack = null, string DbName = "")
        {
            #region Initialize

            string sqlText = "";

            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region try

            try
            {
                #region open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                        if (!string.IsNullOrWhiteSpace(DbName))
                        {
                            currConn.ChangeDatabase(DbName);
                        }
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }

                #endregion open connection and transaction

                List<ColumnDetails> columnDetails = ValidateColumnNames(tableName, data, currConn, transaction);

                using (var sqlBulkCopy = new SqlBulkCopy(currConn, SqlBulkCopyOptions.Default, transaction))
                {
                    sqlBulkCopy.BulkCopyTimeout = 500;
                    sqlBulkCopy.DestinationTableName = tableName;

                    if (batchSize > 0)
                        sqlBulkCopy.BatchSize = batchSize;

                    if (rowsCopiedCallBack != null)
                    {
                        sqlBulkCopy.NotifyAfter = 500;
                        sqlBulkCopy.SqlRowsCopied += rowsCopiedCallBack;
                    }


                    foreach (DataColumn column in data.Columns)
                    {
                        sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    sqlBulkCopy.WriteToServer(data);
                }

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Data Imported Successfully.";
                retResults[2] = "";

                #endregion SuccessResult
            }

            #endregion

            #region Catch and Finally

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }


                throw ex;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            #region Results

            return retResults;

            #endregion
        }
        public string[] FielApproved(string tableName, string conditionField, string conditionValue,
         SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("PostTo" + tableName);
                }

                #endregion open connection and transaction

                #region Check is  it used

                #endregion Check is  it used

                #region Update Settings

                sqlText = "";
                sqlText = "UPDATE " + tableName + " SET";
                sqlText += " IsApprove=@IsApprove";
                sqlText += " ,ApprovedBy=@ApprovedBy";
                sqlText += " ,ApprovedAt=@ApprovedAt";
                sqlText += " WHERE 1=1 AND";
                sqlText += " " + conditionField + "=@ConditionValue";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@IsApprove", true);
                cmdUpdate.Parameters.AddWithValue("@ConditionValue", conditionValue);
                cmdUpdate.Parameters.AddWithValue("@ApprovedBy", identity.FullName);
                cmdUpdate.Parameters.AddWithValue("@ApprovedAt", DateTime.Now);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    //////throw new ArgumentNullException("Post", conditionValue + " could not Post.");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Posted Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }

        public string[] FielReject(string tableName, string conditionField, string conditionValue,
     SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; //Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0"; // Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Post" + tableName; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

            #endregion

            try
            {
                #region open connection and transaction

                #region New open connection and transaction

                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }

                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

                #endregion New open connection and transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("PostTo" + tableName);
                }

                #endregion open connection and transaction

                #region Check is  it used

                #endregion Check is  it used

                #region Update Settings

                sqlText = "";
                sqlText = "UPDATE " + tableName + " SET";
                sqlText += " IsApprove=@IsApprove";
                sqlText += " ,Post=@Post";
                sqlText += " WHERE 1=1 AND";
                sqlText += " " + conditionField + "=@ConditionValue";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@IsApprove", false);
                cmdUpdate.Parameters.AddWithValue("@Post", false);
                cmdUpdate.Parameters.AddWithValue("@ConditionValue", conditionValue);
                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                retResults[2] = ""; // Return Id
                retResults[3] = sqlText; //  SQL Query

                #region Commit

                if (transResult <= 0)
                {
                    //////throw new ArgumentNullException("Post", conditionValue + " could not Post.");
                }

                #endregion Commit

                #endregion Update Settings

                iSTransSuccess = true;
                if (iSTransSuccess == true)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Rejected Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post " + tableName + ".";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch

            catch (Exception ex)
            {
                retResults[0] = "Fail"; //Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null)
                {
                    if (currConn != null)
                    {
                        if (currConn.State == ConnectionState.Open)
                        {
                            currConn.Close();
                        }
                    }
                }
            }

            #endregion

            return retResults;
        }
    }
}