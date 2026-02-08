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
    public class ReturnOnInvestmentDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAllNotTransferPDF=================
        public List<ReturnOnInvestmentVM> SelectAllNotTransferPDF(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
             , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ReturnOnInvestmentVM> VMs = new List<ReturnOnInvestmentVM>();
            ReturnOnInvestmentVM vm;
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
roi.Id
,roi.InvestmentId
,roi.ROIDate
,roi.ROIRate
,roi.ROITotalValue
,roi.TotalInterestValue
,roi.Remarks
,roi.Post
,roi.IsActive
,roi.IsArchive
,roi.CreatedBy
,roi.CreatedAt
,roi.CreatedFrom
,roi.LastUpdateBy
,roi.LastUpdateAt
,roi.LastUpdateFrom

,et.Name InvestmentType
,ISNULL(roi.IsTransferPDF, 0) IsTransferPDF

FROM ReturnOnInvestments roi
LEFT OUTER JOIN Investments inv ON roi.InvestmentId = inv.Id
LEFT OUTER JOIN EnumInvestmentTypes et ON inv.InvestmentTypeId = et.Id
WHERE  1=1 AND roi.IsTransferPDF = 0 AND roi.Post = 1
";

                if (Id > 0)
                {
                    sqlText += @" and roi.Id=@Id";
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
                    vm = new ReturnOnInvestmentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.InvestmentId = Convert.ToInt32(dr["InvestmentId"]);
                    vm.ROIDate = Ordinary.StringToDate(dr["ROIDate"].ToString());
                    vm.ROIRate = Convert.ToDecimal(dr["ROIRate"]);
                    vm.ROITotalValue = Convert.ToDecimal(dr["ROITotalValue"]);
                    vm.TotalInterestValue = Convert.ToDecimal(dr["TotalInterestValue"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.InvestmentType = dr["InvestmentType"].ToString();
                    vm.IsTransferPDF = Convert.ToBoolean(dr["IsTransferPDF"]);


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

        //==================SelectAll=================
        public List<ReturnOnInvestmentVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
             , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ReturnOnInvestmentVM> VMs = new List<ReturnOnInvestmentVM>();
            ReturnOnInvestmentVM vm;
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
roi.Id
,roi.TransactionCode
,roi.TransactionType
,roi.ReferenceId

,roi.InvestmentId
,ISNULL(roi.IsFixed,0) IsFixed
,inv.ReferenceNo
,ISNULL(inv.InvestmentTypeId,0)InvestmentTypeId
,eit.Name InvestmentType
,roi.ROIDate
,roi.ROIRate
,roi.ROITotalValue
,roi.ActualInterestAmount
,roi.ServiceChargeAmount
,roi.TotalInterestValue
,ISNULL(roi.IsBankDeposited,0) IsBankDeposited

,roi.Remarks
,roi.Post
,roi.IsActive
,roi.IsArchive
,roi.CreatedBy
,roi.CreatedAt
,roi.CreatedFrom
,roi.LastUpdateBy
,roi.LastUpdateAt
,roi.LastUpdateFrom

,ISNULL(roi.IsTransferPDF, 0) IsTransferPDF

FROM ReturnOnInvestments roi
LEFT OUTER JOIN Investments inv ON roi.InvestmentId = inv.Id
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id
WHERE  1=1
";

                if (Id > 0)
                {
                    sqlText += @" and roi.Id=@Id";
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
                    vm = new ReturnOnInvestmentVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);

                    vm.TransactionCode = Convert.ToString(dr["TransactionCode"]);
                    vm.TransactionType = Convert.ToString(dr["TransactionType"]);
                    vm.ReferenceId = Convert.ToInt32(dr["ReferenceId"]);


                    vm.InvestmentId = Convert.ToInt32(dr["InvestmentId"]);
                    vm.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.InvestmentTypeId = Convert.ToInt32(dr["InvestmentTypeId"]);
                    vm.InvestmentType = dr["InvestmentType"].ToString();
                    vm.ROIDate = Ordinary.StringToDate(dr["ROIDate"].ToString());
                    vm.ROIRate = Convert.ToDecimal(dr["ROIRate"]);
                    vm.ROITotalValue = Convert.ToDecimal(dr["ROITotalValue"]);
                    vm.ActualInterestAmount = Convert.ToDecimal(dr["ActualInterestAmount"]);
                    vm.ServiceChargeAmount = Convert.ToDecimal(dr["ServiceChargeAmount"]);
                    vm.TotalInterestValue = Convert.ToDecimal(dr["TotalInterestValue"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);

                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.InvestmentType = dr["InvestmentType"].ToString();
                    vm.IsTransferPDF = Convert.ToBoolean(dr["IsTransferPDF"]);


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

        //==================PreInsert=================
        public ReturnOnInvestmentVM PreInsert(ReturnOnInvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                sqlText = "";

                #region ReturnOnInvestment

                InvestmentVM invVM = new InvestmentVM();
                InvestmentDAL _invDAL = new InvestmentDAL();
                invVM = _invDAL.SelectAll(vm.ReferenceId, null, null, currConn, transaction).FirstOrDefault();
                vm.ROITotalValue = invVM.InvestmentValue;
                vm.InvestmentId = invVM.Id;
                vm.InvestmentTypeId = invVM.InvestmentTypeId;
                vm.ReferenceNo = invVM.ReferenceNo;

                #endregion

                #region Detail

                #region SqlText
                sqlText = "";
                #region ROIDetail
                sqlText = "";
                sqlText = @"
------declare @InvestmentId int = 1

select        
0 AccountId
, inv.InvestmentValue DebitAmount
, 0 CreditAmount
,'Asset' Remarks
,'Asset' AccountType
from Investments inv
where 1=1 and inv.Id = @InvestmentId


UNION ALL

select        
0 AccountId
,0 DebitAmount
,0 CreditAmount
,'Income' Remarks
,'Income' AccountType

UNION ALL

select * from
(
select top 1
ind.AccountId
, 0 DebitAmount
, ind.DebitAmount CreditAmount
,ind.Remarks
,acc.AccountType
from InvestmentDetails ind
left outer join Accounts acc on ind.AccountId=acc.Id
where 1=1
and ind.InvestmentId = @InvestmentId
order by ind.Id
) as credit

";
                #endregion
                #endregion

                if (!string.IsNullOrWhiteSpace(sqlText))
                {
                    #region SqlExecution
                    SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                    da.SelectCommand.Transaction = transaction;
                    da.SelectCommand.Parameters.AddWithValue("@InvestmentId", invVM.Id);


                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    List<ROIDetailVM> detailVMs = new List<ROIDetailVM>();
                    ROIDetailVM detailVM = new ROIDetailVM();
                    foreach (DataRow ddr in dt.Rows)
                    {
                        detailVM = new ROIDetailVM();
                        detailVM.AccountId = Convert.ToInt32(ddr["AccountId"]);
                        detailVM.DebitAmount = Convert.ToDecimal(ddr["DebitAmount"]);
                        detailVM.CreditAmount = Convert.ToDecimal(ddr["CreditAmount"]);
                        detailVM.Remarks = Convert.ToString(ddr["Remarks"]);
                        detailVM.AccountType = Convert.ToString(ddr["AccountType"]);

                        detailVMs.Add(detailVM);
                    }

                    vm.detailVMs = detailVMs;

                    #endregion SqlExecution
                }


                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return vm;
        }

        //==================Insert =================
        public string[] Insert(ReturnOnInvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertReturnOnInvestment"; //Method Name
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


                vm.Id = _cDal.NextId("ReturnOnInvestments", currConn, transaction);
                if (vm != null)
                {
                    vm.TransactionCode = "ROI-" + (vm.Id.ToString()).PadLeft(4, '0');

                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO ReturnOnInvestments(
Id
,TransactionCode
,TransactionType
,ReferenceId

,InvestmentId
,IsFixed
,InvestmentTypeId
,ROIDate
,ROIRate
,ROITotalValue
,ActualInterestAmount
,ServiceChargeAmount
,TotalInterestValue
,Post
,IsTransferPDF
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@TransactionCode
,@TransactionType
,@ReferenceId

,@InvestmentId
,@IsFixed
,@InvestmentTypeId
,@ROIDate
,@ROIRate
,@ROITotalValue
,@ActualInterestAmount
,@ServiceChargeAmount
,@TotalInterestValue
,@Post
,@IsTransferPDF
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion

                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdInsert.Parameters.AddWithValue("@ReferenceId", vm.ReferenceId);


                    cmdInsert.Parameters.AddWithValue("@InvestmentId", vm.InvestmentId);
                    cmdInsert.Parameters.AddWithValue("@IsFixed", vm.IsFixed);

                    cmdInsert.Parameters.AddWithValue("@InvestmentTypeId", vm.InvestmentTypeId);
                    cmdInsert.Parameters.AddWithValue("@ROIDate", Ordinary.DateToString(vm.ROIDate));
                    cmdInsert.Parameters.AddWithValue("@ROIRate", vm.ROIRate);
                    cmdInsert.Parameters.AddWithValue("@ROITotalValue", vm.ROITotalValue);

                    cmdInsert.Parameters.AddWithValue("@ActualInterestAmount", vm.ActualInterestAmount);
                    cmdInsert.Parameters.AddWithValue("@ServiceChargeAmount", vm.ServiceChargeAmount);

                    cmdInsert.Parameters.AddWithValue("@TotalInterestValue", vm.TotalInterestValue);
                    cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
                    cmdInsert.Parameters.AddWithValue("@IsTransferPDF", vm.IsTransferPDF);
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
                        throw new ArgumentNullException("Unexpected error to update ReturnOnInvestments.", "");
                    }
                    #endregion

                    #region Detail
                    {
                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                        {
                            ROIDetailDAL _detailDAL = new ROIDetailDAL();

                            foreach (ROIDetailVM detailVM in vm.detailVMs)
                            {
                                detailVM.ROIId = vm.Id;
                                detailVM.TransactionDate = vm.ROIDate;
                                detailVM.TransactionType = vm.TransactionType;
                            }

                            retResults = _detailDAL.Insert(vm.detailVMs, currConn, transaction);

                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }

                        }

                    }
                    #endregion

                    #region GLTransactionDetail
                    {
                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                        {
                            GLTransactionDetailDAL _detailDAL = new GLTransactionDetailDAL();
                            List<GLTransactionDetailVM> detailVMs = new List<GLTransactionDetailVM>();
                            GLTransactionDetailVM dVM = new GLTransactionDetailVM();

                            foreach (ROIDetailVM detailVM in vm.detailVMs)
                            {
                                dVM = new GLTransactionDetailVM();
                                dVM.TransactionMasterId = vm.Id;
                                dVM.TransactionCode = vm.TransactionCode;
                                dVM.TransactionDate = vm.ROIDate;
                                dVM.TransactionType = vm.TransactionType;
                                dVM.AccountId = detailVM.AccountId;
                                dVM.DebitAmount = detailVM.DebitAmount;
                                dVM.CreditAmount = detailVM.CreditAmount;
                                dVM.TransactionAmount = 0;////vm.DepositAmount;
                                dVM.Post = vm.Post;
                                dVM.Remarks = detailVM.Remarks;
                                dVM.CreatedBy = vm.CreatedBy;
                                dVM.CreatedAt = vm.CreatedAt;
                                dVM.CreatedFrom = vm.CreatedFrom;

                                detailVMs.Add(dVM);
                            }

                            retResults = _detailDAL.InsertX(detailVMs, currConn, transaction);

                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }

                        }
                    }
                    #endregion

                }
                else
                {
                    retResults[1] = "This ReturnOnInvestment already used!";
                    throw new ArgumentNullException("Please Input ReturnOnInvestment Value", "");
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
        public string[] Update(ReturnOnInvestmentVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee ReturnOnInvestment Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Update"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings

                    #region SqlText

                    sqlText = "";
                    sqlText = "UPDATE ReturnOnInvestments SET";
                    sqlText += " InvestmentId=@InvestmentId";
                    sqlText += " ,IsFixed=@IsFixed";
                    sqlText += " ,InvestmentTypeId=@InvestmentTypeId";
                    sqlText += " ,ROIDate=@ROIDate";
                    sqlText += " ,ROIRate=@ROIRate";
                    sqlText += " ,ROITotalValue=@ROITotalValue";

                    sqlText += " ,ActualInterestAmount=@ActualInterestAmount";
                    sqlText += " ,ServiceChargeAmount=@ServiceChargeAmount";

                    sqlText += " ,TotalInterestValue=@TotalInterestValue";
                    sqlText += " ,Post=@Post";
                    sqlText += " ,IsTransferPDF=@IsTransferPDF";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";

                    #endregion

                    #region SqlExecution

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentId", vm.InvestmentId);
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", vm.IsFixed);

                    cmdUpdate.Parameters.AddWithValue("@InvestmentTypeId", vm.InvestmentTypeId);
                    cmdUpdate.Parameters.AddWithValue("@ROIDate", Ordinary.DateToString(vm.ROIDate));
                    cmdUpdate.Parameters.AddWithValue("@ROIRate", vm.ROIRate);
                    cmdUpdate.Parameters.AddWithValue("@ROITotalValue", vm.ROITotalValue);

                    cmdUpdate.Parameters.AddWithValue("@ActualInterestAmount", vm.ActualInterestAmount);
                    cmdUpdate.Parameters.AddWithValue("@ServiceChargeAmount", vm.ServiceChargeAmount);

                    cmdUpdate.Parameters.AddWithValue("@TotalInterestValue", vm.TotalInterestValue);
                    cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);
                    cmdUpdate.Parameters.AddWithValue("@IsTransferPDF", vm.IsTransferPDF);

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
                        throw new ArgumentNullException("Unexpected error to update ReturnOnInvestments.", "");
                    }
                    #endregion

                    #region Detail
                    {
                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                        {
                            #region Delete First
                            sqlText = " ";
                            sqlText = "DELETE ROIDetails WHERE ROIId=@Id";
                            SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                            cmdDelete.Parameters.AddWithValue("@Id", vm.Id);
                            exeRes = cmdDelete.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            #endregion

                            #region Insert Again

                            ROIDetailDAL _detailDAL = new ROIDetailDAL();

                            foreach (ROIDetailVM detailVM in vm.detailVMs)
                            {
                                detailVM.ROIId = vm.Id;
                                detailVM.TransactionDate = vm.ROIDate;
                                detailVM.TransactionType = vm.TransactionType;
                            }

                            retResults = _detailDAL.Insert(vm.detailVMs, currConn, transaction);

                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion
                        }
                    }

                    #endregion

                    #region GLTransactionDetail
                    {
                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                        {
                            #region Delete First
                            sqlText = " ";
                            sqlText = "DELETE GLTransactionDetails WHERE TransactionCode=@TransactionCode";
                            SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                            cmdDelete.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);
                            exeRes = cmdDelete.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            #endregion

                            #region Insert Again


                            GLTransactionDetailDAL _detailDAL = new GLTransactionDetailDAL();
                            List<GLTransactionDetailVM> detailVMs = new List<GLTransactionDetailVM>();
                            GLTransactionDetailVM dVM = new GLTransactionDetailVM();

                            foreach (ROIDetailVM detailVM in vm.detailVMs)
                            {
                                dVM = new GLTransactionDetailVM();
                                dVM.TransactionMasterId = vm.Id;
                                dVM.TransactionCode = vm.TransactionCode;

                                dVM.TransactionDate = vm.ROIDate;
                                dVM.TransactionType = vm.TransactionType;
                                dVM.AccountId = detailVM.AccountId;
                                dVM.DebitAmount = detailVM.DebitAmount;
                                dVM.CreditAmount = detailVM.CreditAmount;
                                dVM.TransactionAmount = 0;////vm.DepositAmount;
                                dVM.Post = vm.Post;
                                dVM.Remarks = detailVM.Remarks;
                                dVM.CreatedBy = vm.LastUpdateBy;
                                dVM.CreatedAt = vm.LastUpdateAt;
                                dVM.CreatedFrom = vm.LastUpdateFrom;

                                detailVMs.Add(dVM);
                            }

                            retResults = _detailDAL.InsertX(detailVMs, currConn, transaction);

                            if (retResults[0] == "Fail")
                            {
                                throw new ArgumentNullException(retResults[1], "");
                            }
                            #endregion

                        }
                    }
                    #endregion

                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ReturnOnInvestment Update", "Could not found any item.");
                }
                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
                retResults[2] = vm.Id.ToString();// Return Id
                retResults[3] = sqlText; //  SQL Query
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
        public string[] Delete(ReturnOnInvestmentVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteReturnOnInvestment"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnectionPF();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToReturnOnInvestment"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("ReturnOnInvestments", "Post", "Id", ids[i].ToString(), currConn, transaction);
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
                        sqlText = " ";
                        sqlText = "DELETE ReturnOnInvestments";
                        sqlText += " WHERE Id=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update ReturnOnInvestments.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("ReturnOnInvestment Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("ReturnOnInvestment Information Delete", "Could not found any item.");
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
            retResults[5] = "PostReturnOnInvestment"; //Method Name
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
                        retResults = _cDal.FieldPost("ReturnOnInvestments", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("ReturnOnInvestments Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ReturnOnInvestment Information Post - Could not found any item.", "");
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
        public DataTable Report(ReturnOnInvestmentVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                string hrmDB = _dbsqlConnection.HRMDB;
                #region sql statement
                sqlText = @"
SELECT
roi.Id
,roi.InvestmentId
,inv.ReferenceNo
,ISNULL(inv.InvestmentTypeId,0)InvestmentTypeId
,eit.Name InvestmentType
,roi.ROIDate
,roi.ROIRate
,roi.ROITotalValue
,roi.ActualInterestAmount
,roi.ServiceChargeAmount
,roi.TotalInterestValue
,roi.Remarks
,roi.Post
,ISNULL(roi.IsTransferPDF, 0) IsTransferPDF

FROM ReturnOnInvestments roi
LEFT OUTER JOIN Investments inv ON roi.InvestmentId = inv.Id
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id

WHERE  1=1
";
                //InvestmentId
                //InvestmentTypeId
                //ReferenceNo
                //InvestmentType 
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

                string[] DtColumnChange = { "ROIDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, DtColumnChange);

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



        public string SelectSingleRowFromMultipleRows(string ReferenceIds, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string ReturnValue = "";

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
                string ReturnOnInvestmentIds = ReferenceIds.Replace('~', ',');
                ReturnOnInvestmentIds = ReturnOnInvestmentIds.Trim(',');





                sqlText = "";
                sqlText += @" 
SELECT STUFF((SELECT '~' + CAST(ReferenceNo AS nvarchar(500))
 FROM (SELECT DISTINCT  inv.ReferenceNo from ReturnOnInvestments roi
LEFT OUTER JOIN Investments inv on roi.InvestmentId = inv.Id
where roi.Id in (
";
                sqlText += " and roi.Id In (" + ReturnOnInvestmentIds + " )";

                sqlText += @"
)
) US ORDER BY ReferenceNo
FOR XML PATH('')), 1, 1, '') [ReturnValue]
";

                #endregion SqlText
                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    ReturnValue = Convert.ToString(dr["ReturnValue"]);
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
            return ReturnValue;
        }


        #endregion
    }
}
