using SymOrdinary;
using SymServices.Common;
using SymViewModel.Attendance;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.Leave;
using SymViewModel.Payroll;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace SymServices.Common
{
    public class EmployeeInfoDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        //==================SelectAll=================
        public DataTable SelectEmpForAttendance(string EmployeeId, string Date, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText = @"
------DECLARE @EmployeeId as varchar(100)
------DECLARE @IncrementDate as varchar(100)
------SET @EmployeeId='1_26'
------SET @IncrementDate='20181101'

SELECT j.*, d.DinnerAmount,IfterAmount,TiffinAmount,ETiffinAmount
FROM
(
SELECT 
ej.EmployeeId
,SUM(CASE WHEN essd.SalaryType = 'Gross' THEN essd.Amount ELSE 0 END) GrossSalary
,SUM(CASE WHEN essd.SalaryType = 'Basic' THEN essd.Amount ELSE 0 END) BasicSalary

FROM employeejob ej
LEFT OUTER JOIN EmployeeSalaryStructureDetail essd ON ej.EmployeeId=essd.EmployeeId AND essd.SalaryType IN ('Gross','Basic')
WHERE  ej.EmployeeId=@EmployeeId
AND essd.IncrementDate <= @IncrementDate

GROUP BY ej.EmployeeId
) AS j

LEFT OUTER JOIN EmployeePromotion p ON p.employeeid=j.employeeid
LEFT OUTER JOIN Designation d ON p.designationId=d.id AND p.isCurrent=1

WHERE j.EmployeeId=@EmployeeId
";
                SqlCommand objComm = new SqlCommand();

                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                objComm.Parameters.AddWithValue("@IncrementDate", Ordinary.DateToString(Date));

                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(dt);

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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            return dt;
        }

        public DataTable SelectEmpForAttendance(string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText = @"
--declare @EmployeeId as varchar(100)
--set @EmployeeId='1_538'

 select d.DinnerAmount,IfterAmount,TiffinAmount,ETiffinAmount,j.GrossSalary,j.BasicSalary from employeejob j
left outer join EmployeePromotion p on p.employeeid=j.employeeid
left outer join Designation d on p.designationId=d.id and p.isCurrent=1
 where j.EmployeeId=@EmployeeId
";
                SqlCommand objComm = new SqlCommand();

                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(dt);

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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            return dt;
        }

        public string SelectEmpByCode(string Code, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string attnUserId = ""; // Variable to hold the result
            try
            {
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
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

                sqlText = @"Select Id from EmployeeInfo where Code=@Code ";
                SqlCommand objComm = new SqlCommand();

                objComm.Parameters.AddWithValue("@Code", Code);

                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;

                // Execute the query and fetch the single result as a string
                object result = objComm.ExecuteScalar();
                if (result != null)
                {
                    attnUserId = result.ToString();
                }
            }
            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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

            return attnUserId;
        }

        public List<EmployeeInfoVM> SelectAll(EmployeeInfoVM paramVM, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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

                sqlText = @"SELECT *
From ViewEmployeeInformation
Where 1=1 
-- IsArchive=0 and IsActive=1
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

                sqlText += "     ORDER By Code, Department, Section, EmpName desc";

                #endregion
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
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeInfoVM();
                        vm.Id = dr["Id"].ToString();
                        vm.EmployeeId = dr["EmployeeId"].ToString();
                        vm.Code = dr["Code"].ToString();
                        vm.EmpName = dr["EmpName"].ToString();
                        vm.JoinDate = dr["JoinDate"].ToString();
                        vm.Project = dr["Project"].ToString();
                        vm.Department = dr["Department"].ToString();
                        vm.Section = dr["Section"].ToString();
                        vm.Designation = dr["Designation"].ToString();
                        vm.ProjectId = dr["ProjectId"].ToString();
                        vm.SectionId = dr["SectionId"].ToString();
                        vm.DepartmentId = dr["DepartmentId"].ToString();
                        vm.DesignationId = dr["DesignationId"].ToString();
                        vm.GradeId = dr["GradeId"].ToString();
                        vm.StepSL = dr["StepSL"].ToString();
                        vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                        vm.AttnUserId = dr["AttnUserId"].ToString();
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                        //vm.EmployeeSalaryStructureId = dr["EmployeeSalaryStructureId"].ToString();
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                #endregion

                #endregion
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
            #region Catch and Finall
            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
            }
            #endregion
            #region Finally
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

        public DataTable SelectAll_DT(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 ve.EmployeeId
,ve.Code
,ve.EmpName
,ve.Department
,ve.Designation
------,ve.JoinDate
------,ve.Project 
------,ve.Section
------,j.Other1
------,j.Other2
------,j.Other3

FROM ViewEmployeeInformation ve
LEFT OUTER JOIN employeeJob j ON j.EmployeeId=ve.EmployeeId
LEFT OUTER JOIN Designation AS desig ON ve.DesignationId = desig.Id
WHERE 1=1
";

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
                {
                    string MultipleOther3ConditionValues = "";
                    MultipleOther3ConditionValues = vm.MultipleOther3.Replace(",", "','");
                    sqlText += "  AND j.other3 IN('" + MultipleOther3ConditionValues + "')";
                }

                #region Ordering
                if (!string.IsNullOrWhiteSpace(vm.OrderBy))
                {

                    if (vm.OrderBy == "DCG")
                        sqlText += " order by ve.Department, ve.code";
                    else if (vm.OrderBy == "DDC")
                        sqlText += " order by ve.Department, ve.JoinDate, ve.code";
                    else if (vm.OrderBy == "DGC")
                        sqlText += " order by ve.Department, ve.code";
                    else if (vm.OrderBy == "DGDC")
                        sqlText += " order by  ve.Department, ve.JoinDate, ve.code";
                    else if (vm.OrderBy == "CODE")
                        sqlText += " order by ve.code";
                    else if (vm.OrderBy.ToLower() == "designation")
                        sqlText += " order by ISNULL(desig.PriorityLevel,0), ve.code";
                }
                #endregion


                #endregion

                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;


                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                da.Fill(dt);

                #endregion

                #endregion
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
            #region Catch and Finall
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
            }
            #endregion
            #region Finally
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
            return dt;
        }

        public List<EmployeeInfoVM> SelectAllEmployee_SalaryProcess(EmployeeInfoVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                #region EmployeeList

                #region SqlText

                sqlText = @"
SELECT  
ve.Id
,ve.ProjectId
,ve.DepartmentId
,ve.SectionId
,ve.DesignationId
,ve.GradeId
,ve.IsActive
,ve.IsPermanent
,ve.IsPFApplicable
,ve.Code
,ISNULL(fs.Gross,0) GrossSalary
,ISNULL(fs.Basic,0) BasicSalary

FROM ViewEmployeeInformation ve
LEFT OUTER JOIN
(
";
                if (!string.IsNullOrWhiteSpace(paramVM.CompanyName) && paramVM.CompanyName.ToLower() == "tib")
                {
                    sqlText += @"
SELECT Salary.EmployeeId
,SUM(Salary.Basic)Basic
,SUM(Salary.HouseRent)HouseRent
,SUM(Salary.Medical)Medical
,SUM(Salary.Conveyance)Conveyance
,SUM(Salary.Gross)Gross
FROM(
 SELECT 
ssd.EmployeeId
,ISNULL(CASE WHEN  ssd.SalaryType = 'Basic' THEN ssd.Amount  else 0 END, 0) AS Basic
,ISNULL(CASE WHEN   ssd.SalaryType = 'HouseRent' THEN ssd.Amount else 0 END, 0) AS HouseRent
,ISNULL(CASE WHEN   ssd.SalaryType = 'Medical' THEN ssd.Amount  else 0 END, 0) AS Medical
,ISNULL(CASE WHEN   ssd.SalaryType = 'Conveyance' THEN ssd.Amount  else 0 END, 0) AS Conveyance 
,ISNULL(CASE WHEN   ssd.SalaryType = 'Gross' THEN ssd.Amount  else 0 END, 0) AS Gross 
FROM SalaryEarningDetail ssd
 WHERE 1=1
and ssd.FiscalYearDetailId=@FiscalYearDetailId  
and ssd.IsActive=1
) as Salary
GROUP BY Salary.EmployeeId
";
                }
                else
                {
                    sqlText += @"
SELECT Salary.EmployeeId
,SUM(Salary.Basic)Basic
,SUM(Salary.HouseRent)HouseRent
,SUM(Salary.Medical)Medical
,SUM(Salary.Conveyance)Conveyance
,SUM(Salary.Gross)Gross
FROM(
 SELECT 
ssd.EmployeeId
,ISNULL(CASE WHEN  ssd.SalaryType = 'Basic' THEN ssd.Amount  else 0 END, 0) AS Basic
,ISNULL(CASE WHEN   ssd.SalaryType = 'HouseRent' THEN ssd.Amount else 0 END, 0) AS HouseRent
,ISNULL(CASE WHEN   ssd.SalaryType = 'Medical' THEN ssd.Amount  else 0 END, 0) AS Medical
,ISNULL(CASE WHEN   ssd.SalaryType = 'Conveyance' THEN ssd.Amount  else 0 END, 0) AS Conveyance 
,ISNULL(CASE WHEN   ssd.SalaryType = 'Gross' THEN ssd.Amount  else 0 END, 0) AS Gross 
FROM EmployeeSalaryStructureDetail ssd
 WHERE 1=1
and ssd.IncrementDate<=@PeriodEnd  
and ssd.IsActive=1
) AS Salary
GROUP BY Salary.EmployeeId
";
                }
                sqlText += @"
) AS fs ON ve.EmployeeId = fs.EmployeeId
WHERE 1=1

 ";
                if (paramVM.ProjectId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.ProjectId))
                {
                    sqlText += " and  ve.ProjectId=@ProjectId";
                }
                if (paramVM.DepartmentId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.DepartmentId))
                {
                    sqlText += " and  ve.DepartmentId=@DepartmentId";
                }
                if (paramVM.SectionId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.SectionId))
                {
                    sqlText += " and  ve.SectionId=@SectionId";
                }
                if (paramVM.DesignationId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.DesignationId))
                {
                    sqlText += " and  ve.DesignationId=@DesignationId";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.CodeF))
                {
                    sqlText += " and  ve.Code>=@EmployeeCodeFrom";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.CodeT))
                {
                    sqlText += " and  ve.Code<=@EmployeeCodeTo";
                }


                if (paramVM.EmploymentType.ToLower() == "new")
                {
                    sqlText += " and  ve.IsActive=1";
                    sqlText += " and  ve.JoinDate>=@PeriodStart";
                    sqlText += " and  ve.JoinDate<=@PeriodEnd";
                }
                else if (paramVM.EmploymentType.ToLower() == "regular")
                {
                    sqlText += " and  ve.IsActive=1";
                    sqlText += " and  ve.JoinDate<@PeriodStart";
                }
                else if (paramVM.EmploymentType.ToLower() == "left")
                {
                    if (paramVM.CompanyName.ToLower() == "tib")
                    {
                        sqlText += " and  ve.IsActive=0";
                        sqlText += " and  ve.LeftDate>=@PeriodStart";
                        sqlText += " and  ve.LeftDate<=@PeriodEnd";
                        sqlText += " and  ve.IsSalalryProcess=1";
                    }
                    else
                    {
                        sqlText += " and  ve.IsActive=0";
                        sqlText += " and  ve.LeftDate>=@PeriodStart";
                        sqlText += " and  ve.LeftDate<=@PeriodEnd";
                    }

                }
                else if (paramVM.EmploymentType.ToLower() == "archive")
                {
                    sqlText += " and  ve.LeftDate>=@PeriodEnd";
                    sqlText += " and  ve.JoinDate<@PeriodStart";
                }



                #endregion

                #region SqlExecution


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodStart", paramVM.PeriodStart);
                cmd.Parameters.AddWithValue("@PeriodEnd", paramVM.PeriodEnd);


                if (paramVM.ProjectId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.ProjectId))
                {
                    cmd.Parameters.AddWithValue("@ProjectId", paramVM.ProjectId);
                }
                if (paramVM.DepartmentId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.DepartmentId))
                {
                    cmd.Parameters.AddWithValue("@DepartmentId", paramVM.DepartmentId);
                }
                if (paramVM.SectionId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.SectionId))
                {
                    cmd.Parameters.AddWithValue("@SectionId", paramVM.SectionId);
                }
                if (paramVM.DesignationId != "0_0" && !string.IsNullOrWhiteSpace(paramVM.DesignationId))
                {
                    cmd.Parameters.AddWithValue("@DesignationId", paramVM.DesignationId);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.CompanyName) && paramVM.CompanyName.ToLower() == "tib")
                {
                    cmd.Parameters.AddWithValue("@FiscalYearDetailId", paramVM.FiscalYearDetailId);

                }

                cmd.Parameters.AddWithValue("@EmployeeCodeFrom", paramVM.CodeF);
                cmd.Parameters.AddWithValue("@EmployeeCodeTo", paramVM.CodeT);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        vm = new EmployeeInfoVM();
                        vm.Id = dr["Id"].ToString();
                        vm.ProjectId = dr["ProjectId"].ToString();
                        vm.DepartmentId = dr["DepartmentId"].ToString();
                        vm.SectionId = dr["SectionId"].ToString();
                        vm.DesignationId = dr["DesignationId"].ToString();
                        vm.GradeId = dr["GradeId"].ToString();
                        vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                        vm.IsPFApplicable = Convert.ToBoolean(dr["IsPFApplicable"]);
                        vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                        VMs.Add(vm);
                    }
                    dr.Close();
                }
                #endregion


                #region Archive

                if (paramVM.EmploymentType.ToLower() == "archive")
                {
                    VMs.Select(c => { c.IsActive = true; return c; }).ToList();
                }


                #endregion
                #endregion EmployeeList

                #endregion
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
            #region Catch and Finall
            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
            }
            #endregion
            #region Finally
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

        public List<ViewEmployeeInfoVM> ViewSelectAllEmployee(string code, string id, string branchId, string projectId, string departmentId, string sectionId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ViewEmployeeInfoVM> employeeInfoVMs = new List<ViewEmployeeInfoVM>();
            ViewEmployeeInfoVM empViewVM;
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
                sqlText = @"SELECT *
From ViewEmployeeInformation
Where 1=1
--Where IsArchive=0 and IsActive=1
";
                if (code != "")
                    sqlText += " and code=@code";
                if (id != "")
                    sqlText += " and  id=@id";
                if (branchId != "")
                    sqlText += " and  branchId=@branchId";
                if (projectId != "")
                    sqlText += " and  projectId=@projectId";
                if (departmentId != "")
                    sqlText += " and  departmentId=@departmentId";
                if (sectionId != "")
                    sqlText += " and  sectionId=@sectionId";
                sqlText += "     ORDER By EmpName desc";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (code != "")
                    objComm.Parameters.AddWithValue("@code", code);
                if (id != "")
                    objComm.Parameters.AddWithValue("@id", id);
                if (branchId != "")
                    objComm.Parameters.AddWithValue("@branchId", branchId);
                if (projectId != "")
                    objComm.Parameters.AddWithValue("@projectId", projectId);
                if (departmentId != "")
                    objComm.Parameters.AddWithValue("@departmentId", departmentId);
                if (sectionId != "")
                    objComm.Parameters.AddWithValue("@sectionId", sectionId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    empViewVM = new ViewEmployeeInfoVM();
                    empViewVM.Id = dr["Id"].ToString();
                    empViewVM.Code = dr["Code"].ToString();
                    empViewVM.Salutation_E = dr["Salutation_E"].ToString();
                    empViewVM.MiddleName = dr["MiddleName"].ToString();
                    empViewVM.LastName = dr["LastName"].ToString();
                    empViewVM.EmpName = dr["EmpName"].ToString();
                    empViewVM.JoinDate = dr["JoinDate"].ToString();
                    empViewVM.ProbationEnd = dr["ProbationEnd"].ToString();
                    empViewVM.DateOfPermanent = dr["DateOfPermanent"].ToString();
                    empViewVM.EmploymentStatus = dr["EmploymentStatus"].ToString();
                    empViewVM.EmploymentType = dr["EmploymentType"].ToString();
                    empViewVM.Project = dr["Project"].ToString();
                    empViewVM.Branch = dr["Branch"].ToString();
                    empViewVM.Department = dr["Department"].ToString();
                    empViewVM.Section = dr["Section"].ToString();
                    empViewVM.TransferDate = dr["TransferDate"].ToString();
                    empViewVM.Designation = dr["Designation"].ToString();
                    empViewVM.Grade = dr["Grade"].ToString();
                    empViewVM.IsPromotion = Convert.ToBoolean(dr["IsPromotion"]);
                    empViewVM.PromotionDate = dr["PromotionDate"].ToString();
                    empViewVM.ProjectId = dr["ProjectId"].ToString();
                    empViewVM.SectionId = dr["SectionId"].ToString();
                    empViewVM.DepartmentId = dr["DepartmentId"].ToString();
                    empViewVM.DesignationId = dr["DesignationId"].ToString();
                    empViewVM.GradeId = dr["GradeId"].ToString();
                    empViewVM.BranchId = dr["BranchId"].ToString();
                    empViewVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    empViewVM.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    empViewVM.AttnUserId = dr["AttnUserId"].ToString();
                    empViewVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    empViewVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    empViewVM.EmpEmail = dr["Email"].ToString();
                    employeeInfoVMs.Add(empViewVM);
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
            return employeeInfoVMs;
        }

        public List<ViewEmployeeInfoVM> ViewSelectAllEmployee(string code = null, string id = null, string branchId = null, string projectId = null
           , string departmentId = null, string sectionId = null, string designationId = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string EmployeeId = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ViewEmployeeInfoVM> employeeInfoVMs = new List<ViewEmployeeInfoVM>();
            ViewEmployeeInfoVM empViewVM;
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
                sqlText = @"SELECT *
From ViewEmployeeInformation
Where 1=1 
-- IsArchive=0 and IsActive=1
";
                if (!string.IsNullOrWhiteSpace(code))
                    sqlText += " and code=@code";
                if (!string.IsNullOrWhiteSpace(id))
                    //sqlText += " and  id=@id";
                    sqlText += " and  EmployeeId=@id";
                if (!string.IsNullOrWhiteSpace(branchId))
                    sqlText += " and  branchId=@branchId";
                if (!string.IsNullOrWhiteSpace(projectId))
                    sqlText += " and  projectId=@projectId";
                if (!string.IsNullOrWhiteSpace(departmentId))
                    sqlText += " and  departmentId=@departmentId";
                if (!string.IsNullOrWhiteSpace(sectionId))
                    sqlText += " and  sectionId=@sectionId";
                if (!string.IsNullOrWhiteSpace(designationId))
                    sqlText += " and  designationId=@designationId";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                    sqlText += " and  EmployeeId=@EmployeeId";
                sqlText += "     ORDER By Code, Department, Section, EmpName desc";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                ////SqlCommand objComm = new SqlCommand();
                //objComm.Connection = currConn;
                //objComm.CommandText = sqlText;
                //objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(code))
                    objComm.Parameters.AddWithValue("@code", code);
                if (!string.IsNullOrWhiteSpace(id))
                    objComm.Parameters.AddWithValue("@id", id);
                if (!string.IsNullOrWhiteSpace(branchId))
                    objComm.Parameters.AddWithValue("@branchId", branchId);
                if (!string.IsNullOrWhiteSpace(projectId))
                    objComm.Parameters.AddWithValue("@projectId", projectId);
                if (!string.IsNullOrWhiteSpace(departmentId))
                    objComm.Parameters.AddWithValue("@departmentId", departmentId);
                if (!string.IsNullOrWhiteSpace(sectionId))
                    objComm.Parameters.AddWithValue("@sectionId", sectionId);
                if (!string.IsNullOrWhiteSpace(designationId))
                    objComm.Parameters.AddWithValue("@designationId", designationId);
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();
                using (SqlDataReader dr = objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        empViewVM = new ViewEmployeeInfoVM();
                        empViewVM.EmployeeId = dr["EmployeeId"].ToString();
                        empViewVM.Code = dr["Code"].ToString();
                        empViewVM.EmpName = dr["EmpName"].ToString();
                        empViewVM.JoinDate = dr["JoinDate"].ToString();
                        empViewVM.Project = dr["Project"].ToString();
                        empViewVM.Department = dr["Department"].ToString();
                        empViewVM.Section = dr["Section"].ToString();
                        empViewVM.Designation = dr["Designation"].ToString();
                        empViewVM.Grade = dr["Grade"].ToString();
                        empViewVM.ProjectId = dr["ProjectId"].ToString();
                        empViewVM.SectionId = dr["SectionId"].ToString();
                        empViewVM.DepartmentId = dr["DepartmentId"].ToString();
                        empViewVM.DesignationId = dr["DesignationId"].ToString();
                        empViewVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        empViewVM.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                        empViewVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                        empViewVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                        employeeInfoVMs.Add(empViewVM);
                    }
                    dr.Close();
                }
                #endregion
                //#region Commit
                //if (Vtransaction == null)
                //{
                //    if (transaction != null)
                //    {
                //        transaction.Commit();
                //    }
                //}
                //#endregion Commit
            }
            #region Catch and Finall
            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
            }
            #endregion
            #region Finally
            //finally
            //{
            //    if (VcurrConn == null)
            //    {
            //        if (currConn != null)
            //        {
            //            if (currConn.State == ConnectionState.Open)
            //            {
            //                currConn.Close();
            //            }
            //        }
            //    }
            //}
            #endregion
            return employeeInfoVMs;
        }

        public decimal SelectEmployeeBasicSalary(string EmployeeId, string FiscalYearDetailId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            ////List<ViewEmployeeInfoVM> employeeInfoVMs = new List<ViewEmployeeInfoVM>();
            ////ViewEmployeeInfoVM empViewVM;

            decimal BasicSalary = 0;

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

                #region Salary Earning Detail

                sqlText = @"
SELECT Amount BasicSalary From SalaryEarningDetail
where 1=1 and SalaryType='Basic' 
--and EmployeeStatus='regular'
";

                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += " and  employeeid=@employeeid";
                }

                if (!string.IsNullOrWhiteSpace(FiscalYearDetailId))
                {
                    sqlText += " and  FiscalYearDetailId=@FiscalYearDetailId";
                }

                sqlText += "     ORDER By employeeid ";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@employeeid", EmployeeId);
                }

                if (!string.IsNullOrWhiteSpace(FiscalYearDetailId))
                {
                    objComm.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                }

                BasicSalary = Convert.ToDecimal(objComm.ExecuteScalar());

                #endregion

                #region EmployeeJob

                if (BasicSalary <= 0)
                {

                    sqlText = @"
SELECT BasicSalary
From EmployeeJob
where 1=1 

";

                    if (!string.IsNullOrWhiteSpace(EmployeeId))
                    {
                        sqlText += " and  employeeid=@employeeid";
                    }

                    sqlText += "     ORDER By employeeid ";

                    objComm = new SqlCommand(sqlText, currConn, transaction);

                    if (!string.IsNullOrWhiteSpace(EmployeeId))
                    {
                        objComm.Parameters.AddWithValue("@employeeid", EmployeeId);
                    }

                    BasicSalary = Convert.ToDecimal(objComm.ExecuteScalar());


                }

                #endregion

                #endregion

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

            #region Catch and Finall

            catch (Exception ex)
            {

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                }
            }
            #endregion

            #region Finally

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

            return BasicSalary;
        }


        //==================SelectAll=================
        public List<EmployeeInfoVM> SelectCommonFields(string[] conFields = null, string[] conValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                sqlText = @"
SELECT
 Id
,Code
,AttnUserId
From EmployeeInfo 
Where 1=1 AND IsArchive=0
";
                int i = 0;
                if (conFields != null && conValues != null)
                {
                    foreach (string item in conFields)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                        {
                            continue;
                        }
                        sqlText += " AND " + conFields[i] + "=@" + conFields[i];
                        i++;
                    }
                }
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                i = 0;
                if (conFields != null && conValues != null)
                {
                    foreach (string item in conFields)
                    {
                        if (string.IsNullOrWhiteSpace(conFields[i]) || string.IsNullOrWhiteSpace(conValues[i]))
                        {
                            continue;
                        }
                        objComm.Parameters.AddWithValue("@" + conFields[i], conValues[i]);
                        i++;
                    }
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.AttnUserId = dr["AttnUserId"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
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
        public List<EmployeeInfoVM> SelectAll(string BranchId, string DOJFrom = "", string DOJTo = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm = new EmployeeInfoVM();
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
 EmployeeId
,EmpName ,Code, Designation, Department, Section, Project 
,JoinDate
,ISNULL(IsActive, 0) IsActive
From ViewEmployeeInformation 
Where 1=1 AND IsArchive=0 and BranchId=@BranchId
";
                if (DOJFrom != "")
                {
                    sqlText += "  AND JoinDate>=@DOJFrom  ";
                }
                if (DOJTo != "")
                {
                    sqlText += "  AND JoinDate<=@DOJTo  ";
                }
                if (BranchId != "")
                {
                    sqlText += "  AND BranchId=@BranchId  ";
                }
                sqlText += "     ORDER BY Department, EmpName desc";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (DOJFrom != "")
                {
                    objComm.Parameters.AddWithValue("@DOJFrom", DOJFrom);
                }
                if (DOJTo != "")
                {
                    objComm.Parameters.AddWithValue("@DOJTo", DOJTo);
                }
                if (BranchId != "")
                {
                    objComm.Parameters.AddWithValue("@BranchId", BranchId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["EmployeeId"].ToString();
                    vm.Code = dr["Code"].ToString();

                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
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
        public List<EmployeeInfoVM> SelectAllActiveEmp(string branchId, string DOJFrom = "", string DOJTo = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm = new EmployeeInfoVM();
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
SELECT DISTINCT 
 Id
,BranchId
,EmpName ,Code, Designation, Branch, Department, Section, Project 
,JoinDate
From ViewEmployeeInformation 
Where IsArchive=0 AND IsActive=1 and BranchId=@BranchId
";
                if (DOJFrom != "")
                {
                    sqlText += "  AND ve.JoinDate>=@DOJFrom  ";
                }
                if (DOJTo != "")
                {
                    sqlText += "  AND ve.JoinDate<=@DOJTo  ";
                }
                sqlText += "     ORDER BY Department, EmpName desc";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (DOJFrom != "")
                {
                    objComm.Parameters.AddWithValue("@DOJFrom", DOJFrom);
                }
                if (DOJTo != "")
                {
                    objComm.Parameters.AddWithValue("@DOJTo", DOJTo);
                }
                objComm.Parameters.AddWithValue("@BranchId", branchId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.BranchId = Convert.ToInt32(branchId);
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




        //==================SelectByID=================
        public EmployeeInfoVM SelectById(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeInfoVM vm = new EmployeeInfoVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                sqlText = @"
     SELECT
     e.Id
    ,e.Code
    ,ve.EmpName, ve.Code, ve.Designation, ve.Department,ve.Project,ve.Section
    ,e.EmpName
    ,ve.JoinDate
    ,e.Remarks
    ,e.IsActive
    ,e.IsArchive
    ,e.CreatedBy
    ,e.CreatedAt
    ,e.CreatedFrom
    ,e.LastUpdateBy
    ,e.LastUpdateAt
    ,e.LastUpdateFrom
    ,e.PhotoName
	,ve.Email
    From EmployeeInfo e  
    LEFT OUTER JOIN ViewEmployeeInformation ve on e.Id=ve.EmployeeId
   
    Where e.id=@Id and e.IsArchive=0 And e.IsActive=1
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();

                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.Email = dr["Email"].ToString();
                }
                dr.Close();
                #endregion
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
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

        public EmployeeInfoVM SelectLeaveScheduleById(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeInfoVM vm = new EmployeeInfoVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                sqlText = @"
SELECT
     e.Id
    ,e.BranchId
    ,e.Code
    ,ve.EmpName, ve.Code, ve.Designation, ve.Department,ve.Project,ve.Section
    ,e.Salutation_E
    ,e.AttnUserId
    ,e.MiddleName
    ,e.LastName
    ,ve.JoinDate
    ,e.Remarks
    ,e.IsActive
    ,e.IsArchive
    ,e.CreatedBy
    ,e.CreatedAt
    ,e.CreatedFrom
    ,e.LastUpdateBy
    ,e.LastUpdateAt
    ,e.LastUpdateFrom


    ,e.PhotoName
    ,ep.GradeId
    ,ep.stepid
    ,isnull(pd.Gender_E,'NA')Gender_E
    ,IsNull(ej.IsPermanent, 0) IsPermanent
	,ve.Email
    From EmployeeInfo e
    LEFT OUTER JOIN EmployeeJob ej on ej.EmployeeId=e.id
    LEFT OUTER JOIN EmployeePromotion ep on ej.EmployeeId=ep.EmployeeId and ep.IsCurrent=1
    LEFT OUTER JOIN ViewEmployeeInformation ve on e.Id=ve.id
    LEFT OUTER JOIN EmployeePersonalDetail pd on e.Id=pd.EmployeeId
    Where e.id=@Id and e.IsArchive=0 And e.IsActive=1
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.AttnUserId = dr["AttnUserId"].ToString();
                    vm.Salutation_E = dr["Salutation_E"].ToString();
                    vm.MiddleName = dr["MiddleName"].ToString();
                    vm.LastName = dr["LastName"].ToString();
                    //gmployeeInfoVM.AttnUserId = dr["AttnUserId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());

                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    //vm.GradeId = dr["GradeId"].ToString();
                    //vm.StepId = dr["StepId"].ToString();
                }
                dr.Close();
                #endregion
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
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

        public EmployeeInfoVM AllSelectById(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeInfoVM vm = new EmployeeInfoVM();
            #endregion
            try
            {
                #region open connection and transaction
                if (VcurrConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
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
                sqlText = @"
SELECT
     e.Id
    ,e.BranchId
    ,e.Code
    ,ve.EmpName, ve.Code, ve.Designation, ve.Department,ve.Project,ve.Section

    ,ve.JoinDate
    ,e.Remarks
    ,e.IsActive
    ,e.IsArchive
    ,e.CreatedBy
    ,e.CreatedAt
    ,e.CreatedFrom
    ,e.LastUpdateBy
    ,e.LastUpdateAt
    ,e.LastUpdateFrom
    ,e.PhotoName



	,ve.Email
    From EmployeeInfo e


    LEFT OUTER JOIN ViewEmployeeInformation ve on e.Id=ve.id

    Where e.id=@Id 
";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    //vm.AttnUserId = dr["AttnUserId"].ToString();
                    //vm.Salutation_E = dr["Salutation_E"].ToString();
                    //vm.MiddleName = dr["MiddleName"].ToString();
                    //vm.LastName = dr["LastName"].ToString();
                    //gmployeeInfoVM.AttnUserId = dr["AttnUserId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    //vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.Email = dr["Email"].ToString();
                    //vm.Gender_E = dr["Gender_E"].ToString();
                    //vm.GradeId = dr["GradeId"].ToString();
                    //vm.StepId = dr["StepId"].ToString();
                }
                dr.Close();
                #endregion
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
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


        //==================SelectByID=================
        public EmployeeInfoVM SelectByIdAll(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeInfoVM vm = new EmployeeInfoVM();
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
                sqlText = @"select e.id, e.Code
,e.Department ,e.DepartmentId
,e.Designation,e.DesignationId
,e.JoinDate
,e.EmpName
,e.BranchId
,e.Project ,e.Id ProjectId
,e.Section ,e.Id SectionId
,ty.Name EmploymentType_E
,ty.id EmploymentType_EId
,a.Mobile
,a.Address ,a.District,a.Division,a.Country
,ed.Gender_E
,ed.Religion
,e.Grade

,ed.BloodGroup_E
,ed.Email
,e.BasicSalary
,e.GrossSalary

from ViewEmployeeInformation e
left outer join employeeJob j on j.EmployeeId=e.Id
left outer join EnumEmploymentType ty on ty.id=j.EmploymentType_E
left outer join employeePresentAddress a on e.Id=a.EmployeeId
left outer join EmployeePersonalDetail ed on ed.EmployeeId=e.Id
Where e.id=@Id and e.IsArchive=0
ORDER BY e.Department, e.EmpName desc
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    //gmployeeInfoVM.AttnUserId = dr["AttnUserId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    //vm.GradeId = dr["GradeId"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    //vm.AttnUserId = dr["AttnUserId"].ToString();
                    //vm.StepName = dr["StepName"].ToString();
                    //vm.StepSL = dr["StepSL"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
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
            return vm;
        }

        public EmployeeInfoVM SelectEmpForSearch(string empcode, string btn, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeInfoVM gmployeeInfoVM = new EmployeeInfoVM();
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
                sqlText = @"
SELECT top 1 *
From  ViewEmployeeInformation 
Where 1=1 and IsArchive=0 And IsActive=1
";
                SqlCommand objComm = new SqlCommand();
                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND Code=@Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "next")
                {
                    sqlText += @" AND Code > @Code ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND Code < @Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.Id = dr["EmployeeId"].ToString();
                    gmployeeInfoVM.EmployeeId = dr["EmployeeId"].ToString();
                    gmployeeInfoVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    //gmployeeInfoVM.PhotoName = dr["PhotoName"].ToString();
                    gmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    gmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());

                    if (gmployeeInfoVM.empPFForTransferVM == null)
                    {
                        gmployeeInfoVM.empPFForTransferVM = new EmployeeTransferVM();
                    }
                    gmployeeInfoVM.empPFForTransferVM.FromBranch = gmployeeInfoVM.BranchId.ToString();

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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            return gmployeeInfoVM;
        }
        public EmployeeInfoVM SelectEmpForSearchAll(string empcode, string btn, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeInfoVM gmployeeInfoVM = new EmployeeInfoVM();
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
                sqlText = @"
SELECT top 1 *
From  ViewEmployeeInformation 
Where 1=1 
";
                SqlCommand objComm = new SqlCommand();
                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND Code=@Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "next")
                {
                    sqlText += @" AND Code > @Code ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND Code < @Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Transaction = transaction;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.EmployeeId = dr["EmployeeId"].ToString();
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    //gmployeeInfoVM.PhotoName = dr["PhotoName"].ToString();
                    gmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    gmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());

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
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
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
            return gmployeeInfoVM;
        }
        public EmployeeInfoVM SelectExEmpStructure(string empcode, string btn)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeInfoVM gmployeeInfoVM = new EmployeeInfoVM();
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
                sqlText = @"SELECT top 1 *
From  ViewEmployeeInformation 
Where 1=1 
";
                SqlCommand objComm = new SqlCommand();
                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND Code=@Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "next")
                {
                    sqlText += @" AND Code > @Code ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND Code < @Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.Id = dr["Id"].ToString();
                    gmployeeInfoVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    gmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    gmployeeInfoVM.Project = dr["Project"].ToString();

                    gmployeeInfoVM.GradeId = dr["GradeId"].ToString();
                    gmployeeInfoVM.StepSL = dr["StepSL"].ToString();

                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    gmployeeInfoVM.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
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
            return gmployeeInfoVM;
        }

        public EmployeeInfoVM SelectEmpStructure(string empcode, string btn)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeInfoVM gmployeeInfoVM = new EmployeeInfoVM();
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
                sqlText = @"SELECT top 1 *
From  ViewEmployeeInformation 
Where 1=1 and IsArchive=0 And IsActive=1
";
                SqlCommand objComm = new SqlCommand();
                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND Code=@Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "next")
                {
                    sqlText += @" AND Code > @Code ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND Code < @Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.Id = dr["Id"].ToString();
                    gmployeeInfoVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    gmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    gmployeeInfoVM.Project = dr["Project"].ToString();

                    gmployeeInfoVM.GradeId = dr["GradeId"].ToString();
                    gmployeeInfoVM.StepSL = dr["StepSL"].ToString();

                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    gmployeeInfoVM.IsPermanent = Convert.ToBoolean(dr["IsPermanent"]);
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
            return gmployeeInfoVM;
        }

        public EmployeeInfoVM SelectEmpStructureAll(string empcode, string btn)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeInfoVM gmployeeInfoVM = new EmployeeInfoVM();
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
                sqlText = @"SELECT top 1 *
From  ViewEmployeeInformation 
Where 1=1 and IsArchive=0
";
                SqlCommand objComm = new SqlCommand();
                if (btn.ToLower() == "current")
                {
                    sqlText += @" AND Code=@Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "next")
                {
                    sqlText += @" AND Code > @Code ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "previous")
                {
                    sqlText += @" AND Code < @Code ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "first")
                {
                    sqlText += @" ORDER BY Code  ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                else if (btn.ToLower() == "last")
                {
                    sqlText += @" ORDER BY Code desc ";
                    objComm.Parameters.AddWithValue("@Code", empcode);
                }
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.Id = dr["Id"].ToString();
                    gmployeeInfoVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    gmployeeInfoVM.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
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
            return gmployeeInfoVM;
        }

        //==================SelectByID=================
        public EmployeeVM EmployeeInfo(string Id)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeVM employeeVM = new EmployeeVM();
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
                 SELECT E.EMPLOYEEID,E.CODE,E.REMARKS,E.Name EmpName               
                ,E.JOINDATE   
				,ve.Section
				,ve.Designation
				,ve.Department
				,ve.Project
                FROM EMPLOYEEINFO E
				Left Outer Join ViewEmployeeInformation ve on ve.EmployeeId=E.Id
                WHERE E.ID=@Id
                ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                objComm.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    employeeVM.Id = dr["EMPLOYEEID"].ToString();
                    employeeVM.Code = dr["Code"].ToString();
                    employeeVM.FullName = dr["EmpName"].ToString();
                    employeeVM.Remarks = dr["Remarks"].ToString();
                    employeeVM.Designation = dr["DESIGNATION"].ToString();
                    employeeVM.jobjoinDate = Ordinary.StringToDate(dr["JOINDATE"].ToString());
                    employeeVM.Department = dr["DEPARTMENT"].ToString();
                    employeeVM.Project = dr["PROJECT"].ToString();
                    employeeVM.Section = dr["SECTION"].ToString();
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
            return employeeVM;
        }

        //==================Insert =================
        public string[] Insert(EmployeeInfoVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertEmployeeInfo"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            #region Try
            try
            {
                #region Validation
                //if (string.IsNullOrEmpty(EmployeeInfoVM.DepartmentId))
                //{
                //    retResults[1] = "Please Input Employee Travel Course";
                //    return retResults;
                //}
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
                SettingDAL _settingDal = new SettingDAL();
                CommonDAL _commonDal = new CommonDAL();
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var AutoCode = _settingDal.settingValue("AutoCode", "Employee", currConn, transaction);
                if (AutoCode != "Y")
                {
                    if (string.IsNullOrWhiteSpace(vm.Code))
                    {
                        retResults[1] = "Please Enter the Employee Code!";
                        throw new ArgumentNullException(retResults[1], "");
                    }

                    #region Code Exists
                    sqlText = @"Select isnull( isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) ,0)
                     from EmployeeInfo where BranchId=@BranchId and Code=@Code";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn, transaction);
                    cmdExist.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdExist.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    var exeRes = cmdExist.ExecuteScalar();
                    int count2 = Convert.ToInt32(exeRes);
                    if (count2 > 0)
                    {
                        retResults[1] = "Already this Employee Code is exist!";
                        throw new ArgumentNullException("Already this Employee Code is exist!", "");
                    }
                    #endregion Code Exists
                }

                #region Generate Id
                sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from EmployeeInfo where BranchId=@BranchId";
                SqlCommand cmdIdGen = new SqlCommand(sqlText, currConn, transaction);
                cmdIdGen.Parameters.AddWithValue("@BranchId", vm.BranchId);
                var count = cmdIdGen.ExecuteScalar();
                vm.Id = vm.BranchId.ToString() + "_" + (Convert.ToInt32(count) + 1);
                //int foundId = (int)objfoundId;
                #endregion Generate Id
                #region No. of Employee Permission Check
                int NumberOfEmployees = 0, permitedEmployee = 0;
                sqlText = "select count(Id) NumberOfEmployees from EmployeeInfo";
                SqlCommand cmdExistingEmployees = new SqlCommand(sqlText, currConn, transaction);
                SqlDataReader dr;
                dr = cmdExistingEmployees.ExecuteReader();
                while (dr.Read())
                {
                    NumberOfEmployees = Convert.ToInt32(dr["NumberOfEmployees"]) + 1;
                }
                dr.Close();
                sqlText = "select NumberOfEmployees From Company";
                SqlCommand cmdPermitedEmployees = new SqlCommand(sqlText, currConn, transaction);
                dr = cmdPermitedEmployees.ExecuteReader();
                while (dr.Read())
                {
                    permitedEmployee = Convert.ToInt32(dr["NumberOfEmployees"]);
                }
                dr.Close();
                if (NumberOfEmployees > permitedEmployee)
                {
                    retResults[1] = "You have only " + permitedEmployee + " Employee Licence";
                    throw new ArgumentNullException("You have only " + permitedEmployee + " Employee Licence", "");
                }
                #endregion No. of Employee Permission Check


                if (AutoCode == "Y")
                {
                    #region Generate EmployeeCode
                    var databaseName = License.DataBaseName(currConn.Database);
                    var databaseNameSecond = databaseName.Split('~')[1].ToString();
                    var employeecode = _cDal.NextCode("EmployeeInfo", currConn, transaction);
                    currConn.ChangeDatabase(databaseNameSecond);
                    var employeecodeSecond = _cDal.NextCode("EmployeeInfo", currConn, transaction);
                    databaseName = databaseName.Split('~')[0].ToString();
                    currConn.ChangeDatabase(databaseName);
                    if (employeecode < employeecodeSecond)
                    {
                        vm.Code = employeecodeSecond.ToString();
                    }
                    else
                    {
                        vm.Code = "EMP-" + employeecode.ToString();
                    }
                }
                    #endregion Generate EmployeeCode
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeInfo(
 Id
,BranchId
,Code
,Salutation_E
,MiddleName
,LastName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,PhotoName
) 
   VALUES (
@Id
,@BranchId
,@Code
,@Salutation_E
,@MiddleName
,@LastName
,@Remarks
,@IsActive
,@IsArchive
,@CreatedBy
,@CreatedAt
,@CreatedFrom
,@PhotoName
) 
                                        ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdInsert.Parameters.AddWithValue("@Salutation_E", vm.Salutation_E ?? "");
                    cmdInsert.Parameters.AddWithValue("@MiddleName", vm.MiddleName ?? "");
                    cmdInsert.Parameters.AddWithValue("@LastName", vm.LastName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@PhotoName", "0.jpg");
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "This EmployeeInfo already used";
                    throw new ArgumentNullException("Please Input EmployeeInfo Value", "");
                }

                #region User Create
                string AutoUser = _settingDal.settingValue("AutoUser", "Employee", currConn, transaction);
                string AutoPassword = _settingDal.settingValue("AutoPassword", "Employee", currConn, transaction);

                UserInformationDAL _userInfoDal = new UserInformationDAL();
                UserLogsVM userLogsVM = new UserLogsVM();
                if (AutoUser == "Y")
                {
                    EmployeeInfoDAL _empInfoDal = new EmployeeInfoDAL();
                    EmployeeInfoVM empInfoVM = new EmployeeInfoVM();
                    empInfoVM = _empInfoDal.SelectById(vm.Id, currConn, transaction);

                    userLogsVM.LogID = empInfoVM.Code;
                    userLogsVM.EmployeeCode = empInfoVM.Code;
                    userLogsVM.FullName = empInfoVM.EmpName;
                    userLogsVM.CreatedAt = empInfoVM.CreatedAt;
                    userLogsVM.CreatedBy = empInfoVM.CreatedBy;
                    userLogsVM.CreatedFrom = empInfoVM.CreatedFrom;
                    userLogsVM.BranchId = empInfoVM.BranchId;
                    userLogsVM.EmployeeId = empInfoVM.Id;
                    userLogsVM.GroupId = 6; //ESS

                    userLogsVM.Password = Ordinary.Encrypt(AutoPassword, true);
                    retResults = _userInfoDal.Insert(userLogsVM, currConn, transaction);
                    if (retResults[0] == "Fail")
                    {
                        retResults[1] = "User Not Created";
                        throw new ArgumentNullException(retResults[1], "");
                    }

                }


                #endregion User Create

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
                retResults[1] = "Data Save Successfully";
                retResults[2] = vm.Id;
                #endregion SuccessResult
            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update EmployeeInfo.";
                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                }
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
        public string[] Update(EmployeeInfoVM EmployeeInfoVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeInfo Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeInfo"); }
                #endregion open connection and transaction
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeInfo ";
                sqlText += " WHERE Code=@Code AND Id<>@Id";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@Id", EmployeeInfoVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", EmployeeInfoVM.EmployeeId);
                cmdExist.Parameters.AddWithValue("@Code", EmployeeInfoVM.Code.Trim());
                if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                {
                    retResults[1] = "This Employee Code already used";
                    throw new ArgumentNullException("This Employee Code already used", "");
                }
                #endregion Exist
                if (EmployeeInfoVM != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeInfo set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Salutation_E =@Salutation_E   ,";
                    sqlText += " MiddleName=@MiddleName      ,";
                    sqlText += " LastName=@LastName       ,";
                    sqlText += " Remarks=@Remarks        ,";
                    sqlText += " AttnUserId=@AttnUserId        ,";
                    //   sqlText += " IsActive=@IsActive       ,";
                    sqlText += " LastUpdateBy=@LastUpdateBy   ,";
                    sqlText += " LastUpdateAt=@LastUpdateAt   ,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom ";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", EmployeeInfoVM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", EmployeeInfoVM.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Salutation_E", EmployeeInfoVM.Salutation_E ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@MiddleName", EmployeeInfoVM.MiddleName);
                    cmdUpdate.Parameters.AddWithValue("@LastName", EmployeeInfoVM.LastName ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", EmployeeInfoVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@AttnUserId", EmployeeInfoVM.AttnUserId ?? Convert.DBNull);
                    // cmdUpdate.Parameters.AddWithValue("@IsActive", EmployeeInfoVM.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeInfoVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeInfoVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeInfoVM.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeeInfoVM.Id);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = EmployeeInfoVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeInfoVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeInfo Update", "Could not found any item.");
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
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update EmployeeInfo.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update EmployeeInfo.";
                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                }
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

        public string[] UpdatePhoto(string EmployeeId, string PhotoName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee EmployeeInfo Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeInfo"); }
                #endregion open connection and transaction
                if (!string.IsNullOrWhiteSpace(PhotoName))
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update EmployeeInfo set";
                    sqlText += " PhotoName=@PhotoName";
                    sqlText += " where Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@PhotoName", PhotoName);
                    cmdUpdate.Parameters.AddWithValue("@Id", EmployeeId);
                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = EmployeeId.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeInfoVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeInfo Update", "Could not found any item.");
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
                    retResults[1] = "Data Update Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to update EmployeeInfo.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update EmployeeInfo.";
                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                }
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

        //==================Select =================
        public EmployeeInfoVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeInfoVM gmployeeInfoVM = new EmployeeInfoVM();
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
                sqlText = @"SELECT Top 1 
Id
,BranchId
,Code
,Salutation_E
,MiddleName
,LastName
,Remarks
,IsActive
,IsArchive
,CreatedBy
,AttnUserId
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From EmployeeInfo 
";
                if (query == null)
                {
                    if (Id != 0)
                    {
                        sqlText += " AND Id=@Id";
                    }
                    else
                    {
                        sqlText += " ORDER BY Id ";
                    }
                }
                else
                {
                    if (query == "FIRST")
                    {
                        sqlText += " ORDER BY Id ";
                    }
                    else if (query == "LAST")
                    {
                        sqlText += " ORDER BY Id DESC";
                    }
                    else if (query == "NEXT")
                    {
                        sqlText += " and  Id > @Id   ORDER BY Id";
                    }
                    else if (query == "PREVIOUS")
                    {
                        sqlText += "  and  Id < @Id   ORDER BY Id DESC";
                    }
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (Id != null)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                //SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                //dataAdapter.Fill(dataSet);
                SqlDataReader dr;
                try
                {
                    dr = objComm.ExecuteReader();
                    while (dr.Read())
                    {
                        gmployeeInfoVM.Id = dr["Id"].ToString();
                        gmployeeInfoVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        gmployeeInfoVM.Code = dr["Code"].ToString();
                        gmployeeInfoVM.Salutation_E = dr["Salutation_E"].ToString();
                        gmployeeInfoVM.MiddleName = dr["MiddleName"].ToString();
                        gmployeeInfoVM.LastName = dr["LastName"].ToString();
                        gmployeeInfoVM.Remarks = dr["Remarks"].ToString();
                        gmployeeInfoVM.AttnUserId = dr["AttnUserId"].ToString();
                        gmployeeInfoVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        gmployeeInfoVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        gmployeeInfoVM.CreatedBy = dr["CreatedBy"].ToString();
                        gmployeeInfoVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        gmployeeInfoVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        gmployeeInfoVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        gmployeeInfoVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
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
            return gmployeeInfoVM;
        }

        //==================Delete =================
        public string[] Delete(EmployeeInfoVM EmployeeInfoVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeInfo"; //Method Name
            int transResult = 0;
            int countId = 0;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToEmployeeInfo"); }
                #endregion open connection and transaction
                #region Check is  it used
                #endregion Check is  it used
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update EmployeeInfo set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", EmployeeInfoVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", EmployeeInfoVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", EmployeeInfoVM.LastUpdateFrom);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeInfo Delete", EmployeeInfoVM.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("EmployeeInfo Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete EmployeeInfo Information.";
                    throw new ArgumentNullException("", "");
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction != null)
                {
                    try
                    {
                        if (Vtransaction == null) { transaction.Rollback(); }
                    }
                    catch (Exception)
                    {
                        retResults[1] = "Unexpected error to update EmployeeInfo.";
                        return retResults;
                        // throw new ArgumentNullException("Unexpected error to update EmployeeInfo.", "EmployeeInfo");
                    }
                }
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

        public List<string> AutocompleteMarge(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
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
                sqlText = @"SELECT Id, Code ,EmpName   FROM ViewEmployeeInformation ";
                sqlText += @" WHERE (Code like '%" + term + "%'  or  EmpName like '%" + term + "%') and IsArchive=0 and IsActive=1 ORDER BY Code";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Code"].ToString() + " > " + dr["EmpName"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
            return vms;
        }

        public string EmployeeExist(string Item)
        {
            #region Variables
            string result = "";
            SqlConnection currConn = null;
            string sqlText = "";
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
                sqlText = @"SELECT Id, Code ,EmpName   FROM ViewEmployeeInformation ";
                sqlText += @" WHERE (Code = '" + Item + "'  or  EmpName = '" + Item + "') and IsArchive=0 and IsActive=1 ORDER BY Code";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    result = dr["Code"].ToString() + ">" + dr["EmpName"].ToString() + ">" + dr["Id"].ToString();// vms.Insert(i, dr["Code"].ToString() + " - " + dr["Name"].ToString());
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
            return result;
        }

        public List<string> Autocomplete(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
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
                sqlText = @"SELECT Id, Code ,MiddleName   FROM EmployeeInfo ";
                sqlText += @" WHERE Code like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Code";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Code"].ToString() + " > " + dr["MiddleName"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
            return vms;
        }

        public List<string> AutocompleteCode(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
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
                sqlText = @"SELECT Code, Salutation_E ,MiddleName, LastName   FROM EmployeeInfo ";
                sqlText += @" WHERE Code like '%" + term + "%' OR Salutation_E like '%" + term + "%' OR MiddleName like '%" + term + @"%' 
                OR LastName like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Code";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Code"].ToString() + "~" + dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
            return vms;
        }

        public List<string> AutocompleteCodeAll(string term)
        {
            #region Variables
            SqlConnection currConn = null;
            List<string> vms = new List<string>();
            string sqlText = "";
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
                sqlText = @"SELECT Code, Salutation_E ,MiddleName, LastName   FROM EmployeeInfo ";
                sqlText += @" WHERE Code like '%" + term + "%' OR Salutation_E like '%" + term + "%' OR MiddleName like '%" + term + @"%' 
                OR LastName like '%" + term + "%' ORDER BY Code";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Code"].ToString() + "~" + dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString());
                    i++;
                }
                dr.Close();
                vms.Sort();
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
            return vms;
        }

        public string[] EmployeeNameByCode(string empCode)
        {
            if (!string.IsNullOrWhiteSpace(empCode))
            {
                if (empCode.Contains('>'))
                {
                    empCode = empCode.Split('>')[0];
                }
            }
            #region Variables
            SqlConnection currConn = null;
            string[] employee = new string[3];
            string sqlText = "";
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
                sqlText = @"select Top 1 Id,Salutation_E,MiddleName,LastName from EmployeeInfo";
                sqlText += @" WHERE Code =@empCode and IsArchive=0 and IsActive=1 ";
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@empCode", empCode);
                using (SqlDataReader dr = _objComm.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        employee[0] = dr["Id"].ToString();
                        employee[1] = dr["Salutation_E"].ToString() + " " + dr["MiddleName"].ToString() + " " + dr["LastName"].ToString();
                    }
                    dr.Close();
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
            return employee;
        }

        //==================Reports=================

        #region List Reports

        public List<AttLogsVM> EA(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string dtAtnFrom, string dtAtnTo, string PunchMissIn
            , string PunchMissOut, string ReportNo, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<AttLogsVM> attLogsVMs = new List<AttLogsVM>();
            AttLogsVM attLogsVM;
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
                //                declare @CodeF as  varchar(50)
                //declare @CodeT  as varchar(50)
                //declare @Name as  varchar(50)
                //declare @Department as  varchar(50)
                //declare @Project as  varchar(50)
                //declare @dtpJFrom  as varchar(50)
                //declare @dtpJTo as  varchar(50)
                //declare @dtpFrom as  varchar(50)
                //declare @dtpTo as  varchar(50)
                sqlText += @" ";
                sqlText += @"
";
                sqlText += @" 
select  distinct Id EmployeeId,Code,	EmpName,	JoinDate,	Project,	Section,	Department,		Designation	,ProjectId,	SectionId,	DepartmentId	,DesignationId	
 from ViewEmployeeInformation
where IsArchive=0 
";
                if (CodeF != "0_0") sqlText += "  and Code>=@CodeF";
                if (CodeT != "0_0") sqlText += "  and Code<=@CodeT";
                if (ProjectId != "0_0") sqlText += "  and Projectid=@ProjectId";
                if (SectionId != "0_0") sqlText += "  and SectionId=@SectionId";
                if (DepartmentId != "0_0") sqlText += "  and DepartmentId=@DepartmentId";
                if (DesignationId != "0_0") sqlText += "  and DesignationId=@DesignationId";
                if (dtpFrom != "0_0") sqlText += "  and JoinDate>=@dtpFrom";
                if (dtpTo != "0_0") sqlText += "  and JoinDate<=@dtpTo";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and id='" + EmployeeId + "'";
                }
                sqlText += @" 
select * from ViewEmployeeAttendance
where IsArchive=0 
";
                if (CodeF != "0_0") sqlText += "  and Code>=@CodeF";
                if (CodeT != "0_0") sqlText += "  and Code<=@CodeT";
                if (ProjectId != "0_0") sqlText += "  and projectid=@ProjectId";
                if (SectionId != "0_0") sqlText += "  and SectionId=@SectionId";
                if (DepartmentId != "0_0") sqlText += "  and DepartmentId=@DepartmentId";
                if (DesignationId != "0_0") sqlText += "  and DesignationId=@DesignationId";
                if (dtAtnFrom != "0_0") sqlText += "  and AttnDate>=@dtAtnFrom";
                if (dtAtnTo != "0_0") sqlText += "  and AttnDate<=@dtAtnTo";
                if (dtpFrom != "0_0") sqlText += "  and JoinDate>=@dtpFrom";
                if (dtpTo != "0_0") sqlText += "  and JoinDate<=@dtpTo";
                if (PunchMissIn != "0000") sqlText += "  and InPunchTime ==@PunchMissIn";
                if (PunchMissOut != "0000") sqlText += "  and OutPunchTime ==@PunchMissOut";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and EmployeeId='" + EmployeeId + "'";
                }
                sqlText += @" 
select * from ViewEmployeeLeave
where IsArchive=0 
";
                if (CodeF != "0_0") sqlText += "  and Code>=@CodeF";
                if (CodeT != "0_0") sqlText += "  and Code<=@CodeT";
                if (ProjectId != "0_0") sqlText += "  and projectid=@ProjectId";
                if (SectionId != "0_0") sqlText += "  and SectionId=@SectionId";
                if (DepartmentId != "0_0") sqlText += "  and DepartmentId=@DepartmentId";
                if (DesignationId != "0_0") sqlText += "  and DesignationId=@DesignationId";
                if (dtAtnFrom != "0_0") sqlText += "  and LeaveDate>=@dtAtnFrom";
                if (dtAtnTo != "0_0") sqlText += "  and LeaveDate <@dtAtnTo";
                if (dtpFrom != "0_0") sqlText += "  and JoinDate>=@dtpFrom";
                if (dtpTo != "0_0") sqlText += "  and JoinDate<=@dtpTo";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and EmployeeId='" + EmployeeId + "'";
                }
                sqlText += @" 
select * from HoliDay
where IsArchive=0 
";
                if (dtAtnFrom != "0_0") sqlText += "  and HoliDay>=@dtAtnFrom";
                if (dtAtnTo != "0_0") sqlText += "  and HoliDay<=@dtAtnTo";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (!string.IsNullOrWhiteSpace(dtAtnFrom))
                {
                    objComm.Parameters.AddWithValue("@dtAtnFrom", Ordinary.DateToString(dtAtnFrom));
                }
                if (dtAtnTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtAtnTo", Ordinary.DateToString(dtAtnTo));
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(Convert.ToDateTime(dtpTo).AddDays(1).ToString()));
                }
                #endregion
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int diff = 0;
                //if (dtAtnFrom != "0_0" && )
                //{
                diff = Convert.ToInt32((Convert.ToDateTime(dtAtnTo) - Convert.ToDateTime(dtAtnFrom)).TotalDays) + 1;
                //}
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string vEmployeeId = dr["EmployeeId"].ToString();
                    for (int i = 0; i < diff; i++)
                    {
                        string ToDate = Ordinary.DateToString(Convert.ToDateTime(dtAtnFrom).AddDays(i).ToString());
                        DataRow[] holiDay = ds.Tables[3].Select("HoliDay = '" + ToDate + "' ");
                        DataRow[] leaveDay = ds.Tables[2].Select("LeaveDate = '" + ToDate + "'  AND EmployeeId = '" + vEmployeeId + "' ");
                        DataRow[] AttnDay = ds.Tables[1].Select("AttnDate = '" + ToDate + "'  AND EmployeeId = '" + vEmployeeId + "' ");
                        if (holiDay.Length > 0)
                        {
                            //if (ReportNo == "1")
                            //{
                            #region holiDay
                            foreach (DataRow row in holiDay)
                            {
                                attLogsVM = new AttLogsVM();
                                attLogsVM.UserID = "";
                                attLogsVM.AttnDate = Ordinary.StringToDate(row["HoliDay"].ToString());
                                attLogsVM.InPunchTime = "00:00";
                                attLogsVM.OutPunchTime = "00:00";
                                attLogsVM.LunchTime = "00:00";
                                attLogsVM.LunchBreak = "0";
                                attLogsVM.Code = dr["Code"].ToString();
                                attLogsVM.EmpName = dr["EmpName"].ToString();
                                attLogsVM.Project = dr["Project"].ToString();
                                attLogsVM.Section = dr["Section"].ToString();
                                attLogsVM.Department = dr["Department"].ToString();
                                attLogsVM.Designation = dr["Designation"].ToString();
                                attLogsVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                                attLogsVM.ProjectId = dr["ProjectId"].ToString();
                                attLogsVM.SectionId = dr["SectionId"].ToString();
                                attLogsVM.DepartmentId = dr["DepartmentId"].ToString();
                                attLogsVM.DesignationId = dr["DesignationId"].ToString();
                                attLogsVM.EmployeeId = dr["EmployeeId"].ToString();
                                attLogsVM.WorkingHour = 0;
                                attLogsVM.LateMin = 0;
                                attLogsVM.WorkingTime = "-";
                                attLogsVM.AttnStatus = row["HoliDayType"].ToString();
                                attLogsVMs.Add(attLogsVM);
                                break;
                            }
                            #endregion holiDay
                            //}
                        }
                        else if (leaveDay.Length > 0)
                        {
                            #region leaveDay
                            foreach (DataRow row in leaveDay)
                            {
                                attLogsVM = new AttLogsVM();
                                attLogsVM.UserID = "";
                                attLogsVM.AttnDate = Ordinary.StringToDate(row["LeaveDate"].ToString());
                                attLogsVM.InPunchTime = "00:00";
                                attLogsVM.OutPunchTime = "00:00";
                                attLogsVM.LunchTime = "00:00";
                                attLogsVM.LunchBreak = "0";
                                attLogsVM.Code = dr["Code"].ToString();
                                attLogsVM.EmpName = dr["EmpName"].ToString();
                                attLogsVM.Project = dr["Project"].ToString();
                                attLogsVM.Section = dr["Section"].ToString();
                                attLogsVM.Department = dr["Department"].ToString();
                                attLogsVM.Designation = dr["Designation"].ToString();
                                attLogsVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                                attLogsVM.ProjectId = dr["ProjectId"].ToString();
                                attLogsVM.SectionId = dr["SectionId"].ToString();
                                attLogsVM.DepartmentId = dr["DepartmentId"].ToString();
                                attLogsVM.DesignationId = dr["DesignationId"].ToString();
                                attLogsVM.EmployeeId = dr["EmployeeId"].ToString();
                                attLogsVM.WorkingHour = 0;
                                attLogsVM.LateMin = 0;
                                attLogsVM.WorkingTime = "-";
                                attLogsVM.AttnStatus = row["LeaveType_E"].ToString();
                                attLogsVMs.Add(attLogsVM);
                                break;
                            }
                            #endregion leaveDay
                        }
                        else if (AttnDay.Length > 0)
                        {
                            #region AttnDay
                            foreach (DataRow row in AttnDay)
                            {
                                attLogsVM = new AttLogsVM();
                                attLogsVM.UserID = row["UserID"].ToString();
                                attLogsVM.AttnDate = Ordinary.StringToDate(row["AttnDate"].ToString());
                                attLogsVM.InPunchTime = Ordinary.StringToTime(row["InPunchTime"].ToString());
                                attLogsVM.OutPunchTime = Ordinary.StringToTime(row["OutPunchTime"].ToString());
                                attLogsVM.LunchTime = Ordinary.StringToTime(row["LunchTime"].ToString());
                                attLogsVM.LunchBreak = row["LunchBreak"].ToString();
                                attLogsVM.Code = row["Code"].ToString();
                                attLogsVM.EmpName = row["EmpName"].ToString();
                                attLogsVM.Project = row["Project"].ToString();
                                attLogsVM.Section = row["Section"].ToString();
                                attLogsVM.Department = row["Department"].ToString();
                                attLogsVM.Designation = row["Designation"].ToString();
                                attLogsVM.JoinDate = Ordinary.StringToDate(row["JoinDate"].ToString());
                                attLogsVM.ProjectId = row["ProjectId"].ToString();
                                attLogsVM.SectionId = row["SectionId"].ToString();
                                attLogsVM.DepartmentId = row["DepartmentId"].ToString();
                                attLogsVM.DesignationId = row["DesignationId"].ToString();
                                attLogsVM.EmployeeId = row["EmployeeId"].ToString();
                                attLogsVM.WorkingHour = Convert.ToInt32(row["WorkingHour"].ToString());
                                attLogsVM.LateMin = Convert.ToInt32(row["LateMin"].ToString());
                                int totalMin = Convert.ToInt32(row["WorkingHour"].ToString());
                                int restMin = Convert.ToInt32(row["WorkingHour"].ToString()) % 60;
                                int hrs = (totalMin - restMin) / 60;
                                attLogsVM.WorkingTime = hrs.ToString() + " H " + restMin.ToString() + " M";
                                attLogsVM.AttnStatus = "Present";
                                if (row["InPunchTime"].ToString() == "0000")
                                {
                                    attLogsVM.AttnStatus = "Present (In Miss)";
                                }
                                else if (row["OutPunchTime"].ToString() == "0000")
                                {
                                    attLogsVM.AttnStatus = "Present (Out Miss)";
                                }
                                else if (attLogsVM.LateMin > 0)
                                {
                                    attLogsVM.AttnStatus = "Present (Late)";
                                }
                                attLogsVMs.Add(attLogsVM);
                                break;
                            }
                            #endregion AttnDay
                        }
                        else  //Absent
                        {
                            #region Absent
                            //if (ReportNo == "1")
                            //{
                            attLogsVM = new AttLogsVM();
                            attLogsVM.UserID = "";
                            attLogsVM.AttnDate = Ordinary.StringToDate(ToDate);
                            attLogsVM.InPunchTime = "00:00";
                            attLogsVM.OutPunchTime = "00:00";
                            attLogsVM.LunchTime = "00:00";
                            attLogsVM.LunchBreak = "0";
                            attLogsVM.Code = dr["Code"].ToString();
                            attLogsVM.EmpName = dr["EmpName"].ToString();
                            attLogsVM.Project = dr["Project"].ToString();
                            attLogsVM.Section = dr["Section"].ToString();
                            attLogsVM.Department = dr["Department"].ToString();
                            attLogsVM.Designation = dr["Designation"].ToString();
                            attLogsVM.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                            attLogsVM.ProjectId = dr["ProjectId"].ToString();
                            attLogsVM.SectionId = dr["SectionId"].ToString();
                            attLogsVM.DepartmentId = dr["DepartmentId"].ToString();
                            attLogsVM.DesignationId = dr["DesignationId"].ToString();
                            attLogsVM.EmployeeId = dr["EmployeeId"].ToString();
                            attLogsVM.WorkingHour = 0;
                            attLogsVM.LateMin = 0;
                            attLogsVM.AttnStatus = "Absent";
                            attLogsVMs.Add(attLogsVM);
                            //break;
                            //}
                            #endregion Absent
                        }
                    }
                }
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
            return attLogsVMs;
        }

        public List<EmployeeLeaveVM> EmployeeLeaveRegister(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveVM> VMs = new List<EmployeeLeaveVM>();
            EmployeeLeaveVM vm;
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
select
ei.Code EmpCode
, ei.EmpName
, ei.EmployeeId
, ei.JoinDate     
, ei.Branch       
, ei.Project      
, ei.Department   
, ei.Section      
, ei.Designation  
, ei.ProjectId    
, ei.DepartmentId 
, ei.SectionId    
, ei.DesignationId
, ei.Grade        
, isnull(eleave.LeaveYear, 0)           LeaveYear
, isnull(eleave.LeaveType_E,	N'NA')	LeaveType_E
, isnull(eleave.FromDate	,	N'NA')	FromDate
, isnull(eleave.ToDate		,	N'NA')	ToDate
, isnull(eleave.TotalLeave	,	0)		TotalLeave
, isnull(eleave.ApprovedBy	,	N'NA')	ApprovedBy
, isnull(eleave.ApproveDate,	N'NA')	ApproveDate
, isnull(eleave.IsApprove	,	0)		IsApprove
, isnull(eleave.RejectedBy	,	N'NA')	RejectedBy
, isnull(eleave.RejectDate	,	N'NA')	RejectDate
, isnull(eleave.IsReject	,	0)		IsReject
, isnull(eleave.IsHalfDay	,	0)		IsHalfDay
from
ViewEmployeeInformationAll ei
left outer join EmployeeLeave eleave on ei.EmployeeId = eleave.EmployeeId
where ei.IsArchive=0 and ei.IsActive=1
";
                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and eleave.leaveyear=@leaveyear";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and eleave.LEAVETYPE_E=@LEAVETYPE_E";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId='" + EmployeeId + "'";
                }
                sqlText += "  ORDER BY ei.Department, ei.GradeSL, ei.JoinDate, ei.Code, eleave.LEAVETYPE_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    objComm.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    objComm.Parameters.AddWithValue("@LEAVETYPE_E", LEAVETYPE_E);
                }
                #endregion Parrameters Apply
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeaveVM();
                    vm.EmpCode = dr["EmpCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Branch = dr["Branch"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"].ToString());
                    vm.LeaveType_E = dr["LeaveType_E"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"].ToString());
                    vm.ApprovedBy = dr["ApprovedBy"].ToString();
                    vm.ApproveDate = Ordinary.StringToDate(dr["ApproveDate"].ToString());
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"].ToString());
                    vm.RejectedBy = dr["RejectedBy"].ToString();
                    vm.RejectDate = Ordinary.StringToDate(dr["RejectDate"].ToString());
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"].ToString());
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"].ToString());
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

        public List<EmployeeLeaveBalanceVM> EmployeeLeaveList(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId
            , string Gender_E, string Religion, string GradeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveBalanceVM> VMs = new List<EmployeeLeaveBalanceVM>();
            EmployeeLeaveBalanceVM vm;
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
SELECT ei.*
,pd.DateOfBirth
,pd.Gender_E
,pd.Religion
,ei.Grade
,ei.GradeId
,ei.GradeSL
,ES.ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE
FROM ViewEmployeeInformation ei 
left outer join EMPLOYEELEAVESTRUCTURE ES on ES.EMPLOYEEID=ei.id
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
left outer join EmployeePersonalDetail pd on pd.employeeId=ei.Id
where ei.IsArchive=0 and ei.IsActive=1
";
                #region Parrameters Apply
                if (!string.IsNullOrWhiteSpace(CodeF))
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (!string.IsNullOrWhiteSpace(CodeT))
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                //if ( !string.IsNullOrWhiteSpace(ProjectId))
                //{
                //    sqlText += "  and ei.projectid=@ProjectId";
                //}
                //if (!string.IsNullOrWhiteSpace(SectionId))
                //{
                //    sqlText += "  and ei.SectionId=@SectionId";
                //}
                //if (!string.IsNullOrWhiteSpace(DepartmentId))
                //{
                //    sqlText += "  and ei.Departmentid=@DepartmentId";
                //}
                //if (!string.IsNullOrWhiteSpace(DesignationId))
                //{
                //    sqlText += "  and ei.DesignationId=@DesignationId";
                //}
                //if (!string.IsNullOrWhiteSpace(dtpFrom))
                //{
                //    sqlText += "  and ei.JoinDate>=@dtpFrom";
                //}
                //if (!string.IsNullOrWhiteSpace(dtpTo))
                //{
                //    sqlText += "  and ei.JoinDate<=@dtpTo";
                //}
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and es.leaveyear=@leaveyear";
                }
                //if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                //{
                //    sqlText += "  and es.LEAVETYPE_E=@LEAVETYPE_E";
                //}
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId='" + EmployeeId + "'";
                }
                //if (!string.IsNullOrWhiteSpace(Gender_E))
                //{
                //    sqlText += "  and pd.Gender_E=@Gender_E";
                //}
                //if (!string.IsNullOrWhiteSpace(Religion))
                //{
                //    sqlText += "  and  pd.Religion=@Religion";
                //}
                //if (!string.IsNullOrWhiteSpace(GradeId))
                //{
                //    sqlText += "  and ei.GradeId=@GradeId";
                //}
                sqlText += "  ORDER BY ei.Department, ei.GradeSL, ei.JoinDate, ei.Code, ES.LEAVETYPE_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(CodeF))
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (!string.IsNullOrWhiteSpace(CodeT))
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (!string.IsNullOrWhiteSpace(DepartmentId))
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (!string.IsNullOrWhiteSpace(DesignationId))
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (!string.IsNullOrWhiteSpace(ProjectId))
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (!string.IsNullOrWhiteSpace(SectionId))
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (!string.IsNullOrWhiteSpace(dtpFrom))
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (!string.IsNullOrWhiteSpace(dtpTo))
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    objComm.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    objComm.Parameters.AddWithValue("@LEAVETYPE_E", LEAVETYPE_E);
                }
                if (!string.IsNullOrWhiteSpace(Gender_E))
                {
                    objComm.Parameters.AddWithValue("@Gender_E", Gender_E);
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    objComm.Parameters.AddWithValue("@Religion", Religion);
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    objComm.Parameters.AddWithValue("@GradeId", GradeId);
                }
                #endregion Parrameters Apply
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new EmployeeLeaveBalanceVM();
                    vm.Id = dr["id"].ToString();
                    vm.EmpCode = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.JoinDate = Convert.ToDateTime(Ordinary.StringToDate(dr["JoinDate"].ToString()));
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    //gmployeeInfoVM.DepartmentId = dr["DepartmentId"].ToString();
                    //gmployeeInfoVM.SectionId = dr["SectionId"].ToString();
                    //gmployeeInfoVM.DesignationId = dr["DesignationId"].ToString();
                    //gmployeeInfoVM.ProjectId = dr["ProjectId"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.Have = dr["HAVE"].ToString();
                    vm.Used = dr["USED"].ToString();
                    vm.Total = dr["LEAVEDAYS"].ToString();
                    vm.OpeningBalance = dr["OpeningLeaveDays"].ToString();
                    vm.LeaveType = dr["LEAVETYPE_E"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.Religion = dr["Religion"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    VMs.Add(vm);
                }
                //SqlDataReader dr;
                //dr = objComm.ExecuteReader();
                //while (dr.Read())
                //{
                //    gmployeeInfoVM = new EmployeeLeaveBalanceVM();
                //    gmployeeInfoVM.Id =Convert.ToInt32( dr["id"].ToString());
                //    gmployeeInfoVM.EmpCode = dr["Code"].ToString();
                //    gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                //    gmployeeInfoVM.JoinDate =Convert.ToDateTime( Ordinary.StringToDate(dr["JoinDate"].ToString()));
                //    gmployeeInfoVM.Project = dr["Project"].ToString();
                //    gmployeeInfoVM.Department = dr["Department"].ToString();
                //    gmployeeInfoVM.Section = dr["Section"].ToString();
                //    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                //    gmployeeInfoVM.Project = dr["Project"].ToString();
                //    gmployeeInfoVM.Section = dr["Section"].ToString();
                //    gmployeeInfoVM.Grade = dr["Grade"].ToString();
                //    gmployeeInfoVM.Have = dr["HAVE"].ToString();
                //    gmployeeInfoVM.Used = dr["USED"].ToString();
                //    gmployeeInfoVM.Total = dr["LEAVEDAYS"].ToString();
                //    gmployeeInfoVM.OpeningBalance = dr["OpeningLeaveDays"].ToString();
                //    gmployeeInfoVM.LeaveType = dr["LEAVETYPE_E"].ToString();
                //    gmployeeInfoVMs.Add(gmployeeInfoVM);
                //}
                //dr.Close();
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

        public List<EmployeeLeaveBalanceVM> EmployeeLeaveStatementNew(string CodeF, string CodeT, string DepartmentId
            , string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId, string LId = "0")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveBalanceVM> VMs = new List<EmployeeLeaveBalanceVM>();
            EmployeeLeaveBalanceVM vm;
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

ele.EmployeeId
,isnull(ele.IsReject,0)IsReject
,ele.EmployeeLeaveStructureId
,ele.LeaveYear
,ele.LeaveType_E CurrentLeaveType
,ele.FromDate
,ele.ToDate
,ele.TotalLeave
,ele.approvedby
,isnull(ele.ApproveDate,'19000101')ApproveDate
,ele.IsApprove
,isnull(ele.RejectDate,'19000101')RejectDate
,ele.IsHalfDay
,isnull(ele.Remarks,'NA') LeavePurpose
,eview.Code
,eview.EmpName
,eview.JoinDate
,eview.Department
,eview.Section
,eview.Designation
,eview.Project
,eview.Grade
,eview.Email
,eview.Supervisor
,es.ID
,es.LEAVETYPE_E 
,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays
,isnull( es.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED
,ISNULL(isnull( es.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE
FROM ViewEmployeeInformation eview 
LEFT OUTER JOIN EmployeeLeaveStructure es on es.EmployeeId=eview.id
LEFT OUTER JOIN (
SELECT EmployeeLeaveStructureId,SUM(TotalLeave) Leave FROM EmployeeLeave where IsApprove=1  GROUP BY EmployeeLeaveStructureId
) EL ON EL.EmployeeLeaveStructureId=es.ID
left outer join EmployeeLeave ele on ele.EmployeeId = eview.id
where 1=1
AND eview.IsArchive=0 and eview.IsActive=1
and es.LeaveYear=ele.LeaveYear
";
                #region Parameters
                if (LId != "0")
                {
                    sqlText += "  and ele.Id=@LId";
                }
                if (CodeF != "0_0")
                {
                    sqlText += "  and eview.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and eview.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and eview.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and eview.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and eview.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and eview.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and eview.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and eview.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and es.leaveyear=@leaveyear";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and es.LEAVETYPE_E=@LEAVETYPE_E";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and eview.id='" + EmployeeId + "'";
                }
                sqlText += @"   ORDER BY eview.project, eview.department, eview.section, eview.code, ES.LEAVETYPE_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (LId != "0")
                {
                    objComm.Parameters.AddWithValue("@LId", LId);
                }
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    objComm.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    objComm.Parameters.AddWithValue("@LEAVETYPE_E", LEAVETYPE_E);
                }
                #endregion Parameters

                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new EmployeeLeaveBalanceVM();
                    vm.Id = dr["id"].ToString();
                    vm.EmpCode = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.JoinDate = Convert.ToDateTime(Ordinary.StringToDate(dr["JoinDate"].ToString()));
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.Have = dr["HAVE"].ToString();
                    vm.Used = dr["USED"].ToString();
                    vm.Total = dr["LEAVEDAYS"].ToString();
                    vm.OpeningBalance = dr["OpeningLeaveDays"].ToString();
                    vm.LeaveType = dr["LEAVETYPE_E"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeLeaveStructureId = Convert.ToInt32(dr["EmployeeLeaveStructureId"].ToString());
                    vm.LeaveYear = Convert.ToInt32(dr["LeaveYear"].ToString());
                    vm.CurrentLeaveType = dr["CurrentLeaveType"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.TotalLeave = Convert.ToDecimal(dr["TotalLeave"].ToString());
                    vm.approvedby = dr["approvedby"].ToString();
                    vm.ApproveDate = Ordinary.StringToDate(dr["ApproveDate"].ToString());
                    vm.IsApprove = Convert.ToBoolean(dr["IsApprove"].ToString());
                    vm.RejectDate = Ordinary.StringToDate(dr["RejectDate"].ToString());
                    vm.IsReject = Convert.ToBoolean(dr["IsReject"].ToString());
                    vm.IsHalfDay = Convert.ToBoolean(dr["IsHalfDay"].ToString());
                    vm.LeavePurpose = dr["LeavePurpose"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.EmpEmail = dr["Email"].ToString();
                    vm.Branch = "";
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

        public List<EmployeeLeaveStatementVM> EmployeeLeaveStatement(string CodeF, string CodeT, string DepartmentId
            , string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LeaveType, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeaveStatementVM> vms = new List<EmployeeLeaveStatementVM>();
            EmployeeLeaveStatementVM vm;
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
select
  ISNULL(el.LeaveType_E,'NA') ELTypeName,ISNULL(el.OpeningLeaveDays,0) ELOpening,ISNULL(el.LEAVEDAYS,0) ELLDays,ISNULL(el.USED,0) ELUsed,ISNULL(el.HAVE,0) ELBalance
, ISNULL(sl.LeaveType_E,'NA') SLTypeName,ISNULL(Sl.OpeningLeaveDays,0) SLOpening,ISNULL(Sl.LEAVEDAYS,0) SLLDays,ISNULL(Sl.USED,0) SLUsed,ISNULL(Sl.HAVE,0) SLBalance
, ISNULL(Cl.LeaveType_E,'NA') CLTypeName,ISNULL(Cl.OpeningLeaveDays,0) CLOpening,ISNULL(Cl.LEAVEDAYS,0) CLLDays,ISNULL(Cl.USED,0) CLUsed,ISNULL(Cl.HAVE,0) CLBalance
, einfo.* from ViewEmployeeInformation einfo
 left outer join(select es.EmployeeId, ES.ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE from EMPLOYEELEAVESTRUCTURE ES  
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
where es.LeaveType_E='Earned Leave' and es.LeaveYear=@leaveyear)  el on einfo.id=el.EmployeeId
 left outer join(select es.EmployeeId, ES.ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE from EMPLOYEELEAVESTRUCTURE ES  
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
where es.LeaveType_E='Sick Leave' and es.LeaveYear=@leaveyear)  sl on einfo.id=sl.EmployeeId
left outer join(select es.EmployeeId, ES.ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(EL.LEAVE,0),0) HAVE from EMPLOYEELEAVESTRUCTURE ES  
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
where es.LeaveType_E='Compensatory Leave' and es.LeaveYear=@leaveyear
)  cl on einfo.id=cl.EmployeeId
where 1=1 and einfo.IsArchive=0 and einfo.IsActive = 1
";
                if (CodeF != "0_0")
                {
                    sqlText += "  and einfo.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and einfo.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and einfo.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and einfo.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and einfo.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and einfo.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and einfo.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and einfo.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and einfo.id='" + EmployeeId + "'";
                }
                sqlText += @"    ORDER BY  einfo.code, einfo.project,  einfo.department,  einfo.section  ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    objComm.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    vm = new EmployeeLeaveStatementVM();
                    vm.Id = dr["id"].ToString();
                    vm.ELTypeName = dr["ELTypeName"].ToString();
                    vm.ELOpening = Convert.ToDecimal(dr["ELOpening"].ToString());
                    vm.ELLDays = Convert.ToDecimal(dr["ELLDays"].ToString());
                    vm.ELUsed = Convert.ToDecimal(dr["ELUsed"].ToString());
                    vm.ELBalance = Convert.ToDecimal(dr["ELBalance"].ToString());
                    vm.SLTypeName = dr["SLTypeName"].ToString();
                    vm.SLOpening = Convert.ToDecimal(dr["SLOpening"].ToString());
                    vm.SLLDays = Convert.ToDecimal(dr["SLLDays"].ToString());
                    vm.SLUsed = Convert.ToDecimal(dr["SLUsed"].ToString());
                    vm.SLBalance = Convert.ToDecimal(dr["SLBalance"].ToString());
                    vm.CLTypeName = dr["CLTypeName"].ToString();
                    vm.CLOpening = Convert.ToDecimal(dr["CLOpening"].ToString());
                    vm.CLLDays = Convert.ToDecimal(dr["CLLDays"].ToString());
                    vm.CLUsed = Convert.ToDecimal(dr["CLUsed"].ToString());
                    vm.CLBalance = Convert.ToDecimal(dr["CLBalance"].ToString());
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.JoinDate = Convert.ToDateTime(Ordinary.StringToDate(dr["JoinDate"].ToString()));
                    vm.ProbationEnd = Convert.ToDateTime(Ordinary.StringToDate(dr["ProbationEnd"].ToString()));
                    vm.DateOfPermanent = Convert.ToDateTime(Ordinary.StringToDate(dr["DateOfPermanent"].ToString()));
                    vm.EmploymentStatus = dr["EmploymentStatus"].ToString();
                    vm.EmploymentType = dr["EmploymentType"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.TransferDate = Convert.ToDateTime(Ordinary.StringToDate(dr["TransferDate"].ToString()));
                    vm.Designation = dr["Designation"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.IsPromotion = Convert.ToBoolean(dr["IsPromotion"].ToString());
                    vm.PromotionDate = Convert.ToDateTime(Ordinary.StringToDate(dr["PromotionDate"].ToString()));
                    //vm.LeaveType = dr["LEAVETYPE_E"].ToString();
                    //vm.LeaveYear = Convert.ToInt32(dr["leaveyear"].ToString());
                    //vm.leaveyear
                    //gmployeeInfoVM.EmpCode = dr[""].ToString();
                    //gmployeeInfoVM.EmpName = dr["EmpName"].ToString();
                    //gmployeeInfoVM.JoinDate = Convert.ToDateTime(Ordinary.StringToDate(dr["JoinDate"].ToString()));
                    //gmployeeInfoVM.Project = dr["Project"].ToString();
                    //gmployeeInfoVM.Branch = dr["Branch"].ToString();
                    //gmployeeInfoVM.Department = dr["Department"].ToString();
                    //gmployeeInfoVM.Section = dr["Section"].ToString();
                    //gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    //gmployeeInfoVM.Project = dr["Project"].ToString();
                    //gmployeeInfoVM.Section = dr["Section"].ToString();
                    //gmployeeInfoVM.Grade = dr["Grade"].ToString();
                    //gmployeeInfoVM.Have = dr["HAVE"].ToString();
                    //gmployeeInfoVM.Used = dr["USED"].ToString();
                    //gmployeeInfoVM.Total = dr["LEAVEDAYS"].ToString();
                    //gmployeeInfoVM.OpeningBalance = dr["OpeningLeaveDays"].ToString();
                    //gmployeeInfoVM.LeaveType = dr["LEAVETYPE_E"].ToString();
                    vms.Add(vm);
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
            return vms;
        }
        public List<EmployeeInfoVM> EmployeeProfilesFull(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string BloodGroup, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
DISTINCT ei.EmployeeId
, ei.Code, ei.EmpName, ei.Department, ei.Section, ei.JoinDate, ei.GradeSL, ei.Project, ei.AttnUserId
, PD.OtherId, PD.NickName, PD.DateOfBirth, PD.Religion, PD.BloodGroup_E
, PD.Gender_E, PD.MaritalStatus_E, PD.Nationality_E, PD.Email, IsNull(PD.Smoker, 0) Smoker 
, PD.PassportNumber, PD.ExpiryDate, PD.TIN, PD.NID, IsNull(PD.IsDisable, 0) IsDisable, PD.KindsOfDisability
FROM
ViewEmployeeInformation ei
LEFT OUTER JOIN EmployeePersonalDetail			PD ON PD.EmployeeId = ei.Id
WHERE ei.isArchive = 0 and ei.isActive=1
";
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(BloodGroup))
                {
                    sqlText += "  and pd.BloodGroup_E=@BloodGroup";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId=@EmployeeId";
                }
                sqlText += "  order by ei.Department, ei.GradeSL, ei.joindate, ei.Code ";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(BloodGroup))
                {
                    objComm.Parameters.AddWithValue("@BloodGroup", BloodGroup);
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.JoinDate = dr["JoinDate"].ToString();
                    vm.GradeSL = dr["GradeSL"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.AttnUserId = dr["AttnUserId"].ToString();
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.NickName = dr["NickName"].ToString();
                    vm.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());
                    vm.Religion = dr["Religion"].ToString();
                    vm.BloodGroup_E = dr["BloodGroup_E"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.MaritalStatus_E = dr["MaritalStatus_E"].ToString();
                    vm.Nationality_E = dr["Nationality_E"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Smoker = Convert.ToBoolean(dr["Smoker"].ToString());
                    vm.PassportNumber = dr["PassportNumber"].ToString();
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());
                    vm.TIN = dr["TIN"].ToString();
                    vm.NID = dr["NID"].ToString();
                    vm.IsDisable = Convert.ToBoolean(dr["IsDisable"].ToString());
                    vm.KindsOfDisability = dr["KindsOfDisability"].ToString();
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

        public List<EmployeeInfoVM> EmployeeList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string BloodGroup, string EmployeeId, string Gender_E = null
            , string Religion = null, string GradeId = null
            , string other1 = "", string other2 = "", string other3 = ""
            , string OrderByCode = "", string EmpJobType = null, string EmpCategory = null
            )
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
                sqlText = @"WITH CTE_UniqueEmployees AS (
    SELECT 
        e.id, 
        e.Code,
        ed.OtherId,
        e.Salutation_E, 
        e.MiddleName, 
        e.LastName,
        e.Department, 
        e.DepartmentId,
        e.Designation,
        e.DesignationId,
        e.JoinDate,
        ed.DateOfBirth,
        -- DATEADD(year, 65, ed.DateOfBirth) AS RetirementDate,
        e.Branch,
        e.Project, 
        e.Id AS ProjectId,
        e.Section, 
        e.Id AS SectionId,
        ty.Name AS EmploymentType_E,
        ty.id AS EmploymentType_EId,
        ed.CorporateContactNo AS Mobile,
        a.Address, 
        a.District,
        a.Division,
        a.Country,
        ed.Gender_E,
        ed.Religion,
        e.Grade,
        e.GradeId,
        e.GradeSL,
        ed.BloodGroup_E,
        ed.Email,
        e.BasicSalary,
        e.GrossSalary,
        e.AttnUserId,
        e.StepName,
        e.PhotoName,
        e.StepSL,
        j.Other1,
        j.Other2,
        j.Other3,
        j.Other4,
        j.Other5,
        j.Supervisor,
        e.EmpName,
        Se.Email AS SupervisorEmail,
        j.DotedLineReport AS DotedLineManager,
        Se1.Email AS DotedLineManagerEmail,
        en.Name AS NomineeName,
        en.Relation AS NRelation,
        en.Mobile AS NMobile,
        en.DateofBirth AS NDateofBirth,
        en.Address AS NAddress,
        en.PostalCode AS NPostalCode,
        en.District AS NDistrict,
        en.Division AS NDivision,
        en.Country AS NCountry,
        edp.Name AS DependentName,
        edp.Relation AS DRelation,
        edp.Mobile AS DMobile,
        edp.DateofBirth AS DDateofBirth,
        edp.Address AS DAddress,
        edp.PostalCode AS DPostalCode,
        edp.District AS DDistrict,
        edp.Division AS DDivision,
        edp.Country AS DCountry,
        ROW_NUMBER() OVER (PARTITION BY e.Code ORDER BY e.Department, e.GradeSL, e.JoinDate) AS RowNum
    FROM 
        ViewEmployeeInformation e
    LEFT OUTER JOIN employeeJob j ON j.EmployeeId = e.Id
    LEFT OUTER JOIN EnumEmploymentType ty ON ty.id = j.EmploymentType_E
    LEFT OUTER JOIN employeePresentAddress a ON e.Id = a.EmployeeId
    LEFT OUTER JOIN EmployeePersonalDetail ed ON ed.EmployeeId = e.Id
    LEFT OUTER JOIN ViewEmployeeInformation Se ON ISNULL(LEFT(j.Supervisor, CHARINDEX('~', j.Supervisor + '~') - 1), '') = Se.Code
    LEFT OUTER JOIN ViewEmployeeInformation Se1 ON ISNULL(LEFT(j.DotedLineReport, CHARINDEX('~', j.DotedLineReport + '~') - 1), '') = Se1.Code
    LEFT OUTER JOIN EmployeeNominee en ON en.EmployeeId = e.EmployeeId
    LEFT OUTER JOIN EmployeeDependent edp ON edp.EmployeeId = e.EmployeeId
    WHERE 
        e.IsArchive = 0 AND 
        e.IsActive = 1";
                #region ConditionFields
                if (CodeF != "0_0")
                {
                    sqlText += "  and e.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and e.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and e.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and e.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and e.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and e.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    sqlText += "  and e.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {
                    sqlText += "  and e.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(BloodGroup))
                {
                    sqlText += "  and ed.BloodGroup_E=@BloodGroup";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                if (!string.IsNullOrWhiteSpace(Gender_E))
                {
                    sqlText += "  and ed.Gender_E=@Gender_E";
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    sqlText += "  and  ed.Religion=@Religion";
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    sqlText += "  and e.GradeId=@GradeId";
                }

                if (!string.IsNullOrWhiteSpace(other1))
                {
                    sqlText += "  and j.Other1=@other1";
                }
                if (!string.IsNullOrWhiteSpace(other2))
                {
                    sqlText += "  and j.Other2=@other2";
                }
                if (!string.IsNullOrWhiteSpace(other3))
                {
                    sqlText += "  and j.Other3=@other3";
                }
                if (!string.IsNullOrWhiteSpace(EmpJobType))
                {
                    sqlText += "  and j.EmpJobType=@EmpJobType";
                }
                if (!string.IsNullOrWhiteSpace(EmpCategory))
                {
                    sqlText += "  and j.EmpCategory=@EmpCategory";
                }
                sqlText += ")";
                #endregion ConditionFields
                if (OrderByCode == "true")
                {
                    sqlText += "  order by e.Code";
                }
                else if (OrderByCode == "DCG")
                    sqlText += " order by e.department, e.code, e.GradeSl";
                else if (OrderByCode == "DDC")
                    sqlText += " order by e.department, e.JoinDate, e.code";
                else if (OrderByCode == "DGC")
                    sqlText += " order by e.department, e.GradeSl, e.code";
                else if (OrderByCode == "DGDC")
                    sqlText += " order by e.department, e.GradeSl, e.JoinDate, e.code";
                else if (OrderByCode == "CODE")
                    sqlText += " order by  e.code";
                else
                {
                    sqlText += " SELECT * FROM CTE_UniqueEmployees WHERE RowNum = 1 ORDER BY Department, GradeSL, JoinDate, Code";
                }



                SqlCommand objComm = new SqlCommand(sqlText, currConn);
                #region ConditionValues
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0" && dtpFrom != "0" && dtpFrom != "" && dtpFrom != "null" && dtpFrom != null)
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0" && dtpTo != "0" && dtpTo != "" && dtpTo != "null" && dtpTo != null)
                {

                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(BloodGroup))
                {
                    objComm.Parameters.AddWithValue("@BloodGroup", BloodGroup);
                }
                if (!string.IsNullOrWhiteSpace(Gender_E))
                {
                    objComm.Parameters.AddWithValue("@Gender_E", Gender_E);
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    objComm.Parameters.AddWithValue("@Religion", Religion);
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    objComm.Parameters.AddWithValue("@GradeId", GradeId);
                }

                if (!string.IsNullOrWhiteSpace(other1))
                {
                    objComm.Parameters.AddWithValue("@other1", other1);
                }
                if (!string.IsNullOrWhiteSpace(other2))
                {
                    objComm.Parameters.AddWithValue("@other2", other2);
                }
                if (!string.IsNullOrWhiteSpace(other3))
                {
                    objComm.Parameters.AddWithValue("@other3", other3);
                }
                if (!string.IsNullOrWhiteSpace(EmpJobType))
                {
                    objComm.Parameters.AddWithValue("@EmpJobType", EmpJobType);
                }
                if (!string.IsNullOrWhiteSpace(EmpCategory))
                {
                    objComm.Parameters.AddWithValue("@EmpCategory", EmpCategory);
                }

                #endregion ConditionValues
                #region Reading Data
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Salutation_E = dr["Salutation_E"].ToString();
                    vm.MiddleName = dr["MiddleName"].ToString();
                    vm.LastName = dr["LastName"].ToString();
                    vm.FullName = vm.Salutation_E + " " + vm.MiddleName + " " + vm.LastName;
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Branch = dr["Branch"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.EmploymentType_E = dr["EmploymentType_E"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Gender = dr["Gender_E"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["joinDate"].ToString());
                    vm.Address = dr["Address"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.BloodGroup = dr["BloodGroup_E"].ToString();
                    vm.Email = dr["Email"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.Religion = dr["Religion"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());
                    vm.AttnUserId = dr["AttnUserId"].ToString();
                    vm.StepName = dr["StepName"].ToString();
                    vm.StepSL = dr["StepSL"].ToString();
                    vm.PhotoName = dr["PhotoName"].ToString();
                    vm.OtherId = dr["OtherId"].ToString();
                    vm.Supervisor = dr["Supervisor"].ToString();
                    vm.SupervisorEmail = dr["SupervisorEmail"].ToString();
                    vm.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());
                    vm.ServiceLength = Ordinary.DateDifferenceYMD(dr["JoinDate"].ToString(), DateTime.Now.ToString("yyyyMMdd"), true);
                    // vm.RetirementDate = Ordinary.StringToDate(dr["RetirementDate"].ToString());
                    vm.DotedLineManager = dr["DotedLineManager"].ToString();
                    vm.DotedLineManagerEmail = dr["DotedLineManagerEmail"].ToString();
                    vm.Other1 = dr["Other1"].ToString();
                    vm.Other2 = dr["Other2"].ToString();
                    vm.Other3 = dr["Other3"].ToString();
                    vm.Other4 = dr["Other4"].ToString();
                    vm.Other5 = dr["Other5"].ToString();

                    vm.NomineeName = dr["NomineeName"].ToString();
                    vm.NRelation = dr["NRelation"].ToString();
                    vm.NMobile = dr["NMobile"].ToString();
                    vm.NDateofBirth = dr["NDateofBirth"].ToString();
                    vm.NAddress = dr["NAddress"].ToString();
                    vm.NPostalCode = dr["NPostalCode"].ToString();
                    vm.NDistrict = dr["NDistrict"].ToString();
                    vm.NDivision = dr["NDivision"].ToString();
                    vm.NCountry = dr["NCountry"].ToString();

                    vm.DependentName = dr["DependentName"].ToString();
                    vm.DRelation = dr["DRelation"].ToString();
                    vm.DMobile = dr["DMobile"].ToString();
                    vm.DDateofBirth = dr["DDateofBirth"].ToString();
                    vm.DAddress = dr["DAddress"].ToString();
                    vm.DPostalCode = dr["DPostalCode"].ToString();
                    vm.DDistrict = dr["DDistrict"].ToString();
                    vm.DDivision = dr["DDivision"].ToString();
                    vm.DCountry = dr["DCountry"].ToString();


                    VMs.Add(vm);
                }
                dr.Close();
                //Other1
                //Other2
                //Other3
                //Other4
                //Other5
                #endregion Reading Data

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

        public List<ViewEmployeeInfoAllVM> EmployeeInformationAll(string CodeF, string CodeT, string DepartmentId, string SectionId
            , string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string Gender = null, string Religion = null
            , string GradeId = null, string OrderByCode = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<ViewEmployeeInfoAllVM> VMs = new List<ViewEmployeeInfoAllVM>();
            ViewEmployeeInfoAllVM vm;
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
                #region sqlText
                sqlText = @"
Select 
--EmployeeInfo
ei.EmployeeId, ei.Code, ei.Salutation, ei.MiddleName, ei.LastName, ei.EmpName, ei.AttnUserId
--EmployeeJob
, ei.JoinDate
, ei.ProbationEnd
, ei.DateOfPermanent
, ei.LeftDate
, ei.GrossSalary
, ei.BasicSalary
, ei.IsPermanent
, ei.Supervisor
, ei.BankInfo
, ei.BankAccountNo
, ei.EmploymentStatus
, ei.EmploymentType
, ei.Branch         , ei.BranchId
, ei.Department     , ei.DepartmentId
, ei.Section        , ei.SectionId
, ei.Project        , ei.ProjectId
, ei.Designation    , ei.DesignationId
, ei.Grade          , ei.GradeId 
--Transfer, Promotion
, ei.TransferDate
, ei.IsPromotion
, ei.PromotionDate
--EmployeePersonalDetail
, ei.PersonalEmail
, ei.NickName	
, ei.DateOfBirth
, ei.Religion
, ei.BloodGroup
, ei.Gender			
, ei.MaritalStatus
, ei.Nationality
, ei.Smoker
, ei.NID
, ei.PassportNumber
, ei.ExpiryDate
, ei.TIN
, ei.IsDisable
, ei.KindsOfDisability
, ei.OtherId
--EmployeePresentAddress
, ei.PresentAddress
, ei.PresentMobile
, ei.PresentDistrict
, ei.PresentDivision
, ei.PresentCountry
, ei.PresentCity
, ei.PresentPostalCode
, ei.PresentPhone
, ei.PresentFax
--EmployeePermanentAddress
, ei.PermanentAddress
, ei.PermanentMobile  
, ei.PermanentDistrict
, ei.PermanentDivision
, ei.PermanentCountry
, ei.PermanentCity
, ei.PermanentPostalCode
, ei.PermanentPhone
, ei.PermanentFax
--EmployeeEmergencyContact
, ei.EmConName
, ei.EmConRelation
, ei.EmConAddress
, ei.EmConDistrict
, ei.EmConDivision
, ei.EmConCountry
, ei.EmConCity
, ei.EmConPostalCode
, ei.EmConPhone
, ei.EmConMobile
, ei.EmConFax
, ei.LeftType
--EmployeeEduction
, ei.EducationDegree
, ei.EducationInstitute
, ei.EducationMajor
, ei.EducationYearOfPassing
, ei.EducationCGPA
, ei.EducationScale
, ei.EducationResult
, ei.EducationMarks
, ei.EducationTotalYear
from ViewEmployeeInformationAll ei
where ei.IsArchive=0 and ei.IsActive=1
";
                #endregion sqlText
                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(Gender))
                {
                    sqlText += "  and ei.Gender=@Gender";
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    sqlText += "  and  ei.Religion=@Religion";
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    sqlText += "  and ei.GradeId=@GradeId";
                }
                if (OrderByCode == "true")
                {
                    sqlText += "  order by ei.Code";
                }
                else
                {
                    sqlText += "  order by ei.Department, ei.GradeSL, ei.JoinDate, ei.Code";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(Gender))
                {
                    objComm.Parameters.AddWithValue("@Gender", Gender);
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    objComm.Parameters.AddWithValue("@Religion", Religion);
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    objComm.Parameters.AddWithValue("@GradeId", GradeId);
                }
                #endregion Parrameters Apply
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                #region Data Reading
                while (dr.Read())
                {
                    vm = new ViewEmployeeInfoAllVM();
                    //EmployeeInfo
                    vm.Code = dr["Code"].ToString();                                           // 0
                    vm.EmpName = dr["EmpName"].ToString();                                           // 1 
                    vm.AttnUserId = dr["AttnUserId"].ToString();//EmployeeJob                              // 2 
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());                                          // 3 
                    vm.ProbationEnd = Ordinary.StringToDate(dr["ProbationEnd"].ToString());                                          // 4 
                    vm.DateOfPermanent = Ordinary.StringToDate(dr["DateOfPermanent"].ToString());                                         // 5 
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());                                          // 6     
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());                                          // 7 
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"].ToString());                                          // 8 
                    vm.IsPermanent = Convert.ToBoolean(dr["IsPermanent"].ToString());                                          // 9 
                    vm.Supervisor = dr["Supervisor"].ToString();                                           // 10
                    vm.BankInfo = dr["BankInfo"].ToString();                                             // 11
                    vm.BankAccountNo = dr["BankAccountNo"].ToString();                                             // 12
                    vm.EmploymentStatus = dr["EmploymentStatus"].ToString();                                             // 13
                    vm.EmploymentType = dr["EmploymentType"].ToString();                                             // 14
                    vm.Branch = dr["Branch"].ToString();                                             // 15
                    vm.Department = dr["Department"].ToString();                                             // 16
                    vm.Section = dr["Section"].ToString();                                             // 17
                    vm.Project = dr["Project"].ToString();                                             // 18
                    vm.Designation = dr["Designation"].ToString();                                             // 19
                    vm.Grade = dr["Grade"].ToString();                                             // 20
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());                                            // 21  
                    vm.DepartmentId = dr["DepartmentId"].ToString();                                             // 22
                    vm.SectionId = dr["SectionId"].ToString();                                             // 23
                    vm.ProjectId = dr["ProjectId"].ToString();                                             // 24
                    vm.DesignationId = dr["DesignationId"].ToString();                                             // 25
                    vm.GradeId = dr["GradeId"].ToString();//Transfer, Promotion                        // 26
                    vm.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());                                             // 27
                    vm.IsPromotion = Convert.ToBoolean(dr["IsPromotion"].ToString());                                              // 28
                    vm.PromotionDate = Ordinary.StringToDate(dr["PromotionDate"].ToString());//EmployeePersonalDetail                     // 29
                    vm.PersonalEmail = dr["PersonalEmail"].ToString();                                             // 30
                    vm.NickName = dr["NickName"].ToString();                                             // 31  
                    vm.DateOfBirth = Ordinary.StringToDate(dr["DateOfBirth"].ToString());                                             // 32
                    vm.Religion = dr["Religion"].ToString();                                             // 33
                    vm.BloodGroup = dr["BloodGroup"].ToString();                                             // 34
                    vm.Gender = dr["Gender"].ToString();                                             // 35
                    vm.MaritalStatus = dr["MaritalStatus"].ToString();                                             // 36
                    vm.Nationality = dr["Nationality"].ToString();                                             // 37
                    vm.Smoker = Convert.ToBoolean(dr["Smoker"].ToString());                                            // 38
                    vm.NID = dr["NID"].ToString();                                             // 39
                    vm.PassportNumber = dr["PassportNumber"].ToString();                                             // 40
                    vm.ExpiryDate = Ordinary.StringToDate(dr["ExpiryDate"].ToString());                                             // 41
                    vm.TIN = dr["TIN"].ToString();                                             // 42
                    vm.IsDisable = Convert.ToBoolean(dr["IsDisable"].ToString());                                            // 43
                    vm.KindsOfDisability = dr["KindsOfDisability"].ToString();                                             // 44
                    vm.OtherId = dr["OtherId"].ToString();//EmployeePresentAddress                     // 45
                    vm.PresentAddress = dr["PresentAddress"].ToString();                                             // 46
                    vm.PresentMobile = dr["PresentMobile"].ToString();                                             // 47
                    vm.PresentDistrict = dr["PresentDistrict"].ToString();                                             // 48
                    vm.PresentDivision = dr["PresentDivision"].ToString();                                             // 49
                    vm.PresentCountry = dr["PresentCountry"].ToString();                                             // 50
                    vm.PresentCity = dr["PresentCity"].ToString();                                             // 51
                    vm.PresentPostalCode = dr["PresentPostalCode"].ToString();                                             // 52
                    vm.PresentPhone = dr["PresentPhone"].ToString();                                             // 53
                    vm.PresentFax = dr["PresentFax"].ToString();//--EmployeePermanentAddress                 // 54
                    vm.PermanentAddress = dr["PermanentAddress"].ToString();                                             // 55
                    vm.PermanentMobile = dr["PermanentMobile"].ToString();                                             // 56
                    vm.PermanentDistrict = dr["PermanentDistrict"].ToString();                                             // 57
                    vm.PermanentDivision = dr["PermanentDivision"].ToString();                                             // 58
                    vm.PermanentCountry = dr["PermanentCountry"].ToString();                                             // 59
                    vm.PermanentCity = dr["PermanentCity"].ToString();                                             // 60
                    vm.PermanentPostalCode = dr["PermanentPostalCode"].ToString();                                             // 61
                    vm.PermanentPhone = dr["PermanentPhone"].ToString();                                             // 62
                    vm.PermanentFax = dr["PermanentFax"].ToString();//--EmployeeEmergencyContact                 // 63
                    vm.EmConName = dr["EmConName"].ToString();                                             // 64
                    vm.EmConRelation = dr["EmConRelation"].ToString();                                             // 65
                    vm.EmConAddress = dr["EmConAddress"].ToString();                                             // 66
                    vm.EmConDistrict = dr["EmConDistrict"].ToString();                                             // 67
                    vm.EmConDivision = dr["EmConDivision"].ToString();                                             // 68
                    vm.EmConCountry = dr["EmConCountry"].ToString();
                    vm.EmConCity = dr["EmConCity"].ToString();
                    vm.EmConPostalCode = dr["EmConPostalCode"].ToString();
                    vm.EmConPhone = dr["EmConPhone"].ToString();
                    vm.EmConMobile = dr["EmConMobile"].ToString();
                    vm.EmConFax = dr["EmConFax"].ToString();
                    vm.LeftType = dr["LeftType"].ToString();
                    vm.EducationDegree = dr["EducationDegree"].ToString();//--EmployeeEducation 
                    vm.EducationInstitute = dr["EducationInstitute"].ToString();
                    vm.EducationMajor = dr["EducationMajor"].ToString();
                    vm.EducationYearOfPassing = dr["EducationYearOfPassing"].ToString();
                    vm.EducationCGPA = Convert.ToDecimal(dr["EducationCGPA"].ToString());
                    vm.EducationScale = Convert.ToDecimal(dr["EducationScale"].ToString());
                    vm.EducationResult = dr["EducationResult"].ToString();
                    vm.EducationMarks = Convert.ToDecimal(dr["EducationMarks"].ToString());
                    vm.EducationTotalYear = Convert.ToDecimal(dr["EducationTotalYear"].ToString());
                    //vm.Religion = dr["Religion"].ToString();
                    //vm.Grade = dr["Grade"].ToString();
                    //vm.GradeId = dr["GradeId"].ToString();
                    VMs.Add(vm);
                }
                dr.Close();
                #endregion Data Reading
                #endregion sql statement
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

        public DataTable ExportExcelEmpInforAll(string Filepath, string FileName, string CodeF, string CodeT, string DepartmentId
            , string SectionId, string ProjectId
           , string DesignationId, string dtpFrom, string dtpTo, string Gender, string Religion, string GradeId, string OrderByCode = "")
        {
            #region Variables
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlConnection currConn = null;
            string sqlText = "";
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
                #region sqlText
                sqlText = @"
Select 
--EmployeeInfo
ei.EmployeeId, ei.Code, ei.EmpName, ei.AttnUserId
--EmployeeJob
, ei.JoinDate [Join Date]
, ei.ProbationEnd [Probation End]
, ei.DateOfPermanent [Date Of Permanent]
--, convert(varchar(30),cast(ei.JoinDate as datetime),103) [Join Date]
--, convert(varchar(30),cast(ei.ProbationEnd as datetime),103) [Probation End]
--, convert(varchar(30),cast(ei.DateOfPermanent as datetime),103) [Date Of Permanent]
, ei.IsPermanent
, ei.Supervisor
, ei.BankInfo
, ei.BankAccountNo
, ei.EmploymentStatus
, ei.EmploymentType
, ei.Branch         
, ei.Department     
, ei.Section        
, ei.Project        
, ei.Designation    
, ei.Grade          
--Transfer, Promotion
, ei.BasicSalary
, ei.TransferDate [Transfer Date]
, ei.IsPromotion
, ei.JoinDate [Promotion Date]
--EmployeePersonalDetail
, ei.PersonalEmail
, ei.NickName	
, ei.DateOfBirth
, ei.Religion
, ei.BloodGroup
, ei.Gender			
, ei.MaritalStatus
, ei.Nationality
, ei.Smoker
, ei.NID
, ei.PassportNumber
, ei.ExpiryDate [Expiry Date]
, ei.TIN
, ei.IsDisable
, ei.KindsOfDisability
, ei.OtherId
--EmployeePresentAddress
, ei.PresentAddress
, ei.PresentMobile
, ei.PresentDistrict
, ei.PresentDivision
, ei.PresentCountry
, ei.PresentCity
, ei.PresentPostalCode
, ei.PresentPhone
, ei.PresentFax
--EmployeePermanentAddress
, ei.PermanentAddress
, ei.PermanentMobile  
, ei.PermanentDistrict
, ei.PermanentDivision
, ei.PermanentCountry
, ei.PermanentCity
, ei.PermanentPostalCode
, ei.PermanentPhone
, ei.PermanentFax
--EmployeeEmergencyContact
, ei.EmConName
, ei.EmConRelation
, ei.EmConAddress
, ei.EmConDistrict
, ei.EmConDivision
, ei.EmConCountry
, ei.EmConCity
, ei.EmConPostalCode
, ei.EmConPhone
, ei.EmConMobile
, ei.EmConFax
, ei.LeftType
--EmployeeEduction
, ei.EducationDegree
, ei.EducationInstitute
, ei.EducationMajor
, ei.EducationYearOfPassing
, ei.EducationCGPA
, ei.EducationScale
, ei.EducationResult
, ei.EducationMarks
, ei.EducationTotalYear
from ViewEmployeeInformationAll ei
where ei.IsArchive=0 and ei.IsActive=1
";
                #endregion sqlText
                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.ProjectId=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(Gender))
                {
                    sqlText += "  and ei.Gender=@Gender";
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    sqlText += "  and  ei.Religion=@Religion";
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    sqlText += "  and ei.GradeId=@GradeId";
                }
                if (OrderByCode == "true")
                {
                    sqlText += "  order by ei.Code";
                }
                else
                {
                    sqlText += "  order by ei.Department, ei.GradeSL, ei.JoinDate, ei.Code";
                }
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(Gender))
                {
                    objComm.Parameters.AddWithValue("@Gender", Gender);
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    objComm.Parameters.AddWithValue("@Religion", Religion);
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    objComm.Parameters.AddWithValue("@GradeId", GradeId);
                }
                #endregion Parrameters Apply
                #region Data Reading
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.Fill(dt);

                //Join Date //Probation End //Date Of Permanent //Transfer Date //Promotion Date //Expiry Date
                string[] columnsToChange = { "Join Date", "Probation End", "Date Of Permanent", "Transfer Date", "Promotion Date", "Expiry Date" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, columnsToChange);
                #endregion Data Reading
                #endregion sql statement
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
            return dt;
        }

        public List<EmployeeInfoVM> ExEmployeeList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> gmployeeInfoVMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM gmployeeInfoVM;
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
select e.id, e.Code, e.Salutation_E, e.MiddleName, e.LastName 
,dp.Name Department,dp.id DepartmentId
,dg.Name Designation,dg.id DesignationId
,j.joinDate
,eli.LeftDate
,eli.EntryLeftDate
,b.Name Branch
,p.Name Project ,p.Id ProjectId
,s.Name Section,s.Id SectionId
,ty.Name EmploymentType_E,ty.id EmploymentType_EId
,a.Mobile
,a.Address ,a.District,a.Division,a.Country
,ed.Gender_E,ed.BloodGroup_E,ed.Email
from EmployeeInfo e
left outer join EmployeeTransfer et on et.employeeId=e.Id and et.iscurrent=1
left outer join Project p on p.id=et.projectId
left outer join Section s on s.id=et.SectionId
left outer join Department dp on dp.Id=et.DepartmentId
left outer join EmployeePromotion ep on ep.employeeId=e.Id and ep.iscurrent=1
left outer join Designation dg on dg.Id=ep.DesignationId
left outer join Branch b on b.Id=e.BranchId
left outer join employeeJob j on j.EmployeeId=e.Id
left outer join EnumEmploymentType ty on ty.id=j.EmploymentType_E
left outer join employeePresentAddress a on e.Id=a.EmployeeId
left outer join EmployeePersonalDetail ed on ed.EmployeeId=e.Id
Inner join EmployeeLeftInformation eli on eli.EmployeeId=e.Id
";
                if (CodeF != "0_0") sqlText += "  and e.Code>=@CodeF";
                if (CodeT != "0_0") sqlText += "  and e.Code<=@CodeT";
                if (DepartmentId != "0_0") sqlText += "  and dp.id=@DepartmentId";
                if (DesignationId != "0_0") sqlText += "  and dg.id=@DesignationId";
                if (ProjectId != "0_0") sqlText += "  and  p.id=@ProjectId";
                if (SectionId != "0_0") sqlText += "  and  s.id=@SectionId";
                if (dtpFrom != "0_0") sqlText += "  and j.JoinDate>=@dtpFrom";
                if (dtpTo != "0_0") sqlText += "  and j.JoinDate<=@dtpTo";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                sqlText += "  order by dp.name, s.name,  e.code ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0") { objComm.Parameters.AddWithValue("@CodeF", CodeF); }
                if (CodeT != "0_0") { objComm.Parameters.AddWithValue("@CodeT", CodeT); }
                if (DepartmentId != "0_0") { objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId); }
                if (SectionId != "0_0") { objComm.Parameters.AddWithValue("@SectionId", SectionId); }
                if (ProjectId != "0_0") { objComm.Parameters.AddWithValue("@ProjectId", ProjectId); }
                if (DesignationId != "0_0") { objComm.Parameters.AddWithValue("@DesignationId", DesignationId); }
                if (dtpFrom != "0_0") { objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom)); }
                if (dtpTo != "0_0") { objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo)); }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.Id = dr["id"].ToString();
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Salutation_E = dr["Salutation_E"].ToString();
                    gmployeeInfoVM.MiddleName = dr["MiddleName"].ToString();
                    gmployeeInfoVM.LastName = dr["LastName"].ToString();
                    gmployeeInfoVM.FullName = gmployeeInfoVM.Salutation_E + " " + gmployeeInfoVM.MiddleName + " " + gmployeeInfoVM.LastName;
                    gmployeeInfoVM.Branch = dr["Branch"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmploymentType_E = dr["EmploymentType_E"].ToString();
                    gmployeeInfoVM.Mobile = dr["Mobile"].ToString();
                    gmployeeInfoVM.Gender = dr["Gender_E"].ToString();
                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["joinDate"].ToString());
                    gmployeeInfoVM.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
                    gmployeeInfoVM.EntryLeftDate = Ordinary.StringToDate(dr["EntryLeftDate"].ToString());
                    gmployeeInfoVM.Address = dr["Address"].ToString();
                    gmployeeInfoVM.Country = dr["Country"].ToString();
                    gmployeeInfoVM.Division = dr["Division"].ToString();
                    gmployeeInfoVM.District = dr["District"].ToString();
                    gmployeeInfoVM.BloodGroup = dr["BloodGroup_E"].ToString();
                    gmployeeInfoVM.Email = dr["Email"].ToString();
                    gmployeeInfoVMs.Add(gmployeeInfoVM);
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
            return gmployeeInfoVMs;
        }

        public List<EmployeeInfoVM> UnConfirmedList(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> gmployeeInfoVMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM gmployeeInfoVM;
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
select e.id, e.Code, e.Salutation_E, e.MiddleName, e.LastName 
,dp.Name Department,dp.id DepartmentId
,dg.Name Designation,dg.id DesignationId
,j.joinDate
,b.Name Branch
,p.Name Project ,p.Id ProjectId
,s.Name Section,s.Id SectionId
,ty.Name EmploymentType_E,ty.id EmploymentType_EId
,a.Mobile
,a.Address ,a.District,a.Division,a.Country
,ed.Gender_E,ed.BloodGroup_E,ed.Email
from EmployeeInfo e
left outer join EmployeeTransfer et on et.employeeId=e.Id and et.iscurrent=1
left outer join Project p on p.id=et.projectId
left outer join Section s on s.id=et.SectionId
left outer join Department dp on dp.Id=et.DepartmentId
left outer join EmployeePromotion ep on ep.employeeId=e.Id and ep.iscurrent=1
left outer join Designation dg on dg.Id=ep.DesignationId
left outer join Branch b on b.Id=e.BranchId
left outer join employeeJob j on j.EmployeeId=e.Id
left outer join EnumEmploymentType ty on ty.id=j.EmploymentType_E
left outer join employeePresentAddress a on e.Id=a.EmployeeId
left outer join EmployeePersonalDetail ed on ed.EmployeeId=e.Id
where j.IsPermanent=0
";
                if (CodeF != "0_0") sqlText += "  and e.Code>=@CodeF";
                if (CodeT != "0_0") sqlText += "  and e.Code<=@CodeT";
                if (DepartmentId != "0_0") sqlText += "  and dp.id=@DepartmentId";
                if (DesignationId != "0_0") sqlText += "  and dg.id=@DesignationId";
                if (ProjectId != "0_0") sqlText += "  and  p.id=@ProjectId";
                if (SectionId != "0_0") sqlText += "  and  s.id=@SectionId";
                if (dtpFrom != "0_0") sqlText += "  and j.JoinDate>=@dtpFrom";
                if (dtpTo != "0_0") sqlText += "  and j.JoinDate<=@dtpTo";
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                sqlText += "  order by dp.name, s.name,  e.code ";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0") { objComm.Parameters.AddWithValue("@CodeF", CodeF); }
                if (CodeT != "0_0") { objComm.Parameters.AddWithValue("@CodeT", CodeT); }
                if (DepartmentId != "0_0") { objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId); }
                if (SectionId != "0_0") { objComm.Parameters.AddWithValue("@SectionId", SectionId); }
                if (ProjectId != "0_0") { objComm.Parameters.AddWithValue("@ProjectId", ProjectId); }
                if (DesignationId != "0_0") { objComm.Parameters.AddWithValue("@DesignationId", DesignationId); }
                if (dtpFrom != "0_0") { objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom)); }
                if (dtpTo != "0_0") { objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo)); }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    gmployeeInfoVM = new EmployeeInfoVM();
                    gmployeeInfoVM.Id = dr["id"].ToString();
                    gmployeeInfoVM.Code = dr["Code"].ToString();
                    gmployeeInfoVM.Salutation_E = dr["Salutation_E"].ToString();
                    gmployeeInfoVM.MiddleName = dr["MiddleName"].ToString();
                    gmployeeInfoVM.LastName = dr["LastName"].ToString();
                    gmployeeInfoVM.FullName = gmployeeInfoVM.Salutation_E + " " + gmployeeInfoVM.MiddleName + " " + gmployeeInfoVM.LastName;
                    gmployeeInfoVM.Branch = dr["Branch"].ToString();
                    gmployeeInfoVM.Department = dr["Department"].ToString();
                    gmployeeInfoVM.Designation = dr["Designation"].ToString();
                    gmployeeInfoVM.Project = dr["Project"].ToString();
                    gmployeeInfoVM.Section = dr["Section"].ToString();
                    gmployeeInfoVM.EmploymentType_E = dr["EmploymentType_E"].ToString();
                    gmployeeInfoVM.Mobile = dr["Mobile"].ToString();
                    gmployeeInfoVM.Gender = dr["Gender_E"].ToString();
                    gmployeeInfoVM.JoinDate = Ordinary.StringToDate(dr["joinDate"].ToString());
                    gmployeeInfoVM.Address = dr["Address"].ToString();
                    gmployeeInfoVM.Country = dr["Country"].ToString();
                    gmployeeInfoVM.Division = dr["Division"].ToString();
                    gmployeeInfoVM.District = dr["District"].ToString();
                    gmployeeInfoVM.BloodGroup = dr["BloodGroup_E"].ToString();
                    gmployeeInfoVM.Email = dr["Email"].ToString();
                    gmployeeInfoVMs.Add(gmployeeInfoVM);
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
            return gmployeeInfoVMs;
        }

        public List<EmployeeInfoVM> EmpServiceLength(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId, string Gender_E, string Religion, string GradeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
select e.Code, e.EmpName 
,e.Department
,e.Designation
,e.Project 
,e.Section 
,e.JoinDate
,d.DateOfBirth
,d.Gender_E
,d.Religion
,e.Grade
,e.GradeId
,e.GradeSL
from ViewEmployeeInformation e
left outer join EmployeePromotion ep on ep.employeeId=e.Id and ep.iscurrent=1
left outer join EmployeePersonalDetail d on d.employeeId=e.Id
where e.IsArchive=0 and e.IsActive=1
";
                if (CodeF != "0_0")
                {
                    sqlText += "  and e.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and e.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and e.ProjectId=@ProjectId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and e.DepartmentId=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and e.DesignationId=@DesignationId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and e.SectionId=@SectionId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and e.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and e.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                if (!string.IsNullOrWhiteSpace(Gender_E))
                {
                    sqlText += "  and d.Gender_E=@Gender_E";
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    sqlText += "  and  d.Religion=@Religion";
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    sqlText += "  and e.GradeId=@GradeId";
                }
                sqlText += "  order by e.Department, e.GradeSL, e.JoinDate, e.Code";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(Gender_E))
                {
                    objComm.Parameters.AddWithValue("@Gender_E", Gender_E);
                }
                if (!string.IsNullOrWhiteSpace(Religion))
                {
                    objComm.Parameters.AddWithValue("@Religion", Religion);
                }
                if (!string.IsNullOrWhiteSpace(GradeId))
                {
                    objComm.Parameters.AddWithValue("@GradeId", GradeId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Gender_E = dr["Gender_E"].ToString();
                    vm.Religion = dr["Religion"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.GradeId = dr["GradeId"].ToString();
                    vm.Age = Ordinary.DateDifferenceYMD(dr["DateOfBirth"].ToString(), DateTime.Now.ToString("yyyyMMdd"), true);
                    vm.ServiceLength = Ordinary.DateDifferenceYMD(dr["JoinDate"].ToString(), DateTime.Now.ToString("yyyyMMdd"), true);
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

        public List<EmployeeInfoVM> EmpTransfer(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtjFrom, string dtjTo, string EmployeeId, string dttFrom, string dttTo)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
select e.id,e.Code, e.Salutation_E, e.MiddleName, e.LastName 
,dp.Name Department
,dg.Name Designation
,s.Name Section
,p.Name Project
,t.transferDate
, t.isCurrent
,ej.joindate
,g.sl
from EmployeeInfo e
left outer join EmployeeTransfer t on e.Id=t.employeeId and t.IsArchive=0
left outer join Project p on p.id=t.projectId
left outer join Section s on s.id=t.SectionId
left outer join Department dp on dp.Id=t.DepartmentId
left outer join EmployeePromotion ep on ep.employeeId=e.Id and ep.IsArchive=0 and ep.IsCurrent=1
left outer join Designation dg on dg.Id=ep.DesignationId
left outer join EmployeeJob ej  on ej.EmployeeId=e.Id
left outer join Grade g  on ep.gradeId=g.Id
where e.IsArchive=0 and e.IsActive=1
";
                #region Parrameters Apply
                if (ProjectId != "0_0")
                {
                    sqlText += "  and p.id=@ProjectId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and dp.id=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and dg.id=@DesignationId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and s.id=@SectionId";
                }
                if (CodeF != "0_0")
                {
                    sqlText += "  and e.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and e.Code<=@CodeT";
                }
                if (dtjFrom != "0_0")
                {
                    sqlText += "  and ej.joindate>=@dtjFrom";
                }
                if (dtjTo != "0_0")
                {
                    sqlText += "  and ej.joindate<=@dtjTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                if (dttFrom != "0_0")
                {
                    sqlText += "  and t.transferDate>=@dttFrom";
                }
                if (dttTo != "0_0")
                {
                    sqlText += "  and t.transferDate<=@dttTo";
                }
                sqlText += " order by dp.Name, ej.JoinDate, e.code, t.transferDate desc, t.Id desc";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtjFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttFrom", Ordinary.DateToString(dtjFrom));
                }
                if (dtjTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttTo", Ordinary.DateToString(dtjTo));
                }
                if (dttFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttFrom", Ordinary.DateToString(dttFrom));
                }
                if (dttTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttTo", Ordinary.DateToString(dttTo));
                }
                #endregion Parrameters Apply
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Salutation_E = dr["Salutation_E"].ToString();
                    vm.MiddleName = dr["MiddleName"].ToString();
                    vm.LastName = dr["LastName"].ToString();
                    vm.FullName = vm.Salutation_E + " " + vm.MiddleName + " " + vm.LastName;
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Date = Ordinary.StringToDate(dr["transferDate"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
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

        public List<EmployeeInfoVM> EmpTraining(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtpFrom, string dtpTo, string topics, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
E.ID,E.CODE,E.SALUTATION_E, E.MIDDLENAME,E.LASTNAME
,D.NAME DEPARTMENT
,sec.Name Section
,pro.name Project
,DG.NAME DESIGNATION
,T.TOPICS,T.INSTITUTENAME,T.LOCATION,T.DATEFROM, T.DATETO,T.DURATIONDAY 
FROM EMPLOYEETRAINING T
LEFT OUTER JOIN EMPLOYEEINFO E ON E.ID=T.EMPLOYEEID
LEFT OUTER JOIN EMPLOYEETRANSFER TR ON TR.EMPLOYEEID=E.ID AND TR.ISCURRENT=1
LEFT OUTER JOIN DEPARTMENT D ON D.ID=TR.DEPARTMENTID
LEFT OUTER JOIN EMPLOYEEPROMOTION P ON P.EMPLOYEEID=E.ID AND P.ISCURRENT=1
LEFT OUTER JOIN DESIGNATION DG ON DG.ID=P.DESIGNATIONID
LEFT OUTER JOIN Project PRO ON PRO.ID=TR.ProjectId
LEFT OUTER JOIN Section Sec ON Sec.ID=TR.SectionId
where E.IsArchive=0 and E.IsActive=1
";
                if (ProjectId != "0_0")
                {
                    sqlText += "  and PRO.ID=@ProjectId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and D.ID=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and dg.id=@DesignationId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and s.id=@SectionId";
                }
                if (CodeF != "0_0")
                {
                    sqlText += "  and E.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and E.Code<=@CodeT";
                }
                if (!string.IsNullOrWhiteSpace(topics))
                {
                    sqlText += "  and T.TOPICS like @topics";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and T.DATEFROM>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and T.DATETO<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                sqlText += " ORDER BY E.CODE,T.DATEFROM DESC,T.ID DESC";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (!string.IsNullOrWhiteSpace(topics))
                {
                    objComm.Parameters.AddWithValue("@topics", "%" + topics + "%");
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Salutation_E = dr["Salutation_E"].ToString();
                    vm.MiddleName = dr["MiddleName"].ToString();
                    vm.LastName = dr["LastName"].ToString();
                    vm.FullName = vm.Salutation_E + " " + vm.MiddleName + " " + vm.LastName;
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Topics = dr["TOPICS"].ToString();
                    vm.Institute = dr["INSTITUTENAME"].ToString();
                    vm.Location = dr["LOCATION"].ToString();
                    vm.DateFrom = Ordinary.StringToDate(dr["DATEFROM"].ToString());
                    vm.DateTo = Ordinary.StringToDate(dr["DATETO"].ToString());
                    vm.Duration = Convert.ToDecimal(dr["DURATIONDAY"]);
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

        public List<EmployeeInfoVM> EmpPromotion(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
            , string DesignationId, string dtjFrom, string dtjTo, string EmployeeId, string dtpFrom, string dtpTo, string PL = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            List<EmployeeInfoVM> reportVMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
select e.id,e.Code, e.Salutation_E, e.MiddleName, e.LastName 
,dp.Name Department,s.Name Section, p.name Project,dg.Name Designation
,isnull(g.Name, N'NA') Grade
,ep.PromotionDate
, ep.isCurrent ,ej.JoinDate
from EmployeeInfo e
left outer join EmployeeTransfer et on e.Id=et.employeeId and et.IsArchive=0 and et.isCurrent=1
left outer join Department dp on dp.Id=et.DepartmentId
left outer join Project p on p.id=et.projectId
left outer join Section s on s.id=et.SectionId
 left outer join EmployeePromotion ep on ep.employeeId=e.Id and ep.IsArchive=0
left outer join Designation dg on dg.Id=ep.DesignationId
left outer join Grade g  on g.id=ep.GradeId
left outer join EmployeeJob ej  on ej.EmployeeId=e.Id
where e.IsArchive=0 and e.IsActive=1
";
                #region Parrameters Apply
                if (ProjectId != "0_0")
                {
                    sqlText += "  and p.id=@ProjectId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and dp.id=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and dg.id=@DesignationId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and s.id=@SectionId";
                }
                if (CodeF != "0_0")
                {
                    sqlText += "  and e.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and e.Code<=@CodeT";
                }
                if (dtjFrom != "0_0")
                {
                    sqlText += "  and ej.joindate>=@dtjFrom";
                }
                if (dtjTo != "0_0")
                {
                    sqlText += "  and ej.joindate<=@dtjTo";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ep.PromotionDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ep.PromotionDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                sqlText += " order by dp.name, ej.JoinDate, e.Code, ep.PromotionDate desc, ep.Id desc";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (dtjFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtjFrom", Ordinary.DateToString(dtjFrom));
                }
                if (dtjTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtjTo", Ordinary.DateToString(dtjTo));
                }
                if (dtpFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                #endregion Parrameters Apply
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Salutation_E = dr["Salutation_E"].ToString();
                    vm.MiddleName = dr["MiddleName"].ToString();
                    vm.LastName = dr["LastName"].ToString();
                    vm.FullName = vm.Salutation_E + " " + vm.MiddleName + " " + vm.LastName;
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Grade = dr["Grade"].ToString();
                    vm.Date = Ordinary.StringToDate(dr["PromotionDate"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    VMs.Add(vm);
                }
                dr.Close();
                DataTable dtDistinctInvoices = dt.DefaultView.ToTable(true, "Id");
                for (int i = 0; i < dtDistinctInvoices.Rows.Count; i++)
                {
                    List<EmployeeInfoVM> empSingle = new List<EmployeeInfoVM>();
                    empSingle = VMs.Where(x => x.Id == dtDistinctInvoices.Rows[i].ItemArray[0].ToString()).ToList();
                    for (int j = 0; j < empSingle.Count; j++)
                    {
                        if (!string.IsNullOrWhiteSpace(PL))
                        {
                            if (j > 0)
                                break;
                        }
                        vm = new EmployeeInfoVM();
                        vm.Id = empSingle.ElementAt(j).Id;
                        vm.Code = empSingle.ElementAt(j).Code;
                        vm.Salutation_E = empSingle.ElementAt(j).Salutation_E;
                        vm.MiddleName = empSingle.ElementAt(j).MiddleName;
                        vm.LastName = empSingle.ElementAt(j).LastName;
                        vm.FullName = empSingle.ElementAt(j).FullName;
                        vm.Department = empSingle.ElementAt(j).Department;
                        vm.Designation = empSingle.ElementAt(j).Designation;
                        vm.Grade = empSingle.ElementAt(j).Grade;
                        vm.Date = empSingle.ElementAt(j).Date;
                        vm.JoinDate = empSingle.ElementAt(j).JoinDate;
                        vm.FromDesignation = empSingle.Count > j + 1 ? empSingle.ElementAt(j + 1).Designation : "*";
                        reportVMs.Add(vm);
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
                if (currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
            #endregion
            return reportVMs;
        }

        public List<EmployeeInfoVM> EmpTransferLetter(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtjFrom, string dtjTo, string EmployeeId, string dttFrom, string dttTo, string LetterName = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            List<EmployeeInfoVM> reportVMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm;
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
select e.id,e.Code, e.Salutation_E, e.MiddleName, e.LastName 
,br.name Branch
,dp.Name Department
,dg.Name Designation
,s.Name Section
,p.Name Project
,t.transferDate
, t.isCurrent
,ej.joindate
,g.sl
from EmployeeInfo e
left outer join EmployeeTransfer t on e.Id=t.employeeId and t.IsArchive=0
left outer join Project p on p.id=t.projectId
left outer join Section s on s.id=t.SectionId
left outer join Department dp on dp.Id=t.DepartmentId
left outer join Branch br on br.Id = t.BranchId
left outer join EmployeePromotion ep on ep.employeeId=e.Id and ep.IsArchive=0 and ep.IsCurrent=1
left outer join Designation dg on dg.Id=ep.DesignationId
left outer join EmployeeJob ej  on ej.EmployeeId=e.Id
left outer join Grade g  on ep.gradeId=g.Id
where e.IsArchive=0 and e.IsActive=1
";
                #region Parrameters Apply
                if (DesignationId != "0_0")
                {
                    sqlText += "  and dg.id=@DesignationId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and dp.id=@DepartmentId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and s.id=@SectionId";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and p.id=@ProjectId";
                }
                if (CodeF != "0_0")
                {
                    sqlText += "  and e.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and e.Code<=@CodeT";
                }
                if (dtjFrom != "0_0")
                {
                    sqlText += "  and ej.joindate>=@dtjFrom";
                }
                if (dtjTo != "0_0")
                {
                    sqlText += "  and ej.joindate<=@dtjTo";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and e.id='" + EmployeeId + "'";
                }
                if (dttFrom != "0_0")
                {
                    sqlText += "  and t.transferDate>=@dttFrom";
                }
                if (dttTo != "0_0")
                {
                    sqlText += "  and t.transferDate<=@dttTo";
                }
                sqlText += " order by dp.Name, ej.JoinDate, e.code, t.transferDate desc, t.Id desc";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                if (CodeF != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    objComm.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    objComm.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtjFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttFrom", Ordinary.DateToString(dtjFrom));
                }
                if (dtjTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttTo", Ordinary.DateToString(dtjTo));
                }
                if (dttFrom != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttFrom", Ordinary.DateToString(dttFrom));
                }
                if (dttTo != "0_0")
                {
                    objComm.Parameters.AddWithValue("@dttTo", Ordinary.DateToString(dttTo));
                }
                #endregion Parrameters Apply
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();
                da.Fill(dt);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    vm.Id = dr["id"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Salutation_E = dr["Salutation_E"].ToString();
                    vm.MiddleName = dr["MiddleName"].ToString();
                    vm.LastName = dr["LastName"].ToString();
                    vm.FullName = vm.Salutation_E + " " + vm.MiddleName + " " + vm.LastName;
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Date = Ordinary.StringToDate(dr["transferDate"].ToString());
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    VMs.Add(vm);
                }
                dr.Close();
                DataTable dtDistinct = dt.DefaultView.ToTable(true, "Id");
                for (int i = 0; i < dtDistinct.Rows.Count; i++)
                {
                    List<EmployeeInfoVM> empSingle = new List<EmployeeInfoVM>();
                    empSingle = VMs.Where(x => x.Id == dtDistinct.Rows[i].ItemArray[0].ToString()).ToList();
                    for (int j = 0; j < empSingle.Count; j++)
                    {
                        vm = new EmployeeInfoVM();
                        vm.Id = empSingle.ElementAt(j).Id;
                        vm.Code = empSingle.ElementAt(j).Code;
                        vm.Salutation_E = empSingle.ElementAt(j).Salutation_E;
                        vm.MiddleName = empSingle.ElementAt(j).MiddleName;
                        vm.LastName = empSingle.ElementAt(j).LastName;
                        vm.FullName = empSingle.ElementAt(j).FullName;
                        vm.Branch = empSingle.ElementAt(j).Branch;
                        vm.Department = empSingle.ElementAt(j).Department;
                        vm.Designation = empSingle.ElementAt(j).Designation;
                        vm.Project = empSingle.ElementAt(j).Project;
                        vm.Section = empSingle.ElementAt(j).Section;
                        vm.Date = empSingle.ElementAt(j).Date;
                        vm.JoinDate = empSingle.ElementAt(j).JoinDate;
                        vm.FromBranch = empSingle.Count > j + 1 ? empSingle.ElementAt(j + 1).Branch : "*";
                        vm.FromDepartment = empSingle.Count > j + 1 ? empSingle.ElementAt(j + 1).Department : "*";
                        vm.FromSection = empSingle.Count > j + 1 ? empSingle.ElementAt(j + 1).Section : "*";
                        vm.FromProject = empSingle.Count > j + 1 ? empSingle.ElementAt(j + 1).Project : "*";
                        reportVMs.Add(vm);
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

        #endregion

        //==================Letters=================

        public DataTable EmployeeListLetter(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null, string OrderByCode = "")
        {
            #region Variables
            SqlConnection currConn = null;
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
select e.EmployeeId, e.Code, e.Salutation_E Salutation, e.MiddleName, e.LastName, e.EmpName  
,e.Department
,e.Section
,e.Project 
,e.Designation
,e.JoinDate
,e.Branch
,e.EmploymentType
,e.EmploymentStatus
,ed.PersonalContactNo Mobile
,a.Address ,a.District,a.Division,a.Country
,ed.Gender_E Gender
,ed.Religion
,e.Grade

,ed.BloodGroup_E BloodGroup
,ed.Email
,e.BasicSalary
,e.GrossSalary
,e.AttnUserId
,e.StepName 

,j.Other1
,j.Other2
,j.Other3
,j.Other4
,j.Other5
,e.MedicalAmount
,e.HouseRentAmount
,e.ConveyanceAmount
,e.PassportNumber
,e.ExpiryDate
,e.DateOfPermanent
,0 ProbationMonth-- case when e.ProbationMonth > 0 then  e.ProbationMonth else  DATEDIFF(MONTH, e.JoinDate,  e.DateOfPermanent) end as ProbationMonth
,e.PromotionDate
,ess.IncrementDate

from ViewEmployeeSalaryStructureDetail e
left outer join employeeJob j on j.EmployeeId=e.Id
left outer join employeePresentAddress a on e.Id=a.EmployeeId
left outer join EmployeePersonalDetail ed on ed.EmployeeId=e.Id
left outer join EmployeeSalaryStructure ess on ess.EmployeeId=e.Id and IsCurrent = 1

where e.IsArchive=0 and e.IsActive=1
";
                #endregion SqlText
                #region More Conditions

                if (vm.CodeList != null && vm.CodeList.Count > 0)
                {
                    string MultipleCode = "";
                    foreach (var item in vm.CodeList)
                    {
                        MultipleCode += "'" + item + "',";
                    }
                    MultipleCode = MultipleCode.Trim(',');
                    sqlText += " AND e.Code IN(" + MultipleCode + ")";
                }
                #endregion More Conditions
                #region ConditionFields
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                #endregion ConditionFields
                if (OrderByCode == "true")
                {
                    sqlText += "  order by e.Code";
                }
                else
                {
                    sqlText += "  order by e.Department, e.GradeSL, e.JoinDate, e.Code";
                }
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region ConditionValues
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                #endregion ConditionValues



                da.Fill(dt);
                string[] dateColumnChange = { "JoinDate", "DateOfPermanent", "ExpiryDate", "PromotionDate", "IncrementDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumnChange);

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
            return dt;
        }

        public DataTable EmployeeListTravelLetter(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null, string OrderByCode = "")
        {
            #region Variables
            SqlConnection currConn = null;
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
select 
e.EmployeeId
,et.Country TravelCountry, et.PassportNumber, et.IssueDate, et.ExpiryDate, et.TravelType_E TravelType, et.FromDate, et.ToDate
,et.EmbassyName
, e.Code, e.Salutation_E Salutation, e.MiddleName, e.LastName, e.EmpName  
,e.Department
,e.Section
,e.Project 
,e.Designation
,e.JoinDate
,e.Branch
,e.EmploymentType
,e.EmploymentStatus

,a.Mobile
,a.Address ,a.District,a.Division,a.Country

,ed.Gender_E Gender
,ed.Religion
,e.Grade

,ed.BloodGroup_E BloodGroup
,ed.Email

,e.BasicSalary
,e.GrossSalary
,e.AttnUserId
,e.StepName 
,j.Other1
,j.Other2
,j.Other3

,e.MedicalAmount
,e.HouseRentAmount
,e.ConveyanceAmount
,e.DateOfPermanent
,case when e.ProbationMonth > 0 then  e.ProbationMonth else  DATEDIFF(MONTH, e.JoinDate,  e.DateOfPermanent) end as ProbationMonth
,e.PromotionDate


from ViewEmployeeSalaryStructureDetail e
left outer join employeeJob j on j.EmployeeId=e.EmployeeId
left outer join employeePresentAddress a on e.Id=a.EmployeeId
left outer join EmployeePersonalDetail ed on ed.EmployeeId=e.EmployeeId
left outer join EmployeeTravel et on e.EmployeeId = et.EmployeeId
where e.IsArchive=0 and e.IsActive=1
";
                #endregion SqlText

                #region More Conditions

                if (vm.CodeList != null && vm.CodeList.Count > 0)
                {
                    string MultipleCode = "";
                    foreach (var item in vm.CodeList)
                    {
                        MultipleCode += "'" + item + "',";
                    }
                    MultipleCode = MultipleCode.Trim(',');
                    sqlText += " AND e.Code IN(" + MultipleCode + ")";
                }
                #endregion More Conditions

                #region ConditionFields
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }


                #endregion ConditionFields
                if (OrderByCode == "true")
                {
                    sqlText += "  order by e.Code";
                }
                else
                {
                    sqlText += "  order by e.Department, e.GradeSL, e.JoinDate, e.Code";
                }
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region ConditionValues
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                #endregion ConditionValues



                da.Fill(dt);
                string[] dateColumnChange = { "JoinDate", "DateOfPermanent", "IssueDate", "ExpiryDate", "FromDate", "ToDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumnChange);

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
            return dt;
        }

        public DataTable EmployeeNewReport(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            #region Variables
            SqlConnection currConn = null;
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
                #region sqlText
                sqlText = @"
SELECT        
ei.Id AS EmployeeId
, ei.Id
, ei.Code
, ei.Salutation_E
, ei.MiddleName
, ei.LastName
, ISNULL(LTRIM(RTRIM(ei.Salutation_E)), N'') 
  + ' ' + ISNULL(LTRIM(RTRIM(ei.MiddleName)), N'') + ' ' + ISNULL(LTRIM(RTRIM(ei.LastName)), N'') AS EmpName
, ISNULL(NULLIF (ej.JoinDate, N''), N'19000101') AS JoinDate
, ISNULL(NULLIF (ej.ProbationEnd, N''), N'19000101') AS ProbationEnd
, ISNULL(NULLIF (ej.DateOfPermanent, N''), N'19000101') AS DateOfPermanent
, ISNULL(NULLIF (ej.LeftDate, N''), N'19000101') AS LeftDate
, ISNULL(ees.Name, N'NA') AS EmploymentStatus
, ISNULL(eet.Name, N'NA') AS EmploymentType
, ISNULL(p.Name, N'NA') AS Project
, ISNULL(b.Name, N'NA') AS Branch
, ISNULL(d.Name, N'NA') AS Department
, ISNULL(s.Name, N'NA') AS Section
, ISNULL(NULLIF (et.TransferDate, N''), N'19000101') AS TransferDate
, ISNULL(desig.Name, N'NA') AS Designation
, ISNULL(g.Name, N'NA') AS Grade
, ISNULL(ep.IsPromotion, N'False') AS IsPromotion
, ISNULL(NULLIF (ep.PromotionDate, N''), N'19000101') AS PromotionDate
, ISNULL(p.Id, N'0') AS ProjectId
, ISNULL(s.Id, N'0') AS SectionId
, ISNULL(d.Id, N'0') AS DepartmentId
, ISNULL(desig.Id, N'0') AS DesignationId
, ISNULL(g.Id, N'0') AS GradeId
, ISNULL(b.Id, N'0') AS BranchId
, ISNULL(ei.IsActive, N'False') AS IsActive
, ISNULL(ei.IsArchive, N'False') AS IsArchive
, ei.AttnUserId
, ISNULL(ej.GrossSalary, 0) AS GrossSalary
, ISNULL(ej.BasicSalary, 0) AS BasicSalary
, ISNULL(ej.IsPermanent, N'False') AS IsPermanent
, ISNULL(esg.PFStructureId, N'0_0') AS PFStructureId
, ISNULL(esg.ProjectAllocationId, N'0_0') AS ProjectAllocationId
, ISNULL(esg.SalaryStructureId, N'0_0') AS SalaryStructureId
, ISNULL(esg.TaxStructureId, N'0_0') AS TaxStructureId
, ISNULL(esg.EmployeeGroupId, N'0_0') AS EmployeeGroupId
, ISNULL(esg.LeaveStructureId, N'0_0') AS LeaveStructureId
, ISNULL(g.SL, 99) AS GradeSL
, ISNULL(st.Name, N'NA') AS StepName
, ISNULL(st.SL, 99) AS StepSL
, empp.Gender_E AS Gender
, empp.Religion
, ei.PhotoName
, empp.Email
, ISNULL(ej.Other1, N'NA') AS Other1
, ISNULL(ej.Other2, N'NA') AS Other2
, ISNULL(ej.Other3, N'NA') AS Other3
, ISNULL(ej.Other4, N'NA') AS Other4
, ISNULL(ej.Other5, N'NA') AS Other5
, ISNULL(empp.CorporateContactNo,'NA') AS CorporateContactNo
, ISNULL(empp.CorporateContactLimit,0) AS CorporateContactLimit
, ISNULL(empp.PersonalContactNo ,'NA')  AS PersonalContactNo
, ISNULL(empp.DateOfBirth		  ,'NA')  AS DateOfBirth
, ISNULL(empLeft.LeftType_E	  ,'NA')  AS LeftType
, ISNULL(empLeft.EntryLeftDate  ,'NA')  AS EntryLeftDate
, empEd.Degree_E Degree
, empEd.Major
, empEd.YearOfPassing

, ISNULL(empEmCon.Name ,'NA') AS EmConName
, ISNULL(empEmCon.Relation	,'NA') AS	EmConRelation
, ISNULL(empEmCon.Phone		,'NA') AS	EmConPhone	
, ISNULL(empEmCon.Mobile		,'NA') AS	EmConMobile	
, ISNULL(empEmCon.Address		,'NA') AS	EmConAddress	
, ISNULL(ej.BankInfo			,'NA') AS	BankInfo		
, ISNULL(ej.BankAccountNo		,'NA') AS	BankAccountNo
, ISNULL(empp.MaritalStatus_E	,'NA') AS	MaritalStatus
, ISNULL(empp.Nationality_E	,'NA') AS	Nationality	
, ISNULL(DATEDIFF(yy,  Convert(date, empp.DateOfBirth), Convert(date,GetDate())),'0') as Age
, '' BirthDayStatus
, case
 when ISNULL( NULLIF(NULLIF (ej.LeftDate, '19000101'), ''), '') = '' then CONVERT(char(8), GetDate(),112)
 else ej.LeftDate 
 end as ServiceToDate
, case when ei.IsActive = 1 then 'Active' else  empLeft.LeftType_E end as ActiveStatus
, '' ServiceDuration
, ISNULL(datename(month, (Convert(date, ej.JoinDate))), 'NA') MonthName






FROM     
dbo.EmployeeInfo AS ei LEFT OUTER JOIN       
dbo.EmployeePresentAddress AS prAdd ON prAdd.EmployeeId = ei.Id   LEFT OUTER JOIN
dbo.EmployeeEducation AS empEd ON ei.Id = empEd.EmployeeId and empEd.IsLast = 1 LEFT OUTER JOIN

dbo.EmployeeEmergencyContact AS empEmCon ON ei.Id = empEmCon.EmployeeId LEFT OUTER JOIN
dbo.EmployeePermanentAddress AS perAdd ON ei.Id = perAdd.EmployeeId LEFT OUTER JOIN

dbo.EmployeeJob AS ej ON ei.Id = ej.EmployeeId LEFT OUTER JOIN
dbo.EmployeeTransfer AS et ON ei.Id = et.EmployeeId AND et.IsCurrent = 1 LEFT OUTER JOIN
dbo.EmployeePromotion AS ep ON ei.Id = ep.EmployeeId AND ep.IsCurrent = 1 LEFT OUTER JOIN
dbo.EnumEmploymentStatus AS ees ON ej.EmploymentStatus_E = ees.Id LEFT OUTER JOIN
dbo.EnumEmploymentType AS eet ON ej.EmploymentType_E = eet.Id LEFT OUTER JOIN
dbo.Project AS p ON et.ProjectId = p.Id LEFT OUTER JOIN
dbo.Department AS d ON et.DepartmentId = d.Id LEFT OUTER JOIN
dbo.Section AS s ON et.SectionId = s.Id LEFT OUTER JOIN
dbo.Designation AS desig ON ep.DesignationId = desig.Id LEFT OUTER JOIN
dbo.Grade AS g ON ep.GradeId = g.Id LEFT OUTER JOIN
dbo.EnumSalaryStep AS st ON ep.StepId = st.Id LEFT OUTER JOIN
dbo.Branch AS b ON ei.BranchId = b.Id LEFT OUTER JOIN
dbo.EmployeePersonalDetail AS empp ON empp.EmployeeId = ei.Id LEFT OUTER JOIN
dbo.EmployeeStructureGroup AS esg ON ei.Id = esg.EmployeeId LEFT OUTER JOIN
dbo.EmployeeLeftInformation AS empLeft ON ei.Id = empLeft.EmployeeId


WHERE 1=1
and ei.Id <> '1_0'
";
                #endregion
                #region More Conditions
                if (vm.CodeList != null && vm.CodeList.Count > 0)
                {
                    string MultipleCode = "";
                    foreach (var item in vm.CodeList)
                    {
                        MultipleCode += "'" + item + "',";
                    }
                    MultipleCode = MultipleCode.Remove(MultipleCode.Length - 1);
                    sqlText += " AND ei.Code IN(" + MultipleCode + ")";
                }
                if (vm.DesignationList != null && vm.DesignationList.Count > 0)
                {
                    string MultipleDesignation = "";
                    foreach (var item in vm.DesignationList)
                    {
                        MultipleDesignation += "'" + item + "',";
                    }
                    MultipleDesignation = MultipleDesignation.Remove(MultipleDesignation.Length - 1);
                    sqlText += " AND desig.Id IN(" + MultipleDesignation + ")";
                }
                if (vm.DepartmentList != null && vm.DepartmentList.Count > 0)
                {
                    string MultipleDepartment = "";
                    foreach (var item in vm.DepartmentList)
                    {
                        MultipleDepartment += "'" + item + "',";
                    }
                    MultipleDepartment = MultipleDepartment.Remove(MultipleDepartment.Length - 1);
                    sqlText += " AND d.Id IN(" + MultipleDepartment + ")";
                }
                if (vm.SectionList != null && vm.SectionList.Count > 0)
                {
                    string MultipleSection = "";
                    foreach (var item in vm.SectionList)
                    {
                        MultipleSection += "'" + item + "',";
                    }
                    MultipleSection = MultipleSection.Remove(MultipleSection.Length - 1);
                    sqlText += " AND s.Id IN(" + MultipleSection + ")";
                }
                if (vm.ProjectList != null && vm.ProjectList.Count > 0)
                {
                    string MultipleProject = "";
                    foreach (var item in vm.ProjectList)
                    {
                        MultipleProject += "'" + item + "',";
                    }
                    MultipleProject = MultipleProject.Remove(MultipleProject.Length - 1);
                    sqlText += " AND p.Id IN(" + MultipleProject + ")";
                }
                if (vm.Other1List != null && vm.Other1List.Count > 0)
                {
                    string MultipleOther1 = "";
                    foreach (var item in vm.Other1List)
                    {
                        MultipleOther1 += "'" + item + "',";
                    }
                    MultipleOther1 = MultipleOther1.Remove(MultipleOther1.Length - 1);
                    sqlText += " AND ej.Other1 IN(" + MultipleOther1 + ")";
                }
                if (vm.Other2List != null && vm.Other2List.Count > 0)
                {
                    string MultipleOther2 = "";
                    foreach (var item in vm.Other2List)
                    {
                        MultipleOther2 += "'" + item + "',";
                    }
                    MultipleOther2 = MultipleOther2.Remove(MultipleOther2.Length - 1);
                    sqlText += " AND ej.Other2 IN(" + MultipleOther2 + ")";
                }
                if (vm.Other3List != null && vm.Other3List.Count > 0)
                {
                    string MultipleOther3 = "";
                    foreach (var item in vm.Other3List)
                    {
                        MultipleOther3 += "'" + item + "',";
                    }
                    MultipleOther3 = MultipleOther3.Remove(MultipleOther3.Length - 1);
                    sqlText += " AND ej.Other3 IN(" + MultipleOther3 + ")";
                }

                #endregion
                #region ConditionFields
                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                #endregion ConditionFields
                if (vm.OrderByCode == "true")
                {
                    sqlText += "  order by ei.Code";
                }
                else
                {
                    sqlText += "  order by d.Name, g.SL, ej.JoinDate, ei.Code";
                }
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region ConditionValues
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                #endregion ConditionValues

                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    dr["ServiceDuration"] = Ordinary.DateDifferenceYMD(dr["JoinDate"].ToString(), dr["ServiceToDate"].ToString(), true);
                }

                string[] dateColumnChange = { "JoinDate", "DateOfPermanent", "PromotionDate", "LeftDate", "TransferDate", "PromotionDate", "DateOfBirth", "EntryLeftDate", "ServiceToDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, dateColumnChange);

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
            return dt;
        }

        public DataTable EmployeeSummaryReport(EmployeeInfoVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
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

                if (!string.IsNullOrWhiteSpace(vm.DBName))
                {
                    currConn.ChangeDatabase(vm.DBName);
                }
                #endregion open connection and transaction
                #region sql statement
                #region sqlText
                sqlText = @"
declare @StartDate as varchar(20)  
declare @EndDate as varchar(20)
  
------declare @FiscalYearDetailId as varchar(20) 
------set @FiscalYearDetailId = 1016

select @StartDate=periodStart,@EndDate=PeriodEnd from FiscalYearDetail
where id=@FiscalYearDetailId
---------------------------------------------------------------------------------------------------
-------------------------------Regular-------------------------------------------------------------
select p.Name Project, d.Name Department, mp.LastMonthEmployees, mp.NewEmployees, mp.LeftEmployees
, (mp.LastMonthEmployees+mp.NewEmployees-mp.LeftEmployees) CurrentMonthEmployees 
from(
select lme.ProjectId,  lme.DepartmentId
, ISNULL(lme.LastMonthEmployees,0) LastMonthEmployees
, ISNULL(ne.NewEmployees,0) NewEmployees
, ISNULL(le.LeftEmployees,0) LeftEmployees
  from 
(

select ve.ProjectId, ve.DepartmentId, Count(ve.Code) LastMonthEmployees from ViewEmployeeInformation ve 
where 1=1 
AND ve.JoinDate <@StartDate
AND ISNULL(ve.LeftDate,'19000101') not between @StartDate and @EndDate
AND ve.IsActive=1
Group By  ve.ProjectId, ve.DepartmentId
) as lme 
---------------------------------------------------------------------------------------------------
-------------------------------New-----------------------------------------------------------------
LEFT OUTER JOIN
(
select  ve.ProjectId, ve.DepartmentId, Count(ve.Code) NewEmployees from ViewEmployeeInformation ve
where 1=1 
and ve.JoinDate between @StartDate and @EndDate
and ve.IsActive=1
Group By  ve.ProjectId, ve.DepartmentId
) as ne ON lme.DepartmentId = ne.DepartmentId
---------------------------------------------------------------------------------------------------
-------------------------------Left----------------------------------------------------------------
LEFT OUTER JOIN
(
select  ve.ProjectId, ve.DepartmentId, Count(ve.Code) LeftEmployees from ViewEmployeeInformation ve
where 1=1 
and ve.JoinDate < @StartDate and ISNULL(ve.LeftDate,'19000101') between @StartDate and @EndDate
and ve.IsActive=0
Group By  ve.ProjectId, ve.DepartmentId
) as le ON lme.DepartmentId = le.DepartmentId
) as mp
LEFT OUTER JOIN Project p ON p.Id = mp.ProjectId
LEFT OUTER JOIN Department d ON d.Id = mp.DepartmentId
WHERE 1=1
AND (p.Name is not null AND p.Name != 'NA')
AND (d.Name is not null AND d.Name != 'NA')
ORDER BY p.Name, d.Name

";
                #endregion
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                da.Fill(dt);

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
            return dt;
        }

        public DataTable CombinedEmployeeSummaryReport(EmployeeInfoVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
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

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                if (CompanyName.ToLower() == "kbl" || CompanyName.ToLower() == "anupam" || CompanyName.ToLower() == "kajol")
                {
                    #region KajolBrothersHRM
                    vm.DBName = "KajolBrothersHRM";
                    dt = EmployeeSummaryReport(vm);

                    #endregion

                    #region KajolBrothersHRM
                    vm.DBName = "AnupamPrintersHRM";
                    DataTable dtAnupamPrinters_EmployeeSummaryReport = new DataTable();
                    dtAnupamPrinters_EmployeeSummaryReport = EmployeeSummaryReport(vm);

                    foreach (DataRow dr in dtAnupamPrinters_EmployeeSummaryReport.Rows)
                    {
                        dr["Project"] = "Anupam Printers";
                    }
                    #endregion

                    dt.Merge(dtAnupamPrinters_EmployeeSummaryReport);

                }
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
            return dt;
        }

        #region Reports

        public DataTable LeaveStatus_Type(EmployeeInfoVM vm, string[] conditionFields = null, string[] conditionValues = null)
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
----declare @LeaveYear as int = 2020
----declare @EmployeeId as varchar(100) = '1_33'

select lev.EmployeeId
, ve.Code
, ve.EmpName
, ve.Designation
, lev.AllowedLeave
, lev.EarnedLeave
, lev.SickLeave
, lev.CasualLeave
, (lev.EarnedLeave + lev.SickLeave + lev.CasualLeave) TotalLeaveTaken
, lev.AllowedLeave - (lev.EarnedLeave + lev.SickLeave + lev.CasualLeave) Balance
, '' Remarks 
from
(
select levD.EmployeeId
, sum(levD.AllowedLeave) AllowedLeave
, sum(levD.EarnedLeave) EarnedLeave
, sum(levD.SickLeave) SickLeave
, sum(levD.CasualLeave) CasualLeave
from
(
select EmployeeId, sum(LeaveDays) AllowedLeave, 0 'EarnedLeave', 0 'SickLeave', 0 'CasualLeave'
from EmployeeLeaveStructure
where 1=1
and LeaveYear=@LeaveYear and EmployeeId=@EmployeeId
and LeaveType_E in ('Sick leave','Casual Leave','Earned Leave')
group by EmployeeId

union all

select EmployeeId, 0 AllowedLeave, sum(TotalLeave) 'EarnedLeave', 0 'SickLeave', 0 'CasualLeave'
from EmployeeLeave
where 1=1
and LeaveYear=@LeaveYear and EmployeeId=@EmployeeId
and LeaveType_E in ('Earned Leave')
group by EmployeeId

union all

select EmployeeId, 0 AllowedLeave, 0 'EarnedLeave', sum(TotalLeave) 'SickLeave', 0 'CasualLeave'
from EmployeeLeave
where 1=1
and LeaveYear=@LeaveYear and EmployeeId=@EmployeeId
and LeaveType_E in ('Sick leave')
group by EmployeeId

union all

select EmployeeId, 0 AllowedLeave, 0 'EarnedLeave', 0 'SickLeave', sum(TotalLeave) 'CasualLeave'
from EmployeeLeave
where 1=1
and LeaveYear=@LeaveYear and EmployeeId=@EmployeeId
and LeaveType_E in ('Casual Leave')
group by EmployeeId
) as levD
group by EmployeeId
) as lev
left outer join ViewEmployeeInformation ve on ve.EmployeeId=lev.EmployeeId
LEFT OUTER JOIN Designation AS desig ON ve.DesignationId = desig.Id
where 1=1 


";
                if (string.IsNullOrWhiteSpace(vm.EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[i].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        sqlText += " AND " + conditionFields[i] + "=@" + cField;
                    }
                }

                if (!string.IsNullOrWhiteSpace(vm.MultipleOther3))
                {
                    string MultipleOther3ConditionValues = "";
                    MultipleOther3ConditionValues = vm.MultipleOther3.Replace(",", "','");
                    sqlText += "  AND ve.Other3 IN('" + MultipleOther3ConditionValues + "')";
                }

                #region Ordering
                if (!string.IsNullOrWhiteSpace(vm.OrderBy))
                {

                    if (vm.OrderBy == "DCG")
                        sqlText += " order by ve.Department, ve.code";
                    else if (vm.OrderBy == "DDC")
                        sqlText += " order by ve.Department, ve.JoinDate, ve.code";
                    else if (vm.OrderBy == "DGC")
                        sqlText += " order by ve.Department, ve.code";
                    else if (vm.OrderBy == "DGDC")
                        sqlText += " order by  ve.Department, ve.JoinDate, ve.code";
                    else if (vm.OrderBy == "CODE")
                        sqlText += " order by ve.code";
                    else if (vm.OrderBy.ToLower() == "designation")
                        sqlText += " order by ISNULL(desig.PriorityLevel,0), ve.code";
                }
                #endregion


                #endregion

                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (!string.IsNullOrWhiteSpace(vm.EmployeeId))
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                }

                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int j = 0; j < conditionFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
                        {
                            continue;
                        }
                        cField = conditionFields[j].ToString();
                        cField = Ordinary.StringReplacing(cField);
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                da.Fill(dt);

                #endregion

                #endregion
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit
            }
            #region Catch and Finall
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            #endregion
            #region Finally
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
            return dt;
        }

        #endregion

        public DataTable EmployeeSummaryLeaveRegisterReport(string CodeF, string CodeT, string DepartmentId, string SectionId
           , string ProjectId
           , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
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
                #region sqlText
                sqlText = @"
Select el.EmployeeId
,ve.MiddleName EmpName
,ve.Code 
,el.Year
,sum(el.CFBalance)CFBalance
,sum(el.AnnualLeaveEntitle)AnnualLeaveEntitle
,sum(el.AnnualLeaveTaken)AnnualLeaveTaken
,sum(el.AnnualBalance)AnnualBalance
,sum(ele.EncashmentBalance)EncashmentBalance
from EarnLeaveEncashmentStatement ele
left outer join  ViewEmployeeInformation ve  on ele.EmployeeId=ve.EmployeeId
left outer join EarnLeaveStatement el on ele.EmployeeId=el.EmployeeId
where 1=1 


";

                #endregion
                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ve.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ve.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ve.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ve.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ve.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ve.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ve.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ve.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += " and ele.Year=@leaveyear and el.Year=@leaveyear";
                }

                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ve.EmployeeId='" + EmployeeId + "'";
                }
                sqlText += "  group by el.EmployeeId,ve.MiddleName,ve.Code,el.Year";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                if (CodeF != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    da.SelectCommand.Parameters.AddWithValue("@leaveyear", leaveyear);
                }

                #endregion Parrameters Apply


                da.Fill(dt);

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
            return dt;
        }


        public List<EmployeeInfoVM> SelectActiveEmp(string DOJFrom = "", string DOJTo = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
            EmployeeInfoVM vm = new EmployeeInfoVM();
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
                                    e.EmpName, 
                                    e.Code,
                                    e.Designation,
                                    e.Branch,
                                    e.Department,
                                    e.Section,
                                    e.Project,                          
                                    e.JoinDate                                                           
                                FROM ViewEmployeeInformation e
                                LEFT OUTER JOIN EmployeeTransfer empt 
                                    ON CAST(e.EmployeeId AS NVARCHAR) = CAST(empt.EmployeeId AS NVARCHAR)
                                     WHERE e.IsArchive = 0 
                                    AND e.IsActive = 1
                                    ";

                //sqlText += "     ORDER BY e.Department, e.EmpName desc";
                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeInfoVM();
                    //vm.Id = dr["Id"].ToString();
                    //vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    //vm.Salutation_E = dr["Salutation_E"].ToString();
                    //vm.MiddleName = dr["MiddleName"].ToString();
                    //vm.LastName = dr["LastName"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    //vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    //vm.Other1 = dr["Other1"].ToString();
                    //vm.Other2 = dr["Other2"].ToString();
                    //vm.Other3 = dr["Other3"].ToString();
                    //vm.Other4 = dr["Other4"].ToString();
                    //vm.Other5 = dr["Other5"].ToString();
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

        public DataSet EmployeeLeaveRegisterShampan(string CodeF, string CodeT, string DepartmentId, string SectionId
         , string ProjectId
         , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            EmployeeLeaveVM vm;
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
select
ei.Code EmpCode
, ei.EmpName
, ei.EmployeeId
, ei.JoinDate     
, ei.Branch       
, ei.Project      
, ei.Department   
, ei.Section      
, ei.Designation  
, ei.ProjectId    
, ei.DepartmentId 
, ei.SectionId    
, ei.DesignationId
, ei.Grade        
, isnull(eleave.LeaveYear, 0)           LeaveYear
, isnull(eleave.LeaveType_E,	N'NA')	LeaveType_E
, isnull(eleave.FromDate	,	N'NA')	FromDate
, isnull(eleave.ToDate		,	N'NA')	ToDate
, isnull(eleave.TotalLeave	,	0)		TotalLeave
, isnull(eleave.ApprovedBy	,	N'NA')	ApprovedBy
, isnull(eleave.ApproveDate,	N'NA')	ApproveDate
, isnull(eleave.IsApprove	,	0)		IsApprove
, isnull(eleave.RejectedBy	,	N'NA')	RejectedBy
, isnull(eleave.RejectDate	,	N'NA')	RejectDate
, isnull(eleave.IsReject	,	0)		IsReject
, isnull(eleave.IsHalfDay	,	0)		IsHalfDay
from
ViewEmployeeInformationAll ei
left outer join EmployeeLeave eleave on ei.EmployeeId = eleave.EmployeeId
where ei.IsArchive=0 and ei.IsActive=1 and eleave.IsApprove=1
";


                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and eleave.leaveyear=@leaveyear";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and eleave.LEAVETYPE_E=@LEAVETYPE_E";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId='" + EmployeeId + "'";
                }
                #endregion Parrameters Apply

                sqlText += "  ORDER BY ei.Department, ei.GradeSL, ei.JoinDate, ei.Code, eleave.LEAVETYPE_E";

                sqlText += @"

SELECT ei.EmployeeId

,ES.ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(cast(EL.LEAVE as decimal(18,2)),0),0) HAVE
FROM ViewEmployeeInformation ei 
left outer join EMPLOYEELEAVESTRUCTURE ES on ES.EMPLOYEEID=ei.id
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
left outer join EmployeePersonalDetail pd on pd.employeeId=ei.Id
where ei.IsArchive=0 and ei.IsActive=1 
";

                #region Parrameters Apply
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and ES.leaveyear=@leaveyear";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and ES.LEAVETYPE_E=@LEAVETYPE_E";
                }
                #endregion Parrameters Apply

                sqlText += @"

SELECT ei.EmployeeId,
ES.LeaveYear
,ES.ID,ES.LEAVETYPE_E ,sum(isnull(es.OpeningLeaveDays,0))OpeningLeaveDays,sum(isnull( ES.LEAVEDAYS,0))LEAVEDAYS,sum(isnull(ELE.EncashmentBalance,0))Encashment
,sum(ISNULL(EL.LEAVE,0)) USED,Sum (ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(cast(EL.LEAVE as decimal(18,2)),0),0)) HAVE
FROM ViewEmployeeInformation ei 
left outer join EMPLOYEELEAVESTRUCTURE ES on ES.EMPLOYEEID=ei.id
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
LEFT OUTER JOIN (Select EmployeeId,[Year], sum (isnull(EncashmentBalance,0))EncashmentBalance from EarnLeaveEncashmentStatement
group by  EmployeeId,[Year]) ELE  ON ES.EmployeeId=ELE.EmployeeId and ES.LeaveYear=ELE.[Year]
left outer join EmployeePersonalDetail pd on pd.employeeId=ei.Id
where ei.IsArchive=0 and ei.IsActive=1 
and LEAVETYPE_E='Annual Leave'
group by ei.EmployeeId,ES.LeaveYear,ES.ID,ES.LEAVETYPE_E
order by ES.LeaveYear 

";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region Parrameters Apply
                if (CodeF != "0_0")
                {

                    da.SelectCommand.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    da.SelectCommand.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    da.SelectCommand.Parameters.AddWithValue("@LEAVETYPE_E", LEAVETYPE_E);
                }
                #endregion Parrameters Apply

                da.Fill(ds);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];
                dt1 = Ordinary.DtColumnStringToDate(dt1, "JoinDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "FromDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "ToDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");


                ds = new DataSet();
                ds.Tables.Add(dt1.Copy());
                ds.Tables.Add(dt2.Copy());
                ds.Tables.Add(dt3.Copy());


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
            return ds;
        }

        public DataSet EmployeeLeaveRegisterG4S(string CodeF, string CodeT, string DepartmentId, string SectionId
          , string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            EmployeeLeaveVM vm;
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
select
ei.Code EmpCode
, ei.EmpName
, ei.EmployeeId
, ei.JoinDate     
, ei.Branch       
, ei.Project      
, ei.Department   
, ei.Section      
, ei.Designation  
, ei.ProjectId    
, ei.DepartmentId 
, ei.SectionId    
, ei.DesignationId
, ei.Grade        
, isnull(eleave.LeaveYear, 0)           LeaveYear
, isnull(eleave.LeaveType_E,	N'NA')	LeaveType_E
, isnull(eleave.FromDate	,	N'NA')	FromDate
, isnull(eleave.ToDate		,	N'NA')	ToDate
, isnull(eleave.TotalLeave	,	0)		TotalLeave
, isnull(eleave.ApprovedBy	,	N'NA')	ApprovedBy
, isnull(eleave.ApproveDate,	N'NA')	ApproveDate
, isnull(eleave.IsApprove	,	0)		IsApprove
, isnull(eleave.RejectedBy	,	N'NA')	RejectedBy
, isnull(eleave.RejectDate	,	N'NA')	RejectDate
, isnull(eleave.IsReject	,	0)		IsReject
, isnull(eleave.IsHalfDay	,	0)		IsHalfDay
from
ViewEmployeeInformationAll ei
left outer join EmployeeLeave eleave on ei.EmployeeId = eleave.EmployeeId
where ei.IsArchive=0 and ei.IsActive=1 and eleave.IsApprove=1
";


                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and eleave.leaveyear=@leaveyear";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and eleave.LEAVETYPE_E=@LEAVETYPE_E";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId='" + EmployeeId + "'";
                }
                #endregion Parrameters Apply

                sqlText += "  ORDER BY ei.Department, ei.GradeSL, ei.JoinDate, ei.Code, eleave.LEAVETYPE_E";

                sqlText += @"

SELECT ei.EmployeeId

,ES.ID,ES.LEAVETYPE_E ,isnull(es.OpeningLeaveDays,0)OpeningLeaveDays,isnull( ES.LEAVEDAYS,0)LEAVEDAYS
,ISNULL(EL.LEAVE,0) USED,ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(cast(EL.LEAVE as decimal(18,2)),0),0) HAVE
FROM ViewEmployeeInformation ei 
left outer join EMPLOYEELEAVESTRUCTURE ES on ES.EMPLOYEEID=ei.id
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
left outer join EmployeePersonalDetail pd on pd.employeeId=ei.Id
where ei.IsArchive=0 and ei.IsActive=1 
";

                #region Parrameters Apply
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and ES.leaveyear=@leaveyear";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and ES.LEAVETYPE_E=@LEAVETYPE_E";
                }
                #endregion Parrameters Apply


                sqlText += @"

SELECT ei.EmployeeId,
ES.LeaveYear
,ES.ID,ES.LEAVETYPE_E ,sum(isnull(es.OpeningLeaveDays,0))OpeningLeaveDays,sum(isnull( ES.LEAVEDAYS,0))LEAVEDAYS,sum(isnull(ELE.EncashmentBalance,0))Encashment
,sum(ISNULL(EL.LEAVE,0)) USED,Sum (ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(cast(EL.LEAVE as decimal(18,2)),0),0)) HAVE
FROM ViewEmployeeInformation ei 
left outer join EMPLOYEELEAVESTRUCTURE ES on ES.EMPLOYEEID=ei.id
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
LEFT OUTER JOIN (Select EmployeeId,[Year], sum (isnull(EncashmentBalance,0))EncashmentBalance from EarnLeaveEncashmentStatement
where isnull(IsApproved,'Y')='Y'
group by  EmployeeId,[Year]) ELE  ON ES.EmployeeId=ELE.EmployeeId and ES.LeaveYear=ELE.[Year]
left outer join EmployeePersonalDetail pd on pd.employeeId=ei.Id
where ei.IsArchive=0 and ei.IsActive=1 
and LEAVETYPE_E='Annual Leave'
group by ei.EmployeeId,ES.LeaveYear,ES.ID,ES.LEAVETYPE_E
order by ES.LeaveYear 
";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region Parrameters Apply
                if (CodeF != "0_0")
                {

                    da.SelectCommand.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    da.SelectCommand.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    da.SelectCommand.Parameters.AddWithValue("@LEAVETYPE_E", LEAVETYPE_E);
                }
                #endregion Parrameters Apply

                da.Fill(ds);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];
                dt1 = Ordinary.DtColumnStringToDate(dt1, "JoinDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "FromDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "ToDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");


                ds = new DataSet();
                ds.Tables.Add(dt1.Copy());
                ds.Tables.Add(dt2.Copy());
                ds.Tables.Add(dt3.Copy());


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
            return ds;
        }

        public DataSet EmployeeLeaveEncashmentG4S(string CodeF, string CodeT, string DepartmentId, string SectionId
                 , string ProjectId
                 , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            EmployeeLeaveVM vm;
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
select
ei.Code EmpCode
, ei.EmpName
, ei.EmployeeId
, ei.JoinDate     
, ei.Branch       
, ei.Project      
, ei.Department   
, ei.Section      
, ei.Designation  
, ei.ProjectId    
, ei.DepartmentId 
, 'InDirect' EmpCategory
, ei.GrossSalary
, ei.Supervisor
, ei.SectionId    
, ei.DesignationId
, ei.Grade        
, isnull(eleave.LeaveYear, 0)           LeaveYear
, isnull(eleave.LeaveType_E,	N'NA')	LeaveType_E
, isnull(eleave.FromDate	,	N'NA')	FromDate
, isnull(eleave.ToDate		,	N'NA')	ToDate
, isnull(eleave.TotalLeave	,	0)		TotalLeave
, isnull(eleave.ApprovedBy	,	N'NA')	ApprovedBy
, isnull(eleave.ApproveDate,	N'NA')	ApproveDate
, isnull(eleave.IsApprove	,	0)		IsApprove
, isnull(eleave.RejectedBy	,	N'NA')	RejectedBy
, isnull(eleave.RejectDate	,	N'NA')	RejectDate
, isnull(eleave.IsReject	,	0)		IsReject
, isnull(eleave.IsHalfDay	,	0)		IsHalfDay
from
ViewEmployeeInformationAll ei
left outer join EmployeeLeave eleave on ei.EmployeeId = eleave.EmployeeId
where ei.IsArchive=0 and ei.IsActive=1 and eleave.IsApprove=1
";


                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and eleave.leaveyear=@leaveyear-1";
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    sqlText += "  and eleave.LEAVETYPE_E=@LEAVETYPE_E";
                }
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId='" + EmployeeId + "'";
                }
                #endregion Parrameters Apply

                sqlText += "  ORDER BY ei.Department, ei.GradeSL, ei.JoinDate, ei.Code, eleave.LEAVETYPE_E";

                sqlText += @"

SELECT 
      MAX(PunchData) PunchData      
  FROM DownloadData d  
  Left Outer Join EmployeeInfo ei on ei.AttnUserId=d.ProxyID
  where 1=1
";

                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }

                #endregion Parrameters Apply

                sqlText += @"

SELECT ei.EmployeeId,
ES.LeaveYear
,ES.ID,ES.LEAVETYPE_E ,sum(isnull(es.OpeningLeaveDays,0))OpeningLeaveDays,sum(isnull( ES.LEAVEDAYS,0))LEAVEDAYS,sum(isnull(ELE.EncashmentBalance,0))Encashment
,sum(ISNULL(EL.LEAVE,0)) USED,Sum (ISNULL(isnull( ES.LEAVEDAYS,0)+isnull(es.OpeningLeaveDays,0)-ISNULL(cast(EL.LEAVE as decimal(18,2)),0),0)) HAVE
FROM ViewEmployeeInformation ei 
left outer join EMPLOYEELEAVESTRUCTURE ES on ES.EMPLOYEEID=ei.id
LEFT OUTER JOIN (
SELECT EMPLOYEELEAVESTRUCTUREID,SUM(TOTALLEAVE) LEAVE FROM EMPLOYEELEAVE where IsApprove=1  GROUP BY EMPLOYEELEAVESTRUCTUREID
) EL ON EL.EMPLOYEELEAVESTRUCTUREID=ES.ID
LEFT OUTER JOIN (Select EmployeeId,[Year], sum (isnull(EncashmentBalance,0))EncashmentBalance from EarnLeaveEncashmentStatement
where isnull(IsApproved,'0')='1'
group by  EmployeeId,[Year]) ELE  ON ES.EmployeeId=ELE.EmployeeId and ES.LeaveYear=ELE.[Year]
left outer join EmployeePersonalDetail pd on pd.employeeId=ei.Id
where ei.IsArchive=0 and ei.IsActive=1 
and LEAVETYPE_E='Annual Leave'";
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and ES.leaveyear=@leaveyear";
                }

                sqlText += " group by ei.EmployeeId,ES.LeaveYear,ES.ID,ES.LEAVETYPE_E order by ES.LeaveYear ";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region Parrameters Apply
                if (CodeF != "0_0")
                {

                    da.SelectCommand.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    da.SelectCommand.Parameters.AddWithValue("@leaveyear", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(LEAVETYPE_E))
                {
                    da.SelectCommand.Parameters.AddWithValue("@LEAVETYPE_E", LEAVETYPE_E);
                }
                #endregion Parrameters Apply

                da.Fill(ds);
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];
                DataTable dt3 = ds.Tables[2];
                dt1 = Ordinary.DtColumnStringToDate(dt1, "JoinDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "FromDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "ToDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");
                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");


                ds = new DataSet();
                ds.Tables.Add(dt1.Copy());
                ds.Tables.Add(dt2.Copy());
                ds.Tables.Add(dt3.Copy());


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
            return ds;
        }

        public DataTable EmployeeLeaveRegisterDownload(string CodeF, string CodeT, string DepartmentId, string SectionId
         , string ProjectId
         , string DesignationId, string dtpFrom, string dtpTo, string leaveyear, string LEAVETYPE_E, string EmployeeId, string EmpCategory, string EmpJobType)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable Dt = new DataTable();
            EmployeeLeaveVM vm;
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
pd.OtherId [G4S ID],
  ei.EmpName, 
  ei.Designation, 
  ei.Department[Division], 
  ei.Project[Department],
  ei.EmploymentStatus, 
  ei.EmploymentType, 
  ei.JoinDate,
 
 CONCAT(
  FLOOR(DATEDIFF(DAY, ei.JoinDate, GETDATE()) / 365.25), ' Yr ',
  DATEDIFF(MONTH, DATEADD(YEAR, FLOOR(DATEDIFF(DAY, ei.JoinDate, GETDATE()) / 365.25), ei.JoinDate), GETDATE()), ' M'
) AS [Services Length in Year],
  --pd.DateOfBirth,
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Annual Leave' THEN ISNULL(es.OpeningLeaveDays, 0)ELSE 0 END) AS [CF ANNUAL BALANCE @LeaveYear ],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Annual Leave' THEN ISNULL(ES.LEAVEDAYS, 0)ELSE 0 END) AS [ANNUAL Leave ENTITLE (YTD)],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Annual Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [ANNUAL LEAVE TAKEN],
   SUM(CASE WHEN ES.LEAVETYPE_E = 'Annual Leave' THEN (ISNULL(CAST(ES.LEAVEDAYS AS DECIMAL(18, 2)), 0)- ISNULL(CAST(EL.LEAVE AS DECIMAL(18, 2)), 0)) ELSE 0 END) AS [ANNUAL CURRENT BALANCE],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Annual Leave' THEN ISNULL(es.OpeningLeaveDays, 0) + ISNULL(ES.LEAVEDAYS, 0) - ISNULL(CAST(EL.LEAVE AS DECIMAL(18, 2)), 0) ELSE 0 END) AS [ANNUAL TOTAL BALANCE],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Casual Leave' THEN ISNULL(ES.LEAVEDAYS, 0)ELSE 0 END) AS [CASUAL LEAVE ENTITLE],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Casual Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [CASUAL LEAVE TAKEN],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Casual Leave' THEN ISNULL(es.OpeningLeaveDays, 0) + ISNULL(ES.LEAVEDAYS, 0) - ISNULL(CAST(EL.LEAVE AS DECIMAL(18, 2)), 0) ELSE 0 END) AS [CURRENT CASUAL BALANCE],

  
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Sick Leave' THEN ISNULL(ES.LEAVEDAYS, 0)ELSE 0 END) AS [SICK LEAVE ENTITLE],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Sick Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [SICK LEAVE TAKEN],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Sick Leave' THEN ISNULL(es.OpeningLeaveDays, 0) + ISNULL(ES.LEAVEDAYS, 0) - ISNULL(CAST(EL.LEAVE AS DECIMAL(18, 2)), 0) ELSE 0 END) AS [SICK LEAVE BALANCE],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Maternity Leavee' THEN ISNULL(ES.LEAVEDAYS, 0)ELSE 0 END) AS [MATERNITY LEAVE ENTITLE],
  SUM(CASE WHEN ES.LEAVETYPE_E = 'Maternity Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [MATERNITY LEAVE TAKEN],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Compensatory Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [COMPENSATORY LEAVE TAKEN],

  SUM(CASE WHEN ES.LEAVETYPE_E = 'Without Pay Leave' THEN ISNULL(EL.LEAVE, 0)ELSE 0 END) AS [WITHOUT PAY LEAVE TAKEN]
  
FROM 
  ViewEmployeeInformation ei 
  LEFT OUTER JOIN EMPLOYEELEAVESTRUCTURE ES ON ES.EMPLOYEEID = ei.id
  LEFT OUTER JOIN (
    SELECT EMPLOYEELEAVESTRUCTUREID, SUM(TOTALLEAVE) LEAVE 
    FROM EMPLOYEELEAVE 
    WHERE IsApprove = 1  
    GROUP BY EMPLOYEELEAVESTRUCTUREID
  ) EL ON EL.EMPLOYEELEAVESTRUCTUREID = ES.ID
  LEFT OUTER JOIN EmployeePersonalDetail pd ON pd.employeeId = ei.Id
WHERE 
  ei.IsArchive = 0 
  AND ei.IsActive = 1 


";
                #region Parrameters Apply
                if (CodeF != "0_0")
                {
                    sqlText += "  and ei.Code>=@CodeF";
                }
                if (CodeT != "0_0")
                {
                    sqlText += "  and ei.Code<=@CodeT";
                }
                if (ProjectId != "0_0")
                {
                    sqlText += "  and ei.projectid=@ProjectId";
                }
                if (SectionId != "0_0")
                {
                    sqlText += "  and ei.SectionId=@SectionId";
                }
                if (DepartmentId != "0_0")
                {
                    sqlText += "  and ei.Departmentid=@DepartmentId";
                }
                if (DesignationId != "0_0")
                {
                    sqlText += "  and ei.DesignationId=@DesignationId";
                }
                if (dtpFrom != "0_0")
                {
                    sqlText += "  and ei.JoinDate>=@dtpFrom";
                }
                if (dtpTo != "0_0")
                {
                    sqlText += "  and ei.JoinDate<=@dtpTo";
                }

                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    sqlText += "  and ES.leaveyear=@Year";
                    sqlText = sqlText.Replace("@LeaveYear", leaveyear);
                }

                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId='" + EmployeeId + "'";
                }
                if (!string.IsNullOrWhiteSpace(EmpCategory))
                {
                    sqlText += "  and ei.EmpCategory='" + EmpCategory + "'";
                }
                if (!string.IsNullOrWhiteSpace(EmpJobType))
                {
                    sqlText += "  and ei.EmpJobType='" + EmpJobType + "'";
                }
                #endregion Parrameters Apply


                sqlText += @"
