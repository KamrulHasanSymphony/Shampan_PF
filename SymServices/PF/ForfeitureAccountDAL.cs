using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
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
    public class ForfeitureAccountDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================SelectAllNotTransferPDF=================
        public List<ForfeitureAccountVM> SelectAllNotTransferPDF(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ForfeitureAccountVM> VMs = new List<ForfeitureAccountVM>();
            ForfeitureAccountVM vm;
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
 fa.Id
,fa.PFSettlementId
,fa.ProjectId
,fa.DepartmentId
,fa.SectionId
,fa.DesignationId
,fa.EmployeeId
,fa.ForfeitDate
,fa.EmployeContributionForfeitValue
,fa.EmployeProfitForfeitValue
,fa.EmployerContributionForfeitValue
,fa.EmployerProfitForfeitValue
,fa.Remarks
,fa.IsTransferPDF
,fa.Post
,fa.IsActive
,fa.IsArchive
,fa.CreatedBy
,fa.CreatedAt
,fa.CreatedFrom
,fa.LastUpdateBy
,fa.LastUpdateAt
,fa.LastUpdateFrom
from ForfeitureAccounts fa
WHERE  1=1 AND fa.IsTransferPDF = 0
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
                    vm = new ForfeitureAccountVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PFSettlementId = Convert.ToInt32(dr["PFSettlementId"]);
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ForfeitDate = Ordinary.StringToDate(dr["ForfeitDate"].ToString());
                    vm.EmployeContributionForfeitValue = Convert.ToDecimal(dr["EmployeContributionForfeitValue"]);
                    vm.EmployeProfitForfeitValue = Convert.ToDecimal(dr["EmployeProfitForfeitValue"]);
                    vm.EmployerContributionForfeitValue = Convert.ToDecimal(dr["EmployerContributionForfeitValue"]);
                    vm.EmployerProfitForfeitValue = Convert.ToDecimal(dr["EmployerProfitForfeitValue"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsTransferPDF = Convert.ToBoolean(dr["IsTransferPDF"]);
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

        //==================SelectEmployeeList=================
        public List<ForfeitureAccountVM> SelectAll_ResignEmployee(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ForfeitureAccountVM> VMs = new List<ForfeitureAccountVM>();
            ForfeitureAccountVM vm;
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
SELECT DISTINCT 
ve.Id
,ve.EmployeeId
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.LeftDate
";
                sqlText += " FROM " + hrmDB + ".[dbo].ViewEmployeeInformation ve";
                sqlText += @" WHERE  1=1 AND ve.IsActive = 0  AND ve.EmployeeId <> '1_0'
";
                sqlText += @" 
AND ve.EmployeeId NOT IN (SELECT EmployeeId FROM ForfeitureAccounts WHERE 1=1 AND Post = 1)
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
                    vm = new ForfeitureAccountVM();
                    ////////vm.Id = dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.EmpResignDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
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

        //==================SelectAll=================
        public List<ForfeitureAccountVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ForfeitureAccountVM> VMs = new List<ForfeitureAccountVM>();
            ForfeitureAccountVM vm;
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
                sqlText = @"
SELECT
 fa.Id
,fa.PFSettlementId
,fa.ProjectId
,fa.DepartmentId
,fa.SectionId
,fa.DesignationId
,fa.EmployeeId
,fa.ForfeitDate
,fa.EmployeContributionForfeitValue
,fa.EmployeProfitForfeitValue
,fa.EmployerContributionForfeitValue
,fa.EmployerProfitForfeitValue
,ISNULL(fa.TotalForfeitValue,0)TotalForfeitValue

,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.LeftDate

,fa.Remarks
,fa.IsTransferPDF
,fa.Post
,fa.IsActive
,fa.IsArchive
,fa.CreatedBy
,fa.CreatedAt
,fa.CreatedFrom
,fa.LastUpdateBy
,fa.LastUpdateAt
,fa.LastUpdateFrom

FROM ForfeitureAccounts fa
";
                 sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON ve.EmployeeId=fa.EmployeeId";
                 sqlText += @"
WHERE  1=1
";

                if (Id > 0)
                {
                    sqlText += @" and fa.Id=@Id";
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
                    vm = new ForfeitureAccountVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PFSettlementId = Convert.ToInt32(dr["PFSettlementId"]);

                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.ForfeitDate = Ordinary.StringToDate(dr["ForfeitDate"].ToString());
                    vm.EmployeContributionForfeitValue = Convert.ToDecimal(dr["EmployeContributionForfeitValue"]);
                    vm.EmployeProfitForfeitValue = Convert.ToDecimal(dr["EmployeProfitForfeitValue"]);
                    vm.EmployerContributionForfeitValue = Convert.ToDecimal(dr["EmployerContributionForfeitValue"]);
                    vm.EmployerProfitForfeitValue = Convert.ToDecimal(dr["EmployerProfitForfeitValue"]);
                    vm.TotalForfeitValue = Convert.ToDecimal(dr["TotalForfeitValue"]);

                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.EmpResignDate = Ordinary.StringToDate(dr["LeftDate"].ToString());

                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsTransferPDF = Convert.ToBoolean(dr["IsTransferPDF"]);
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(dr["CreatedAt"].ToString())))));
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(Ordinary.StringToDate(dr["LastUpdateAt"].ToString())))));
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

        //==================PreInsert =================
        public ForfeitureAccountVM PreInsert(ForfeitureAccountVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                string hrmDB = _dbsqlConnection.HRMDB;

                #region sql statement
                sqlText = @"
--------declare @EmployeeId as varchar(100)
--------set @EmployeeId = '1_1'

---------------------PF Forfeiture------------------------------
----------------------------------------------------------------
;WITH 

PFForfeiture AS
(
SELECT EmployeeId, SUM(EmployeerPFValue) TotalEmployeerPFValue, SUM(EmployeePFValue) TotalEmployeePFValue   
FROM PFDetails
WHERE 1=1 AND Post=1
AND EmployeeId=@EmployeeId
GROUP BY EmployeeId
)



SELECT 
ve.EmployeeId
,ISNULL(f.TotalEmployeerPFValue,0) TotalEmployeerPFValue
,ISNULL(f.TotalEmployeePFValue,0) TotalEmployeePFValue
,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.LeftDate

";
                sqlText += " FROM " + hrmDB + ".[dbo].ViewEmployeeInformation ve ";
                sqlText += @"
LEFT OUTER JOIN PFForfeiture f ON f.EmployeeId = ve.EmployeeId
WHERE 1=1
AND ve.EmployeeId=@EmployeeId
";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeContributionForfeitValue = Convert.ToDecimal(dr["TotalEmployeerPFValue"]);
                    vm.TotalForfeitValue = vm.EmployeContributionForfeitValue;

                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.EmpResignDate = Ordinary.StringToDate(dr["LeftDate"].ToString());


                    ////vm.EmployeContributionForfeitValue = Convert.ToDecimal(dr["EmployeContributionForfeitValue"]);
                    ////vm.EmployeProfitForfeitValue = Convert.ToDecimal(dr["EmployeProfitForfeitValue"]);
                    ////vm.EmployerContributionForfeitValue = Convert.ToDecimal(dr["EmployerContributionForfeitValue"]);
                    //////vm.EmployerProfitForfeitValue = Convert.ToDecimal(dr["EmployerProfitForfeitValue"]);

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
            return vm;
        }

        //==================Insert =================
        public string[] Insert(ForfeitureAccountVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertForfeitureAcount"; //Method Name
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


                vm.Id = _cDal.NextId("ForfeitureAccounts", currConn, transaction);
                if (vm != null)
                {
                    #region Delete Existing
                    retResults = _cDal.DeleteTableByCondition("ForfeitureAccounts", "EmployeeId", vm.EmployeeId, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #endregion
                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO ForfeitureAccounts(
Id
,PFSettlementId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmployeeId
,ForfeitDate
,EmployeContributionForfeitValue
,EmployeProfitForfeitValue
,EmployerContributionForfeitValue
,EmployerProfitForfeitValue
,TotalForfeitValue
,Post
,IsTransferPDF

,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
) VALUES (
@Id
,@PFSettlementId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmployeeId
,@ForfeitDate
,@EmployeContributionForfeitValue
,@EmployeProfitForfeitValue
,@EmployerContributionForfeitValue
,@EmployerProfitForfeitValue
,@TotalForfeitValue
,@Post
,@IsTransferPDF
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";
                    #endregion
                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@PFSettlementId", vm.PFSettlementId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId ?? "0");
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@ForfeitDate", Ordinary.DateToString(vm.ForfeitDate));
                    cmdInsert.Parameters.AddWithValue("@EmployeContributionForfeitValue", vm.EmployeContributionForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployeProfitForfeitValue", vm.EmployeProfitForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerContributionForfeitValue", vm.EmployerContributionForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfitForfeitValue", vm.EmployerProfitForfeitValue);
                    cmdInsert.Parameters.AddWithValue("@TotalForfeitValue", vm.TotalForfeitValue);
                    
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
                        throw new ArgumentNullException("Unexpected error to update ForfeitureAccounts.", "");
                    }
                    #endregion
                }
                else
                {
                    retResults[1] = "This ForfeitureAcount already used!";
                    throw new ArgumentNullException("Please Input ForfeitureAcount Value", "");
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
        public string[] Update(ForfeitureAccountVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee ForfeitureAcount Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToForfeitureAcount"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE ForfeitureAccounts SET";
                    sqlText += "  ProjectId=@ProjectId";

                    sqlText += " , PFSettlementId=@PFSettlementId";
                    sqlText += " , DepartmentId=@DepartmentId";
                    sqlText += " , SectionId=@SectionId";
                    sqlText += " , DesignationId=@DesignationId";
                    sqlText += " , EmployeeId=@EmployeeId";
                    sqlText += " , ForfeitDate=@ForfeitDate";
                    sqlText += " , EmployeContributionForfeitValue=@EmployeContributionForfeitValue";
                    sqlText += " , EmployeProfitForfeitValue=@EmployeProfitForfeitValue";
                    sqlText += " , EmployerContributionForfeitValue=@EmployerContributionForfeitValue";
                    sqlText += " , EmployerProfitForfeitValue=@EmployerProfitForfeitValue";
                    sqlText += " , TotalForfeitValue=@TotalForfeitValue";
                    
                    sqlText += " , Post=@Post";
                    sqlText += " , IsTransferPDF=@IsTransferPDF";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@PFSettlementId", vm.PFSettlementId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId ?? "0");
                    cmdUpdate.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId ?? "0");
                    cmdUpdate.Parameters.AddWithValue("@SectionId", vm.SectionId ?? "0");
                    cmdUpdate.Parameters.AddWithValue("@DesignationId", vm.DesignationId ?? "0");
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@ForfeitDate", Ordinary.DateToString(vm.ForfeitDate));
                    cmdUpdate.Parameters.AddWithValue("@EmployeContributionForfeitValue", vm.EmployeContributionForfeitValue);
                    cmdUpdate.Parameters.AddWithValue("@EmployeProfitForfeitValue", vm.EmployeProfitForfeitValue);
                    cmdUpdate.Parameters.AddWithValue("@EmployerContributionForfeitValue", vm.EmployerContributionForfeitValue);
                    cmdUpdate.Parameters.AddWithValue("@EmployerProfitForfeitValue", vm.EmployerProfitForfeitValue);
                    cmdUpdate.Parameters.AddWithValue("@TotalForfeitValue", vm.TotalForfeitValue);

                    
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
                        throw new ArgumentNullException("Unexpected error to update ForfeitureAccounts.", "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("ForfeitureAcount Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ForfeitureAcount Update", "Could not found any item.");
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
        public string[] Delete(ForfeitureAccountVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteForfeitureAcount"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToForfeitureAcount"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Check Posted or Not Posted
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retVal = _cDal.SelectFieldValue("ForfeitureAccounts", "Post", "Id", ids[i].ToString(), currConn, transaction);
                        vm.Post = Convert.ToBoolean(retVal);
                        if (vm.Post == true)
                        {
                            retResults[0] = "Fail";
                            retResults[1] = "Data Alreday Posted! Cannot be Deleted.";
                            throw new ArgumentNullException(retResults[1], "");
                        }
                    }
                    #endregion Check Posted or Not Posted


                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = " ";
                        sqlText = "DELETE ForfeitureAccounts";
                        sqlText += " WHERE Id=@Id";
                        SqlCommand cmdDelete = new SqlCommand(sqlText, currConn, transaction);
                        cmdDelete.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdDelete.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            throw new ArgumentNullException("Unexpected error to update ForfeitureAccounts.", "");
                        }
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("ForfeitureAcount Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("ForfeitureAcount Information Delete", "Could not found any item.");
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
            retResults[5] = "PostForfeitureAccount"; //Method Name
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
                        retResults = _cDal.FieldPost("ForfeitureAccounts", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("ForfeitureAccounts Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("ForfeitureAccount Information Post - Could not found any item.", "");
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
        public DataTable Report(ForfeitureAccountVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 fa.Id
,fa.PFSettlementId
,fa.ProjectId
,fa.DepartmentId
,fa.SectionId
,fa.DesignationId
,fa.EmployeeId
,fa.ForfeitDate
,fa.EmployeContributionForfeitValue
,fa.EmployeProfitForfeitValue
,fa.EmployerContributionForfeitValue
,fa.EmployerProfitForfeitValue
,fa.Remarks
,fa.IsTransferPDF
,fa.Post
from ForfeitureAccounts fa
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


        #endregion
    }
}
