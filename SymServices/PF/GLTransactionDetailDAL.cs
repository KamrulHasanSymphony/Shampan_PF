using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SymServices.PF
{
    public class GLTransactionDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods

        //==================SelectAll=================

        public List<GLTransactionDetailVM> SelectAllX(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLTransactionDetailVM> VMs = new List<GLTransactionDetailVM>();
            GLTransactionDetailVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
SELECT td.Id
      ,td.DrAccountIdForCredit
      ,td.TransactionMasterId
      ,td.TransactionDate
      ,td.TransactionType
      ,td.AccountId
      ,td.DebitAmount
      ,td.CreditAmount
      ,td.Post
      ,td.Remarks
      ,td.IsActive
      ,td.IsArchive
      ,td.CreatedBy
      ,td.CreatedAt
      ,td.CreatedFrom
      ,td.LastUpdateBy
      ,td.LastUpdateAt
      ,td.LastUpdateFrom
      ,td.PostedBy
      ,td.PostedAt
      ,td.PostedFrom
      ,td.TransactionAmount
      ,td.TransactionCode
	  ,acc.Name AccountHead
	  ,acc.AccountType
  FROM GLTransactionDetails td
  left outer join Accounts acc on td.AccountId=acc.Id
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

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLTransactionDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);

                    vm.DrAccountIdForCredit = Convert.ToInt32(dr["DrAccountIdForCredit"]);
                    vm.TransactionMasterId = Convert.ToInt32(dr["TransactionMasterId"]);

                    vm.TransactionCode = Convert.ToString(dr["TransactionCode"]);
                    vm.TransactionDate = Ordinary.StringToDate(dr["TransactionDate"].ToString());
                    vm.TransactionType = Convert.ToString(dr["TransactionType"]);
                    vm.AccountId = Convert.ToInt32(dr["AccountId"]);
                    vm.DebitAmount = Convert.ToDecimal(dr["DebitAmount"]);
                    vm.CreditAmount = Convert.ToDecimal(dr["CreditAmount"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = Convert.ToString(dr["Remarks"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedBy = Convert.ToString(dr["CreatedBy"]);
                    vm.CreatedAt = Convert.ToString(dr["CreatedAt"]);
                    vm.CreatedFrom = Convert.ToString(dr["CreatedFrom"]);
                    vm.LastUpdateBy = Convert.ToString(dr["LastUpdateBy"]);
                    vm.LastUpdateAt = Convert.ToString(dr["LastUpdateAt"]);
                    vm.LastUpdateFrom = Convert.ToString(dr["LastUpdateFrom"]);
                    vm.PostedBy = Convert.ToString(dr["PostedBy"]);
                    vm.PostedAt = Convert.ToString(dr["PostedAt"]);
                    vm.PostedFrom = Convert.ToString(dr["PostedFrom"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.AccountHead = Convert.ToString(dr["AccountHead"]);
                    vm.PostedFrom = Convert.ToString(dr["PostedFrom"]);

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
        public string[] InsertX(List<GLTransactionDetailVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertGLTransactionDetail"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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


                NextId = _cDal.NextId("GLTransactionDetails", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO [dbo].[GLTransactionDetails] (
Id
,DrAccountIdForCredit
,TransactionCode
,TransactionMasterId
,TransactionDate
,TransactionType
,AccountId
,IsDr
,IsSingle

,DebitAmount
,CreditAmount
,TransactionAmount
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
@Id
,@DrAccountIdForCredit
,@TransactionCode
,@TransactionMasterId
,@TransactionDate
,@TransactionType
,@AccountId
,@IsDr
,@IsSingle

,@DebitAmount
,@CreditAmount
,@TransactionAmount
,@Post
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) 
";
                    #endregion SqlText

                    #region SqlExecution
                    int DrAccountIdForCredit = 0;

                    #region Debit/Credit Count
                    
                    int DrCount = 0;
                    int CrCount = 0;

                    DrCount = VMs.Where(c => c.DebitAmount > 0).ToList().Count;
                    CrCount = VMs.Where(c => c.CreditAmount > 0).ToList().Count;

                    bool IsDrSingle = true;
                    bool IsCrSingle = true;

                    if (DrCount > 1)
                    {
                        IsDrSingle = false;
                    }
                    if (CrCount > 1)
                    {
                        IsCrSingle = false;
                    }
                    #endregion

                    foreach (GLTransactionDetailVM vm in VMs)
                    {
                        #region DrAccountIdForCredit


                        if (vm.DebitAmount > 0)
                        {
                            DrAccountIdForCredit = vm.AccountId;
                        }

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@Id", NextId);

                        if (vm.CreditAmount > 0)
                        {
                            vm.DrAccountIdForCredit = DrAccountIdForCredit;

                        }

                        #endregion

                        #region IsDr and IsSingle
                        if (vm.DebitAmount > 0)
                        {
                            vm.IsDr = true;
                            vm.IsSingle = IsDrSingle;
                            vm.TransactionAmount = vm.DebitAmount;
                        }
                        else
                        {
                            vm.IsDr = false;
                            vm.IsSingle = IsCrSingle;
                            vm.TransactionAmount = vm.CreditAmount;
                        }

                        #endregion


                        cmdInsert.Parameters.AddWithValue("@TransactionMasterId", vm.TransactionMasterId);
                        cmdInsert.Parameters.AddWithValue("@DrAccountIdForCredit", vm.DrAccountIdForCredit);

                        cmdInsert.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);

                        cmdInsert.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.TransactionDate));
                        cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@AccountId", vm.AccountId);
                        cmdInsert.Parameters.AddWithValue("@IsDr", vm.IsDr);
                        cmdInsert.Parameters.AddWithValue("@IsSingle", vm.IsSingle);

                        cmdInsert.Parameters.AddWithValue("@DebitAmount", vm.DebitAmount);
                        cmdInsert.Parameters.AddWithValue("@CreditAmount", vm.CreditAmount);
                        cmdInsert.Parameters.AddWithValue("@TransactionAmount", vm.TransactionAmount);
                        cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        NextId++;
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


        public DataTable Report(PFParameterVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnectionPF();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                #region sql statement

                #region SqlText

                sqlText = @"

SELECT
td.TransactionDate
,acc.Name AccountHead
,td.DebitAmount
,td.CreditAmount
,td.Remarks
,td.TransactionAmount
,td.TransactionType
,td.AccountId
,acc.AccountType
,td.TransactionCode
FROM GLTransactionDetails td
left outer join Accounts acc on td.AccountId=acc.Id
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
                #endregion

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

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

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");
                #endregion

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
                if (currConn != null && currConn.State == ConnectionState.Open)
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
