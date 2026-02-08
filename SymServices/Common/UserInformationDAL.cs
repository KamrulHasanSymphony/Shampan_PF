using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using SymViewModel.Common;
using SymOrdinary;
using SymViewModel.HRM;
using Newtonsoft.Json;
using SymphonySofttech.Utilities;
namespace SymServices.Common
{
    public class UserInformationDAL
    {
        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        #endregion
        #region New Methods

        //==================SelectAll=================
        public List<UserLogsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<UserLogsVM> VMs = new List<UserLogsVM>();
            UserLogsVM vm;
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

                sqlText = @"
SELECT
u.Id
,u.GroupId
,u.FullName
,u.Email
,u.LogId
,u.Password
,u.VerificationCode
,u.BranchId
,u.EmployeeId
,u.IsAdmin
,u.IsActive
,u.IsVerified
,u.IsArchived
,u.CreatedBy
,u.CreatedAt
,u.CreatedFrom
,u.LastUpdateBy
,u.LastUpdateAt
,u.LastUpdateFrom

FROM [User] u 
WHERE  1=1

";


                if (Id > 0)
                {
                    sqlText += @" and u.Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]))
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]))
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }


                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dtUser = new DataTable();
                da.Fill(dtUser);
                string JSONString = "";
                JSONString = JsonConvert.SerializeObject(dtUser);
                VMs = JsonConvert.DeserializeObject<List<UserLogsVM>>(JSONString);


                

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
            return VMs;
        }
        
        
        public Tuple<bool, UserLogsVM> UserLogIn(UserLogsVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            UserLogsVM userLogsVM = new UserLogsVM();
            bool isLogin = false;
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
                sqlText = @"Select 
 [User].Id
,[User].LogId
,[User].BranchId
,[User].FullName
,[User].Email
,[User].EmployeeId
,isnull(ug.IsAdmin,0)IsAdmin
,isnull(ug.IsESS,0)IsESS
,isnull(ug.IsHRM,0)IsHRM
,isnull(ug.IsAttendance,0)IsAttendance
,isnull(ug.IsPayroll,0)IsPayroll
,isnull(ug.IsTAX,0)IsTAX
,isnull(ug.IsPF,0)IsPF
,isnull(ug.IsGF,0)IsGF


,EmployeeInfo.Code EmployeeCode
,EmployeeInfo.PhotoName PhotoName
    From [User]  left outer join EmployeeInfo on EmployeeInfo.Id=[User].EmployeeId
	left outer join UserGroup ug on [User].GroupId=ug.Id
where  [User].LogId=@LogId and [User].Password=@Password and [User].IsActive=@IsActive and [User].IsVerified=@IsVerified and [User].IsArchived=@IsArchived
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@LogId", vm.LogID);
                objComm.Parameters.AddWithValue("@Password", vm.Password);             
                objComm.Parameters.AddWithValue("@IsActive", true);
                objComm.Parameters.AddWithValue("@IsVerified", true);
                objComm.Parameters.AddWithValue("@IsArchived", false);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    userLogsVM.Id = dr["Id"].ToString();
                    userLogsVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    userLogsVM.LogID = dr["LogId"].ToString().Trim();
                    userLogsVM.FullName = dr["FullName"].ToString().Trim();
                    userLogsVM.Email = dr["Email"].ToString();
                    userLogsVM.EmployeeId = dr["EmployeeId"].ToString();
                    userLogsVM.EmployeeCode = dr["EmployeeCode"].ToString();
                    userLogsVM.PhotoName = dr["PhotoName"].ToString();
                    userLogsVM.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    userLogsVM.IsESS = Convert.ToBoolean(dr["IsESS"]);
                    userLogsVM.IsHRM = Convert.ToBoolean(dr["IsHRM"]);
                    userLogsVM.IsAttenance = Convert.ToBoolean(dr["IsAttendance"]);
                    userLogsVM.IsPayroll = Convert.ToBoolean(dr["IsPayroll"]);
                    userLogsVM.IsTAX = Convert.ToBoolean(dr["IsTAX"]);
                    userLogsVM.IsPF = Convert.ToBoolean(dr["IsPF"]);
                    userLogsVM.IsGF = Convert.ToBoolean(dr["IsGF"]);
                    isLogin = true;
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
            return Tuple.Create(isLogin, userLogsVM);
        }
        
        public string[] InsertToUserInformationNew(string UserID, string UserName, string UserPassword, string ActiveStatus, string LastLoginDateTime, string CreatedBy, string CreatedOn, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
            #region Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            #endregion
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user name.");
                }
                if (string.IsNullOrEmpty(UserPassword))
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Please enter user password.");
                }
                #endregion Validation
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("InsertToUserInformationNew"); }
                #endregion open connection and transaction
                #region UserName existence checking
                //select @Present = count(distinct UserName) from UserInformations where  UserName=@UserName;
                sqlText = "select count(distinct UserName) from UserInformations where  UserName = '" + UserName + "'";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                var exeRes = userNameExist.ExecuteScalar();
                countId = Convert.ToInt32(exeRes);
                if (countId > 0)
                {
                    throw new ArgumentNullException("InsertToUserInformationNew", "Same user name already used.");
                }
                #endregion UserName existence checking
                #region User new id generation
                //select @UserID= isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations;
                sqlText = "select isnull(max(cast(UserID as int)),0)+1 FROM  UserInformations";
                SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                cmdNextId.Transaction = transaction;
                exeRes = cmdNextId.ExecuteScalar();
                int nextId = Convert.ToInt32(exeRes);
                if (nextId <= 0)
                {
                    throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                }
                #endregion User new id generation
                #region Insert new user
                sqlText = "";
                sqlText += "insert into UserInformations";
                sqlText += "(";
                sqlText += "UserID,";
                sqlText += "UserName,";
                sqlText += "UserPassword,";
                sqlText += "ActiveStatus,";
                sqlText += "LastLoginDateTime,";
                sqlText += "CreatedBy,";
                sqlText += "CreatedOn,";
                sqlText += "LastModifiedBy,";
                sqlText += "LastModifiedOn";
                sqlText += ")";
                sqlText += " values(";
                sqlText += "'" + nextId + "',";
                sqlText += "'" + UserName + "',";
                sqlText += "'" + UserPassword + "',";
                sqlText += "'" + ActiveStatus + "',";
                sqlText += "'" + LastLoginDateTime + "',";
                sqlText += "'" + CreatedBy + "',";
                sqlText += "'" + CreatedOn + "',";
                sqlText += "'" + LastModifiedBy + "',";
                sqlText += "'" + LastModifiedOn + "'";
                sqlText += ")";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                exeRes = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information successfully Added.";
                        retResults[2] = "" + nextId;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to add user";
                        retResults[2] = "";
                    }
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to add user ";
                    retResults[2] = "";
                }
                #endregion Commit
                #endregion Insert new user
            }
            #region catch
            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw sqlex;
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
            return retResults;
        }
        
        public string InsertUserLogin(string LogID, string ComputerName, string ComputerLoginUserName,
            string ComputerIPAddress, string SoftwareUserId, string SessionDate, string LogInDateTime,
            string LogOutDateTime)
        {
            #region Variables
            string retResults = string.Empty;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                var tt = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog
                //commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn); //tablename,fieldName, datatype
                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);
                #endregion UserLog
                if (transaction == null) { transaction = currConn.BeginTransaction("InsertToUserInformationNew"); }
                #endregion open connection and transaction
                sqlText = "";
                sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                sqlText += " WHERE ComputerName='" + ComputerName + "'";
                sqlText += " and ComputerLoginUserName='" + ComputerLoginUserName + "'";
                sqlText += " and ComputerIPAddress='" + ComputerIPAddress + "'";
                sqlText += " and SoftwareUserId='" + SoftwareUserId + "'";
                sqlText += " and SessionDate='" + SessionDate + "'";
                sqlText += " and LogInDateTime='" + LogInDateTime + "'";
                sqlText += " and LogOutDateTime='" + LogOutDateTime + "'";
                SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                cmdFindId.Transaction = transaction;
                var exeRes = cmdFindId.ExecuteScalar();
                int IDExist = Convert.ToInt32(exeRes);
                if (IDExist > 0)//update
                {
                    #region Update
                    sqlText = "";
                    sqlText += " UPDATE UserAuditLogs";
                    sqlText += " SET LogOutDateTime = '" + LogOutDateTime + "'";
                    sqlText += " WHERE LogID='" + LogID + "'";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    #region Commit
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults = "" + LogID;
                        }
                        else
                        {
                            transaction.Rollback();
                            retResults = "0";
                        }
                    }
                    else
                    {
                        retResults = "0";
                    }
                    #endregion Commit
                    #endregion Insert new user
                }
                else // insert 
                {
                    #region User new id generation
                    sqlText = "select isnull(max(cast(LogID as int)),0)+1 FROM  UserAuditLogs";
                    SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                    cmdNextId.Transaction = transaction;
                    exeRes = cmdNextId.ExecuteScalar();
                    int vLogID = Convert.ToInt32(exeRes);
                    if (vLogID <= 0)
                    {
                        throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                    }
                    #endregion User new id generation
                    #region Insert new user
                    sqlText = "";
                    sqlText += " INSERT INTO UserAuditLogs";
                    sqlText += " (	LogID,";
                    sqlText += " 	ComputerName,";
                    sqlText += " 	ComputerLoginUserName,";
                    sqlText += " 	ComputerIPAddress,";
                    sqlText += " 	SoftwareUserId,";
                    sqlText += " 	SessionDate,";
                    sqlText += " 	LogInDateTime,";
                    sqlText += " 	LogOutDateTime";
                    sqlText += " )";
                    sqlText += "   VALUES";
                    sqlText += " (";
                    sqlText += "'" + vLogID + "',";
                    sqlText += "'" + ComputerName + "',";
                    sqlText += "'" + ComputerLoginUserName + "',";
                    sqlText += "'" + ComputerIPAddress + "',";
                    sqlText += "'" + SoftwareUserId + "',";
                    sqlText += "'" + SessionDate + "',";
                    sqlText += "'" + LogInDateTime + "',";
                    sqlText += "'" + LogOutDateTime + "'";
                    sqlText += " )";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Transaction = transaction;
                    exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    #endregion Insert new user
                    #region Commit
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                            retResults = "" + vLogID;
                        }
                        else
                        {
                            transaction.Rollback();
                            retResults = "0";
                        }
                    }
                    else
                    {
                        retResults = "0";
                    }
                    #endregion Commit
                }
            }
            #region catch finally
            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResults;
        }
        
        public string InsertUserLogin(List<UserLogsVM> Details, string LogOutDateTime)
        {
            #region Variables
            string retResults = string.Empty;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                var vDatabaseName = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog
                commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction); //tablename,fieldName, datatype
                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);
                #endregion UserLog
                if (transaction == null) { transaction = currConn.BeginTransaction("InsertToUserInformationNew"); }
                #endregion open connection and transaction
                foreach (var Item in Details.ToList())
                {
                    #region IfSameDatabase
                    if (Item.DataBaseName == vDatabaseName)
                    {
                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName='" + Item.ComputerName + "'";
                        sqlText += " and ComputerLoginUserName='" + Item.ComputerLoginUserName + "'";
                        sqlText += " and ComputerIPAddress='" + Item.ComputerIPAddress + "'";
                        sqlText += " and SoftwareUserId='" + Item.SoftwareUserId + "'";
                        sqlText += " and SessionDate='" + Item.SessionDate + "'";
                        sqlText += " and LogInDateTime='" + Item.LogInDateTime + "'";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        var exeRes = cmdFindId.ExecuteScalar();
                        int IDExist = Convert.ToInt32(exeRes);
                        if (IDExist <= 0)//update
                        {
                            #region User new id generation
                            sqlText = "select isnull(max(cast(LogID as int)),0)+1 FROM  UserAuditLogs";
                            SqlCommand cmdNextId = new SqlCommand(sqlText, currConn);
                            cmdNextId.Transaction = transaction;
                            exeRes = cmdNextId.ExecuteScalar();
                            int vLogID = Convert.ToInt32(exeRes);
                            if (vLogID <= 0)
                            {
                                throw new ArgumentNullException("Insert To User Information New", "Unable to create new user");
                            }
                            #endregion User new id generation
                            #region Insert new user
                            sqlText = "";
                            sqlText += " INSERT INTO UserAuditLogs";
                            sqlText += " (	LogID,";
                            sqlText += " 	ComputerName,";
                            sqlText += " 	ComputerLoginUserName,";
                            sqlText += " 	ComputerIPAddress,";
                            sqlText += " 	SoftwareUserId,";
                            sqlText += " 	SessionDate,";
                            sqlText += " 	LogInDateTime";
                            sqlText += " )";
                            sqlText += "   VALUES";
                            sqlText += " (";
                            sqlText += "'" + vLogID + "',";
                            sqlText += "'" + Item.ComputerName + "',";
                            sqlText += "'" + Item.ComputerLoginUserName + "',";
                            sqlText += "'" + Item.ComputerIPAddress + "',";
                            sqlText += "'" + Item.SoftwareUserId + "',";
                            sqlText += "'" + Item.SessionDate + "',";
                            sqlText += "'" + Item.LogInDateTime + "'";
                            sqlText += " )";
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            exeRes = cmdInsert.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);
                            #endregion Insert new user
                            #region Commit
                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();
                                    retResults = "" + vLogID;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";
                                }
                            }
                            else
                            {
                                retResults = "0";
                            }
                            #endregion Commit
                        }
                    }
                    #endregion IfSameDatabase
                    #region IfNotSameDatabase
                    else
                    {
                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName='" + Item.ComputerName + "'";
                        sqlText += " and ComputerLoginUserName='" + Item.ComputerLoginUserName + "'";
                        sqlText += " and ComputerIPAddress='" + Item.ComputerIPAddress + "'";
                        sqlText += " and SoftwareUserId='" + Item.SoftwareUserId + "'";
                        sqlText += " and SessionDate='" + Item.SessionDate + "'";
                        sqlText += " and LogInDateTime='" + Item.LogInDateTime + "'";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        var exeRes = cmdFindId.ExecuteScalar();
                        int IDExist = Convert.ToInt32(exeRes);
                        if (IDExist > 0) //update
                        {
                            #region Update
                            sqlText = "";
                            sqlText += " UPDATE UserAuditLogs";
                            sqlText += " SET LogOutDateTime = '" + Item.LogOutDateTime + "'";
                            sqlText += " WHERE ComputerName='" + Item.ComputerName + "'";
                            sqlText += " and ComputerLoginUserName='" + Item.ComputerLoginUserName + "'";
                            sqlText += " and ComputerIPAddress='" + Item.ComputerIPAddress + "'";
                            sqlText += " and SoftwareUserId='" + Item.SoftwareUserId + "'";
                            sqlText += " and SessionDate='" + Item.SessionDate + "'";
                            sqlText += " and LogInDateTime='" + Item.LogInDateTime + "'";
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            exeRes = cmdInsert.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);
                            #region Commit
                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();
                                    retResults = "" + Item.LogID;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";
                                }
                            }
                            else
                            {
                                retResults = "0";
                            }
                            #endregion Commit
                            #endregion Insert new user
                        }
                    }
                    #endregion IfSameDatabase
                }
            }
            #region catch finally
            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResults;
        }
        
        public string InsertUserLogOut(List<UserLogsVM> Details, string LogOutDateTime)
        {
            #region Variables
            string retResults = string.Empty;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                var vDatabaseName = currConn.Database;
                CommonDAL commonDal = new CommonDAL();
                #region UserLog
                commonDal.TableAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction); //tablename,fieldName, datatype
                commonDal.TableFieldAdd("UserAuditLogs", "LogID", "varchar(50)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerLoginUserName", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "ComputerIPAddress", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SoftwareUserId", "varchar(200)", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "SessionDate", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogInDateTime", "datetime", currConn, transaction);
                commonDal.TableFieldAdd("UserAuditLogs", "LogOutDateTime", "datetime", currConn, transaction);
                #endregion UserLog
                if (transaction == null) { transaction = currConn.BeginTransaction("InsertToUserInformationNew"); }
                #endregion open connection and transaction
                foreach (var Item in Details.ToList())
                {
                    if (Item.DataBaseName == vDatabaseName)
                    {
                        sqlText = "";
                        sqlText += "  select ISNULL( COUNT (DISTINCT logid),0)logid FROM   UserAuditLogs ";
                        sqlText += " WHERE ComputerName='" + Item.ComputerName + "'";
                        sqlText += " and ComputerLoginUserName='" + Item.ComputerLoginUserName + "'";
                        sqlText += " and ComputerIPAddress='" + Item.ComputerIPAddress + "'";
                        sqlText += " and SoftwareUserId='" + Item.SoftwareUserId + "'";
                        sqlText += " and SessionDate='" + Item.SessionDate + "'";
                        sqlText += " and LogInDateTime='" + Item.LogInDateTime + "'";
                        //sqlText += " and LogOutDateTime='" + Item.LogOutDateTime + "'";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        var exeRes = cmdFindId.ExecuteScalar();
                        int IDExist = Convert.ToInt32(exeRes);
                        if (IDExist > 0) //update
                        {
                            #region Update
                            sqlText = "";
                            sqlText += " UPDATE UserAuditLogs";
                            sqlText += " SET LogOutDateTime = '" + LogOutDateTime + "'";
                            sqlText += " WHERE ComputerName='" + Item.ComputerName + "'";
                            sqlText += " and ComputerLoginUserName='" + Item.ComputerLoginUserName + "'";
                            sqlText += " and ComputerIPAddress='" + Item.ComputerIPAddress + "'";
                            sqlText += " and SoftwareUserId='" + Item.SoftwareUserId + "'";
                            sqlText += " and SessionDate='" + Item.SessionDate + "'";
                            sqlText += " and LogInDateTime='" + Item.LogInDateTime + "'";
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Transaction = transaction;
                            exeRes = cmdInsert.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);
                            #region Commit
                            if (transaction != null)
                            {
                                if (transResult > 0)
                                {
                                    transaction.Commit();
                                    retResults = "" + Item.LogID;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    retResults = "0";
                                }
                            }
                            else
                            {
                                retResults = "0";
                            }
                            #endregion Commit
                            #endregion Insert new user
                        }
                    }
                }
            }
            #region catch finally
            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResults;
        }
        
        public string InsertUserLogOut(string LogID, string LogOutDateTime)
        {
            #region Variables
            string retResults = string.Empty;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("InsertToUserInformationNew"); }
                #endregion open connection and transaction
                #region Update
                sqlText = "";
                sqlText += " UPDATE UserAuditLogs";
                sqlText += " SET LogOutDateTime = '" + LogOutDateTime + "'";
                sqlText += " WHERE LogID='" + LogID + "'";
                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;
                var exeRes = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults = "" + LogID;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults = "0";
                    }
                }
                else
                {
                    retResults = "0";
                }
                #endregion Commit
                #endregion Insert new user
            }
            #region catch
            catch (SqlException sqlex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResults;
        }
        
        public DataTable SearchUserLog(string ComputerLoginUserName, string SoftwareUserName, string ComputerName, string StartDate, string EndDate)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchtUserLog");
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
                sqlText = @"SELECT 
