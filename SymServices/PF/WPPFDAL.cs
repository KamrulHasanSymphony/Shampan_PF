using Excel;
using SymOrdinary;
using SymServices.Common;

using SymServices.Payroll;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace SymServices.PF
{
    public class WPPFDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion

        #region Methods

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
,pfd.TotalProfitValue TotalPF
,pfd.WPPFValue
,pfd.WWFValue
,pfd.BWWF


FROM WPPFHeader pfd
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
                    vm.TotalPF = Convert.ToDecimal(dr["TotalPF"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.EmployeePFValue = Convert.ToDecimal(dr["WPPFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["WWFValue"]);
                    vm.TotalEmployeeValue = Convert.ToDecimal(dr["BWWF"]);

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

        public List<PFHeaderVM> SelectProfitDistribution(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText = @"Select a.Id, b.Code As WPPFCode, c.Name, a.DistributionDate, a.EmployeeProfit, a.Post  From WPPFProfitDistribution a";

                sqlText += "  left join WPPFHeader b on a.WPPFHeaderId = b.Id";
                sqlText += "  left join EmployeeInfo c on a.EmployeeId = c.Id";
                sqlText += @" WHERE  1=1 AND a.IsArchive = 0 ";
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
                sqlText += " ORDER BY c.Name";

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
                    vm.Code = dr["WPPFCode"].ToString();
                    vm.ProjectName = dr["Name"].ToString();
                    vm.DistributionDate = Ordinary.StringToDate(dr["DistributionDate"].ToString());
                    vm.TotalPF = Convert.ToDecimal(dr["EmployeeProfit"]);
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

        public string[] Insert(decimal? TotalProfit, string FiscalYearDetailId, int? FiscalYear, ShampanIdentityVM auditvm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] ret = new string[6] { "Fail", "Fail", "0", "", "", "PFProcess" };

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                // Open connection
                currConn = VcurrConn ?? _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                    currConn.Open();

                transaction = Vtransaction ?? currConn.BeginTransaction();

                decimal distValue = TotalProfit.Value * 0.05m;
                decimal disWPPF = distValue * 0.8m;
                decimal disWFF = distValue * 0.1m;
                decimal disWWF = distValue * 0.1m;

                string distributionDate;
                string fySql = @"
            SELECT PeriodEnd 
            FROM FiscalYearDetail 
            WHERE Id = @FiscalYearDetailId";

                using (SqlCommand fyCmd = new SqlCommand(fySql, currConn, transaction))
                {
                    fyCmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                    object obj = fyCmd.ExecuteScalar();
                    distributionDate = obj == null || obj == DBNull.Value ? DateTime.Now.ToString() : obj.ToString();
                }

                string NewCode = new CommonDAL().CodeGenerationPF("PF", "WPPF", DateTime.Now.ToString(), currConn, transaction);

               
                string checkPostSql = @"
            SELECT Post
            FROM WPPFHeader
            WHERE FiscalYearDetailId = @FiscalYearDetailId
            AND Year = @Year";

                SqlCommand checkPostCmd = new SqlCommand(checkPostSql, currConn, transaction);
                checkPostCmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                checkPostCmd.Parameters.AddWithValue("@Year", FiscalYear);

                object postStatus = checkPostCmd.ExecuteScalar();
                bool isPostExists = postStatus != null && Convert.ToInt32(postStatus) == 1;

                if (isPostExists)
                {
                   
                    ret[0] = "Fail";
                    ret[1] = "WPPF already posted for this fiscal year.";
                    return ret;
                }
                else
                {
                    
                    string checkExistingDataSql = @"
                SELECT Id
                FROM WPPFHeader
                WHERE FiscalYearDetailId = @FiscalYearDetailId
                AND Year = @Year
                AND Post = 0";

                    SqlCommand checkExistingDataCmd = new SqlCommand(checkExistingDataSql, currConn, transaction);
                    checkExistingDataCmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                    checkExistingDataCmd.Parameters.AddWithValue("@Year", FiscalYear);

                    object existingDataId = checkExistingDataCmd.ExecuteScalar();
                    int id = existingDataId != null ? Convert.ToInt32(existingDataId) : 0;

                    if (id != 0)
                    {
                       
                        string archiveSql = @"
                    Delete  from WPPFHeader
                    WHERE Id = @Id";

                        SqlCommand archiveCmd = new SqlCommand(archiveSql, currConn, transaction);
                        archiveCmd.Parameters.AddWithValue("@Id", id);
                        archiveCmd.ExecuteNonQuery();

                        string archiveDetailsSql = @"
                    Delete From WPPFProfitDistribution
                    WHERE WPPFHeaderId = @WPPFHeaderId";

                        SqlCommand archiveDetailsCmd = new SqlCommand(archiveDetailsSql, currConn, transaction);
                        archiveDetailsCmd.Parameters.AddWithValue("@WPPFHeaderId", id);
                        archiveDetailsCmd.ExecuteNonQuery();
                    }

                   
                    string insertSql = @"
            INSERT INTO WPPFHeader
            (
                Code, FiscalYearDetailId, Year, ProjectId, TotalProfitValue, Post, Remarks, IsActive, IsArchive,
                CreatedBy, CreatedAt, CreatedFrom, WPPFValue, WWFValue, BWWF
            )
            VALUES
            (
                @Code, @FiscalYearDetailId, @Year, @ProjectId, @TotalProfitValue, 0, @Remarks, 1, 0, 
                @CreatedBy, @CreatedAt, @CreatedFrom, @WPPFValue, @WWFValue, @BWWF
            );

            SELECT SCOPE_IDENTITY();";

                    SqlCommand cmd = new SqlCommand(insertSql, currConn, transaction);
                    cmd.Parameters.AddWithValue("@Code", NewCode);
                    cmd.Parameters.AddWithValue("@FiscalYearDetailId", FiscalYearDetailId);
                    cmd.Parameters.AddWithValue("@TotalProfitValue", TotalProfit);
                    cmd.Parameters.AddWithValue("@Year", FiscalYear);
                    cmd.Parameters.AddWithValue("@ProjectId", "1_1");
                    cmd.Parameters.AddWithValue("@Remarks", "");
                    cmd.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedAt", auditvm.CreatedAt);
                    cmd.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);
                    cmd.Parameters.AddWithValue("@WPPFValue", disWPPF);
                    cmd.Parameters.AddWithValue("@WWFValue", disWFF);
                    cmd.Parameters.AddWithValue("@BWWF", disWWF);

                    decimal newId = Convert.ToDecimal(cmd.ExecuteScalar());

                    
                    string empSql = @"
                SELECT E.Id
                FROM EmployeeInfo E
                WHERE E.IsActive = 1
                ORDER BY E.Id";

                    SqlCommand empCmd = new SqlCommand(empSql, currConn, transaction);
                    SqlDataAdapter daEmp = new SqlDataAdapter(empCmd);
                    DataTable dt = new DataTable();
                    daEmp.Fill(dt);

                    decimal disPerson = dt.Rows.Count;
                    decimal disEmpValue = disWPPF / disPerson;

                    foreach (DataRow dr in dt.Rows)
                    {
                        int empId = Convert.ToInt32(dr["Id"]);

                        string insertDetailSql = @"
                    INSERT INTO WPPFProfitDistribution
                    (
                        WPPFHeaderId, EmployeeId, DistributionDate, EmployeeProfit, Post, IsPaid, Remarks, IsActive, IsArchive,
                        CreatedBy, CreatedAt, CreatedFrom
                    )
                    VALUES
                    (
                        @WPPFHeaderId, @EmployeeId, @DistributionDate, @EmployeeProfit, 0, 0, '', 1, 0, 
                        @CreatedBy, GETDATE(), @CreatedFrom
                    );";

                        SqlCommand detailCmd = new SqlCommand(insertDetailSql, currConn, transaction);
                        detailCmd.Parameters.AddWithValue("@WPPFHeaderId", newId);
                        detailCmd.Parameters.AddWithValue("@EmployeeId", empId);
                        detailCmd.Parameters.AddWithValue("@DistributionDate", distributionDate);
                        detailCmd.Parameters.AddWithValue("@EmployeeProfit", disEmpValue);
                        detailCmd.Parameters.AddWithValue("@CreatedBy", auditvm.CreatedBy);
                        detailCmd.Parameters.AddWithValue("@CreatedFrom", auditvm.CreatedFrom);

                        detailCmd.ExecuteNonQuery();
                    }

                    if (Vtransaction == null)
                        transaction.Commit();

                    ret[0] = "Success";
                    ret[1] = "WPPF successfully saved";
                    ret[2] = newId.ToString();

                    return ret;
                }
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                    transaction.Rollback();

                ret[1] = ex.Message;
                return ret;
            }
            finally
            {
                if (VcurrConn == null && currConn != null)
                    currConn.Close();
            }
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
            retResults[5] = " WPPF Post"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post WPPF"); }
                #endregion open connection and transaction
                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "update WPPFHeader set";
                    sqlText += "  Post=@Post";

                    sqlText += @" where Id=@Id
                    update WPPFProfitDistribution set Post=@Post where WPPFHeaderId=@Id
                    UPDATE BWWFHeader SET Post=@Post WHERE WPPFHeaderId=@Id
                    UPDATE WWFHeader SET Post=@Post WHERE WPPFHeaderId=@Id";
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
                    throw new ArgumentNullException("Post WPPF", "Could not found any item.");
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

        public List<PFHeaderVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<PFHeaderVM> VMs = new List<PFHeaderVM>();
            PFHeaderVM vm;
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
    ,pfd.Code
    ,pfd.FiscalYearDetailId
    ,p.Name AS ProjectName
    ,p.Id AS ProjectId
    ,fyd.PeriodName
    ,fyd.PeriodStart
    ,pfd.Post 
    ,pfd.TotalProfitValue AS TotalPF
    ,pfd.WPPFValue
    ,pfd.WWFValue
    ,pfd.BWWF
FROM WPPFHeader pfd
";

                sqlText += " LEFT OUTER JOIN [dbo].[Project] p ON pfd.ProjectId = p.Id";
                sqlText += " LEFT OUTER JOIN [dbo].[FiscalYearDetail] fyd ON pfd.FiscalYearDetailId = fyd.Id";
                sqlText += " WHERE 1 = 1 AND pfd.IsArchive = 0";
                sqlText += " ORDER BY fyd.PeriodStart DESC";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();

                while (dr.Read())
                {
                    vm = new PFHeaderVM();

                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.ProjectName = dr["ProjectName"].ToString();
                    vm.ProjectId = dr["ProjectId"].ToString();
                    vm.FiscalPeriod = dr["PeriodName"].ToString();
                    vm.PeriodStart = Ordinary.StringToDate(dr["PeriodStart"].ToString());
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.TotalPF = Convert.ToDecimal(dr["TotalPF"]);
                    vm.EmployeePFValue = Convert.ToDecimal(dr["WPPFValue"]);
                    vm.EmployeerPFValue = Convert.ToDecimal(dr["WWFValue"]);
                    vm.TotalEmployeeValue = Convert.ToDecimal(dr["BWWF"]);

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
        #endregion Methods
    }
}
