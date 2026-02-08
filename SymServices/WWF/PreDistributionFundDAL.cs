using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SymServices.WWF
{
    public class PreDistributionFundDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of Pre Distribution Fund from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Pre Distribution Fund.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="BankBranchVM"/> representing the Pre Distribution Fund matching the criteria.</returns>
        public List<PreDistributionFundVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PreDistributionFundVM> VMs = new List<PreDistributionFundVM>();
            PreDistributionFundVM vm;


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
 pdf.Id
,Code
,Format(cast(TransactionDate as datetime),'dd-MMM-yyyy')TransactionDate
,TotalValue
,Remarks
,pdf.Post
,pdf.IsDistribute
,pdf.Remarks
,pdf.IsActive
,pdf.IsArchive
,pdf.CreatedBy
,pdf.CreatedAt
,pdf.CreatedFrom
,pdf.LastUpdateBy
,pdf.LastUpdateAt
,pdf.LastUpdateFrom

FROM PreDistributionFunds pdf
WHERE  1=1 AND IsArchive = 0
";
                //TotalFundingValue
                //FundingValue
                //ReservedFundingValue
                //FundingReferenceIds

                if (Id > 0)
                {
                    sqlText += @" and pdf.Id=@Id";
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
                    vm = new PreDistributionFundVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = Convert.ToString(dr["Code"]);
                    vm.TransactionDate = Convert.ToString(dr["TransactionDate"]);
                    vm.TotalValue = Convert.ToString(dr["TotalValue"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);

                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsDistribute = Convert.ToBoolean(dr["IsDistribute"]);
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
        /// Inserts a new Pre Distribution Fund record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="PreDistributionFundVM"/> containing the Pre Distribution Fund data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertPreDistributionFund").
        /// </returns>
        public string[] Insert(PreDistributionFundVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertPreDistributionFund"; //Method Name
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



                int nextId = _cDal.NextId("PreDistributionFunds", currConn, transaction);
                vm.Id = nextId;
                string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "PreDistributionFund", vm.TransactionDate, currConn, transaction);
                vm.Code = NewCode;



                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO PreDistributionFunds(
Id
,Code
,TransactionDate
,TotalValue
,Post
,IsDistribute
,TransType


,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,BranchId
) VALUES (
@Id
,@code
,@TransactionDate
,@TotalValue
,@Post
,@IsDistribute
,@TransType

,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@BranchId
) 
";
                    //TotalFundingValue
                    //FundingValue
                    //ReservedFundingValue
                    //FundingReferenceIds
                    //TransactionType


                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.TransactionDate));
                    cmdInsert.Parameters.AddWithValue("@TotalValue", vm.TotalValue);

                    cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                    cmdInsert.Parameters.AddWithValue("@IsDistribute", vm.IsDistribute);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "WWF");

                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update PreDistributionFunds.", "");
                    }



                    #endregion SqlExecution
                    
                    
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
        /// Updates an existing Pre Distribution Fund record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the updated Pre Distribution Fund information.</param>
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
        public string[] Update(PreDistributionFundVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee PreDistributionFund Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPreDistributionFund"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE PreDistributionFunds SET";


                    sqlText += "  TransactionDate=@TransactionDate";
                    sqlText += " , TotalValue=@TotalValue";
                    sqlText += " , Post=@Post";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , TransType=@TransType";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.TransactionDate));

                    cmdUpdate.Parameters.AddWithValue("@TotalValue", vm.TotalValue);
                    cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);
     
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "WWF");

                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update PreDistributionFunds.", "");
                    }
                    #endregion SqlExecution
                   
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("PreDistributionFund Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PreDistributionFund Update", "Could not found any item.");
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
        /// /// <summary>
        /// Performs a soft delete (archives) of one or more Pre Distribution Fund records based on the provided IDs.
        /// Sets <c>IsActive</c> to false and <c>IsArchive</c> to true without removing data from the database.
        /// Allows optional use of an external SQL connection and transaction for transactional consistency.
        /// </summary>
        /// <param name="vm">The <see cref="PreDistributionFundVM"/> containing metadata such as user and timestamp for the update.</param>
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
        /// [5] = Method name ("DeletePreDistributionFund").
        /// </returns>
        public string[] Delete(PreDistributionFundVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePreDistributionFund"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPreDistributionFund"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("PreDistributionFunds", "Post", "Id", ids[i].ToString(), currConn, transaction);
                        vm.Post = Convert.ToBoolean(retVal);
                        if (vm.Post == true)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException("Data Alreday Posted! Cannot Deleted.", "");
                        }
                    }
                    #endregion Check Posted or Not Posted
                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        #region SELECT PreDistributionFunds
                        PreDistributionFundVM preDistributionFundVM = new PreDistributionFundVM();
                        preDistributionFundVM = SelectAll(Convert.ToInt32(ids[i])).FirstOrDefault();

                        #endregion SELECT PreDistributionFunds
                        #region  Update Sources False
                        if (preDistributionFundVM.TransactionType == "ReturnOnInvestment")
                        {
                            #region Update ReturnOnInvestments IsTransferPDF  False
                            string OtherIds = preDistributionFundVM.FundingReferenceIds;
                            OtherIds = OtherIds.Trim('~');
                            OtherIds = OtherIds.Replace("~", "','");

                            sqlText = "";
                            sqlText = "update ReturnOnInvestments set";
                            sqlText += " IsTransferPDF=0";

                            sqlText += " where Id In ('" + OtherIds + "')";
                            SqlCommand cmdIsTransferPDF = new SqlCommand(sqlText, currConn, transaction);
                            var exeRes = cmdIsTransferPDF.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            if (transResult <= 0)
                            {
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Unexpected error to update ReturnOnInvestments.", "");
                            }
                            #endregion Update ReturnOnInvestments IsTransferPDF False
                        }
                        else if (preDistributionFundVM.TransactionType == "ReturnOnBankInterest")
                        {
                            #region Update ReturnOnBankInterest IsTransferPDF False
                            string OtherIds = preDistributionFundVM.FundingReferenceIds;
                            OtherIds = OtherIds.Trim('~');
                            OtherIds = OtherIds.Replace("~", "','");

                            sqlText = "";
                            sqlText = "update ReturnOnBankInterests set";
                            sqlText += " IsTransferPDF=0";

                            sqlText += " where Id In ('" + OtherIds + "')";
                            SqlCommand cmdIsTransferPDF = new SqlCommand(sqlText, currConn, transaction);
                            var exeRes = cmdIsTransferPDF.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            if (transResult <= 0)
                            {
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Unexpected error to update ReturnOnBankInterests.", "");
                            }
                            #endregion
                        }
                        else if (preDistributionFundVM.TransactionType == "ReservedFund")
                        {
                            #region Update ReservedFunds IsTransferPDF False
                            string OtherIds = preDistributionFundVM.FundingReferenceIds;
                            OtherIds = OtherIds.Trim('~');
                            OtherIds = OtherIds.Replace("~", "','");

                            sqlText = "";
                            sqlText = "update ReservedFunds set";
                            sqlText += " IsTransferPDF=0";

                            sqlText += " where Id In ('" + OtherIds + "')";
                            SqlCommand cmdIsTransferPDF = new SqlCommand(sqlText, currConn, transaction);
                            var exeRes = cmdIsTransferPDF.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            if (transResult <= 0)
                            {
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Unexpected error to update ReservedFunds.", "");
                            }
                            #endregion
                        }
                        else if (preDistributionFundVM.TransactionType == "ForfeitureAccount")
                        {
                            #region Update ReturnOnInvestments IsTransferPDF False
                            string OtherIds = preDistributionFundVM.FundingReferenceIds;
                            OtherIds = OtherIds.Trim('~');
                            OtherIds = OtherIds.Replace("~", "','");

                            sqlText = "";
                            sqlText = "update ForfeitureAccounts set";
                            sqlText += " IsTransferPDF=0";

                            sqlText += " where Id In ('" + OtherIds + "')";
                            SqlCommand cmdIsTransferPDF = new SqlCommand(sqlText, currConn, transaction);
                            var exeRes = cmdIsTransferPDF.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            if (transResult <= 0)
                            {
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Unexpected error to update ForfeitureAccounts.", "");
                            }
                            #endregion Update ReturnOnInvestments IsTransferPDF False
                        }



                        #endregion  Update Sources False
                        #region DELETE ReservedFunds
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE ReservedFunds";
                        sqlText += " WHERE PDFId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeResult = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeResult);
                        ////////if (transResult <= 0)
                        ////////{
                        ////////    retResults[3] = sqlText;
                        ////////    throw new ArgumentNullException("Unexpected error to Delete ReservedFunds.", "");
                        ////////}
                        #endregion DELETE ReservedFunds
                        #region DELETE PreDistributionFunds
                        sqlText = " ";
                        sqlText = "DELETE PreDistributionFunds";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeResult = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeResult);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to Delete PreDistributionFunds.", "");
                        }
                        #endregion DELETE PreDistributionFunds



                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("PreDistributionFund Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("PreDistributionFund Information Delete", "Could not found any item.");
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
            retResults[5] = "PostPreDistributionFund"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("PostPreDistributionFund"); }


                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retResults = _cDal.FieldPost("PreDistributionFunds", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("PreDistributionFunds Post", ids[i] + " could not Post.");
                        }
                        retResults = _cDal.FieldPost("ReservedFunds", "PDFId", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("ReservedFunds Post", ids[i] + " could not Post.");
                        }

                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PreDistributionFund Information Post", "Could not found any item.");
                }
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
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
        public DataTable Report(PreDistributionFundVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
pdf.FundingDate
,pdf.TotalFundingValue
,pdf.FundingValue
,pdf.ReservedFundingValue
,pdf.FundingReferenceIds
,pdf.TransactionType
,pdf.Post
,pdf.IsDistribute
,pdf.Remarks
FROM PreDistributionFunds pdf

";
                sqlText += @" WHERE  1=1  AND pdf.IsArchive = 0
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

        public string[] AutoJournalSave(string JournalType, string TransactionForm, string TransactionCode, string BranchId, SqlConnection currConn, SqlTransaction transaction, ShampanIdentityVM auditvm)
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

            string EmployeeCOAID = "";
            string EmployerCOAID = "";
            string BankCOAID = "";

            string Journal = @"SELECT
                                JournalName,COAID
                                FROM AutoJournalSetup
                                WHERE  1=1 AND JournalFor = @JournalFor and IsActive=1";
            SqlCommand cmdj = new SqlCommand(Journal, currConn, transaction);
            cmdj.Parameters.AddWithValue("JournalFor", "3");
            SqlDataAdapter adapterj = new SqlDataAdapter(cmdj);
            DataTable dtj = new DataTable();
            adapterj.Fill(dtj);
            if (dtj.Rows.Count > 0)
            {
                EmployeeCOAID = dtj.Rows[0]["COAID"].ToString();
                EmployerCOAID = dtj.Rows[1]["COAID"].ToString();
                BankCOAID = dtj.Rows[2]["COAID"].ToString();

            }

            SettingDAL _settingDal = new SettingDAL();
            string IsAutoJournal = _settingDal.settingValue("WWF", "IsAutoJournal").Trim();

            if (IsAutoJournal == "Y")
            {

                string id = @"SELECT
                            TotalValue,TransactionDate
                            FROM PreDistributionFunds
                            WHERE  1=1 AND Code = @Code";
                SqlCommand cmdid = new SqlCommand(id, currConn, transaction);
                cmdid.Parameters.AddWithValue("@Code", TransactionCode);
                SqlDataAdapter adapterid = new SqlDataAdapter(cmdid);
                DataTable dtpf = new DataTable();
                adapterid.Fill(dtpf);

               

                GLJournalVM vmj = new GLJournalVM
                {
                    Id = 1,
                    CreatedAt = DateTime.Now.ToString(),
                    CreatedBy = "admin",
                    CreatedFrom = "",
                    //TransactionDate = DateTime.Now.ToString(),
                    TransactionDate =dtpf.Rows[0]["TransactionDate"].ToString(),
                    TransactionType = 31,
                    JournalType = 1,
                    TransType = "WWF",
                    TransactionValue = Convert.ToDecimal(dtpf.Rows[0]["TotalValue"].ToString()),

                    GLJournalDetails = new List<GLJournalDetailVM>
                    {
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployeeCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["TotalValue"].ToString())/2,
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployerCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["TotalValue"].ToString())/2,
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                            new GLJournalDetailVM
                        {
                            COAId =Convert.ToInt32(BankCOAID),
                            DrAmount = Convert.ToDecimal(dtpf.Rows[0]["TotalValue"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        }
                    }
                };
                vmj.Code = TransactionCode;
                vmj.BranchId = BranchId;
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
