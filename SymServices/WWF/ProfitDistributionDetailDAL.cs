using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.WPPF;
using System.Linq;
using System.Threading;

namespace SymServices.WPPF
{
    public class ProfitDistributionDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        public static Thread thread;

        #endregion
        #region Methods

        //==================SelectByMasterId=================
        public List<ProfitDistributionDetailVM> SelectByMasterId(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionDetailVM> VMs = new List<ProfitDistributionDetailVM>();
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
                string[] conditionField = { "pdd.ProfitDistributionId" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        
        //==================SelectById=================
        public List<ProfitDistributionDetailVM> SelectById(int Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionDetailVM> VMs = new List<ProfitDistributionDetailVM>();
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
                string[] conditionField = { "pdd.Id" };
                string[] conditionValue = { Id.ToString() };
                VMs = SelectAll(conditionField, conditionValue, currConn, transaction);
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
        public List<ProfitDistributionDetailVM> SelectAll(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionDetailVM> VMs = new List<ProfitDistributionDetailVM>();
            ProfitDistributionDetailVM vm;
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

 pdd.Id
,pdd.ProfitDistributionId
,pdd.FiscalYearDetailId
,pdd.FiscalYearDetailIdTo
,pdd.ProjectId
,pdd.DepartmentId
,pdd.SectionId
,pdd.DesignationId
,pdd.EmployeeId
,pdd.EmployeeProfitValue
,pdd.EmployerProfitValue
,pdd.EmployeeTotalContribution
,pdd.EmployerTotalContribution

,pdd.IndividualTotalContribution
,pdd.ServiceLengthMonthWeight
,pdd.IndividualWeightedContribution
,pdd.MultiplicationFactor
,pdd.IndividualProfitValue
,pdd.ServiceLengthMonth


,ve.EmpName 
,ve.Code 
,ve.Designation
,ve.Department
,ve.Section
,ve.Project


,ISNULL(pdd.IsPaid,0) IsPaid
,pdd.Post

,pdd.Remarks,pdd.IsActive,pdd.IsArchive,pdd.CreatedBy,pdd.CreatedAt,pdd.CreatedFrom,pdd.LastUpdateBy,pdd.LastUpdateAt,pdd.LastUpdateFrom
FROM  ProfitDistributionDetails  pdd

";
                sqlText += " LEFT OUTER JOIN " + hrmDB + ".[dbo].ViewEmployeeInformation ve ON pdd.EmployeeId=ve.EmployeeId";
                sqlText += " WHERE  1=1 ";

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
                    vm = new ProfitDistributionDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ProfitDistributionId = Convert.ToInt32(dr["ProfitDistributionId"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.EmployeeProfitValue = Convert.ToDecimal(dr["EmployeeProfitValue"]);
                    vm.EmployerProfitValue = Convert.ToDecimal(dr["EmployerProfitValue"]);
                    vm.EmployeeTotalContribution = Convert.ToDecimal(dr["EmployeeTotalContribution"]);
                    vm.EmployerTotalContribution = Convert.ToDecimal(dr["EmployerTotalContribution"]);

                    vm.IndividualTotalContribution = Convert.ToDecimal(dr["IndividualTotalContribution"]);
                    vm.ServiceLengthMonthWeight = Convert.ToDecimal(dr["ServiceLengthMonthWeight"]);
                    vm.IndividualWeightedContribution = Convert.ToDecimal(dr["IndividualWeightedContribution"]);
                    vm.MultiplicationFactor = Convert.ToDecimal(dr["MultiplicationFactor"]);
                    vm.IndividualProfitValue = Convert.ToDecimal(dr["IndividualProfitValue"]);
                    vm.ServiceLengthMonth = Convert.ToDecimal(dr["ServiceLengthMonth"]);


                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();

                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);


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

        public ProfitDistributionDetailVM Select_Summary(string EmployeeId, string IsPaid, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            ProfitDistributionDetailVM vm = new ProfitDistributionDetailVM();
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
--------DECLARE @EmployeeId as varchar(100)
--------set @EmployeeId = '1_172'


SELECT
pdd.EmployeeId
,ISNULL(SUM(pdd.EmployeeProfitValue	   ),0)EmployeeProfitValue
,ISNULL(SUM(pdd.EmployerProfitValue	   ),0)EmployerProfitValue
,ISNULL(SUM(pdd.EmployeeTotalContribution ),0)EmployeeTotalContribution
,ISNULL(SUM(pdd.EmployerTotalContribution ),0)EmployerTotalContribution
,ISNULL(SUM(pdd.IndividualProfitValue	  ),0)IndividualProfitValue

FROM ProfitDistributionDetails pdd
WHERE  1=1 
AND Post=1
AND EmployeeId=@EmployeeId
AND IsPaid=1
GROUP BY pdd.EmployeeId

";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }
                if (string.IsNullOrWhiteSpace(IsPaid))
                {
                    sqlText = sqlText.Replace("IsPaid=1", "1=1");
                }
                #endregion SqlText
                #region SqlExecution

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm.Parameters.AddWithValue("@IsPaid", IsPaid);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.EmployeeProfitValue = Convert.ToDecimal(dr["EmployeeProfitValue"]);
                    vm.EmployerProfitValue = Convert.ToDecimal(dr["EmployerProfitValue"]);
                    vm.EmployeeTotalContribution = Convert.ToDecimal(dr["EmployeeTotalContribution"]);
                    vm.EmployerTotalContribution = Convert.ToDecimal(dr["EmployerTotalContribution"]);

                    vm.IndividualProfitValue = Convert.ToDecimal(dr["IndividualProfitValue"]);

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


        //==================Insert =================
        public string[] InsertBackup(ProfitDistributionDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert ProfitDistributionDetail"; //Method Name
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


                vm.Id = _cDal.NextId(" ProfitDistributionDetails", currConn, transaction);
                if (vm != null)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  ProfitDistributionDetails(
Id
,ProfitDistributionId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmployeeId
,EmployeeProfitValue
,EmployerProfitValue
,EmployeeTotalContribution
,EmployerTotalContribution

,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom

) VALUES (
@Id
,@ProfitDistributionId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmployeeId
,@EmployeeProfitValue
,@EmployerProfitValue
,@EmployeeTotalContribution
,@EmployerTotalContribution

,@Post
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@ProfitDistributionId", vm.ProfitDistributionId);

                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                    cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                    cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                    cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeProfitValue", vm.EmployeeProfitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfitValue", vm.EmployerProfitValue);
                    cmdInsert.Parameters.AddWithValue("@EmployeeTotalContribution", vm.EmployeeTotalContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployerTotalContribution", vm.EmployerTotalContribution);

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
                        throw new ArgumentNullException("Unexpected error to update  ProfitDistributionDetails.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  ProfitDistributionDetail already used!";
                    throw new ArgumentNullException("Please Input  ProfitDistributionDetail Value", "");
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

        //==================Insert List =================
        public string[] Insert(List<ProfitDistributionDetailVM> VMs, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "Insert ProfitDistributionDetail"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int NextId = 0;
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


                NextId = _cDal.NextId("ProfitDistributionDetails", currConn, transaction);
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO  ProfitDistributionDetails(
Id
,ProfitDistributionId
,FiscalYearDetailId
,ProjectId
,DepartmentId
,SectionId
,DesignationId
,EmployeeId
,EmployeeProfitValue
,EmployerProfitValue
,EmployeeTotalContribution
,EmployerTotalContribution

,FiscalYearDetailIdTo
,IndividualTotalContribution
,ServiceLengthMonthWeight
,IndividualWeightedContribution
,MultiplicationFactor
,IndividualProfitValue
,ServiceLengthMonth


,Post
,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom

) VALUES (
@Id
,@ProfitDistributionId
,@FiscalYearDetailId
,@ProjectId
,@DepartmentId
,@SectionId
,@DesignationId
,@EmployeeId
,@EmployeeProfitValue
,@EmployerProfitValue
,@EmployeeTotalContribution
,@EmployerTotalContribution

,@FiscalYearDetailIdTo
,@IndividualTotalContribution
,@ServiceLengthMonthWeight
,@IndividualWeightedContribution
,@MultiplicationFactor
,@IndividualProfitValue
,@ServiceLengthMonth

,@Post
,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
) 
";

                    #endregion SqlText
                    #region SqlExecution
                    foreach (ProfitDistributionDetailVM vm in VMs)
                    {
                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                        cmdInsert.Parameters.AddWithValue("@Id", NextId);
                        cmdInsert.Parameters.AddWithValue("@ProfitDistributionId", vm.ProfitDistributionId);

                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                        cmdInsert.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                        cmdInsert.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                        cmdInsert.Parameters.AddWithValue("@SectionId", vm.SectionId);
                        cmdInsert.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                        cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdInsert.Parameters.AddWithValue("@EmployeeProfitValue", vm.EmployeeProfitValue);
                        cmdInsert.Parameters.AddWithValue("@EmployerProfitValue", vm.EmployerProfitValue);
                        cmdInsert.Parameters.AddWithValue("@EmployeeTotalContribution", vm.EmployeeTotalContribution);
                        cmdInsert.Parameters.AddWithValue("@EmployerTotalContribution", vm.EmployerTotalContribution);

                        cmdInsert.Parameters.AddWithValue("@FiscalYearDetailIdTo", vm.FiscalYearDetailIdTo);
                        cmdInsert.Parameters.AddWithValue("@IndividualTotalContribution", vm.IndividualTotalContribution);
                        cmdInsert.Parameters.AddWithValue("@ServiceLengthMonthWeight", vm.ServiceLengthMonthWeight);
                        cmdInsert.Parameters.AddWithValue("@IndividualWeightedContribution", vm.IndividualWeightedContribution);
                        cmdInsert.Parameters.AddWithValue("@MultiplicationFactor", vm.MultiplicationFactor);
                        cmdInsert.Parameters.AddWithValue("@IndividualProfitValue", vm.IndividualProfitValue);
                        cmdInsert.Parameters.AddWithValue("@ServiceLengthMonth", vm.ServiceLengthMonth);



                        cmdInsert.Parameters.AddWithValue("@Post", false);

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

                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update  ProfitDistributionDetails.", "");
                    }
                    #endregion SqlExecution
                }
                else
                {
                    retResults[1] = "This  ProfitDistributionDetail already used!";
                    throw new ArgumentNullException("Please Input  ProfitDistributionDetail Value", "");
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
                retResults[2] = VMs.FirstOrDefault().ProfitDistributionId.ToString();
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                retResults[2] = VMs.FirstOrDefault().ProfitDistributionId.ToString();
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
        #endregion

    }
}
