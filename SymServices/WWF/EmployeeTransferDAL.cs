using Excel;
using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.WWF
{
    public class EmployeeTransferDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion


        public List<EmployeeTransferVM> SelectAll(string empid = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            SqlTransaction transaction = null;
            List<EmployeeTransferVM> vms = new List<EmployeeTransferVM>();
            EmployeeTransferVM vm;
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
                #endregion
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
                #endregion

                #region sql statement
             
                sqlText = @"
               SELECT et.Id
                       ,[EmployeeCode]
	                   ,ve.EmpName
                      ,b.Name [FromBranch]
                      ,b2.Name [ToBranch]
                      ,[TransferDate]    
                  FROM EmployeeTransfer et 
                  Left Join ViewEmployeeInformation ve on ve.Code=et.EmployeeCode
                  Left Join Branch b on b.Id=et.FromBranch  
                  Left Join Branch b2 on b2.Id=et.ToBranch
                ";             
             
                #endregion

                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);                
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTransferVM();
                    vm.Id = (dr["Id"]).ToString();
                    vm.Code = dr["EmployeeCode"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.FromBranch = dr["FromBranch"].ToString();
                    vm.ToBranch = dr["ToBranch"].ToString();
                    vm.TransferDate = Ordinary.StringToDate(dr["TransferDate"].ToString());
                   
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

        public List<EmployeeTransferVM> SelectAllList(string empid = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<EmployeeTransferVM> VMs = new List<EmployeeTransferVM>();
            EmployeeTransferVM vm;
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
                #endregion
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
                #endregion

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
From EmployeeForfeiture pfo
";
                sqlText += " left outer join ViewEmployeeInformation e on e.EmployeeId=pfo.Id";
                sqlText += " Where 1=1 and pfo.IsArchive=0 and pfo.IsActive=1  AND e.Code=@Code AND pfo.Id=@Id AND TRY_CONVERT(date, e.JoinDate, 106)>=@DateFrom AND TRY_CONVERT(date, e.JoinDate, 106)<=@DateTo";
                #endregion

                if (!string.IsNullOrEmpty(empid))
                {
                    sqlText += @" and pfo.EmployeeId=@EmployeeId ";
                }

                sqlText += @" ORDER BY pfo.EmployeeId";
                #endregion

                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Code", conditionValues[0]);
                objComm.Parameters.AddWithValue("@Id", conditionValues[1]);
                objComm.Parameters.AddWithValue("@DateFrom", conditionValues[2]);
                objComm.Parameters.AddWithValue("@DateTo", conditionValues[3]);

                if (!string.IsNullOrEmpty(empid))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", empid);
                }

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTransferVM();
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
                #endregion

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

        public EmployeeTransferVM SelectById(string Id, string empId)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            EmployeeTransferVM vm = new EmployeeTransferVM();
            #endregion

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion

                #region sql statement
                sqlText = @"
                     SELECT 
                    ve.Code
                    ,ve.EmpName
                    ,ve.BranchId [FromBranch]                    
                    From ViewEmployeeInformation ve                 
                    where ve.Code=@Id
                ";

                #endregion

                #region SqlExecution
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", empId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeTransferVM();                  
                    vm.Code = dr["Code"].ToString();
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.FromBranch = dr["FromBranch"].ToString();
               
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

        public EmployeeTransferVM SelectByIdAll(string Id, string empId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTransferVM vm = new EmployeeTransferVM();
            #endregion

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

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
From EmployeeForfeiture pfo
";
                sqlText += " left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId";
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
                    vm = new EmployeeTransferVM();
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

        public string[] Insert(EmployeeTransferVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int Id = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail"; // Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = Id.ToString(); // Return Id
            retResults[3] = sqlText; // SQL Query
            retResults[4] = "ex"; // catch ex
            retResults[5] = "InsertEmployeeTransfer"; // Method Name
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
                #endregion
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
                #endregion

                #region Save
                vm.Id = cdal.NextId("EmployeeTransfer", currConn, transaction).ToString();

                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeTransfer
(

EmployeeCode
,FromBranch
,ToBranch
,TransferDate
,Remarks
,CreatedBy
,CreatedAt
,CreatedFrom
) VALUES (
@EmployeeCode
,@FromBranch
,@ToBranch
,@TransferDate
,@Remarks
,@CreatedBy
,@CreatedAt
,@CreatedFrom
) ";

                    sqlText += @" Update EmployeeInfo set BranchId=@ToBranch where Code=@EmployeeCode ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@EmployeeCode", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@FromBranch", vm.FromBranch);
                    cmdInsert.Parameters.AddWithValue("@ToBranch", vm.ToBranch);
                    cmdInsert.Parameters.AddWithValue("@TransferDate", Ordinary.DateToString(vm.TransferDate));
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
                    cmdInsert.ExecuteNonQuery();
                }
                else
                {
                    retResults[1] = "Please Input Employee Transfer Value";
                    throw new ArgumentNullException("Please Input Employee Transfer Value", "");
                }
                #endregion

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion

                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Saved Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion
            }
            #endregion

            #region Catch and Finally
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message.ToString();

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

        public string[] Update(EmployeeTransferVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail"; // Success or Fail
            retResults[1] = "Fail"; // Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; // SQL Query
            retResults[4] = "ex"; // catch ex
            retResults[5] = "EmployeeTransfer Update"; // Method Name

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
                #endregion

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
                    transaction = currConn.BeginTransaction("UpdateToEmployeeTransfer");
                }
                #endregion

                if (vm != null)
                {
                    #region Exist
                    sqlText = "  ";
                    sqlText += " SELECT COUNT(Id) Id FROM EmployeeForfeiture ";
                    sqlText += " WHERE EmployeeId=@EmployeeId AND Id<>@Id";
                    SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                    cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                    var exeRes = cmdExist.ExecuteScalar();
                    int objfoundId = Convert.ToInt32(exeRes);

                    if (objfoundId < 0)
                    {
                        throw new ArgumentNullException("Could not find any item. Please add first.", "");
                    }
                    #endregion

                    #region Post Check
                    sqlText = "  ";
                    sqlText += " SELECT isnull(Post,0) FROM EmployeeForfeiture ";
                    sqlText += " WHERE Id=@Id";
                    cmdExist = new SqlCommand(sqlText, currConn);
                    cmdExist.Transaction = transaction;
                    cmdExist.Parameters.AddWithValue("@Id", vm.Id);

                    var postCheck = cmdExist.ExecuteScalar();
                    bool Checkpost = Convert.ToBoolean(postCheck);

                    if (Checkpost)
                    {
                        throw new ArgumentNullException("This transaction is already posted", "");
                    }
                    #endregion

                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE EmployeeForfeiture SET";
                    sqlText += " EmployeeContribution=@EmployeeContribution,";
                    sqlText += " EmployerContribution=@EmployerContribution,";
                    sqlText += " EmployeeProfit=@EmployeeProfit,";
                    sqlText += " EmployerProfit=@EmployerProfit,";
                    sqlText += " OpeningDate=@OpeningDate,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " WHERE Id=@Id";

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

                    retResults[2] = vm.Id.ToString(); // Return Id
                    retResults[3] = sqlText; // SQL Query

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Employee Transfer Update", "Could not be updated.");
                    }
                    #endregion

                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Could not find any item.", "");
                }

                if (iSTransSuccess)
                {
                    if (Vtransaction == null)
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }

                    retResults[0] = "Success";
                    retResults[1] = "Data Updated Successfully.";
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error occurred while updating EmployeeTransfer.";
                    throw new ArgumentNullException("", "");
                }
            }

            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                if (Vtransaction == null)
                {
                    transaction.Rollback();
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

        public EmployeeTransferVM GetCodeById(string id, string empId)
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            EmployeeTransferVM vm = new EmployeeTransferVM();
            #endregion

            try
            {
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion

                #region sql statement
                sqlText = @"
SELECT
 pfo.Id
,pfo.EmployeeId
,e.Code
From EmployeeForfeiture pfo
";
                sqlText += " left outer join ViewEmployeeInformation e on e.EmployeeId=pfo.Id";
                sqlText += " Where 1=1 and pfo.IsArchive=0 and pfo.IsActive=1";

                if (!string.IsNullOrEmpty(id))
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

                if (!string.IsNullOrEmpty(id))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                }
                if (!string.IsNullOrWhiteSpace(empId))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", empId);
                }

                SqlDataReader dr;
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    vm = new EmployeeTransferVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.Code = dr["Code"].ToString();
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
    }
}
