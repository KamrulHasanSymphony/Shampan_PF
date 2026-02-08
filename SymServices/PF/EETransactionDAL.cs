using SymOrdinary;
using SymServices.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.PF
{
    public class EETransactionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        public List<EETransactionVM> DropDown(string tType = null, int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EETransactionVM> VMs = new List<EETransactionVM>();
            EETransactionVM vm;
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
Id
,Code
   FROM EETransactions
WHERE  1=1
";
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    sqlText += " AND TransactionType = @TransactionType";
                }
                if (branchId > 0)
                {
                    sqlText += " AND BranchId = @BranchId";
                }
                SqlCommand _objComm = new SqlCommand(sqlText, currConn);
                if (!string.IsNullOrWhiteSpace(tType))
                {
                    _objComm.Parameters.AddWithValue("@TransactionType", tType);
                }
                if (branchId > 0)
                {
                    _objComm.Parameters.AddWithValue("@BranchId", branchId);
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EETransactionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
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
        public List<EETransactionVM> SelectAll(int Id = 0, string[] conditionField = null, string[] conditionValue = null, bool IsPS = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EETransactionVM> VMs = new List<EETransactionVM>();
            EETransactionVM vm;
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
                sqlText = @"
SELECT 
tr.Id
,tr.BranchId
,tr.Code
,tr.TransactionDateTime
,tr.GrandTotal
,tr.ReferenceNo1
,tr.ReferenceNo2
,tr.ReferenceNo3
,isnull(tr.Post,0)Post
,tr.TransactionType
,tr.Remarks
,tr.IsActive
,tr.IsArchive
,tr.CreatedBy
,tr.CreatedAt
,tr.CreatedFrom
,tr.LastUpdateBy
,tr.LastUpdateAt
,tr.LastUpdateFrom
From EETransactions tr
Where   1=1
";
                if (!IsPS)
                    sqlText += @" and  tr.IsPS=0";
                if (Id > 0)
                {
                    sqlText += @" and  tr.id=@Id";
                }
                if (conditionField != null)
                {
                    int i = 0;
                    foreach (var item in conditionField)
                    {
                        if (!string.IsNullOrWhiteSpace(conditionValue[i]))
                        {
                            sqlText += " AND tr." + conditionField[i] + "='" + conditionValue[i] + "'";
                            i++;
                        }
                    }
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EETransactionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDateTime = Ordinary.StringToDate(dr["TransactionDateTime"].ToString());
                    vm.GrandTotal = Convert.ToDecimal(dr["GrandTotal"]);
                    vm.ReferenceNo1 = dr["ReferenceNo1"].ToString();
                    vm.ReferenceNo2 = dr["ReferenceNo2"].ToString();
                    vm.ReferenceNo3 = dr["ReferenceNo3"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.TransactionType = dr["TransactionType"].ToString();
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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
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
            return VMs;
        }
        //==================Insert =================
        public string[] Insert(EETransactionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertEETransaction"; //Method Name
            int transResult = 0;
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
                #region Exist
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                //string tableName = "EETransaction";	
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
                #region FiscalYear Check
                //FiscalYearDAL fdal = new FiscalYearDAL();
                //var fp = fdal.PeriodLockByTransactionDate(vm.TransactionDateTime, currConn, transaction);
                //if (fp.PeriodLock)
                //{
                //    retResults[1] = "Fyscal Period is Lock";
                //    throw new ArgumentNullException("Fyscal Period is Lock", "Fyscal Period is Lock");
                //}
                #endregion FiscalYear Check
                vm.Id = Ordinary.NextId("EETransactions", currConn, transaction);
                if (vm != null)
                {
                    vm.Code = "EET-" + (vm.Id.ToString()).PadLeft(4, '0');

                    sqlText = "  ";
                    sqlText += @" INSERT INTO EETransactions(Id
,BranchId
,Code
,TransactionDateTime
,GrandTotal
,ReferenceNo1
,ReferenceNo2
,ReferenceNo3
,TransactionType
,Post
,IsPS
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
VALUES (@Id
,@BranchId
,@Code
,@TransactionDateTime
,@GrandTotal
,@ReferenceNo1
,@ReferenceNo2
,@ReferenceNo3
,@TransactionType
,@Post
,@IsPS
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdInsert.Parameters.AddWithValue("@GrandTotal", vm.GrandTotal);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsPS", vm.IsPS);
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, EETransactionVM.Remarks);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EETransactions.", "");
                    }
                    #region insert Details from Master into Detail Table
                    EETransactionDetailDAL _dDAL = new EETransactionDetailDAL();
                    if (vm.eeTransactionDetailVMs != null && vm.eeTransactionDetailVMs.Count > 0)
                    {
                        int i = 1;
                        foreach (var eeTransactionDVM in vm.eeTransactionDetailVMs)
                        {
                            EETransactionDetailVM dVM = new EETransactionDetailVM();
                            dVM = eeTransactionDVM;
                            dVM.SL = i++;
                            dVM.EETransactionId = vm.Id;
                            dVM.TransactionType = vm.TransactionType;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.IsPS = vm.IsPS;
                            dVM.BranchId = vm.BranchId;
                            dVM.CreatedAt = vm.CreatedAt;
                            dVM.CreatedBy = vm.CreatedBy;
                            dVM.CreatedFrom = vm.CreatedFrom;
                            retResults = _dDAL.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("EETransaction Details", "UnExpected Error.");
                            }
                        }
                    }
                    #endregion insert Details from Master into Detail Table
                    #region Update Grand Total in Header
                    sqlText = " ";
                    sqlText += @"update EETransactions set 
                                GrandTotal=(select sum(SubTotal) from EETransactionDetails  where EETransactionId=@Id)
                                where Id=@Id";
                    SqlCommand cmdGrandTotal = new SqlCommand(sqlText, currConn, transaction);
                    cmdGrandTotal.Parameters.AddWithValue("@Id", vm.Id);
                    exeRes = cmdGrandTotal.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EETransactions.", "");
                    }
                    #endregion Update Grand Total in Header
                }
                else
                {
                    retResults[1] = "This EETransaction already used!";
                    throw new ArgumentNullException("Please Input EETransaction Value", "");
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
        //==================Update =================
        public string[] Update(EETransactionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EETransaction Update"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEETransaction"); }
                #endregion open connection and transaction
                #region FiscalYear Check
                //FiscalYearDAL fdal = new FiscalYearDAL();
                //var fp = fdal.PeriodLockByTransactionDate(vm.TransactionDateTime, currConn, transaction);
                //if (fp.PeriodLock)
                //{
                //    retResults[1] = "Fyscal Period is Lock";
                //    throw new ArgumentNullException("Fyscal Period is Lock", "Fyscal Period is Lock");
                //}
                #endregion FiscalYear Check
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EETransactions set";
                    sqlText += "  Code=@Code";
                    sqlText += " , TransactionDateTime=@TransactionDateTime";
                    sqlText += " , GrandTotal=@GrandTotal";
                    sqlText += " , ReferenceNo1=@ReferenceNo1";
                    sqlText += " , ReferenceNo2=@ReferenceNo2";
                    sqlText += " , ReferenceNo3=@ReferenceNo3";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code);
                    //cmdUpdate.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdUpdate.Parameters.AddWithValue("@TransactionDateTime", Ordinary.DateToString(vm.TransactionDateTime));
                    cmdUpdate.Parameters.AddWithValue("@GrandTotal", vm.GrandTotal);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo1", vm.ReferenceNo1 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo2", vm.ReferenceNo2 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo3", vm.ReferenceNo3 ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);//, EETransactionVM.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EETransactions.", "");
                    }
                    #region insert Details from Master into Detail Table
                    EETransactionDetailDAL _eeTransactionDDal = new EETransactionDetailDAL();
                    if (vm.eeTransactionDetailVMs != null && vm.eeTransactionDetailVMs.Count > 0)
                    {
                        #region Delete Detail
                        try
                        {
                            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "EETransactionDetails", "EETransactionId", currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("EETransaction Details", "could not updated.");
                            }
                        }
                        catch (Exception)
                        {
                            throw new ArgumentNullException("EETransaction Details", "could not updated.");
                        }
                        #endregion Delete Detail
                        #region Insert Detail Again
                        int i = 1;
                        foreach (var eeTransactionDVM in vm.eeTransactionDetailVMs)
                        {
                            EETransactionDetailVM dVM = new EETransactionDetailVM();
                            dVM = eeTransactionDVM;
                            dVM.EETransactionId = vm.Id;
                            dVM.SL = i++;
                            dVM.TransactionType = vm.TransactionType;
                            dVM.TransactionDateTime = vm.TransactionDateTime;
                            dVM.CreatedAt = vm.LastUpdateAt;
                            dVM.CreatedBy = vm.LastUpdateBy;
                            dVM.CreatedFrom = vm.LastUpdateFrom;
                            dVM.BranchId = vm.BranchId;
                            retResults = _eeTransactionDDal.Insert(dVM, currConn, transaction);
                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException("EETransaction Details", "could not updated.");
                            }
                        }
                        #endregion Insert Detail Again
                    }
                    #endregion insert Details from Master into Detail Table
                    #region Update Grand Total in Header
                    sqlText = " ";
                    sqlText += @"update EETransactions set 
                                GrandTotal=(select sum(SubTotal) from EETransactionDetails  where EETransactionId=@Id)
                                where Id=@Id";
                    SqlCommand cmdGrandTotal = new SqlCommand(sqlText, currConn, transaction);
                    cmdGrandTotal.Parameters.AddWithValue("@Id", vm.Id);
                    exeRes = cmdGrandTotal.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update EETransactions.", "");
                    }
                    #endregion Update Grand Total in Header
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EETransactionVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("EETransaction Update", "Could not found any item.");
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
        ////==================Delete =================
        public string[] Delete(EETransactionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEETransaction"; //Method Name
            int transResult = 0;
            string sqlText = "";
            string retVal = "";
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
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEETransaction"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("EETransactions", "Post", "Id", ids[i].ToString(), currConn, transaction);
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
                        sqlText = "";
                        sqlText += " ";
                        sqlText += "DELETE EETransactionDetails";
                        sqlText += " WHERE EETransactionId=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update EETransactionDetails.", "");
                        }
                        sqlText = " ";
                        sqlText = "DELETE EETransactions";
                        sqlText += " WHERE Id=@Id";
                        cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update EETransactions.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EETransaction Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("EETransaction Information Delete", "Could not found any item.");
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
            retResults[5] = "PostEETransaction"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionPF();
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
                        retResults = _cDal.FieldPost("EETransactions", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("EETransactions Post", ids[i] + " could not Post.");
                        }
                        retResults = _cDal.FieldPost("EETransactionDetails", "EETransactionId", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("EETransactionDetails Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("EETransaction Information Post", "Could not found any item.");
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
        public DataTable Report(EETransactionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
tr.Id
,tr.BranchId
,br.Name BranchName
,tr.Code
,tr.TransactionDateTime
,tr.GrandTotal
,tr.ReferenceNo1
,tr.Post
,tr.TransactionType
,tr.Remarks
From EETransactions tr
LEFT OUTER JOIN Branchs br ON tr.BranchId = br.Id
Where   1=1
";
                if (!vm.IsPS)
                    sqlText += @" AND  tr.IsPS=0";

                if (vm.BranchId > 0)
                    sqlText += @" AND  tr.BranchId=@BranchId";
                if (vm.PostStatus == "Posted")
                {
                    sqlText += " AND tr.Post = 1";
                }
                else if (vm.PostStatus == "Not Posted")
                {
                    sqlText += " AND tr.Post = 0";
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
