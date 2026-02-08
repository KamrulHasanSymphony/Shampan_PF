using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.BWWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SymServices.BWWF
{
    public class PFBankDepositDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        //==================SelectAll=================

        /// <summary>
        /// Retrieves a list of Bank Deposit from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Bank Deposit.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="BankBranchVM"/> representing the Bank Deposit matching the criteria.</returns>
        public List<PFBankDepositVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFBankDepositVM> VMs = new List<PFBankDepositVM>();
            PFBankDepositVM vm;
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
pfbd.Id
,pfbd.Code
,ISNULL(pfbd.FiscalYearDetailId,0) FiscalYearDetailId
,ISNULL(pfbd.TotalEmployeePFValue,0) TotalEmployeePFValue
,ISNULL(pfbd.TotalEmployeerPFValue,0) TotalEmployeerPFValue
,ISNULL(pfbd.DepositAmount,0) DepositAmount
,pfbd.DepositDate
,ISNULL(pfbd.BankBranchId,0) BankBranchId

,b.Name+'~'+bb.BranchName as BankBranchName

,pfbd.TransactionType

,pfbd.ReferenceNo
,pfbd.TransactionMediaId
,tm.Name TransactionMedia

,ISNULL(pfbd.ReferenceId,0)ReferenceId
,ISNULL(pfbd.Post,0)Post

,pfbd.Remarks
,pfbd.IsActive
,pfbd.IsArchive
,pfbd.CreatedBy
,pfbd.CreatedAt
,pfbd.CreatedFrom
,pfbd.LastUpdateBy
,pfbd.LastUpdateAt
,pfbd.LastUpdateFrom
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
FROM PFBankDeposits pfbd
LEFT OUTER JOIN BankBranchs bb  ON pfbd.BankBranchId = bb.Id
LEFT OUTER JOIN BankNames b ON bb.BankId = b.Id
LEFT OUTER JOIN TransactionMedias tm ON pfbd.TransactionMediaId = tm.Id
";
                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfbd.FiscalYearDetailId=fyd.Id";
                sqlText += @" WHERE  1=1  AND pfbd.IsArchive = 0
";
                if (Id > 0)
                {
                    sqlText += @" AND pfbd.Id=@Id";
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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFBankDepositVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();

                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.TotalEmployeePFValue = Convert.ToDecimal(dr["TotalEmployeePFValue"]);
                    vm.TotalEmployeerPFValue = Convert.ToDecimal(dr["TotalEmployeerPFValue"]);
                    vm.DepositAmount = Convert.ToDecimal(dr["DepositAmount"]);
                    vm.DepositDate = Ordinary.StringToDate(dr["DepositDate"].ToString());
                    vm.BankBranchId = Convert.ToInt32(dr["BankBranchId"]);
                    vm.BankBranchName = dr["BankBranchName"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();

                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.TransactionMediaId = Convert.ToInt32(dr["TransactionMediaId"]);
                    vm.TransactionMedia = dr["TransactionMedia"].ToString();


                    vm.ReferenceId = Convert.ToInt32(dr["ReferenceId"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();


                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
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

        //==================PreInsert=================

        //==================Insert =================
        /// <summary>
        /// Inserts a new Bank Deposit record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="PFBankDepositVM"/> containing the Bank Deposit data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertBankDeposit").
        /// </returns>
        public string[] Insert(PFBankDepositVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertPFBankDeposit"; //Method Name
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


                vm.Id = _cDal.NextId("PFBankDeposits", currConn, transaction);
                if (vm != null)
                {
                    string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "BankDeposit", vm.DepositDate, currConn, transaction);
                    
                    vm.Code = NewCode;
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO PFBankDeposits(
Id
,Code
,FiscalYearDetailId
,DepositAmount
,TotalEmployeePFValue
,TotalEmployeerPFValue
,DepositDate
,BankBranchId
,TransactionType

,ReferenceNo
,TransactionMediaId

,ReferenceId
,TransType

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@Code
,@FiscalYearDetailId
,@DepositAmount
,@TotalEmployeePFValue
,@TotalEmployeerPFValue
,@DepositDate
,@BankBranchId
,@TransactionType

,@ReferenceNo
,@TransactionMediaId

,@ReferenceId
,@TransType

,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion SqlText

                    #region SqlExecution
                    //FiscalYearDAL _fYearDal = new FiscalYearDAL();
                    //vm.FiscalYearDetailId = _fYearDal.PeriodLockByTransactionDate(Ordinary.DateToString(vm.DepositDate), currConn, transaction).Id;

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);

                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@DepositAmount", vm.DepositAmount);
                    cmdInsert.Parameters.AddWithValue("@TotalEmployeePFValue", vm.TotalEmployeePFValue);
                    cmdInsert.Parameters.AddWithValue("@TotalEmployeerPFValue", vm.TotalEmployeerPFValue);
                    cmdInsert.Parameters.AddWithValue("@DepositDate", Ordinary.DateToString(vm.DepositDate));
                    cmdInsert.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TransactionMediaId", vm.TransactionMediaId);

                    cmdInsert.Parameters.AddWithValue("@ReferenceId", vm.ReferenceId);

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
	                cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "BWWF");

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update PFBankDeposits.", "");
                    }
                    #endregion SqlExecution

                }
                else
                {
                    retResults[1] = "This PFBankDeposit already used!";
                    throw new ArgumentNullException("Please Input PFBankDeposit Value", "");
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
        /// Updates an existing Bank Deposit record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="PFBankDepositVM"/> containing the updated Bank Deposit information.</param>
        /// <param name="VcurrConn">Optional external SQL connection. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">Optional external SQL transaction. If null, a new transaction will be created and committed.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Updated record ID,  
        /// [3] = The executed SQL query,  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("BankBankDeposit").
        /// </returns>
        public string[] Update(PFBankDepositVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFBankDepositUpdate"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE PFBankDeposits SET";

                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , DepositAmount=@DepositAmount";
                    sqlText += " , TotalEmployeePFValue=@TotalEmployeePFValue";
                    sqlText += " , TotalEmployeerPFValue=@TotalEmployeerPFValue";
                    sqlText += " , DepositDate=@DepositDate";
                    sqlText += " , BankBranchId=@BankBranchId";

                    sqlText += " , ReferenceNo=@ReferenceNo";
                    sqlText += " , TransactionMediaId=@TransactionMediaId";



                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , TransType=@TransType";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText

                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@DepositAmount", vm.DepositAmount);
                    cmdUpdate.Parameters.AddWithValue("@TotalEmployeePFValue", vm.TotalEmployeePFValue);
                    cmdUpdate.Parameters.AddWithValue("@TotalEmployeerPFValue", vm.TotalEmployeerPFValue);
                    cmdUpdate.Parameters.AddWithValue("@DepositDate", Ordinary.DateToString(vm.DepositDate));
                    cmdUpdate.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);

                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TransactionMediaId", vm.TransactionMediaId);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
	                cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "BWWF");

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update PFBankDeposits.", "");
                    }
                    #endregion SqlExecution

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("PFBankDeposit Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PFBankDeposit Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
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
        /// /// <summary>
        /// Performs a soft delete (archives) of one or more Bank Deposit records based on the provided IDs.
        /// Sets <c>IsActive</c> to false and <c>IsArchive</c> to true without removing data from the database.
        /// Allows optional use of an external SQL connection and transaction for transactional consistency.
        /// </summary>
        /// <param name="vm">The <see cref="PFBankDepositVM"/> containing metadata such as user and timestamp for the update.</param>
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
        public string[] Delete(PFBankDepositVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePFBankDeposit"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        PFBankDepositVM varPFBankDepositVM = new PFBankDepositVM();
                        varPFBankDepositVM = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();
                        ////retVal = _cDal.SelectFieldValue("PFBankDeposits", "Post", "Id", ids[i].ToString(), currConn, transaction);
                        ////vm.Post = Convert.ToBoolean(retVal);
                        if (varPFBankDepositVM.Post)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                        if (varPFBankDepositVM.TransactionType != "Other")
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Reference Data Can't Be Deleted!";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion Check Posted or Not Posted

                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = "DELETE PFBankDeposits WHERE Id=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update ForfeitureAccounts.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("ForfeitureAcount Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("PFBankDeposit Information Delete", "Could not found any item.");
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

        ////==================Post =================
        public string[] Post(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PostPFBankDeposit"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = " ";
            int transResult = 0;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = "UPDATE PFBankDeposits SET POST=1 WHERE Id=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);


                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PFBankDeposit Information Post - Could not found any item.", "");
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Posted Successfully.";
                #endregion

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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



        ////==================Report=================
        public DataTable Report(PFBankDepositVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                sqlText = @"
SELECT
ISNULL(pfbd.FiscalYearDetailId,0) FiscalYearDetailId
,ISNULL(pfbd.TotalEmployeePFValue,0) TotalEmployeePFValue
,ISNULL(pfbd.TotalEmployeerPFValue,0) TotalEmployeerPFValue
,ISNULL(dd.DebitAmount,0) DepositAmount
,pfbd.DepositDate
,a.Name  as BankName
,pfbd.Remarks
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
FROM PFBankDepositDetails dd
LEFT OUTER JOIN PFBankDeposits pfbd  ON dd.PFBankDepositId = pfbd.Id
LEFT OUTER JOIN Accounts a  ON a.Id = dd.AccountId
";
                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfbd.FiscalYearDetailId=fyd.Id";
                sqlText += @" WHERE  1=1  AND pfbd.IsArchive = 0
";
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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "DepositDate");

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
            return dt;
        }


        ////==================Report=================
        public DataTable PFBankStatementReport(PFBankDepositVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                sqlText = @"
--declare @DateStart as varchar(20)
--declare @DateEnd as varchar(20)
--declare @BankBranchId as int

declare @Opening as decimal(18,2)

set @Opening=0

SELECT
@Opening=@Opening+DepositAmount
FROM PFBankDeposits
WHERE  1=1  AND IsArchive = 0
 AND DepositDate<@DateStart 
 AND BankBranchId=@BankBranchId

 SELECT
@Opening=@Opening-WithdrawAmount
FROM Withdraws
WHERE  1=1  AND IsArchive = 0
 AND WithdrawDate<@DateStart 
 AND BankBranchId=@BankBranchId

 SELECT
@Opening=@Opening+TotalInterestValue
FROM ReturnOnBankInterests
WHERE  1=1  AND IsArchive = 0
 AND ROBIDate<@DateStart 
 AND BankBranchId=@BankBranchId




SELECT a.*, ISNULL(b.Name+'~'+bb.BranchName, 'NA') as BankBranchName FROM(

SELECT
'B' SL
,DepositDate [Date]
,DepositAmount Amount 
,BankBranchId
,Remarks
,'Deposit' TransactionType

FROM PFBankDeposits
WHERE  1=1  AND IsArchive = 0
 AND DepositDate>=@DateStart AND DepositDate<=@DateEnd
 AND BankBranchId=@BankBranchId
Union All

SELECT
'C'
,WithdrawDate
,WithdrawAmount
,BankBranchId
,Remarks
,'Withdraw'

FROM Withdraws
WHERE  1=1  AND IsArchive = 0
 AND WithdrawDate>=@DateStart AND WithdrawDate<=@DateEnd
 AND BankBranchId=@BankBranchId


union all


SELECT
'D'
,ROBIDate
,TotalInterestValue
,BankBranchId
,Remarks
,'Bank Interest Deposit'

FROM ReturnOnBankInterests
WHERE  1=1  AND IsArchive = 0
 AND ROBIDate>=@DateStart AND ROBIDate<=@DateEnd
 AND BankBranchId=@BankBranchId


union all
 select  'A' SL,@DateStart,@Opening,@BankBranchId,'Opening','Opening'

) as a

LEFT OUTER JOIN BankBranchs bb  ON a.BankBranchId = bb.Id
LEFT OUTER JOIN BankNames b ON bb.BankId = b.Id

 order by [Date], SL
";



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@DateStart", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DateEnd", Ordinary.DateToString(vm.DateTo));
                da.SelectCommand.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "Date");

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
            return dt;
        }

        #endregion Methods
    }
}
