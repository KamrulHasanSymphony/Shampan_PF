using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SymServices.Common
{
    public class SymUserRollDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion

        #region Methods
        //==================SelectAll=================
        public List<SymUserRollVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SymUserRollVM> VMs = new List<SymUserRollVM>();
            SymUserRollVM VM;
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
,BranchId
,GroupId
,symArea
,symController
,IsIndex
,IsAdd
,IsEdit
,IsDelete
,IsReport
,IsProcess
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
From SymUserRoll
Where IsArchive=0 

ORDER BY symArea
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new SymUserRollVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GroupId = Convert.ToInt32(dr["GroupId"].ToString());
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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

        public List<SymUserRollVM> SelectAllByGroupId(string GroupId, string SymArea = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SymUserRollVM> VMs = new List<SymUserRollVM>();
            SymUserRollVM VM;
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
SymUserRoll.Id
,SymUserRoll.BranchId
,SymUserRoll.GroupId
,sd.symArea
,sd.symController
,SymUserRoll.IsIndex
,SymUserRoll.IsAdd
,SymUserRoll.IsEdit
,SymUserRoll.IsDelete
,SymUserRoll.IsReport
,SymUserRoll.IsProcess
,SymUserRoll.Remarks
,SymUserRoll.IsActive
,SymUserRoll.IsArchive
,SymUserRoll.CreatedBy
,SymUserRoll.CreatedAt
,SymUserRoll.CreatedFrom
,SymUserRoll.LastUpdateBy
,SymUserRoll.LastUpdateAt
,SymUserRoll.LastUpdateFrom
From SymUserRoll
left outer join SymUserDefaultRoll sd on sd.Id=SymUserRoll.DefaultRollId
Where SymUserRoll.IsArchive=0 
and SymUserRoll.GroupId=@GroupId  

";
                if (!string.IsNullOrWhiteSpace(SymArea))
                {
                    sqlText += " and symArea=@symArea ";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@GroupId", GroupId);
                if (!string.IsNullOrWhiteSpace(SymArea))
                {
                    objComm.Parameters.AddWithValue("@symArea", SymArea);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new SymUserRollVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GroupId = Convert.ToInt32(dr["GroupId"]);
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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
        //==================SelectAll=================
        public List<SymUserDefaultRollVM> SelectSymUserDefaultRollAll(string GroupId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SymUserDefaultRollVM> VMs = new List<SymUserDefaultRollVM>();
            SymUserDefaultRollVM VM;
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
,BranchId
,symArea
,symController
,IsIndex
,IsAdd
,IsEdit
,IsDelete
,IsReport
,IsProcess
From SymUserDefaultRoll
Where IsArchive=0
and id not in(
select distinct DefaultRollId from SymUserRoll where GroupId=@GroupId
)
ORDER BY symArea
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@GroupId", GroupId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new SymUserDefaultRollVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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
        //==================SelectAll=================
        public List<UserLogsVM> SelectAllUser()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            UserLogsVM userVM = new UserLogsVM();
            List<UserLogsVM> userVMs = new List<UserLogsVM>();
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
,FullName
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
From [User]";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    userVM = new UserLogsVM();
                    userVM.Id = dr["Id"].ToString();
                    userVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    userVM.FullName = dr["FullName"].ToString();
                    userVM.Email = dr["Email"].ToString();
                    userVM.LogID = dr["LogId"].ToString();
                    userVM.Password = dr["Password"].ToString();
                    userVM.VerificationCode = dr["VerificationCode"].ToString();
                    userVM.EmployeeId = dr["EmployeeId"].ToString();
                    userVM.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    userVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    userVM.IsVerified = Convert.ToBoolean(dr["IsVerified"]);
                    userVM.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
                    userVMs.Add(userVM);
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
            return userVMs;
        }
        public UserLogsVM SelectGroupId(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            UserLogsVM userVM = new UserLogsVM();
            List<UserLogsVM> userVMs = new List<UserLogsVM>();
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
,FullName
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
From [User]
where Id=@Id"
;
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    userVM.Id = dr["Id"].ToString();
                    userVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    userVM.FullName = dr["FullName"].ToString();
                    userVM.Email = dr["Email"].ToString();
                    userVM.LogID = dr["LogId"].ToString();
                    userVM.Password = dr["Password"].ToString();
                    userVM.VerificationCode = dr["VerificationCode"].ToString();
                    userVM.EmployeeId = dr["EmployeeId"].ToString();
                    userVM.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    userVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    userVM.IsVerified = Convert.ToBoolean(dr["IsVerified"]);
                    userVM.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
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
            return userVM;
        }
        public UserLogsVM SelectUserByEmployee(string EmployeeId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            UserLogsVM userVM = new UserLogsVM();
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

                sqlText = @"SELECT top 1
 Id
,FullName
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
From [User]
Where EmployeeId=@EmployeeId
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    userVM.Id = dr["Id"].ToString();
                    userVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    userVM.FullName = dr["FullName"].ToString();
                    userVM.Email = dr["Email"].ToString();
                    userVM.LogID = dr["LogId"].ToString();
                    userVM.Password = dr["Password"].ToString();
                    userVM.VerificationCode = dr["VerificationCode"].ToString();
                    userVM.EmployeeId = dr["EmployeeId"].ToString();
                    userVM.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
                    userVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    userVM.IsVerified = Convert.ToBoolean(dr["IsVerified"]);
                    userVM.IsArchived = Convert.ToBoolean(dr["IsArchived"]);
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

            return userVM;
        }
        //==================SelectByID=================
        public SymUserRollVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SymUserRollVM VM = new SymUserRollVM();

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
,BranchId
,GroupId
,symArea
,symController
,IsIndex
,IsAdd
,IsEdit
,IsDelete
,IsReport
,IsProcess
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
From SymUserRoll
Where id=@Id and IsArchive=0
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GroupId = Convert.ToInt32(dr["GroupId"].ToString());
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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
        //==================SelectByID=================
        public List<SymUserDefaultRollVM> SelectSymUserById(string Id)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SymUserDefaultRollVM VM = new SymUserDefaultRollVM();
            List<SymUserDefaultRollVM> VMs = new List<SymUserDefaultRollVM>();
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
                sqlText = @"SELECT* from 
(select s.Id
,s.DefaultRollId ,s.BranchId,us.Id GroupId,symArea,symController,IsIndex,IsAdd,IsEdit,IsDelete,IsReport,IsProcess
,Remarks,s.IsActive,s.IsArchive,s.CreatedBy,s.CreatedAt,s.CreatedFrom,s.LastUpdateBy,s.LastUpdateAt,s.LastUpdateFrom
,us.FullName,us.Email,us.Password from SymUserRoll s
LEFT OUTER JOIN dbo.[User] us ON us.Id=s.GroupId
where us.id=@Id
 union all
select '0' Id
,id DefaultRollId,BranchId,'0' GroupId,symArea,symController,IsIndex,IsAdd,IsEdit,IsDelete,IsReport,IsProcess
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt
,LastUpdateFrom,'NA'FullName,'NA'Email,'NA' Password
from SymUserDefaultRoll where 1=1
and id not in( select DefaultRollId from SymUserRoll)) as a";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new SymUserDefaultRollVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GroupId = Convert.ToInt32(dr["GroupId"]);
                    VM.FullName = dr["FullName"].ToString();
                    VM.Email = dr["Email"].ToString();
                    VM.Password = dr["Password"].ToString();
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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
        public SymUserRollVM SelectById(string Id, string symArea, string symController)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SymUserRollVM VM = new SymUserRollVM();

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
,BranchId
,GroupId
,symArea
,symController
,IsIndex
,IsAdd
,IsEdit
,IsDelete
,IsReport
,IsProcess
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
From SymUserRoll
Where IsArchive=0
ORDER BY symArea
Where Id=@Id and symArea=@symArea and symController=@symController  and IsArchive=0
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                objComm.Parameters.AddWithValue("@symArea", symArea);
                objComm.Parameters.AddWithValue("@symController", symController);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GroupId = Convert.ToInt32(dr["GroupId"]);
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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
        public List<SymUserDefaultRollVM> UserRollDetail(string empId, string symArea)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SymUserDefaultRollVM VM = new SymUserDefaultRollVM();
            List<SymUserDefaultRollVM> VMs = new List<SymUserDefaultRollVM>();

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

                sqlText = @"SELECT* from 
(select s.Id
,s.DefaultRollId ,s.BranchId,us.Id GroupId,symArea,symController
,IsIndex,IsAdd,IsEdit,IsDelete,IsReport
,IsProcess,Remarks,s.IsActive,s.IsArchive,s.CreatedBy,s.CreatedAt,s.CreatedFrom,s.LastUpdateBy
,s.LastUpdateAt
,s.LastUpdateFrom
,us.FullName,us.Email,us.Password from SymUserRoll s
LEFT OUTER JOIN dbo.[UserGroup] us ON us.Id=s.GroupId
where 1=1";
                if (!string.IsNullOrWhiteSpace(empId) && empId != "undefined" && empId != "null")
                {
                    sqlText += @" and s.GroupId=@Id";
                }
                if (!string.IsNullOrWhiteSpace(symArea) && symArea != "" && symArea != "null" && symArea != "undefined")
                {
                    sqlText += @" and s.symArea=@symArea";
                }


                sqlText += @" 
 union all
select '0' Id
,id DefaultRollId
,BranchId
,'0' GroupId
,symArea
,symController
,IsIndex
,IsAdd
,IsEdit
,IsDelete
,IsReport
,IsProcess
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom,'NA'FullName,'NA'Email,'NA' Password
from SymUserDefaultRoll where 1=1";
                if (!string.IsNullOrWhiteSpace(symArea) && symArea != "" && symArea != "null" && symArea != "undefined")
                {
                    sqlText += @" and symArea=@symArea";
                }


                sqlText += @"  and id not in( select DefaultRollId from SymUserRoll)) as a ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(empId) && empId != "undefined" && empId != "null")
                    objComm.Parameters.AddWithValue("@Id", empId);
                if (!string.IsNullOrWhiteSpace(symArea) && symArea != "" && symArea != "null")
                    objComm.Parameters.AddWithValue("@symArea", symArea);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    VM = new SymUserDefaultRollVM();
                    VM.Id = dr["Id"].ToString();
                    VM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    VM.GroupId = Convert.ToInt32(dr["GroupId"]);
                    VM.FullName = dr["FullName"].ToString();
                    VM.Email = dr["Email"].ToString();
                    VM.Password = dr["Password"].ToString();
                    VM.symArea = dr["symArea"].ToString();
                    VM.symController = dr["symController"].ToString();
                    VM.IsIndex = Convert.ToBoolean(dr["IsIndex"].ToString());
                    VM.IsAdd = Convert.ToBoolean(dr["IsAdd"].ToString());
                    VM.IsEdit = Convert.ToBoolean(dr["IsEdit"].ToString());
                    VM.IsDelete = Convert.ToBoolean(dr["IsDelete"].ToString());
                    VM.IsReport = Convert.ToBoolean(dr["IsReport"].ToString());
                    VM.IsProcess = Convert.ToBoolean(dr["IsProcess"].ToString());
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
        public string[] SelectAllSymRollwithInsert(SymUserRollVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            string sqlText1 = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertSymUserRoll"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(SymUserRollVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
                CommonDAL cdal = new CommonDAL();
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
                sqlText1 = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SymUserRoll where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText1, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);
                var SymDefaultRollList = SelectSymUserDefaultRollAll(vm.GroupId.ToString());
                foreach (var item in SymDefaultRollList)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO SymUserRoll(Id,BranchId,DefaultRollId,GroupId,IsIndex,IsAdd,IsEdit,IsDelete,IsReport,IsProcess,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@DefaultRollId,@GroupId,@IsIndex,@IsAdd,@IsEdit,@IsDelete,@IsReport,@IsProcess,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    var symId = vm.BranchId.ToString() + "_" + (count + 1);
                    cmdInsert.Parameters.AddWithValue("@Id", symId);
                    cmdInsert.Parameters.AddWithValue("@GroupId", vm.GroupId);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@DefaultRollId", item.Id.Trim());
                    cmdInsert.Parameters.AddWithValue("@IsIndex", item.IsIndex);
                    cmdInsert.Parameters.AddWithValue("@IsAdd", item.IsAdd);
                    cmdInsert.Parameters.AddWithValue("@IsEdit", item.IsEdit);
                    cmdInsert.Parameters.AddWithValue("@IsDelete", item.IsDelete);
                    cmdInsert.Parameters.AddWithValue("@IsReport", item.IsReport);
                    cmdInsert.Parameters.AddWithValue("@IsProcess", item.IsProcess);
                    cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, SymUserRollVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                    count++;

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
                //retResults[2] = vm.Id;

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
        //==================Insert =================
        public string[] Insert(SymUserRollVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            string sqlText1 = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertSymUserRoll"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(SymUserRollVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
                CommonDAL cdal = new CommonDAL();
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
                foreach (var item in vm.SymUserDefaultRollVMs)
                {

                    if (1 == 1)
                    {
                        CommonDAL _cdal = new CommonDAL();
                        var checkexit = _cdal.CheckDuplicateInInsert("SymUserRoll", "GroupId", vm.UserlogVM.Id, null, null);
                        if (checkexit == false)
                        {
                            sqlText = "  ";
                            sqlText += @" INSERT INTO SymUserRoll(Id,BranchId,DefaultRollId,GroupId,symArea,symController,IsIndex,IsAdd,IsEdit,IsDelete,IsReport,IsProcess,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@DefaultRollId,@GroupId,@symArea,@symController,@IsIndex,@IsAdd,@IsEdit,@IsDelete,@IsReport,@IsProcess,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom)";


                        }
                        else
                        {
                            sqlText = "";
                            sqlText = "update SymUserRoll set";
                            sqlText += "  IsIndex=@IsIndex";
                            sqlText += " , IsAdd=@IsAdd";
                            sqlText += " , IsEdit=@IsEdit";
                            sqlText += " , IsDelete=@IsDelete";
                            sqlText += " , IsReport=@IsReport";
                            sqlText += " , IsProcess=@IsProcess";
                            sqlText += " , Remarks=@Remarks";
                            sqlText += " , LastUpdateBy=@LastUpdateBy";
                            sqlText += " , LastUpdateAt=@LastUpdateAt";
                            sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                            sqlText += " where symArea=@symArea and symController=@symController and GroupId=@GroupId ";

                        }
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        sqlText1 = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from SymUserRoll where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText1, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        var symId = vm.BranchId.ToString() + "_" + (count + 1);
                        if (checkexit == false)
                        {
                            cmdInsert.Parameters.AddWithValue("@Id", symId);
                        }
                        cmdInsert.Parameters.AddWithValue("@GroupId", vm.UserlogVM.Id);
                        cmdInsert.Parameters.AddWithValue("@symArea", item.symArea.Trim());
                        cmdInsert.Parameters.AddWithValue("@symController", item.symController.Trim());
                        cmdInsert.Parameters.AddWithValue("@IsIndex", item.IsIndex);
                        cmdInsert.Parameters.AddWithValue("@IsAdd", item.IsAdd);
                        cmdInsert.Parameters.AddWithValue("@IsEdit", item.IsEdit);
                        cmdInsert.Parameters.AddWithValue("@IsDelete", item.IsDelete);
                        cmdInsert.Parameters.AddWithValue("@IsReport", item.IsReport);
                        cmdInsert.Parameters.AddWithValue("@IsProcess", item.IsProcess);
                        cmdInsert.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, SymUserRollVM.Remarks);
                        cmdInsert.Parameters.AddWithValue("@LastUpdateBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@LastUpdateAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@LastUpdateFrom", vm.CreatedFrom);
                        cmdInsert.Transaction = transaction;
                        cmdInsert.ExecuteNonQuery();


                    }
                    else
                    {
                        retResults[1] = "This SymUserRoll already used";
                        throw new ArgumentNullException("Please Input SymUserRoll Value", "");
                    }
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
                //retResults[2] = vm.Id;

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
            retResults[5] = "Employee SymUserRoll Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSymUserRoll"); }

                #endregion open connection and transaction

                foreach (var item in vm.SymUserRollVMs)
                    if (vm != null)
                    {
                        #region Update Settings
                        sqlText = "";
                        sqlText = "update SymUserRoll set";
                        sqlText += "  GroupId=@GroupId";

                        sqlText += " , IsIndex=@IsIndex";
                        sqlText += " , IsAdd=@IsAdd";
                        sqlText += " , IsEdit=@IsEdit";
                        sqlText += " , IsDelete=@IsDelete";
                        sqlText += " , IsReport=@IsReport";
                        sqlText += " , IsProcess=@IsProcess";
                        sqlText += " , Remarks=@Remarks";
                        sqlText += " , IsActive=@IsActive";
                        sqlText += " , LastUpdateBy=@LastUpdateBy";
                        sqlText += " , LastUpdateAt=@LastUpdateAt";
                        sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", item.Id);
                        cmdUpdate.Parameters.AddWithValue("@GroupId", item.GroupId.ToString().Trim());

                        cmdUpdate.Parameters.AddWithValue("@IsIndex", item.IsIndex);
                        cmdUpdate.Parameters.AddWithValue("@IsAdd", item.IsAdd);
                        cmdUpdate.Parameters.AddWithValue("@IsEdit", item.IsEdit);
                        cmdUpdate.Parameters.AddWithValue("@IsDelete", item.IsDelete);
                        cmdUpdate.Parameters.AddWithValue("@IsReport", item.IsReport);
                        cmdUpdate.Parameters.AddWithValue("@IsProcess", item.IsProcess);
                        cmdUpdate.Parameters.AddWithValue("@Remarks", item.Remarks ?? Convert.DBNull);//, SymUserRollVM.Remarks);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", true);
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
                            // throw new ArgumentNullException("Education Update", SymUserRollVM.BranchId + " could not updated.");
                        }

                        #endregion Commit

                        #endregion Update Settings
                        iSTransSuccess = true;
                    }
                    else
                    {
                        throw new ArgumentNullException("SymUserRoll Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update SymUserRoll.";
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
        public string[] Delete(SymUserRollVM SymUserRollVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSymUserRoll"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSymUserRoll"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update SymUserRoll set";
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
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", SymUserRollVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", SymUserRollVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", SymUserRollVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("SymUserRoll Delete", SymUserRollVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("SymUserRoll Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete SymUserRoll Information.";
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

        public List<SymUserRollVM> DropDownsymArea()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SymUserRollVM> VMs = new List<SymUserRollVM>();
            SymUserRollVM vm;
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

                sqlText = @"SELECT DISTINCT
symArea
   FROM SymUserDefaultRoll
WHERE IsArchive=0 and IsActive=1
    ORDER BY symArea
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SymUserRollVM();

                    vm.symArea = dr["symArea"].ToString();
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

        public List<SymUserRollVM> DropDownsymController()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SymUserRollVM> VMs = new List<SymUserRollVM>();
            SymUserRollVM vm;
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

                sqlText = @"SELECT DISTINCT
symController
   FROM SymUserDefaultRoll
WHERE IsArchive=0 and IsActive=1
    ORDER BY symController
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SymUserRollVM();
                    vm.symController = dr["symController"].ToString();
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
        #endregion


        public DataTable RollByGroupId(string userId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();

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
SymUserRoll.Id
,SymUserRoll.DefaultRollId
,SymUserRoll.BranchId
,SymUserRoll.GroupId
,r.symArea
,r.symController
,SymUserRoll.IsIndex
,SymUserRoll.IsAdd
,SymUserRoll.IsEdit
,SymUserRoll.IsDelete
,SymUserRoll.IsReport
,SymUserRoll.IsProcess

From SymUserRoll 
left outer join UserGroup ug on SymUserRoll.GroupId=ug.Id
left outer join [User] u on u.GroupId=ug.Id
left outer join SymUserDefaultRoll r on r.Id=SymUserRoll.DefaultRollId
Where u.id=@userId 
";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@userId", userId);
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(dt);



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
            return dt;
        }


        public bool SymRollSessionBackup(string UserId, string symArea, string symController, string symAction)
        {
            DataTable dt = new DataTable();
            DataTable returndt = new DataTable();
            bool result = false;

            try
            {
                dt = HttpContext.Current.Session[UserId.ToString().Trim() + "-SymRoll"] as DataTable;

                returndt = dt.Select("symArea='" + symArea + "' and symController='" + symController + "'").CopyToDataTable();
                if (symAction.ToLower() == "index")
                    result = Convert.ToBoolean(returndt.Rows[0]["IsIndex"]);
                else if (symAction.ToLower() == "add")
                    result = Convert.ToBoolean(returndt.Rows[0]["IsAdd"]);
                else if (symAction.ToLower() == "edit")
                    result = Convert.ToBoolean(returndt.Rows[0]["IsEdit"]);
                else if (symAction.ToLower() == "delete")
                    result = Convert.ToBoolean(returndt.Rows[0]["IsDelete"]);
                else if (symAction.ToLower() == "report")
                    result = Convert.ToBoolean(returndt.Rows[0]["IsReport"]);
                else if (symAction.ToLower() == "process")
                    result = Convert.ToBoolean(returndt.Rows[0]["IsProcess"]);
            }
            catch (Exception)
            {
                return result;

            }
            return result = true;
        }

        public bool SymRoleSession(string UserId, string DefaultRollId, string symAction)
        {
            DataTable dt = new DataTable();
            DataTable returndt = new DataTable();
            bool result = false;

            try
            {
                dt = HttpContext.Current.Session[UserId.ToString().Trim() + "-SymRoll"] as DataTable;
                if (dt.Rows.Count > 0)
                {
                    returndt = dt.Select("DefaultRollId='" + DefaultRollId + "'").CopyToDataTable();
                    if (returndt.Rows.Count > 0)
                    {
                        if (symAction.ToLower() == "index")
                            result = Convert.ToBoolean(returndt.Rows[0]["IsIndex"]);
                        else if (symAction.ToLower() == "add")
                            result = Convert.ToBoolean(returndt.Rows[0]["IsAdd"]);
                        else if (symAction.ToLower() == "edit")
                            result = Convert.ToBoolean(returndt.Rows[0]["IsEdit"]);
                        else if (symAction.ToLower() == "delete")
                            result = Convert.ToBoolean(returndt.Rows[0]["IsDelete"]);
                        else if (symAction.ToLower() == "report")
                            result = Convert.ToBoolean(returndt.Rows[0]["IsReport"]);
                        else if (symAction.ToLower() == "process")
                            result = Convert.ToBoolean(returndt.Rows[0]["IsProcess"]);
                    }
                }
            }
            catch (Exception)
            {
                return result=true;

            }
            return result = true;
        }

    }
}
