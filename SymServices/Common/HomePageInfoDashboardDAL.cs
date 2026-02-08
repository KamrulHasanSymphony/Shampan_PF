using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using JQueryDataTables.Models;
using System.Threading;
using System.Web.Mvc;

namespace SymServices.Common
{
    public class HomePageInfoDashboardDAL
    {
        #region Global Variables
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        public string FieldDelimeter { get; set; }
        public AdminInfoDashboardVM GetAdminInfoDashboard()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            AdminInfoDashboardVM vm = new AdminInfoDashboardVM();
            #endregion

            try
            {
                #region Open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region SQL statement
                sqlText = @"  SELECT    
                        ISNULL(SUM(CASE WHEN AttnStatus = 'Present' THEN 1 ELSE 0 END),0) AS Present,
                        ISNULL(SUM(CASE WHEN AttnStatus = 'Absent' THEN 1 ELSE 0 END),0) AS Absent,
                        ISNULL(SUM(CASE WHEN AttnStatus = 'Late' THEN 1 ELSE 0 END),0) AS Late,
                        ISNULL(SUM(CASE WHEN AttnStatus = 'In Miss' THEN 1 ELSE 0 END),0) AS InMiss  
                        FROM AttendanceDailyNew where DailyDate= DATEADD(DAY, -1, CAST(GETDATE() AS DATE))";
                using (SqlCommand objComm = new SqlCommand(sqlText, currConn))
                {
                    objComm.CommandType = CommandType.Text;

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vm.Present = dr["Present"].ToString();
                            vm.Absent = dr["Absent"].ToString();
                            vm.Late = dr["Late"].ToString();
                            vm.InMiss = dr["InMiss"].ToString();
                        }
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return vm;
        }
        public HrmInfoDashboardVM GetHrmInfoDashboard()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
           
            HrmInfoDashboardVM vms = new HrmInfoDashboardVM();
            #endregion

            try
            {
                #region Open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region SQL statement
                sqlText = @"
                    SELECT 
                        COUNT(ef.Code) AS TotalEmployee,
                        SUM(CASE WHEN ef.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveEmployee,
                        SUM(CASE WHEN ef.IsActive = 0 THEN 1 ELSE 0 END) AS InactiveEmployee,
                        SUM(CASE WHEN ISNULL(pd.Gender_E, 'NA') = 'Male' THEN 1 ELSE 0 END) AS Male,
                        SUM(CASE WHEN ISNULL(pd.Gender_E, 'NA') = 'Female' THEN 1 ELSE 0 END) AS Female,
                        SUM(CASE WHEN ISNULL(pd.Gender_E, 'NA') = 'NA' or ISNULL(pd.Gender_E, 'NA') = 'Other' THEN 1 ELSE 0 END) AS NotApplicable
                    FROM EmployeeInfo ef
                    LEFT OUTER JOIN EmployeePersonalDetail pd ON pd.EmployeeId = ef.Id";

                using (SqlCommand objComm = new SqlCommand(sqlText, currConn))
                {
                    objComm.CommandType = CommandType.Text;

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        if (dr.Read())
                        {

                            vms.TotalEmployee = Convert.ToDecimal(dr["TotalEmployee"]);
                            vms.ActiveEmployee = Convert.ToDecimal(dr["ActiveEmployee"]);
                            vms.InactiveEmployee = Convert.ToDecimal(dr["InactiveEmployee"]);
                            vms.Male = Convert.ToDecimal(dr["Male"]);
                            vms.Female = Convert.ToDecimal(dr["Female"]);
                            vms.NotApplicable = Convert.ToDecimal(dr["NotApplicable"]);
                         
                        }
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return vms;
        }
        public PayrollInfoDashboardVM GetPayrollInfoDashboard()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";

            PayrollInfoDashboardVM vm = new PayrollInfoDashboardVM();
            #endregion

            try
            {
                #region Open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region SQL statement
                sqlText = @"SELECT
                           ISNULL( COUNT(FiscalYearDetailId),0) AS TotalPerson,
                             ISNULL(SUM(GrossSalary),0) AS GrossSalary    
                            FROM
                            MonthlyAttendance
                            where [FiscalYearDetailId] = (Select MAX([FiscalYearDetailId]) from MonthlyAttendance)";

                using (SqlCommand objComm = new SqlCommand(sqlText, currConn))
                {
                    objComm.CommandType = CommandType.Text;

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vm.TotalPerson = Convert.ToDecimal(dr["TotalPerson"].ToString());

                            vm.GrossSalary = Convert.ToDecimal(dr["GrossSalary"].ToString());                           
                        }
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return vm;
        }
        public TaxInfoDashboardVM GetTaxInfoDashboard()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";

            TaxInfoDashboardVM vm = new TaxInfoDashboardVM();
            #endregion

            try
            {
                #region Open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region SQL statement
                sqlText = @"SELECT COUNT(FiscalYearDetailId) AS TotalPerson
                              , ISNULL(SUM(TaxValue),0) AS TaxValue    
                          FROM SalaryTaxDetail
                          where [FiscalYearDetailId] = (Select MAX([FiscalYearDetailId]) from SalaryTaxDetail) and TaxValue>0";

                using (SqlCommand objComm = new SqlCommand(sqlText, currConn))
                {
                    objComm.CommandType = CommandType.Text;

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vm.TotalPerson = Convert.ToDecimal(dr["TotalPerson"].ToString());
                            vm.TaxValue = Convert.ToDecimal(dr["TaxValue"].ToString());
                        }
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return vm;
        }
        public PfInfoDashboardVM GetPfInfoDashboard()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";

            PfInfoDashboardVM vm = new PfInfoDashboardVM();
            #endregion

            try
            {
                #region Open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region SQL statement
                sqlText = @"SELECT COUNT(FiscalYearDetailId) AS TotalPerson
                              , ISNULL(SUM(PFValue),0) AS PFValue    
                          FROM SalaryPFDetail
                          where [FiscalYearDetailId] = (Select MAX([FiscalYearDetailId]) from SalaryPFDetail) and PFValue>0";

                using (SqlCommand objComm = new SqlCommand(sqlText, currConn))
                {
                    objComm.CommandType = CommandType.Text;

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vm.TotalPerson = Convert.ToDecimal(dr["TotalPerson"].ToString());
                            vm.PFValue = Convert.ToDecimal(dr["PFValue"].ToString());
                        }
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return vm;
        }
        public GfInfoDashboardVM GetGfInfoDashboard()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";

            GfInfoDashboardVM vm = new GfInfoDashboardVM();
            #endregion

            try
            {
                #region Open connection
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region SQL statement
                sqlText = @"SELECT COUNT(FiscalYearDetailId) AS TotalPerson
                              ,ISNULL(SUM(ProvisionAmount),0) AS GFValue    
                          FROM GFEmployeeProvisions
                          where [FiscalYearDetailId] = (Select MAX([FiscalYearDetailId]) from GFEmployeeProvisions)";

                using (SqlCommand objComm = new SqlCommand(sqlText, currConn))
                {
                    objComm.CommandType = CommandType.Text;

                    using (SqlDataReader dr = objComm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            vm.TotalPerson = Convert.ToDecimal(dr["TotalPerson"].ToString());
                            vm.GFValue = Convert.ToDecimal(dr["GFValue"].ToString());
                        }
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
                if (currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return vm;
        }
    }
}
