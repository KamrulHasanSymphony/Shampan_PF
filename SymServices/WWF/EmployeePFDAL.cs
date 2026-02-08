using Excel;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.WWF
{
    public class EmployeePFDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        /// <summary>
        /// Retrieves all active, non-archived EmployeePF records from the database,
        /// mapping each record to an EmployeePFVM object and returning a list of these objects.
        /// </summary>
        /// <returns>
        /// A list of EmployeePFVM objects representing the EmployeePF records.
        /// </returns>
        public List<EmployeePFVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePFVM> EmployeePFVMs = new List<EmployeePFVM>();
            EmployeePFVM EmployeePFVM;
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
,EmployeeId
,PFStructureId
,isnull(PFValue  ,0)PFValue
,IsFixed
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePF
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
                    EmployeePFVM = new EmployeePFVM();
                    EmployeePFVM.Id = Convert.ToInt32(dr["Id"]);
                    EmployeePFVM.EmployeeId = dr["EmployeeId"].ToString();
                    EmployeePFVM.PFStructureId = dr["PFStructureId"].ToString();
                    EmployeePFVM.PFValue = Convert.ToDecimal(dr["PFValue"]);
                    EmployeePFVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    EmployeePFVM.Remarks = dr["Remarks"].ToString();
                    EmployeePFVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    EmployeePFVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    EmployeePFVM.CreatedBy = dr["CreatedBy"].ToString();
                    EmployeePFVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    EmployeePFVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    EmployeePFVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    EmployeePFVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    EmployeePFVMs.Add(EmployeePFVM);
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

            return EmployeePFVMs;
        }
        //==================SelectByID=================
        /// <summary>
        /// Retrieves an EmployeePFVM object from the database based on the specified Id.
        /// </summary>
        /// <param name="Id">The identifier of the EmployeePF record to retrieve.</param>
        /// <returns>Returns the EmployeePFVM object populated with data from the database if found; otherwise, returns an empty EmployeePFVM object.</returns>
        public EmployeePFVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFVM EmployeePFVM = new EmployeePFVM();

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
,PFStructureId
,EmployeeId
,isnull(PFValue  ,0)PFValue
,IsFixed
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeePF
where  id=@Id
     
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
                    EmployeePFVM.Id = Convert.ToInt32(dr["Id"]);
                    EmployeePFVM.PFStructureId = dr["PFStructureId"].ToString();
                    EmployeePFVM.EmployeeId = dr["EmployeeId"].ToString();
                    EmployeePFVM.PFValue = Convert.ToDecimal(dr["PFValue"]);
                    EmployeePFVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    EmployeePFVM.Remarks = dr["Remarks"].ToString();
                    EmployeePFVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    EmployeePFVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    EmployeePFVM.CreatedBy = dr["CreatedBy"].ToString();
                    EmployeePFVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    EmployeePFVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    EmployeePFVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    EmployeePFVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return EmployeePFVM;
        }
        //==================Insert =================
        public string[] Insert(EmployeePFVM EmployeePFVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeePF"; //Method Name


            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(EmployeePFVM.EmployeePFId))
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
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeePF ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeePFVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", EmployeePFVM.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                #endregion Exist
                #region Exist
                //                sqlText = "  ";
                //                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM EmployeePF ";
                //                sqlText += " WHERE Code=@Code  ";
                //                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //                cmdExist.Transaction = transaction;
                //var exeRes = cmdExist.ExecuteScalar();
                //int objfoundId = Convert.ToInt32(exeRes);

                //                if (objfoundId > 0)
                //                {
                //                    retResults[1] = "Code already used!";
                //                    retResults[3] = sqlText;
                //                    throw new ArgumentNullException("Code already used!", "");
                //                }
                #endregion Exist

                #region Save
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeePF  ";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);

                EmployeePFVM.Id = (count + 1);
                //int foundId = (int)objfoundId;
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePF(Id,PFStructureId,EmployeeId,PFValue ,IsFixed,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@PFStructureId,@EmployeeId,@PFValue ,@IsFixed,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", EmployeePFVM.Id);
                    cmdExist1.Parameters.AddWithValue("@PFStructureId", EmployeePFVM.PFStructureId);
                    cmdExist1.Parameters.AddWithValue("@PFValue", EmployeePFVM.PFValue);
                    cmdExist1.Parameters.AddWithValue("@IsFixed", EmployeePFVM.IsFixed);
                    cmdExist1.Parameters.AddWithValue("@EmployeeId", EmployeePFVM.EmployeeId);
                    cmdExist1.Parameters.AddWithValue("@Remarks", EmployeePFVM.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", EmployeePFVM.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", EmployeePFVM.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", EmployeePFVM.CreatedFrom);

                    cmdExist1.Transaction = transaction;
                    cmdExist1.ExecuteNonQuery();


                }
                else
                {
                    retResults[1] = "This EmployeePF already used";
                    throw new ArgumentNullException("Please Input EmployeePF Value", "");
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
                retResults[2] = EmployeePFVM.Id.ToString();

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
        /// <summary>
        /// Updates an existing EmployeePF record in the database using the provided EmployeePF view model.
        /// Handles database connection and transaction management, including reusing external connection and transaction if provided.
        /// Checks for duplicate Code before proceeding with update.
        /// Commits the transaction if successful, or rolls back on error.
        /// </summary>
        /// <param name="EmployeePFVM">The EmployeePF view model containing updated data.</param>
        /// <param name="VcurrConn">Optional external SqlConnection to use; if null, a new connection is created.</param>
        /// <param name="Vtransaction">Optional external SqlTransaction to use; if null, a new transaction is started.</param>
        /// <returns>
        /// An array of strings containing:
        /// [0] - "Success" or "Fail" indicating operation result,
        /// [1] - Message describing the result or error,
        /// [2] - The Id of the updated EmployeePF record as string,
        /// [3] - The SQL query executed,
        /// [4] - Exception message if failed,
        /// [5] - Method identifier string ("Employee EmployeePF Update").
        /// </returns>
        public string[] Update(EmployeePFVM EmployeePFVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeePF Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePF"); }

                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) Code FROM EmployeePF ";
                sqlText += " WHERE Code=@Code AND Id<>@Id  ";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", EmployeePFVM.Id);
                var exeRes = cmdExist.ExecuteScalar();
                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    retResults[1] = "Code already used!";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Code already used!", "");
                }

                #endregion Exist

                if (EmployeePFVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeePF set";
                    sqlText += " PFStructureId=@PFStructureId,";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " PFValue=@PFValue,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeePFVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@PFStructureId", EmployeePFVM.PFStructureId);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeePFVM.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@PFValue", EmployeePFVM.PFValue);
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", EmployeePFVM.IsFixed);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", EmployeePFVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePFVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePFVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePFVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = EmployeePFVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query



                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeePF Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update EmployeePF.";
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
        public string[] Delete(EmployeePFVM EmployeePFVM, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeePF"; //Method Name

            int transResult = 0;
            int countId = 0;
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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeePF"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeePF set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeePFVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeePFVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeePFVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeePF Delete", EmployeePFVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeePF Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeePF Information.";
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
        public List<EmployeePFVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePFVM> VMs = new List<EmployeePFVM>();
            EmployeePFVM vm;
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
   FROM EmployeePF
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
                    vm = new EmployeePFVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
                sqlText = @"SELECT Id, Name    FROM EmployeePF ";
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



        ////==================ExportExcelFile =================
        /// <summary>
        /// Retrieves and returns a DataTable containing employee provident fund (WWF) details,
        /// including employee information and WWF structure data, for active and non-archived employees.
        /// Optionally supports filtering with condition fields and values (currently commented out).
        /// </summary>
        /// <param name="vm">An EmployeePFVM object representing the employee WWF view model (not used in current implementation).</param>
        /// <param name="conFields">Optional array of condition field names for filtering the query (currently not applied).</param>
        /// <param name="conValues">Optional array of condition values corresponding to the fields for filtering the query (currently not applied).</param>
        /// <returns>
        /// A DataTable containing employee WWF information merged from EmployeePF and ViewEmployeeInformation tables,
        /// with additional joined data from PFStructure and an added "Type" column set as "Employee WWF".
        /// Throws an ArgumentNullException if no employee WWF records are found.
        /// </returns>
        public DataTable ExportExcelFile(EmployeePFVM vm, string[] conFields = null, string[] conValues = null)
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @"
SELECT * FROM (
SELECT 

emab.Code
,emab.EmpName
,ISNULL( etax.PFValue,0)  PFValue

,emab.Project
,emab.Department
,emab.Section
,emab.Designation

,ISNULL(tx.Name, 'NA') PFName
,ISNULL(etax.PortionSalaryType, 'NA') PortionSalaryType
,ISNULL(etax.IsFixed, '0') IsFixed


,emab.EmployeeId
,emab.ProjectId
,emab.DepartmentId
,emab.SectionId
,emab.DesignationId
,ISNULL(etax.PFStructureId, '1_1') PFStructureId

FROM EmployeePF etax
LEFT OUTER JOIN ViewEmployeeInformation emab ON emab.EmployeeId =ISNULL(etax.EmployeeId,0)
LEFT OUTER JOIN PFStructure tx ON etax.PFStructureId = tx.Id
WHERE emab.IsArchive=0 and emab.IsActive=1

UNION ALL

SELECT 
Code
,EmpName
,'0' 


,Project
,Department
,Section
,Designation

,'NA'
,'NA'
,'0'
,EmployeeId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,'0'

FROM ViewEmployeeInformation
WHERE EmployeeId NOT IN(
SELECT DISTINCT EmployeeId FROM EmployeePF
) 
)as a
WHERE 1=1  and a.EmployeeId <> '1_0' 
";
                //string cField = "";
                //if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                //{
                //    for (int i = 0; i < conFields.Length; i++)
                //    {
                //        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                //        {
                //            continue;
                //        }
                //        cField = conFields[i].ToString();
                //        cField = Ordinary.StringReplacing(cField);
                //        sqlText += " AND " + conFields[i] + "=@" + cField;
                //    }
                //}
                sqlText += " Order By a.Code, a.DepartmentId, a.ProjectId, a.SectionId, a.EmployeeId";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                //if (conFields != null && conValues != null && conFields.Length == conValues.Length)
                //{
                //    for (int j = 0; j < conFields.Length; j++)
                //    {
                //        if (string.IsNullOrWhiteSpace(conFields[j]) || string.IsNullOrWhiteSpace(conValues[j]))
                //        {
                //            continue;
                //        }
                //        cField = conFields[j].ToString();
                //        cField = Ordinary.StringReplacing(cField);
                //        da.SelectCommand.Parameters.AddWithValue("@" + cField, conValues[j]);
                //    }
                //}
                da.Fill(dt);
                #endregion
                dt.Columns.Add("Type");
                if (dt.Rows.Count == 0)
                {
                    throw new ArgumentNullException("Employee WWF has not given to any employee!");
                }
                foreach (DataRow row in dt.Rows)
                {
                    row["Type"] = "Employee WWF";
                }
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }

        ////==================ImportExcelFile =================
        /// <summary>
        /// Imports employee provident fund (WWF) data from an Excel file and saves it to the database.
        /// Reads the Excel file, maps each row to EmployeePFVM objects, deletes existing records by EmployeeId,
        /// inserts new records, and manages database connection and transaction.
        /// </summary>
        /// <param name="fullPath">The full file path of the Excel file to import.</param>
        /// <param name="fileName">The name of the Excel file (used to determine format).</param>
        /// <param name="auditvm">Audit information to set created metadata on the imported records.</param>
        /// <param name="VcurrConn">Optional existing SQL connection to use. If not provided, a new connection is created.</param>
        /// <param name="Vtransaction">Optional existing SQL transaction to use. If not provided, a new transaction is created.</param>
        /// <returns>
        /// A string array with the following indices:
        /// [0] - Status ("Success" or "Fail")
        /// [1] - Message describing the result or error
        /// [2] - Id (currently returns "0" as no insert ID is captured)
        /// [3] - Executed SQL query (if any)
        /// [4] - Exception message in case of failure
        /// [5] - Method name ("Employee WWF")
        /// </returns>
        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Employee WWF"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region try
            try
            {
                #region Reading Excel
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                FileStream stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];
                reader.Close();
                #endregion Reading Excel
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
                List<EmployeePFVM> VMs = new List<EmployeePFVM>();
                EmployeePFVM vm = new EmployeePFVM();

                foreach (DataRow item in dt.Rows)
                {
                    #region CheckPoint
                    #endregion CheckPoint
                    #region Read Data
                    vm = new EmployeePFVM();
                    vm.EmployeeId = item["EmployeeId"].ToString();
                    vm.Code = item["Code"].ToString();

                    vm.PFStructureId = item["PFStructureId"].ToString();
                    vm.PFValue = Convert.ToDecimal(item["PFValue"]);
                    vm.IsFixed = Convert.ToBoolean(item["IsFixed"]);
                    vm.PortionSalaryType = item["PortionSalaryType"].ToString();

                    vm.ProjectId = item["ProjectId"].ToString();
                    vm.DepartmentId = item["DepartmentId"].ToString();
                    vm.SectionId = item["SectionId"].ToString();
                    vm.DesignationId = item["DesignationId"].ToString();


                    vm.CreatedAt = auditvm.CreatedAt;
                    vm.CreatedBy = auditvm.CreatedBy;
                    vm.CreatedFrom = auditvm.CreatedFrom;
                    VMs.Add(vm);
                    #endregion Read Data
                }
                #region Insert Data
                CommonDAL _cDal = new CommonDAL();


                foreach (var item in VMs)
                {
                    retResults = _cDal.DeleteTableByCondition("EmployeePF", "EmployeeId", item.EmployeeId, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    retResults = Insert(item, currConn, transaction);

                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }
                }

                #endregion Insert Data
                #region Commit
                if (Vtransaction == null && transaction != null)
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
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction == null) { transaction.Rollback(); }
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



        #endregion

    }
}
