using Excel;
using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SymServices.WPPF
{
    public class EmployeePFOpeinigDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of EmployeePFOpeinigVM objects representing employee provident fund opening details.
        /// Optionally filters the results by a specific employee ID.
        /// </summary>
        /// <param name="empid">Optional employee ID to filter the provident fund records.</param>
        /// <returns>A list of EmployeePFOpeinigVM instances containing provident fund and employee information.</returns>
        public List<EmployeePFOpeinigVM> SelectAll(string empid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePFOpeinigVM> vms = new List<EmployeePFOpeinigVM>();
            EmployeePFOpeinigVM vm;
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
 pfo.EmployeeId
,e.EmpName
,e.Code
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,isnull(pfo.EmployeeContribution,0)       EmployeeContribution
,isnull(pfo.EmployerContribution,0)       EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)       EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)       EmployerProfit

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
From EmployeePFOpeinig pfo

";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId";
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
                    vm = new EmployeePFOpeinigVM();
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
        /// Retrieves a list of EmployeePFOpeinigVM objects representing employee provident fund forfeiture details.
        /// </summary>
        /// <param name="empid">Optional employee ID to filter the results; if null, returns data for all employees.</param>
        /// <returns>A list of EmployeePFOpeinigVM objects containing provident fund forfeiture information for employees.</returns>
        public List<EmployeePFOpeinigVM> SelectAllForFeiture(string empid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePFOpeinigVM> vms = new List<EmployeePFOpeinigVM>();
            EmployeePFOpeinigVM vm;
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
,e.EmpName
,e.Code
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,isnull(pfo.EmployeeContribution,0)       EmployeeContribution
,isnull(pfo.EmployerContribution,0)       EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)       EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)       EmployerProfit

,pfo.ForFeitureDate
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
From EmployeeForFeiture_New pfo

