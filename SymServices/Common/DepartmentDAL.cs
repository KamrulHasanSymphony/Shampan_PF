using Excel;
using SymOrdinary;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
namespace SymServices.Common
{
    public class DepartmentDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<DepartmentVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<DepartmentVM> departmentVMs = new List<DepartmentVM>();
            DepartmentVM departmentVM;
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
,Code
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Department
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    departmentVM = new DepartmentVM();
                    departmentVM.Id = dr["Id"].ToString();
                    departmentVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    departmentVM.Code = dr["Code"].ToString();
                    departmentVM.Name = dr["Name"].ToString();
                    departmentVM.Remarks = dr["Remarks"].ToString();
                    departmentVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    departmentVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    departmentVM.CreatedBy = dr["CreatedBy"].ToString();
                    departmentVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    departmentVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    departmentVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    departmentVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    departmentVMs.Add(departmentVM);
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
            return departmentVMs;
        }
        //==================SelectByID=================
        public DepartmentVM SelectById(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DepartmentVM departmentVM = new DepartmentVM();
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
,Code
,BranchId
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Department
Where  id=@Id  and IsArchive=0
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
                    departmentVM.Id = dr["Id"].ToString();
                    departmentVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    departmentVM.Code = dr["Code"].ToString();
                    departmentVM.Name = dr["Name"].ToString();
                    departmentVM.Remarks = dr["Remarks"].ToString();
                    departmentVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    departmentVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    departmentVM.CreatedBy = dr["CreatedBy"].ToString();
                    departmentVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    departmentVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    departmentVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    departmentVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
            return departmentVM;
        }
        //==================Insert =================
        public string[] Insert(DepartmentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertDepartment"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                bool iSTransSuccess = false;
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
                string tableName = "Department";	
                string[] fieldName = { "Code"};
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {

                        sqlText = "  ";
                        sqlText += @"UPDATE Department SET 
                                    BranchId = @BranchId, 
                                    Code = @Code, 
                                    Name = @Name, 
                                    Remarks = @Remarks, 
                                    IsActive = @IsActive, 
                                    IsArchive = @IsArchive, 
                                    CreatedBy = @CreatedBy, 
                                    CreatedAt = @CreatedAt, 
                                    CreatedFrom = @CreatedFrom
                                WHERE Id = @Id;
                                ";
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                        cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                        cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                        cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
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
                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Department where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                        //int foundId = (int)objfoundId;
                        if (1 == 1)
                        {
                            sqlText = "  ";
                            sqlText += @" INSERT INTO Department(Id,BranchId,Code,Name,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@Code,@Name,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";
                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                            cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                            cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                            cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                            cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                            cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                            cmdInsert.Transaction = transaction;
                            cmdInsert.ExecuteNonQuery();
                            iSTransSuccess = true;
                        }
                        else
                        {
                            retResults[1] = "This Department already used!";
                            throw new ArgumentNullException("Please Input Department Value", "");
                        }
                        #endregion Save
                    }
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
                #endregion Exist	
               
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id;
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
        public string[] Update(DepartmentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Department Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToDepartment"); }
                #endregion open connection and transaction
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Department";				
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdateWithBranch(vm.Id, tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Department ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", DepartmentVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", DepartmentVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", DepartmentVM.Name);
                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Travel already used!";
                //    throw new ArgumentNullException("Please Input Travel Value", "");
                //}
                //#endregion Exist
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update Department set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
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
                        // throw new ArgumentNullException("Education Update", DepartmentVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Department Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Department.";
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
        public string[] Delete(DepartmentVM departmentVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteDepartment"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToDepartment"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Department set";
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
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", departmentVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", departmentVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", departmentVM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
						var exeRes = cmdUpdate.ExecuteNonQuery();
						transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Department Delete", departmentVM.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Department Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Department Information.";
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
        public List<DepartmentVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<DepartmentVM> VMs = new List<DepartmentVM>();
            DepartmentVM vm;
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
Name
   FROM Department
WHERE IsArchive=0 and IsActive=1
    ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DepartmentVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
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
        public List<AppraisalCategoryVM> AppraisalCategory()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalCategoryVM> VMs = new List<AppraisalCategoryVM>();
            AppraisalCategoryVM vm;
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
CategoryName
   FROM AppraisalCategory
WHERE IsActive=1
    ORDER BY CategoryName
";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalCategoryVM();
                    vm.Id =Convert.ToInt32(dr["Id"].ToString());
                    vm.CategoryName = dr["CategoryName"].ToString();
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
        public List<AppraisalAssignToVM> AppraisalAssignTo()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalAssignToVM> VMs = new List<AppraisalAssignToVM>();
            AppraisalAssignToVM vm;
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
                        AssignToName
                           FROM AppraisalAssignTo
                        WHERE IsActive=1
                            ORDER BY AssignToName
                        ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalAssignToVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.AssignToName = dr["AssignToName"].ToString();
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
        public List<AppraisalQuestionBankVM> AppraisalQuestion()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalQuestionBankVM> VMs = new List<AppraisalQuestionBankVM>();
            AppraisalQuestionBankVM vm;
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
                        Question
                           FROM AppraisalQuestionBank
                        WHERE IsActive=1
                            ORDER BY Question
                        ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalQuestionBankVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.Question = dr["Question"].ToString();
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
        
        public List<AppraisalEvaluationVM> EvaluationFor()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalEvaluationVM> VMs = new List<AppraisalEvaluationVM>();
            AppraisalEvaluationVM vm;
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
                        EvaluationName
                           FROM AppraisalEvaluation
                        WHERE IsActive=1
                            ORDER BY EvaluationName
                        ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalEvaluationVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.EvaluationFor = dr["EvaluationName"].ToString();                   
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
        public List<AppraisalQuestionSetVM> AppraisalQuestionSet()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalQuestionSetVM> VMs = new List<AppraisalQuestionSetVM>();
            AppraisalQuestionSetVM vm;
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
                        QuestionSetName
                           FROM AppraisalQuestionsSet                      
                            ORDER BY QuestionSetName
                        ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalQuestionSetVM();
                    vm.Id= Convert.ToInt32(dr["Id"].ToString());
                    vm.QuestionSetName = dr["QuestionSetName"].ToString();
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
        public List<DropDownVM> DropDownByProject(string projectId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<DropDownVM> VMs = new List<DropDownVM>();
            DropDownVM vm = new DropDownVM();
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
select distinct  DepartmentId Id,Department Name
from ViewEmployeeInformation
WHERE   ProjectId=@ProjectId
ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@projectId", projectId);
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DropDownVM();
                    vm.value = dr["Id"].ToString();
                    vm.Text = dr["Name"].ToString();
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
            List<string> vms = new List<string>();
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
                sqlText = @"SELECT Id, Name    FROM Department ";
                sqlText += @" WHERE Name like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Name";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Name"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
            return vms;
        }
        public List<DepartmentVM> AutocompleteForSalary(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<DepartmentVM> VMs = new List<DepartmentVM>();
            DepartmentVM vm;
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
                sqlText = @" select DISTINCT  dept.Id Id,  dept.Name Name from  dbo.EmployeeTransfer emtf ";
                sqlText += @" left outer join dbo.Project proj  on emtf.ProjectId = proj.Id ";
                sqlText +=@" left outer join dbo.Department dept on emtf.DepartmentId=dept.Id ";
                sqlText += @" WHERE emtf.ProjectId = " + term + " and and emtf.IsCurrent=1 ORDER BY dept.Name";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DepartmentVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
                    VMs.Add(vm);
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
//==================Select =================
        public DepartmentVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DepartmentVM departmentVM = new DepartmentVM();
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
                sqlText = @"SELECT Top 1 
Id
,BranchId
,Code
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Department 
";
                if (query == null)
                {
                    if (Id != 0)
                    {
                        sqlText += " AND Id=@Id";
                    }
                    else
                    {
                        sqlText += " ORDER BY Id ";
                    }
                }
                else
                {
                    if (query == "FIRST")
                    {
                        sqlText += " ORDER BY Id ";
                    }
                    else if (query == "LAST")
                    {
                        sqlText += " ORDER BY Id DESC";
                    }
                    else if (query == "NEXT")
                    {
                        sqlText += " and  Id > @Id   ORDER BY Id";
                    }
                    else if (query == "PREVIOUS")
                    {
                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
                    }
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                try
                {
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        departmentVM.Id = dr["Id"].ToString();
                        departmentVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        departmentVM.Code = dr["Code"].ToString();
                        departmentVM.Name = dr["Name"].ToString();
                        departmentVM.Remarks = dr["Remarks"].ToString();
                        departmentVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        departmentVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        departmentVM.CreatedBy = dr["CreatedBy"].ToString();
                        departmentVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        departmentVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        departmentVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        departmentVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return departmentVM;
        }
        #endregion

        public List<AppraisalAssignToVM> AssignTo()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AppraisalAssignToVM> VMs = new List<AppraisalAssignToVM>();
            AppraisalAssignToVM vm;
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
                        AssignToName
                           FROM AppraisalAssignTo
                        WHERE IsActive=1
                            ORDER BY Id
                        ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new AppraisalAssignToVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.AssignToName = dr["AssignToName"].ToString();
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


        public string[] InsertExportData(DepartmentVM paramVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "ImportExcelFile"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                dt = ds.Tables[0];
                reader.Close();

                File.Delete(Fullpath);
                #endregion

                #region open connection and transaction
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
                string Code = "";

                DepartmentVM vDepartmentVM = new DepartmentVM();

                #region Assign Data
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    vDepartmentVM.Id = dr["Id"].ToString();
                    vDepartmentVM.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vDepartmentVM.Code = dr["Code"].ToString();
                    vDepartmentVM.Name = dr["Name"].ToString();
                    vDepartmentVM.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());
                    vDepartmentVM.Remarks = dr["Remarks"].ToString();
                    //vDepartmentVM.IsActive =Convert.ToBoolean (dr["IsActive"].ToString());
                    //vDepartmentVM.IsArchive = Convert.ToBoolean(dr["IsArchive"].ToString());
                    vDepartmentVM.CreatedAt = paramVM.CreatedAt;
                    vDepartmentVM.CreatedBy = paramVM.CreatedBy;
                    vDepartmentVM.CreatedFrom = paramVM.CreatedFrom;
                    //vDepartmentVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    //vDepartmentVM.LastUpdateAt = dr["LastUpdateAt"].ToString();
                    //vDepartmentVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    retResults = Insert(vDepartmentVM, currConn, transaction);
                }
                #endregion

                #region Data Insert


                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion
                #endregion
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex
                transaction.Rollback();
                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion

        }

    }
}
