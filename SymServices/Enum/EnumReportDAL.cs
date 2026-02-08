using SymOrdinary;
using SymViewModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace SymServices.Enum
{
    public class EnumReportDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion


        public List<EnumReportVM> DropDown(string ReportType)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EnumReportVM> VMs = new List<EnumReportVM>();
            EnumReportVM vm;
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
Id
,ReportId
,ISNULL(ReportSL,0) ReportSL
,CONCAT(ISNULL(ReportSL,0),'. ',ReportName) ReportName
,ReportType
,ReportFileName
   FROM EnumReport
WHERE 1=1 and isnull(Isvisible,1)=1
";
                if (!string.IsNullOrWhiteSpace(ReportType))
                {
                    sqlText += @" and   ReportType=@ReportType";
                }
                sqlText += @" ORDER BY ReportSL, ReportName";


                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(ReportType))
                {
                    _objComm.Parameters.AddWithValue("@ReportType", ReportType);
                }
                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EnumReportVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ReportId = dr["ReportId"].ToString();
                    vm.ReportSL = Convert.ToInt32(dr["ReportSL"]);
                    vm.ReportName = dr["ReportName"].ToString();
                    vm.Name = vm.ReportName;
                    //////vm.Name = vm.ReportSL+". "+vm.ReportName;
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
        public List<EnumReportVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EnumReportVM> VMs = new List<EnumReportVM>();
            EnumReportVM vm;
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
SELECT
Id
,ReportId
,ISNULL(ReportSL,0) ReportSL
,ReportName
,ReportType
,ReportFileName
from EnumReport
WHERE  1=1 
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
                    vm = new EnumReportVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.ReportId = dr["ReportId"].ToString();
                    vm.ReportName = dr["ReportName"].ToString();
                    vm.ReportType = dr["ReportType"].ToString();
                    vm.ReportFileName = dr["ReportFileName"].ToString();
                    vm.Name = vm.ReportName;

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
    }
}
