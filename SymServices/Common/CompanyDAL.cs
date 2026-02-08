using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.Common
{
    public class CompanyDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion
        #region Methods
        //==================SelectAll=================
        public List<CompanyVM> SelectAll()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CompanyVM> VMs = new List<CompanyVM>();
            CompanyVM vm;
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
,Code
,Name
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,TaxId
,RegistrationNumber
,Mail
,NumberOfEmployees
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Company
Where IsArchive=0
    ORDER BY Code
";
                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CompanyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();

                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();

                    vm.TaxId = dr["TaxId"].ToString();
                    vm.RegistrationNumber = dr["RegistrationNumber"].ToString();
                    vm.Email = dr["Mail"].ToString();
                    vm.NumberOfEmployees = Convert.ToInt32(dr["NumberOfEmployees"]);

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
        //==================SelectByID=================
        public CompanyVM SelectById(int Id)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            CompanyVM vm = new CompanyVM();

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
,Code
,Name
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,TaxId
,RegistrationNumber
,Mail
,NumberOfEmployees
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
,YearStart
,Year
    From Company
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
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Code = dr["Code"].ToString();
                    vm.Name = dr["Name"].ToString();

                    vm.Address = dr["Address"].ToString();
                    vm.District = dr["District"].ToString();
                    vm.Division = dr["Division"].ToString();
                    vm.Country = dr["Country"].ToString();
                    vm.City = dr["City"].ToString();
                    vm.PostalCode = dr["PostalCode"].ToString();
                    vm.Phone = dr["Phone"].ToString();
                    vm.Mobile = dr["Mobile"].ToString();
                    vm.Fax = dr["Fax"].ToString();

                    vm.TaxId = dr["TaxId"].ToString();
                    vm.RegistrationNumber = dr["RegistrationNumber"].ToString();
                    vm.Email = dr["Mail"].ToString();
                    vm.NumberOfEmployees = Convert.ToInt32(dr["NumberOfEmployees"]);


                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
                    vm.YearStart = Ordinary.StringToDate(dr["YearStart"].ToString());
                    vm.Year = dr["Year"].ToString();

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

            return vm;
        }
        //==================Insert =================
        public string[] Insert(CompanyVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            retResults[5] = "InsertCompany"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                //if (string.IsNullOrEmpty(CompanyVM.DepartmentId))
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
                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Company";	
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };
				
                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInInsert(tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist	


                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Company ";
                //sqlText += " WHERE EmployeeId=@EmployeeId And Name=@Name";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@EmployeeId", CompanyVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", CompanyVM.Name);
                //object objfoundId = cmdExist.ExecuteScalar();

                //if (objfoundId == null)
                //{
                //    retResults[1] = "Please Input Employee Travel Value";
                //    retResults[3] = sqlText;
                //    throw new ArgumentNullException("Please Input Employee Travel Value", "");
                //}
                //#endregion Exist
                #region Save

                //int foundId = (int)objfoundId;
                if (1 == 1)
                {

                    sqlText = "  ";
                    sqlText += @" INSERT INTO Company(
                                            Code,Name
                                            ,Address,District,Division,Country,City,PostalCode,Phone,Mobile,Fax
,TaxId,RegistrationNumber,Mail,NumberOfEmployees
                                ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom,VATNo) 
                                VALUES (@Code,@Name
                                        ,@Address,@District,@Division,@Country,@City,@PostalCode,@Phone,@Mobile,@Fax
,@TaxId,@RegistrationNumber,@Mail,@NumberOfEmployees
                                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom,@VATNo) 
                                        SELECT SCOPE_IDENTITY()";

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);

                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@Name", vm.Name);

                    cmdInsert.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);

                    cmdInsert.Parameters.AddWithValue("@TaxId", vm.TaxId ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@RegistrationNumber", vm.RegistrationNumber ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@Mail", vm.Email ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@NumberOfEmployees", vm.NumberOfEmployees);
                    cmdInsert.Parameters.AddWithValue("@VATNo", 1);


                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

                    cmdInsert.Transaction = transaction;
					var exeRes = cmdInsert.ExecuteScalar();
					Id = Convert.ToInt32(exeRes);

                    if (Id <= 0)
                    {
                        retResults[1] = "Please Input Company Value";
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Please Input Company Value", "");
                    }
                }
                else
                {
                    retResults[1] = "This Company already used";
                    throw new ArgumentNullException("Please Input Company Value", "");
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
        public string[] Update(CompanyVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Company Update"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction= null;
            int transResult = 0;

            string sqlText = "";

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction
                #region Exist

                CommonDAL cdal = new CommonDAL();
                bool check = false;
                string tableName = "Company";				
                string[] fieldName = { "Code", "Name" };
                string[] fieldValue = { vm.Code.Trim(), vm.Name.Trim() };

                for (int i = 0; i < fieldName.Length; i++)
                {
                    check = cdal.CheckDuplicateInUpdate(vm.Id.ToString(), tableName, fieldName[i], fieldValue[i], currConn, transaction);
                    if (check == true)
                    {
                        retResults[1] = "This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!";
                        throw new ArgumentNullException("This " + fieldName[i] + ": \"" + fieldValue[i] + "\" already used!", "");
                    }
                }
                #endregion Exist

                //#region Exist
                //sqlText = "  ";
                //sqlText += " SELECT COUNT(DISTINCT Id)Id FROM Company ";
                //sqlText += " WHERE EmployeeId=@EmployeeId  AND Name=@Name Id<>@Id";
                //SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                //cmdExist.Transaction = transaction;
                //cmdExist.Parameters.AddWithValue("@Id", CompanyVM.Id);
                //cmdExist.Parameters.AddWithValue("@EmployeeId", CompanyVM.EmployeeId);
                //cmdExist.Parameters.AddWithValue("@Name", CompanyVM.Name);

                //if (Convert.ToInt32(cmdExist.ExecuteScalar()) > 0)
                //{
                //    retResults[1] = "This Travel already used";
                //    throw new ArgumentNullException("Please Input Travel Value", "");
                //}
                //#endregion Exist

                if (vm != null)
                {

                    #region Fiscalyear
                    FiscalYearDAL fiscalYearDal = new FiscalYearDAL();
                    FiscalYearVM fiscalYearVM = fiscalYearDal.SelectAll(vm.CurrentBranch).FirstOrDefault();
                    if (fiscalYearVM == null)
                    {
                        List<FiscalYearDetailVM> fiscalYearDVMs = new List<FiscalYearDetailVM>();
                        FiscalYearDetailVM fiscalYearDVM;
                        fiscalYearVM = new FiscalYearVM();
                        var date = Ordinary.DateToString(vm.YearStart);
                       
                        DateTime start_date = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), 01);

                        fiscalYearVM.YearStart = start_date.ToString("dd-MMM-yyyy");
                        fiscalYearVM.YearEnd = start_date.AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
                        fiscalYearVM.Year = start_date.Year;
                        for (int i = 0; i < 12; i++)
                        {
                            fiscalYearDVM = new FiscalYearDetailVM();
                            fiscalYearDVM.PeriodName = start_date.AddMonths(i).ToString("MMMM") + "-" + fiscalYearVM.Year;
                            fiscalYearDVM.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
                            fiscalYearDVM.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
                            fiscalYearDVMs.Add(fiscalYearDVM);
                        }
                        fiscalYearVM.FiscalYearDetailVM = fiscalYearDVMs;
                        fiscalYearVM.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                        fiscalYearVM.CreatedBy = vm.LastUpdateBy;
                        fiscalYearVM.CreatedFrom = vm.LastUpdateFrom;
                        fiscalYearVM.BranchId = vm.CurrentBranch;
                        string[] result = fiscalYearDal.FiscalYearInsert(fiscalYearVM, currConn, transaction, true);
                        if (result[0]=="Fail")
                        {
                            throw new ArgumentNullException("Fiscal year error", "Fiscal year error.");
                        }
                    }
                    #endregion
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Company set";
                    sqlText += " Code=@Code,";
                    sqlText += " Name=@Name,";

                    sqlText += " Address=@Address,";
                    sqlText += " District=@District,";
                    sqlText += " Division=@Division,";
                    sqlText += " Country=@Country,";
                    sqlText += " City=@City,";
                    sqlText += " PostalCode=@PostalCode,";
                    sqlText += " Phone=@Phone,";
                    sqlText += " Mobile=@Mobile,";
                    sqlText += " Fax=@Fax,";

                    sqlText += " TaxId=@TaxId,";
                    sqlText += " RegistrationNumber=@RegistrationNumber,";
                    sqlText += " Mail=@Mail,";
                    sqlText += " NumberOfEmployees=@NumberOfEmployees,";


                    sqlText += " Remarks=@Remarks,";
                    sqlText += " IsActive=@IsActive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom,";
                    sqlText += " YearStart=@YearStart,";
                    sqlText += " Year=@Year";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Code", vm.Code);
                    cmdUpdate.Parameters.AddWithValue("@Name", vm.Name);

                    cmdUpdate.Parameters.AddWithValue("@Address", vm.Address ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@District", vm.District ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Division", vm.Division ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Country", vm.Country ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@City", vm.City ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@PostalCode", vm.PostalCode ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Phone", vm.Phone ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mobile", vm.Mobile ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Fax", vm.Fax ?? Convert.DBNull);

                    cmdUpdate.Parameters.AddWithValue("@TaxId", vm.TaxId ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@RegistrationNumber", vm.RegistrationNumber ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@Mail", vm.Email ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@NumberOfEmployees", vm.NumberOfEmployees);

                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", false);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@YearStart",Ordinary.DateToString(fiscalYearVM.YearStart));
                    cmdUpdate.Parameters.AddWithValue("@Year", fiscalYearVM.Year);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult	= Convert.ToInt32(exeRes);

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Education Update", CompanyVM.CompanyId + " could not updated.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Company Update", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to update Company.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }
        //==================Select =================
        public CompanyVM Select(string query, int Id)//FITST, LAST, NEXT, PREVIOUS
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";

            CompanyVM CompanyVM = new CompanyVM();

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
,Code
,Name
,Address
,District
,Division
,Country
,City
,PostalCode
,Phone
,Mobile
,Fax
,Remarks
,IsActive
,IsArchive
,CreatedBy
,CreatedAt
,CreatedFrom
,LastUpdateBy
,LastUpdateAt
,LastUpdateFrom
    From Company
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
                        CompanyVM.Id = Convert.ToInt32(dr["Id"]);
                        CompanyVM.Code = dr["Code"].ToString();
                        CompanyVM.Name = dr["Name"].ToString();

                        CompanyVM.Name = dr["Address"].ToString();
                        CompanyVM.Name = dr["District"].ToString();
                        CompanyVM.Name = dr["Division"].ToString();
                        CompanyVM.Name = dr["Country"].ToString();
                        CompanyVM.Name = dr["City"].ToString();
                        CompanyVM.Name = dr["PostalCode"].ToString();
                        CompanyVM.Name = dr["Phone"].ToString();
                        CompanyVM.Name = dr["Mobile"].ToString();
                        CompanyVM.Name = dr["Fax"].ToString();

                        CompanyVM.Remarks = dr["Remarks"].ToString();
                        CompanyVM.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        CompanyVM.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                        CompanyVM.CreatedBy = dr["CreatedBy"].ToString();
                        CompanyVM.CreatedFrom = dr["CreatedFrom"].ToString();
                        CompanyVM.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                        CompanyVM.LastUpdateBy = dr["LastUpdateBy"].ToString();
                        CompanyVM.LastUpdateFrom = dr["LastUpdateFrom"].ToString();
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

            return CompanyVM;
        }
        //==================Delete =================
        public string[] Delete(CompanyVM companyVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteCompany"; //Method Name

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
                if (transaction == null)
                {
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction  
                #region Check is  it used

                #endregion Check is  it used

                if (companyVM.Id > 0)
                {
                    #region Update Settings

                    sqlText = "";
                    sqlText = "update Company set";
                    sqlText += " IsArchive=@IsArchive,";
                    sqlText += " LastUpdateBy=@LastUpdateBy,";
                    sqlText += " LastUpdateAt=@LastUpdateAt,";
                    sqlText += " LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " where Id=@Id";

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", companyVM.Id);
                    cmdUpdate.Parameters.AddWithValue("@IsArchive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", companyVM.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", companyVM.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", companyVM.LastUpdateFrom);

                    cmdUpdate.Transaction = transaction;
					var exeRes = cmdUpdate.ExecuteNonQuery();
					transResult	= Convert.ToInt32(exeRes);

                    retResults[2] = companyVM.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query

                    #region Commit

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("Company Delete", companyVM.Id + " could not Delete.");
                    }

                    #endregion Commit

                    #endregion Update Settings
                    iSTransSuccess = true;
                }
                else
                {
                    throw new ArgumentNullException("Company Information Delete", "Could not found any item.");
                }


                if (iSTransSuccess == true)
                {
                    if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                    retResults[0] = "Success";
                    retResults[1] = "Data Delete Successfully.";

                }
                else
                {
                    retResults[1] = "Unexpected error to delete Company Information.";
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }
        #endregion
        //==================DropDown =================
        public List<CompanyVM> DropDown()
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<CompanyVM> VMs = new List<CompanyVM>();
            CompanyVM vm;
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
               
                FROM Company
                WHERE IsArchive=0 and IsActive=1
                ORDER BY Name
                ";
                 //FROM Companies
                SqlCommand _objComm = new SqlCommand();
                _objComm.Connection = currConn;
                _objComm.CommandText = sqlText;
                _objComm.CommandType = CommandType.Text;

                SqlDataReader dr;
                dr = _objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new CompanyVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
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
    }
}
