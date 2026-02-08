using Excel;
using SymOrdinary;
using SymServices.Common;

using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SymServices.WWF
{
    public class PFDetailDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        //==================SelectEmployeeList=================
        /// <summary>
        /// Retrieves a list of Employee List from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Employee List.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="PFDetailVM"/> representing the Employee List matching the criteria.</returns>
        public List<PFDetailVM> SelectEmployeeList(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                //                sqlText = @"
                //                            SELECT  
                //                            pfd.Id
                //                            ,pfd.PFHeaderId
                //                            ,pfd.EmployeeId
                //                            ,fyd.PeriodName
                //                            ,ve.EmpName 
                //                            ,ve.Code 
                //                            ,pf.Code PFHeaderCode
                //                            ,ve.Designation
                //                            ,ve.Department
                //                            ,ve.Section
                //                            ,ve.Project
                //                            ,pfd.BasicSalary
                //                            ,pfd.GrossSalary
                //                            ,pfd.EmployeePFValue
                //                            ,pfd.EmployeerPFValue
                //                            FROM PFDetails pfd
                //                            LEFT OUTER JOIN PFHeader pf ON pf.Id=pfd.PFHeaderId 
                //";
                //                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                //                sqlText += " LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfd.EmployeeId=ve.EmployeeId";
                //                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0 and fyd.PeriodName is not null
                //";
                sqlText = @"
            SELECT  
            pfd.Id
            ,pfd.PFHeaderId
            ,pfd.EmployeeId
            ,fyd.PeriodName
            ,ve.EmpName 
            ,ve.Code 
            ,pf.Code PFHeaderCode
            ,ve.Designation
            ,ve.Department
            ,ve.Section
            ,ve.Project
            ,pfd.BasicSalary
            ,pfd.GrossSalary
            ,pfd.EmployeePFValue
            ,pfd.EmployeerPFValue
            FROM PFDetails pfd
            LEFT OUTER JOIN PFHeader pf ON pf.Id = pfd.PFHeaderId 
";

                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId = fyd.Id";
                sqlText += " LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfd.EmployeeId = ve.EmployeeId";
                sqlText += @" WHERE 1=1 AND pfd.IsArchive = 0 AND fyd.PeriodName IS NOT NULL ";

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
                sqlText += @" ORDER BY ve.Department, ve.EmpName desc";

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
                    vm = new PFDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.PFHeaderCode = dr["PFHeaderCode"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.PeriodName = dr["PeriodName"].ToString();
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.EmployeePFValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["EmployeerPFValue"]);
                    //vm.TotalPF = vm.EmployeePFValue + vm.EmployeerPFValue;
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



        //==================SelectFiscalPeriod=================
        /// <summary>
        /// Retrieves a list of Fiscal Period from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Fiscal Period.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="PFDetailVM"/> representing the Fiscal Period matching the criteria.</returns>
        public List<PFDetailVM> SelectFiscalPeriod(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                string hrmDB = _dbsqlConnection.GetConnection().Database;
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT pfd.FiscalYearDetailId
,p.Name ProjectName
,p.Id ProjectId
,fyd.PeriodName
,fyd.PeriodStart
,fyd.PeriodEnd
,pfd.Post 
,sum(pfd.EmployeePFValue) EmployeePFValue
,sum(pfd.EmployeerPFValue) EmployerPFValue
,sum(pfd.EmployeePFValue)+sum(pfd.EmployeerPFValue) TotalPF
,ISNULL(pfd.IsBankDeposited,0) IsBankDeposited
FROM PFDetails pfd
";

                sqlText += " LEFT OUTER JOIN [dbo].Project p ON pfd.ProjectId=p.Id";
                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfd.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0
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
                sqlText += "  GROUP BY p.Name,p.Id, pfd.FiscalYearDetailId, fyd.PeriodName, fyd.PeriodStart, fyd.PeriodEnd, pfd.Post, ISNULL(pfd.IsBankDeposited,0) ";
                sqlText += " ORDER BY fyd.PeriodStart DESC";

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
                    vm = new PFDetailVM();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.ProjectName = dr["ProjectName"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.TotalEmployeeValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.TotalEmployerValue = Convert.ToDecimal(dr["EmployerPFValue"]);
                    vm.TotalPF = Convert.ToDecimal(dr["TotalPF"]);
                    vm.PeriodEnd = dr["PeriodEnd"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);

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


        //==================SelectFiscalPeriod=================
        /// <summary>
        /// Retrieves a list of Fiscal Period Header from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Fiscal Period Header.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="PFDetailVM"/> representing the Fiscal Period Header matching the criteria.</returns>
        public List<PFHeaderVM> SelectFiscalPeriodHeader(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFHeaderVM> VMs = new List<PFHeaderVM>();
            PFHeaderVM vm;
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
                string hrmDB = _dbsqlConnection.GetConnection().Database;
                #region sql statement
                #region SqlText
                sqlText = @"
SELECT 
pfd.Id
,pfd.Code
,pfd.FiscalYearDetailId
,p.Name ProjectName
,p.Id ProjectId
,fyd.PeriodName
,fyd.PeriodStart
,pfd.Post 
,pfd.WWFValue 
,pfd.TotalProfitValue TotalPF


FROM WWFHeader pfd
";

                sqlText += "  LEFT OUTER JOIN [dbo].Project p ON pfd.ProjectId=p.Id";
                sqlText += "  LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0
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
                //  sqlText += "  GROUP BY p.Name,p.Id, pfd.FiscalYearDetailId, fyd.PeriodName, fyd.PeriodStart, fyd.PeriodEnd, pfd.Post ";
                sqlText += " ORDER BY fyd.PeriodStart DESC";

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
                    vm = new PFHeaderVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.ProjectName = dr["ProjectName"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = dr["PeriodStart"].ToString();
                    vm.WWFValue = Convert.ToDecimal(dr["WWFValue"]);
                    vm.TotalPF = Convert.ToDecimal(dr["TotalPF"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);

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
        /// <summary>
        /// Retrieves a list of PF Details from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific PF Details.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="PFDetailVM"/> representing the PF Details matching the criteria.</returns>
        public List<PFDetailVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
                            pfd.Id
                            ,pfd.PFHeaderId
                            ,pfd.FiscalYearDetailId
                            ,pfd.PFStructureId
                            ,pfd.ProjectId
                            ,pfd.DepartmentId
                            ,pfd.SectionId
                            ,pfd.DesignationId
                            ,pfd.EmployeeId
                            ,pfd.EmployeePFValue
                            ,pfd.EmployeerPFValue
                            ,pfd.BasicSalary
                            ,pfd.GrossSalary
                            ,pfd.IsDistribute
                            ,pfd.Post
                            ,ISNULL(pfd.IsBankDeposited,0) IsBankDeposited

                            ,pfd.Remarks
                            ,pfd.IsActive
                            ,pfd.IsArchive
                            ,pfd.CreatedBy
                            ,pfd.CreatedAt
                            ,pfd.CreatedFrom
                            ,pfd.LastUpdateBy
                            ,pfd.LastUpdateAt
                            ,pfd.LastUpdateFrom
                            ,pfh.code


                            FROM PFDetails pfd
                            left outer join  PFHeader pfh on pfh.id=pfd.PFHeaderId
                            WHERE  1=1
                            ";
                if (Id > 0)
                {
                    sqlText += @" and pfd.Id=@Id";
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
                    vm = new PFDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.PFStructureId = dr["PFStructureId"].ToString();
                    vm.PFHeaderId = dr["PFHeaderId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeePFValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["EmployeerPFValue"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);

                    vm.IsDistribute = Convert.ToBoolean(dr["IsDistribute"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);



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


        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of PF Details Header from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific PF Details Header.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="PFDetailVM"/> representing the PF Details Header matching the criteria.</returns>
        public List<PFDetailVM> SelectAllHeader(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<PFDetailVM> VMs = new List<PFDetailVM>();
            PFDetailVM vm;
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
 
	   [Id]
      ,[Code]
      ,[FiscalYearDetailId]
      ,[ProjectId]
      ,[EmployeePFValue]
      ,[EmployeerPFValue]
      ,[Post]
      ,[Remarks]
      ,[IsActive]
      ,[IsArchive]
      ,[CreatedBy]
      ,[CreatedAt]
      ,[CreatedFrom]
      ,[LastUpdateBy]
      ,[LastUpdateAt]
      ,[LastUpdateFrom]

FROM PFHeader pfd 
WHERE  1=1
";
                if (Id > 0)
                {
                    sqlText += @" and pfd.Id=@Id";
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
                    vm = new PFDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.PFStructureId = dr["PFStructureId"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.DepartmentId = dr["DepartmentId"].ToString();
                    vm.SectionId = dr["SectionId"].ToString();
                    vm.DesignationId = dr["DesignationId"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeePFValue = Convert.ToDecimal(dr["EmployeePFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["EmployeerPFValue"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);

                    vm.IsDistribute = Convert.ToBoolean(dr["IsDistribute"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsBankDeposited = Convert.ToBoolean(dr["IsBankDeposited"]);



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



        ////==================PFProcess =================
        public string[] PFProcess(string FiscalYearDetailId, string ProjectId, string chkAll, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFProcess"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSchedule1SalaryMonthly"); }
                #endregion open connection and transaction



                #region Checkpoint

                #region Post Check

                PFDetailVM varPFDetailVM = new PFDetailVM();
                string[] cFields = { "pfd.FiscalYearDetailId", "pfd.ProjectId" };
                string[] cValues = { FiscalYearDetailId, ProjectId };

                varPFDetailVM = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                if (varPFDetailVM != null && varPFDetailVM.Post)
                {
                    retResults[1] = "PF Already Posted for this Month! Can't Process!";
                    throw new ArgumentNullException("", retResults[1]);
                }

                CommonDAL _cDal = new CommonDAL();



                #endregion

                #endregion



                if (chkAll == "true")
                {
                    string fyid = @"Select Id from dbo.Project";
                    SqlCommand cmdfyid = new SqlCommand(fyid, currConn, transaction);
                    SqlDataAdapter da = new SqlDataAdapter(cmdfyid);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            ProjectId = row["Id"].ToString();

                            retResults = PFProcessSingle(FiscalYearDetailId, ProjectId, chkAll, auditvm, null, null);
                        }
                    }
                }
                else
                {
                    retResults = PFProcessSingle(FiscalYearDetailId, ProjectId, chkAll, auditvm, null, null);
                }
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
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

        public string[] PFProcessSingle(string FiscalYearDetailId, string ProjectId, string chkAll, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFProcess"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSchedule1SalaryMonthly"); }
                #endregion open connection and transaction



                #region Checkpoint

                #region Post Check

                PFDetailVM varPFDetailVM = new PFDetailVM();
                string[] cFields = { "pfd.FiscalYearDetailId", "pfd.ProjectId" };
                string[] cValues = { FiscalYearDetailId, ProjectId };

                varPFDetailVM = SelectAll(0, cFields, cValues, currConn, transaction).FirstOrDefault();

                if (varPFDetailVM != null && varPFDetailVM.Post)
                {
                    retResults[1] = "PF Already Posted for this Month! Can't Process!";
                    throw new ArgumentNullException("", retResults[1]);
                }

                CommonDAL _cDal = new CommonDAL();



                #endregion

                #endregion


                #region Single Project
                sqlText = " ";
                sqlText = @"
                        insert into PFHeader (
                            Code       
                            ,[Id]
                            ,[FiscalYearDetailId]
                            ,[ProjectId]
                            ,[EmployeePFValue]
                            ,[EmployeerPFValue]
                            ,[Post]
                            ,[Remarks]
                            ,[IsActive]
                            ,[IsArchive]
                            ,[CreatedBy]
                            ,[CreatedAt]
                            ,[CreatedFrom]
                            ,[LastUpdateBy]
                            ,[LastUpdateAt]
                            ,[LastUpdateFrom]
                            )
                            select 
                            @code
                            ,@Id
                            ,@FiscalYearDetailId
                            ,@ProjectId
                            ,sum(ei.BasicSalary*.10)PFValue
                            ,sum(ei.BasicSalary*.10)EmployeerPF
                            ,@Post
                            ,'-' Remarks
                            ,@IsActive
                            ,@IsArchive
                            ,@CreatedBy
                            ,@CreatedAt
                            ,@CreatedFrom
                            ,@LastUpdateBy
                            ,@LastUpdateAt
                            ,@LastUpdateFrom
                            FROM ViewEmployeeInformation ei
                ";

                string deleteCurrent =
                    @" DELETE FROM PFHeader  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId ";

                SqlCommand cmd = new SqlCommand(deleteCurrent, currConn, transaction);
                cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.ExecuteNonQuery();

                int nextId = _cDal.NextId("PFHeader", currConn, transaction);

                string NewCode = new CommonDAL().CodeGenerationPF("PF", "PFContribution", DateTime.Now.ToString(), currConn, transaction);

                string code = NewCode;

                //////string code = "PFC-" + nextId.ToString().PadLeft(4, '0');

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@Id", nextId);
                cmd.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@Post", false);

                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@IsArchive", false);
                cmd.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmd.Parameters.AddWithValue("@LastUpdateBy", "");
                cmd.Parameters.AddWithValue("@LastUpdateAt", "");
                cmd.Parameters.AddWithValue("@LastUpdateFrom", "");
                cmd.ExecuteNonQuery();

                ResultVM resultvm = InsertPFDetails(FiscalYearDetailId, ProjectId, currConn, transaction, nextId.ToString());
                #endregion

                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "PFDetail Saved Successfully.";
                }
                #endregion Commit

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
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


        /// <summary>
        /// Inserts a new PF Details into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the PF Details data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertPFDetails").
        /// </returns>
        private ResultVM InsertPFDetails(string FiscalYearDetailId, string ProjectId, SqlConnection currConn,
            SqlTransaction transaction, string headerId)
        {
            try
            {
                string sqlText;
                int transResult;

                #region Save Data

                #region SqlText

                sqlText = "";
                sqlText = @"
                --declare @FiscalYearDetailId as varchar(20)
                declare @FiscalYear as varchar(20)
                declare @FiscalYearId as varchar(20)
                --set @FiscalYearDetailId=9
                select @FiscalYear=[year],@FiscalYearId=FiscalYearId from FiscalYearDetail where Id=@FiscalYearDetailId";
                sqlText += @" SELECT @FiscalYear,@FiscalYearId";

                sqlText += @" DELETE FROM PFDetails  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId
------------------------------------------------
------------------------------------------------
                declare @maxId as int
                set @maxId = 0 
                select @maxId=isnull(max(Id),0) from PFDetails


                DBCC CHECKIDENT ('PFDetails', RESEED, @maxId)
                ------------------------------------------------
                ------------------------------------------------
                INSERT INTO PFDetails 
                (
                FiscalYearDetailId
                ,PFStructureId
                ,ProjectId
                ,DepartmentId
                ,SectionId
                ,DesignationId
                ,EmployeeId
                ,EmployeePFValue,EmployeerPFValue
                ,IsDistribute
                ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom
                ,BasicSalary,GrossSalary
                ,Post
                ,PFHeaderId
                ) 
                select 
                1
                ,1
                ,Vei.ProjectId
                ,Vei.DepartmentId
                ,Vei.SectionId
                ,Vei.DesignationId
                ,Vei.EmployeeId
                ,(Vei.BasicSalary *.10) PFValue
                ,(Vei.BasicSalary *.10) PFValuee
                ,0
                ,Vei.Remarks
                ,Vei.IsActive
                ,Vei.IsArchive
                ,Vei.CreatedBy
                ,Vei.CreatedAt
                ,Vei.CreatedFrom
                ,Vei.LastUpdateBy
                ,Vei.LastUpdateAt
                ,Vei.LastUpdateFrom
                ,Vei.BasicSalary
                ,Vei.GrossSalary
                ,1
                ,1
                FROM ViewEmployeeInfo Vei";
                #endregion SqlText

                #region SqlExecution

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmdUpdate.Parameters.AddWithValue("@ProjectId", ProjectId);
                cmdUpdate.Parameters.AddWithValue("@Post", false);
                cmdUpdate.Parameters.AddWithValue("@PFHeaderId", headerId);
                var exeRes = cmdUpdate.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {

                    throw new ArgumentNullException("No Data Found for Process", "");
                }

                #endregion SqlExecution

                #endregion Save Data


                ResultVM resultVm = new ResultVM();
                resultVm.Status = "Success";
                resultVm.Message = "Success";

                return resultVm;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal GetPFAmount(int FiscalYearDetailId, string EmployeeId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            decimal BonusAmount = 0;
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

                #region Update
                sqlText = " ";
                sqlText += @"
--------------------------------
            ----declare @EmployeeId as varchar(100)
            ----declare @FiscalYearDetailId as int

            ----set @EmployeeId = '1_1'
            ----set @FiscalYearDetailId = 1054
--------------------------------

                select ISNULL(sum(PFValue),0) Amount from SalaryPFDetail 
                WHERE 1=1
                AND FiscalYearDetailId=@FiscalYearDetailId
                AND EmployeeId=@EmployeeId
                ";
                if (string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText = sqlText.Replace("EmployeeId=@EmployeeId", "1=1");
                }
                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                var exeRes = cmdUpdate.ExecuteScalar();

                BonusAmount = Convert.ToDecimal(exeRes);

                #endregion Update
                #region Commit

                #endregion Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
            }
            #region catch
            catch (Exception ex)
            {
                if (Vtransaction == null) { transaction.Rollback(); }
                return BonusAmount;
            }
            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return BonusAmount;
        }

        public string[] Post(PFDetailVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " PFDetails Post"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post PFDetails"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update PFDetails set";
                    sqlText += "  Post=@Post";

                    sqlText += " where FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Post", true);
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@ProjectId", vm.ProjectId);

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EEHeadVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Post PFDetails", "Could not found any item.");
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
                    retResults[1] = "Data  Successfully Post.";
                }
                else
                {
                    retResults[1] = "Unexpected error to Post PFDetails.";
                    throw new ArgumentNullException("", "");
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
        public string[] PostHeader(PFHeaderVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = " PFDetails Post"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post PFDetails"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update PFHeader set";
                    sqlText += "  Post=@Post";

                    sqlText += @" where Id=@Id
                    update PFDetails set Post=@Post where PFHeaderId=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Post", true);

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EEHeadVM.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Post PFDetails", "Could not found any item.");
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }
                retResults[0] = "Success";
                retResults[1] = "Data  Successfully Post.";
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

        ////==================Report=================
        public DataTable Report(PFDetailVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText = @"
                            SELECT  
                            pfd.Id
                            ,pfd.EmployeeId
                            ,ve.EmpName 
                            ,ve.Code 
                            ,ve.Designation
                            ,ve.Department
                            ,ve.Section
                            ,ve.Project
                            ,pfd.EmployeePFValue
                            ,pfd.EmployeerPFValue
                            ,fyd.PeriodName
                            ,fyd.PeriodStart
                            ,fyd.PeriodEnd
                            ,pfd.FiscalYearDetailId
                            FROM PFDetails pfd
                            ";
                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId=fyd.Id";
                sqlText += "  LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfd.EmployeeId=ve.EmployeeId";
                sqlText += @" WHERE  1=1  AND pfd.IsArchive = 0
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

        //==================SelectTotalContribution=================
        public PFDetailVM SelectTotalContribution_TillMonth(int FiscalYearDetailIdTo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            PFDetailVM vm = new PFDetailVM();
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
                        --------declare @FiscalYearDetailIdTo as int
                        --------
                        --------set @FiscalYearDetailIdTo = 1042

                        SELECT
                        SUM(pfd.EmployeePFValue) TotalEmployeeContribution
                        ,SUM(pfd.EmployeerPFValue) TotalEmployerContribution

                        FROM PFDetails pfd
                        WHERE  1=1
                        AND pfd.Post = 1
                        --------AND pfd.IsDistribute = 0
                        AND pfd.FiscalYearDetailId <= @FiscalYearDetailIdTo

                        HAVING 1=1
                        AND sum(pfd.EmployeePFValue) > 0
                        AND sum(pfd.EmployeerPFValue) > 0
                        ";

                #endregion SqlText
                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFDetailVM();
                    vm.TotalEmployeeContribution = Convert.ToDecimal(dr["TotalEmployeeContribution"]);
                    vm.TotalEmployerContribution = Convert.ToDecimal(dr["TotalEmployerContribution"]);
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

        //==================SelectDetailContribution=================
        public List<PFSettlementVM> SelectDetailContribution_TillMonth(int FiscalYearDetailIdTo, string EmployeeId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText = "";



                sqlText = @"
                    --------declare @FiscalYearDetailIdTo as int
                    --------declare @EmployeeId as nvarchar(100)
                    --------
                    --------set @FiscalYearDetailIdTo = 1042
                    --------set @EmployeeId = '1_1'

                    ---------------------PF Summary Contribution--------------------
                    ----------------------------------------------------------------
                    ;WITH PFSummaryContribution AS
                    (
                    SELECT
                     pfd.ProjectId
                    ,pfd.DepartmentId
                    ,pfd.SectionId
                    ,pfd.DesignationId
                    ,pfd.EmployeeId
                    ,SUM(pfd.EmployeePFValue) EmployeeTotalContribution
                    ,SUM(pfd.EmployeerPFValue) EmployerTotalContribution
                    ,0 EmployerProfit
                    ,0 EmployeeProfit

                    FROM PFDetails pfd
                    WHERE  1=1
                    AND pfd.Post = 1
                    --------AND pfd.IsDistribute = 0
                    AND pfd.FiscalYearDetailId <= @FiscalYearDetailIdTo
                    AND pfd.EmployeeId=@EmployeeId

                    GROUP BY pfd.EmployeeId
                    ,pfd.ProjectId
                    ,pfd.DepartmentId
                    ,pfd.SectionId
                    ,pfd.DesignationId

                    HAVING 1=1
                    AND SUM(pfd.EmployeePFValue) > 0
                    AND SUM(pfd.EmployeerPFValue) > 0

                    UNION ALL

                    SELECT
                     '' ProjectId
                    ,'' DepartmentId
                    ,'' SectionId
                    ,'' DesignationId
                    ,pfd.EmployeeId
                    ,pfd.EmployeeContribution EmployeeTotalContribution
                    ,pfd.EmployerContribution EmployerTotalContribution
                    ,pfd.EmployerProfit
                    ,pfd.EmployeeProfit
                    FROM EmployeePFOpeinig pfd
                    WHERE  1=1
                    AND pfd.EmployeeId=@EmployeeId
                    )


---------------------PFStatus-----------------------------------
----------------------------------------------------------------
                    , PFStatus AS
                    (
                    SELECT 
                    ROW_NUMBER() OVER (PARTITION BY pf.EmployeeId ORDER BY pf.FiscalYearDetailId ASC) AS RowNumber
                    , pf.* 
                    FROM PFDetails pf
                    WHERE 1=1
                    AND pf.EmployeePFValue > 0
                    )


                    , PFStart AS
                    (
                    SELECT * FROM PFStatus
                    WHERE RowNumber = 1
                    )


                    ---------------------PFEndStatus--------------------------------
                    ----------------------------------------------------------------
                    , PFEndStatus AS
                    (
                    SELECT 

                    ROW_NUMBER() OVER (PARTITION BY pf.EmployeeId ORDER BY pf.FiscalYearDetailId DESC) AS RowNumber
                    , pf.* 

                    from PFDetails pf

                    WHERE 1=1

                    AND pf.EmployeePFValue > 0
                    )

                    , PFEnd AS
                    (
                    SELECT * FROM PFEndStatus
                    WHERE RowNumber = 1
                    )


----------------------------------------------------------------
----------------------------------------------------------------
                SELECT 
                 ve.EmpName 
                ,ve.Code 
                ,ve.Designation
                ,ve.Department
                ,ve.Section
                ,ve.Project
                ,ve.JoinDate
                ,ve.ResignDate LeftDate
                , fyd.PeriodStart PFStartDate
                , fyd.PeriodName

                , fydEnd.PeriodEnd PFEndDate
                , fydEnd.PeriodName PFEndPeriodName

                ,pfsc.ProjectId
                ,pfsc.DepartmentId
                ,pfsc.SectionId
                ,pfsc.DesignationId
                ,pfsc.EmployeeId
                ,IsNull(pfsc.EmployeeTotalContribution,0)   EmployeeTotalContribution
                ,IsNull(pfsc.EmployerTotalContribution,0)  EmployerTotalContribution
                ,IsNull(pd.EmployerProfit,0)+pfsc.EmployerProfit  EmployerProfit 
                ,IsNull(pd.EmployeeProfit,0)+pfsc.EmployeeProfit EmployeeProfit

                FROM PFSummaryContribution pfsc
                LEFT OUTER JOIN PFStart pfs ON pfs.EmployeeId = pfsc.EmployeeId
                LEFT OUTER JOIN PFEnd pfe ON pfe.EmployeeId = pfsc.EmployeeId
                ";
                sqlText += " LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON pfsc.EmployeeId=ve.EmployeeId";
                sqlText += " LEFT OUTER JOIN [dbo].FiscalYearDetail fyd ON pfs.FiscalYearDetailId = fyd.Id";
                sqlText += " LEFT OUTER JOIN [dbo].FiscalYearDetail fydEnd ON pfe.FiscalYearDetailId = fydEnd.Id";
                sqlText += "  LEFT OUTER JOIN ProfitDistributionNew pd on pd.EmployeeId=ve.EmployeeId";


                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += " where pfsc.EmployeeId=@EmployeeId";
                }
                sqlText += " ORDER BY ve.Code";
                #endregion SqlText
                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);

                objComm.Parameters.AddWithValue("@FiscalYearDetailIdTo", FiscalYearDetailIdTo);
                objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);


                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFSettlementVM();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.PFStartDate = Ordinary.StringToDate(dr["PFStartDate"].ToString());
                    vm.PFEndDate = Ordinary.StringToDate(dr["PFEndDate"].ToString());
                    vm.ProjectId = Convert.ToString(dr["ProjectId"]);
                    vm.DepartmentId = Convert.ToString(dr["DepartmentId"]);
                    vm.SectionId = Convert.ToString(dr["SectionId"]);
                    vm.DesignationId = Convert.ToString(dr["DesignationId"]);
                    vm.EmployeeId = Convert.ToString(dr["EmployeeId"]);
                    vm.EmployeeTotalContribution = Convert.ToDecimal(dr["EmployeeTotalContribution"]);
                    vm.EmployerTotalContribution = Convert.ToDecimal(dr["EmployerTotalContribution"]);
                    vm.EmployeeProfitValue = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfitValue = Convert.ToDecimal(dr["EmployerProfit"]);

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


        public DataTable PFReportSummaryDetail(string fydid, string rType, string ProjectId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                FiscalYearDetailVM dVM = new FiscalYearDetailVM();
                dVM = new FiscalYearDAL().SelectAll_FiscalYearDetail(Convert.ToInt32(fydid)).FirstOrDefault();
                string PeriodEnd = dVM.PeriodEnd;



                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction


                #region sql statement
                if (rType == "Summary")
                {
                    sqlText = @" 
----select @TransactionDate TransactionDate, 'Contribution' Particular,'Cont-001' AccountCode, sum(EmployeePFValue+EmployeerPFValue) DebitAmount, 
----0 CreditAmount, '' Remarks 
----from PFDetails pfd
----where pfd.FiscalYearDetailId = @FiscalYearDetailId
----And pfd.ProjectId = @ProjectId
----
----
----union all
----select @TransactionDate, 'PF Account', 'PF-001', 0, sum(EmployeePFValue+EmployeerPFValue), ''  from PFDetails
----where FiscalYearDetailId = @FiscalYearDetailId
----And ProjectId = @ProjectId

select distinct f.PeriodName, p.Name ProjectName, sum( d.EmployeePFValue) MemberContribution,sum(d.EmployeerPFValue) CompanyContribution ,sum( d.EmployeePFValue)+sum(d.EmployeerPFValue) Total from PFDetails d
left outer join Project p on d.ProjectId=p.Id
left outer join FiscalYearDetail f on d.FiscalYearDetailId=f.Id
where FiscalYearDetailId = @FiscalYearDetailId
And ProjectId = @ProjectId
group by  f.PeriodName, p.Name

";
                }
                else
                {
                    sqlText = @" 

select @TransactionDate TransactionDate, ve.EmpName Particular, ve.Code AccountCode, (EmployeePFValue+EmployeerPFValue) DebitAmount, 0 CreditAmount, '' Remarks 
from PFDetails pfd
left outer join HRMDB.ViewEmployeeInformation ve on pfd.EmployeeId = ve.EmployeeId

where pfd.FiscalYearDetailId = @FiscalYearDetailId
And pfd.ProjectId = @ProjectId

union all
select @TransactionDate, 'PF Account', 'PF-001', 0, sum(EmployeePFValue+EmployeerPFValue), ''  
from PFDetails
where FiscalYearDetailId = @FiscalYearDetailId
And ProjectId = @ProjectId

";

                }

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@FiscalYearDetailId", fydid);
                objComm.Parameters.AddWithValue("@TransactionDate", PeriodEnd);
                objComm.Parameters.AddWithValue("@ProjectId", ProjectId);



                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.SelectCommand.Transaction = transaction;


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");


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

        public DataTable PFEmployersProvisionsReport(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                    select 
                    ts.PFHeaderId
                    ,PeriodName
                    ,Project
                    ,Section
                    ,SectionOrderNo
                    ,ts.BasicAmount
                    ,ts.EmployeePFValue
                    ,ts.EmployeerPFValue
                    ,ts.TotalAmount
                      from (
                    select distinct ts.PFHeaderId
                    ,fs.PeriodName
                    ,p.Name Project
                    ,st.Name Section
                    ,st.OrderNo SectionOrderNo
                    ,count(ts.employeeid)TotalEmployee
                    ,sum(isnull(ts.BasicSalary,0))BasicAmount
                    ,sum(isnull(ts.EmployeePFValue,0))EmployeePFValue
                    ,sum(isnull(ts.EmployeerPFValue,0))EmployeerPFValue
                    ,sum(isnull(ts.EmployeePFValue,0)+isnull(ts.EmployeerPFValue,0))TotalAmount
                    ,0 EmployeeContribution
                    ,0 EmployerContribution
                    from PFDetails ts 
                     LEFT OUTER JOIN PFHeader ph on ph.Id=ts.PFHeaderId
                     LEFT OUTER JOIN [Section] st on ts.SectionId = st.Id 
                     LEFT OUTER JOIN [Project] p on ts.ProjectId = p.Id  
                     LEFT OUTER JOIN [FiscalYearDetail] fs on ts.FiscalYearDetailId = fs.Id 
                     LEFT OUTER JOIN [Designation] dg on ts.DesignationId = dg.Id

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

                sqlText += @" 
group by  ts.PFHeaderId
,fs.PeriodName  ,p.Name  ,st.Name  ,st.OrderNo 
) as ts
order by SectionOrderNo ";

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

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion
            }

            #region catch

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

        public DataTable ExportExcelFilePF(string Filepath, string FileName, string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid, string Orderby, string BranchId)
        {
            DataTable dt = new DataTable();
            string[] results = new string[6];
            try
            {
                #region Fiscal Period
                FiscalYearDAL fdal = new FiscalYearDAL();
                FiscalYearDetailVM fyDVM = new FiscalYearDetailVM();
                fyDVM = fdal.FYPeriodDetail(fid, null, null).FirstOrDefault();
                var fname = fyDVM.PeriodName;

                string PeriodEnd = fyDVM.PeriodEnd;
                string PeriodStart = fyDVM.PeriodStart;
                #endregion

                #region Variables
                SqlConnection currConn = null;
                SqlConnection currConnpf = null;
                string sqlText = "";
                int j = 2;
                #endregion

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                currConnpf = _dbsqlConnection.GetConnection();
                if (currConnpf.State != ConnectionState.Open)
                {
                    currConnpf.Open();
                }

                #endregion open connection and transaction

                #region sql statement
                sqlText = @"
                 Select ve.Code EmpCode, ve.EmpName, JoinDate, 0 GradeSL,0 
                 Grade,Designation,Department,ve.UnitName,ve.BasicSalary, ISNULL((ve.BasicSalary*.1),0) Amount,0 FYDId 
                 from ViewEmployeeInformation ve Left Outer Join PFDetails pd on pd.EmployeeId=ve.EmployeeId and FiscalYearDetailId=@FiscalYearDetailId  where 1=1 AND ve.IsActive=1 AND ve.IsArchive=0 ";

                //if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                //    sqlText += @" and ve.ProjectId='" + ProjectId + "'";
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                    sqlText += @" and ve.DepartmentId='" + DepartmentId + "'";
                //if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                //    sqlText += @" and ve.SectionId='" + SectionId + "'";
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                    sqlText += @" and ve.DesignationId='" + DesignationId + "'";
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                    sqlText += @" and ve.Code >='" + CodeF + "'";
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                    sqlText += @" and ve.Code<='" + CodeT + "'";
                if (BranchId != "0_0" && CodeT != "0" && BranchId != "" && BranchId != "null" && BranchId != null)
                    sqlText += @" and ve.BranchId='" + BranchId + "'";

                if (Orderby == "CODE")
                    sqlText += " ORDER BY  EmpCode";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;

                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", fid);

                da.Fill(dt);
                dt.Columns.Add("Fiscal Period");
                dt.Columns.Add("Salary Period");
                // dt.Columns.Add("Type");
                dt.Columns.Remove("GradeSl");
                dt.Columns.Remove("JoinDate");

                foreach (DataRow row in dt.Rows)
                {
                    row["Fiscal Period"] = fname;
                    row["Salary Period"] = fname;
                    // row["Type"] = "";
                    row["FYDId"] = fid;
                    // row["EDId"] = 0;
                }
                #endregion

                #region Value Round

                string[] columnNames = { "Amount" };

                dt = Ordinary.DtValueRound(dt, columnNames);

                #endregion
            }
            catch (Exception ex)
            {
                results[4] = ex.Message.ToString(); //catch ex
                //return results;
                throw ex;
            }
            return dt;
        }


        /// <summary>
        /// Inserts a new PF Details Header into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the PF Details Header data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertPFDetailsHeader").
        /// </returns>
        private string[] InsertPFHeader(int FiscalYearDetailId, string PId, SqlConnection currConn, SqlTransaction transaction, ShampanIdentityVM auditvm)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFProcess"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
            SqlCommand cmd = null;

            try
            {

                #region Save Data

                #region SqlText

                sqlText = @"insert into PFHeader (Id,Code,[FiscalYearDetailId],[ProjectId],[EmployeePFValue],[EmployeerPFValue],[Post],
                            [Remarks],[IsActive],[IsArchive],[CreatedBy],[CreatedAt],[CreatedFrom],[LastUpdateBy],[LastUpdateAt],[LastUpdateFrom],BranchId) 
                            values (@Id,@Code ,@FiscalYearDetailId,@ProjectId,@EmployeePFValue,@EmployeerPFValue,@Post,@Remarks,@IsActive,@IsArchive,
                            @CreatedBy,@CreatedAt,@CreatedFrom,@LastUpdateBy,@LastUpdateAt,@LastUpdateFrom,@BranchId)  
                          ";

                string deleteCurrent = @" DELETE FROM PFHeader  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId and BranchId=@BranchId";

                cmd = new SqlCommand(deleteCurrent, currConn, transaction);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@ProjectId", PId);
                cmd.Parameters.AddWithValue("@BranchId", auditvm.BranchId);
                cmd.ExecuteNonQuery();

                int nextId = _cDal.NextId("PFHeader", currConn, transaction);

                string NewCode = new CommonDAL().CodeGenerationPF("PF", "PFContribution", DateTime.Now.ToString(), currConn, transaction);
                string code = NewCode;

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@Id", nextId);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@ProjectId", PId);
                cmd.Parameters.AddWithValue("@EmployeePFValue", 0);
                cmd.Parameters.AddWithValue("@EmployeerPFValue", 0);
                cmd.Parameters.AddWithValue("@Post", false);
                cmd.Parameters.AddWithValue("@Remarks", "-");
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@IsArchive", false);
                cmd.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                cmd.Parameters.AddWithValue("@LastUpdateBy", "");
                cmd.Parameters.AddWithValue("@LastUpdateAt", "");
                cmd.Parameters.AddWithValue("@LastUpdateFrom", "");
                cmd.Parameters.AddWithValue("@BranchId", auditvm.BranchId);

                var exeRes = cmd.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("No Data Found for Process", "");
                }

                #endregion SqlExecution

                #endregion Save Data

                #region Commit
                // transaction.Commit();
                retResults[0] = "Success";
                retResults[1] = "PF Header Saved Successfully.";
                #endregion Commit

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                if (transaction == null) { transaction.Rollback(); }
                return retResults;
            }
            #endregion
            return retResults;
        }

        /// <summary>
        /// Updates an existing PF Details Header record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the updated PF Details Header information.</param>
        /// <param name="VcurrConn">Optional external SQL connection. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">Optional external SQL transaction. If null, a new transaction will be created and committed.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Updated record ID,  
        /// [3] = The executed SQL query,  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("PFDetailsHeader").
        /// </returns>
        private string[] UpdatePFHeader(int FiscalYearDetailId, string PId, string BranchId, SqlConnection currConn, SqlTransaction transaction, ShampanIdentityVM auditvm)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFProcess"; //Method Name
            int transResult = 0;
            string sqlText = "";
            #endregion
            SqlCommand cmd = null;

            try
            {
                #region Update Data

                string id = @"Select SUM(pd.EmployeePFValue) EmployeePFValue, SUM(pd.EmployeerPFValue) EmployeerPFValue from PFDetails pd
                                Left Outer Join PFHeader ph on ph.Id=pd.PFHeaderId 
                                where ph.BranchId=@BranchId and pd.FiscalYearDetailId=@FiscalDetailYearId and pd.ProjectId=@ProjectId";
                SqlCommand cmdid = new SqlCommand(id, currConn, transaction);
                cmdid.Parameters.AddWithValue("@ProjectId", PId);
                cmdid.Parameters.AddWithValue("@FiscalDetailYearId", FiscalYearDetailId);
                cmdid.Parameters.AddWithValue("@BranchId", BranchId);
                SqlDataAdapter adapterid = new SqlDataAdapter(cmdid);
                DataTable dtid = new DataTable();
                adapterid.Fill(dtid);

                #region SqlText

                sqlText = @"Update PFHeader set [EmployeePFValue]=@EmployeePFValue,[EmployeerPFValue] =@EmployeerPFValue where FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId ";
                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@ProjectId", PId);
                cmd.Parameters.AddWithValue("@EmployeePFValue", dtid.Rows[0]["EmployeePFValue"].ToString());
                cmd.Parameters.AddWithValue("@EmployeerPFValue", dtid.Rows[0]["EmployeerPFValue"].ToString());



                var exeRes = cmd.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("No Data Found for Process", "");
                }

                #endregion SqlExecution

                #endregion Save Data

                #region Commit
                // transaction.Commit();
                retResults[0] = "Success";
                retResults[1] = "PF Header Update Successfully.";
                #endregion Commit

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                if (transaction == null) { transaction.Rollback(); }
                return retResults;
            }
            #endregion
            return retResults;
        }


        public string[] ImportExcelFile(string fullPath, string fileName, ShampanIdentityVM avm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, int FYDId = 0, string PId = "")
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
            retResults[5] = "Salary Provident Fund"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion
            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            FiscalYearDAL fydal = new FiscalYearDAL();
            FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();

            FYDVM = fydal.FYPeriodDetail(FYDId, currConn, transaction).FirstOrDefault();


            #region try
            try
            {
                #region Reading Excel
                DataSet ds = new DataSet();
                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (fileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];
                reader.Close();
                #endregion Reading Excel
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
                    transaction = currConn.BeginTransaction("PFHeader");
                }
                #endregion open connection and transaction

                PFDetailVM vm = new PFDetailVM();

                retResults = InsertPFHeader(FYDVM.Id, PId, currConn, transaction, avm);

                // Get Last PF Header Id
                string sqlTextid = "select isnull(max(cast(id as int)),0) FROM  PFHeader";
                SqlCommand cmd2 = new SqlCommand(sqlTextid, currConn);
                cmd2.Transaction = transaction;
                var exeRes = cmd2.ExecuteScalar();
                int nextId = Convert.ToInt32(exeRes);


                if (retResults[0] == "Success")
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, null, null).FirstOrDefault();

                        if (empVM == null)
                        {
                            throw new ArgumentNullException("Employee Code " + item["EmpCode"].ToString() + " Not in System", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                        }
                        else
                        {
                            if (FYDVM == null)
                            {
                                throw new ArgumentNullException("Fiscal Period" + item["FYDId"].ToString() + " Not in System", "Fiscal Period " + item["FYDId"].ToString() + " Not in System");
                            }
                            else
                            {
                                if (!Ordinary.IsNumeric(item["Amount"].ToString()))
                                {
                                    throw new ArgumentNullException("Please input the Numeric Value in Amount", "Please input the Numeric Value in Amount");
                                }
                                else
                                {
                                    vm.PFHeaderId = nextId.ToString();
                                    vm.FiscalYearDetailId = FYDVM.Id;
                                    vm.PFStructureId = empVM.PFStructureId;
                                    vm.ProjectId = PId;
                                    vm.DepartmentId = empVM.DepartmentId;
                                    vm.SectionId = empVM.SectionId;
                                    vm.DesignationId = empVM.DesignationId;
                                    vm.EmployeeId = empVM.EmployeeId;
                                    vm.EmployeePFValue = Convert.ToInt32(item["Amount"].ToString());
                                    vm.EmployeerPFValue = Convert.ToInt32(item["Amount"].ToString());
                                    vm.Post = false;
                                    vm.Remarks = "";
                                    vm.IsActive = true;
                                    vm.IsArchive = false;
                                    vm.CreatedBy = avm.CreatedAt;
                                    vm.CreatedAt = avm.CreatedBy;
                                    vm.CreatedFrom = avm.CreatedFrom;
                                    vm.LastUpdateBy = avm.CreatedBy;
                                    vm.LastUpdateAt = avm.CreatedAt;
                                    vm.LastUpdateFrom = avm.CreatedFrom;
                                    vm.BasicSalary = empVM.BasicSalary;
                                    vm.GrossSalary = empVM.GrossSalary;
                                    vm.IsDistribute = false;
                                    vm.IsBankDeposited = false;
                                    vm.TransType = "PF";

                                    retResults = InsertPFDetails(vm, currConn, transaction);

                                    if (retResults[0] != "Success")
                                    {
                                        throw new ArgumentNullException("PF details not saved");
                                    }

                                    #region SuccessResult
                                    retResults[0] = "Success";
                                    retResults[1] = "Data Save Successfully.";
                                    #endregion SuccessResult

                                }
                            }
                        }
                    }

                    retResults = UpdatePFHeader(FYDVM.Id, PId, avm.BranchId, currConn, transaction, avm);

                    //SettingDAL _settingDal = new SettingDAL();
                    //string AutoJournal = _settingDal.settingValue("PF", "AutoJournal").Trim();
                    //if (AutoJournal == "Y")
                    //{
                    //    retResults = AutoJournalSave(FYDVM.Id, PId, avm.BranchId, currConn, transaction, avm);
                    //}

                    #region Commit
                    if (Vtransaction == null && transaction != null)
                    {
                        transaction.Commit();
                        retResults[0] = "Success";
                        retResults[1] = "PF Details Saved Successfully.";
                    }
                    #endregion Commit
                }


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

        public string[] AutoJournalSave(string TransactionMonth, string TransactionForm, string TransactionCode, string BranchId, SqlConnection currConn, SqlTransaction transaction, ShampanIdentityVM auditvm)
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
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message

            string EmployeeCOAID = "";
            string EmployerCOAID = "";
            string BankCOAID = "";

            string startdata = @"Select PeriodEnd from FiscalYearDetail where PeriodName=@PeriodName ";
            SqlCommand cmdd = new SqlCommand(startdata, currConn, transaction);
            cmdd.Parameters.AddWithValue("@PeriodName", TransactionMonth);
            SqlDataAdapter adapterd = new SqlDataAdapter(cmdd);
            DataTable dtd = new DataTable();
            adapterd.Fill(dtd);



            string Journal = @"Select JournalFor, JournalName,Nature,GroupName,COAID from AutoJournalSetup ";
            SqlCommand cmdj = new SqlCommand(Journal, currConn, transaction);
            cmdj.Parameters.AddWithValue("JournalFor", TransactionForm);
            SqlDataAdapter adapterj = new SqlDataAdapter(cmdj);
            DataTable dtj = new DataTable();
            adapterj.Fill(dtj);
            if (dtj.Rows.Count > 0)
            {
                EmployeeCOAID = dtj.Rows[0]["COAID"].ToString();
                EmployerCOAID = dtj.Rows[1]["COAID"].ToString();
                BankCOAID = dtj.Rows[2]["COAID"].ToString();

            }

            SettingDAL _settingDal = new SettingDAL();
            string IsAutoJournal = _settingDal.settingValue("PF", "IsAutoJournal").Trim();

            if (IsAutoJournal == "Y")
            {

                string id = @"Select SUM(pd.EmployeePFValue) EmployeePFValue, SUM(pd.EmployeerPFValue) EmployeerPFValue from PFDetails pd
                             LEFT OUTER JOIN PFHeader ph on ph.id=pd.PFHeaderId where ph.Code=@Code";
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
                    TransactionType = 31,
                    JournalType = 1,
                    TransType = "PF",


                    GLJournalDetails = new List<GLJournalDetailVM>
                    {
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployeeCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeePFValue"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                        new GLJournalDetailVM
                        {                                  
                            COAId =Convert.ToInt32(EmployerCOAID),
                            CrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeerPFValue"].ToString()),
                            IsDr = false,
                            IsYearClosing = false,
                            Post = false
                        },
                            new GLJournalDetailVM
                        {
                            COAId =Convert.ToInt32(BankCOAID),
                            DrAmount = Convert.ToDecimal(dtpf.Rows[0]["EmployeePFValue"].ToString()) + Convert.ToDecimal(dtpf.Rows[0]["EmployeerPFValue"].ToString()),
                            IsDr = true,
                            IsYearClosing = false,
                            Post = false
                        }
                    }
                };
                vmj.Code = TransactionCode;
                vmj.TransactionDate = dtd.Rows[0][0].ToString();
                vmj.BranchId = BranchId;
                vmj.Remarks = "Contribution Employee & Employer";
                GLJournalDAL glJournalDal = new GLJournalDAL();
                retResults = glJournalDal.Insert(vmj);
            }

            #region Results
            return retResults;
            #endregion

        }
        /// <summary>
        /// Inserts a new PF Details into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the PF Details data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertPFDetails").
        /// </returns>
        private string[] InsertPFDetails(PFDetailVM vm, SqlConnection currConn, SqlTransaction transaction)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PFProcess"; //Method Name
            int transResult = 0;
            string sqlText = "";

            SqlCommand cmd = null;

            #endregion

            try
            {
                #region Save Data

                #region SqlText

                sqlText = @"insert into PFDetails (PFHeaderId,FiscalYearDetailId,PFStructureId,ProjectId,DepartmentId,SectionId,DesignationId,EmployeeId,EmployeePFValue,EmployeerPFValue,
                            Post,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,LastUpdateBy,LastUpdateAt,LastUpdateFrom,BasicSalary,GrossSalary,IsDistribute,IsBankDeposited,TransactionType,TransType) 
                           values ( @PFHeaderId,@FiscalYearDetailId,@PFStructureId,@ProjectId,@DepartmentId,@SectionId,@DesignationId,@EmployeeId,@EmployeePFValue,@EmployeerPFValue,
                            @Post,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@LastUpdateBy,@LastUpdateAt,@LastUpdateFrom,@BasicSalary,@GrossSalary,@IsDistribute,@IsBankDeposited,@TransactionType,@TransType)  
                          ";

                string deleteCurrent = @" DELETE FROM PFDetails  WHERE FiscalYearDetailId=@FiscalYearDetailId and ProjectId=@ProjectId and EmployeeId=@EmployeeId";

                cmd = new SqlCommand(deleteCurrent, currConn, transaction);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                cmd.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PFHeaderId", vm.PFHeaderId);
                cmd.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                cmd.Parameters.AddWithValue("@PFStructureId", 1);
                cmd.Parameters.AddWithValue("@ProjectId", vm.ProjectId);
                cmd.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);
                cmd.Parameters.AddWithValue("@SectionId", vm.SectionId);
                cmd.Parameters.AddWithValue("@DesignationId", vm.DesignationId);
                cmd.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmd.Parameters.AddWithValue("@EmployeePFValue", vm.EmployeePFValue);
                cmd.Parameters.AddWithValue("@EmployeerPFValue", vm.EmployeerPFValue);
                cmd.Parameters.AddWithValue("@Post", vm.Post);
                cmd.Parameters.AddWithValue("@Remarks", vm.Remarks);
                cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                cmd.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                cmd.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                cmd.Parameters.AddWithValue("@BasicSalary", vm.BasicSalary);
                cmd.Parameters.AddWithValue("@GrossSalary", vm.GrossSalary);
                cmd.Parameters.AddWithValue("@IsDistribute", vm.IsDistribute);
                cmd.Parameters.AddWithValue("@IsBankDeposited", vm.IsBankDeposited);
                cmd.Parameters.AddWithValue("@TransactionType", "PF");
                cmd.Parameters.AddWithValue("@TransType", vm.TransType);

                var exeRes = cmd.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    throw new ArgumentNullException("No Data Found for Process", "");
                }

                #endregion SqlExecution

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                #endregion SuccessResult

                #endregion Save Data

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                if (transaction == null) { transaction.Rollback(); }
                return retResults;
            }


            #endregion
            return retResults;
        }

        #endregion Methods
    }
}
