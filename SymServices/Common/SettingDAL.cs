using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace SymServices.Common
{
    public class SettingDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion
        #region Methods
        public DataSet SearchSettings()
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettings");


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

                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM Settings
                                      ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM Settings s ORDER BY s.SettingGroup;
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataSet);

                #endregion
            }
            #region catch

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return dataSet;
        }
        public string[] SettingsUpdate(List<SettingsVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";
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
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                if (VMs.Any())
                {
                    foreach (var item in VMs)
                    {
                        #region Update Settings
                        sqlText = "";
                        sqlText += "update Setting set";
                        sqlText += " SettingValue='" + item.SettingValue + "',";
                        sqlText += " IsActive='" + item.IsActive + "',";
                        sqlText += " LastModifiedBy='" + UserInfoVM.UserName + "',";
                        sqlText += " LastModifiedOn='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " where SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "'";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        #region Commit

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                    }

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SettingsUpdate", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                    {
                        transaction.Commit();
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Requested Settings Information Successfully Updated.";
                    retResults[2] = "";

                }
                else
                {
                    if (Vtransaction == null) { transaction.Rollback(); }
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update settings.";
                    retResults[2] = "";
                }

            }
            #region catch

            catch (SqlException sqlex)
            {
                if (transaction != null)
                    if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    if (Vtransaction == null) { transaction.Rollback(); }
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public string[] settingsDataUpdate(SettingsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Bank Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToBank"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update Setting set";
                    sqlText += " BranchId=@BranchId";
                    sqlText += " , SettingValue=@SettingValue";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where SettingGroup=@SettingGroup and  SettingName=@SettingName";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@SettingGroup", vm.SettingGroup.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingName", vm.SettingName.Trim());
                    cmdUpdate.Parameters.AddWithValue("@SettingValue", vm.SettingValue.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    //retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", BankVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Setting Update", "Could not found any item.");
                }
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
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update Setting.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
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
        public List<SettingsVM> SettingsAll(int branchID = 0)
        {
            List<SettingsVM> settingsVMs = new List<SettingsVM>();
            SettingsVM settingsVM;
            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                sqlText = @"SELECT
                            Id
                            ,BranchId
                            ,SettingGroup
                            ,SettingName
                            ,SettingValue
                            ,SettingType
                            ,Remarks
                            FROM Setting 
WHERE 1=1 AND IsArchive=0 ";

                if (branchID > 0)
                {
                    sqlText += " AND BranchId=@branchID";
                }

                    sqlText += " ORDER BY SettingGroup, SettingName ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                if (branchID > 0)
                {
                    cmd.Parameters.AddWithValue("@BranchId", branchID);
                }
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        settingsVM = new SettingsVM();
                        settingsVM.Id = dr["Id"].ToString();
                        settingsVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        settingsVM.SettingGroup = dr["SettingGroup"].ToString();
                        settingsVM.SettingName = dr["SettingName"].ToString();
                        settingsVM.SettingValue = dr["SettingValue"].ToString();
                        settingsVM.SettingType = dr["SettingType"].ToString();
                        settingsVM.Remarks = dr["Remarks"].ToString();
                        settingsVMs.Add(settingsVM);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return settingsVMs;
        }
        public decimal FormatingNumeric(decimal value, int DecPlace)
        {
            object outPutValue = 0;
            string decPointLen = "";
            try
            {
                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }
                if (value < 1000)
                {
                    var a = "0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
            }
            #region Catch
            //catch (IndexOutOfRangeException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (NullReferenceException ex)
            //{
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (FormatException ex)
            //{

            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}

            //catch (SoapHeaderException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }

            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //catch (SoapException ex)
            //{
            //    string exMessage = ex.Message;
            //    if (ex.InnerException != null)
            //    {
            //        exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
            //                    ex.StackTrace;

            //    }
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            //}
            //catch (EndpointNotFoundException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            //catch (WebException ex)
            //{
            //    MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    FileLogger.Log("Program", "FormatTextBoxQty4", ex.Message + Environment.NewLine + ex.StackTrace);
            //}
            catch (Exception ex)
            {
                //string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            #endregion Catch

            return Convert.ToDecimal(outPutValue);
        }
        public string settingValue(string settingGroup, string settingName, SqlConnection VcurrConn = null
            , SqlTransaction Vtransaction = null, string DBName="")
        {

            string retResults = "0";

            SqlConnection currConn = null;
            string sqlText = "";
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                sqlText += " SELECT SettingValue FROM Setting ";
                sqlText += " WHERE SettingGroup='" + settingGroup + "' AND SettingName='" + settingName + "'";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        retResults = dr["SettingValue"].ToString();                     
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return retResults;
        }
        public string[] settingsDataInsert(SettingsVM vm, string settingGroup, string settingName, string settingType, string settingValue)
        {
            string[] retResults = new string[6];
            //SettingsVM vm = new SettingsVM();
            vm.SettingGroup = settingGroup;
            vm.SettingName = settingName;
            vm.SettingType = settingType;
            vm.SettingValue = settingValue;
            retResults = InsertSettingsData(vm, null, null);
            return retResults;
        }
        #endregion Methods

        #region Unused
        public string[] InsertSettingsData(SettingsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Insert"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(vm.SettingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(vm.SettingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(vm.SettingType))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(vm.SettingValue))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }

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
                    transaction = currConn.BeginTransaction("Insert");
                }
                #endregion open connection and transaction

                #region Exist Check
                CommonDAL _cDal = new CommonDAL();
                string[] conditionField = { "SettingGroup", "SettingName" };
                string[] conditionValue = { vm.SettingGroup.Trim(), vm.SettingName.Trim() };
                bool exist = _cDal.ExistCheck("Setting", conditionField, conditionValue, currConn, transaction);
                if (!exist)
                {

                #endregion Exist Check

                    #region SettingsExist
                    sqlText = @"Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Setting
                    where 1=1 -- and BranchId=@BranchId";
                    SqlCommand cmd111 = new SqlCommand(sqlText, currConn);
                    //cmd111.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd111.Transaction = transaction;
                    var exeRes = cmd111.ExecuteScalar();
                    int count = Convert.ToInt32(exeRes);

                    //vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                    vm.Id =  "1_" + (count + 1);

                    #endregion ProductExist

                    #region Last Settings

                    if (true)
                    {
                        sqlText = "  ";
                        sqlText += @" INSERT INTO Setting(
Id
,BranchId
,SettingGroup
,SettingName
,SettingValue
,SettingType
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
 @Id
,@BranchId
,@SettingGroup
,@SettingName
,@SettingValue
,@SettingType
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
)";

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmdInsert.Parameters.AddWithValue("@SettingGroup", vm.SettingGroup);
                        cmdInsert.Parameters.AddWithValue("@SettingName", vm.SettingName);
                        cmdInsert.Parameters.AddWithValue("@SettingValue", vm.SettingValue);
                        cmdInsert.Parameters.AddWithValue("@SettingType", vm.SettingType);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                        cmdInsert.ExecuteNonQuery();
                        if (Vtransaction == null)
                        {
                            if (transaction != null)
                            {
                                transaction.Commit();
                            }
                        }
                        retResults[0] = "Success";
                        retResults[1] = "Data Save Successfully.";
                        retResults[2] = "0";
                    }
                    #endregion Last Price
                }
                else
                {
                    retResults[0] = "Success";
                    retResults[1] = "Data Save Successfully.";
                    retResults[2] = "0";
                }
            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                retResults[4] = ex.Message.ToString();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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
        public void SettingsUpdate(string companyId)
        {
            #region Variables

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            #endregion

            try
            {
                #region open connection and transaction

                CommonDAL commonDal = new CommonDAL();
                #endregion open connection and transaction

            }
            #region catch

            catch (SqlException sqlex)
            {

                throw sqlex;
            }
            catch (ArgumentNullException sqlex)
            {

                throw sqlex;

            }
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion

        }
        public string settingsDataDelete(string settingGroup, string settingName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }


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


                #endregion open connection and transaction

                #region SettingsExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE REPLACE(SettingGroup,' ','')='" + settingGroup + "' AND SettingName='" + settingName + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Settings

                int foundId = (int)objfoundId;
                if (foundId > 0)
                {
                    sqlText = "  ";
                    sqlText += " DELETE FROM Settings";
                    sqlText += " WHERE REPLACE(SettingGroup,' ','')='" + settingGroup + "' ";
                    sqlText += " AND SettingName='" + settingName + "'";


                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                    cmdExist1.Transaction = transaction;
                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }



                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public string settingsDataUpdate(string settingGroup, string settingName, string settingGroupNew, string settingNameNew, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Initializ

            string retResults = "0";
            string sqlText = "";

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(settingGroup))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                else if (string.IsNullOrEmpty(settingName))
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
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

                #region ProductExist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM Settings ";
                sqlText += " WHERE SettingGroup='" + settingGroup + "' AND SettingName='" + settingName + "' ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                object objfoundId = cmdExist.ExecuteScalar();
                if (objfoundId == null)
                {
                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                }
                #endregion ProductExist
                #region Last Price

                int foundId = (int)objfoundId;
                if (foundId > 0)
                {
                    sqlText = "";
                    sqlText = "update Settings set";
                    sqlText += " SettingName='" + settingNameNew + "',";
                    sqlText += " SettingValue='" + settingGroupNew + "'";
                    sqlText += " where SettingGroup='" + settingGroup + "' and SettingName='" + settingName + "'";

                    //sqlText += " where SettingId='" + item.SettingId + "'" + " and SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "'";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Transaction = transaction;
                    object objfoundId1 = cmdUpdate.ExecuteNonQuery();

                    if (objfoundId1 == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int save = (int)objfoundId1;
                    if (save <= 0)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }



                }


                #endregion Last Price

            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        #endregion Unused

    }
}
