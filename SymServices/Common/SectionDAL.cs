using Excel;
using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace SymServices.Common
{
    public class SectionDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        #region Methods
        //==================SelectAll=================
        public List<SectionVM> SelectAll()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SectionVM> vms = new List<SectionVM>();
            SectionVM vm;
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
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,ISNULL(OrderNo,0)OrderNo
    From Section
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
                    vm = new SectionVM();
                    vm.Id = dr["Id"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());
                    vms.Add(vm);
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

            return vms;
        }
        //==================SelectByID=================
        public SectionVM SelectById(string Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            SectionVM sectionVM = new SectionVM();

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
,Code
,BranchId
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,ISNULL(OrderNo,0)OrderNo
From Section
Where  id=@Id  and IsArchive=0
     
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
                    sectionVM.Id = dr["Id"].ToString();
                    sectionVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                    sectionVM.Code = dr["Code"].ToString();
                    sectionVM.Name = dr["Name"].ToString();
                    sectionVM.Remarks = dr["Remarks"].ToString();
                    sectionVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    sectionVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    sectionVM.CreatedBy = dr["CreatedBy"].ToString();
                    sectionVM.CreatedFrom = dr["CreatedFrom"].ToString();
                    sectionVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    sectionVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    sectionVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    sectionVM.OrderNo = Convert.ToInt32(dr["OrderNo"]);

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

            return sectionVM;
        }
        //==================Insert =================
        public string[] Insert(SectionVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertSection"; //Method Name

            SqlConnection currConn = null;
                SqlTransaction transaction = null;
            #endregion

            #region Try

            try
            {

                #region Validation      
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
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Section";	
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
				
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsertWithBranch(tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                      
                    }
                    else
                    {           
                        #region Save
                        sqlText = "Select isnull(max(convert(int,  SUBSTRING(CONVERT(varchar(10), id),CHARINDEX('_', CONVERT(varchar(10), id))+1,10))),0) from Section where BranchId=@BranchId";
                        SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                        cmd2.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd2.Transaction = transaction;
                        var exeRes = cmd2.ExecuteScalar();
                        int count = Convert.ToInt32(exeRes);
                        vm.Id = vm.BranchId.ToString() + "_" + (count + 1);
                        if (1 == 1)
                        {

                            sqlText = "  ";
                            sqlText += @" INSERT INTO Section(Id,BranchId,Code,Name,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,OrderNo) 
                                VALUES (@Id,@BranchId,@Code,@Name,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@OrderNo) 
                                        ";

                            SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                            cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                            cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                            cmdInsert.Parameters.AddWithValue("@Code", vm.Code.Trim());
                            cmdInsert.Parameters.AddWithValue("@Name", vm.Name.Trim());
                            cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                            cmdInsert.Parameters.AddWithValue("@IsActive", true);
                            cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                            cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                            cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                            cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                            cmdInsert.Parameters.AddWithValue("@OrderNo", vm.OrderNo);

                            cmdInsert.Transaction = transaction;
                            cmdInsert.ExecuteNonQuery();
                        }
                        else
                        {
                            retResults[1] = "This Section already used";
                            throw new ArgumentNullException("Please Input Section Value", "");
                        }

                        #endregion Save
                    }
                }
                #endregion Exist
                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";
                retResults[2] = vm.Id;

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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }


            #endregion

            #region Results

            return retResults;
            #endregion


        }
        //==================Update =================
        public string[] Update(SectionVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Section Update"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToSection"); }

                #endregion open connection and transaction
                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Section";				
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdateWithBranch(vm.Id, tableName, fieldName[i], fieldValue[i], vm.BranchId, currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Section ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", SectionVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", SectionVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", SectionVM.Name.Trim());

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Travel already used";
                //    throw new ArgumentNullException("Please Input Travel Value", "");
                //}
                //#endregion Exist

                if (vm != null)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Section set";
                    sqlText += " BranchId=@BranchId,";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";
                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom,";
                    sqlText += " OrderNo=@OrderNo";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code.Trim());
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name.Trim());

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@OrderNo", vm.OrderNo);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult = Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", SectionVM.BranchId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Section Update", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to update Section.";
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
        public SectionVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            SectionVM sectionVM = new SectionVM();

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
,Name
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,ISNULL(OrderNo,0)OrderNo

    From Section 
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
                        sectionVM.Id = dr["Id"].ToString();
                        sectionVM.BranchId = Convert.ToInt32(dr["BranchId"]);
                        sectionVM.Code = dr["Code"].ToString();
                        sectionVM.Name = dr["Name"].ToString();
                        sectionVM.Remarks = dr["Remarks"].ToString();
                        sectionVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        sectionVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        sectionVM.CreatedBy = dr["CreatedBy"].ToString();
                        sectionVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        sectionVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        sectionVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        sectionVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                        sectionVM.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());
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

            return sectionVM;
        }
        //==================Delete =================
        public string[] Delete(SectionVM sectionVM, string[] ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteSection"; //Method Name

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

                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToSection"); }

                #endregion open connection and transaction
                #region Check is  it used

                #endregion Check is  it used

                if (ids.Length>0)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length-1; i++)
                    {
                        sqlText = "";
                        sqlText = "update Section set";
                        sqlText += " IsActive=@IsActive,";
                        sqlText += " IsArchive=@IsArchive,";
                        sqlText += " LastUpdateBy=@LastUpdateBy,";
                        sqlText += " LastUpdateAt=@LastUpdateAt,";
                        sqlText += " LastUpdateFrom=@LastUpdateFrom";
                        sqlText += " where Id=@Id";

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Parameters.AddWithValue("@IsActive", false);
                        cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", sectionVM.LastUpdateBy);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", sectionVM.LastUpdateAt);
                        cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", sectionVM.LastUpdateFrom);

                        cmdUpdate.Transaction = transaction;
					    var exeRes = cmdUpdate.ExecuteNonQuery();
					    transResult = Convert.ToInt32(exeRes);
                    }


                    retResults[2] = "";// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Section Delete", sectionVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Section Information Delete", "Could not found any item.");
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
                    retResults[1] = "Unexpected error to delete Section Information.";
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

        public List<SectionVM> DropDown()
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<SectionVM> VMs = new List<SectionVM>();
            SectionVM vm;
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
   FROM Section
