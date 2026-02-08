using Excel;
using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SymServices.PF
{
    public class EmployeeBreakMonthPFDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        //==================SelectAll=================

        /// <summary>
        /// Inserts a new Employee Break Month record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="EmployeeBreakMonthVM"/> containing the Employee Break Month data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any)    
        /// </returns>
        public List<EmployeeBreakMonthPFVM> SelectAll(string empid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeBreakMonthPFVM> vms = new List<EmployeeBreakMonthPFVM>();
            EmployeeBreakMonthPFVM vm;
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
 pfo.Id
,pfo.EmployeeId

,CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,e.JoinDate
--,e.EmpName
--,e.Code
--,e.Designation
--,e.Department, e.JoinDate, e.Section, e.Project,


,isnull(e.GrossSalary,0)GrossSalary
,isnull(e.BasicSalary,0)BasicSalary
,isnull(pfo.EmployeeContribution,0)       EmployeeContribution
,isnull(pfo.EmployerContribution,0)       EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)       EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)       EmployerProfit

,pfo.OpeningDate
,pfo.Post
,isnull(pfo.Remarks,'')Remarks
,pfo.IsActive
,pfo.IsArchive
,isnull(pfo.CreatedBy,'')CreatedBy
,pfo.CreatedAt
,isnull(pfo.CreatedFrom,'')CreatedFrom
,isnull(pfo.LastUpdateBy,'')LastUpdateBy
,pfo.LastUpdateAt
,isnull(pfo.LastUpdateFrom,'')LastUpdateFrom
From EmployeeBreakMonthPF pfo

";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId";
                sqlText += "  left outer join  ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId";

                sqlText += " Where 1=1 and  pfo.IsArchive=0 and  pfo.IsActive=1";

                #endregion

                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and pfo.EmployeeId=@EmployeeId ";
                }


                sqlText += @" ORDER BY pfo.EmployeeId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(empid))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", empid);
                }
               
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeBreakMonthPFVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["OpeningDate"].ToString());
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.Remarks = dr["Remarks"].ToString();
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
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                    vms.Add(vm);
                }
                dr.Close();


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

        /// <summary>
        /// Inserts a new Employee Break Month record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="EmployeeBreakMonthVM"/> containing the bank branch data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any)    
        /// </returns>
        public List<EmployeeBreakMonthPFVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeBreakMonthPFVM> VMs = new List<EmployeeBreakMonthPFVM>();
            EmployeeBreakMonthPFVM vm;
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
 pfo.Id
,pfo.EmployeeId
,e.EmpName
,e.Code
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,isnull(pfo.EmployeeContribution,0)EmployeeContribution
,isnull(pfo.EmployerContribution,0)EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)EmployerProfit
,pfo.OpeningDate
,pfo.Post
,pfo.Remarks
,pfo.IsActive
,pfo.IsArchive
,pfo.CreatedBy
,pfo.CreatedAt
,pfo.CreatedFrom
,pfo.LastUpdateBy
,pfo.LastUpdateAt
,pfo.LastUpdateFrom
From EmployeeBreakMonthPF pfo

