using ACCPAC.Advantage;
using SymOrdinary;
using SymServices.Payroll;
using SymViewModel.Payroll;
using SymServices.HRM;
using SymViewModel.HRM;
using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymServices.Payroll
{
        public class SageDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        private Session session;
        string[] Condition = new string[] { "one" };
        SettingDAL _setDAL = new SettingDAL();
        CommonDAL _cdal = new CommonDAL();
        FiscalYearDAL _fyDal = new FiscalYearDAL();
        GLAccountDAL _glAccDal = new GLAccountDAL();
        string UserName = string.Empty;
        string Password = string.Empty;
        string SAGEDB = string.Empty;

        public string[] SageIntegration(List<JournalLedgerDetailVM> getAllData, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {//Post
            #region Initializ
            int transResult = 0;

            string sqlText = "";
            int Id = 0;
            decimal debit = 0;
            decimal credit = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertBank"; //Method Name
            //List<JournalLedgerDetailVM> ledgerVms = new List<JournalLedgerDetailVM>();
            List<JournalLedgerEmployeeHistoryVM> journalLedgerEmployeeHistoryVMs = new List<JournalLedgerEmployeeHistoryVM>();
            JournalLedgerDetailVM ledgerVm = new JournalLedgerDetailVM();
            JournalLedgerVM ledgerHeadVm = new JournalLedgerVM();
          

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string JournalLedgerId = "0";
            #endregion
            #region Try
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
                #region Exist
                var srceType = _setDAL.settingValue("SAGE", "SrceType", currConn, transaction);
                JournalLedgerDetailVM vm = new JournalLedgerDetailVM();
                vm = getAllData.FirstOrDefault();
                var fiscalDetail = _fyDal.FYPeriodDetail(Convert.ToInt32(vm.FiscalYearDetailId), currConn, transaction).FirstOrDefault();
                bool sagePostComplete = Convert.ToBoolean(fiscalDetail.SagePostComplete);

                string desc = "salary for the month of " + fiscalDetail.PeriodName;
                if (sagePostComplete)
                {
                    //retResults[1] = "This Month Sage Posting Already completed";
                    //throw new ArgumentNullException("", "");
                }

                var OutstandingLiabilities1 = _glAccDal.SelectAll(null,currConn, transaction).Where(x => x.OutstandingLiabilities.Equals(true)).FirstOrDefault();

                #endregion Exist
                #region Save

                journalLedgerEmployeeHistoryVMs = JournalLedgerEmployeeHistory(vm.FiscalYearDetailId, vm.DepartmentId, vm.SectionId, vm.ProjectId,   vm.IsReverse, currConn, transaction);


                if (getAllData.Count > 0)
                {
                    #region Insert into JournalLedger Table
                    #region Sql
                    //string EmpCodes = "";
                    List<ViewEmployeeInfoVM> empInfo = new List<ViewEmployeeInfoVM>();
                    EmployeeInfoDAL _empDAL = new EmployeeInfoDAL();

                    ledgerHeadVm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                    ledgerHeadVm.CreatedBy = identity.Name;
                    ledgerHeadVm.CreatedFrom = identity.WorkStationIP;
                    ledgerHeadVm.Code = DateTime.Now.ToString("yyyyMMddhhmm");
                    ledgerHeadVm.FiscalYearDetailId = vm.FiscalYearDetailId;

                    #region Description
                    ledgerHeadVm.Description = "FY:" + fiscalDetail.PeriodName;
                    if (vm.DepartmentId != "0_0")
                    {
                        DepartmentDAL dDAL = new DepartmentDAL();
                        string department = dDAL.SelectById(vm.DepartmentId).Name;
                        ledgerHeadVm.Description += ", Dep:" + department;
                    }
                    if (vm.SectionId != "0_0")
                    {
                        SectionDAL sDAL = new SectionDAL();
                        string section = sDAL.SelectById(vm.SectionId).Name;
                        ledgerHeadVm.Description += ", Sec:" + section;
                    }
                    if (vm.ProjectId != "0_0")
                    {
                        ProjectDAL pDAL = new ProjectDAL();
                        string project = pDAL.SelectById(vm.ProjectId).Name;
                        ledgerHeadVm.Description += ", Pro:" + project;
                    }

                    ledgerHeadVm.Description += ", PDate:" + vm.TransactionDate;
                    #endregion Description
                    ledgerHeadVm.PostDate = vm.TransactionDate;
                    ledgerHeadVm.IsReverse = vm.IsReverse;


                    retResults = InsertJournalLedger(ledgerHeadVm, currConn, transaction);
                    if (retResults[0].ToLower() == "fail")
                    {
                        retResults[1] = "This Month Sage Posting not completed";
                        throw new ArgumentNullException("", "");
                    }
                    else
                    {
                        JournalLedgerId = retResults[2];
                    }

                    retResults = InsertJournalLedgerDetail(getAllData, JournalLedgerId,vm.IsReverse, currConn, transaction);
                    if (retResults[0].ToLower() == "fail")
                    {
                        retResults[1] = "This Month Sage Posting not completed";
                        throw new ArgumentNullException("", "");
                    }

                    if (vm.IsReverse == false)
                    {
                        retResults = InsertJournalLedgerEmployeeHistory(journalLedgerEmployeeHistoryVMs, currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            retResults[1] = "This Month Sage Posting not completed";
                            throw new ArgumentNullException("", "");
                        }
                    }
                    else
                    {
                        retResults = DeleteJournalLedgerEmployeeHistory(journalLedgerEmployeeHistoryVMs, currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            retResults[1] = "This Month Sage Posting not completed";
                            throw new ArgumentNullException("", "");
                        }
                    }

                    #endregion Sql
                    #endregion Insert into JournalLedger Table
                }
                else
                {
                    //retResults[1] = "This Month Salary is not Proceesed Yet";
                    if (vm.IsReverse)
                        retResults[1] = journalLedgerEmployeeHistoryVMs.Count + " Employee Data Reversed Successfully.";
                    else if (!vm.IsReverse)
                        retResults[1] = journalLedgerEmployeeHistoryVMs.Count + " Employee Data Posted Successfully.";
                    throw new ArgumentNullException("", "");
                }
                #endregion Save
                #region Update Settings

                sqlText = "";
                sqlText = "update FiscalYearDetail set";
                sqlText += " SagePostComplete=1";
                sqlText += " where Id=@Id";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Parameters.AddWithValue("@Id", vm.FiscalYearDetailId);

                cmdUpdate.Transaction = transaction;
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);


                #region Commit

                if (transResult <= 0)
                {
                    retResults[1] = "This Month Sage Posting not completed";
                    throw new ArgumentNullException("", "");
                }

                #endregion Commit

                #endregion Update Settings
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
                if(vm.IsReverse)
                retResults[1] = journalLedgerEmployeeHistoryVMs.Count + " Employee Data Reversed Successfully.";
                else if (!vm.IsReverse)
                retResults[1] = journalLedgerEmployeeHistoryVMs.Count + " Employee Data Posted Successfully.";
                #endregion SuccessResult
            }

            #endregion try
            #region Catch and Finall



            catch (Exception ex)
            {
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
        public List<JournalLedgerVM> SelectAllJournalLedger()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<JournalLedgerVM> VMs = new List<JournalLedgerVM>();
            JournalLedgerVM vm;
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
Select
 JL.Id
,JL.Code
,JL.FiscalYearDetailId
,JL.Description
,JL.PostDate
,isnull(JL.IsReverse,0) IsReverse
,JL.IsActive
,JL.IsArchive
,JL.CreatedBy
,JL.CreatedAt
,JL.CreatedFrom
,JL.LastUpdateBy
,JL.LastUpdateAt
,JL.LastUpdateFrom
,fyd.PeriodName                        
from  JournalLedger JL
left outer join FiscalYearDetail fyd on JL.FiscalYearDetailId =fyd.Id
Where 1=1 
";
                //if (!string.IsNullOrEmpty(FiscalYearDetailId))
                //{
                //    sqlText += @" and JLD.FiscalYearDetailId=@FiscalYearDetailId";
                //}
                //if (!string.IsNullOrEmpty(PostingDate))
                //{
                //    sqlText += @" and JLD.TransactionDate=@PostingDate";
                //}

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                //if (!string.IsNullOrEmpty(FiscalYearDetailId))
                //{
                //    objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                //}
                //if (!string.IsNullOrEmpty(PostingDate))
                //{
                //    objComm.Parameters.AddWithValue("@PostingDate", Ordinary.DateToString(PostingDate));
                //}

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new JournalLedgerVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.PostDate = Ordinary.StringToDate(dr["PostDate"].ToString());
                    vm.IsReverse = Convert.ToBoolean(dr["IsReverse"].ToString());
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedAt = dr["CreatedAt"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateAt = dr["LastUpdateAt"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    VMs.Add(vm);
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
        public List<JournalLedgerDetailVM> SelectAllJournalLedgerDetail(string FiscalYearDetailId = null, string PostingDate = null, string Id=null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<JournalLedgerDetailVM> VMs = new List<JournalLedgerDetailVM>();
            JournalLedgerDetailVM vm;
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
Select
 JLD.Id    
,JLD.JournalLedgerId
,JLD.JournalType            
,JLD.TransactionDate   
,JLD.BatchDesc         
,JLD.GLCode            
,JLD.Reference         
,JLD.Remarks           
,JLD.AccDescription    
,JLD.IsDebit           
,JLD.TransactionAmount 
,JLD.SrceType          
,JLD.JrnlDesc          
,JLD.FiscalYearDetailId
,isnull(JLD.IsReverse,0) IsReverse
,fyd.PeriodName                        
,JL.Code
,JL.Description
,JL.PostDate             
from  JournalLedgerDetail JLD
left outer join JournalLedger JL on JL.Id = JLD.JournalLedgerId
left outer join FiscalYearDetail fyd on JLD.FiscalYearDetailId =fyd.Id
Where 1=1 
";
                if (!string.IsNullOrEmpty(FiscalYearDetailId))
                {
                    sqlText += @" and JLD.FiscalYearDetailId=@FiscalYearDetailId";
                }
                if (!string.IsNullOrEmpty(PostingDate))
                {
                    sqlText += @" and JLD.TransactionDate=@PostingDate";
                }
                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @" and JLD.JournalLedgerId=@JournalLedgerId";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(FiscalYearDetailId))
                {
                    objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                }
                if (!string.IsNullOrEmpty(PostingDate))
                {
                    objComm.Parameters.AddWithValue("@PostingDate", Ordinary.DateToString(PostingDate));
                }
                  if (!string.IsNullOrEmpty(Id))
                {
                    objComm.Parameters.AddWithValue("@JournalLedgerId", Id);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new JournalLedgerDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.JournalLedgerId = Convert.ToInt32(dr["JournalLedgerId"]);
                    vm.JournalType = dr["JournalType"].ToString();
                    vm.TransactionDate = Ordinary.StringToDate(dr["TransactionDate"].ToString());
                    vm.BatchDesc = dr["BatchDesc"].ToString();
                    vm.GLCode = dr["GLCode"].ToString();
                    vm.Reference = dr["Reference"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.AccDescription = dr["AccDescription"].ToString();
                    vm.IsDebit = Convert.ToBoolean(dr["IsDebit"]);
                    vm.TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]);
                    vm.SrceType = dr["SrceType"].ToString();
                    vm.JrnlDesc = dr["JrnlDesc"].ToString();
                    vm.FiscalYearDetailId = dr["FiscalYearDetailId"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.IsReverse = Convert.ToBoolean(dr["IsReverse"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Description = dr["Description"].ToString();
                    vm.PostDate = Ordinary.StringToDate(dr["PostDate"].ToString());
                    VMs.Add(vm);
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
        public List<JournalLedgerDetailVM> SageIntegrationVoucher(string FiscalYearDetailId, string DepartmentId = null, string SectionId = null, string ProjectId = null
            , List<string> multiEmployeeId = null, string JournalDesc = null, bool IsReverse = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            //int transResult = 0;

            string sqlText = "";
            int Id = 0;
            decimal debit = 0;
            decimal credit = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Select SageIntegrationVoucher"; //Method Name
            List<JournalLedgerDetailVM> ledgerVms = new List<JournalLedgerDetailVM>();
            JournalLedgerDetailVM ledgerVm = new JournalLedgerDetailVM();
            bool IsOutstandingLiabilities = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion
            #region Try

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
                #region Exist
                var fiscalDetail = _fyDal.FYPeriodDetail(Convert.ToInt32(FiscalYearDetailId), currConn, transaction).FirstOrDefault();
                //bool sagePostComplete = Convert.ToBoolean(fiscalDetail.SagePostComplete);

                string desc = "salary for the month of " + fiscalDetail.PeriodName;
                //if (sagePostComplete)
                //{
                //    retResults[1] = "This Month Sage Posting Already completed";
                //    throw new ArgumentNullException("", "");
                //}

                var OutstandingLiabilities = _glAccDal.SelectAll(null,currConn, transaction).Where(x => x.OutstandingLiabilities.Equals(true)).FirstOrDefault();

                #endregion Exist
                #region Read
                #region sql statement

                sqlText = @"select distinct vspc.FiscalYearDetailId, vspc.GLAccountCode
                            ,gla.Description
                            ,vspc.IsEarning,sum(vspc.Amount)Amount from ViewSalaryPreCalculation vspc
                            left outer join GLAccount gla on vspc.GLAccountCode=gla.GLAccountCode
                            where vspc.GLAccountCode not in('NA')
                            and vspc.FiscalYearDetailId=@FiscalYearDetailId ";
                if (IsReverse)
                {
                    sqlText += " and EmployeeId in";
                }
                else
                {
                    sqlText += " and EmployeeId not in";
                
                }
                sqlText += @"  (select EmployeeId from JournalLedgerEmployeeHistory where 
							        FiscalYearDetailId=@FiscalYearDetailId) ";

                if (DepartmentId != "0_0" && DepartmentId != null)
                {
                    sqlText += "  and vspc.DepartmentId=@DepartmentId";
                }
                if (SectionId != "0_0" && SectionId != null)
                {
                    sqlText += "  and vspc.SectionId=@SectionId";
                }
                if (ProjectId != "0_0" && ProjectId != null)
                {
                    sqlText += "  and vspc.ProjectId=@ProjectId";
                }
                if (multiEmployeeId.Count > 0 && !string.IsNullOrWhiteSpace(multiEmployeeId.FirstOrDefault()))
                {
                    sqlText += "  and vspc.EmployeeId in(";
                    for (int i = 0; i < multiEmployeeId.Count; i++)
                    {
                        sqlText += "'" + multiEmployeeId[i] + "'";
                        if (i < multiEmployeeId.Count - 1)
                            sqlText += ", ";
                    }
                    sqlText += ")";
                }


                sqlText += @" group by vspc.FiscalYearDetailId, vspc.GLAccountCode,gla.Description,vspc.IsEarning
                             order by vspc.IsEarning desc, vspc.GLAccountCode asc";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.Transaction = transaction;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                if (DepartmentId != "0_0" && DepartmentId != null)
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (SectionId != "0_0" && SectionId != null)
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (ProjectId != "0_0" && ProjectId != null)
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    IsOutstandingLiabilities = true;
                    ledgerVm = new JournalLedgerDetailVM();
                    ledgerVm.FiscalYearDetailId = FiscalYearDetailId;
                    ledgerVm.PeriodName = fiscalDetail.PeriodName;
                    ledgerVm.JournalType = "Salary";
                    ledgerVm.TransactionDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");
                    ledgerVm.BatchDesc = desc;//dr["Code"].ToString();         
                    ledgerVm.GLCode = dr["GLAccountCode"].ToString();
                    ledgerVm.AccDescription = dr["Description"].ToString();
                    ledgerVm.Reference = "Sampan HRM";//dr["Code"].ToString(); 
                    ledgerVm.Remarks = JournalDesc;//dr["Code"].ToString(); 
                    ledgerVm.IsDebit = Convert.ToBoolean(dr["IsEarning"]);
                    ledgerVm.TransactionAmount = Convert.ToDecimal(dr["Amount"]);
                    ledgerVm.SrceType = "PR";
                    ledgerVm.JrnlDesc = desc;// dr["Code"].ToString(); 

                    if (ledgerVm.IsDebit)
                    {
                        debit = debit + ledgerVm.TransactionAmount;
                    }
                    else
                    {
                        credit = credit + ledgerVm.TransactionAmount;
                    }

                    ledgerVms.Add(ledgerVm);
                }


                dr.Close();
                if (IsOutstandingLiabilities)
                {
                    ledgerVm = new JournalLedgerDetailVM();
                    ledgerVm.FiscalYearDetailId = FiscalYearDetailId;
                    ledgerVm.PeriodName = fiscalDetail.PeriodName;
                    ledgerVm.JournalType = "Salary";
                    ledgerVm.TransactionDate = DateTime.Now.Date.ToString("dd-MMM-yyy");
                    ledgerVm.BatchDesc = desc;//dr["Code"].ToString();         
                    ledgerVm.GLCode = OutstandingLiabilities.GLAccountCode;
                    ledgerVm.AccDescription = OutstandingLiabilities.Description;
                    ledgerVm.Reference = "Sampan HRM";//dr["Code"].ToString(); 
                    ledgerVm.Remarks = JournalDesc;//dr["Code"].ToString(); 
                    ledgerVm.IsDebit = false;//Convert.ToBoolean(dr["IsEarning"]);
                    ledgerVm.TransactionAmount = debit - credit;// Convert.ToDecimal(dr["Amount"]);
                    ledgerVm.SrceType = "PR";
                    ledgerVm.JrnlDesc = desc;// dr["Code"].ToString(); 

                    ledgerVms.Add(ledgerVm);
                }
                #endregion
                #endregion Read


                #region Commit

                //if (transResult <= 0)
                //{
                //    retResults[1] = "This Month Sage Posting not completed";
                //    throw new ArgumentNullException("", "");
                //}

                #endregion Commit


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

            #endregion try
            #region Catch and Finall



            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null) { transaction.Rollback(); }
                return ledgerVms;
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

            return ledgerVms;
            #endregion
        }
        public List<JournalLedgerEmployeeHistoryVM> JournalLedgerEmployeeHistory(string FiscalYearDetailId, string DepartmentId = null, string SectionId = null, string ProjectId = null
          //, List<string> multiEmployeeId = null
            ,   bool IsReverse = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Initializ
            //int transResult = 0;

            string sqlText = "";
            int Id = 0;
            decimal debit = 0;
            decimal credit = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Select SageIntegrationVoucher"; //Method Name
            List<JournalLedgerEmployeeHistoryVM> Vms = new List<JournalLedgerEmployeeHistoryVM>();
            JournalLedgerEmployeeHistoryVM Vm = new JournalLedgerEmployeeHistoryVM();
            bool IsOutstandingLiabilities = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

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
                #region Exist
                var fiscalDetail = _fyDal.FYPeriodDetail(Convert.ToInt32(FiscalYearDetailId), currConn, transaction).FirstOrDefault();
                string desc = "salary for the month of " + fiscalDetail.PeriodName;


                #endregion Exist
                #region Read
                #region sql statement

                sqlText = @" select distinct Employeeid from ViewSalaryPreCalculation vspc
                            where  vspc.FiscalYearDetailId=@FiscalYearDetailId";
                if (IsReverse)
                {
                    sqlText += " and EmployeeId in";
                }
                else
                {
                    sqlText += " and EmployeeId not in";

                }
                sqlText += @"  (select EmployeeId from JournalLedgerEmployeeHistory where 
							        FiscalYearDetailId=@FiscalYearDetailId) ";

                if (DepartmentId != "0_0" && DepartmentId != null)
                {
                    sqlText += "  and vspc.DepartmentId=@DepartmentId";
                }
                if (SectionId != "0_0" && SectionId != null)
                {
                    sqlText += "  and vspc.SectionId=@SectionId";
                }
                if (ProjectId != "0_0" && ProjectId != null)
                {
                    sqlText += "  and vspc.ProjectId=@ProjectId";
                }
                //if (multiEmployeeId != null)
                //{
                //    sqlText += "  and vspc.EmployeeId in(";
                //    for (int i = 0; i < multiEmployeeId.Count; i++)
                //    {
                //        sqlText += "'" + multiEmployeeId[i] + "'";
                //        if (i < multiEmployeeId.Count - 1)
                //            sqlText += ", ";
                //    }
                //    sqlText += ")";
                //}

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.Transaction = transaction;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);

                if (DepartmentId != "0_0" && DepartmentId != null)
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (SectionId != "0_0" && SectionId != null)
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (ProjectId != "0_0" && ProjectId != null)
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    Vm = new JournalLedgerEmployeeHistoryVM();
                    Vm.FiscalYearDetailId = FiscalYearDetailId;
                    Vm.EmployeeId = dr["EmployeeId"].ToString();
                    Vms.Add(Vm);
                }


                dr.Close();
                
                #endregion
                #endregion Read

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

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString(); //catch ex

                if (Vtransaction == null) { transaction.Rollback(); }
                return Vms;
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

            return Vms;
            #endregion


        }
        public string[] InsertJournalLedger(JournalLedgerVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "JournalLedger"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try

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

                #region Save

                //int foundId = (int)objfoundId;
                if (vm != null)
                {
                    sqlText = "  ";
sqlText += @" INSERT INTO JournalLedger(
                    Code
                    ,FiscalYearDetailId
                    ,Description
                    ,PostDate
                    ,IsReverse
                    ,IsActive
                    ,IsArchive
                    ,CreatedBy
                    ,CreatedAt
                    ,CreatedFrom
                    )
                    VALUES (
                    @Code
                    ,@FiscalYearDetailId
                    ,@Description
                    ,@PostDate
                    ,@IsReverse
                    ,@IsActive
                    ,@IsArchive
                    ,@CreatedBy
                    ,@CreatedAt
                    ,@CreatedFrom
                    ) 
                    SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@Description", vm.Description);
                    cmdInsert.Parameters.AddWithValue("@PostDate", Ordinary.DateToString(vm.PostDate));
                   
                    cmdInsert.Parameters.AddWithValue("@IsReverse", vm.IsReverse);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);


                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input JournalLedger";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input BranchJournalLedger", "");
                    }
                }
                else
                {
                    retResults[1] = "Please Input JournalLedger";
                    throw new ArgumentNullException("Please Input JournalLedger", "");
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
                retResults[2] = Id.ToString();

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
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
        public string[] InsertJournalLedgerDetail(List<JournalLedgerDetailVM> ledgerVms, string JournalLedgerId, bool isReverse=false, SqlConnection VcurrConn=null, SqlTransaction Vtransaction=null)
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
            retResults[5] = "InsertJournalLedgerDetail"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
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

                #region Save

                if (ledgerVms != null)
                {
                    sqlText = "  ";
                    sqlText = @"Insert Into JournalLedgerDetail
                    (
                    FiscalYearDetailId,JournalLedgerId,JournalType,TransactionDate,BatchDesc        
                    ,GLCode ,Reference  ,Remarks  ,AccDescription,IsDebit         
                    ,TransactionAmount ,SrceType,JrnlDesc
                    ,isReverse
                    ,IsActive
                    ,IsArchive
                    ,CreatedBy
                    ,CreatedAt
                    ,CreatedFrom
                    ) Values (
                    @FiscalYearDetailId
                    ,@JournalLedgerId,@JournalType,@TransactionDate ,@BatchDesc        
                    ,@GLCode ,@Reference ,@Remarks         
                    ,@AccDescription,@IsDebit ,@TransactionAmount,@SrceType,@JrnlDesc  
                    ,@isReverse
                    ,@IsActive
                    ,@IsArchive
                    ,@CreatedBy
                    ,@CreatedAt
                    ,@CreatedFrom       
                    )";
                    SqlCommand cmdInsert;
                    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                    JournalLedgerDetailVM vmInsertDay = new JournalLedgerDetailVM();
                    vmInsertDay.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                    vmInsertDay.CreatedBy = identity.Name;
                    vmInsertDay.CreatedFrom = identity.WorkStationIP;
                    
                    foreach (JournalLedgerDetailVM vm in ledgerVms)
                    {
                        cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@JournalLedgerId", JournalLedgerId);
                        cmdInsert.Parameters.AddWithValue("@JournalType", vm.JournalType);
                        cmdInsert.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.TransactionDate));
                        cmdInsert.Parameters.AddWithValue("@BatchDesc", vm.BatchDesc ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@GLCode", vm.GLCode);
                        cmdInsert.Parameters.AddWithValue("@Reference", vm.Reference);
                        cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                        cmdInsert.Parameters.AddWithValue("@AccDescription", vm.AccDescription);
                        if (isReverse)
                        {
                            cmdInsert.Parameters.AddWithValue("@IsDebit", vm.IsDebit==true?false:true);
                        }
                        else
                        {
                        cmdInsert.Parameters.AddWithValue("@IsDebit", vm.IsDebit);
                        }
                        cmdInsert.Parameters.AddWithValue("@TransactionAmount", vm.TransactionAmount);
                        cmdInsert.Parameters.AddWithValue("@SrceType", vm.SrceType);
                        cmdInsert.Parameters.AddWithValue("@JrnlDesc", vm.BatchDesc);
                        cmdInsert.Parameters.AddWithValue("@isReverse", isReverse);
                        cmdInsert.Parameters.AddWithValue("@IsActive", true);
                        cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                        cmdInsert.Parameters.AddWithValue("@CreatedBy", vmInsertDay.CreatedBy);
                        cmdInsert.Parameters.AddWithValue("@CreatedAt", vmInsertDay.CreatedAt);
                        cmdInsert.Parameters.AddWithValue("@CreatedFrom", vmInsertDay.CreatedFrom);
                        cmdInsert.ExecuteNonQuery();
                    }

                }
                else
                {
                    retResults[1] = "Please Input JournalLedgerDetail";
                    throw new ArgumentNullException("Please Input JournalLedgerDetail", "");
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
                retResults[2] = Id.ToString();

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
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
        public string[] InsertJournalLedgerEmployeeHistory(List<JournalLedgerEmployeeHistoryVM> Vms, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "JournalLedgerEmployeeHistoryVM"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
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

                #region Save

                if (Vms != null)
                {

                    sqlText = "  ";
                    sqlText = @"Insert Into JournalLedgerEmployeeHistory
                    (
                    FiscalYearDetailId,EmployeeId 
                    ) Values (
                    @FiscalYearDetailId
                    ,@EmployeeId     
                    )";
                    SqlCommand cmdInsert;
                    ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                    foreach (JournalLedgerEmployeeHistoryVM vm in Vms)
                    {
                        cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdInsert.ExecuteNonQuery();
                    }

                }
                else
                {
                    retResults[1] = "Please Input JournalLedgerDetail";
                    throw new ArgumentNullException("Please Input JournalLedgerDetail", "");
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
                retResults[2] = Id.ToString();

                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall



            catch (Exception ex)
            {
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
        //==================Delete JournalLedgerEmployeeHistory =================
        public string[] DeleteJournalLedgerEmployeeHistory(List<JournalLedgerEmployeeHistoryVM> VMs, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Delete"; //Method Name

            int transResult = 0;
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            bool iSTransSuccess = false;

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToBranch"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (VMs.Count>=1)
                {
                    #region Update Settings
                     sqlText = "";
                        sqlText = @"DELETE FROM JournalLedgerEmployeeHistory
                        where EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";

                    foreach (JournalLedgerEmployeeHistoryVM empId in VMs)
                    {
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@EmployeeId", empId.EmployeeId);
                        cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", empId.FiscalYearDetailId);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }

                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("JournalLedgerEmployee History Delete",  " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("JournalLedgerEmployee Information Delete", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to JournalLedgerEmployee Branch Information.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
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
        public string GlIntegration1(List<JournalLedgerDetailVM> vms)
        {

            string sql = "";


            string msg = "";
            List<string> SageChartOfAccount = new List<string>();

            SAGEDB = _setDAL.settingValue("SAGE", "SAGEDB", null, null);

            var dtSageChartOfAccount = DtSageChartOfAccount(SAGEDB);
            foreach (DataRow item in dtSageChartOfAccount.Rows)
            {
                SageChartOfAccount.Add(item["ACCTFMTTD"].ToString());
            }

            foreach (JournalLedgerDetailVM item in vms)
            {
                if (!SageChartOfAccount.Contains(item.GLCode))
                {
                    return "[" + item + "] GLCode is not contain in SAGE Account";
                }
            }

            Session();
            DBLink mDBLinkCmpRW;
            mDBLinkCmpRW = session.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);

            DBLink mDBLinkSysRW;
            mDBLinkSysRW = session.OpenDBLink(DBLinkType.System, DBLinkFlags.ReadWrite);

            bool temp;

            sql = "";

            try
            {

                foreach (JournalLedgerDetailVM item in vms)
                {
                    #region GL
                    #region inisalize
                    ACCPAC.Advantage.View GLBATCH1batch = mDBLinkCmpRW.OpenView("GL0008");
                    ACCPAC.Advantage.View GLBATCH1header = mDBLinkCmpRW.OpenView("GL0006");
                    ACCPAC.Advantage.View GLBATCH1detail1 = mDBLinkCmpRW.OpenView("GL0010");
                    ACCPAC.Advantage.View GLBATCH1detail2 = mDBLinkCmpRW.OpenView("GL0402");
                    GLBATCH1batch.Compose(new ACCPAC.Advantage.View[] { GLBATCH1header });
                    GLBATCH1header.Compose(new ACCPAC.Advantage.View[] { GLBATCH1batch, GLBATCH1detail1 });
                    GLBATCH1detail1.Compose(new ACCPAC.Advantage.View[] { GLBATCH1header, GLBATCH1detail2 });
                    GLBATCH1detail2.Compose(new ACCPAC.Advantage.View[] { GLBATCH1detail1 });
                    GLBATCH1batch.Browse("((BATCHSTAT = " + 1 + " OR BATCHSTAT = " + 6 + " OR BATCHSTAT = " + 9 + "))", false);
                    GLBATCH1batch.RecordCreate(0);
                    GLBATCH1batch.Read(false);
                    GLBATCH1batch.Fields.FieldByName("PROCESSCMD").SetValue("1", false);
                    GLBATCH1batch.Process();
                    GLBATCH1header.Fields.FieldByName("BTCHENTRY").SetValue("", false);
                    GLBATCH1header.Browse("", true);
                    GLBATCH1header.Fetch(false);
                    GLBATCH1header.Fields.FieldByName("BTCHENTRY").SetValue("00000", true);
                    GLBATCH1header.RecordCreate(0);
                    temp = GLBATCH1header.Exists;
                    #endregion
                    GLBATCH1batch.Fields.FieldByName("BTCHDESC").SetValue(item.BatchDesc, false);

                    //GLBATCH1batch.Fields.FieldByName("BTCHDESC").SetValue(item.BATCHDESC, false);
                    GLBATCH1batch.Insert();
                    GLBATCH1header.Fields.FieldByName("DOCDATE").SetValue(Convert.ToDateTime(item.TransactionDate).ToString("yyyy,MM,dd"), false);
                    GLBATCH1header.Fields.FieldByName("DATEENTRY").SetValue(Convert.ToDateTime(item.TransactionDate).ToString("yyyy,MM,dd"), false);
                    //GLBATCH1header.Fields.FieldByName("FSCSYR").SetValue(Convert.ToDateTime(item.TransactionDate).ToString("yyyy"), false);
                    //GLBATCH1header.Fields.FieldByName("FSCSPERD").SetValue(Convert.ToDateTime(item.TransactionDate).ToString("MM"), false);
                    GLBATCH1header.Fields.FieldByName("SRCETYPE").SetValue(item.SrceType, false);

                    temp = GLBATCH1detail1.Exists;
                    GLBATCH1detail1.RecordClear();
                    foreach (JournalLedgerDetailVM item2 in vms)
                    {
                        temp = GLBATCH1detail1.Exists;
                        GLBATCH1detail1.RecordCreate(0);
                        GLBATCH1detail1.Fields.FieldByName("ACCTID").SetValue(item2.GLCode, false);

                        GLBATCH1detail1.Fields.FieldByName("TRANSDESC").SetValue(item2.AccDescription.ToString(), false);     //Reference
                        GLBATCH1detail1.Fields.FieldByName("TRANSREF").SetValue(item2.Reference.ToString(), false);     //Reference
                        GLBATCH1detail1.Process();
                        decimal amount = item2.IsDebit ? item2.TransactionAmount : -1 * item2.TransactionAmount;
                        GLBATCH1detail1.Fields.FieldByName("SCURNAMT").SetValue(amount, false);
                        GLBATCH1detail1.Fields.FieldByName("COMMENT").SetValue(item2.Remarks, false); //Comment
                        GLBATCH1detail1.Insert();
                        GLBATCH1detail1.Fields.FieldByName("TRANSNBR").SetValue("-1", false);
                        GLBATCH1detail1.Read(false);
                    }
                    temp = GLBATCH1header.Exists;
                    GLBATCH1batch.Read(false);
                    GLBATCH1header.Fields.FieldByName("JRNLDESC").SetValue("NA", false);
                    GLBATCH1header.Insert();
                    GLBATCH1header.Read(false);
                    temp = GLBATCH1header.Exists;
                    temp = GLBATCH1batch.Exists;
                    #endregion


                    break;//for single time 
                }
                msg = "Success";
            }
            catch (Exception)
            {

                msg = "Failed";
            }


            return msg;
        }
        public string GlIntegrationBank1(List<JournalLedgerDetailVM> vms, string BankCode, string BankGLCode, bool IsDeposit, string transactionType)
        {
            #region open connection and transaction
            SqlConnection currConn = null;

            currConn = _dbsqlConnection.GetConnection();
            if (currConn.State != ConnectionState.Open)
            {
                currConn.Open();
            }

            #endregion open connection and transaction

            #region
            string msg = "";

            List<string> SageChartOfAccount = new List<string>();

            SAGEDB = _setDAL.settingValue("SAGE", "SAGEDB", null, null);

            var dtSageChartOfAccount = _cdal.DataTableLoad("GLAMF", null, SAGEDB);
            foreach (DataRow item in dtSageChartOfAccount.Rows)
            {
                SageChartOfAccount.Add(item["ACCTID"].ToString());
            }

            foreach (JournalLedgerDetailVM item in vms)
            {
                if (!SageChartOfAccount.Contains(item.GLCode))
                {
                    return "[" + item + "] GLCode is not contain in SAGE Account";
                }
            }


            Session();
            DBLink mDBLinkCmpRW;
            mDBLinkCmpRW = session.OpenDBLink(DBLinkType.Company, DBLinkFlags.ReadWrite);

            DBLink mDBLinkSysRW;
            mDBLinkSysRW = session.OpenDBLink(DBLinkType.System, DBLinkFlags.ReadWrite);

            bool temp;

            #endregion

            string sql = "";
            //int BankEntryNumber = 0;

            try
            {
                foreach (JournalLedgerDetailVM item in vms)
                {

                    //// Bank
                    #region Hearde map
                    ACCPAC.Advantage.View BKTRANSENT1header = mDBLinkCmpRW.OpenView("BK0450");
                    ACCPAC.Advantage.View BKTRANSENT1detail = mDBLinkCmpRW.OpenView("BK0460");

                    BKTRANSENT1header.Compose(new ACCPAC.Advantage.View[] { BKTRANSENT1detail });
                    BKTRANSENT1detail.Compose(new ACCPAC.Advantage.View[] { BKTRANSENT1header });


                    BKTRANSENT1header.Browse("", true);
                    BKTRANSENT1header.Fields.FieldByName("SEQUENCENO").SetValue("0", false);
                    temp = BKTRANSENT1header.Exists;

                    BKTRANSENT1header.Init();
                    temp = BKTRANSENT1header.Exists;
                    temp = BKTRANSENT1header.Exists;
                    temp = BKTRANSENT1header.Exists;
                    temp = BKTRANSENT1header.Exists;
                    //BKTRANSENT1header.Fields.FieldByName("ENTRYNBR").SetValue(BankEntryNumber, false);

                    //BKTRANSENT1header.Order = 1;
                    //BKTRANSENT1header.Order = 0;
                    BKTRANSENT1header.Fields.FieldByName("REFERENCE").SetValue("PY_" + Convert.ToDateTime(item.TransactionDate).ToString("MMM-yyyy"), false);
                    BKTRANSENT1header.Fields.FieldByName("BANK").SetValue(BankCode, false);
                    temp = BKTRANSENT1header.Exists;
                    temp = BKTRANSENT1header.Exists;
                    temp = BKTRANSENT1header.Exists;

                    if (IsDeposit)
                    {
                        BKTRANSENT1header.Fields.FieldByName("TRANSTYPE").SetValue("2", false); //Deposit
                        //sum = -1;
                    }
                    else
                    {
                        BKTRANSENT1header.Fields.FieldByName("TRANSTYPE").SetValue("1", false); //Withdraw
                    }

                    BKTRANSENT1header.Fields.FieldByName("TYPE").SetValue("1", false);
                    BKTRANSENT1header.Fields.FieldByName("POSTDATE").SetValue(Convert.ToDateTime(item.TransactionDate).ToString("yyyy,MM,dd"), false);
                    BKTRANSENT1detail.RecordClear();
                    #endregion
                    foreach (JournalLedgerDetailVM item2 in vms)
                    {
                        #region Details map
                        if (item2.GLCode == BankGLCode)
                        {
                            continue;
                        }
                        BKTRANSENT1detail.RecordCreate(0);
                        temp = BKTRANSENT1header.Exists;
                        BKTRANSENT1detail.Fields.FieldByName("GLACCOUNT").SetValue(item2.GLCode, false);
                        decimal amount = 0;
                        if (IsDeposit == false)
                        {
                            //withdraw
                            amount = item2.IsDebit ? item2.TransactionAmount : -1 * item2.TransactionAmount;
                        }
                        else
                        {
                            amount = item2.IsDebit ? item2.TransactionAmount * -1 : item2.TransactionAmount;
                        }


                        BKTRANSENT1detail.Fields.FieldByName("SRCEAMT").SetValue(Convert.ToDecimal(amount), false);
                        BKTRANSENT1detail.Fields.FieldByName("REFERENCE").SetValue(item2.Reference, false);//dr[0].ItemArray[9]
                        BKTRANSENT1detail.Fields.FieldByName("COMMENT").SetValue(item2.Remarks, false);
                        BKTRANSENT1detail.Fields.FieldByName("BIGCOMMENT").SetValue("", false);
                        //BKTRANSENT1detail.Fields.FieldByName("BIGCOMMENT").SetValue(drh.ItemArray[13].ToString().Trim(), false);
                        BKTRANSENT1detail.Insert();
                        temp = BKTRANSENT1header.Exists;
                        temp = BKTRANSENT1header.Exists;
                        BKTRANSENT1detail.Fields.FieldByName("LINE").SetValue("-1", false);
                        BKTRANSENT1detail.Fields.FieldByName("LINE").SetValue("-1", false);
                        BKTRANSENT1detail.Read(true);
                        #endregion
                    }
                    temp = BKTRANSENT1header.Exists;
                    BKTRANSENT1header.Insert();
                    temp = BKTRANSENT1header.Exists;
                    ///Bank

                    try
                    {
                        //sql = "update FdrGLPost set GLPost=1 , LastEntryNumber='" + BankEntryNumber + "' where FDRID=@FDRID  and FDRType=@transactionType";
                        //MyCommand = MyCommandBuilder(sql);
                        //MyCommand.AddParameterWithValue("@FDRID", item.FDRID);
                        //MyCommand.AddParameterWithValue("@transactionType", transactionType);
                        //var tt = Execute(MyCommand);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    break;
                }
                msg = "GL Integration successfull!";
            }
            catch (Exception)
            {

                msg = "GL Integration failed!";
            }


            return msg;
        }
        private void Session()
        {
            session = new Session();
            session.Init("", "XY", "XY1000", "63A");
            try
            {
                SAGEDB = _setDAL.settingValue("SAGE", "SAGEDB", null, null);
                Password = _setDAL.settingValue("SAGE", "Password", null, null);
                UserName = _setDAL.settingValue("SAGE", "UserName", null, null);
                session.Open(UserName.ToUpper(), Password.ToUpper(), SAGEDB, DateTime.Now, 0);
            }
            catch (Exception)
            {

                //Error2 = "Can't open sage api";
                throw new ArgumentNullException("Can't open sage api", "");
            }
        }
        public System.Data.DataTable DtSageChartOfAccount(string DatabaseName)
        {
            System.Data.DataTable dt = new System.Data.DataTable("");
            SqlConnection currConn = null;

            try
            {

                #region New open connection and transaction


                #endregion New open connection and transaction

                currConn = _dbsqlConnection.GetConnectionSage();

                if (currConn.State != ConnectionState.Open)
                    currConn.Open();
                if (DatabaseName != null)
                {
                    currConn.ChangeDatabase(DatabaseName);
                }

                string sql = "";
                sql = sql + @" select  ltrim(rtrim(ACCTFMTTD))ACCTFMTTD from GLAMF";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sql;
                objComm.CommandType = CommandType.Text;

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dt);

                return dt;

                #region Commit


                #endregion Commit
            }
            catch (Exception)
            {
                return dt;
            }
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

        }


    }
}
