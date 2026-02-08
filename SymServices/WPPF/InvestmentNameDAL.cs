using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace SymServices.WPPF
{
    public class InvestmentNameDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<InvestmentNameVM> DropDown(string TransType = "WPPF")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<InvestmentNameVM> VMs = new List<InvestmentNameVM>();
            InvestmentNameVM vm;
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
,Name
   FROM InvestmentNames
WHERE  1=1 AND IsArchive = 0 and TransType=@TransType
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                objComm.Parameters.AddWithValue("@TransType", TransType);

                
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new InvestmentNameVM();
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
        //==================SelectAll=================
        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment Name information.
        /// </summary>      
        /// <returns>View containing Investment Name</returns>
        public List<InvestmentNameVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null 
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<InvestmentNameVM> VMs = new List<InvestmentNameVM>();
            InvestmentNameVM vm;
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
,Name
,Isnull(Code,'-')Code
,Isnull(InvestmentTypeId,0)InvestmentTypeId
,Isnull(InvestmentDate,'20210101')InvestmentDate
,Isnull(FromDate,'20210101')FromDate
,Isnull(ToDate,'20210101')ToDate
,Isnull(MaturityDate,'20210101')MaturityDate
,Isnull(AitInterest,0)AitInterest
,Isnull(BankBranchId,0)BankBranchId
,Isnull(BankNameId,0)BankNameId
,Address
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom

from InvestmentNames
WHERE  1=1 AND IsArchive = 0
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
                    vm = new InvestmentNameVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Name = dr["Name"].ToString();
                    vm.Address = dr["Address"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.InvestmentTypeId = Convert.ToInt32(dr["InvestmentTypeId"].ToString());
                    vm.InvestmentDate = Ordinary.StringToDate(dr["InvestmentDate"].ToString());
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.MaturityDate = Ordinary.StringToDate(dr["MaturityDate"].ToString());
                    vm.AitRate = Convert.ToDecimal(dr["AitInterest"].ToString());
                    vm.BankBranchId = Convert.ToInt32(dr["BankBranchId"]);
                    vm.BankNameId = Convert.ToInt32(dr["BankNameId"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
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

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment Name Details information.
        /// </summary>      
        /// <returns>View containing Investment Name Details</returns>
        public List<InvestmentNameDetailsVM> SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<InvestmentNameDetailsVM> VMs = new List<InvestmentNameDetailsVM>();
            InvestmentNameDetailsVM vm;
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
,InvestmentNameId
,FromMonth
,ToMonth
,InterestRate
,Remarks
 From InvestmentNameDetails

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
                    vm = new InvestmentNameDetailsVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.InvestmentNameId = Convert.ToInt32(dr["InvestmentNameId"].ToString());
                    vm.InterestRate = Convert.ToDecimal(dr["InterestRate"].ToString());
                    vm.FromMonth = Convert.ToDecimal(dr["FromMonth"].ToString());
                    vm.ToMonth = Convert.ToDecimal(dr["ToMonth"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
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
        /// Inserts a new Investment Name record into the database, including associated details, with optional support for external transactions.
        /// </summary>
        /// <param name="vm">The <see cref="InvestmentNameVM"/> view model containing investment data to be inserted.</param>
        /// <param name="VcurrConn">
        /// An optional <see cref="SqlConnection"/> used for the operation. If not provided, a new connection is opened.
        /// </param>
        /// <param name="Vtransaction">
        /// An optional <see cref="SqlTransaction"/> used for the operation. If not provided, a new transaction is started.
        /// </param>
        /// <returns>
        /// A string array with the following indices:
        /// <list type="number">
        /// <item><description>Result status ("Success" or "Fail").</description></item>
        /// <item><description>Status message or error message.</description></item>
        /// <item><description>The inserted record ID, or 0 on failure.</description></item>
        /// <item><description>The SQL query executed (if available).</description></item>
        /// <item><description>Exception message if any error occurred.</description></item>
        /// <item><description>The name of the method ("InsertInvestmentName").</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This method performs several steps: checks for code duplication, retrieves the fiscal year, inserts the main record,
        /// and inserts associated detail records. It supports being called with an external connection and transaction context, which
        /// is useful for batch or transactional operations. If no external connection or transaction is provided, it handles connection
        /// opening, committing, or rolling back internally.
        /// </remarks>

        public string[] Insert(InvestmentNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            int countId = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertInvestmentName"; //Method Name
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

                #region Exist Check
                sqlText = "select count(Id) from InvestmentNames where  Code=@Code";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@Code", vm.Code);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    retResults[1] = "Same Code('" + vm.Code + "') already exist ";
                    throw new ArgumentNullException("", retResults[1]);
                }

                CommonDAL _cDal = new CommonDAL();



                #endregion

                #region Save
                vm.FiscalYearDetailId = new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(Ordinary.DateToString(vm.InvestmentDate), currConn, transaction).Id.ToString();
                
                vm.Id = _cDal.NextId("InvestmentNames", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO InvestmentNames(
Id
,Name
,Address
,Code
,InvestmentTypeId
,InvestmentDate
,FromDate
,ToDate
,MaturityDate
,AitInterest
,BankBranchId
,BankNameId
,FiscalYearDetailId
,TransType
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,BranchId
) VALUES (
@Id
,@Name
,@Address
,@Code
,@InvestmentTypeId
,@InvestmentDate
,@FromDate
,@ToDate
,@MaturityDate
,@AitInterest
,@BankBranchId
,@BankNameId
,@FiscalYearDetailId
,@TransType
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@BranchId
) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);
                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@InvestmentTypeId", vm.InvestmentTypeId);
                    cmdInsert.Parameters.AddWithValue("@InvestmentDate", Ordinary.DateToString(vm.InvestmentDate));
                    cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdInsert.Parameters.AddWithValue("@MaturityDate", Ordinary.DateToString(vm.MaturityDate));
                    cmdInsert.Parameters.AddWithValue("@AitInterest", vm.AitRate);
                    cmdInsert.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);
                    cmdInsert.Parameters.AddWithValue("@BankNameId", vm.BankNameId);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "WPPF");
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update InvestmentNames.", "");
                    }
                    if (vm.InvestmentNameDetails.Count > 0)
                    {
                        InsertDetails(vm, currConn, transaction);
                    }
                }
                else
                {
                    retResults[1] = "This InvestmentName already used!";
                    throw new ArgumentNullException("Please Input InvestmentName Value", "");
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
        /// Updates an existing Investment record along with its associated details in the database.
        /// Performs validation to ensure uniqueness of the investment code, handles fiscal year calculations,
        /// and uses a SQL transaction to maintain data integrity. If the update is successful,
        /// the method returns a status message and the updated record ID.
        /// </summary>
        /// <param name="vm">The view model containing the InvestmentName and related detail records to update.</param>
        /// <param name="VcurrConn">Optional SQL connection used when the update is part of a broader transaction.</param>
        /// <param name="Vtransaction">Optional SQL transaction used to wrap the update operation for rollback support.</param>
        /// <returns>
        /// A string array containing:
        /// [0] - Status ("Success" or "Fail"),
        /// [1] - Descriptive message,
        /// [2] - Updated record ID or empty if failed,
        /// [3] - Executed SQL (for debugging),
        /// [4] - Exception message (if any),
        /// [5] - Method name ("Update").
        /// </returns>

        public string[] Update(InvestmentNameVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            int countId = 0;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToInvestmentName"); }
                #endregion open connection and transaction

                #region Exist Check
                sqlText = "select count(Id) from InvestmentNames where  Code=@Code and Id <>@Id";
                SqlCommand cmdCodeExist = new SqlCommand(sqlText, currConn);
                cmdCodeExist.Transaction = transaction;
                cmdCodeExist.Parameters.AddWithValue("@Code", vm.Code);
                cmdCodeExist.Parameters.AddWithValue("@Id", vm.Id);

                countId = (int)cmdCodeExist.ExecuteScalar();
                if (countId > 0)
                {
                    retResults[1] = "Same Code('" + vm.Code + "') already exist ";
                    throw new ArgumentNullException("", retResults[1]);
                }

                CommonDAL _cDal = new CommonDAL();



                #endregion


                if (vm != null)
                {
                    vm.FiscalYearDetailId = new FiscalYearDAL().SelectAll_FiscalYearDetailByDate(Ordinary.DateToString(vm.InvestmentDate), currConn, transaction).Id.ToString();

                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE InvestmentNames SET";
                    sqlText += " Name=@Name";
                    sqlText += " , Code=@Code";
                    sqlText += " , Address=@Address";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , InvestmentTypeId=@InvestmentTypeId";
                    sqlText += " , InvestmentDate=@InvestmentDate";
                    sqlText += " , FromDate=@FromDate";
                    sqlText += " , ToDate=@ToDate";
                    sqlText += " , MaturityDate=@MaturityDate";
                    sqlText += " , AitInterest=@AitInterest";
                    sqlText += " , BankBranchId=@BankBranchId";
                    sqlText += " , BankNameId=@BankNameId";
                    sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , TransType=@TransType";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code);
                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentDate", Ordinary.DateToString(vm.InvestmentDate));
                    cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdUpdate.Parameters.AddWithValue("@MaturityDate", Ordinary.DateToString(vm.MaturityDate));
                    cmdUpdate.Parameters.AddWithValue("@AitInterest", vm.AitRate);
                    cmdUpdate.Parameters.AddWithValue("@BankBranchId", vm.BankBranchId);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentTypeId", vm.InvestmentTypeId);
                    cmdUpdate.Parameters.AddWithValue("@BankNameId", vm.BankNameId);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                   
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "WPPF");

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update InvestmentNames.", "");
                    }
                    string deleteDetails = "delete from InvestmentNameDetails where InvestmentNameId = @InvestmentNameId";
                    cmdUpdate.CommandText = deleteDetails;
                    cmdUpdate.Parameters.AddWithValue("@InvestmentNameId", vm.Id);
                    cmdUpdate.ExecuteNonQuery();

                    if (vm.InvestmentNameDetails.Count > 0)
                    {
                        InsertDetails(vm, currConn, transaction);
                    }
                    else
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLJournals.", "");
                    }
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("InvestmentName Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("InvestmentName Update", "Could not found any item.");
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
        public string[] Delete(InvestmentNameVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteInvestmentName"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToInvestmentName"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update InvestmentNames set";
                        sqlText += " IsActive=@IsActive";
                        sqlText += " ,IsArchive=@IsArchive";
                        sqlText += " ,LastUpdateBy=@LastUpdateBy";
                        sqlText += " ,LastUpdateAt=@LastUpdateAt";
                        sqlText += " ,LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("InvestmentName Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("InvestmentName Information Delete", "Could not found any item.");
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

        /// <summary>
        /// Inserts a list of Investment detail records into the database for a given InvestmentName.
        /// Uses parameterized SQL and supports external SQL connection and transaction management.
        /// If no detail records are provided, the method throws an exception. All records are inserted
        /// in a single transaction to ensure data integrity.
        /// </summary>
        /// <param name="InvestmentNameVM">The view model containing the InvestmentName ID and its associated detail records.</param>
        /// <param name="VcurrConn">
        /// Optional SQL connection. If null, a new connection will be opened for this operation.
        /// </param>
        /// <param name="Vtransaction">
        /// Optional SQL transaction. If null, a new transaction will be created and committed within the method.
        /// </param>
        /// <returns>
        /// A string array containing:
        /// [0] - Status ("Success" or "Fail"),
        /// [1] - Message ("Data Save Successfully." or error message),
        /// [2] - Return ID (not used currently, defaults to 0),
        /// [3] - Executed SQL (currently unused),
        /// [4] - Exception message (if any),
        /// [5] - Method name ("InvestmentNameDetailsVM").
        /// </returns>

        public string[] InsertDetails(InvestmentNameVM InvestmentNameVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int NextId = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = NextId.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InvestmentNameDetailsVM"; //Method Name
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

                List<InvestmentNameDetailsVM> VMs = InvestmentNameVM.InvestmentNameDetails;
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO InvestmentNameDetails (
 InvestmentNameId
,FromMonth
,ToMonth
,InterestRate
,Remarks
,TransType


) VALUES (
@InvestmentNameId
,@FromMonth
,@ToMonth
,@InterestRate
,@Remarks
,@TransType
) 
";
                    #endregion SqlText

                    #region SqlExecution
                    foreach (InvestmentNameDetailsVM vm in VMs)
                    {

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@InvestmentNameId", InvestmentNameVM.Id);
                        cmdInsert.Parameters.AddWithValue("@FromMonth", vm.FromMonth);
                        cmdInsert.Parameters.AddWithValue("@ToMonth", vm.ToMonth);
                        cmdInsert.Parameters.AddWithValue("@InterestRate", vm.InterestRate);
                        cmdInsert.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(vm.Remarks) ? Convert.DBNull : vm.Remarks);
                        cmdInsert.Parameters.AddWithValue("@TransType", InvestmentNameVM.TransType ?? "WPPF");

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "No Detail Found!";
                    throw new ArgumentNullException(retResults[1], "");
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
                retResults[2] = NextId.ToString();
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



        public string[] AutoJournalSave(string JournalType, string JournalFor, string InterestAmount, string AccruedId, string TransactionDate, string BranchId, SqlConnection currConn, SqlTransaction transaction, ShampanIdentityVM auditvm)
        {
            if (currConn == null)
            {
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
            }

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message

            string InvestmentCOAID = "";
            string IntReciveCOAID = "";
            string IntIncomeCOAID = "";

            string Journal = @"SELECT
                                JournalName,COAID
                                FROM AutoJournalSetup
                                WHERE  1=1 AND JournalFor = @JournalFor and IsActive=1";
            SqlCommand cmdj = new SqlCommand(Journal, currConn, transaction);
            cmdj.Parameters.AddWithValue("JournalFor", JournalFor);
            SqlDataAdapter adapterj = new SqlDataAdapter(cmdj);
            DataTable dtj = new DataTable();
            adapterj.Fill(dtj);
            if (dtj.Rows.Count > 0)
            {               
                IntReciveCOAID = dtj.Rows[0]["COAID"].ToString();
                IntIncomeCOAID = dtj.Rows[1]["COAID"].ToString();
            }

            SettingDAL _settingDal = new SettingDAL();
            string IsAutoJournal = _settingDal.settingValue("WPPF", "IsAutoJournal").Trim();

            if (IsAutoJournal == "Y")
            {              

                GLJournalVM vmj = new GLJournalVM
                {
                    Id = 1,
                    CreatedAt = DateTime.Now.ToString(),
                    CreatedBy = "admin",
                    CreatedFrom = "",
                    TransactionDate = DateTime.Now.ToString(),
                    TransactionType = 31,
                    JournalType = 1,
                    TransType = "WPPF",
                    TransactionValue = Convert.ToDecimal(InterestAmount),

                    GLJournalDetails = new List<GLJournalDetailVM>
                    {
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(IntReciveCOAID),
                            DrAmount = Convert.ToDecimal(InterestAmount),
                            IsDr = true,
                            IsYearClosing = false,
                            Post = false
                        },
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(IntIncomeCOAID),
                            CrAmount = Convert.ToDecimal(InterestAmount),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                      
                    }
                };
                vmj.Code = AccruedId;
                vmj.BranchId = BranchId;
                vmj.TransactionDate = TransactionDate;
                vmj.Remarks = "Interest from Investment";
                GLJournalDAL glJournalDal = new GLJournalDAL();
                retResults = glJournalDal.Insert(vmj);

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                #endregion SuccessResult
            }

            #region Results
            return retResults;
            #endregion

        }
        #endregion
    }
}
