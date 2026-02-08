using Excel;
using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class BranchDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<BranchVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<BranchVM> branchVMs = new List<BranchVM>();
            BranchVM branchVM;
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
,CompanyId
,Code
,Name
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Branch
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
                    branchVM = new BranchVM();
                    branchVM.Id = Convert.ToInt32(dr["Id"]);
                    branchVM.CompanyId = Convert.ToInt32(dr["CompanyId"]);
                    branchVM.Code = dr["Code"].ToString();
                    branchVM.Name = dr["Name"].ToString();

                    branchVM.Address = dr["Address"].ToString();
                    branchVM.District = dr["District"].ToString();
                    branchVM.Division = dr["Division"].ToString();
                    branchVM.Country = dr["Country"].ToString();
                    branchVM.City = dr["City"].ToString();
                    branchVM.PostalCode = dr["PostalCode"].ToString();
                    branchVM.Phone = dr["Phone"].ToString();
                    branchVM.Mobile = dr["Mobile"].ToString();
                    branchVM.Fax = dr["Fax"].ToString();
                    
                    branchVM.Remarks = dr["Remarks"].ToString();
                    branchVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    branchVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    branchVM.CreatedBy = dr["CreatedBy"].ToString();
                    branchVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    branchVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    branchVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    branchVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    branchVMs.Add(branchVM);
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

            return branchVMs;
        }
        //==================SelectByID=================
        public BranchVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            BranchVM branchVM = new BranchVM();

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
,CompanyId
,Code
,Name
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Branch
Where  id=@Id and IsArchive=0    
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
                    branchVM.Id = Convert.ToInt32(dr["Id"]);
                    branchVM.CompanyId = Convert.ToInt32(dr["CompanyId"]);
                    branchVM.Code = dr["Code"].ToString();
                    branchVM.Name = dr["Name"].ToString();

                    branchVM.Address = dr["Address"].ToString();
                    branchVM.District = dr["District"].ToString();
                    branchVM.Division = dr["Division"].ToString();
                    branchVM.Country = dr["Country"].ToString();
                    branchVM.City = dr["City"].ToString();
                    branchVM.PostalCode = dr["PostalCode"].ToString();
                    branchVM.Phone = dr["Phone"].ToString();
                    branchVM.Mobile = dr["Mobile"].ToString();
                    branchVM.Fax = dr["Fax"].ToString();

                    branchVM.Remarks = dr["Remarks"].ToString();
                    branchVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    branchVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    branchVM.CreatedBy = dr["CreatedBy"].ToString();
                    branchVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    branchVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    branchVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    branchVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return branchVM;
        }
        //==================Insert =================
        public string[] Insert(BranchVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertBranch"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(BranchVM.DepartmentId))
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
                string tableName = "Branch";	
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
				
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                      
                    }
                    else
                    {
                        if (1 == 1)
                        {

                            sqlText = "  ";
                            sqlText += @" INSERT INTO Branch(
                                            CompanyId,Code,Name
                                            ,Address,District,Division,Country,City,PostalCode,Phone,Mobile,Fax
                                ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@CompanyId,@Code,@Name
                                        ,@Address,@District,@Division,@Country,@City,@PostalCode,@Phone,@Mobile,@Fax
                                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        SELECT SCOPE_IDENTITY()";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@CompanyId", 1);
                            cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                            cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());

                            cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile);
                            cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);

                            cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, branchVM.Remarks);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                            cmdInsert.Transaction = transaction;
                            var exeRes = cmdInsert.ExecuteScalar();
                            Id = Convert.ToInt32(exeRes);

                            if (Id <= 0)
                            {
                                retResults[1] = "Please Input Branch Value";
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Please Input Branch Value", "");
                            }
                        }
                        else
                        {
                            retResults[1] = "This Branch already used";
                            throw new ArgumentNullException("Please Input Branch Value", "");
                        }
                       #endregion Save
                    }
                }
                #endregion Exist	           
                #region Save
              
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
                retResults[2] = Id.ToString();

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
        public string[] Update(BranchVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Branch Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToBranch"); }

                #endregion open connection and transaction

                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Branch";				
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

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
                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Branch ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", BranchVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", BranchVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", BranchVM.Name.Trim());

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Travel already used";
                //    throw new ArgumentNullException("Please Input Travel Value", "");
                //}
                //#endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Branch set";
                    sqlText += " CompanyId=@CompanyId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";

                    sqlText += " Address=@Address,";
                    sqlText += " District=@District,";
                    sqlText += " Division=@Division,";
                    sqlText += " Country=@Country,";
                    sqlText += " City=@City,";
                    sqlText += " PostalCode=@PostalCode,";
                    sqlText += " Phone=@Phone,";
                    sqlText += " Mobile=@Mobile,";
                    sqlText += " Fax=@Fax,";

                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@CompanyId", vm.CompanyId);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());

                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", vm.Mobile);
                    cmdUpdate.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, branchVM.Remarks);
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
                        // throw new ArgumentNullException("Education Update", BranchVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Branch Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Branch.";
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
        //==================Select =================
        public BranchVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            BranchVM branchVM = new BranchVM();

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
,CompanyId
,Code
,Name
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Branch
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
                        branchVM.Id = Convert.ToInt32(dr["Id"]);
                        branchVM.CompanyId = Convert.ToInt32(dr["BranchId"]);
                        branchVM.Code = dr["Code"].ToString();
                        branchVM.Name = dr["Name"].ToString();

                        branchVM.Name = dr["Address"].ToString();
                        branchVM.Name = dr["District"].ToString();
                        branchVM.Name = dr["Division"].ToString();
                        branchVM.Name = dr["Country"].ToString();
                        branchVM.Name = dr["City"].ToString();
                        branchVM.Name = dr["PostalCode"].ToString();
                        branchVM.Name = dr["Phone"].ToString();
                        branchVM.Name = dr["Mobile"].ToString();
                        branchVM.Name = dr["Fax"].ToString();

                        branchVM.Remarks = dr["Remarks"].ToString();
                        branchVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        branchVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        branchVM.CreatedBy = dr["CreatedBy"].ToString();
                        branchVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        branchVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        branchVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        branchVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return branchVM;
        }
        //==================Delete =================
        public string[] Delete(BranchVM branchVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteBranch"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBranch"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Branch set";
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
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", branchVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", branchVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", branchVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
				        var exeRes = cmdUpdate.ExecuteNonQuery();
				        transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Branch Delete", branchVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Branch Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Branch Information.";
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
        public List<BranchVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<BranchVM> VMs = new List<BranchVM>();
            BranchVM vm;
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
Id,
Name
   FROM Branch
WHERE IsArchive=0 and IsActive=1
   -- ORDER BY Name
";
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BranchVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
        #endregion



        public string[] InsertExportData(BranchVM paramVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                BranchVM vBranchVM = new BranchVM();

                #region Assign Data
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    vBranchVM.CompanyId = Convert.ToInt32(dr["CompanyId"].ToString());
                    vBranchVM.Code = dr["Code"].ToString();
                    vBranchVM.Name = dr["Name"].ToString();
                    vBranchVM.Address = dr["Address"].ToString();
                    vBranchVM.District = dr["District"].ToString();
                    vBranchVM.Division = dr["Division"].ToString();
                    vBranchVM.City = dr["City"].ToString();
                    vBranchVM.PostalCode = dr["PostalCode"].ToString();
                    vBranchVM.Phone = dr["Phone"].ToString();
                    vBranchVM.Mobile = dr["Mobile"].ToString();
                    vBranchVM.Fax = dr["Fax"].ToString();
                    vBranchVM.Remarks = dr["Remarks"].ToString();
                    vBranchVM.CreatedAt = paramVM.CreatedAt;
                    vBranchVM.CreatedBy = paramVM.CreatedBy;
                    vBranchVM.CreatedFrom = paramVM.CreatedFrom;
                    retResults = Insert(vBranchVM, currConn, transaction);
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
