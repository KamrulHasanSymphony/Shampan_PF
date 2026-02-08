using SymOrdinary;
using SymServices.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.WPPF
{
   public class PFStructureDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<PFStructureVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<PFStructureVM> PFStructureVMs = new List<PFStructureVM>();
            PFStructureVM PFStructureVM;
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

                sqlText = @"SELECT
Id
,BranchId
,Code
,Name
,isnull(PFValue  ,0)PFValue
,IsFixed
,PortionSalaryType
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From PFStructure
Where IsArchive=0
    ORDER BY Name
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    PFStructureVM = new PFStructureVM();
                    PFStructureVM.Id = dr["Id"].ToString();
                    PFStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    PFStructureVM.Code = dr["Code"].ToString();
                    PFStructureVM.Name = dr["Name"].ToString();
                    PFStructureVM.PFValue = Convert.ToDecimal(dr["PFValue"]);
                    PFStructureVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    PFStructureVM.Remarks = dr["Remarks"].ToString();
                    PFStructureVM.PortionSalaryType = dr["PortionSalaryType"].ToString();
                    PFStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    PFStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    PFStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                    PFStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    PFStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    PFStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    PFStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    PFStructureVMs.Add(PFStructureVM);
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

            return PFStructureVMs;
        }
        //==================SelectByID=================
        public PFStructureVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            PFStructureVM PFStructureVM = new PFStructureVM();

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

                sqlText = @"SELECT