WHERE IsArchive=0 and IsActive=1
    ORDER BY Name
";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SectionVM();
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
                sqlText = @"SELECT Id, Name    FROM Section ";
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
        public List<DropDownVM> DropDownByDepartment( string departmentId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<DropDownVM> VMs = new List<DropDownVM>();
            DropDownVM vm = new DropDownVM();
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
select distinct  SectionId Id,Section Name from ViewEmployeeInformation 
WHERE   1=1
 
";
                if (departmentId != "0_0")
                    sqlText += @"   and   DepartmentId=@DepartmentId";
                sqlText += @"   ORDER BY Name";

                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;
                if (departmentId != "0_0")
                    _objComm.Parameters.AddWithValue("@DepartmentId", departmentId);

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new DropDownVM();
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
        #endregion


        public string[] InsertExportData(SectionVM paramVM, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "ImportExcelFile"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();


                dt = ds.Tables[0];
                reader.Close();

                File.Delete(Fullpath);
                #endregion

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
                #region Save
                string Code = "";

                SectionVM vSectionVM = new SectionVM();

                #region Assign Data
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    vSectionVM.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vSectionVM.Code = dr["Code"].ToString();
                    vSectionVM.Name = dr["Name"].ToString();
                    vSectionVM.OrderNo = Convert.ToInt32(dr["OrderNo"].ToString());
                    vSectionVM.Remarks = dr["Remarks"].ToString();
                    vSectionVM.CreatedAt = paramVM.CreatedAt;
                    vSectionVM.CreatedBy = paramVM.CreatedBy;
                    vSectionVM.CreatedFrom = paramVM.CreatedFrom;
                    retResults = Insert(vSectionVM, currConn, transaction);
                }
                #endregion

                #region Data Insert


                if (retResults[0] == "Fail")
                {
                    throw new ArgumentNullException("", retResults[1]);
                }
                #endregion
                #endregion
                #region Commit
                if (transaction != null)
                {
                    transaction.Commit();
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
                retResults[4] = ex.Message.ToString(); //catch ex
                transaction.Rollback();
                return retResults;
            }
            finally
            {
            }
            #endregion
            #region Results
            return retResults;
            #endregion

        }
    }
}