";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.Id";
                sqlText += " Where 1=1 and  pfo.IsArchive=0 and GrossSalary>0 and  pfo.IsActive=1";

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
                    vm = new EmployeePFOpeinigVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["ForFeitureDate"].ToString());
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


        public EmployeePFOpeinigVM SelectForFeitureById(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFOpeinigVM vm = new EmployeePFOpeinigVM();

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
,pfo.ForFeitureDate
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
From EmployeeForFeiture_New pfo
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
                    vm = new EmployeePFOpeinigVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["ForFeitureDate"].ToString());
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

        /// <summary>
        /// Retrieves an EmployeePFOpeinigVM object from the database based on the provided record Id and/or employee Id.
        /// </summary>
        /// <param name="Id">The unique identifier of the EmployeePF opening record.</param>
        /// <param name="empId">The unique identifier of the employee.</param>
        /// <returns>
        /// An instance of EmployeePFOpeinigVM populated with data retrieved from the database. 
        /// If no matching record is found, returns an empty EmployeePFOpeinigVM object.
        /// </returns>
        public EmployeePFOpeinigVM SelectById(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFOpeinigVM vm = new EmployeePFOpeinigVM();

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
From EmployeePFOpeinig pfo
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
                    vm = new EmployeePFOpeinigVM();
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

        public EmployeePFOpeinigVM SelectByIdForFeiture(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFOpeinigVM vm = new EmployeePFOpeinigVM();

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
,pfo.ForFeitureDate
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
From EmployeeForFeiture_New pfo
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
                    vm = new EmployeePFOpeinigVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.OpeningDate = Ordinary.StringToDate(dr["ForFeitureDate"].ToString());
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

        /// <summary>
        /// Retrieves the complete EmployeePFOpeinigVM record by the specified Id and/or EmployeeId.
        /// Queries the database for the matching record and maps the result to an EmployeePFOpeinigVM object.
        /// </summary>
        /// <param name="Id">The identifier of the EmployeePFOpeinig record to retrieve. Can be null or empty.</param>
        /// <param name="empId">The employee identifier used to filter the record. Can be null or whitespace.</param>
        /// <returns>
        /// An instance of EmployeePFOpeinigVM populated with the data from the database if a matching record is found; 
        /// otherwise, an empty EmployeePFOpeinigVM object.
        /// </returns>
        public EmployeePFOpeinigVM SelectByIdAll(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFOpeinigVM vm = new EmployeePFOpeinigVM();

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
From EmployeePFOpeinig pfo
";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.Id";
                sqlText += " Where 1=1";

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
                    vm = new EmployeePFOpeinigVM();
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
        public string[] Insert(EmployeePFOpeinigVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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

                //#region Exist

                //sqlText = "  ";
                //sqlText += " SELECT   count(Id) FROM EmployeePFOpeinig ";
                //sqlText += @" WHERE EmployeeId=@EmployeeId ";

                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                //var exeRes = cmdExist.ExecuteScalar();

                //int objfoundId = Convert.ToInt32(exeRes);

                //if (objfoundId > 0)
                //{
                //    throw new ArgumentNullException("This Employee already exits", "");
                //}

                //#endregion Exist

                #region Exist

                sqlText = "";
                sqlText += " SELECT   count(Id) FROM EmployeePFOpeinig ";
                sqlText += @" WHERE OpeningDate=@OpeningDate and EmployeeId=@EmployeeId ";

                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@OpeningDate", Ordinary.DateToString(vm.OpeningDate));
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                var exeRes = cmdExist.ExecuteScalar();

                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    throw new ArgumentNullException("Same Date Trasaction already exits", "");
                }

                #endregion Exist


                #region Save

                vm.Id = cdal.NextId("EmployeePFOpeinig", currConn, transaction).ToString();

                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePFOpeinig
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
                    retResults[1] = "Please Input EmployeePFOpeinig Value";
                    throw new ArgumentNullException("Please Input EmployeePFOpeinig Value", "");
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
        public string[] Update(EmployeePFOpeinigVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFOpeinig Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePFOpeinig"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeePFOpeinig ";
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
                    sqlText += " SELECT isnull(Post,0) FROM EmployeePFOpeinig ";
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
                    sqlText = "update EmployeePFOpeinig set";
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
                    retResults[1] = "Data Update Successfully.";

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update EmployeePFOpeinig.";
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
        /// Posts the EmployeePFOpeinig record by updating its status to posted in the database.
        /// Checks if the record exists and is not already posted, then updates the post status with audit information.
        /// Supports optional use of existing SQL connection and transaction.
        /// </summary>
        /// <param name="vm">The EmployeePFOpeinig view model containing the data to post.</param>
        /// <param name="VcurrConn">Optional existing SQL connection to use; if null, a new connection will be opened.</param>
        /// <param name="Vtransaction">Optional existing SQL transaction to use; if null, a new transaction will be started.</param>
        /// <returns>
        /// An array of strings containing:
        /// [0] - "Success" or "Fail" status,
        /// [1] - Message describing the result,
        /// [2] - The Id of the processed record,
        /// [3] - The SQL query executed,
        /// [4] - Exception message if any,
        /// [5] - The method name.
        /// </returns>
        public string[] Post(EmployeePFOpeinigVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFOpeinig Post"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePFOpeinig"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeePFOpeinig ";
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
                    sqlText += " SELECT isnull(Post,0) FROM EmployeePFOpeinig ";
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
                    sqlText = "update EmployeePFOpeinig set";
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
                    retResults[1] = "Unexpected error to Post EmployeePFOpeinig.";
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
        /// <summary>
        /// Processes multiple EmployeePFOpeinig records by posting updates within a SQL transaction.
        /// If external connection and transaction are not provided, it creates and manages its own.
        /// Commits the transaction if all posts succeed; rolls back on failure.
        /// </summary>
        /// <param name="vm">The view model containing EmployeePFOpeinig data to post.</param>
        /// <param name="Ids">Array of record Ids to process.</param>
        /// <param name="VcurrConn">Optional existing SQL connection to use.</param>
        /// <param name="Vtransaction">Optional existing SQL transaction to use.</param>
        /// <returns>
        /// An array of strings containing:
        /// [0] - "Success" or "Fail" indicating the operation result.
        /// [1] - Message describing success or error details.
        /// [2] - Returned Id as string (empty when multiple processed).
        /// [3] - The SQL query text executed.
        /// [4] - Exception message if any exception occurred.
        /// [5] - Name of the method processing the request ("DeleteEmployeePFOpeinig").
        /// </returns>
        public string[] MultiplePost(EmployeePFOpeinigVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeePFOpeinig"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("PostToEmployeePFOpeinig"); }

                #endregion open connection and transaction

                if (Ids.Length > 1)
                {
                    #region Update Settings

                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        #region Exist

                        sqlText = "  ";
                        sqlText += " SELECT EmployeeId FROM EmployeePFOpeinig ";
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
                    retResults[1] = "Unexpected error to delete EmployeePFOpeinig Information.";
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


        /// <summary>
        /// Exports employee data to an Excel file based on the provided filter criteria in the EmployeePFOpeinigVM view model.
        /// Connects to the HRM database, constructs a dynamic SQL query with filters for project, department, section, designation, and employee codes,
        /// then fetches the filtered list of employees along with default contribution and profit values.
        /// </summary>
        /// <param name="vm">The view model containing filter criteria and ordering preferences.</param>
        /// <param name="Filepath">The file path where the Excel file should be saved (currently not used in the method).</param>
        /// <param name="FileName">The file name for the Excel file (currently not used in the method).</param>
        /// <returns>Returns a DataTable containing the filtered employee data including employee code, name, grade, designation, department, section, project, contributions, profits, and opening date.</returns>
        public DataTable ExportExcelFileFormEmployee(EmployeePFOpeinigVM vm, string Filepath, string FileName)
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
ISNULL(Grade,'NA') Grade,
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
                if (vm.BranchId != "0_0" && vm.BranchId != "0" && vm.BranchId != "" && vm.BranchId != "null" && vm.BranchId != null)
                    sqlText += @" and BranchId=@BranchId";

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
                if (vm.BranchId != "0_0" && vm.BranchId != "0" && vm.BranchId != "" && vm.BranchId != "null" && vm.BranchId != null)
                    da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);

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

        public DataTable ExportExcelFileFormPFOpening(EmployeePFOpeinigVM vm, string Filepath, string FileName)
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
,ISNULL(emp.Grade,'NA') Grade
,(case when emp.Designation is null and emp.Designation='=NA=' then 'NA' else emp.Designation end) Designation
,(case when emp.Department is null and emp.Department='=NA=' then 'NA' else emp.Department end) Department 
,(case when emp.Section is null and emp.Section='=NA=' then 'NA' else emp.Section end) Section
,(case when emp.Project is null and emp.Project='=NA=' then 'NA' else emp.Project end) Project
,ISNULL(EmployeeContribution,0) EmployeeContribution
,ISNULL(EmployerContribution,0) EmployerContribution
,ISNULL(EmployeeProfit,0) EmployeeProfit
,ISNULL(EmployerProfit,0) EmployerProfit
,OpeningDate

  FROM EmployeePFOpeinig pfo

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
                if (vm.BranchId != "0_0" && vm.BranchId != "0" && vm.BranchId != "" && vm.BranchId != "null" && vm.BranchId != null)
                    sqlText += @" and BranchId=@BranchId";

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
                if (vm.BranchId != "0_0" && vm.BranchId != "0" && vm.BranchId != "" && vm.BranchId != "null" && vm.BranchId != null)
                    da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
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
            retResults[5] = "EmployeePFOpeinig"; //Method Name
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
                    EmployeePFOpeinigVM vm = new EmployeePFOpeinigVM();

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
                        throw new ArgumentNullException("", "Employee Code " + item["EmpCode"].ToString() + " Not in System");
                    }
                    else
                    {

                        if (true)
                        {

                            #region Get Employee id

                            sqlText = "  ";

                            sqlText += " SELECT Id from EmployeePFOpeinig WHERE EmployeeId=@EmployeeId ";

                            cmdExist = new SqlCommand(sqlText, currConn);
                            cmdExist.Transaction = transaction;
                            cmdExist.Parameters.AddWithValue("@EmployeeId", empId);

                            var PFId = cmdExist.ExecuteScalar();
                            string PFOprningId = "";

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
                                sqlText += " SELECT isnull(Post,0) FROM EmployeePFOpeinig ";
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


        //==================Insert GF Forfeiture =================
        /// <summary>
        /// Inserts a new forfeiture record for an employee into the EmployeeForFeiture_New table.
        /// Checks for existing records with the same employee ID and forfeiture date to prevent duplicates.
        /// Manages the SQL connection and transaction, optionally using provided ones.
        /// </summary>
        /// <param name="vm">The view model containing forfeiture details to insert.</param>
        /// <param name="VcurrConn">An optional existing SQL connection to use. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">An optional existing SQL transaction to use. If null, a new transaction will be started.</param>
        /// <returns>
        /// A string array containing:
        /// [0] - Status ("Success" or "Fail").
        /// [1] - Message related to the operation result or error.
        /// [2] - ID of the inserted record (if successful).
        /// [3] - The SQL query executed (for logging/debugging).
        /// [4] - Exception message if any occurred.
        /// [5] - The method name ("InsertGFForfeiture").
        /// </returns>
        public string[] InsertForfeiture(EmployeePFOpeinigVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertGFForfeiture"; //Method Name
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

                //#region Exist

                //sqlText = "  ";
                //sqlText += " SELECT   count(Id) FROM EmployeePFOpeinig ";
                //sqlText += @" WHERE EmployeeId=@EmployeeId ";

                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                //var exeRes = cmdExist.ExecuteScalar();

                //int objfoundId = Convert.ToInt32(exeRes);

                //if (objfoundId > 0)
                //{
                //    throw new ArgumentNullException("This Employee already exits", "");
                //}

                //#endregion Exist

                #region Exist

                sqlText = "";
                sqlText += " SELECT   count(Id) FROM EmployeeForFeiture_New ";
                sqlText += @" WHERE ForFeitureDate=@ForFeitureDate and EmployeeId=@EmployeeId ";

                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@ForFeitureDate", Ordinary.DateToString(vm.ForFeitureDate));
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                var exeRes = cmdExist.ExecuteScalar();

                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    throw new ArgumentNullException("Same Date Trasaction already exits", "");
                }

                #endregion Exist


                #region Save

                vm.Id = cdal.NextId("EmployeeForFeiture_New", currConn, transaction).ToString();

                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeForFeiture_New
(
 Id
,EmployeeId
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit

,ForFeitureDate
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
, @ForFeitureDate
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
                    cmdInsert.Parameters.AddWithValue("@ForFeitureDate", Ordinary.DateToString(vm.ForFeitureDate));
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
                    retResults[1] = "Please Input Employee ForFeiture Value";
                    throw new ArgumentNullException("Please Input Employee ForFeiture Value", "");
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

        public string[] UpdateForFeiture(EmployeePFOpeinigVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFOpeinig Update"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePFOpeinig"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeForFeiture_New ";
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
                    sqlText += " SELECT isnull(Post,0) FROM EmployeeForFeiture_New ";
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
                    sqlText = "update EmployeeForFeiture_New set";
                    sqlText += " EmployeeContribution=@EmployeeContribution,";
                    sqlText += " EmployerContribution=@EmployerContribution,";
                    sqlText += " EmployeeProfit=@EmployeeProfit,";
                    sqlText += " EmployerProfit=@EmployerProfit,";
                    sqlText += " ForFeitureDate=@ForFeitureDate,";
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
                    cmdUpdate.Parameters.AddWithValue("@ForFeitureDate", Ordinary.DateToString(vm.ForFeitureDate));
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
                    retResults[1] = "Data Update Successfully.";

                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update ForFeiture.";
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

        /// <summary>
        /// Posts the forfeiture information for an employee by updating the corresponding record in the database.
        /// Performs existence checks, validates the post status, and updates the post flag along with audit fields within a transaction.
        /// </summary>
        /// <param name="vm">The view model containing employee forfeiture data to be posted.</param>
        /// <param name="VcurrConn">An optional existing SQL connection. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">An optional existing SQL transaction. If null, a new transaction will be started.</param>
        /// <returns>
        /// An array of strings containing the result status and related information:
        /// [0] - "Success" or "Fail" indicating operation result.
        /// [1] - Message detailing success or failure reason.
        /// [2] - The Id of the processed record as string.
        /// [3] - The SQL query executed as string.
        /// [4] - Exception message if any error occurred.
        /// [5] - The name of the method ("EmployeePFOpeinig Post").
        /// </returns>
        public string[] PostForFeiture(EmployeePFOpeinigVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFOpeinig Post"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePFOpeinig"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeeForFeiture_New ";
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
                    sqlText += " SELECT isnull(Post,0) FROM EmployeeForFeiture_New ";
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
                    sqlText = "update EmployeeForFeiture_New set";
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
                    retResults[1] = "Unexpected error to Post Employee ForFeiture.";
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

        public string[] MultiplePostForFeiture(EmployeePFOpeinigVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeePFOpeinig"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("PostToEmployeePFOpeinig"); }

                #endregion open connection and transaction

                if (Ids.Length > 1)
                {
                    #region Update Settings

                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        #region Exist

                        sqlText = "  ";
                        sqlText += " SELECT EmployeeId FROM EmployeeForFeiture_New ";
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

                        retResults = PostForFeiture(vm, currConn, transaction);

                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

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
                    retResults[1] = "Unexpected error to delete Employee ForFeiture Information.";
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

    }
}
