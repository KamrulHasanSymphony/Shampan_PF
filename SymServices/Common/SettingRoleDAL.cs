
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
    public class SettingRoleDAL
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        #endregion

       
        public DataSet SearchSettingsRole()
        {

            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            //DataSet dataTable = new DataTable("Search Settings");
            DataSet dataSet = new DataSet("SearchSettingsRole");
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                //transaction = currConn.BeginTransaction("InsertSettingsRoll");

                #endregion open connection and transaction

                #region sql statement search from settings

                sqlText = @" Select * from Settings
                                 ORDER BY SettingGroup,SettingName;
";

                SqlCommand cmdSettingRole = new SqlCommand();
                cmdSettingRole.Connection = currConn;
                cmdSettingRole.CommandText = sqlText;
                cmdSettingRole.CommandType = CommandType.Text;
                DataTable dt = new DataTable("Search Settings");
                
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdSettingRole);
                dataAdapter.Fill(dt);
                foreach (DataRow item in dt.Rows)
                {

                    sqlText = "  ";
                    sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
                    sqlText += " WHERE SettingGroup='" + item["SettingGroup"].ToString()+ "' AND SettingName='" + item["SettingName"].ToString() + "'";
                    sqlText += " AND SettingType='" + item["SettingType"].ToString() + "' AND UserId='" + UserInfoVM.UserId + "'";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    object objfoundId = cmdExist.ExecuteScalar();
                    if (objfoundId == null)
                    {
                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                    }
                    int found = (int) objfoundId;
                    if (found<=0)// not exist
                    {
                        sqlText = "  ";
                        sqlText +=
                            " INSERT INTO SettingsRole(	SettingGroup,SettingName,SettingValue,SettingType,UserID,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
                        sqlText += " VALUES(";
                        sqlText += " '" + item["SettingGroup"].ToString() + "',";
                        sqlText += " '" + item["SettingName"].ToString()+ "',";
                        sqlText += " '" + item["SettingValue"].ToString() + "',";
                        sqlText += " '" + item["SettingType"].ToString()+ "',";
                        sqlText += " '" + UserInfoVM.UserId + "',";
                        sqlText += " 'Y',";
                        sqlText += " '" + UserInfoVM.UserName + "',";
                        sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                        sqlText += " '" + UserInfoVM.UserName + "',";
                        sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        sqlText += " )";

                        SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
                        cmdExist1.Transaction = transaction;
                        object objfoundId1 = cmdExist1.ExecuteNonQuery();
                        if (objfoundId1 == null)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                        transResult = (int)objfoundId1;
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
                        }
                    }
                   

                }
               
                sqlText = @"SELECT [SettingId]
                                      ,[SettingGroup]
                                      ,[SettingName]
                                      ,[SettingValue]
                                      ,[SettingType]
                                      ,[ActiveStatus]
                                      FROM SettingsRole where UserID=@userId
                                     ORDER BY SettingGroup,SettingName;