";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.Id";
                sqlText += " Where 1=1 and  pfo.IsArchive=0 and  pfo.IsActive=1";

                #endregion


                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and pfo.EmployeeId=@EmployeeId ";
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

                sqlText += @" ORDER BY pfo.EmployeeId";

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

                if (!string.IsNullOrEmpty(empid))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", empid);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeBreakMonthPFVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["OpeningDate"].ToString());
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.Remarks = dr["Remarks"].ToString();
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
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);

                    VMs.Add(vm);
                }
                dr.Close();
                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion
            return VMs;
        }

        /// <summary>
        /// Retrieves detailed information for an Employee Break Month Provident Fund (PF) record based on the specified record ID and/or Employee ID.
        /// </summary>
        /// <param name="Id">The unique identifier of the EmployeeBreakMonthPF record. Optional if <paramref name="empId"/> is provided.</param>
        /// <param name="empId">The unique identifier of the employee. Optional if <paramref name="Id"/> is provided.</param>
        /// <returns>
        /// An instance of <see cref="EmployeeBreakMonthPFVM"/> containing PF details, employee information, and contribution amounts.
        /// Returns an empty object if no matching record is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when an error occurs during SQL execution. The exception message includes the SQL query and error message.
        /// </exception>

        public EmployeeBreakMonthPFVM SelectById(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeBreakMonthPFVM vm = new EmployeeBreakMonthPFVM();

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
 pfo.Id
,pfo.EmployeeId
,e.EmpName
,e.Code
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,isnull(pfo.EmployeeContribution,0)EmployeeContribution
,isnull(pfo.EmployerContribution,0)EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)EmployerProfit
,pfo.OpeningDate
,pfo.Post
,pfo.Remarks
,pfo.IsActive
,pfo.IsArchive
,pfo.CreatedBy
,pfo.CreatedAt
,pfo.CreatedFrom
,pfo.LastUpdateBy
,pfo.LastUpdateAt
,pfo.LastUpdateFrom
From EmployeeBreakMonthPF pfo
";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.Id";
                sqlText += " Where 1=1 and  pfo.IsArchive=0 and  pfo.IsActive=1";

                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @" and pfo.Id=@Id ";
                }
                if (!string.IsNullOrWhiteSpace(empId))
                {
                    sqlText += @" and pfo.EmployeeId=@EmployeeId ";
                }

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = currConn;
                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(Id))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                }
                if (!string.IsNullOrWhiteSpace(empId))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", empId);
                }


                SqlDataReader dr;
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    vm = new EmployeeBreakMonthPFVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["OpeningDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
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
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                }
                dr.Close();


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

        public EmployeeBreakMonthPFVM SelectByIdAll(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeBreakMonthPFVM vm = new EmployeeBreakMonthPFVM();

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
 pfo.Id
,pfo.EmployeeId
,e.EmpName
,e.Code
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,isnull(pfo.EmployeeContribution,0)EmployeeContribution
,isnull(pfo.EmployerContribution,0)EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)EmployerProfit
,pfo.OpeningDate
,pfo.Post
,pfo.Remarks
,pfo.IsActive
,pfo.IsArchive
,pfo.CreatedBy
,pfo.CreatedAt
,pfo.CreatedFrom
,pfo.LastUpdateBy
,pfo.LastUpdateAt
,pfo.LastUpdateFrom
From EmployeeBreakMonthPF pfo
";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId";
                sqlText += " Where 1=1 ";

                if (!string.IsNullOrEmpty(Id))
                {
                    sqlText += @" and pfo.Id=@Id ";
                }
                if (!string.IsNullOrWhiteSpace(empId))
                {
                    sqlText += @" and pfo.EmployeeId=@EmployeeId ";
                }

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = currConn;
                cmd.CommandText = sqlText;
                cmd.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(Id))
                {
                    cmd.Parameters.AddWithValue("@Id", Id);
                }
                if (!string.IsNullOrWhiteSpace(empId))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", empId);
                }


                SqlDataReader dr;
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    vm = new EmployeeBreakMonthPFVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["OpeningDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
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
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"]);
                    vm.BasicSalary = Convert.ToDecimal(dr["BasicSalary"]);
                }
                dr.Close();


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


        //==================Insert =================

        /// <summary>
        /// Inserts a new  Employee Break Month record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="EmployeeBreakMonthVM"/> containing the  Employee Break Month data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertEmployeeBreakMonth").
        /// </returns>
        public string[] Insert(EmployeeBreakMonthPFVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertPFOpeinig"; //Method Name
            EmployeeInfoDAL _dal = new EmployeeInfoDAL();


            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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

                //if (!Ordinary.IsNumeric(vm.PFOpeningValue.ToString()))
                //{
                //    throw new ArgumentNullException("", "Please input the Numeric Value in OpeningValue");
                //}
                //if (vm.PFOpeningValue == 0)
                //{
                //    throw new ArgumentNullException("", "Please input the Numeric Value in OpeningValue");
                //}

                #region Exist

                sqlText = "  ";
                sqlText += " SELECT   count(Id) FROM EmployeeBreakMonthPF ";
                sqlText += @" WHERE EmployeeId=@EmployeeId and OpeningDate=@OpeningDate ";


                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@OpeningDate", Ordinary.DateToString(vm.OpeningDate));

                var exeRes = cmdExist.ExecuteScalar();

                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    throw new ArgumentNullException("Same Date Trasaction already exits", "");
                }

                #endregion Exist

                #region Save

                vm.Id = cdal.NextId("EmployeeBreakMonthPF", currConn, transaction).ToString();

                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeBreakMonthPF
