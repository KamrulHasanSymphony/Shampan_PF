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
    public class ProjectDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<ProjectVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<ProjectVM> projectVMs = new List<ProjectVM>();
            ProjectVM projectVM;
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

,Startdate
,EndDate
,ManpowerRequired
,ContactPerson
,ContactPersonDesignation
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
    From Project
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
                    projectVM = new ProjectVM();
                    projectVM.Id = dr["Id"].ToString();
                    projectVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    projectVM.Code = dr["Code"].ToString();
                    projectVM.Name = dr["Name"].ToString();

                    projectVM.Startdate = dr["Startdate"].ToString();
                    projectVM.EndDate = dr["EndDate"].ToString();
                    projectVM.ManpowerRequired = Convert.ToInt32(dr["ManpowerRequired"]);
                    projectVM.ContactPerson = dr["ContactPerson"].ToString();
                    projectVM.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    projectVM.Address = dr["Address"].ToString();
                    projectVM.District = dr["District"].ToString();
                    projectVM.Division = dr["Division"].ToString();
                    projectVM.Country = dr["Country"].ToString();
                    projectVM.City = dr["City"].ToString();
                    projectVM.PostalCode = dr["PostalCode"].ToString();
                    projectVM.Phone = dr["Phone"].ToString();
                    projectVM.Mobile = dr["Mobile"].ToString();
                    projectVM.Fax = dr["Fax"].ToString();

                    projectVM.Remarks = dr["Remarks"].ToString();
                    projectVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    projectVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    projectVM.CreatedBy = dr["CreatedBy"].ToString();
                    projectVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    projectVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    projectVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    projectVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    projectVMs.Add(projectVM);
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

            return projectVMs;
        }
        //==================SelectByID=================
        public ProjectVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            ProjectVM projectVM = new ProjectVM();

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

,Startdate
,EndDate
,ManpowerRequired
,ContactPerson
,ContactPersonDesignation
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
    From Project
where  id=@Id and IsArchive=0 
     
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
                    projectVM.Id = dr["Id"].ToString();
                    projectVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    projectVM.Code = dr["Code"].ToString();
                    projectVM.Name = dr["Name"].ToString();

                    projectVM.Startdate = dr["Startdate"].ToString();
                    projectVM.EndDate = dr["EndDate"].ToString();
                    projectVM.ManpowerRequired = Convert.ToInt32(dr["ManpowerRequired"]);
                    projectVM.ContactPerson = dr["ContactPerson"].ToString();
                    projectVM.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    projectVM.Address = dr["Address"].ToString();
                    projectVM.District = dr["District"].ToString();
                    projectVM.Division = dr["Division"].ToString();
                    projectVM.Country = dr["Country"].ToString();
                    projectVM.City = dr["City"].ToString();
                    projectVM.PostalCode = dr["PostalCode"].ToString();
                    projectVM.Phone = dr["Phone"].ToString();
                    projectVM.Mobile = dr["Mobile"].ToString();
                    projectVM.Fax = dr["Fax"].ToString();

                    projectVM.Remarks = dr["Remarks"].ToString();
                    projectVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    projectVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    projectVM.CreatedBy = dr["CreatedBy"].ToString();
                    projectVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    projectVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    projectVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    projectVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return projectVM;
        }
        //==================Insert =================
        public string[] Insert(ProjectVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertProject"; //Method Name
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

                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Project";	
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
				
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                       
                    }
                    else
                    {
                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Project where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);

                        vm.Id = vm.BranchId.ToString() + "_" + (count + 1);

                        if (1 == 1)
                        {

                            sqlText = "  ";
                            sqlText += @" INSERT INTO Project(Id,BranchId,Code,Name,Startdate,EndDate,ManpowerRequired,ContactPerson
                                ,ContactPersonDesignation,Address,District,Division,Country,City,PostalCode,Phone,Mobile,Fax
                                ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                                                VALUES (@Id,@BranchId,@Code,@Name,@Startdate,@EndDate,@ManpowerRequired,@ContactPerson
                                ,@ContactPersonDesignation,@Address,@District,@Division,@Country,@City,@PostalCode,@Phone,@Mobile,@Fax
                                ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                            cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                            cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                            cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                            cmdInsert.Parameters.AddWithValue("@Startdate", vm.Startdate ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@EndDate", vm.EndDate ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ManpowerRequired", vm.ManpowerRequired);
                            cmdInsert.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
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
                            retResults[1] = "This Project already used";
                            throw new ArgumentNullException("Please Input Project Value", "");
                        }

                        #endregion Save
                    }
                }
                #endregion Exist			               
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
        public string[] Update(ProjectVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Project Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToProject"); }

                #endregion open connection and transaction
                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Project";				
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
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Project ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", ProjectVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", ProjectVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", ProjectVM.Name.Trim());

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
                    sqlText = "  update Project set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";

                    sqlText += " Startdate=@Startdate,";
                    sqlText += " EndDate=@EndDate,";
                    sqlText += " ManpowerRequired=@ManpowerRequired,";
                    sqlText += " ContactPerson=@ContactPerson,";
                    sqlText += " ContactPersonDesignation=@ContactPersonDesignation,";
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
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Startdate", vm.Startdate ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@EndDate", vm.EndDate ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ManpowerRequired", vm.ManpowerRequired );
                    cmdUpdate.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);
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
                        // throw new ArgumentNullException("Education Update", ProjectVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Project Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Project.";
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
        public ProjectVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            ProjectVM projectVM = new ProjectVM();

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
,Code
,Name