GROUP BY 
  ei.EmpName, 
  pd.OtherId, 
  ei.Designation, 
  ei.Department, 
  ei.Project,
  ei.EmploymentStatus, 
  ei.EmploymentType, 
  ei.JoinDate,
    pd.DateOfBirth 
order by pd.OtherId  ";
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

                #region Parrameters Apply
                if (CodeF != "0_0")
                {

                    da.SelectCommand.Parameters.AddWithValue("@CodeF", CodeF);
                }
                if (CodeT != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", CodeT);
                }
                if (DepartmentId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                }
                if (DesignationId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", DesignationId);
                }
                if (ProjectId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", ProjectId);
                }
                if (SectionId != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", SectionId);
                }
                if (dtpFrom != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpFrom", Ordinary.DateToString(dtpFrom));
                }
                if (dtpTo != "0_0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@dtpTo", Ordinary.DateToString(dtpTo));
                }
                if (!string.IsNullOrWhiteSpace(leaveyear))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Year", leaveyear);
                }
                if (!string.IsNullOrWhiteSpace(EmpCategory))
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmpCategory", EmpCategory);
                }
                if (!string.IsNullOrWhiteSpace(EmpJobType))
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmpJobType", EmpJobType);
                }

                #endregion Parrameters Apply

                da.Fill(Dt);

                Dt = Ordinary.DtColumnStringToDate(Dt, "JoinDate");






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
            return Dt;
        }



        //        public DataTable EmployeeNomineeDownload(EmployeeInfoVM vm)
        //        {
        //            #region Variables
        //            SqlConnection currConn = null;
        //            string sqlText = "";
        //            DataSet ds = new DataSet();
        //            List<EmployeeLeaveVM> vms = new List<EmployeeLeaveVM>();
        //            #endregion
        //            try
        //            {
        //                #region open connection and transaction
        //                currConn = _dbsqlConnection.GetConnection();
        //                if (currConn.State != ConnectionState.Open)
        //                {
        //                    currConn.Open();
        //                }
        //                #endregion open connection and transaction
        //                #region sql statement
        //                sqlText = @"
        //Select ei.Code,
        //ei.EmpName,
        //en.Name as NomineeName,
        //en.Relation,en.Mobile,
        //en.DateOfBirth,
        //en.BirthCertificateNo,
        //en.Address,
        //en.PostalCode,
        //en.District,
        //en.Division,
        //en.Country 
        //from  ViewEmployeeInformation ei
        //Left Join EmployeeNominee en on en.EmployeeId=ei.Id
        //Left Join EmployeePersonalDetail epd on epd.EmployeeId=ei.Id
        //where ei.IsArchive=0 and ei.IsActive=1
        //";


        //                #region Parrameters Apply
        //                if (vm.Code != "0_0")
        //                {
        //                    sqlText += "  and ei.Code>=@Code";
        //                }              
        //                #endregion Parrameters Apply

        //                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);

        //                #region Parrameters Set
        //                if (vm.Code != "0_0")
        //                {

        //                    da.SelectCommand.Parameters.AddWithValue("@Code", vm.Code);
        //                }             
        //                #endregion Parrameters Set

        //                da.Fill(ds);
        //                DataTable dt1 = ds.Tables[0];
        //                DataTable dt2 = ds.Tables[1];
        //                DataTable dt3 = ds.Tables[2];
        //                dt1 = Ordinary.DtColumnStringToDate(dt1, "JoinDate");
        //                dt1 = Ordinary.DtColumnStringToDate(dt1, "FromDate");
        //                dt1 = Ordinary.DtColumnStringToDate(dt1, "ToDate");
        //                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");
        //                dt1 = Ordinary.DtColumnStringToDate(dt1, "RejectDate");


        //                ds = new DataSet();
        //                ds.Tables.Add(dt1.Copy());
        //                ds.Tables.Add(dt2.Copy());
        //                ds.Tables.Add(dt3.Copy());


        //                #endregion
        //            }
        //            #region catch
        //            catch (SqlException sqlex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
        //            }
        //            #endregion
        //            #region finally
        //            finally
        //            {
        //                if (currConn != null)
        //                {
        //                    if (currConn.State == ConnectionState.Open)
        //                    {
        //                        currConn.Close();
        //                    }
        //                }
        //            }
        //            #endregion
        //            return ds;
        //        }

        public DataTable EmployeePromotionValue(EmployeeInfoVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            DataTable dtemp = new DataTable();
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

                string query = "Select id From EmployeeInfo where Code='" + vm.CodeF + "'";
                SqlDataAdapter dae = new SqlDataAdapter(query, currConn);
                dae.Fill(dtemp);
                string EmployeeId = dtemp.Rows[0]["id"].ToString();

                #region SqlText
                sqlText = @"
                        SELECT dg.Name as NewDesignation,
                        [PromotionDate]
                        ,( SELECT top 1 dg.Name  
                        FROM EmployeePromotion ep
                        Left Outer Join Designation dg on dg.Id = ep.DesignationId
                        where EmployeeId='" + EmployeeId + "' and IsCurrent!=1 order by PromotionDate desc) as PreviousDesignation";
                sqlText += "  FROM EmployeePromotion ep  Left Outer Join Designation dg on dg.Id = ep.DesignationId   where EmployeeId='" + EmployeeId + "' and IsCurrent=1";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.Fill(dt);

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
            return dt;
        }

        public DataTable EmployeeIncrementValue(EmployeeInfoVM vm)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            DataTable dtemp = new DataTable();
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

                string query = "Select id From EmployeeInfo where Code='" + vm.CodeF + "'";
                SqlDataAdapter dae = new SqlDataAdapter(query, currConn);
                dae.Fill(dtemp);
                string EmployeeId = dtemp.Rows[0]["id"].ToString();

                #region SqlText
                sqlText = @"
                       SELECT
                            IncrementDate,
                            MAX(CASE WHEN SalaryType = 'Medical' THEN Amount ELSE 0.00 END) AS Medical,
                            MAX(CASE WHEN SalaryType = 'Conveyance' THEN Amount ELSE 0.00 END) AS Conveyance,
                            MAX(CASE WHEN SalaryType = 'HouseRent' THEN Amount ELSE 0.00 END) AS HouseRent,
	                        MAX(CASE WHEN SalaryType = 'Basic' THEN Amount ELSE 0.00 END) AS Basic,
                            MAX(CASE WHEN SalaryType = 'Gross' THEN Amount ELSE 0.00 END) AS Gross
                        FROM
                            EmployeeSalaryStructureDetail
                        WHERE
                            EmployeeId = '" + EmployeeId + "' GROUP BY IncrementDate ORDER BY IncrementDate";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.Fill(dt);

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
            return dt;
        }
    }
}