Id
,BranchId
,Code
,Name
,isnull(PFValue  ,0)PFValue
,IsFixed
,PortionSalaryType
,Remarks
,IsActive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From PFStructure
where  id=@Id
     
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
                    PFStructureVM.Id = dr["Id"].ToString();
                    PFStructureVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    PFStructureVM.Code = dr["Code"].ToString();
                    PFStructureVM.Name = dr["Name"].ToString();
                    PFStructureVM.PFValue = Convert.ToDecimal(dr["PFValue"]);
                    PFStructureVM.IsFixed = Convert.ToBoolean(dr["IsFixed"]);
                    PFStructureVM.Remarks = dr["Remarks"].ToString();
                    PFStructureVM.PortionSalaryType = dr["PortionSalaryType"].ToString();
                    PFStructureVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    PFStructureVM.CreatedAt = Ordinary.StringToDate( dr["CreatedAt"].ToString());
                    PFStructureVM.CreatedBy = dr["CreatedBy"].ToString();
                    PFStructureVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    PFStructureVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    PFStructureVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    PFStructureVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

            return PFStructureVM;
        }
        //==================Insert =================
        public string[] Insert(PFStructureVM PFStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertPFStructure"; //Method Name


            #endregion
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            
            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(PFStructureVM.PFStructureId))
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

             
                #region Exist

                bool check = false;
                string[] fieldName = { "Name", "Code" };
                string[] fieldValue = { PFStructureVM.Name, PFStructureVM.Code };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert("PFStructure", fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = fieldName[i] + " already used!";
                        throw new ArgumentNullException(fieldName[i] + " already used!", fieldName[i] + " already used!");
                    }
                }
                #endregion Exist

                #region Save
                sqlText = "Select  isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0)  from PFStructure where BranchId=@BranchId";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Parameters.AddWithValue("@BranchId", PFStructureVM.BranchId);
                cmd2.Transaction = transaction;
               var exeRes = cmd2.ExecuteScalar();
                int count = Convert.ToInt32(exeRes);

                PFStructureVM.Id = PFStructureVM.BranchId.ToString() + "_" + (count + 1);
                //int foundId = (int)objfoundId;
                if (true)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO PFStructure(Id,BranchId,Code,Name,PFValue ,IsFixed,Remarks,PortionSalaryType,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom) 
                                VALUES (@Id,@BranchId,@Code,@Name,@PFValue ,@IsFixed,@Remarks,@PortionSalaryType,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom) 
                                        ";

                    SqlCommand cmdExist1 = new SqlCommand(sqlText, currConn);

                    cmdExist1.Parameters.AddWithValue("@Id", PFStructureVM.Id);
                    cmdExist1.Parameters.AddWithValue("@BranchId", PFStructureVM.BranchId);
                    cmdExist1.Parameters.AddWithValue("@Code", PFStructureVM.Code);
                    cmdExist1.Parameters.AddWithValue("@PFValue", PFStructureVM.PFValue);
                    cmdExist1.Parameters.AddWithValue("@IsFixed", PFStructureVM.IsFixed);
                    cmdExist1.Parameters.AddWithValue("@Name", PFStructureVM.Name);
                    cmdExist1.Parameters.AddWithValue("@PortionSalaryType", PFStructureVM.PortionSalaryType ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@Remarks", PFStructureVM.Remarks ?? Convert.DBNull);
                    cmdExist1.Parameters.AddWithValue("@IsActive", true);
                    cmdExist1.Parameters.AddWithValue("@IsArchive", false);
                    cmdExist1.Parameters.AddWithValue("@CreatedBy", PFStructureVM.CreatedBy);
                    cmdExist1.Parameters.AddWithValue("@CreatedAt", PFStructureVM.CreatedAt);
                    cmdExist1.Parameters.AddWithValue("@CreatedFrom", PFStructureVM.CreatedFrom);

                    cmdExist1.Transaction = transaction;
                    cmdExist1.ExecuteNonQuery();


                }
                else
                {
                    retResults[1] = "This PFStructure already used";
                    throw new ArgumentNullException("Please Input PFStructure Value", "");
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
                retResults[2] = PFStructureVM.Id;

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
        public string[] Update(PFStructureVM PFStructureVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee PFStructure Update"; //Method Name

            int transResult = 0;

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToPFStructure"); }

                #endregion open connection and transaction
                 #region Exist

                bool check = false;
                string[] fieldName = { "Name", "Code" };
                string[] fieldValue = { PFStructureVM.Name, PFStructureVM.Code };
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdate(PFStructureVM.Id,"PFStructure", fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = fieldName[i] + " already used!";
                        throw new ArgumentNullException(fieldName[i] + " already used!", fieldName[i] + " already used!");
                    }
                }
                #endregion Exist

                if (PFStructureVM != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update PFStructure set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " PortionSalaryType=@PortionSalaryType,";
                    
                    sqlText += " PFValue=@PFValue,";
                    sqlText += " IsFixed=@IsFixed,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", PFStructureVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", PFStructureVM.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", PFStructureVM.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", PFStructureVM.Name);
                    cmdUpdate.Parameters.AddWithValue("@PortionSalaryType", PFStructureVM.PortionSalaryType ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PFValue", PFStructureVM.PFValue);
                    cmdUpdate.Parameters.AddWithValue("@IsFixed", PFStructureVM.IsFixed);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", PFStructureVM.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", PFStructureVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", PFStructureVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", PFStructureVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
var  exeRes = cmdUpdate.ExecuteNonQuery();
transResult = Convert.ToInt32(exeRes);

                    retResults[2] = PFStructureVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", PFStructureVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PFStructure Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update PFStructure.";
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

        //==================Delete =================
        public string[] Delete(PFStructureVM PFStructureVM, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeletePFStructure"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToPFStructure"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (Ids.Length > 1)
                {
                    #region Update Settings
                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        sqlText = "";
                        sqlText = "update PFStructure set";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", Ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", PFStructureVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", PFStructureVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", PFStructureVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
var exeRes = cmdUpdate.ExecuteNonQuery();
transResult = Convert.ToInt32(exeRes);
                    }




                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("PFStructure Delete", PFStructureVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("PFStructure Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete PFStructure Information.";
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
        public List<PFStructureVM> DropDown(int branch)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<PFStructureVM> VMs = new List<PFStructureVM>();
            PFStructureVM vm;
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

                sqlText = @"SELECT
Id,
Name
   FROM PFStructure
WHERE IsArchive=0 and IsActive=1 and BranchId=@branch
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                _objComm.Parameters.AddWithValue("@branch", branch);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new PFStructureVM();
                    vm.Id = dr["Id"].ToString();
                    vm.Name = dr["Name"].ToString();
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
                sqlText = "";
                sqlText = @"SELECT Id, Name    FROM PFStructure ";
                sqlText += @" WHERE Name like '%" + term + "%' and IsArchive=0 and IsActive=1 ORDER BY Name";



                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    vms.Insert(i, dr["Name"].ToString());
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

        #endregion
    }
}