(
 Id
,EmployeeId
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit

,OpeningDate
,Post
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
  @Id
, @EmployeeId
, @EmployeeContribution
, @EmployerContribution
, @EmployeeProfit
, @EmployerProfit
, @OpeningDate
, @Post
, @Remarks
, @IsActive
, @IsArchive
, @CreatedBy
, @CreatedAt
, @CreatedFrom
) ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@EmployeeContribution", vm.EmployeeContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployerContribution", vm.EmployerContribution);
                    cmdInsert.Parameters.AddWithValue("@EmployeeProfit", vm.EmployeeProfit);
                    cmdInsert.Parameters.AddWithValue("@EmployerProfit", vm.EmployerProfit);
                    cmdInsert.Parameters.AddWithValue("@OpeningDate", Ordinary.DateToString(vm.OpeningDate));
                    cmdInsert.Parameters.AddWithValue("@Post", false);                    
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();


                }
                else
                {
                    retResults[1] = "Please Input EmployeeBreakMonthPF Value";
                    throw new ArgumentNullException("Please Input EmployeeBreakMonthPF Value", "");
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
                retResults[1] = ex.Message.ToString(); //catch ex

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

        // <summary>
        /// Updates an existing Employee Break Month record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="EmployeeBreakMonthVM"/> containing the updated Employee Break Month information.</param>
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
        public string[] Update(EmployeeBreakMonthPFVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeBreakMonthPF Update"; //Method Name

            int transResult = 0;

            string sqlText = "";

            bool iSTransSuccess = false;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion
            
            try
            {
                //if (!Ordinary.IsNumeric(vm.PFOpeningValue.ToString()))
                //{
                //    throw new ArgumentNullException("Please input the Numeric Value in OpeningValue","");
                //}
                //if (vm.PFOpeningValue == 0)
                //{
                //    throw new ArgumentNullException("Please input the Numeric Value in OpeningValue","");
                //}

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeBreakMonthPF"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeBreakMonthPF ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND  Id<>@Id";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    //cmdExist.Parameters.AddWithValue("@EarningDate", Ordinary.DateToString(vm.EarningDate));
                    var exeRes = cmdExist.ExecuteScalar();
                    int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId < 0)
                    {
                        throw new ArgumentNullException("Could not found any item. Please Add Fast", "");
                    }

                    #endregion Exist

                    #region Post Check

                    sqlText = "  ";
                    sqlText += " SELECT isnull(Post,0) FROM EmployeeBreakMonthPF ";
                    sqlText += " WHERE Id=@Id";
                    cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);

                    ////cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    ////cmdExist.Parameters.AddWithValue("@EarningDate", Ordinary.DateToString(vm.EarningDate));

                    var postCheck = cmdExist.ExecuteScalar();
                    bool Checkpost = Convert.ToBoolean(postCheck);

                    if (Checkpost)
                    {
                        throw new ArgumentNullException("This transaction already posted", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeBreakMonthPF set";
                    sqlText += " EmployeeContribution=@EmployeeContribution,";
                    sqlText += " EmployerContribution=@EmployerContribution,";
                    sqlText += " EmployeeProfit=@EmployeeProfit,";
                    sqlText += " EmployerProfit=@EmployerProfit,";
                    sqlText += " OpeningDate=@OpeningDate,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeContribution", vm.EmployeeContribution);
                    cmdUpdate.Parameters.AddWithValue("@EmployerContribution", vm.EmployerContribution);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeProfit", vm.EmployeeProfit);
                    cmdUpdate.Parameters.AddWithValue("@EmployerProfit", vm.EmployerProfit);
                    cmdUpdate.Parameters.AddWithValue("@OpeningDate", Ordinary.DateToString(vm.OpeningDate));
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeOtherEarningVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Could not found any item.","");
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
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update EmployeeBreakMonthPF.";
                    throw new ArgumentNullException("", "");
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

        //==================Post =================

        /// <summary>
        /// Posts one or more Employee Break Month records by updating their status in the database.  
        /// Internally calls the <c>FieldPost</c> method for each provided ID to execute the posting logic.  
        /// Supports optional external SQL connection and transaction for integration with broader transactional workflows.
        /// </summary>
        /// <param name="ids">An array of string IDs representing the bank charge records to be posted.</param>
        /// <param name="VcurrConn">Optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">Optional SQL transaction. If not provided, a new transaction is created and committed upon success.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Unused or placeholder return value,  
        /// [3] = Placeholder for last executed SQL query (not set in this method),  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("PostEmployeeBreakMonth").
        /// </returns>
        public string[] Post(EmployeeBreakMonthPFVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeeBreakMonthPF Post"; //Method Name

            int transResult = 0;

            string sqlText = "";

            bool iSTransSuccess = false;

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeeBreakMonthPF"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeBreakMonthPF ";
                    sqlText += " WHERE EmployeeId=@EmployeeId ";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    //cmdExist.Parameters.AddWithValue("@EarningDate", Ordinary.DateToString(vm.EarningDate));
                    var exeRes = cmdExist.ExecuteScalar();
                    int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId < 0)
                    {
                        throw new ArgumentNullException("Could not found any item. Please Add Fast", "");
                    }

                    #endregion Exist

                    #region Post Check

                    sqlText = "  ";
                    sqlText += " SELECT isnull(Post,0) FROM EmployeeBreakMonthPF ";
                    sqlText += " WHERE Id=@Id";
                    cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);

                    ////cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    ////cmdExist.Parameters.AddWithValue("@EarningDate", Ordinary.DateToString(vm.EarningDate));

                    var postCheck = cmdExist.ExecuteScalar();
                    bool Checkpost = Convert.ToBoolean(postCheck);

                    if (Checkpost)
                    {
                        retResults[0] = "Fail";
                        retResults[1] = "This transaction already posted";

                        return retResults;

                        ////throw new ArgumentNullException("This transaction already posted", "");
                    }

                    #endregion Exist

                    #region Update Settings

                    sqlText = "";
                    sqlText = "update EmployeeBreakMonthPF set";
                    sqlText += " Post=@Post,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Post", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
                    exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", EmployeeOtherEarningVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Could not found any item.", "");
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
                    retResults[1] = "Data Post Successfully.";

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to Post EmployeeBreakMonthPF.";
                    throw new ArgumentNullException("", "");
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


        //==================Delete =================
        public string[] MultiplePost(EmployeeBreakMonthPFVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeBreakMonthPF"; //Method Name

            int transResult = 0;
            int countId = 0;
            string sqlText = "";

            bool iSTransSuccess = false;

            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("PostToEmployeeBreakMonthPF"); }

                #endregion open connection and transaction
                
                if (Ids.Length > 1)
                {
                    #region Update Settings

                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        #region Exist

                        sqlText = "  ";
                        sqlText += " SELECT EmployeeId FROM EmployeeBreakMonthPF ";
                        sqlText += " WHERE Id=@Id ";
                        SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                        cmdExist.Transaction = transaction;
                        cmdExist.Parameters.AddWithValue("@Id", Ids[i]);
                        ////cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        ////cmdExist.Parameters.AddWithValue("@EarningDate", Ordinary.DateToString(vm.EarningDate));
                        var exeChk = cmdExist.ExecuteScalar();
                        string empId = exeChk.ToString();

                        if (string.IsNullOrWhiteSpace(empId))
                        {
                            throw new ArgumentNullException("Could not found any item. Please Add Fast", "");
                        }

                        #endregion Exist

                        vm.Id = Ids[i];
                        vm.EmployeeId = empId;

                        retResults = Post(vm, currConn, transaction);

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #endregion Update Settings

                    iSTransSuccess = true;

                }
                else
                {
                    throw new ArgumentNullException("Could not found any item.","");
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
                    retResults[1] = "Data Post Successfully.";
                }
                else
                {
                    retResults[1] = "Unexpected error to delete EmployeeBreakMonthPF Information.";
                    throw new ArgumentNullException("", "");
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


        public DataTable ExportExcelFileFormEmployee(EmployeeBreakMonthPFVM vm, string Filepath, string FileName)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            int j = 2;
            DataTable dt = new DataTable();
            string[] results = new string[6];

            #endregion

            try
            {
                
                #region DataRead From DB


                

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction

                #region sql statement
                sqlText = @"
select Code EmpCode,EmpName, 
ISNULL(Grade+'-'+StepName,'NA') Grade,
(case when Designation is null and Designation='=NA=' then 'NA' else Designation end) Designation,
(case when Department is null and Department='=NA=' then 'NA' else Department end) Department ,
(case when Section is null and Section='=NA=' then 'NA' else Section end) Section,
(case when Project is null and Project='=NA=' then 'NA' else Project end) Project
,'0.00' EmployeeContribution
,'0.00' EmployerContribution
,'0.00' EmployeeProfit
,'0.00' EmployerProfit
, @OpeningDate OpeningDate,'' Remarks

from ViewEmployeeInformation where 1=1
";

                if (vm.ProjectId != "0_0" && vm.ProjectId != "0" && vm.ProjectId != "" && vm.ProjectId != "null" && vm.ProjectId != null)
                    sqlText += @" and ProjectId=@ProjectId";
                if (vm.DepartmentId != "0_0" && vm.DepartmentId != "0" && vm.DepartmentId != "" && vm.DepartmentId != "null" && vm.DepartmentId != null)
                    sqlText += @" and DepartmentId=@DepartmentId";
                if (vm.SectionId != "0_0" && vm.SectionId != "0" && vm.SectionId != "" && vm.SectionId != "null" && vm.SectionId != null)
                    sqlText += @" and SectionId=@SectionId";
                if (vm.DesignationId != "0_0" && vm.DesignationId != "0" && vm.DesignationId != "" && vm.DesignationId != "null" && vm.DesignationId != null)
                    sqlText += @" and DesignationId=@DesignationId";
                if (vm.Code != "0_0" && vm.Code != "0" && vm.Code != "" && vm.Code != "null" && vm.Code != null)
                    sqlText += @" and Code >=@Code";
                if (vm.CodeT != "0_0" && vm.CodeT != "0" && vm.CodeT != "" && vm.CodeT != "null" && vm.CodeT != null)
                    sqlText += @" and Code<=@CodeT";


                if (vm.Orderby == "DCG")
                    sqlText += " order by department, EmpCode, GradeSl";
                else if (vm.Orderby == "DDC")
                    sqlText += " order by department, JoinDate, code";
                else if (vm.Orderby == "DGC")
                    sqlText += " order by department, GradeSl, code";
                else if (vm.Orderby == "DGDC")
                    sqlText += " order by department, GradeSl, JoinDate, EmpCode";
                else if (vm.Orderby == "CODE")
                    sqlText += " ORDER BY  EmpCode";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;

                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);

                if (vm.ProjectId != "0_0" && vm.ProjectId != "0" && vm.ProjectId != "" && vm.ProjectId != "null" && vm.ProjectId != null)
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", vm.ProjectId);

                if (vm.DepartmentId != "0_0" && vm.DepartmentId != "0" && vm.DepartmentId != "" && vm.DepartmentId != "null" && vm.DepartmentId != null)
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);


                if (vm.SectionId != "0_0" && vm.SectionId != "0" && vm.SectionId != "" && vm.SectionId != "null" && vm.SectionId != null)
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", vm.SectionId);


                if (vm.DesignationId != "0_0" && vm.DesignationId != "0" && vm.DesignationId != "" && vm.DesignationId != "null" && vm.DesignationId != null)
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", vm.DesignationId);


                if (vm.Code != "0_0" && vm.Code != "0" && vm.Code != "" && vm.Code != "null" && vm.Code != null)
                    da.SelectCommand.Parameters.AddWithValue("@Code", vm.Code);


                if (vm.CodeT != "0_0" && vm.CodeT != "0" && vm.CodeT != "" && vm.CodeT != "null" && vm.CodeT != null)
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeT);

                da.SelectCommand.Parameters.AddWithValue("@OpeningDate", DateTime.Now.ToString("dd-MMM-yyyy"));


                da.Fill(dt);
                ////dt.Columns.Remove("GradeSl");

                #endregion

                #endregion


                #region Value Round

                ////string[] columnNames = { "OpeningValue" };

                ////dt = Ordinary.DtValueRound(dt, columnNames);

                #endregion

            }
            catch (Exception ex)
            {
                results[1] = ex.Message.ToString(); //catch ex
                //return results;
                throw ex;
            }
            return dt;
        }

        public DataTable ExportExcelFileFormPFOpening(EmployeeBreakMonthPFVM vm, string Filepath, string FileName)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            int j = 2;
            DataTable dt = new DataTable();
            string[] results = new string[6];

            #endregion

            try
            {

                #region DataRead From DB
                
                

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
 emp.Code EmpCode
,emp.EmpName
,emp.JoinDate
,ISNULL(emp.Grade+'-'+emp.StepName,'NA') Grade
,(case when emp.Designation is null and emp.Designation='=NA=' then 'NA' else emp.Designation end) Designation
,(case when emp.Department is null and emp.Department='=NA=' then 'NA' else emp.Department end) Department 
,(case when emp.Section is null and emp.Section='=NA=' then 'NA' else emp.Section end) Section
,(case when emp.Project is null and emp.Project='=NA=' then 'NA' else emp.Project end) Project
,ISNULL(EmployeeContribution,0) EmployeeContribution
,ISNULL(EmployerContribution,0) EmployerContribution
,ISNULL(EmployeeProfit,0) EmployeeProfit
,ISNULL(EmployerProfit,0) EmployerProfit
,OpeningDate
,Remarks
  FROM EmployeeBreakMonthPF pfo

";

                sqlText += " left outer join ViewEmployeeInformation emp on pfo.EmployeeId=emp.EmployeeId";
                sqlText += " Where 1=1 ";


                if (vm.ProjectId != "0_0" && vm.ProjectId != "0" && vm.ProjectId != "" && vm.ProjectId != "null" && vm.ProjectId != null)
                    sqlText += @" and emp.ProjectId=@ProjectId";
                if (vm.DepartmentId != "0_0" && vm.DepartmentId != "0" && vm.DepartmentId != "" && vm.DepartmentId != "null" && vm.DepartmentId != null)
                    sqlText += @" and emp.DepartmentId=@DepartmentId";
                if (vm.SectionId != "0_0" && vm.SectionId != "0" && vm.SectionId != "" && vm.SectionId != "null" && vm.SectionId != null)
                    sqlText += @" and emp.SectionId=@SectionId";
                if (vm.DesignationId != "0_0" && vm.DesignationId != "0" && vm.DesignationId != "" && vm.DesignationId != "null" && vm.DesignationId != null)
                    sqlText += @" and emp.DesignationId=@DesignationId";
                if (vm.Code != "0_0" && vm.Code != "0" && vm.Code != "" && vm.Code != "null" && vm.Code != null)
                    sqlText += @" and emp.Code >=@Code";
                if (vm.CodeT != "0_0" && vm.CodeT != "0" && vm.CodeT != "" && vm.CodeT != "null" && vm.CodeT != null)
                    sqlText += @" and emp.Code<=@CodeT";


                if (vm.Orderby == "DCG")
                    sqlText += " order by emp.department, emp.EmpCode, emp.GradeSl";
                else if (vm.Orderby == "DDC")
                    sqlText += " order by emp.department, emp.JoinDate, emp.code";
                else if (vm.Orderby == "DGC")
                    sqlText += " order by emp.department, emp.GradeSl, emp.code";
                else if (vm.Orderby == "DGDC")
                    sqlText += " order by emp.department, emp.GradeSl, emp.JoinDate, emp.EmpCode";
                else if (vm.Orderby == "CODE")
                    sqlText += " ORDER BY  emp.Code";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;

                objComm.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(objComm);

                if (vm.ProjectId != "0_0" && vm.ProjectId != "0" && vm.ProjectId != "" && vm.ProjectId != "null" && vm.ProjectId != null)
                    da.SelectCommand.Parameters.AddWithValue("@ProjectId", vm.ProjectId);

                if (vm.DepartmentId != "0_0" && vm.DepartmentId != "0" && vm.DepartmentId != "" && vm.DepartmentId != "null" && vm.DepartmentId != null)
                    da.SelectCommand.Parameters.AddWithValue("@DepartmentId", vm.DepartmentId);


                if (vm.SectionId != "0_0" && vm.SectionId != "0" && vm.SectionId != "" && vm.SectionId != "null" && vm.SectionId != null)
                    da.SelectCommand.Parameters.AddWithValue("@SectionId", vm.SectionId);


                if (vm.DesignationId != "0_0" && vm.DesignationId != "0" && vm.DesignationId != "" && vm.DesignationId != "null" && vm.DesignationId != null)
                    da.SelectCommand.Parameters.AddWithValue("@DesignationId", vm.DesignationId);


                if (vm.Code != "0_0" && vm.Code != "0" && vm.Code != "" && vm.Code != "null" && vm.Code != null)
                    da.SelectCommand.Parameters.AddWithValue("@Code", vm.Code);


                if (vm.CodeT != "0_0" && vm.CodeT != "0" && vm.CodeT != "" && vm.CodeT != "null" && vm.CodeT != null)
                    da.SelectCommand.Parameters.AddWithValue("@CodeT", vm.CodeT);

                da.Fill(dt);

                dt.Columns.Remove("JoinDate");

                #endregion

                #endregion

                #region String To Date

                string DatecolumnNames = "OpeningDate";

                dt = Ordinary.DtColumnStringToDate(dt, DatecolumnNames);


                #endregion

                #region Value Round

                string[] columnNames = { "OpeningValue" };

                dt = Ordinary.DtValueRound(dt, columnNames);

                #endregion

            }
            catch (Exception ex)
            {
                results[1] = ex.Message.ToString(); //catch ex
                //return results;
                throw ex;
            }
            return dt;
        }

        public string[] ImportExcelFile(string Fullpath, string fileName, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "EmployeeBreakMonthPF"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            EmployeeInfoDAL _dalemp = new EmployeeInfoDAL();
            //ViewEmployeeInfoVM empVM = new ViewEmployeeInfoVM();
            //FiscalYearDAL fydal = new FiscalYearDAL();
            //EarningDeductionTypeDAL eddal = new EarningDeductionTypeDAL();
            //FiscalYearDetailVM FYDVM = new FiscalYearDetailVM();

            #region try
            try
            {
                

                DataSet ds = new DataSet();

                #region Excel Read
                
                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                // We return the interface, so that
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
                dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                var a = "";

                foreach (DataRow item in dt.Rows)
                {
                    EmployeeBreakMonthPFVM vm = new EmployeeBreakMonthPFVM();
                    
                    //empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();

                    string empCode = item["EmpCode"].ToString();

                    #region Get Employee id

                    sqlText = "  ";
                    
                    sqlText += " SELECT EmployeeId from ViewEmployeeInformation ";
                    sqlText += " WHERE Code=@Code";

                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Code", empCode);

                    var getId = cmdExist.ExecuteScalar();
                    string empId = getId.ToString();

                   
                    #endregion Exist


                    if (string.IsNullOrWhiteSpace(empId))
                    {
                        throw new ArgumentNullException("","Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {

                        if (!Ordinary.IsNumeric(item["OpeningValue"].ToString()))
                        {
                            throw new ArgumentNullException("","Please input the Numeric Value in OpeningValue");
                        }
                        else
                        {

                            #region Get Employee id

                            sqlText = "  ";

                            sqlText += " SELECT Id from EmployeeBreakMonthPF WHERE EmployeeId=@EmployeeId ";

                            cmdExist = new SqlCommand(sqlText, currConn);
                            cmdExist.Transaction = transaction;
                            cmdExist.Parameters.AddWithValue("@EmployeeId", empId);

                            var PFId = cmdExist.ExecuteScalar();
                            string PFOprningId ="";

                            if (PFId != null)
                            {
                                PFOprningId = PFId.ToString();
                            }

                            #endregion Exist

                            if (!string.IsNullOrWhiteSpace(PFOprningId))
                            {

                                #region Value assign
                                
                                vm.Id = PFOprningId;
                                vm.EmployeeId = empId;
                                vm.EmployeeContribution = Convert.ToDecimal(item["EmployeeContribution"]);
                                vm.EmployerContribution = Convert.ToDecimal(item["EmployerContribution"]);
                                vm.EmployeeProfit = Convert.ToDecimal(item["EmployeeProfit"]);
                                vm.EmployerProfit = Convert.ToDecimal(item["EmployerProfit"]);
                                vm.OpeningDate = item["OpeningDate"].ToString();
                                vm.Remarks = item["Remarks"].ToString();
                                vm.LastUpdateAt = auditvm.LastUpdateAt;
                                vm.LastUpdateBy = auditvm.LastUpdateBy;
                                vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                                vm.CreatedAt = auditvm.CreatedAt;
                                vm.CreatedBy = auditvm.CreatedBy;
                                vm.CreatedFrom = auditvm.CreatedFrom;

                                #endregion

                                #region Post Check and Update

                                sqlText = "  ";
                                sqlText += " SELECT isnull(Post,0) FROM EmployeeBreakMonthPF ";
                                sqlText += " WHERE Id=@Id";
                                cmdExist = new SqlCommand(sqlText, currConn);
                                cmdExist.Transaction = transaction;
                                cmdExist.Parameters.AddWithValue("@Id", vm.Id);

                                var postCheck = cmdExist.ExecuteScalar();
                                bool Checkpost = Convert.ToBoolean(postCheck);

                                if (!Checkpost)
                                {
                                    retResults = Update(vm, currConn, transaction);

                                }

                                #endregion Exist

                            }
                            else
                            {

                                #region Insert

                                #region Value assign

                                vm.EmployeeId = empId;
                                vm.EmployeeContribution = Convert.ToDecimal(item["EmployeeContribution"]);
                                vm.EmployerContribution = Convert.ToDecimal(item["EmployerContribution"]);
                                vm.EmployeeProfit = Convert.ToDecimal(item["EmployeeProfit"]);
                                vm.EmployerProfit = Convert.ToDecimal(item["EmployerProfit"]);
                                vm.OpeningDate = item["OpeningDate"].ToString();
                                vm.Remarks = item["Remarks"].ToString();
                                vm.LastUpdateAt = auditvm.LastUpdateAt;
                                vm.LastUpdateBy = auditvm.LastUpdateBy;
                                vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                                vm.CreatedAt = auditvm.CreatedAt;
                                vm.CreatedBy = auditvm.CreatedBy;
                                vm.CreatedFrom = auditvm.CreatedFrom;

                                #endregion

                                retResults = Insert(vm, currConn, transaction);

                                #endregion

                            }

                        
                            
                        }
                       
                    }
                }

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
                //retResults[2] = vm.Id.ToString();
                #endregion SuccessResult

            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[1] = ex.Message.ToString(); //catch ex
                retResults[0] = "Fail";//Success or Fail
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


    }
}
