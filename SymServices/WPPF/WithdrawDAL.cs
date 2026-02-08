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
    public class WithdrawDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<WithdrawVM> DropDownX(string tType = "", int branchId = 0, string TransType="WPPF")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<WithdrawVM> VMs = new List<WithdrawVM>();
            WithdrawVM vm;
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
   FROM Withdraws
WHERE  1=1 AND IsArchive = 0 and TransType=@TransType
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@TransType", TransType);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new WithdrawVM();
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

        public WithdrawVM SelectAvailableBalanceX(WithdrawVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
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
--------declare @BankBranchId as int
--------set @BankBranchId =1
--------
--------declare @WithdrawDate as nvarchar(14)
--------set @WithdrawDate ='20191002'

------------------------------------------------------------
----------------------Balance-------------------------------
;WITH

Balance AS
(
SELECT ISNULL(SUM(DepositAmount),0) TotalDepositAmount, ISNULL(SUM(WithdrawAmount),0) TotalWithdrawAmount, BankBranchId FROM
(
SELECT ISNULL(DepositAmount,0) DepositAmount, 0 WithdrawAmount, BankBranchId FROM PFBankDeposits
WHERE 1=1
AND DepositDate<@WithdrawDate
AND Post = 1
AND BankBranchId=@BankBranchId

UNION ALL

SELECT 0, ISNULL(WithdrawAmount,0), BankBranchId FROM Withdraws
WHERE 1=1
AND WithdrawDate<@WithdrawDate
AND Post = 1
AND BankBranchId=@BankBranchId
) as a
Group BY a.BankBranchId
)


------------------------------------------------------------
------------------------------------------------------------

SELECT  
@WithdrawDate WithdrawDate
, ISNULL(bal.TotalDepositAmount,0) TotalDepositAmount
, ISNULL(bal.TotalWithdrawAmount,0)TotalWithdrawAmount
, ISNULL(( bal.TotalDepositAmount-bal.TotalWithdrawAmount),0) AvailableBalance
, bal.BankBranchId, bn.Name BankName,  bb.BranchName, bb.BankAccountType, bb.BankAccountNo
FROM Balance bal
LEFT OUTER JOIN BankBranchs bb ON bal.BankBranchId=bb.Id 
LEFT OUTER JOIN BankNames bn ON bb.BankId=bn.Id


";



                #endregion SqlText
                #region SqlExecution
                if (vm.BankBranchId == 0)
                {
                    sqlText = sqlText.Replace("BankBranchId=@BankBranchId", "1=1");
                }

                if (string.IsNullOrWhiteSpace(vm.WithdrawDate))
                {
                    sqlText = sqlText.Replace("DepositDate<@WithdrawDate", "1=1");
                    sqlText = sqlText.Replace("WithdrawDate<@WithdrawDate", "1=1");
                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);
                objComm.Parameters.AddWithValue("@WithdrawDate", Ordinary.DateToString(vm.WithdrawDate) ?? "20500101");




                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.TotalDepositAmount = Convert.ToDecimal(dr["TotalDepositAmount"]);
                    vm.TotalWithdrawAmount = Convert.ToDecimal(dr["TotalWithdrawAmount"]);
                    vm.AvailableBalance = Convert.ToDecimal(dr["AvailableBalance"]);
                    vm.BankBranchId = Convert.ToInt32(dr["BankBranchId"]);
                    vm.BankName = dr["BankName"].ToString();
                    vm.BranchName = dr["BranchName"].ToString();
                    vm.BankAccountType = dr["BankAccountType"].ToString();
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();

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
            return vm;
        }


        //==================SelectAll=================

        /// <summary>
        /// Retrieves a list of Withdraw from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Withdraw.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="WithdrawVM"/> representing the Withdraw matching the criteria.</returns>
        public List<WithdrawVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<WithdrawVM> VMs = new List<WithdrawVM>();
            WithdrawVM vm;
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
 w.Id
