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
    public class ProfitDistributionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        //==================DropDown=================
        public List<ProfitDistributionVM> DropDown(string tType = "", int branchId = 0)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ProfitDistributionVM> VMs = new List<ProfitDistributionVM>();
            ProfitDistributionVM vm;
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
   FROM ProfitDistributions
WHERE  1=1  AND IsArchive = 0
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProfitDistributionVM();
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


        //==================SelectAll=================
        public List<ProfitDistributionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionVM> VMs = new List<ProfitDistributionVM>();
            ProfitDistributionVM vm;
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
                string hrmDB = _dbsqlConnection.HRMDB;

                #region sql statement
                #region SqlText

                sqlText = @"
SELECT
 pd.Id
,fydFrom.PeriodName PeriodNameFrom
,fydTo.PeriodName PeriodNameTo
,pd.PFDetailFiscalYearDetailIds
,pd.PreDistributionFundIds
,pd.DistributionDate
,pd.FiscalYearDetailId
,pd.TotalEmployeeContribution
,pd.TotalEmployerContribution
,pd.TotalProfit

,pd.FiscalYearDetailIdTo
,pd.TotalExpense
,pd.AvailableDistributionAmount
,pd.TotalWeightedContribution

,pd.MultiplicationFactor

,pd.TransactionType
,ISNULL(pd.IsPaid,0) IsPaid

,pd.Post
,pd.Remarks

,pd.IsActive
,pd.IsArchive
,pd.CreatedBy
,pd.CreatedAt
,pd.CreatedFrom
,pd.LastUpdateBy
,pd.LastUpdateAt
,pd.LastUpdateFrom

