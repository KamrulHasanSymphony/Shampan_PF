using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.WWF;
using System.Linq;
using System.Threading;
using SymViewModel.Common;

namespace SymServices.WWF
{
    public class PFSettlementDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;

        #endregion
        #region Methods
        //==================SelectEmployeeList=================
        /// <summary>
        /// Retrieves a list of Resigne Employee from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Resigne Employee.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="BankBranchVM"/> representing the Resigne Employee matching the criteria.</returns>
        public List<PFSettlementVM> SelectAll_ResignEmployee(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFSettlementVM> VMs = new List<PFSettlementVM>();
            PFSettlementVM vm;
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
SELECT DISTINCT 
 ve.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.ResignDate
";
                sqlText += " FROM [dbo].ViewEmployeeInformation ve";
                sqlText += @" WHERE  1=1 AND ve.IsActive = 0 and ResignReason!='Resignation'
";
                sqlText += @" 
AND ve.EmployeeId NOT IN (
SELECT EmployeeId FROM PFSettlements WHERE 1=1 AND Post = 1
UNION ALL
SELECT EmployeeId FROM ForfeitureAccounts WHERE 1=1 AND Post = 1
)
";

                //////LeftDate BETWEEN 20180101 AND 20991231
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
                sqlText += @" ORDER BY ve.Code";

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
                    vm = new PFSettlementVM();
                    ////////vm.Id = dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.EmpResignDate = Ordinary.StringToDate(dr["ResignDate"].ToString());
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
        /// <summary>
        /// Retrieves a list of Profit Distribution Detail from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Profit Distribution Detail.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="BankBranchVM"/> representing the Profit Distribution Detail matching the criteria.</returns>
        public PFSettlementVM SelectAll_ProfitDistributionDetail(string EmployeeId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            PFSettlementVM vm = new PFSettlementVM();
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
select distinct p.EmployeeId
,e.ProjectId
,e.DepartmentId
,e.SectionId
,e.DesignationId
,e.JoinDate EmpDOJ
,e.leftDate EmpResignDate
,sum(p.EmployeeProfitValue)EmployeeProfitValue
,sum(p.EmployerProfitValue)EmployerProfitValue
,sum(p.EmployeeTotalContribution)EmployeeTotalContribution
,sum(p.EmployerTotalContribution)EmployerTotalContribution
from ProfitDistributionDetails as p ";
                sqlText += @" LEFT OUTER JOIN ViewEmployeeInformation AS e ON p.EmployeeId=e.EmployeeId";
                sqlText += @" WHERE 1=1 AND p.Post = 1 AND e.EmployeeId = @EmployeeId";
                sqlText += @" 
GROUP BY 
 p.EmployeeId
,e.ProjectId
,e.DepartmentId
,e.SectionId
,e.DesignationId
,e.JoinDate
,e.leftDate
";


                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);



                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFSettlementVM();
                    ////////vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.EmployeeProfitValue = Convert.ToDecimal(dr["EmployeeProfitValue"]);
                    vm.EmployerProfitValue = Convert.ToDecimal(dr["EmployerProfitValue"]);
                    vm.EmployeeTotalContribution = Convert.ToDecimal(dr["EmployeeTotalContribution"]);
                    vm.EmployerTotalContribution = Convert.ToDecimal(dr["EmployerTotalContribution"]);
                    vm.EmpDOJ = Ordinary.StringToDate(dr["EmpDOJ"].ToString());
                    vm.EmpResignDate = Ordinary.StringToDate(dr["EmpResignDate"].ToString());

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
        /// Retrieves a list of WWF Settlement from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific bank Name.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="BankBranchVM"/> representing the WWF Settlement matching the criteria.</returns>
        public List<PFSettlementVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFSettlementVM> VMs = new List<PFSettlementVM>();
            PFSettlementVM vm;
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

 pfs.Id
,pfs.TransactionType
,pfs.TransactionCode
,pfs.FiscalYearDetailId
,pfs.ProjectId
,pfs.DepartmentId
,pfs.SectionId
,pfs.DesignationId
,pfs.EmployeeId

,pfs.EmployeeTotalContribution
,pfs.EmployerTotalContribution
,pfs.EmployeeProfitValue
,pfs.EmployerProfitValue
,pfs.EmployeeActualContribution
,pfs.EmployerActualContribution
,pfs.EmployeeActualProfitValue
,pfs.EmployerActualProfitValue
,ISNULL(pfs.EmployeeContributionForfeitValue    ,0) EmployeeContributionForfeitValue 
,ISNULL(pfs.EmployeeProfitForfeitValue          ,0) EmployeeProfitForfeitValue
,ISNULL(pfs.EmployerContributionForfeitValue    ,0) EmployerContributionForfeitValue 
,ISNULL(pfs.EmployerProfitForfeitValue          ,0) EmployerProfitForfeitValue 
,ISNULL(pfs.TotalForfeitValue                   ,0) TotalForfeitValue
,ISNULL(pfs.TotalPayableAmount                  ,0) TotalPayableAmount
,ISNULL(pfs.AlreadyPaidAmount                   ,0) AlreadyPaidAmount
,ISNULL(pfs.NetPayAmount                        ,0) NetPayAmount
,ISNULL(pfs.ProvidentFundAmount                 ,0) ProvidentFundAmount


,pfs.PFStartDate
,pfs.PFEndDate


,pfs.EmpDOJ
,pfs.EmpResignDate
,pfs.SettlementDate
,pfs.SettlementPolicyId
,pfs.JobAgeInMonth
,pfs.EmployeeContributionRatio
,pfs.EmployerContributionRatio
,pfs.EmployeeProfitRatio
,pfs.EmployerProfitRatio




,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project


,pfs.Post

,pfs.Remarks,pfs.IsActive,pfs.IsArchive,pfs.CreatedBy,pfs.CreatedAt,pfs.CreatedFrom,pfs.LastUpdateBy,pfs.LastUpdateAt,pfs.LastUpdateFrom
FROM  PFSettlements  pfs

";
                sqlText += " LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfs.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 ";


                if (Id > 0)
                {
                    sqlText += @" and pfs.Id=@Id";
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
                sqlText += " ORDER BY pfs.SettlementDate, ve.Code ";

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
                    vm = new PFSettlementVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.TransactionCode = Convert.ToString(dr["TransactionCode"]);
                    vm.TransactionType = Convert.ToString(dr["TransactionType"]);

                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);

                    vm.EmployeeTotalContribution = Convert.ToDecimal(dr["EmployeeTotalContribution"]);
                    vm.EmployerTotalContribution = Convert.ToDecimal(dr["EmployerTotalContribution"]);
                    vm.EmployeeProfitValue = Convert.ToDecimal(dr["EmployeeProfitValue"]);
                    vm.EmployerProfitValue = Convert.ToDecimal(dr["EmployerProfitValue"]);

                    vm.EmployeeActualContribution = Convert.ToDecimal(dr["EmployeeActualContribution"]);
                    vm.EmployerActualContribution = Convert.ToDecimal(dr["EmployerActualContribution"]);
                    vm.EmployeeActualProfitValue = Convert.ToDecimal(dr["EmployeeActualProfitValue"]);
                    vm.EmployerActualProfitValue = Convert.ToDecimal(dr["EmployerActualProfitValue"]);

                    vm.EmployeeContributionForfeitValue = Convert.ToDecimal(dr["EmployeeContributionForfeitValue"]);
                    vm.EmployeeProfitForfeitValue = Convert.ToDecimal(dr["EmployeeProfitForfeitValue"]);
                    vm.EmployerContributionForfeitValue = Convert.ToDecimal(dr["EmployerContributionForfeitValue"]);
                    vm.EmployerProfitForfeitValue = Convert.ToDecimal(dr["EmployerProfitForfeitValue"]);
                    vm.TotalForfeitValue = Convert.ToDecimal(dr["TotalForfeitValue"]);
                    vm.TotalPayableAmount = Convert.ToDecimal(dr["TotalPayableAmount"]);
                    vm.AlreadyPaidAmount = Convert.ToDecimal(dr["AlreadyPaidAmount"]);
                    vm.NetPayAmount = Convert.ToDecimal(dr["NetPayAmount"]);
                    vm.ProvidentFundAmount = Convert.ToDecimal(dr["ProvidentFundAmount"]);


                    vm.PFStartDate = Ordinary.StringToDate(dr["PFStartDate"].ToString());
                    vm.PFEndDate = Ordinary.StringToDate(dr["PFEndDate"].ToString());



                    vm.EmpDOJ = Ordinary.StringToDate(dr["EmpDOJ"].ToString());
                    vm.EmpResignDate = Ordinary.StringToDate(dr["EmpResignDate"].ToString());
                    vm.SettlementDate = Ordinary.StringToDate(dr["SettlementDate"].ToString());
                    vm.SettlementPolicyId = Convert.ToInt32(dr["SettlementPolicyId"]);
                    vm.JobAgeInMonth = Convert.ToDecimal(dr["JobAgeInMonth"]);
                    vm.EmployeeContributionRatio = Convert.ToDecimal(dr["EmployeeContributionRatio"]);
                    vm.EmployerContributionRatio = Convert.ToDecimal(dr["EmployerContributionRatio"]);

                    vm.EmployeeProfitRatio = Convert.ToDecimal(dr["EmployeeProfitRatio"]);
                    vm.EmployerProfitRatio = Convert.ToDecimal(dr["EmployerProfitRatio"]);


                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();


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
        /// Inserts a new WWF Settlement record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="PFSettlementVM"/> containing the WWF Settlement data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertPFSettlement").
        /// </returns>
        public string[] Insert(PFSettlementVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert PFSettlement"; //Method Name
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


                vm.Id = _cDal.NextId(" PFSettlements", currConn, transaction);
                if (vm != null)
                {
                    vm.TransactionCode = "STM-" + (vm.Id.ToString()).PadLeft(4, '0');

                    #region Delete Existing
                    retResults = _cDal.DeleteTableByCondition("PFSettlements", "EmployeeId", vm.EmployeeId, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #endregion

                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  PFSettlements(
Id
,TransactionType
,TransactionCode
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmployeeId
,EmployeeTotalContribution
,EmployerTotalContribution
,EmployeeProfitValue
,EmployerProfitValue
,EmployeeActualContribution
,EmployerActualContribution
,EmployeeActualProfitValue
,EmployerActualProfitValue
,EmpDOJ
,EmpResignDate
,SettlementDate
,SettlementPolicyId
,JobAgeInMonth
,EmployeeContributionRatio
,EmployerContributionRatio
,EmployeeProfitRatio
,EmployerProfitRatio
,PFStartDate
,PFEndDate


,EmployeeContributionForfeitValue
,EmployeeProfitForfeitValue
,EmployerContributionForfeitValue
,EmployerProfitForfeitValue
,TotalForfeitValue
,TotalPayableAmount
,AlreadyPaidAmount
,NetPayAmount
,ProvidentFundAmount

,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom

) VALUES (
@Id
,@TransactionType
,@TransactionCode
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmployeeId
,@EmployeeTotalContribution
,@EmployerTotalContribution
,@EmployeeProfitValue
,@EmployerProfitValue
,@EmployeeActualContribution
,@EmployerActualContribution
,@EmployeeActualProfitValue
,@EmployerActualProfitValue
,@EmpDOJ
,@EmpResignDate
,@SettlementDate
,@SettlementPolicyId
,@JobAgeInMonth
,@EmployeeContributionRatio
,@EmployerContributionRatio
,@EmployeeProfitRatio
,@EmployerProfitRatio
,@PFStartDate
,@PFEndDate

,@EmployeeContributionForfeitValue
,@EmployeeProfitForfeitValue
,@EmployerContributionForfeitValue
,@EmployerProfitForfeitValue
,@TotalForfeitValue
,@TotalPayableAmount
,@AlreadyPaidAmount
,@NetPayAmount
,@ProvidentFundAmount



,@Post
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);

                    cmdInsert.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeTotalContribution", vm.EmployeeTotalContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployerTotalContribution", vm.EmployerTotalContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployeeProfitValue", vm.EmployeeProfitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfitValue", vm.EmployerProfitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployeeActualContribution", vm.EmployeeActualContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployerActualContribution", vm.EmployerActualContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployeeActualProfitValue", vm.EmployeeActualProfitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerActualProfitValue", vm.EmployerActualProfitValue);
                    cmdInsert.Parameters.AddWithValue("@EmpDOJ", Ordinary.DateToString(vm.EmpDOJ));
                    cmdInsert.Parameters.AddWithValue("@EmpResignDate", Ordinary.DateToString(vm.EmpResignDate));
                    cmdInsert.Parameters.AddWithValue("@SettlementDate", Ordinary.DateToString(vm.SettlementDate));
                    cmdInsert.Parameters.AddWithValue("@SettlementPolicyId", vm.SettlementPolicyId);
                    cmdInsert.Parameters.AddWithValue("@JobAgeInMonth", vm.JobAgeInMonth);
                    cmdInsert.Parameters.AddWithValue("@EmployeeContributionRatio", vm.EmployeeContributionRatio);
                    cmdInsert.Parameters.AddWithValue("@EmployerContributionRatio", vm.EmployerContributionRatio);
                    cmdInsert.Parameters.AddWithValue("@EmployeeProfitRatio", vm.EmployeeProfitRatio);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfitRatio", vm.EmployerProfitRatio);
                    cmdInsert.Parameters.AddWithValue("@PFStartDate", Ordinary.DateToString(vm.PFStartDate));
                    cmdInsert.Parameters.AddWithValue("@PFEndDate", Ordinary.DateToString(vm.PFEndDate));


                    cmdInsert.Parameters.AddWithValue("@EmployeeContributionForfeitValue", vm.EmployeeContributionForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployeeProfitForfeitValue", vm.EmployeeProfitForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerContributionForfeitValue", vm.EmployerContributionForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfitForfeitValue ", vm.EmployerProfitForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@TotalForfeitValue", vm.TotalForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@TotalPayableAmount", vm.EmployeeTotalContribution + vm.EmployerTotalContribution + vm.EmployeeProfitValue + vm.EmployerProfitValue);
                    cmdInsert.Parameters.AddWithValue("@AlreadyPaidAmount", vm.AlreadyPaidAmount);
                    cmdInsert.Parameters.AddWithValue("@NetPayAmount", vm.NetPayAmount);
                    cmdInsert.Parameters.AddWithValue("@ProvidentFundAmount", vm.ProvidentFundAmount);



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
                        throw new ArgumentNullException("Unexpected error to update  PFSettlements.", "");
                    }
                    #endregion SqlExecution

                    #region Detail
                    {
                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                        {
                            PFSettlementDetailDAL _detailDAL = new PFSettlementDetailDAL();

                            foreach (PFSettlementDetailVM detailVM in vm.detailVMs)
                            {
                                detailVM.PFSettlementId = vm.Id;
                                detailVM.TransactionDate = vm.SettlementDate;
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

                    SettingDAL _settingDal = new SettingDAL();
                    string AutoJournal = _settingDal.settingValue("WWF", "AutoJournal").Trim();
                    if (AutoJournal == "Y")
                    {
                        retResults = AutoJournalSave(vm.FiscalYearDetailId, vm.EmployeeId, vm.BranchId, currConn, transaction);
                    }

                    #endregion

                }
                else
                {
                    retResults[1] = "This  PFSettlement already used!";
                    throw new ArgumentNullException("Please Input  PFSettlement Value", "");
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

        public string[] AutoJournalSave(int FiscalYearDetailId, string EmployeeId, string BranchId, SqlConnection currConn, SqlTransaction transaction)
        {
               
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message

            SettingDAL _settingDal = new SettingDAL();

            string EmployeeCOAID = _settingDal.settingValue("WWF", "EmployeeCOAID").Trim();
            string EmployerCOAID = _settingDal.settingValue("WWF", "EmployerCOAID").Trim();
            string EmployerProfitCOAID = _settingDal.settingValue("WWF", "EmployerProfitCOAID").Trim();
            string EmployeeProfitCOAID = _settingDal.settingValue("WWF", "EmployeeProfitCOAID").Trim();
            string BankCOAID = _settingDal.settingValue("WWF", "BankCOAID").Trim();
            string IsAutoJournal = _settingDal.settingValue("WWF", "IsAutoJournal").Trim();
            if (IsAutoJournal == "Y")
            {

                string id = @"SELECT
                               [FiscalYearDetailId]     
                              ,[EmployeeId]
                              ,[EmployeeProfitValue]
                              ,[EmployerProfitValue]
                              ,[EmployeeTotalContribution]
                              ,[EmployerTotalContribution]    
                          FROM [PFSettlements] where EmployeeId=@EmployeeId and FiscalYearDetailId=@FiscalYearDetailId";
                SqlCommand cmdid = new SqlCommand(id, currConn, transaction);
                cmdid.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdid.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                SqlDataAdapter adapterid = new SqlDataAdapter(cmdid);
                DataTable dtpf = new DataTable();
                adapterid.Fill(dtpf);

                GLJournalVM vmj = new GLJournalVM
                {
                    Id = 1,
                    CreatedAt = DateTime.Now.ToString(),
                    CreatedBy = "admin",
                    CreatedFrom = "",
                    TransactionDate = DateTime.Now.ToString(),
                    TransactionType = 31,
                    JournalType = 1,
                    TransType = "WWF",
                    TransactionValue = Convert.ToDecimal(dtpf.Rows[0]["EmployeeTotalContribution"].ToString()) + Convert.ToDecimal(dtpf.Rows[0]["EmployerTotalContribution"].ToString())+ Convert.ToDecimal(dtpf.Rows[0]["EmployeeProfitValue"].ToString())+ Convert.ToDecimal(dtpf.Rows[0]["EmployerProfitValue"].ToString()),

                    GLJournalDetails = new List<GLJournalDetailVM>
                    {
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployeeCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeeTotalContribution"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployerCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployerTotalContribution"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                          new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployeeProfitCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeeProfitValue"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                          new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployerProfitCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployerProfitValue"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                            new GLJournalDetailVM
                        {
                            COAId =Convert.ToInt32(BankCOAID),
                            DrAmount =Convert.ToDecimal(dtpf.Rows[0]["EmployeeTotalContribution"].ToString()) + Convert.ToDecimal(dtpf.Rows[0]["EmployerTotalContribution"].ToString())+ Convert.ToDecimal(dtpf.Rows[0]["EmployeeProfitValue"].ToString())+ Convert.ToDecimal(dtpf.Rows[0]["EmployerProfitValue"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        }
                    }
                };
                vmj.BranchId = BranchId;
                GLJournalDAL glJournalDal = new GLJournalDAL();
                retResults = glJournalDal.Insert(vmj);
            }

            #region Results
            return retResults;
            #endregion

        }
        //==================Update =================
        /// <summary>
        /// Updates an existing WWF Settlement record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the updated WWF Settlement information.</param>
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
        public string[] Update(PFSettlementVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFSettlementUpdate"; //Method Name
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
                    sqlText = "UPDATE PFSettlements SET";

                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId";
                    sqlText += " , SettlementDate=@SettlementDate";

                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);

                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@SettlementDate", Ordinary.DateToString(vm.SettlementDate));

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
                        throw new ArgumentNullException("Unexpected error to update PFSettlements.", "");
                    }
                    #endregion SqlExecution

                    #region Detail
                    {
                        if (vm.detailVMs != null && vm.detailVMs.Count > 0)
                        {
                            #region Delete First
                            sqlText = " ";
                            sqlText = "DELETE PFSettlementDetails WHERE PFSettlementId=@Id";
                            SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                            cmdDelete.Parameters.AddWithValue("@Id", vm.Id);
                            exeRes = cmdDelete.ExecuteNonQuery();
                            transResult = Convert.ToInt32(exeRes);

                            #endregion

                            #region Insert Again

                            PFSettlementDetailDAL _detailDAL = new PFSettlementDetailDAL();

                            foreach (PFSettlementDetailVM detailVM in vm.detailVMs)
                            {
                                detailVM.PFSettlementId = vm.Id;
                                detailVM.TransactionDate = vm.SettlementDate;
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



                        }
                    }
                    #endregion

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("PFSettlement Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PFSettlement Update", "Could not found any item.");
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

        ////==================PreInsert =================
        /// /// <summary>
        /// Inserts a new WWF Settlement record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="PFSettlementVM"/> containing the WWF Settlement data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertPFSettlement").
        /// </returns>
        public PFSettlementVM PreInsert(PFSettlementVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            //////PFSettlementVM vm = new PFSettlementVM();

            #endregion
            try
            {
                #region Get FiscalYearDetailId
                FiscalYearDAL _fiscalYearDAL = new FiscalYearDAL();
                FiscalYearDetailVM fydVM = new FiscalYearDetailVM();
                fydVM = _fiscalYearDAL.PeriodLockByTransactionDate(Ordinary.DateToString(vm.SettlementDate), null, null);
                if (fydVM == null)
                {
                    //////retResults[1] = "Fiscal year is not available for this Date: " + vm.SettlementDate + "! Please create!";
                    //////return retResults;
                }
                vm.FiscalYearDetailId = fydVM.Id;

                #endregion

                #region Required Data

                SettingDAL _sDAL = new SettingDAL();
                string EntitleDate = _sDAL.settingValue("WWF", "EntitleDate");

                EntitleDate = Ordinary.StringToDate(EntitleDate);

                DateTime DateEntitleDate = Convert.ToDateTime(EntitleDate);


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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPFSettlement"); }
                #endregion open connection and transaction
                #region Process

                DataTable dt = new DataTable();

                #region PFSettlements
                vm.TransactionType = "PFSettlement";

                #region Contribution

                PFSettlementVM varPFDetailVM = new PFSettlementVM();
                PFDetailDAL _PFDetailDAL = new PFDetailDAL();

                varPFDetailVM = _PFDetailDAL.SelectDetailContribution_TillMonth(vm.FiscalYearDetailId, vm.EmployeeId, currConn, transaction).FirstOrDefault();


                #endregion

                if (varPFDetailVM != null)
                {
                    #region Profit

                    vm.Code = varPFDetailVM.Code;
                    vm.Designation = varPFDetailVM.Designation;
                    vm.EmpName = varPFDetailVM.EmpName;
                    vm.Department = varPFDetailVM.Department;
                    vm.NetPayAmount = varPFDetailVM.EmployeeTotalContribution;
                    vm.TotalForfeitValue = varPFDetailVM.EmployerTotalContribution;
                    vm.ProvidentFundAmount = varPFDetailVM.EmployeeTotalContribution + varPFDetailVM.EmployerTotalContribution;
                    vm.EmployerProfitValue = varPFDetailVM.EmployerProfitValue;
                    vm.EmployeeProfitValue = varPFDetailVM.EmployeeProfitValue;
                    vm.EmployeeTotalContribution = varPFDetailVM.EmployeeTotalContribution;
                    vm.EmployerTotalContribution = varPFDetailVM.EmployerTotalContribution;
                    #endregion

                }
                #endregion

                #region Going to be Obsolete


                if (false)
                {
                    #region Insert Data

                    ////retResults = Insert(vm, currConn, transaction);

                    ////if (retResults[0] == "Fail")
                    ////{
                    ////    throw new ArgumentNullException(retResults[1], "");
                    ////}

                    #endregion
                }

                if (false)
                {
                    #region Update NetPayAmount

                    ////retResults = Update_NetPayAmount(vm.EmployeeId, currConn, transaction);

                    ////if (retResults[0] == "Fail")
                    ////{
                    ////    throw new ArgumentNullException(retResults[1], "");
                    ////}

                    #endregion
                }
                if (false)
                {

                    #region Insert into ForfeitureAccounts


                    //ForfeitureAccountDAL _forfeitureAccountDAL = new ForfeitureAccountDAL();
                    ForfeitureAccountVM forfeitureAccountVM = new ForfeitureAccountVM();

                    #region Get Data from PFSettlement

                    PFSettlementVM newPFSettlementVM = new PFSettlementVM();
                    string[] cFields = { "pfs.EmployeeId" };
                    string[] cValues = { vm.EmployeeId };
                    newPFSettlementVM = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                    #endregion

                    //////forfeitureAccountVM.Post = true;
                    forfeitureAccountVM.PFSettlementId = newPFSettlementVM.Id;//////newPFSettlementVM.Id
                    forfeitureAccountVM.ProjectId = vm.ProjectId;
                    forfeitureAccountVM.DepartmentId = vm.DepartmentId;
                    forfeitureAccountVM.SectionId = vm.SectionId;
                    forfeitureAccountVM.DesignationId = vm.DesignationId;
                    forfeitureAccountVM.EmployeeId = vm.EmployeeId;
                    forfeitureAccountVM.ForfeitDate = vm.SettlementDate;
                    forfeitureAccountVM.EmployeContributionForfeitValue = vm.EmployeeActualContribution - vm.EmployeeTotalContribution;
                    forfeitureAccountVM.EmployeProfitForfeitValue = vm.EmployeeActualProfitValue - vm.EmployeeProfitValue;
                    forfeitureAccountVM.EmployerContributionForfeitValue = vm.EmployerActualContribution - vm.EmployerTotalContribution;
                    forfeitureAccountVM.EmployerProfitForfeitValue = vm.EmployerActualProfitValue - vm.EmployerProfitValue;
                    forfeitureAccountVM.TotalForfeitValue = (
                    forfeitureAccountVM.EmployeContributionForfeitValue + forfeitureAccountVM.EmployeProfitForfeitValue
                    + forfeitureAccountVM.EmployerContributionForfeitValue + forfeitureAccountVM.EmployerProfitForfeitValue
                );

                    forfeitureAccountVM.CreatedBy = vm.CreatedBy;
                    forfeitureAccountVM.CreatedAt = vm.CreatedAt;
                    forfeitureAccountVM.CreatedFrom = vm.CreatedFrom;

                    ////retResults = _forfeitureAccountDAL.Insert(forfeitureAccountVM, currConn, transaction);

                    ////if (retResults[0] == "Fail")
                    ////{
                    ////    throw new ArgumentNullException(retResults[1], "");
                    ////}



                    #endregion
                }

                #endregion

                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

            }
            #region catch and finally
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
            }
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

        ////==================Post =================
        public string[] Post(string ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Success";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PostPFSettlement"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction
                if (ids != "")
                {
                    #region Update Settings
                    //for (int i = 0; i < ids.Length - 1; i++)
                    //{
                    #region Post

                    try
                    {
                        sqlText = " ";
                        sqlText = @"
                            Update PFSettlements SET Post=1 WHERE Id=@Id
                            ";
                        SqlCommand cmdPost = new SqlCommand(sqlText, currConn, transaction);
                        cmdPost.Parameters.AddWithValue("@Id", ids);
                        var exeRes = cmdPost.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[0] = "Fail";
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update ProfitDistributions.", "");
                        }
                    #endregion

                        #region Insert into ForfeitureAccounts

                        //ForfeitureAccountVM facVM = new ForfeitureAccountVM();

                        //#region Get Data from PFSettlement

                        //PFSettlementVM vm = new PFSettlementVM();
                        //string[] cFields = { "pfs.Id" };
                        //string[] cValues = { ids[i] };
                        //vm = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                        //#endregion

                        //facVM.Post = true;
                        //facVM.PFSettlementId = vm.Id;//////newPFSettlementVM.Id
                        //facVM.ProjectId = vm.ProjectId;
                        //facVM.DepartmentId = vm.DepartmentId;
                        //facVM.SectionId = vm.SectionId;
                        //facVM.DesignationId = vm.DesignationId;
                        //facVM.EmployeeId = vm.EmployeeId;
                        //facVM.ForfeitDate = vm.SettlementDate;
                        //facVM.EmployeContributionForfeitValue = vm.EmployeeContributionForfeitValue;
                        //facVM.EmployeProfitForfeitValue = vm.EmployeeProfitForfeitValue;
                        //facVM.EmployerContributionForfeitValue = vm.EmployerContributionForfeitValue;
                        //facVM.EmployerProfitForfeitValue = vm.EmployerProfitForfeitValue;
                        //facVM.TotalForfeitValue = vm.TotalForfeitValue;

                        //facVM.CreatedBy = vm.CreatedBy;
                        //facVM.CreatedAt = vm.CreatedAt;
                        //facVM.CreatedFrom = vm.CreatedFrom;

                        //retResults = _facDAL.Insert(facVM, currConn, transaction);

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }


                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }



                        #endregion

                    // }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("PFSettlement Information Post", "Could not found any item.");
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

        //==================Report=================
        public DataTable Report(PFSettlementVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
                #region SqlText

                sqlText = @"
SELECT

pfs.FiscalYearDetailId
,pfs.EmployeeTotalContribution
,pfs.EmployerTotalContribution
,pfs.EmployeeProfitValue
,pfs.EmployerProfitValue
,pfs.EmployeeActualContribution
,pfs.EmployerActualContribution
,pfs.EmployeeActualProfitValue
,pfs.EmployerActualProfitValue



,pfs.EmpDOJ JoinDate
,pfs.EmpResignDate LeftDate
,pfs.SettlementDate
,pfs.SettlementPolicyId
,pfs.JobAgeInMonth
,pfs.EmployeeContributionRatio
,pfs.EmployerContributionRatio
,pfs.EmployeeProfitRatio
,pfs.EmployerProfitRatio


,pfs.TotalPayableAmount
,pfs.AlreadyPaidAmount
,pfs.NetPayAmount

,pfs.PFStartDate
,pfs.PFEndDate

,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,pfs.Post

,pfs.Remarks
FROM  PFSettlements  pfs

";
                sqlText += " LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfs.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 ";

                if (vm.Id > 0)
                {
                    sqlText += " AND pfs.Id=@Id ";
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
                sqlText += " ORDER BY ve.Code ";

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

                if (vm.Id > 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                da.Fill(dt);
                string[] dateColumns = { "JoinDate", "LeftDate", "SettlementDate", "PFStartDate", "PFEndDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumns);
                #endregion SqlExecution

                if (transaction != null)
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        ////==================Payment =================
        public string[] Update_NetPayAmount(string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Update_NetPayAmount"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                #endregion open connection and transaction
                #region Update
                sqlText = " ";
                sqlText = @"
--------DECLARE @EmployeeId as varchar(100)
--------set @EmployeeId = '1_172'

--------------------------------------------------------------------------------
----------------------------EmployeePaymentCTE----------------------------------
;WITH 
EmployeePaymentCTE AS
(
SELECT
EmployeeId
,ISNULL(SUM(IndividualProfitValue),0) AlreadyPaidAmount

FROM ProfitDistributionDetails
WHERE 1=1
AND EmployeeId=@EmployeeId
AND IsPaid = 1
GROUP BY EmployeeId

)

----SELECT * FROM EmployeePayment

--------------------------------------------------------------------------------
-------------------------------PFSettlementCTE----------------------------------
, PFSettlementCTE AS
(
SELECT
s.EmployeeId
,(s.EmployeeProfitValue+s.EmployerProfitValue+s.EmployeeTotalContribution+s.EmployerTotalContribution) TotalPayableAmount

FROM PFSettlements s
WHERE 1=1 
AND s.EmployeeId=@EmployeeId
)


UPDATE PFSettlements SET 
TotalPayableAmount = scte.TotalPayableAmount
,AlreadyPaidAmount = ep.AlreadyPaidAmount
,NetPayAmount = (scte.TotalPayableAmount-ep.AlreadyPaidAmount)

FROM PFSettlementCTE scte
LEFT OUTER JOIN EmployeePaymentCTE ep ON scte.EmployeeId=ep.EmployeeId
WHERE 1=1 
AND PFSettlements.EmployeeId=scte.EmployeeId
AND PFSettlements.EmployeeId=@EmployeeId

";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                    sqlText = sqlText.Replace("s.EmployeeId=@EmployeeId", "1=1");
                    sqlText = sqlText.Replace("PFSettlements.EmployeeId=@EmployeeId", "1=1");
                }
                SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                cmdDelete.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                var exeRes = cmdDelete.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Unexpected error to update PFSettlements.", "");
                }
                retResults[2] = "";// Return Id
                retResults[3] = sqlText; //  SQL Query
                #endregion

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Date Update Successfully!";
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
            retResults[0] = "Fail";
            retResults[1] = "Fail";

            string EmployeeProfitCOAID = "";
            string EmployerProfitCOAID = "";
            string EmployerCOAID = "";
            string EmployeeCOAID = "";
            string BankCOAID = "";

            string Journal = @"SELECT
                                JournalName,COAID
                                FROM AutoJournalSetup
                                WHERE  1=1 AND JournalFor = @JournalFor and IsActive=1";
            SqlCommand cmdj = new SqlCommand(Journal, currConn, transaction);
            cmdj.Parameters.AddWithValue("JournalFor", TransactionForm);
            SqlDataAdapter adapterj = new SqlDataAdapter(cmdj);
            DataTable dtj = new DataTable();
            adapterj.Fill(dtj);
            if (dtj.Rows.Count > 0)
            {
                EmployerCOAID = dtj.Rows[0]["COAID"].ToString();
                EmployeeCOAID = dtj.Rows[1]["COAID"].ToString();              
                EmployeeProfitCOAID = dtj.Rows[2]["COAID"].ToString();
                EmployerProfitCOAID = dtj.Rows[3]["COAID"].ToString();
                BankCOAID = dtj.Rows[4]["COAID"].ToString();
            }

            SettingDAL _settingDal = new SettingDAL();
            string IsAutoJournal = _settingDal.settingValue("WWF", "IsAutoJournal").Trim();

            if (IsAutoJournal == "Y")
            {

                string id = @"SELECT 
       SettlementDate
       ,EmployeeProfitValue
      ,EmployerProfitValue
      ,EmployeeTotalContribution
      ,EmployerTotalContribution
	  ,EmployeeProfitValue
      +EmployerProfitValue
      +EmployeeTotalContribution
      +EmployerTotalContribution AS Total    
  FROM PFSettlements ps
  Left Outer Join EmployeeInfo ei on ei.Id=ps.EmployeeId
  where ei.Code=@Code";
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
                    TransactionDate = dtpf.Rows[0]["SettlementDate"].ToString(),
                    TransactionType = 31,
                    JournalType = 1,
                    TransType = "WWF",
                    TransactionValue = Convert.ToDecimal(dtpf.Rows[0]["Total"].ToString()),
                   

                    GLJournalDetails = new List<GLJournalDetailVM>
                    {
                         new GLJournalDetailVM
                        {
                            COAId =Convert.ToInt32(BankCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["Total"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        }
                        ,
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployeeCOAID),
                            DrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeeTotalContribution"].ToString()),
                            IsDr = true,
                            IsYearClosing = false,
                            Post = false
                        },
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployerCOAID),
                            DrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployerTotalContribution"].ToString()),
                            IsDr = true,
                            IsYearClosing = false,
                            Post = false
                        },
                       
                        new GLJournalDetailVM
                        {
                            COAId =Convert.ToInt32(EmployeeProfitCOAID),
                            DrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeeProfitValue"].ToString()),
                            IsDr = true,
                            IsYearClosing = false,
                            Post = false
                        }
                                                ,
                        new GLJournalDetailVM
                        {
                            COAId =Convert.ToInt32(EmployerProfitCOAID),
                            DrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployerProfitValue"].ToString()),
                            IsDr = true,
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