,Startdate
,EndDate
,ManpowerRequired
,ContactPerson
,ContactPersonDesignation
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
    From Project 
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
                        projectVM.Id = dr["Id"].ToString();
                        projectVM.Code = dr["Code"].ToString();
                        projectVM.Name = dr["Name"].ToString();

                        projectVM.Startdate = dr["Startdate"].ToString();
                        projectVM.EndDate = dr["EndDate"].ToString();
                        projectVM.ManpowerRequired = Convert.ToInt32(dr["ManpowerRequired"]);
                        projectVM.ContactPerson = dr["ContactPerson"].ToString();
                        projectVM.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                        projectVM.Address = dr["Address"].ToString();
                        projectVM.District = dr["District"].ToString();
                        projectVM.Division = dr["Division"].ToString();
                        projectVM.Country = dr["Country"].ToString();
                        projectVM.City = dr["City"].ToString();
                        projectVM.PostalCode = dr["PostalCode"].ToString();
                        projectVM.Phone = dr["Phone"].ToString();
                        projectVM.Mobile = dr["Mobile"].ToString();
                        projectVM.Fax = dr["Fax"].ToString();

                        projectVM.Remarks = dr["Remarks"].ToString();
                        projectVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        projectVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        projectVM.CreatedBy = dr["CreatedBy"].ToString();
                        projectVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        projectVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        projectVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        projectVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return projectVM;
        }
        //==================Delete =================
        public string[] Delete(ProjectVM projectVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteProject"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToProject"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>=1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Project set";
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
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", projectVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", projectVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", projectVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Project Delete", projectVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Project Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Project Information.";
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
        public List<ProjectVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<ProjectVM> VMs = new List<ProjectVM>();
            ProjectVM vm;
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
   FROM Project
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
                    vm = new ProjectVM();
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
        public List<DropDownVM> DropDownByDepartment(string departmentId, string sectionId)
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
                select Id, Name from Project
                WHERE   1=1";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;              
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DropDownVM();
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
        public List<DropDownVM> DropDownByDepartment(string departmentId)
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
select distinct  ProjectId Id,Project Name from ViewEmployeeInformation
WHERE   DepartmentId=@DepartmentId
ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@DepartmentId", departmentId);
         

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DropDownVM();
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

        #endregion


        public string[] InsertExportData(ProjectVM paramVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                ProjectVM vProjectVM = new ProjectVM();

                #region Assign Data
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    vProjectVM.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vProjectVM.Code = dr["Code"].ToString();
                    vProjectVM.Name = dr["Name"].ToString();
                    vProjectVM.Startdate = dr["Startdate"].ToString();
                    vProjectVM.EndDate = dr["EndDate"].ToString();
                    vProjectVM.ManpowerRequired =Convert.ToInt32( dr["ManpowerRequired"].ToString());
                    vProjectVM.ContactPerson = dr["ContactPerson"].ToString();
                    vProjectVM.ContactPersonDesignation = dr["ContactPersonDesignation"].ToString();
                    vProjectVM.Address = dr["Address"].ToString();
                    vProjectVM.Division = dr["Division"].ToString();
                    vProjectVM.District = dr["District"].ToString();
                    vProjectVM.Country = dr["Country"].ToString();
                    vProjectVM.City = dr["City"].ToString();
                    vProjectVM.PostalCode = dr["PostalCode"].ToString();
                    vProjectVM.Phone = dr["Phone"].ToString();
                    vProjectVM.Mobile = dr["Mobile"].ToString();
                    vProjectVM.Fax = dr["Fax"].ToString();
                    vProjectVM.Remarks = dr["Remarks"].ToString();
                    vProjectVM.CreatedAt = paramVM.CreatedAt;
                    vProjectVM.CreatedBy = paramVM.CreatedBy;
                    vProjectVM.CreatedFrom = paramVM.CreatedFrom;
                    retResults = Insert(vProjectVM, currConn, transaction);
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
