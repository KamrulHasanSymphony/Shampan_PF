using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SymServices.WPPF
{
    public class SettlementPolicyDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of Settlement Policy from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Settlement Policy.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="SettlementPolicyVM"/> representing the Settlement Policy matching the criteria.</returns>
        public List<SettlementPolicyVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
             , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SettlementPolicyVM> VMs = new List<SettlementPolicyVM>();
            SettlementPolicyVM vm;
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
                sqlText = @"
SELECT
Id
,PolicyName
,JobAgeInMonth
,isnull(EmployeeContributionRatio,0)EmployeeContributionRatio
,isnull(EmployerContributionRatio,0)EmployerContributionRatio
,isnull(EmployeeProfitRatio      ,0)EmployeeProfitRatio
,isnull(EmployerProfitRatio      ,0)EmployerProfitRatio
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

from SettlementPolicies
WHERE  1=1
";

                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SettlementPolicyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PolicyName = dr["PolicyName"].ToString();
                    vm.JobAgeInMonth = Convert.ToDecimal(dr["JobAgeInMonth"]);
                    vm.EmployeeContributionRatio = Convert.ToDecimal(dr["EmployeeContributionRatio"]);
                    vm.EmployerContributionRatio = Convert.ToDecimal(dr["EmployerContributionRatio"]);

                    vm.EmployeeProfitRatio = Convert.ToDecimal(dr["EmployeeProfitRatio"]);
                    vm.EmployerProfitRatio = Convert.ToDecimal(dr["EmployerProfitRatio"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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
        //==================Insert =================
        /// <summary>
        /// Inserts a new ettlement Policy record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="SettlementPolicyVM"/> containing the ettlement Policy data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertBankBranch").
        /// </returns>
        public string[] Insert(SettlementPolicyVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSettlementPolicy"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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


                vm.Id = _cDal.NextId("SettlementPolicies", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO SettlementPolicies(
Id
,PolicyName
,JobAgeInMonth
,EmployeeContributionRatio
,EmployerContributionRatio

,EmployeeProfitRatio
,EmployerProfitRatio

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@PolicyName
,@JobAgeInMonth
,@EmployeeContributionRatio
,@EmployerContributionRatio

,@EmployeeProfitRatio
,@EmployerProfitRatio

,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@PolicyName", vm.PolicyName);
                    cmdInsert.Parameters.AddWithValue("@JobAgeInMonth", vm.JobAgeInMonth);
                    cmdInsert.Parameters.AddWithValue("@EmployeeContributionRatio", vm.EmployeeContributionRatio);
                    cmdInsert.Parameters.AddWithValue("@EmployerContributionRatio", vm.EmployerContributionRatio);

                    cmdInsert.Parameters.AddWithValue("@EmployeeProfitRatio", vm.EmployeeProfitRatio);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfitRatio", vm.EmployerProfitRatio);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update SettlementPolicy.", "");
                    }
                }
                else
                {
                    retResults[1] = "This SettlementPolicy already used!";
                    throw new ArgumentNullException("Please Input SettlementPolicy Value", "");
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
        /// <summary>
        /// Updates an existing Settlement Policy record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="SettlementPolicyVM"/> containing the updated Settlement Policy information.</param>
        /// <param name="VcurrConn">Optional external SQL connection. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">Optional external SQL transaction. If null, a new transaction will be created and committed.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Updated record ID,  
        /// [3] = The executed SQL query,  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("BankBranchUpdate").
        /// </returns>
        public string[] Update(SettlementPolicyVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee SettlementPolicy Update"; //Method Name
            int transResult = 0;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSettlementPolicy"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE SettlementPolicies SET";
                    sqlText += " PolicyName=@PolicyName";
                    sqlText += " ,JobAgeInMonth=@JobAgeInMonth";
                    sqlText += " ,EmployeeContributionRatio=@EmployeeContributionRatio";
                    sqlText += " ,EmployerContributionRatio=@EmployerContributionRatio";

                    sqlText += " ,EmployeeProfitRatio=@EmployeeProfitRatio";
                    sqlText += " ,EmployerProfitRatio=@EmployerProfitRatio";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@PolicyName", vm.PolicyName);
                    cmdUpdate.Parameters.AddWithValue("@JobAgeInMonth", vm.JobAgeInMonth);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeContributionRatio", vm.EmployeeContributionRatio);
                    cmdUpdate.Parameters.AddWithValue("@EmployerContributionRatio", vm.EmployerContributionRatio);

                    cmdUpdate.Parameters.AddWithValue("@EmployeeProfitRatio", vm.EmployeeProfitRatio);
                    cmdUpdate.Parameters.AddWithValue("@EmployerProfitRatio", vm.EmployerProfitRatio);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update SettlementPolicy.", "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("SettlementPolicy Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("SettlementPolicy Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }
        ////==================Delete =================
        ////// <summary>
        /// Performs a soft delete (archives) of one or more Settlement Policy records based on the provided IDs.
        /// Sets <c>IsActive</c> to false and <c>IsArchive</c> to true without removing data from the database.
        /// Allows optional use of an external SQL connection and transaction for transactional consistency.
        /// </summary>
        /// <param name="vm">The <see cref="SettlementPolicyVM"/> containing metadata such as user and timestamp for the update.</param>
        /// <param name="ids">An array of string IDs identifying the records to be soft-deleted.</param>
        /// <param name="VcurrConn">Optional SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">Optional SQL transaction. If null, a new transaction is started and committed.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Unused or empty string for return ID,  
        /// [3] = The last executed SQL query,  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("DeleteBankBranch").
        /// </returns>
        public string[] Delete(SettlementPolicyVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSettlementPolicy"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string retVal = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSettlementPolicy"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = "DELETE SettlementPolicies";
                        sqlText += " WHERE Id=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update SettlementPolicies.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("SettlementPolicy Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("SettlementPolicy Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return retResults;
        }
        #endregion
    }
}
