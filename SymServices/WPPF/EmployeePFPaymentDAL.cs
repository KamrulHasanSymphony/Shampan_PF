using Excel;
using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymServices.WPPF
{
    public class EmployeePFPaymentDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL cdal = new CommonDAL();
        #endregion

        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of EmployeePFPaymentVM objects, optionally filtered by a specific employee ID.
        /// The method fetches employee provident fund payment details along with associated employee information,
        /// including salary, designation, department, and profit contributions where records are active and not archived.
        /// </summary>
        /// <param name="empid">Optional employee ID to filter the results. If null or empty, all active and non-archived records are returned.</param>
        /// <returns>A list of EmployeePFPaymentVM instances containing provident fund payment and employee information.</returns>
        public List<EmployeePFPaymentVM> SelectAll(string empid = null)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            List<EmployeePFPaymentVM> vms = new List<EmployeePFPaymentVM>();
            EmployeePFPaymentVM vm;
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
,e.Department, e.JoinDate, e.Section, e.Project 
,isnull(e.GrossSalary,0)GrossSalary
,isnull(e.BasicSalary,0)BasicSalary
,isnull(pfo.EmployeeContribution,0)       EmployeeContribution
,isnull(pfo.EmployerContribution,0)       EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)       EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)       EmployerProfit