LogID,
SoftwareUserId ,
ui.UserName SoftwareUserName,
convert (DATETIME,SessionDate,101)SessionDate,
convert (DATETIME,LogInDateTime,101)LogInDateTime,
convert (DATETIME,isnull(LogOutDateTime,'1900/01/01'),101)LogOutDateTime,
ComputerName,
ComputerLoginUserName,
ComputerIPAddress
FROM UserAuditLogs ul 
LEFT OUTER JOIN UserInformations ui  ON ul.SoftwareUserId=ui.UserID 
                            WHERE 
                            (ComputerLoginUserName LIKE '%' + @ComputerLoginUserName	 + '%' OR @ComputerLoginUserName	 IS NULL) 
                            AND (ui.UserName LIKE '%' + @SoftwareUserName + '%' OR @SoftwareUserName IS NULL)
                            AND (ComputerName LIKE '%' + @ComputerName + '%' OR @ComputerName IS NULL)
                            AND (LogInDateTime>= @StartDate OR @StartDate IS NULL)
                            AND (LogInDateTime <dateadd(d,1, @EndDate) OR @EndDate IS NULL)
                            order by username";
                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;
                if (!objCommUser.Parameters.Contains("@ComputerLoginUserName"))
                { objCommUser.Parameters.AddWithValue("@ComputerLoginUserName", ComputerLoginUserName); }
                else { objCommUser.Parameters["@ComputerLoginUserName"].Value = ComputerLoginUserName; }
                if (!objCommUser.Parameters.Contains("@SoftwareUserName"))
                { objCommUser.Parameters.AddWithValue("@SoftwareUserName", SoftwareUserName); }
                else { objCommUser.Parameters["@SoftwareUserName"].Value = SoftwareUserName; }
                if (!objCommUser.Parameters.Contains("@ComputerName"))
                { objCommUser.Parameters.AddWithValue("@ComputerName", ComputerName); }
                else { objCommUser.Parameters["@ComputerName"].Value = ComputerName; }
                if (!objCommUser.Parameters.Contains("@StartDate"))
                { objCommUser.Parameters.AddWithValue("@StartDate", StartDate); }
                else { objCommUser.Parameters["@StartDate"].Value = StartDate; }
                if (!objCommUser.Parameters.Contains("@EndDate"))
                { objCommUser.Parameters.AddWithValue("@EndDate", EndDate); }
                else { objCommUser.Parameters["@EndDate"].Value = EndDate; }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);
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
            return dataTable;
        }
        
        public string[] UpdateToUserInformationNew(string UserID, string UserName, string UserPassword, string ActiveStatus, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
            #region Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            #endregion
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(UserID))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user id.");
                }
                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user name.");
                }
                //if (string.IsNullOrEmpty(UserPassword))
                //{
                //    throw new ArgumentNullException("UpdateToUserInformationNew", "Please enter user password.");
                //}
                #endregion Validation
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Update To User Information New"); }
                #endregion open connection and transaction
                #region UserID existence checking by id
                //select @Present = count(UserID) from UserInformations where  UserID=@UserID;
                sqlText = "select count(UserID) from UserInformations where  UserID = '" + UserID + "'";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                var exeRes = userIDExist.ExecuteScalar();
                countId = Convert.ToInt32(exeRes);
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Could not find requested user id.");
                }
                #endregion UserID existence checking by id
                #region UserName existence checking by id and requied field
                sqlText = "select count(UserName) from UserInformations ";
                sqlText += " where  UserID='" + UserID + "'";
                sqlText += " and UserName ='" + UserName + "'";
                SqlCommand userNameExist = new SqlCommand(sqlText, currConn);
                userNameExist.Transaction = transaction;
                exeRes = userNameExist.ExecuteScalar();
                countId = Convert.ToInt32(exeRes);
                if (countId > 0)
                {
                    throw new ArgumentNullException("UpdateToUserInformationNew", "Same user name already used.");
                }
                #endregion UserName existence checking by id and requied field
                #region Update user
                sqlText = "";
                sqlText = "update UserInformations set";
                sqlText += " UserName='" + UserName + "',";
                sqlText += " ActiveStatus='" + ActiveStatus + "',";
                sqlText += " LastModifiedBy='" + LastModifiedBy + "',";
                sqlText += " LastModifiedOn='" + LastModifiedOn + "'";
                sqlText += " where UserID='" + UserID + "'";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Information Successfully Update.";
                        retResults[2] = "" + UserID;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserID;
                    }
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserID;
                }
                #endregion Commit
                #endregion Update user
            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResults;
        }
        
        public string[] UpdateUserPasswordNew(string UserName, string UserPassword, string LastModifiedBy, string LastModifiedOn, string databaseName)
        {
            #region Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            #endregion
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(UserName))
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Please enter user name.");
                }
                if (string.IsNullOrEmpty(UserPassword))
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Please enter user password.");
                }
                #endregion Validation
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToUserInformationNew"); }
                #endregion open connection and transaction
                #region UserName existence checking by id
                //select @Present = count(UserID) from UserInformations where  UserID=@UserID;
                sqlText = "select count(UserName) from UserInformations where  UserName = '" + UserName + "'";
                SqlCommand userIDExist = new SqlCommand(sqlText, currConn);
                userIDExist.Transaction = transaction;
                var exeRes = userIDExist.ExecuteScalar();
                countId = Convert.ToInt32(exeRes);
                if (countId <= 0)
                {
                    throw new ArgumentNullException("UpdateUserPasswordNew", "Could not find requested user name.");
                }
                #endregion UserName existence checking by id
                #region Update user
                sqlText = "";
                sqlText = "update UserInformations set";
                sqlText += " UserPassword='" + UserPassword + "',";
                sqlText += " LastModifiedBy='" + LastModifiedBy + "',";
                sqlText += " LastModifiedOn='" + LastModifiedOn + "'";
                sqlText += " where UserName='" + UserName + "'";
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;
                exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Password Information Successfully Update.";
                        retResults[2] = "" + UserName;
                    }
                    else
                    {
                        transaction.Rollback();
                        retResults[0] = "Fail";
                        retResults[1] = "Unexpected error to update user.";
                        retResults[2] = "" + UserName;
                    }
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update user group";
                    retResults[2] = "" + UserName;
                }
                #endregion Commit
                #endregion Update user
            }
            #region catch
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return retResults;
        }
        //==================Search User=================
        /// <summary>
        /// Search User with separate SQL
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="ActiveStatus"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataTable SearchUserDataTable(string UserID, string UserName, string ActiveStatus, string databaseName)
        {
            #region Variables
            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("User Search");
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
                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword
                            FROM UserInformations
                            WHERE 
                            (UserID LIKE '%' + @UserID	 + '%' OR @UserID	 IS NULL) 
                            AND (UserName LIKE '%' + @UserName + '%' OR @UserName IS NULL)
                            AND (ActiveStatus LIKE '%' + @ActiveStatus + '%' OR @ActiveStatus IS NULL)
                            order by username";
                SqlCommand objCommUser = new SqlCommand();
                objCommUser.Connection = currConn;
                objCommUser.CommandText = sqlText;
                objCommUser.CommandType = CommandType.Text;
                if (!objCommUser.Parameters.Contains("@UserID"))
                { objCommUser.Parameters.AddWithValue("@UserID", UserID); }
                else { objCommUser.Parameters["@UserID"].Value = UserID; }
                if (!objCommUser.Parameters.Contains("@UserName"))
                { objCommUser.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUser.Parameters["@UserName"].Value = UserName; }
                if (!objCommUser.Parameters.Contains("@ActiveStatus"))
                { objCommUser.Parameters.AddWithValue("@ActiveStatus", ActiveStatus); }
                else { objCommUser.Parameters["@ActiveStatus"].Value = ActiveStatus; }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUser);
                dataAdapter.Fill(dataTable);
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
            return dataTable;
        }
        //==================Search User Has=================
        /// <summary>
        /// Search User Has with separate SQL
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public DataTable SearchUserHasNew(string UserName, string databaseName)
        {
            #region Variables
            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchUserHas");
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
                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword
                            FROM         UserInformations                
                            WHERE (UserName = @UserName )
                            order by username";
                SqlCommand objCommBankInformation = new SqlCommand();
                objCommBankInformation.Connection = currConn;
                objCommBankInformation.CommandText = sqlText;
                objCommBankInformation.CommandType = CommandType.Text;
                if (!objCommBankInformation.Parameters.Contains("@UserName"))
                { objCommBankInformation.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommBankInformation.Parameters["@UserName"].Value = UserName; }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommBankInformation);
                dataAdapter.Fill(dataTable);
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
            return dataTable;
        }
        #endregion
        
        public string[] InsertToUserRoll(List<UserRollVM> VMs, string databaseName)
        {
            #region Variables
            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string userId = "";
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("InsertToUserRoll"); }
                #endregion open connection and transaction
                #region id existence checking
                if (VMs.Any())
                {
                    foreach (var item in VMs)
                    {
                        if (!string.IsNullOrEmpty(item.UserID))
                        {
                            userId = item.UserID;
                        }
                        break;
                    }
                }
                sqlText = "select  count(FormID) from UserRolls where  UserID = '" + userId + "'";
                SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                cmdIdExist.Transaction = transaction;
                var exeRes = cmdIdExist.ExecuteScalar();
                countId = Convert.ToInt32(exeRes);
                if (countId > 0)
                {
                    sqlText = "delete from UserRolls where  UserID = '" + userId + "'";
                    SqlCommand cmdIdExist1 = new SqlCommand(sqlText, currConn);
                    cmdIdExist1.Transaction = transaction;
                    cmdIdExist1.ExecuteScalar();
                }
                #endregion
                if (VMs.Any())
                {
                    int j = 0;
                    foreach (var item in VMs)
                    {
                        Debug.WriteLine(j.ToString());
                        j++;
                        #region Update Settings
                        sqlText = "";
                        //sqlText += "declare @Present numeric";
                        //sqlText +=" select @Present = count(FormID) from UserRolls ";
                        //sqlText += " where  UserID = '" + item.UserID + "' and FormID='" + item.FormID + "'; ";
                        //                sqlText +=" if(@Present <=0 ) ";
                        //                sqlText +=" BEGIN ";
                        sqlText += " insert into UserRolls( ";
                        sqlText += " UserID, ";
                        sqlText += " FormID, ";
                        sqlText += " Access, ";
                        sqlText += " CreatedBy, ";
                        sqlText += " CreatedOn, ";
                        sqlText += " LastModifiedBy, ";
                        sqlText += " LastModifiedOn, ";
                        sqlText += " AddAccess,EditAccess, ";
                        sqlText += " LineID,FormName,PostAccess) ";
                        sqlText += " values( ";
                        sqlText += " '" + item.UserID + "', ";
                        sqlText += " '" + item.FormID + "', ";
                        sqlText += " '" + item.Access + "', ";
                        sqlText += " '" + item.CreatedBy + "', ";
                        sqlText += "'" + item.CreatedOn + "', ";
                        sqlText += " '" + item.LastModifiedBy + "', ";
                        sqlText += " '" + item.LastModifiedOn + "', ";
                        sqlText += " '" + item.AddAccess + "', '" + item.EditAccess + "',";
                        sqlText += " '" + item.LineID + "','" + item.FormName + "','" + item.PostAccess + "'); ";
                        //sqlText +=" END ";
                        //sqlText +=" else ";
                        //sqlText +=" BEGIN ";
                        //sqlText +=" update UserRolls ";
                        //sqlText +=" set  ";
                        //sqlText += " Access='" + item.Access + "', ";
                        //sqlText += " LineID='" + item.LineID + "', ";
                        //sqlText += " FormName='" + item.FormName + "', ";
                        //sqlText += " LastModifiedBy='" + item.LastModifiedBy + "', ";
                        //sqlText += " LastModifiedOn='" + item.LastModifiedOn + "', ";
                        //sqlText += " PostAccess='" + item.PostAccess + "' ";
                        //sqlText += " where userid='" + item.UserID + "' and FormID='" + item.FormID + "'; ";
                        //sqlText +=" END";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        exeRes = cmdInsDetail.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("User", "Not Save");
                        }
                        #endregion Update Settings
                    }
                }
                else
                {
                    throw new ArgumentNullException("InsertToUserRoll", "Could not found any item.");
                }
                #region Commit
                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "Requested User Roll Information Successfully Updated.";
                    }
                }
                #endregion Commit
            }
            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            #endregion Catch and Finall
            #region Result
            return retResults;
            #endregion Result
        }
        
        public string SearchUserRoll(string UserID)
        {
            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            string decryptedData = string.Empty;
            string encriptedData = string.Empty;
            try
            {
                #region open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection
                #region MyRegion
                CommonDAL commonDal = new CommonDAL();
                int insertCol = 0;
                SqlTransaction transaction = currConn.BeginTransaction("");
                insertCol = commonDal.TableFieldAdd("UserRolls", "AddAccess", "varchar(1)", currConn, transaction);
                insertCol = commonDal.TableFieldAdd("UserRolls", "EditAccess", "varchar(1)", currConn, transaction);
                if (insertCol < 0)
                {
                    transaction.Commit();
                }
                #endregion
                sqlText = @"
                SELECT LineID,UserID,FormID,isnull(Access,'N')Access,isnull(PostAccess,'N')PostAccess,isnull(AddAccess,'N')AddAccess,isnull(EditAccess,'N')EditAccess 
                FROM dbo.UserRolls
                WHERE (UserID  = @UserID ) 
                order by UserID,LineID
                ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.CommandType = CommandType.Text;
                if (!objComm.Parameters.Contains("@UserID"))
                { objComm.Parameters.AddWithValue("@UserID", UserID); }
                else { objComm.Parameters["@UserID"].Value = UserID; }
                SqlDataReader reader = objComm.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        decryptedData = decryptedData + FieldDelimeter + reader[j].ToString();
                    }
                    decryptedData = decryptedData + LineDelimeter;
                }
                reader.Close();
                encriptedData = Converter.DESEncrypt(PassPhrase, EnKey, decryptedData);
                return encriptedData;
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
            return encriptedData;
        }
        
        public DataTable SearchUserHas(string UserName)
        {
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable("UserHas");
            try
            {
                #region open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection
                sqlText = @"SELECT UserID, UserName, ActiveStatus,UserPassword,GETDATE() ServerDate
                            FROM  UserInformations                
                            WHERE (UserName = @UserName )
                            order by username";
                SqlCommand objCommUI = new SqlCommand();
                objCommUI.Connection = currConn;
                objCommUI.CommandText = sqlText;
                objCommUI.CommandType = CommandType.Text;
                if (!objCommUI.Parameters.Contains("@UserName"))
                { objCommUI.Parameters.AddWithValue("@UserName", UserName); }
                else { objCommUI.Parameters["@UserName"].Value = UserName; }
                //SqlDataReader reader = objCommBankInformation.ExecuteReader();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommUI);
                dataAdapter.Fill(dt);
            }
            #region catch and final
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion catch and final
            return dt;
        }
        
        public UserLogsVM SelectUserByEmployee(string EmployeeId = null, string UserId = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            UserLogsVM vm = new UserLogsVM();
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
                sqlText = @"
SELECT top 1
 Id
,FullName
,isnull(GroupId,'0')GroupId
,Email
,LogId
,Password
,VerificationCode
,BranchId
,EmployeeId
,IsAdmin
,IsActive
,isnull(IsVerified,0)IsVerified
,IsArchived
From [User]
Where 1=1  
";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                    sqlText += " and  EmployeeId=@EmployeeId";
                if (!string.IsNullOrWhiteSpace(UserId))
                    sqlText += " and  Id=@UserId";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                if (!string.IsNullOrWhiteSpace(UserId))
                    objComm.Parameters.AddWithValue("@UserId", UserId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.FullName = dr["FullName"].ToString();
                    vm.GroupId = Convert.ToInt32(dr["GroupId"].ToString());
                    vm.Email = dr["Email"].ToString();
                    vm.LogID = dr["LogId"].ToString();
                    vm.Password = dr["Password"].ToString();
                    vm.VerificationCode = dr["VerificationCode"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsVerified = Convert.ToBoolean(dr["IsVerified"]);
                    vm.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
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
            return vm;
        }
        //==================Insert =================
        public string[] Insert(UserLogsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            //int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertUser"; //Method Name
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "[User]";
                string[] fieldName = { "LogId" };
                string[] fieldValue = { vm.LogID.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsertUser(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist
                #region Save
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from [User] where LogId=@LogId and BranchId=@BranchId";


                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
                cmd1.Parameters.AddWithValue("@LogId", vm.LogID);
                cmd1.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd1.Transaction = transaction;
                var exeRes = cmd1.ExecuteScalar();
                int user = Convert.ToInt32(exeRes);
                //sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from [User] where BranchId=@BranchId";
                sqlText = "SELECT ISNULL(MAX(CONVERT(INT, REPLACE(SUBSTRING(CONVERT(VARCHAR(10), id), CHARINDEX('_', CONVERT(VARCHAR(10), id)) + 1, 10), CHAR(13) + CHAR(10), ''))), 0) from [User] where BranchId=@BranchId";

                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd2.Transaction = transaction;
                exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                if (user == 0)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO [User] (
Id
,FullName
,GroupId
,Email
,LogId
,Password
,VerificationCode
,BranchId
,EmployeeId
,IsAdmin
,IsActive
,IsVerified
,IsArchived
,CreatedBy
,CreatedAt
,CreatedFrom
) 
                                VALUES (
 @Id
,@FullName
,@GroupId
,@Email
,@LogId
,@Password
,@VerificationCode
,@BranchId
,@EmployeeId
,@IsAdmin
,@IsActive
,@IsVerified
,@IsArchived
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) 
                                       ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@GroupId", vm.GroupId);
                    cmdInsert.Parameters.AddWithValue("@FullName", vm.FullName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@LogId", vm.LogID);
                    cmdInsert.Parameters.AddWithValue("@Password", vm.Password);
                    cmdInsert.Parameters.AddWithValue("@VerificationCode", vm.VerificationCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@IsAdmin", true);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsVerified", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchived", vm.IsArchived);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", Ordinary.DateToString(vm.CreatedAt));
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This User already used!";
                    throw new ArgumentNullException("Please Input User Value!", "");
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
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully";
                retResults[2] = vm.Id;
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                transaction.Rollback();
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
        //==================Update =================
        public string[] Update(UserLogsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "User Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UserUpdate"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update [User] set";
                    sqlText += "  FullName=@FullName,";
                    sqlText += "  Email=@Email,";
                    sqlText += "  LogId=@LogId,";
                    sqlText += "  VerificationCode=@VerificationCode,";
                    sqlText += "  BranchId=@BranchId,";
                    sqlText += "  EmployeeId=@EmployeeId,";
                    sqlText += "  IsAdmin=@IsAdmin,";
                    sqlText += "  GroupId=@GroupId,";
                    sqlText += "  IsActive=@IsActive,";
                    sqlText += "  IsVerified=@IsVerified,";
                    sqlText += "  IsArchived=@IsArchived,";
                    sqlText += "   LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@GroupId", vm.GroupId);
                    cmdUpdate.Parameters.AddWithValue("@FullName", vm.FullName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LogId", vm.LogID);
                    cmdUpdate.Parameters.AddWithValue("@VerificationCode", vm.VerificationCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@IsAdmin", vm.IsAdmin);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsVerified", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchived", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                            retResults[0] = "Success";
                            retResults[1] = "Requested User Information Successfully Updated.";
                        }
                    }
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Unexpected error to update User.";
                retResults[4] = ex.Message; //catch ex
                transaction.Rollback();
                return retResults;
            }
            #endregion
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
        //==================Update =================
        public string[] Update(UserLogsVM vm, string[] roles, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "User Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UserUpdate"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update [User] set";
                    sqlText += "  FullName=@FullName,";
                    sqlText += "  Email=@Email,";
                    sqlText += "  LogId=@LogId,";
                    sqlText += "  VerificationCode=@VerificationCode,";
                    sqlText += "  BranchId=@BranchId,";
                    sqlText += "  EmployeeId=@EmployeeId,";
                    sqlText += "  IsAdmin=@IsAdmin,";
                    sqlText += "  GroupId=@GroupId,";
                    sqlText += "  IsActive=@IsActive,";
                    sqlText += "  IsVerified=@IsVerified,";
                    sqlText += "  IsArchived=@IsArchived,";
                    sqlText += "   LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@GroupId", vm.GroupId);
                    cmdUpdate.Parameters.AddWithValue("@FullName", vm.FullName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Email", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LogId", vm.LogID);
                    cmdUpdate.Parameters.AddWithValue("@VerificationCode", vm.VerificationCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@IsAdmin", vm.IsAdmin);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsVerified", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchived", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    // Previousrole
                    UserRolesVM urolesVM;
                    List<UserRolesVM> OldRole = new List<UserRolesVM>();
                    List<UserRolesVM> NewRole = new List<UserRolesVM>();
                    if (roles != null && roles.Length > 0)
                    {
                        for (int i = 0; i < roles.Length; i++)
                        {
                            urolesVM = new UserRolesVM();
                            urolesVM.RoleInfoId = roles[i];
                            NewRole.Add(urolesVM);
                        }
                    }
                    else
                    {
                        retResults[1] = "No Role is assigned to this User!";
                        throw new ArgumentNullException("No Role is assigned to this User!", "");
                    }
                    List<UserRolesVM> DeleteRole = new List<UserRolesVM>();
                    List<UserRolesVM> AddRole = new List<UserRolesVM>();
                    sqlText = @"SELECT
Id
,BranchId
,UserInfoId
,RoleInfoId
    From UserRoles
Where IsArchived=0 and BranchId=@BranchId and UserInfoId=@UserInfoId
";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = currConn;
                    cmd.CommandText = sqlText;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@UserInfoId", vm.Id);
                    cmd.Transaction = transaction;
                    SqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        urolesVM = new UserRolesVM();
                        urolesVM.Id = Convert.ToInt32(dr["Id"]);
                        urolesVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        urolesVM.UserInfoId = dr["UserInfoId"].ToString();
                        urolesVM.RoleInfoId = dr["RoleInfoId"].ToString();
                        OldRole.Add(urolesVM);
                    }
                    dr.Close();
                    // New roll
                    foreach (string item in roles)
                    {
                        if (!OldRole.Any(r => r.RoleInfoId == item))
                        {
                            urolesVM = new UserRolesVM();
                            urolesVM.RoleInfoId = item;
                            AddRole.Add(urolesVM);
                        }
                    }
                    foreach (UserRolesVM item2 in OldRole)
                    {
                        if (!NewRole.Any(r => r.RoleInfoId == item2.RoleInfoId))
                        {
                            urolesVM = new UserRolesVM();
                            urolesVM.RoleInfoId = item2.RoleInfoId;
                            DeleteRole.Add(urolesVM);
                        }
                    }
                    foreach (UserRolesVM item in AddRole)
                    {
                        sqlText = @"Insert into UserRoles (BranchId,UserInfoId,RoleInfoId,IsArchived) values (@BranchId,@UserInfoId,@RoleInfoId,@IsArchived)";
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = currConn;
                        cmd2.CommandText = sqlText;
                        cmd2.CommandType = CommandType.Text;
                        cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd2.Parameters.AddWithValue("@UserInfoId", vm.Id);
                        cmd2.Parameters.AddWithValue("@RoleInfoId", item.RoleInfoId);
                        cmd2.Parameters.AddWithValue("@IsArchived", false);
                        cmd2.Transaction = transaction;
                        cmd2.ExecuteNonQuery();
                    }
                    foreach (UserRolesVM item in DeleteRole)
                    {
                        sqlText = @"Delete from UserRoles where BranchId=@BranchId and UserInfoId=@UserInfoId and RoleInfoId=@RoleInfoId";
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = currConn;
                        cmd2.CommandText = sqlText;
                        cmd2.CommandType = CommandType.Text;
                        cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd2.Parameters.AddWithValue("@UserInfoId", vm.Id);
                        cmd2.Parameters.AddWithValue("@RoleInfoId", item.RoleInfoId);
                        cmd2.Transaction = transaction;
                        cmd2.ExecuteNonQuery();
                    }
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", ProjectVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("User Update", "Could not found any user.");
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
                    retResults[1] = "Requested User Information Successfully Updated.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update User.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = "Unexpected error to update User.";
                retResults[4] = ex.Message; //catch ex
                transaction.Rollback();
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
        #region Backup
        //        //==================Insert =================
        //        public string[] InsertBackUp(UserLogsVM userLogVM, string[] roles, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //        {
        //            #region Initializ
        //            string sqlText = "";
        //            //int Id = 0;
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = "0";// Return Id
        //            retResults[3] = sqlText; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "InsertUser"; //Method Name
        //            #endregion
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            #region Try
        //            try
        //            {
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null)
        //                {
        //                    transaction = currConn.BeginTransaction("");
        //                }
        //                #endregion open connection and transaction
        //                #region Save
        //                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from [User] where LogId=@LogId and BranchId=@BranchId";
        //                SqlCommand cmd1 = new SqlCommand(sqlText, currConn);
        //                cmd1.Parameters.AddWithValue("@LogId", userLogVM.LogID);
        //                cmd1.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                cmd1.Transaction = transaction;
        //                var exeRes = cmd1.ExecuteScalar();
        //                int user = Convert.ToInt32(exeRes);
        //                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from [User] where BranchId=@BranchId";
        //                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
        //                cmd2.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                cmd2.Transaction = transaction;
        //                exeRes = cmd2.ExecuteScalar();
        //                int count = Convert.ToInt32(exeRes);
        //                userLogVM.Id = userLogVM.BranchId.ToString() + "_" + (count + 1);
        //                if (user == 0)
        //                {
        //                    sqlText = "  ";
        //                    sqlText += @" INSERT INTO [User] (
        //Id
        //,FullName
        //,Email
        //,LogId
        //,Password
        //,VerificationCode
        //,BranchId
        //,EmployeeId
        //,IsAdmin
        //,IsActive
        //,IsVerified
        //,IsArchived
        //,CreatedBy
        //,CreatedAt
        //,CreatedFrom
        //) 
        //                                VALUES (
        // @Id
        //,@FullName
        //,@Email
        //,@LogId
        //,@Password
        //,@VerificationCode
        //,@BranchId
        //,@EmployeeId
        //,@IsAdmin
        //,@IsActive
        //,@IsVerified
        //,@IsArchived
        //,@CreatedBy
        //,@CreatedAt
        //,@CreatedFrom
        //) 
        //                                       ";
        //                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
        //                    cmdInsert.Parameters.AddWithValue("@Id", userLogVM.Id);
        //                    cmdInsert.Parameters.AddWithValue("@FullName", userLogVM.FullName ?? Convert.DBNull);
        //                    cmdInsert.Parameters.AddWithValue("@Email", userLogVM.Email ?? Convert.DBNull);
        //                    cmdInsert.Parameters.AddWithValue("@LogId", userLogVM.LogID);
        //                    cmdInsert.Parameters.AddWithValue("@Password", userLogVM.Password);
        //                    cmdInsert.Parameters.AddWithValue("@VerificationCode", userLogVM.VerificationCode ?? Convert.DBNull);
        //                    cmdInsert.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                    cmdInsert.Parameters.AddWithValue("@EmployeeId", userLogVM.EmployeeId);
        //                    cmdInsert.Parameters.AddWithValue("@IsAdmin", userLogVM.IsAdmin);
        //                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
        //                    cmdInsert.Parameters.AddWithValue("@IsVerified", true);
        //                    cmdInsert.Parameters.AddWithValue("@IsArchived", userLogVM.IsArchived);
        //                    cmdInsert.Parameters.AddWithValue("@CreatedBy", userLogVM.CreatedBy);
        //                    cmdInsert.Parameters.AddWithValue("@CreatedAt", userLogVM.CreatedAt);
        //                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", userLogVM.CreatedFrom);
        //                    cmdInsert.Transaction = transaction;
        //                    cmdInsert.ExecuteNonQuery();
        //                    //if (Id <= 0)
        //                    //{
        //                    //    retResults[1] = "Please Input User Value";
        //                    //    retResults[3] = sqlText;
        //                    //    throw new ArgumentNullException("Please Input User Value", "");
        //                    //}
        //                    if (roles != null && roles.Length > 0)
        //                    {
        //                        for (int i = 0; i < roles.Length; i++)
        //                        {
        //                            sqlText = @"Insert into UserRoles (BranchId,UserInfoId,RoleInfoId,IsArchived) values (@BranchId,@UserInfoId,@RoleInfoId,@IsArchived)";
        //                            SqlCommand cmd3 = new SqlCommand();
        //                            cmd3.Connection = currConn;
        //                            cmd3.CommandText = sqlText;
        //                            cmd3.CommandType = CommandType.Text;
        //                            cmd3.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                            cmd3.Parameters.AddWithValue("@UserInfoId", userLogVM.Id);
        //                            cmd3.Parameters.AddWithValue("@RoleInfoId", roles[i]);
        //                            cmd3.Parameters.AddWithValue("@IsArchived", false);
        //                            cmd3.Transaction = transaction;
        //                            cmd3.ExecuteNonQuery();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        retResults[1] = "No Role is assigned to this User!";
        //                        throw new ArgumentNullException("No Role is assigned to this User!", "");
        //                    }
        //                }
        //                else
        //                {
        //                    retResults[1] = "This User already used!";
        //                    throw new ArgumentNullException("Please Input User Value!", "");
        //                }
        //                #endregion Save
        //                #region Commit
        //                if (Vtransaction == null)
        //                {
        //                    if (transaction != null)
        //                    {
        //                        transaction.Commit();
        //                    }
        //                }
        //                #endregion Commit
        //                #region SuccessResult
        //                retResults[0] = "Success";
        //                retResults[1] = "Data Save Successfully";
        //                retResults[2] = userLogVM.Id;
        //                #endregion SuccessResult
        //            }
        //            #endregion try
        //            #region Catch and Finall
        //            catch (Exception ex)
        //            {
        //                retResults[1] = "Unexpected error to Create User.";
        //                retResults[4] = ex.Message.ToString(); //catch ex
        //                transaction.Rollback();
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null)
        //                {
        //                    if (currConn != null)
        //                    {
        //                        if (currConn.State == ConnectionState.Open)
        //                        {
        //                            currConn.Close();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region Results
        //            return retResults;
        //            #endregion
        //        }
        //        //==================Update =================
        //        public string[] UpdateBackUp(UserLogsVM userLogVM, string[] roles, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        //        {
        //            #region Variables
        //            string[] retResults = new string[6];
        //            retResults[0] = "Fail";//Success or Fail
        //            retResults[1] = "Fail";// Success or Fail Message
        //            retResults[2] = "0";
        //            retResults[3] = "sqlText"; //  SQL Query
        //            retResults[4] = "ex"; //catch ex
        //            retResults[5] = "User Update"; //Method Name
        //            int transResult = 0;
        //            string sqlText = "";
        //            bool iSTransSuccess = false;
        //            #endregion
        //            SqlConnection currConn = null;
        //            SqlTransaction transaction = null;
        //            try
        //            {
        //                #region open connection and transaction
        //                #region New open connection and transaction
        //                if (VcurrConn != null)
        //                {
        //                    currConn = VcurrConn;
        //                }
        //                if (Vtransaction != null)
        //                {
        //                    transaction = Vtransaction;
        //                }
        //                #endregion New open connection and transaction
        //                if (currConn == null)
        //                {
        //                    currConn = _dbsqlConnection.GetConnection();
        //                    if (currConn.State != ConnectionState.Open)
        //                    {
        //                        currConn.Open();
        //                    }
        //                }
        //                if (transaction == null) { transaction = currConn.BeginTransaction("UserUpdate"); }
        //                #endregion open connection and transaction
        //                if (userLogVM != null)
        //                {
        //                    #region Update Settings
        //                    sqlText = "";
        //                    sqlText = "update [User] set";
        //                    sqlText += "  FullName=@FullName,";
        //                    sqlText += "  Email=@Email,";
        //                    sqlText += "  LogId=@LogId,";
        //                    if (userLogVM.Password != null)
        //                    {
        //                        sqlText += "  Password=@Password,";
        //                    }
        //                    sqlText += "  VerificationCode=@VerificationCode,";
        //                    sqlText += "  BranchId=@BranchId,";
        //                    sqlText += "  EmployeeId=@EmployeeId,";
        //                    sqlText += "  IsAdmin=@IsAdmin,";
        //                    sqlText += "  IsActive=@IsActive,";
        //                    sqlText += "  IsVerified=@IsVerified,";
        //                    sqlText += "  IsArchived=@IsArchived,";
        //                    sqlText += "   LastUpdateBy=@LastUpdateBy,";
        //                    sqlText += " LastUpdateAt=@LastUpdateAt,";
        //                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
        //                    sqlText += " where Id=@Id";
        //                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
        //                    cmdUpdate.Parameters.AddWithValue("@Id", userLogVM.Id);
        //                    cmdUpdate.Parameters.AddWithValue("@FullName", userLogVM.FullName ?? Convert.DBNull);
        //                    cmdUpdate.Parameters.AddWithValue("@Email", userLogVM.Email ?? Convert.DBNull);
        //                    cmdUpdate.Parameters.AddWithValue("@LogId", userLogVM.LogID);
        //                    if (userLogVM.Password != null)
        //                    {
        //                        cmdUpdate.Parameters.AddWithValue("@Password", userLogVM.Password);
        //                    }
        //                    cmdUpdate.Parameters.AddWithValue("@VerificationCode", userLogVM.VerificationCode ?? Convert.DBNull);
        //                    cmdUpdate.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", userLogVM.EmployeeId);
        //                    cmdUpdate.Parameters.AddWithValue("@IsAdmin", userLogVM.IsAdmin);
        //                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
        //                    cmdUpdate.Parameters.AddWithValue("@IsVerified", true);
        //                    cmdUpdate.Parameters.AddWithValue("@IsArchived", false);
        //                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", userLogVM.LastUpdateBy);
        //                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", userLogVM.LastUpdateAt);
        //                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", userLogVM.LastUpdateFrom);
        //                    cmdUpdate.Transaction = transaction;
        //                    var exeRes = cmdUpdate.ExecuteNonQuery();
        //                    transResult = Convert.ToInt32(exeRes);
        //                    retResults[2] = userLogVM.Id.ToString();// Return Id
        //                    retResults[3] = sqlText; //  SQL Query
        //                    // Previousrole
        //                    UserRolesVM urolesVM;
        //                    List<UserRolesVM> OldRole = new List<UserRolesVM>();
        //                    List<UserRolesVM> NewRole = new List<UserRolesVM>();
        //                    if (roles != null && roles.Length > 0)
        //                    {
        //                        for (int i = 0; i < roles.Length; i++)
        //                        {
        //                            urolesVM = new UserRolesVM();
        //                            urolesVM.RoleInfoId = roles[i];
        //                            NewRole.Add(urolesVM);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        retResults[1] = "No Role is assigned to this User!";
        //                        throw new ArgumentNullException("No Role is assigned to this User!", "");
        //                    }
        //                    List<UserRolesVM> DeleteRole = new List<UserRolesVM>();
        //                    List<UserRolesVM> AddRole = new List<UserRolesVM>();
        //                    sqlText = @"SELECT
        //Id
        //,BranchId
        //,UserInfoId
        //,RoleInfoId
        //
        //    From UserRoles
        //Where IsArchived=0 and BranchId=@BranchId and UserInfoId=@UserInfoId
        //";
        //                    SqlCommand cmd = new SqlCommand();
        //                    cmd.Connection = currConn;
        //                    cmd.CommandText = sqlText;
        //                    cmd.CommandType = CommandType.Text;
        //                    cmd.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                    cmd.Parameters.AddWithValue("@UserInfoId", userLogVM.Id);
        //                    cmd.Transaction = transaction;
        //                    SqlDataReader dr;
        //                    dr = cmd.ExecuteReader();
        //                    while (dr.Read())
        //                    {
        //                        urolesVM = new UserRolesVM();
        //                        urolesVM.Id = Convert.ToInt32(dr["Id"]);
        //                        urolesVM.BranchId = Convert.ToInt32(dr["BranchId"]);
        //                        urolesVM.UserInfoId = dr["UserInfoId"].ToString();
        //                        urolesVM.RoleInfoId = dr["RoleInfoId"].ToString();
        //                        OldRole.Add(urolesVM);
        //                    }
        //                    dr.Close();
        //                    // New roll
        //                    foreach (string item in roles)
        //                    {
        //                        if (!OldRole.Any(r => r.RoleInfoId == item))
        //                        {
        //                            urolesVM = new UserRolesVM();
        //                            urolesVM.RoleInfoId = item;
        //                            AddRole.Add(urolesVM);
        //                        }
        //                    }
        //                    foreach (UserRolesVM item2 in OldRole)
        //                    {
        //                        if (!NewRole.Any(r => r.RoleInfoId == item2.RoleInfoId))
        //                        {
        //                            urolesVM = new UserRolesVM();
        //                            urolesVM.RoleInfoId = item2.RoleInfoId;
        //                            DeleteRole.Add(urolesVM);
        //                        }
        //                    }
        //                    foreach (UserRolesVM item in AddRole)
        //                    {
        //                        sqlText = @"Insert into UserRoles (BranchId,UserInfoId,RoleInfoId,IsArchived) values (@BranchId,@UserInfoId,@RoleInfoId,@IsArchived)";
        //                        SqlCommand cmd2 = new SqlCommand();
        //                        cmd2.Connection = currConn;
        //                        cmd2.CommandText = sqlText;
        //                        cmd2.CommandType = CommandType.Text;
        //                        cmd2.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                        cmd2.Parameters.AddWithValue("@UserInfoId", userLogVM.Id);
        //                        cmd2.Parameters.AddWithValue("@RoleInfoId", item.RoleInfoId);
        //                        cmd2.Parameters.AddWithValue("@IsArchived", false);
        //                        cmd2.Transaction = transaction;
        //                        cmd2.ExecuteNonQuery();
        //                    }
        //                    foreach (UserRolesVM item in DeleteRole)
        //                    {
        //                        sqlText = @"Delete from UserRoles where BranchId=@BranchId and UserInfoId=@UserInfoId and RoleInfoId=@RoleInfoId";
        //                        SqlCommand cmd2 = new SqlCommand();
        //                        cmd2.Connection = currConn;
        //                        cmd2.CommandText = sqlText;
        //                        cmd2.CommandType = CommandType.Text;
        //                        cmd2.Parameters.AddWithValue("@BranchId", userLogVM.BranchId);
        //                        cmd2.Parameters.AddWithValue("@UserInfoId", userLogVM.Id);
        //                        cmd2.Parameters.AddWithValue("@RoleInfoId", item.RoleInfoId);
        //                        cmd2.Transaction = transaction;
        //                        cmd2.ExecuteNonQuery();
        //                    }
        //                    #region Commit
        //                    if (transResult <= 0)
        //                    {
        //                        // throw new ArgumentNullException("Education Update", ProjectVM.BranchId + " could not updated.");
        //                    }
        //                    #endregion Commit
        //                    #endregion Update Settings
        //                    iSTransSuccess = true;
        //                }
        //                else
        //                {
        //                    throw new ArgumentNullException("User Update", "Could not found any user.");
        //                }
        //                if (iSTransSuccess == true)
        //                {
        //                    if (Vtransaction == null)
        //                    {
        //                        if (transaction != null)
        //                        {
        //                            transaction.Commit();
        //                        }
        //                    }
        //                    retResults[0] = "Success";
        //                    retResults[1] = "Requested User Information Successfully Updated.";
        //                }
        //                else
        //                {
        //                    retResults[1] = "Unexpected error to update User.";
        //                    throw new ArgumentNullException("", "");
        //                }
        //            }
        //            #region catch
        //            catch (Exception ex)
        //            {
        //                retResults[1] = "Unexpected error to update User.";
        //                retResults[4] = ex.Message; //catch ex
        //                transaction.Rollback();
        //                return retResults;
        //            }
        //            finally
        //            {
        //                if (VcurrConn == null)
        //                {
        //                    if (currConn != null)
        //                    {
        //                        if (currConn.State == ConnectionState.Open)
        //                        {
        //                            currConn.Close();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            return retResults;
        //        }
        #endregion Backup
        public string[] ChangePassword(UserLogsVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "User Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            bool iSTransSuccess = false;
            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UserUpdate"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update [User] set";
                    sqlText += " Password=@Password,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE 1=1 AND EmployeeId=@EmployeeId";
                    if (!vm.IsAdmin)
                    {
                        sqlText += " And Password=@OldPassword";
                    }
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    vm.Password = Ordinary.Encrypt(vm.Password, true);
                    cmdUpdate.Parameters.AddWithValue("@Password", vm.Password);

                    if (!vm.IsAdmin)
                    {
                        vm.OldPassword = Ordinary.Encrypt(vm.OldPassword, true);
                        cmdUpdate.Parameters.AddWithValue("@OldPassword", vm.OldPassword);
                    }

                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult == 0)
                    {
                        retResults[1] = "This employee is not a user Or Invalide old password!";
                        throw new ArgumentNullException("This employee is not a user Or Invalide old password!", "No user found");
                    }
                    retResults[2] = vm.EmployeeId.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("User Update", "No user found");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("User Update", "Could not found any user.");
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
                    retResults[1] = "Requested User Information Successfully Updated.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update User.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                transaction.Rollback();
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
        
        public List<EmployeeInfoVM> SelectAllActiveEmp()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                sqlText = @"
	SELECT
	e.EmployeeId
	,e.EmpName, e.Code, e.Designation, e.Department
	,e.JoinDate
	From ViewEmployeeInformation e
	Where e.IsArchive=0 AND e.IsActive = 1
	and EmployeeId not in(
	select EmployeeId from [User])
	ORDER BY e.Code
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                //objComm.Parameters.AddWithValue("@IsArchive", true);
                //objComm.Parameters.AddWithValue("@IsActive", true);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    VMs.Add(vm);
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
            return VMs;
        }
        
        public List<EmployeeInfoVM> SelectAllUser(string Id = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                sqlText = @"
                    SELECT
                    usr.Id
                    ,isnull(e.EmpName,'NA')EmpName, isnull(e.Code,'NA')Code
                    , isnull(e.Designation,'NA')Designation, isnull(e.Department,'NA')Department
                    ,e.JoinDate 
					,e.Email
					,usr.IsAdmin
					from [User] usr					
                    left outer join ViewEmployeeInformation e on usr.EmployeeId=e.EmployeeId
                    where 1=1
                    ";
                if (!string.IsNullOrWhiteSpace(Id))
                    sqlText += " and user.Id = @Id";
                sqlText += " ORDER BY e.Code";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                //objComm.Parameters.AddWithValue("@IsArchive", true);
                //objComm.Parameters.AddWithValue("@IsActive", true);
                if (!string.IsNullOrWhiteSpace(Id))
                    objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Email = dr["Email"].ToString();
                    VMs.Add(vm);
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
            return VMs;
        }

        public List<EmployeeInfoVM> SelectAllAdminUser(string Id = "", string Code = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                sqlText = @"
                    SELECT
                    usr.Id
                    ,isnull(e.EmpName,'NA')EmpName, isnull(e.Code,'NA')Code
                    , isnull(e.Designation,'NA')Designation, isnull(e.Department,'NA')Department
                    ,e.JoinDate 
					,e.Email
					,usr.IsAdmin
					from [User] usr					
                    left outer join ViewEmployeeInformation e on usr.EmployeeId=e.EmployeeId
                    where 1=1
					AND usr.IsAdmin = 1
                    ";
                if (!string.IsNullOrWhiteSpace(Id))
                    sqlText += " and user.Id = @Id";
                if (!string.IsNullOrWhiteSpace(Code))
                    sqlText += " and e.Code = @Code";
                sqlText += " ORDER BY e.Code";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                //objComm.Parameters.AddWithValue("@IsArchive", true);
                //objComm.Parameters.AddWithValue("@IsActive", true);
                if (!string.IsNullOrWhiteSpace(Id))
                    objComm.Parameters.AddWithValue("@Id", Id);
                if (!string.IsNullOrWhiteSpace(Code))
                    objComm.Parameters.AddWithValue("@Code", Code);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Email = dr["Email"].ToString();
                    vm.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);

                    VMs.Add(vm);
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
            return VMs;
        }

        public List<EmployeeInfoVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                sqlText = @" SELECT
Id,
(Salutation_E+' '+MiddleName+' '+LastName) As FullName
   FROM EmployeeInfo
WHERE IsActive=1
";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.FullName = dr["FullName"].ToString();
                    VMs.Add(vm);
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
            return VMs;
        }
        public List<EmployeeInfoVM> GetUser(string deptId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                sqlText = @" SELECT 
 EmployeeId AS Id
,EmpName AS FullName
 FROM ViewEmployeeInformation
 WHERE IsActive=1 ";

                if (!string.IsNullOrEmpty(deptId) && deptId != "0")
                {
                    sqlText += @" AND DepartmentId = '" + deptId + "'";
                }

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.FullName = dr["FullName"].ToString();
                    VMs.Add(vm);
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
            return VMs;
        }

    }
}
