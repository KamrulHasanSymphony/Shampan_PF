using Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.PF
{
    public class EmployeeLeftInformationDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<EmployeeLeftInformationVM> SelectAll(string branchId, int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeftInformationVM> VMs = new List<EmployeeLeftInformationVM>();
            EmployeeLeftInformationVM vm;
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
el.Id
,el.EmployeeId
,el.LeftType_E
,el.LeftDate
,el.EntryLeftDate
,el.FileName


,ve.BranchId
,ve.EmpName ,ve.Code, ve.Designation, ve.Branch, ve.Department, ve.Section, ve.Project 
,ve.JoinDate


,el.Remarks
,el.IsActive
,el.IsArchive
,isnull(el.IsSalalryProcess,0)IsSalalryProcess
,el.CreatedBy
,el.CreatedAt
,el.CreatedFrom
,el.LastUpdateBy
,el.LastUpdateAt
,el.LastUpdateFrom
From ViewEmployeeInformation ve
LEFT OUTER JOIN  EmployeeLeftInformation el ON ve.EmployeeId = el.EmployeeId
Where 1=1 and ve.IsArchive=1  and el.BranchId = @BranchId
    
";
                if (Id > 0)
                {
                    sqlText += " and el.Id=@Id";
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


               // sqlText += " ORDER BY el.LeftType_E";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

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
                objComm.Parameters.AddWithValue("@BranchId", branchId);
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeftInformationVM();
                    vm.Id = dr["Id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.LeftType_E = dr["LeftType_E"].ToString();
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
                    vm.EntryLeftDate = Ordinary.StringToDate(dr["EntryLeftDate"].ToString());

                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.EmpName = dr["EmpName"].ToString();
                    vm.Code = dr["Code"].ToString();
                    vm.Designation = dr["Designation"].ToString();
                    vm.Branch = dr["Branch"].ToString();
                    vm.Department = dr["Department"].ToString();
                    vm.Section = dr["Section"].ToString();
                    vm.Project = dr["Project"].ToString();
                    vm.JoinDate = Ordinary.StringToDate(dr["JoinDate"].ToString());



                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"]);
                    vm.IsSalalryProcess = Convert.ToBoolean(dr["IsSalalryProcess"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        //==================Insert =================
        public string[] Insert(EmployeeLeftInformationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertLeftInformation"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(vm.LeftType_E))
                {
                    retResults[1] = "Please Input Employee LeftInformation";
                    return retResults;
                }
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
                #region Exist
                sqlText = "  ";
                sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeLeftInformation ";
                sqlText += " WHERE EmployeeId=@EmployeeId And LeftType_E=@LeftType_E";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@LeftType_E", vm.LeftType_E);
                object objfoundId = cmdExist.ExecuteScalar();

                if (objfoundId == null)
                {
                    retResults[1] = "Please Input Employee Left Information Value";
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Please Input Employee Left Information Value", "");
                }
                #endregion Exist
                #region Save

                sqlText = "";
                sqlText = "update EmployeeInfo set";
                sqlText += " IsActive=@IsActive,";
                sqlText += " IsArchive=@IsArchive,";
                sqlText += " LastUpdateBy=@LastUpdateBy,";
                sqlText += " LastUpdateAt=@LastUpdateAt,";
                sqlText += " LastUpdateFrom=@LastUpdateFrom";
                sqlText += " where Id=@Id";

                SqlCommand cmdeInfo = new SqlCommand(sqlText, currConn, transaction);
                cmdeInfo.Parameters.AddWithValue("@Id", vm.EmployeeId);
                cmdeInfo.Parameters.AddWithValue("@IsActive", false);
                cmdeInfo.Parameters.AddWithValue("@IsArchive", true);

                cmdeInfo.Parameters.AddWithValue("@LastUpdateBy", vm.CreatedBy);
                cmdeInfo.Parameters.AddWithValue("@LastUpdateAt", vm.CreatedAt);
                cmdeInfo.Parameters.AddWithValue("@LastUpdateFrom", vm.CreatedFrom);

                try
                {
                    cmdeInfo.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Employee LeftInformation is not valid", "");
                }

                #region Update EmployeeJob Left Date
                sqlText = "";
                sqlText = "update EmployeeJob set";
                sqlText += " IsActive=@IsActive,";
                sqlText += " LeftDate=@LeftDate,";
                sqlText += " IsSalalryProcess=@IsSalalryProcess,";
                sqlText += " LastUpdateBy=@LastUpdateBy,";
                sqlText += " LastUpdateAt=@LastUpdateAt,";
                sqlText += " LastUpdateFrom=@LastUpdateFrom";
                sqlText += " where EmployeeId=@EmployeeId";

                SqlCommand cmdJobInfo = new SqlCommand(sqlText, currConn, transaction);
                cmdJobInfo.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdJobInfo.Parameters.AddWithValue("@LeftDate", Ordinary.DateToString(vm.LeftDate));
                cmdJobInfo.Parameters.AddWithValue("@IsActive", false);
                cmdJobInfo.Parameters.AddWithValue("@IsSalalryProcess", vm.IsSalalryProcess);


                cmdJobInfo.Parameters.AddWithValue("@LastUpdateBy", vm.CreatedBy);
                cmdJobInfo.Parameters.AddWithValue("@LastUpdateAt", vm.CreatedAt);
                cmdJobInfo.Parameters.AddWithValue("@LastUpdateFrom", vm.CreatedFrom);

                try
                {
                    cmdJobInfo.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw new ArgumentNullException("Employee LeftInformation is not valid", "");
                }
                #endregion Update EmployeeJob Left Date

                //int foundId = (int)objfoundId;
                if (true)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeeLeftInformation(	
EmployeeId,LeftType_E,LeftDate, EntryLeftDate, FileName
,Remarks,IsSalalryProcess,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,BranchId)
VALUES (
@EmployeeId,@LeftType_E,@LeftDate,@EntryLeftDate,@FileName
,@Remarks,@IsSalalryProcess,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@BranchId) 
SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                    cmdInsert.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdInsert.Parameters.AddWithValue("@LeftType_E", vm.LeftType_E);
                    cmdInsert.Parameters.AddWithValue("@LeftDate", Ordinary.DateToString(vm.LeftDate));
                    cmdInsert.Parameters.AddWithValue("@EntryLeftDate", Ordinary.DateToString(vm.EntryLeftDate));
                    cmdInsert.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@IsSalalryProcess", vm.IsSalalryProcess);
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdInsert.Transaction = transaction;
                    var exeRes = cmdInsert.ExecuteScalar();
                    Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Employee Left Information Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Employee Left Information Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Employee Left Information already used";
                    throw new ArgumentNullException("This Employee Left Information already used", "");
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
                retResults[2] = Id.ToString();

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
        public string[] Update(EmployeeLeftInformationVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee LeftInformation Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToLeftInformation"); }

                #endregion open connection and transaction
                #region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM EmployeeLeftInformation ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND LeftType_E=@LeftType_E";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", vm.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@LeftType_E", vm.LeftType_E);

                //var exeRes = cmdExist.ExecuteScalar();
                //int exists = Convert.ToInt32(exeRes);
                //if (exists > 0)
                //{
                //    retResults[1] = "This Left Information already used";
                //    throw new ArgumentNullException("This Left Information already used", "");
                //}
                #endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    #region Update EmployeeJob Left Date
                    sqlText = "";
                    sqlText = "update EmployeeJob set";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LeftDate=@LeftDate,";
                    sqlText += " IsSalalryProcess=@IsSalalryProcess,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where EmployeeId=@EmployeeId";

                    SqlCommand cmdJobInfo = new SqlCommand(sqlText, currConn, transaction);
                    cmdJobInfo.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdJobInfo.Parameters.AddWithValue("@LeftDate", Ordinary.DateToString(vm.LeftDate));
                    cmdJobInfo.Parameters.AddWithValue("@IsSalalryProcess", vm.IsSalalryProcess);
                    cmdJobInfo.Parameters.AddWithValue("@IsActive", false);


                    cmdJobInfo.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdJobInfo.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdJobInfo.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);

                    try
                    {
                        cmdJobInfo.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw new ArgumentNullException("Employee LeftInformation is not valid", "");
                    }
                    #endregion Update EmployeeJob Left Date


                    sqlText = "";
                    sqlText = "update EmployeeLeftInformation set";
                    sqlText += " EmployeeId=@EmployeeId,";
                    sqlText += " LeftType_E=@LeftType_E,";
                    sqlText += " LeftDate=@LeftDate,";
                    sqlText += " EntryLeftDate=@EntryLeftDate,";
                    sqlText += " IsSalalryProcess=@IsSalalryProcess,";
                    if (vm.FileName != null)
                    {
                        sqlText += " FileName=@FileName,";
                    }
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                    cmdUpdate.Parameters.AddWithValue("@LeftType_E", vm.LeftType_E);
                    cmdUpdate.Parameters.AddWithValue("@LeftDate", Ordinary.DateToString(vm.LeftDate));
                    cmdUpdate.Parameters.AddWithValue("@EntryLeftDate", Ordinary.DateToString(vm.EntryLeftDate));
                    if (vm.FileName != null)
                    {
                        cmdUpdate.Parameters.AddWithValue("@FileName", vm.FileName ?? Convert.DBNull);
                    }
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@IsSalalryProcess", vm.IsSalalryProcess);

                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("LeftInformation Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update LeftInformation.";
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
        //==================Select =================
        public EmployeeLeftInformationVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            EmployeeLeftInformationVM EmployeeLeftInformationVM = new EmployeeLeftInformationVM();

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
,EmployeeId
,LeftType_E
,LeftDate
,EntryLeftDate
,FileName
,Remarks
,IsActive
,CreatedAt 
,CreatedBy 
,CreatedFrom 
,LastUpdateAt
,LastUpdateBy
,LastUpdateFrom
FROM EmployeeLeftInformation    
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
                        EmployeeLeftInformationVM.Id = Convert.ToInt32(dr["Id"]);
                        EmployeeLeftInformationVM.EmployeeId = dr["EmployeeId"].ToString();
                        EmployeeLeftInformationVM.LeftType_E = dr["LeftType_E"].ToString();
                        EmployeeLeftInformationVM.LeftDate = dr["LeftDate"].ToString();
                        EmployeeLeftInformationVM.EntryLeftDate = dr["EntryLeftDate"].ToString();
                        EmployeeLeftInformationVM.FileName = dr["FileName"].ToString();
                        EmployeeLeftInformationVM.Remarks = dr["Remarks"].ToString();
                        EmployeeLeftInformationVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        EmployeeLeftInformationVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                        EmployeeLeftInformationVM.CreatedBy = dr["CreatedBy"].ToString();
                        EmployeeLeftInformationVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        EmployeeLeftInformationVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        EmployeeLeftInformationVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        EmployeeLeftInformationVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return EmployeeLeftInformationVM;
        }
        ////==================Delete =================
        public string[] Delete(EmployeeLeftInformationVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeeLeftInformation"; //Method Name
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
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("Delete"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    CommonDAL _cDal = new CommonDAL();
                    string employeeId = "";
                    #region Delete Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        employeeId = _cDal.SelectFieldValue("EmployeeLeftInformation", "EmployeeId", "Id", ids[i], currConn, transaction);
                        vm.EmployeeId = employeeId;

                        #region Update EmployeeInfo
                        sqlText = "";
                        sqlText = "update EmployeeInfo set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdeInfo = new SqlCommand(sqlText, currConn, transaction);
                        cmdeInfo.Parameters.AddWithValue("@Id", vm.EmployeeId);
                        cmdeInfo.Parameters.AddWithValue("@IsActive", true);
                        cmdeInfo.Parameters.AddWithValue("@IsArchive", false);

                        cmdeInfo.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdeInfo.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdeInfo.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdeInfo.ExecuteNonQuery();
                        #endregion Update EmployeeInfo

                        #region Update EmployeeJob

                        sqlText = "";
                        sqlText = "update EmployeeJob set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LeftDate=@LeftDate,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where EmployeeId=@EmployeeId";

                        SqlCommand cmdJobInfo = new SqlCommand(sqlText, currConn, transaction);
                        cmdJobInfo.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                        cmdJobInfo.Parameters.AddWithValue("@LeftDate", "19000101");
                        cmdJobInfo.Parameters.AddWithValue("@IsActive", true);
                        cmdJobInfo.Parameters.AddWithValue("@IsArchive", false);


                        cmdJobInfo.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                        cmdJobInfo.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                        cmdJobInfo.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                        cmdJobInfo.ExecuteNonQuery();
                        #endregion Update EmployeeJob

                        sqlText = "";
                        sqlText = "Delete EmployeeLeftInformation WHERE 1=1 and Id=@Id";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("EmployeeLeftInformation Delete", vm.Id + " could not Delete.");
                    }
                    #endregion Commit
                    #endregion Delete Settings
                }
                else
                {
                    throw new ArgumentNullException("EmployeeLeftInformation Information Delete", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Employee Re-Active Successfully.";
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


        //==================SelectAll=================
        public List<EmployeeLeftInformationVM> SelectAllForReport(string CodeF, string CodeT, string DepartmentId, string SectionId, string ProjectId
          , string DesignationId, string dtpFrom, string dtpTo, string EmployeeId)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeeLeftInformationVM> VMs = new List<EmployeeLeftInformationVM>();
            EmployeeLeftInformationVM vm;
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
 isnull(LInfo.Id,0)						  Id
,ei.EmployeeId							EmployeeId
,isnull(LInfo.LeftType_E, 'NA')			 LeftType_E
,isnull(LInfo.LeftDate,'NA')			 LeftDate
,isnull(LInfo.EntryLeftDate,'NA')		EntryLeftDate
,isnull(LInfo.[FileName]	, 'NA')		[FileName]		
,isnull(LInfo.Remarks		, 'NA')			Remarks
,isnull(LInfo.IsActive, 0)			IsActive
,isnull(LInfo.IsArchive, 0)			IsArchive
,isnull(LInfo.CreatedBy, 'NA')		 CreatedBy
,isnull(LInfo.CreatedAt, 'NA')		 CreatedAt
,isnull(LInfo.CreatedFrom, 'NA')		CreatedFrom
,isnull(LInfo.LastUpdateBy, 'NA')	 LastUpdateBy
,isnull(LInfo.LastUpdateAt,	'NA')	 LastUpdateAt
,isnull(LInfo.LastUpdateFrom,	'NA')	 LastUpdateFrom   

    From ViewEmployeeInformation ei
		left outer join EmployeeLeftInformation LInfo on ei.EmployeeId=LInfo.EmployeeId
Where ei.IsArchive=0 and ei.isActive=1
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
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    sqlText += "  and ei.EmployeeId=@EmployeeId";
                }
                sqlText += "  order by ei.Department, ei.GradeSL, ei.joindate, ei.Code ";
                sqlText += " ,LInfo.LeftType_E ";
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
                if (!string.IsNullOrWhiteSpace(EmployeeId))
                {
                    objComm.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new EmployeeLeftInformationVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.LeftType_E = dr["LeftType_E"].ToString();
                    vm.LeftDate = Ordinary.StringToDate(dr["LeftDate"].ToString());
                    vm.EntryLeftDate = Ordinary.StringToDate(dr["EntryLeftDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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
        #endregion

    }
}