SELECT DISTINCT s.SettingGroup FROM SettingsRole s ORDER BY s.SettingGroup;
";

                SqlCommand cmdSettingRole1 = new SqlCommand();
                cmdSettingRole1.Connection = currConn;
                cmdSettingRole1.CommandText = sqlText;
                cmdSettingRole1.CommandType = CommandType.Text;

                if (!cmdSettingRole1.Parameters.Contains("@userId"))
                { cmdSettingRole1.Parameters.AddWithValue("@userId", UserInfoVM.UserId); }
                else { cmdSettingRole1.Parameters["@userId"].Value = UserInfoVM.UserId; }



                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(cmdSettingRole1);
                dataAdapter1.Fill(dataSet);

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
        //public string[] SettingsUpdate(List<SettingsVM> settingsVM)
        //{
        //    #region Variables

        //    string[] retResults = new string[3];
        //    retResults[0] = "Fail";
        //    retResults[1] = "Fail";
        //    retResults[2] = "";

        //    SqlConnection currConn = null;
        //    SqlTransaction transaction = null;
        //    int transResult = 0;
        //    int countId = 0;
        //    string sqlText = "";

        //    bool iSTransSuccess = false;

        //    #endregion

        //    try
        //    {

        //        #region open connection and transaction

        //        currConn = _dbsqlConnection.GetConnection();
        //        if (currConn.State != ConnectionState.Open)
        //        {
        //            currConn.Open();
        //        }

        //        if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSettings"); }

        //        #endregion open connection and transaction
        // //int tt = 0;

        //        if (settingsVM.Any())
        //        {
        //            foreach (var item in settingsVM)
        //            {
        //               #region SettingsExist
        //                sqlText = "  ";
        //                sqlText += " SELECT COUNT(DISTINCT SettingId)SettingId FROM SettingsRole ";
        //                sqlText += " WHERE SettingGroup='" + item.SettingGroup + "' AND SettingName='" + item.SettingName + "' AND SettingType='" + item.SettingType + "' AND UserId='" + UserInfoVM.UserId + "'";
        //                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
        //                cmdExist.Transaction = transaction;
        //                object objfoundId = cmdExist.ExecuteScalar();
        //                if (objfoundId == null)
        //                {
        //                    throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
        //                }
        //                #endregion SettingsExist


        //                #region Last Settings

        //                int foundId = (int)objfoundId;
        //                if (foundId <= 0)
        //                {
        //                    sqlText = "  ";
        //                    sqlText +=
        //                        " INSERT INTO SettingsRole(	SettingGroup,SettingName,SettingValue,SettingType,UserID,ActiveStatus,CreatedBy,CreatedOn,LastModifiedBy,LastModifiedOn)";
        //                    sqlText += " VALUES(";
        //                    sqlText += " '" + item.SettingGroup + "',";
        //                    sqlText += " '" + item.SettingName + "',";
        //                    sqlText += " '" + item.SettingValue + "',";
        //                    sqlText += " '" + item.SettingType + "',";
        //                    sqlText += " '" + UserInfoVM.UserId + "',";
        //                    sqlText += " 'Y',";
        //                    sqlText += " '" + UserInfoVM.UserName + "',";
        //                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
        //                    sqlText += " '" + UserInfoVM.UserName + "',";
        //                    sqlText += " '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        //                    sqlText += " )";

        //                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);
        //                    cmdExist1.Transaction = transaction;
        //                    object objfoundId1 = cmdExist1.ExecuteNonQuery();
        //                    if (objfoundId1 == null)
        //                    {
        //                        throw new ArgumentNullException("settingsDataInsert", "Please Input Settings Value");
        //                    }
        //                    transResult = (int) objfoundId1;
        //                }
        //                    #endregion Last Price

        //                else
        //                {
        //                    #region Update Settings

        //                    sqlText = "";
        //                    sqlText += "update SettingsRole set";
        //                    sqlText += " SettingValue='" + item.SettingValue + "',";
        //                    sqlText += " ActiveStatus='" + item.ActiveStatus + "'";
        //                    //sqlText += " LastModifiedBy='" + item.LastModifiedBy + "',";
        //                    //sqlText += " LastModifiedOn='" + item.LastModifiedOn + "'";
        //                    sqlText += " where SettingGroup='" + item.SettingGroup + "' and SettingName='" + item.SettingName + "' " +
        //                                    " AND UserId='" + UserInfoVM.UserId + "'";

        //                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
        //                    cmdUpdate.Transaction = transaction;
        //                    var exeRes = cmdUpdate.ExecuteNonQuery();
        //                    transResult = Convert.ToInt32(exeRes);
        //                    #endregion Update Settings

        //                }
                       

        //                #region Commit

        //                if (transResult <= 0)
        //                {
        //                    throw new ArgumentNullException("SettingsUpdate", item.SettingName + " could not updated.");
        //                }

        //                #endregion Commit

        //            }

        //            iSTransSuccess = true;
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException("SettingsUpdate", "Could not found any item.");
        //        }


        //        if (iSTransSuccess == true)
        //        {
        //            transaction.Commit();
        //            retResults[0] = "Success";
        //            retResults[1] = "Requested Settings Information Successfully Updated.";
        //            retResults[2] = "";

        //        }
        //        else
        //        {
        //            if (Vtransaction == null) { transaction.Rollback(); }
        //            retResults[0] = "Fail";
        //            retResults[1] = "Unexpected error to update settings.";
        //            retResults[2] = "";
        //        }

        //    }
        //    #region catch

        //    catch (SqlException sqlex)
        //    {
        //        if (transaction != null)
        //            if (Vtransaction == null) { transaction.Rollback(); }
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null)
        //            if (Vtransaction == null) { transaction.Rollback(); }
        //        throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //    }
        //    finally
        //    {
        //        if (currConn != null)
        //        {
        //            if (currConn.State == ConnectionState.Open)
        //            {
        //                currConn.Close();
        //            }
        //        }
        //    }

        //    #endregion

        //    return retResults;
        //}
        public bool CheckUserAccess()
        {
            bool isAlloweduser = false;
            CommonDAL commonDal=new CommonDAL();

            bool isAccessTransaction =
                Convert.ToBoolean(commonDal.settings("Transaction", "AccessTransaction") == "Y" ? true : false);
            if (!isAccessTransaction)
            {
                string userName = commonDal.settings("Transaction", "AccessUser");
                if (userName.ToLower() == UserInfoVM.UserName.ToLower())
                {
                    isAlloweduser = true;
                }
            }
            else
            {
                isAlloweduser = true;
            }
            return isAlloweduser;
        }

    }
}
