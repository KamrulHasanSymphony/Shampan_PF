using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace SymServices.Common
{
    public class UserGroupDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        public List<UserGroupVM> SelectAll()
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<UserGroupVM> VMs = new List<UserGroupVM>();
            UserGroupVM VM;
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
Id
,GroupName
,ISNULL(IsSuper,0)IsSuper
,ISNULL(IsAdmin,0)IsAdmin
,ISNULL(IsHRM,0)IsHRM
,ISNULL(IsAttendance,0)IsAttendance
,ISNULL(IsPayroll,0)IsPayroll
,ISNULL(IsTAX,0)IsTAX
,ISNULL(IsPF,0)IsPF
,ISNULL(IsGF,0)IsGF
,ISNULL(IsESS,0)IsESS
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From UserGroup
Where IsArchive=0
    ORDER BY GroupName
";


                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new UserGroupVM();
                    VM.Id = Convert.ToInt32(dr["Id"].ToString());
                    VM.GroupName = dr["GroupName"].ToString();
                    VM.IsSuper = Convert.ToBoolean(dr["IsSuper"]);
                    VM.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    VM.IsHRM = Convert.ToBoolean(dr["IsHRM"]);
                    VM.IsAttendance = Convert.ToBoolean(dr["IsAttendance"]);
                    VM.IsPayroll = Convert.ToBoolean(dr["IsPayroll"]);
                    VM.IsTAX = Convert.ToBoolean(dr["IsTAX"]);
                    VM.IsPF = Convert.ToBoolean(dr["IsPF"]);
                    VM.IsGF = Convert.ToBoolean(dr["IsGF"]);
                    VM.IsESS = Convert.ToBoolean(dr["IsESS"]);
                    VM.Remarks = dr["Remarks"].ToString();
                    VM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    VM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    VM.CreatedBy = dr["CreatedBy"].ToString();
                    VM.CreatedFrom = dr["CreatedFrom"].ToString();
                    VM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    VM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    VM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(VM);
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
        //==================SelectByID=================
        public UserGroupVM SelectById(string GroupId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            UserGroupVM VM = new UserGroupVM();

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
Id
,GroupName
,ISNULL(IsSuper,0)IsSuper
,ISNULL(IsAdmin,0)IsAdmin
,ISNULL(IsHRM,0)IsHRM
,ISNULL(IsAttendance,0)IsAttendance
,ISNULL(IsPayroll,0)IsPayroll
,ISNULL(IsTAX,0)IsTAX
,ISNULL(IsPF,0)IsPF
,ISNULL(IsGF,0)IsGF
,ISNULL(IsESS,0)IsESS
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From UserGroup
Where  Id=@Id  and IsArchive=0
";
                //IsAdmin
                //IsHRM
                //IsAttendance
                //IsPayroll
                //IsTAX
                //IsPF
                //IsGF
                //IsESS
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", GroupId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM.Id = Convert.ToInt32(dr["Id"].ToString());
                    VM.GroupName = dr["GroupName"].ToString();
                    VM.IsSuper = Convert.ToBoolean(dr["IsSuper"]);
                    VM.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    VM.IsHRM = Convert.ToBoolean(dr["IsHRM"]);
                    VM.IsAttendance = Convert.ToBoolean(dr["IsAttendance"]);
                    VM.IsPayroll = Convert.ToBoolean(dr["IsPayroll"]);
                    VM.IsTAX = Convert.ToBoolean(dr["IsTAX"]);
                    VM.IsPF = Convert.ToBoolean(dr["IsPF"]);
                    VM.IsGF = Convert.ToBoolean(dr["IsGF"]);
                    VM.IsESS = Convert.ToBoolean(dr["IsESS"]);
                    VM.Remarks = dr["Remarks"].ToString();
                    VM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    VM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    VM.CreatedBy = dr["CreatedBy"].ToString();
                    VM.CreatedFrom = dr["CreatedFrom"].ToString();
                    VM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    VM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    VM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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
            return VM;
        }
        //==================Insert =================
        public string[] Insert(UserGroupVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {

            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertUserGroup"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;


            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(UserGroupVM.UserGroupId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "UserGroup";
                string[] fieldName = { "GroupName" };
                string[] fieldValue = { vm.GroupName.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist

                #region Save

                if (vm != null)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO UserGroup(GroupName
,IsAdmin
,IsHRM
,IsAttendance
,IsPayroll
,IsTAX
,IsPF
,IsGF
,IsESS
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@GroupName
,@IsAdmin
,@IsHRM
,@IsAttendance
,@IsPayroll
,@IsTAX
,@IsPF
,@IsGF
,@IsESS
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@GroupName", vm.GroupName.Trim());
                    cmdInsert.Parameters.AddWithValue("@IsAdmin", vm.IsAdmin);
                    cmdInsert.Parameters.AddWithValue("@IsHRM", vm.IsHRM);
                    cmdInsert.Parameters.AddWithValue("@IsAttendance", vm.IsAttendance);
                    cmdInsert.Parameters.AddWithValue("@IsPayroll", vm.IsPayroll);
                    cmdInsert.Parameters.AddWithValue("@IsTAX", vm.IsTAX);
                    cmdInsert.Parameters.AddWithValue("@IsPF", vm.IsPF);
                    cmdInsert.Parameters.AddWithValue("@IsGF", vm.IsGF);
                    cmdInsert.Parameters.AddWithValue("@IsESS", vm.IsESS);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This UserGroup already used!";
                    throw new ArgumentNullException("Please Input UserGroup Value", "");
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
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id.ToString();

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
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

            #region Results

            return retResults;
            #endregion


        }
        //==================Update =================
        public string[] Update(UserGroupVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee UserGroup Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToUserGroup"); }

                #endregion open connection and transaction

                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "UserGroup";
                string[] fieldName = { "GroupName" };
                string[] fieldValue = { vm.GroupName.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist



                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update UserGroup set";
                    sqlText += " GroupName=@GroupName,";

                    sqlText += " IsAdmin=@IsAdmin,";
                    sqlText += " IsHRM=@IsHRM,";
                    sqlText += " IsAttendance=@IsAttendance,";
                    sqlText += " IsPayroll=@IsPayroll,";
                    sqlText += " IsTAX=@IsTAX,";
                    sqlText += " IsPF=@IsPF,";
                    sqlText += " IsGF=@IsGF,";
                    sqlText += " IsESS=@IsESS,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@GroupName", vm.GroupName.Trim());
                    cmdUpdate.Parameters.AddWithValue("@IsAdmin", vm.IsAdmin);
                    cmdUpdate.Parameters.AddWithValue("@IsHRM", vm.IsHRM);
                    cmdUpdate.Parameters.AddWithValue("@IsAttendance", vm.IsAttendance);
                    cmdUpdate.Parameters.AddWithValue("@IsPayroll", vm.IsPayroll);
                    cmdUpdate.Parameters.AddWithValue("@IsTAX", vm.IsTAX);
                    cmdUpdate.Parameters.AddWithValue("@IsPF", vm.IsPF);
                    cmdUpdate.Parameters.AddWithValue("@IsGF", vm.IsGF);
                    cmdUpdate.Parameters.AddWithValue("@IsESS", vm.IsESS);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", UserGroupVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("UserGroup Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update UserGroup.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
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
        //==================Delete =================
        public string[] Delete(UserGroupVM vm, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteUserGroup"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToUserGroup"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update UserGroup set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("UserGroup Delete", vm.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("UserGroup Information Delete", "Could not found any item.");
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
                    retResults[1] = "Data Delete Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to delete UserGroup Information.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
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
        public List<UserGroupVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<UserGroupVM> VMs = new List<UserGroupVM>();
            UserGroupVM vm;
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
Id,
GroupName
   FROM UserGroup
WHERE IsArchive=0 and IsActive=1
    ORDER BY GroupName
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new UserGroupVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Name = dr["GroupName"].ToString();
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
        public List<string> Autocomplete(string term)
        {

            #region Variables

            SqlConnection currConn = null;
            List<string> VMs = new List<string>();

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
                sqlText = "";
                sqlText = @"SELECT Id, GroupName    FROM UserGroup ";
                sqlText += @" WHERE GroupName like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY GroupName";



                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    VMs.Insert(i, dr["GroupName"].ToString());
                    i++;
                }
                dr.Close();
                VMs.Sort();
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
