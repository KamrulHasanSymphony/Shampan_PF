using SymOrdinary;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.WPPF
{
    public class EETransactionDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region Methods
        //==================SelectAll=================
        public List<EETransactionDetailVM> SelectAll(int Id = 0, int EETransactionId = 0, string[] conditionField = null, string[] conditionValue = null, bool IsPS = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EETransactionDetailVM> VMs = new List<EETransactionDetailVM>();
            EETransactionDetailVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion open connection and transaction

                #region sql statement
                sqlText = @"SELECT td.Id
,td.BranchId
,td.SL
,td.EETransactionId
,td.EEHeadId
,hd.Name EEHeadName
,td.SubTotal
,td.TransactionDateTime
,td.TransactionType
,isnull(td.Post,0)Post
,td.Remarks
,td.IsActive
,td.IsArchive
,td.CreatedBy
,td.CreatedAt
,td.CreatedFrom
,td.LastUpdateBy
,td.LastUpdateAt
,td.LastUpdateFrom
From EETransactionDetails td
left outer join EEHeads hd on td.EEHeadId = hd.Id
Where   1=1
";
                if (!IsPS)
                    sqlText += @" AND td.IsPS=0";
                if (EETransactionId > 0)
                {
                    sqlText += @" AND  td.EETransactionId=@EETransactionId";
                }
                if (Id > 0)
                {
                    sqlText += @" AND  td.id=@Id";
                }
                if (conditionField != null)
                {
                    int i = 0;
                    foreach (var item in conditionField)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionValue[i]))
                        {
                            sqlText += " AND td." + conditionField[i] + "='" + conditionValue[i] + "'";
                            i++;
                        }
                    }
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (EETransactionId > 0)
                {
                    objComm.Parameters.AddWithValue("@EETransactionId", EETransactionId);
                }
                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EETransactionDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.SL = Convert.ToInt32(dr["SL"]);
                    vm.EETransactionId = Convert.ToInt32(dr["EETransactionId"]);
                    vm.EEHeadId = Convert.ToInt32(dr["EEHeadId"]);
                    vm.EEHeadName = dr["EEHeadName"].ToString();
                    vm.SubTotal = Convert.ToInt32(dr["SubTotal"]);
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Remarks = dr["Remarks"].ToString();
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
        //==================Insert =================
        public string[] Insert(EETransactionDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = { "Fail", "Fail", Id.ToString(), sqlText, "ex", "InsertEETransactionDetail" };
            //0 - Success or Fail//1 - Success or Fail Message//2 - Return Id//3 - SQL Query//4 - catch ex//5 - Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                else if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                else if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #region Exist
                //CommonDAL cdal = new CommonDAL();
                //bool check = false;
                //string tableName = "EETransactionDetail";	
                //string[] fieldName = { "Code", "Name" };
                //string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
                //for (int i = 0; i < fieldName.Length; i++)
                //{
                //    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                //    if (check == true)
                //    {
                //        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                //        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                //    }
                //}
                #endregion Exist
                #endregion open connection and transaction
                #region Save
                vm.Id = Ordinary.NextId("EETransactionDetails", currConn, transaction);
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EETransactionDetails(Id
,BranchId
,SL
,EETransactionId
,EEHeadId
,SubTotal
,TransactionDateTime
,TransactionType
,Post
,IsPS
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@Id
,@BranchId
,@SL
,@EETransactionId
,@EEHeadId
,@SubTotal
,@TransactionDateTime
,@TransactionType
,@Post
,@IsPS
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@SL", vm.SL);
                    cmdInsert.Parameters.AddWithValue("@EETransactionId", vm.EETransactionId);
                    cmdInsert.Parameters.AddWithValue("@IsPS", vm.IsPS);
                    cmdInsert.Parameters.AddWithValue("@EEHeadId", vm.EEHeadId);
                    cmdInsert.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, bankVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This EETransactionDetail already used!";
                    throw new ArgumentNullException("Please Input EETransactionDetail Value", "");
                }
                #endregion Save
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
        ////==================Report=================
        public DataTable Report(EETransactionDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                currConn = _dbsqlConnection.GetConnectionPF();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
SELECT 
t.Code
,td.BranchId
,br.Name BranchName
,td.SL
,td.EETransactionId
,hd.Name EEHeadName
,td.SubTotal
,t.TransactionDateTime
,t.TransactionType
,isnull(td.Post,0)Post
,t.Remarks
,td.Remarks DetailRemarks
FROM EETransactionDetails td
LEFT OUTER JOIN EETransactions t ON td.EETransactionId=t.Id
LEFT OUTER JOIN EEHeads hd ON td.EEHeadId = hd.Id
LEFT OUTER JOIN Branchs br ON td.BranchId = br.Id
WHERE 1=1
";
                if (!vm.IsPS)
                    sqlText += @" AND  td.IsPS=0";
                if (vm.BranchId > 0)
                    sqlText += @" and  t.BranchId=@BranchId";
                if (vm.PostStatus == "Posted")
                {
                    sqlText += " AND t.Post = 1";
                }
                else if (vm.PostStatus == "Not Posted")
                {
                    sqlText += " AND t.Post = 0";
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

                if (vm.BranchId > 0)
                    da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDateTime");

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