,w.Code
,w.WithdrawDate
,w.WithdrawAmount
,ISNULL(w.BankBranchId,0) BankBranchId
,b.Name+'~'+bb.BranchName as BankBranchName
,w.TransactionType
,w.ReferenceNo
,tm.Name TransactionMedia
,w.TransactionMediaId
,w.TransactionTypeId
,w.IsInvested

,ISNULL(w.Post,0) Post
,w.Remarks
,w.IsActive
,w.IsArchive
,w.CreatedBy
,w.CreatedAt
,w.CreatedFrom
,w.LastUpdateBy
,w.LastUpdateAt
,w.LastUpdateFrom

FROM Withdraws w
LEFT OUTER JOIN BankBranchs bb  ON w.BankBranchId = bb.Id
LEFT OUTER JOIN BankNames b ON bb.BankId = b.Id
LEFT OUTER JOIN TransactionMedias tm ON w.TransactionMediaId = tm.Id
WHERE  1=1  AND w.IsArchive = 0

";

                if (Id > 0)
                {
                    sqlText += @" and w.Id=@Id";
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

                sqlText += " ORDER BY w.WithdrawDate";

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
                    vm = new WithdrawVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.IsInvested = Convert.ToBoolean(dr["IsInvested"]);


                    vm.Code = dr["Code"].ToString();

                    vm.WithdrawDate = Ordinary.StringToDate(dr["WithdrawDate"].ToString());
                    vm.WithdrawAmount = Convert.ToDecimal(dr["WithdrawAmount"]);
                    vm.BankBranchId = Convert.ToInt32(dr["BankBranchId"]);
                    vm.BankBranchName = dr["BankBranchName"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.TransactionMedia = dr["TransactionMedia"].ToString();
                    vm.TransactionMediaId = Convert.ToInt32(dr["TransactionMediaId"].ToString());
                    vm.TransactionTypeId = Convert.ToInt32(dr["TransactionTypeId"]);


                    vm.Post = Convert.ToBoolean(dr["Post"]);
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

        //==================Insert =================

        /// <summary>
        /// Inserts a new Withdraw record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="WithdrawVM"/> containing the Withdraw data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertWithdraw").
        /// </returns>
        public string[] Insert(WithdrawVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertWithdraw"; //Method Name
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


                vm.Id = _cDal.NextId("Withdraws", currConn, transaction);
                if (vm != null)
                {
                    string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "BankWithdraw", vm.WithdrawDate, currConn, transaction);

                    vm.Code = NewCode;

                    ////vm.Code = "WTH-" + (vm.Id.ToString()).PadLeft(4, '0');

                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO Withdraws(
Id
,IsInvested
,Code
,WithdrawDate
,WithdrawAmount
,BankBranchId
,TransactionType
,TransactionTypeId
,ReferenceNo
,TransactionMediaId

,Post
,TransType
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@IsInvested
,@Code
,@WithdrawDate
,@WithdrawAmount
,@BankBranchId
,@TransactionType
,@TransactionTypeId
,@ReferenceNo
,@TransactionMediaId

,@Post
,@TransType
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    //WithdrawDate
                    //WithdrawAmount
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@IsInvested", false);

                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "WPPF");
                    cmdInsert.Parameters.AddWithValue("@WithdrawDate", Ordinary.DateToString(vm.WithdrawDate));
                    cmdInsert.Parameters.AddWithValue("@WithdrawAmount", vm.WithdrawAmount);
                    cmdInsert.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@TransactionMediaId", vm.TransactionMediaId);
                    cmdInsert.Parameters.AddWithValue("@TransactionTypeId", vm.TransactionTypeId);

                    cmdInsert.Parameters.AddWithValue("@Post", false);

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
                        throw new ArgumentNullException("Unexpected error to update Withdraws.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This Withdraw already used!";
                    throw new ArgumentNullException("Please Input Withdraw Value", "");
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
        /// Updates an existing Withdraw record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the updated Withdraw information.</param>
        /// <param name="VcurrConn">Optional external SQL connection. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">Optional external SQL transaction. If null, a new transaction will be created and committed.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Updated record ID,  
        /// [3] = The executed SQL query,  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("BankWithdraw").
        /// </returns>
        public string[] Update(WithdrawVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "WithdrawUpdate"; //Method Name
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
                    sqlText = "UPDATE Withdraws SET";

                    sqlText += " WithdrawDate=@WithdrawDate";
                    sqlText += " , WithdrawAmount=@WithdrawAmount";
                    sqlText += " , BankBranchId=@BankBranchId";
                    sqlText += " , TransactionType=@TransactionType";
                    sqlText += " , TransactionTypeId=@TransactionTypeId";

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
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "WPPF");
                    cmdUpdate.Parameters.AddWithValue("@WithdrawDate", Ordinary.DateToString(vm.WithdrawDate));
                    cmdUpdate.Parameters.AddWithValue("@WithdrawAmount", vm.WithdrawAmount);
                    cmdUpdate.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);
                    cmdUpdate.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TransactionTypeId", vm.TransactionTypeId);

                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@TransactionMediaId", vm.TransactionMediaId);


                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update Withdraws.", "");
                    }
                    #endregion SqlExecution

                    
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Withdraw Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Withdraw Update", "Could not found any item.");
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
        /// <summary>
        /// Performs a soft delete (archives) of one or more Withdraw records based on the provided IDs.
        /// Sets <c>IsActive</c> to false and <c>IsArchive</c> to true without removing data from the database.
        /// Allows optional use of an external SQL connection and transaction for transactional consistency.
        /// </summary>
        /// <param name="vm">The <see cref="WithdrawVM"/> containing metadata such as user and timestamp for the update.</param>
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
        /// [5] = Method name ("DeleteWithdraw").
        /// </returns>
        public string[] Delete(WithdrawVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteWithdraw"; //Method Name
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
                        WithdrawVM varWithdrawVM = new WithdrawVM();
                        varWithdrawVM = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();
                        if (varWithdrawVM.Post)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion Check Posted or Not Posted

                    

                }
                else
                {
                    throw new ArgumentNullException("Withdraw Information Delete", "Could not found any item.");
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
            retResults[5] = "PostWithdraw"; //Method Name
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
                        #region Available Balance Check

                        //////WithdrawVM vm = new WithdrawVM();
                        //////vm = SelectAll(Convert.ToInt32(ids[i]), null, null, currConn, transaction).FirstOrDefault();

                        //////WithdrawVM varWithdrawVM = new WithdrawVM();

                        //////varWithdrawVM = SelectAvailableBalance(vm, currConn, transaction);

                        //////if (vm.WithdrawAmount > varWithdrawVM.AvailableBalance)
                        //////{
                        //////    retResults[1] = "Withdraw Amount: " + vm.WithdrawAmount + " can't be greater than Available Balance: " + varWithdrawVM.AvailableBalance + "! In this Date: " + vm.WithdrawDate;
                        //////    throw new ArgumentNullException(retResults[1], "");
                        //////}



                        #endregion

                        sqlText = " ";
                        //sqlText = "UPDATE WithdrawDetails SET POST=1 WHERE WithdrawId=@Id";
                        sqlText = "UPDATE Withdraws SET POST=1 WHERE Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    #endregion Update Settings

                }
                else
                {
                    throw new ArgumentNullException("Withdraw Information Post - Could not found any item.", "");
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


        //==================SelectAll=================
        public DataTable Report(WithdrawVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 w.Id
,w.TransactionDate WithdrawDate
,w.CreditAmount  WithdrawAmount
,a.Name  as BankBranchName
,w.Remarks

FROM WithdrawDetails w
LEFT OUTER JOIN Accounts a  ON w.AccountId = a.Id
WHERE  1=1   

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
                #endregion SqlText
                #region SqlExecution

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
                dt = Ordinary.DtColumnStringToDate(dt, "WithdrawDate");

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
            return dt;
        }

        #endregion
    }
}