,pfo.PaymentDate
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
From EmployeePFPayment pfo

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
                    vm = new EmployeePFPaymentVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
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
        /// Retrieves an EmployeePFPaymentVM object by the specified payment Id and/or employee Id.
        /// Connects to the database, executes a query to fetch employee provident fund payment details along with related employee information,
        /// and maps the result to an EmployeePFPaymentVM instance.
        /// </summary>
        /// <param name="Id">The unique identifier of the employee provident fund payment record.</param>
        /// <param name="empId">The unique identifier of the employee.</param>
        /// <returns>
        /// An EmployeePFPaymentVM object containing the employee provident fund payment details and related employee information.
        /// If no matching record is found, returns an empty EmployeePFPaymentVM instance.
        /// </returns>
        public EmployeePFPaymentVM SelectById(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFPaymentVM vm = new EmployeePFPaymentVM();

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
,pfo.PaymentDate
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
From EmployeePFPayment pfo
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
                    vm = new EmployeePFPaymentVM();
                    vm.Id = dr["Id"].ToString();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
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
        /// Retrieves an EmployeePFPaymentVM object by specified payment Id and/or employee Id.
        /// This method queries the EmployeePFPayment table joined with employee information and maps the result to a view model.
        /// </summary>
        /// <param name="Id">The unique identifier of the EmployeePFPayment record.</param>
        /// <param name="empId">The unique identifier of the Employee.</param>
        /// <returns>
        /// An EmployeePFPaymentVM object containing the payment and employee details corresponding to the provided identifiers.
        /// Returns an empty EmployeePFPaymentVM if no matching record is found.
        /// </returns>
        public EmployeePFPaymentVM SelectByIdAll(string Id, string empId)
        {

            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            EmployeePFPaymentVM vm = new EmployeePFPaymentVM();

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
pfo.EmployeeId
,e.EmpName
,e.Code
,e.Designation
,e.Department, e.JoinDate, e.Section, e.Project, e.GrossSalary, e.BasicSalary
,isnull(pfo.EmployeeContribution,0)EmployeeContribution
,isnull(pfo.EmployerContribution,0)EmployerContribution
,isnull(pfo.EmployeeProfit      ,0)EmployeeProfit
,isnull(pfo.EmployerProfit      ,0)EmployerProfit
,pfo.PaymentDate
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
From EmployeePFPayment pfo
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
                    vm = new EmployeePFPaymentVM();
                    vm.EmployeeId = dr["EmployeeId"].ToString();
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.PaymentDate = Ordinary.StringToDate(dr["PaymentDate"].ToString());
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
        public string[] Insert(EmployeePFPaymentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
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
            FiscalYearDAL _fyDAL = new FiscalYearDAL();


            #endregion

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #region Try

            try
            {

                //#region Current Fiscal Period Status
                //if (VcurrConn == null && Vtransaction == null)
                //{
                //    var fydVM = new FiscalYearDetailVM();

                //    fydVM = _fyDAL.SelectAll_FiscalYearDetail(Convert.ToInt32(vm.FiscalYearDetailId), null, null, null, null).FirstOrDefault();

                //    if (fydVM.PeriodLock)
                //    {
                //        retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                //        throw new ArgumentNullException("", retResults[1]);
                //    }
                //}

                //#endregion


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
                //sqlText += " SELECT   count(Id) FROM EmployeePFPayment ";
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

                sqlText = "  ";
                sqlText += " SELECT   count(Id) FROM EmployeePFPayment ";
                sqlText += @" WHERE EmployeeId=@EmployeeId and PaymentDate=@PaymentDate ";

                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);
                cmdExist.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(vm.PaymentDate));

                var exeRes = cmdExist.ExecuteScalar();

                int objfoundId = Convert.ToInt32(exeRes);

                if (objfoundId > 0)
                {
                    throw new ArgumentNullException("Same Date Trasaction already exits", "");
                }

                #endregion Exist


                #region Save

                vm.Id = cdal.NextId("EmployeePFPayment", currConn, transaction).ToString();

                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" INSERT INTO EmployeePFPayment
(
 Id
,EmployeeId
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit
,FiscalYearDetailId
,PaymentDate
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
 ,@FiscalYearDetailId
, @PaymentDate
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
                    cmdInsert.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdInsert.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(vm.PaymentDate));
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
                    retResults[1] = "Please Input EmployeePFPayment Value";
                    throw new ArgumentNullException("Please Input EmployeePFPayment Value", "");
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

                if (Vtransaction == null && transaction != null) { transaction.Rollback(); }
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
        /// <summary>
        /// Updates an existing Employee Provident Fund (WPPF) payment record in the database using the provided view model and optional SQL connection and transaction.
        /// Validates fiscal period locking and checks if the record exists and is not already posted before performing the update.
        /// Manages SQL connection and transaction lifecycle if not provided externally.
        /// </summary>
        /// <param name="vm">The EmployeePFPaymentVM object containing updated WPPF payment details.</param>
        /// <param name="VcurrConn">Optional existing SQL connection to be used for the operation.</param>
        /// <param name="Vtransaction">Optional existing SQL transaction to be used for the operation.</param>
        /// <returns>
        /// A string array containing operation results:
        /// [0] = "Success" or "Fail" indicating the update outcome.
        /// [1] = A message associated with the operation result.
        /// [2] = The ID of the updated record as a string.
        /// [3] = The SQL query executed for the update operation.
        /// [4] = Exception message if any occurred during execution.
        /// [5] = The name of the method performing the update ("EmployeePFPayment Update").
        /// </returns>
        public string[] Update(EmployeePFPaymentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFPayment Update"; //Method Name
            FiscalYearDAL _fyDAL = new FiscalYearDAL();

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
                #region Current Fiscal Period Status
                if (VcurrConn == null && Vtransaction == null)
                {
                    var fydVM = new FiscalYearDetailVM();

                    fydVM = _fyDAL.SelectAll_FiscalYearDetail(Convert.ToInt32(vm.FiscalYearDetailId), null, null, null, null).FirstOrDefault();

                    if (fydVM.PeriodLock)
                    {
                        retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePFPayment"); }

                #endregion open connection and transaction


                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeePFPayment ";
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
                    sqlText += " SELECT isnull(Post,0) FROM EmployeePFPayment ";
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
                    sqlText = "update EmployeePFPayment set";
                    sqlText += " EmployeeContribution=@EmployeeContribution,";
                    sqlText += " EmployerContribution=@EmployerContribution,";
                    sqlText += " EmployeeProfit=@EmployeeProfit,";
                    sqlText += " EmployerProfit=@EmployerProfit,";
                    sqlText += " FiscalYearDetailId=@FiscalYearDetailId,";
                    sqlText += " PaymentDate=@PaymentDate,";
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
                    cmdUpdate.Parameters.AddWithValue("@FiscalYearDetailId", vm.FiscalYearDetailId);
                    cmdUpdate.Parameters.AddWithValue("@PaymentDate", Ordinary.DateToString(vm.PaymentDate));
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
                    retResults[1] = "Unexpected error to update EmployeePFPayment.";
                    throw new ArgumentNullException("", "");
                }

            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message; //catch ex
                if (Vtransaction == null && transaction != null) { transaction.Rollback(); }
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
        public string[] Post(EmployeePFPaymentVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFPayment Post"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToEmployeePFPayment"); }

                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Exist

                    sqlText = "  ";
                    sqlText += " SELECT COUNT( Id)Id FROM EmployeePFPayment ";
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
                    sqlText += " SELECT isnull(Post,0) FROM EmployeePFPayment ";
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
                    sqlText = "update EmployeePFPayment set";
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
                    retResults[1] = "Unexpected error to Post EmployeePFPayment.";
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
        /// Posts multiple EmployeePFPayment records identified by the given array of Ids using the provided EmployeePFPaymentVM object.
        /// Uses an existing SqlConnection and SqlTransaction if provided, otherwise creates new ones.
        /// Iterates over the Ids to validate existence and perform post operation within a database transaction.
        /// Commits the transaction if successful, or rolls back on exception.
        /// </summary>
        /// <param name="vm">The EmployeePFPaymentVM object containing the data to post.</param>
        /// <param name="Ids">Array of Id strings representing the EmployeePFPayment records to post.</param>
        /// <param name="VcurrConn">Optional existing SqlConnection to use for database operations.</param>
        /// <param name="Vtransaction">Optional existing SqlTransaction to use for database operations.</param>
        /// <returns>Returns a string array with status and execution details, where:
        /// [0] = "Success" or "Fail" indicating operation result,
        /// [1] = message or exception details,
        /// [2] = returned Id (empty in this method),
        /// [3] = last executed SQL query text,
        /// [4] = exception message if any,
        /// [5] = method name ("DeleteEmployeePFPayment").
        /// </returns>
        public string[] MultiplePost(EmployeePFPaymentVM vm, string[] Ids, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Variables

            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DeleteEmployeePFPayment"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction("PostToEmployeePFPayment"); }

                #endregion open connection and transaction

                if (Ids.Length > 1)
                {
                    #region Update Settings

                    for (int i = 0; i < Ids.Length - 1; i++)
                    {
                        #region Exist

                        sqlText = "  ";
                        sqlText += " SELECT EmployeeId FROM EmployeePFPayment ";
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
                    retResults[1] = "Unexpected error to delete EmployeePFPayment Information.";
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


        public DataTable ExportExcelFileFormEmployee(EmployeePFPaymentVM vm, string Filepath, string FileName)
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
, @PaymentDate PaymentDate,'' Remarks

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

                da.SelectCommand.Parameters.AddWithValue("@PaymentDate", DateTime.Now.ToString("dd-MMM-yyyy"));


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

        /// <summary>
        /// Exports the Employee WPPF opening payments data into a DataTable based on the provided filtering criteria within the EmployeePFPaymentVM view model.
        /// Connects to the database, executes a parameterized SQL query with optional filters for project, department, section, designation, and employee code ranges,
        /// and orders the data according to the specified order option.
        /// The method also formats date columns and rounds specified numeric columns before returning the DataTable.
        /// </summary>
        /// <param name="vm">An object containing filtering and sorting parameters for the query.</param>
        /// <param name="Filepath">The file path intended for the export file (currently unused in the method logic).</param>
        /// <param name="FileName">The file name intended for the export file (currently unused in the method logic).</param>
        /// <returns>A DataTable containing the filtered and processed Employee WPPF opening payment records.</returns>
        public DataTable ExportExcelFileFormPFOpening(EmployeePFPaymentVM vm, string Filepath, string FileName)
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
,PaymentDate
,Remarks
  FROM EmployeePFPayment pfo

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

                string DatecolumnNames = "PaymentDate";

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
            string FiscalYearDetailid = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = Id.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "EmployeePFPayment"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            FiscalYearDAL _fyDAL = new FiscalYearDAL();

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

                #region Current Fiscal Period Status
                foreach (DataRow item in dt.Rows)
                {
                    if (string.IsNullOrWhiteSpace(item["PaymentDate"].ToString()))
                    {
                        throw new ArgumentNullException("", "Payment Date Not Given In Excel");
                    }
                    DateTime newDate = Convert.ToDateTime(Ordinary.StringToDate(item["PaymentDate"].ToString()));
                    string sMonth = newDate.ToString("MMM");
                    string sYear = newDate.ToString("yy");
                    string PriodName = sMonth + "-" + sYear;

                    var fydVM = new FiscalYearDetailVM();

                    fydVM = _fyDAL.SelectAll_FiscalYearDetail(0, new[] { "PeriodName" }, new[] { PriodName }, null, null).FirstOrDefault();

                    if (fydVM.PeriodLock)
                    {
                        retResults[1] = "This Fiscal Period: " + fydVM.PeriodName + " is Locked! Locked Data cannot be processed!";
                        throw new ArgumentNullException("", retResults[1]);
                    }
                }

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

                    EmployeePFPaymentVM vm = new EmployeePFPaymentVM();

                    //empVM = _dalemp.ViewSelectAllEmployee(item["EmpCode"].ToString(), null, null, null, null, null, null, currConn, transaction).FirstOrDefault();

                    string empCode = item["EmpCode"].ToString();
                    DateTime newDate = Convert.ToDateTime(Ordinary.StringToDate(item["PaymentDate"].ToString()));
                    string sMonth = newDate.ToString("MMM");
                    string sYear = newDate.ToString("yy");
                    string PriodName = sMonth + "-" + sYear;


                    #region Get FiscalYearDetailid

                    sqlText = "  ";

                    sqlText += " SELECT Id from dbo.FiscalYearDetail ";
                    sqlText += " WHERE PeriodName=@PeriodName";

                    SqlCommand cmdFiscalYearDetailid = new SqlCommand(sqlText, currConn);
                    cmdFiscalYearDetailid.Transaction = transaction;
                    cmdFiscalYearDetailid.Parameters.AddWithValue("@PeriodName", PriodName);

                    var FiscalYearId = cmdFiscalYearDetailid.ExecuteScalar();
                    FiscalYearDetailid = FiscalYearId.ToString();



                    #endregion Exist

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

                            sqlText += " SELECT Id from EmployeePFPayment WHERE EmployeeId=@EmployeeId  and FiscalYearDetailId=@FiscalYearDetailid";

                            cmdExist = new SqlCommand(sqlText, currConn);
                            cmdExist.Transaction = transaction;
                            cmdExist.Parameters.AddWithValue("@EmployeeId", empId);
                            cmdExist.Parameters.AddWithValue("@FiscalYearDetailid", FiscalYearDetailid);

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
                                vm.PaymentDate = item["PaymentDate"].ToString();
                                vm.Remarks = item["Remarks"].ToString();
                                vm.LastUpdateAt = auditvm.LastUpdateAt;
                                vm.LastUpdateBy = auditvm.LastUpdateBy;
                                vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                                vm.CreatedAt = auditvm.CreatedAt;
                                vm.CreatedBy = auditvm.CreatedBy;
                                vm.CreatedFrom = auditvm.CreatedFrom;
                                vm.FiscalYearDetailId = FiscalYearDetailid;
                                #endregion

                                #region Post Check and Update

                                sqlText = "  ";
                                sqlText += " SELECT isnull(Post,0) FROM EmployeePFPayment ";
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
                                vm.PaymentDate = item["PaymentDate"].ToString();
                                vm.Remarks = item["Remarks"].ToString();
                                vm.LastUpdateAt = auditvm.LastUpdateAt;
                                vm.LastUpdateBy = auditvm.LastUpdateBy;
                                vm.LastUpdateFrom = auditvm.LastUpdateFrom;
                                vm.CreatedAt = auditvm.CreatedAt;
                                vm.CreatedBy = auditvm.CreatedBy;
                                vm.CreatedFrom = auditvm.CreatedFrom;
                                vm.FiscalYearDetailId = FiscalYearDetailid;

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
                if (Vtransaction == null && transaction != null) { transaction.Rollback(); }
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