FROM ProfitDistributions pd
";
                sqlText = sqlText + @" LEFT OUTER JOIN " + hrmDB + ".[dbo].FiscalYearDetail fydFrom ON pd.FiscalYearDetailId=fydFrom.Id";
                sqlText = sqlText + @" LEFT OUTER JOIN " + hrmDB + ".[dbo].FiscalYearDetail fydTo ON pd.FiscalYearDetailIdTo=fydTo.Id";

                sqlText = sqlText + @" WHERE  1=1 AND pd.IsArchive = 0";


                if (Id > 0)
                {
                    sqlText += @" and pd.Id=@Id";
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
                    vm = new ProfitDistributionVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PeriodNameFrom = dr["PeriodNameFrom"].ToString();
                    vm.PeriodNameTo = dr["PeriodNameTo"].ToString();

                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PFDetailFiscalYearDetailIds = dr["PFDetailFiscalYearDetailIds"].ToString();
                    vm.PreDistributionFundIds = dr["PreDistributionFundIds"].ToString();
                    vm.DistributionDate = Ordinary.StringToDate(dr["DistributionDate"].ToString());
                    vm.TotalEmployeeContribution = Convert.ToDecimal(dr["TotalEmployeeContribution"]);
                    vm.TotalEmployerContribution = Convert.ToDecimal(dr["TotalEmployerContribution"]);
                    vm.TotalProfit = Convert.ToDecimal(dr["TotalProfit"]);

                    vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
                    vm.TotalExpense = Convert.ToDecimal(dr["TotalExpense"]);
                    vm.AvailableDistributionAmount = Convert.ToDecimal(dr["AvailableDistributionAmount"]);

                    vm.TotalWeightedContribution = Convert.ToDecimal(dr["TotalWeightedContribution"]);
                    vm.MultiplicationFactor = Convert.ToDecimal(dr["MultiplicationFactor"]);


                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);

                    vm.TransactionType = dr["TransactionType"].ToString();
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
        public string[] Insert(ProfitDistributionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertProfitDistribution"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            #endregion
            #region Try
            try
            {
                #region Get FiscalYearDetailId
                //////FiscalYearDAL _fiscalYearDAL = new FiscalYearDAL();
                //////FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                //////fydVM = _fiscalYearDAL.PeriodLockByTransactionDate(Ordinary.DateToString(vm.DistributionDate), null, null);
                //////if (fydVM == null)
                //////{
                //////    retResults[1] = "Fiscal year not created!";
                //////    throw new ArgumentNullException(retResults[1], "");
                //////}
                //////vm.FiscalYearDetailId = fydVM.Id;

                #endregion

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

                vm.Id = _cDal.NextId("ProfitDistributions", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO ProfitDistributions(
Id
,PFDetailFiscalYearDetailIds
,PreDistributionFundIds
,DistributionDate
,FiscalYearDetailId
,FiscalYearDetailIdTo
,TotalEmployeeContribution
,TotalEmployerContribution
,TotalProfit

,TotalExpense
,AvailableDistributionAmount
,TotalWeightedContribution
,MultiplicationFactor

,TransactionType
,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@PFDetailFiscalYearDetailIds
,@PreDistributionFundIds
,@DistributionDate
,@FiscalYearDetailId
,@FiscalYearDetailIdTo
,@TotalEmployeeContribution
,@TotalEmployerContribution
,@TotalProfit

,@TotalExpense
,@AvailableDistributionAmount
,@TotalWeightedContribution
,@MultiplicationFactor

,@TransactionType
,@Post
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);



                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@PFDetailFiscalYearDetailIds", vm.PFDetailFiscalYearDetailIds);
                    cmdInsert.Parameters.AddWithValue("@PreDistributionFundIds", vm.PreDistributionFundIds);
                    cmdInsert.Parameters.AddWithValue("@DistributionDate", Ordinary.DateToString(vm.DistributionDate));
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@TotalEmployeeContribution", vm.TotalEmployeeContribution);
                    cmdInsert.Parameters.AddWithValue("@TotalEmployerContribution", vm.TotalEmployerContribution);
                    cmdInsert.Parameters.AddWithValue("@TotalProfit", vm.TotalProfit);

                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                    cmdInsert.Parameters.AddWithValue("@TotalExpense", vm.TotalExpense);
                    cmdInsert.Parameters.AddWithValue("@AvailableDistributionAmount", vm.AvailableDistributionAmount);
                    cmdInsert.Parameters.AddWithValue("@TotalWeightedContribution", vm.TotalWeightedContribution);

                    cmdInsert.Parameters.AddWithValue("@MultiplicationFactor", vm.MultiplicationFactor);


                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdInsert.Parameters.AddWithValue("@Post", vm.Post);
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
                        throw new ArgumentNullException("Unexpected error to update ProfitDistributions.", "");
                    }

                    #endregion SqlExecution
                    #region  Update Sources
                    #region Update ProfitDistributions IsDistribute True
                    {
                        string OtherIds = vm.PreDistributionFundIds.Replace('~', ',');
                        OtherIds = OtherIds.Trim(',');

                        sqlText = "";
                        sqlText = "UPDATE PreDistributionFunds set";
                        sqlText += " IsDistribute=1";

                        sqlText += " where Id In (" + OtherIds + " )";
                        SqlCommand cmdIsDistribute = new SqlCommand(sqlText, currConn, transaction);
                        exeRes = cmdIsDistribute.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update PreDistributionFunds.", "");
                        }
                    }
                    #endregion
                    {
                        #region Update PFDetails IsDistribute True
                        string OtherIds = vm.PFDetailFiscalYearDetailIds.Replace('~', ',');
                        OtherIds = OtherIds.Trim(',');

                        sqlText = "";
                        sqlText = "UPDATE PFDetails set";
                        sqlText += " IsDistribute=1";

                        sqlText += " where FiscalYearDetailId In (" + OtherIds + " )";
                        SqlCommand cmdIsDistribute = new SqlCommand(sqlText, currConn, transaction);
                        exeRes = cmdIsDistribute.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);

                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update PFDetails.", "");
                        }
                        #endregion
                    }

                    #endregion  Update Sources

                    #region insert Details from Master into Detail Table
                    ProfitDistributionDetailDAL _dDAL = new ProfitDistributionDetailDAL();
                    List<ProfitDistributionDetailVM> VMs = new List<ProfitDistributionDetailVM>();

                    #region Detail
                    {
                        if (vm.profitDistributionDetailVMs != null && vm.profitDistributionDetailVMs.Count > 0)
                        {
                            foreach (var detailVM in vm.profitDistributionDetailVMs)
                            {
                                ProfitDistributionDetailVM dVM = new ProfitDistributionDetailVM();
                                dVM = detailVM;
                                dVM.ProfitDistributionId = vm.Id;
                                dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                                dVM.FiscalYearDetailIdTo = vm.FiscalYearDetailIdTo;

                                dVM.CreatedBy = vm.CreatedBy;
                                dVM.CreatedAt = vm.CreatedAt;
                                dVM.CreatedFrom = vm.CreatedFrom;
                                VMs.Add(dVM);
                            }

                            if (VMs != null && VMs.Count > 0)
                            {
                                retResults = _dDAL.Insert(VMs, currConn, transaction);
                                if (retResults[0] == "Fail")
                                {
                                    throw new ArgumentNullException(retResults[1], "");
                                }
                            }

                        }
                    }
                    #endregion
                    #endregion

                }
                else
                {
                    retResults[1] = "This ProfitDistribution already used!";
                    throw new ArgumentNullException("Please Input ProfitDistribution Value", "");
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
        public string[] Update(ProfitDistributionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee ProfitDistribution Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            try
            {
                #region Get FiscalYearDetailId
                //////FiscalYearDAL _fiscalYearDAL = new FiscalYearDAL();
                //////FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                //////fydVM = _fiscalYearDAL.PeriodLockByTransactionDate(Ordinary.DateToString(vm.DistributionDate), null, null);
                //////if (fydVM == null)
                //////{
                //////    retResults[1] = "Fiscal year not created!";
                //////    throw new ArgumentNullException(retResults[1], "");
                //////}
                //////vm.FiscalYearDetailId = fydVM.Id;

                #endregion

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToProfitDistribution"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    #region SqlText
                    sqlText = "";
                    sqlText = "UPDATE ProfitDistributions SET";
                    sqlText += "   DistributionDate=@DistributionDate";

                    //////sqlText += "   PFDetailFiscalYearDetailIds=@PFDetailFiscalYearDetailIds";
                    //////sqlText += " , PreDistributionFundIds=@PreDistributionFundIds";
                    //////sqlText += " , FiscalYearDetailId=@FiscalYearDetailId";
                    //////sqlText += " , TotalEmployeeContribution=@TotalEmployeeContribution";
                    //////sqlText += " , TotalEmployerContribution=@TotalEmployerContribution";
                    //////sqlText += " , TotalProfit=@TotalProfit";
                    //////sqlText += " , TransactionType=@TransactionType";

                    sqlText += " , Post=@Post";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@DistributionDate", Ordinary.DateToString(vm.DistributionDate));

                    ////////cmdUpdate.Parameters.AddWithValue("@PFDetailFiscalYearDetailIds", vm.PFDetailFiscalYearDetailIds);
                    ////////cmdUpdate.Parameters.AddWithValue("@PreDistributionFundIds", vm.PreDistributionFundIds);
                    ////////cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    ////////cmdUpdate.Parameters.AddWithValue("@TotalEmployeeContribution", vm.TotalEmployeeContribution);
                    ////////cmdUpdate.Parameters.AddWithValue("@TotalEmployerContribution", vm.TotalEmployerContribution);
                    ////////cmdUpdate.Parameters.AddWithValue("@TotalProfit", vm.TotalProfit);
                    ////////cmdUpdate.Parameters.AddWithValue("@TransactionType", vm.TransactionType);



                    cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);
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
                        throw new ArgumentNullException("Unexpected error to update ProfitDistributions.", "");
                    }
                    #endregion SqlExecution

                    #region Comments

                    ////#region insert Details from Master into Detail Table
                    ////ProfitDistributionDetailDAL _dDAL = new ProfitDistributionDetailDAL();
                    ////List<ProfitDistributionDetailVM> VMs = new List<ProfitDistributionDetailVM>();


                    ////#region Detail
                    ////{
                    ////    if (vm.profitDistributionDetailVMs != null && vm.profitDistributionDetailVMs.Count > 0)
                    ////    {
                    ////        #region Delete Detail
                    ////        try
                    ////        {
                    ////            retResults = _cDal.DeleteTableInformation(vm.Id.ToString(), "ProfitDistributionDetails", "ProfitDistributionId", currConn, transaction);
                    ////            if (retResults[0] == "Fail")
                    ////            {
                    ////                throw new ArgumentNullException(retResults[1], "");
                    ////            }
                    ////        }
                    ////        catch (Exception)
                    ////        {
                    ////            throw new ArgumentNullException(retResults[1], "");
                    ////        }
                    ////        #endregion Delete Detail
                    ////        #region Insert Detail Again

                    ////        foreach (var detailVM in vm.profitDistributionDetailVMs)
                    ////        {
                    ////            ProfitDistributionDetailVM dVM = new ProfitDistributionDetailVM();
                    ////            dVM = detailVM;
                    ////            dVM.ProfitDistributionId = vm.Id;
                    ////            dVM.FiscalYearDetailId = vm.FiscalYearDetailId;
                    ////            dVM.CreatedBy = vm.LastUpdateBy;
                    ////            dVM.CreatedAt = vm.LastUpdateAt;
                    ////            dVM.CreatedFrom = vm.LastUpdateFrom;
                    ////            VMs.Add(dVM);
                    ////        }

                    ////        if (VMs != null && VMs.Count > 0)
                    ////        {
                    ////            retResults = _dDAL.Insert(VMs, currConn, transaction);
                    ////            if (retResults[0] == "Fail")
                    ////            {
                    ////                throw new ArgumentNullException(retResults[1], "");
                    ////            }
                    ////        }
                    ////        #endregion
                    ////    }
                    ////}
                    ////#endregion

                    ////#endregion

                    #endregion

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("ProfitDistribution Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ProfitDistribution Update", "Could not found any item.");
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
        public string[] Delete(ProfitDistributionVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteProfitDistribution"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToProfitDistribution"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("ProfitDistributions", "Post", "Id", ids[i].ToString(), currConn, transaction);
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
                        #region SELECT ProfitDistributions
                        ProfitDistributionVM profitDistributionVM = new ProfitDistributionVM();
                        profitDistributionVM = SelectAll(Convert.ToInt32(ids[i])).FirstOrDefault();

                        #endregion

                        #region  Update Sources False
                        #region Update ProfitDistributions IsDistribute  False
                        {
                            string OtherIds = profitDistributionVM.PreDistributionFundIds;
                            OtherIds = OtherIds.Trim('~');
                            OtherIds = OtherIds.Replace("~", "','");

                            sqlText = "";
                            sqlText = "UPDATE PreDistributionFunds set";
                            sqlText += " IsDistribute=0";

                            sqlText += " where Id In ('" + OtherIds + "')";
                            SqlCommand cmdIsDistribute = new SqlCommand(sqlText, currConn, transaction);
                            var exeRes = cmdIsDistribute.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            if (transResult <= 0)
                            {
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Unexpected error to update ProfitDistributions.", "");
                            }
                        }
                        #endregion
                        #region Update PFDetails IsDistribute  False
                        {
                            string OtherIds = profitDistributionVM.PFDetailFiscalYearDetailIds;
                            OtherIds = OtherIds.Trim('~');
                            OtherIds = OtherIds.Replace("~", "','");

                            sqlText = "";
                            sqlText = "UPDATE PFDetails set";
                            sqlText += " IsDistribute=0";

                            sqlText += " WHERE FiscalYearDetailId IN ('" + OtherIds + "')";
                            SqlCommand cmdIsDistribute = new SqlCommand(sqlText, currConn, transaction);
                            var exeRes = cmdIsDistribute.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            if (transResult <= 0)
                            {
                                retResults[3] = sqlText;
                                throw new ArgumentNullException("Unexpected error to update PFDetails.", "");
                            }
                        }
                        #endregion Update ReturnOnInvestments IsTransferPDF False


                        #endregion
                        #region DELETE ProfitDistributions
                        sqlText = " ";
                        sqlText += " DELETE ProfitDistributionDetails WHERE ProfitDistributionId=@Id";
                        sqlText += " DELETE ProfitDistributions WHERE Id=@Id";
                        sqlText += " ";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeResult = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeResult);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to Delete ProfitDistributions.", "");
                        }
                        #endregion DELETE ProfitDistributions
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("ProfitDistribution Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("ProfitDistribution Information Delete", "Could not found any item.");
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
            retResults[5] = "PostProfitDistribution"; //Method Name
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
                        retResults = _cDal.FieldPost("ProfitDistributionDetails", "ProfitDistributionId", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("ProfitDistributionDetails Post", ids[i] + " could not Post.");
                        }

                        retResults = _cDal.FieldPost("ProfitDistributions", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("ProfitDistributions Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings

                }
                else
                {
                    throw new ArgumentNullException("ProfitDistribution Information Post", "Could not found any item.");
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

        ////==================Payment =================
        public string[] Payment(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PaymentProfitDistribution"; //Method Name
            string sqlText = "";
            int transResult = 0;

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
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = @"
Update ProfitDistributionDetails SET IsPaid=1 WHERE ProfitDistributionId=@Id
Update ProfitDistributions SET IsPaid=1 WHERE Id=@Id

";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update ProfitDistributions.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit
                    #endregion Delete Settings

                    if (Vtransaction == null && transaction != null)
                    {
                        transaction.Commit();
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Payment Done Successfully!";
                    #endregion Commit

                }
                else
                {
                    throw new ArgumentNullException("ProfitDistribution Information Post", "Could not found any item.");
                }
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


        public ProfitDistributionVM PreSelect(ProfitDistributionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            #endregion
            try
            {

                #region Required Data

                SettingDAL _sDAL = new SettingDAL();
                string EntitleDate = _sDAL.settingValue("WPPF", "EntitleDate");

                EntitleDate = Ordinary.StringToDate(EntitleDate);

                string[] cFields = { "Id>", "Id<" };
                string[] cValues = { vm.FiscalYearDetailId.ToString(), vm.FiscalYearDetailIdTo.ToString() };
                string PFDetailFiscalYearDetailIds = "";

                PFDetailFiscalYearDetailIds = _cDal.SelectSingleRowFromMultipleRows("FiscalYearDetail", "Id", cFields, cValues, currConn, transaction);


                FiscalYearDAL _fyDAL = new FiscalYearDAL();
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                string PeriodStart = "";
                string PeriodEnd = "";

                string PeriodNameFrom = "";
                string PeriodNameTo = "";

                fydVM = _fyDAL.SelectAll_FiscalYearDetail(vm.FiscalYearDetailId).FirstOrDefault();
                PeriodStart = fydVM.PeriodStart;
                PeriodNameFrom = fydVM.PeriodName;


                fydVM = new FiscalYearDetailVM();
                fydVM = _fyDAL.SelectAll_FiscalYearDetail(vm.FiscalYearDetailIdTo).FirstOrDefault();
                PeriodEnd = fydVM.PeriodEnd;
                PeriodNameTo = fydVM.PeriodName;


                vm.PeriodNameFrom = PeriodNameFrom;
                vm.PeriodNameTo = PeriodNameTo;

                #endregion

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

                #region Get Data from PFDetailDAL
                if (string.IsNullOrWhiteSpace(PFDetailFiscalYearDetailIds))
                {
                    return vm;
                }

                PFDetailDAL _pfDetailDAL = new PFDetailDAL();

                PFDetailVM pfDetailVM = new PFDetailVM();
                List<PFDetailVM> pfDetailVMs = new List<PFDetailVM>();

                pfDetailVM = _pfDetailDAL.SelectTotalContribution_TillMonth(vm.FiscalYearDetailIdTo, currConn, transaction);
                pfDetailVMs = _pfDetailDAL.SelectDetailContribution_TillMonth(vm.FiscalYearDetailIdTo, "", currConn, transaction);

                if (pfDetailVM.TotalEmployeeContribution == 0 || pfDetailVM.TotalEmployerContribution == 0 || pfDetailVMs.Count() == 0)
                {
                    return vm;
                }

                #endregion

                #region Assign Contribution Data in ProfitDistributionVM (Master)
                vm.PFDetailFiscalYearDetailIds = PFDetailFiscalYearDetailIds;

                vm.TotalEmployeeContribution = pfDetailVM.TotalEmployeeContribution;
                vm.TotalEmployerContribution = pfDetailVM.TotalEmployerContribution;

                #endregion

                #region Total Profit
                string PreDistributionFundIds = vm.PreDistributionFundIds.Replace('~', ',');
                PreDistributionFundIds = PreDistributionFundIds.Trim(',');

                sqlText = "";
                sqlText = @"
select ISNULL(SUM(TotalFundingValue),0) TotalFundingValue from PreDistributionFunds p

where 1=1
and p.Post = 1
and p.IsDistribute = 0

";
                sqlText += " and p.Id In (" + PreDistributionFundIds + " )";

                {
                    SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                    SqlDataReader dr;
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        vm.TotalProfit = Convert.ToDecimal(dr["TotalFundingValue"]);
                    }
                    dr.Close();
                }


                #endregion

                //Get Total Profit
                //Get Total Expense
                //AvailableDistributionAmount

                #region Total Expense
                sqlText = "";
                sqlText = @"
----------declare @fromDate as varchar(14)
----------declare @toDate as varchar(14)
----------
----------set @fromDate = '20180101' 
----------set @toDate = '20191231' 



select ISNULL(SUM(SubTotal),0) SubTotal from EETransactionDetails d
LEFT OUTER JOIN EETransactions m ON d.EETransactionId = m.Id
where 1=1
and m.Post = 1
and m.TransactionDateTime >= @fromDate and m.TransactionDateTime <= @toDate

";
                {
                    SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                    objComm.Parameters.AddWithValue("@fromDate", PeriodStart);
                    objComm.Parameters.AddWithValue("@toDate", PeriodEnd);

                    SqlDataReader dr;
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        vm.TotalExpense = Convert.ToDecimal(dr["SubTotal"]);
                    }
                    dr.Close();
                }

                #endregion

                #region AvailableDistributionAmount

                vm.AvailableDistributionAmount = vm.TotalProfit - vm.TotalExpense;

                #endregion

                #region TillDate/Cut-off Date/PeriodEnd

                DateTime DateTillDate = Convert.ToDateTime(Ordinary.StringToDate(PeriodEnd));

                #endregion

                DateTime DateEntitleDate = Convert.ToDateTime(EntitleDate);

                #region Assign Contribution Data in ProfitDistributionDetailVM (Detail)
                List<ProfitDistributionDetailVM> profitDistributionDetailVMs = new List<ProfitDistributionDetailVM>();


                foreach (PFDetailVM detailVM in pfDetailVMs)
                {

                    #region Debugging
                    if (detailVM.EmployeeId == "1_91")
                    {

                    }

                    #endregion


                    ProfitDistributionDetailVM profitDistributionDetailVM = new ProfitDistributionDetailVM();
                    profitDistributionDetailVM.EmpName = detailVM.EmpName;
                    profitDistributionDetailVM.Code = detailVM.Code;
                    profitDistributionDetailVM.Designation = detailVM.Designation;
                    profitDistributionDetailVM.Department = detailVM.Department;
                    profitDistributionDetailVM.Section = detailVM.Section;
                    profitDistributionDetailVM.Project = detailVM.Project;
                    profitDistributionDetailVM.JoinDate = detailVM.JoinDate;
                    profitDistributionDetailVM.DateOfPermanent = detailVM.DateOfPermanent;
                    profitDistributionDetailVM.PFStartDate = detailVM.PFStartDate;


                    DateTime PFStartDate = Convert.ToDateTime(profitDistributionDetailVM.PFStartDate);

                    if (PFStartDate < DateEntitleDate)
                    {
                        ////int DayDiff = DateTillDate.Day - DateEntitleDate.Day;
                        ////int MonthFromDays = Convert.ToInt32(Math.Round(DayDiff/ 30M));
                        profitDistributionDetailVM.ServiceLengthMonth = ((DateTillDate.Year - DateEntitleDate.Year) * 12) + (DateTillDate.Month - DateEntitleDate.Month) + 1;//// + MonthFromDays;

                    }
                    else
                    {
                        ////int DayDiff = DateTillDate.Day - PFStartDate.Day;
                        ////int MonthFromDays = Convert.ToInt32(Math.Round(DayDiff/ 30M));
                        profitDistributionDetailVM.ServiceLengthMonth = ((DateTillDate.Year - PFStartDate.Year) * 12) + (DateTillDate.Month - PFStartDate.Month) + 1;//// + MonthFromDays;
                    }

                    profitDistributionDetailVM.ServiceLengthMonthWeight = profitDistributionDetailVM.ServiceLengthMonth / 12;


                    profitDistributionDetailVM.ProjectId = detailVM.ProjectId;
                    profitDistributionDetailVM.DepartmentId = detailVM.DepartmentId;
                    profitDistributionDetailVM.SectionId = detailVM.SectionId;
                    profitDistributionDetailVM.DesignationId = detailVM.DesignationId;
                    profitDistributionDetailVM.EmployeeId = detailVM.EmployeeId;
                    profitDistributionDetailVM.EmployeeTotalContribution = detailVM.EmployeeTotalContribution;
                    profitDistributionDetailVM.EmployerTotalContribution = detailVM.EmployerTotalContribution;

                    profitDistributionDetailVM.IndividualTotalContribution = profitDistributionDetailVM.EmployeeTotalContribution + profitDistributionDetailVM.EmployerTotalContribution;

                    profitDistributionDetailVM.IndividualWeightedContribution = profitDistributionDetailVM.IndividualTotalContribution * profitDistributionDetailVM.ServiceLengthMonthWeight;





                    //////profitDistributionDetailVM.EmployeeProfitValue = MultiplicationFactor * detailVM.EmployeeTotalContribution;
                    //////profitDistributionDetailVM.EmployerProfitValue = MultiplicationFactor * detailVM.EmployerTotalContribution;


                    profitDistributionDetailVMs.Add(profitDistributionDetailVM);
                }

                #endregion

                #region Employee Profit

                decimal TotalWeightedContribution = profitDistributionDetailVMs.Sum(item => item.IndividualWeightedContribution);


                vm.MultiplicationFactor = vm.AvailableDistributionAmount / TotalWeightedContribution;

                vm.TotalWeightedContribution = TotalWeightedContribution;




                foreach (ProfitDistributionDetailVM item in profitDistributionDetailVMs)
                {
                    item.IndividualProfitValue = item.IndividualWeightedContribution * vm.MultiplicationFactor;
                    item.EmployeeProfitValue = item.IndividualProfitValue / 2;
                    item.EmployerProfitValue = item.IndividualProfitValue / 2;
                    item.MultiplicationFactor = vm.MultiplicationFactor;
                }


                #endregion




                vm.profitDistributionDetailVMs = profitDistributionDetailVMs;



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


        ////==================Report=================
        public DataTable Report(ProfitDistributionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
fydFrom.PeriodName PeriodFrom
,fydTo.PeriodName PeriodTo
,pd.DistributionDate
,pd.TotalEmployeeContribution
,pd.TotalEmployerContribution
,pd.TotalProfit

,pd.TotalExpense
,pd.AvailableDistributionAmount
,pd.TotalWeightedContribution
,pd.MultiplicationFactor

FROM ProfitDistributions pd

LEFT OUTER JOIN hrmDB.[dbo].FiscalYearDetail fydFrom ON pd.FiscalYearDetailId=fydFrom.Id
LEFT OUTER JOIN hrmDB.[dbo].FiscalYearDetail fydTo ON pd.FiscalYearDetailIdTo=fydTo.Id

WHERE  1=1
";
                sqlText = Ordinary.StringSafeReplace(sqlText, "hrmDB", hrmDB, true);

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

                dt = Ordinary.DtColumnStringToDate(dt,"DistributionDate");

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

        public DataTable Report_Detail(ProfitDistributionVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
ve.Code 
,ve.EmpName 
,pdd.EmployeeTotalContribution
,pdd.EmployerTotalContribution
,pdd.IndividualTotalContribution
,pdd.ServiceLengthMonth
,pdd.ServiceLengthMonthWeight
,pdd.IndividualWeightedContribution
,pdd.MultiplicationFactor
,pdd.IndividualProfitValue
,pdd.EmployeeProfitValue
,pdd.EmployerProfitValue

FROM  ProfitDistributionDetails  pdd
LEFT OUTER JOIN BracEPLHRM.[dbo].ViewEmployeeInformation ve ON pdd.EmployeeId=ve.EmployeeId
WHERE  1=1
--------AND pdd.ProfitDistributionId = 1
";
                sqlText = Ordinary.StringSafeReplace(sqlText, "hrmDB", hrmDB, true);

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





    }
}
