using SymOrdinary;
using SymServices.Common;

using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SymServices.WWF
{
    public class InvestmentRenewDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods
        //==================DropDown=================
        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of InvestmentRenewVM objects from the database based on the specified criteria.
        /// Supports filtering by Id and additional condition fields and values.
        /// Handles database connection and transaction management, with optional external connection and transaction support.
        /// </summary>
        /// <param name="Id">Optional identifier to filter the results by a specific investment renew record Id. Default is 0 (no filter by Id).</param>
        /// <param name="conditionFields">Optional array of additional condition field names for filtering the query.</param>
        /// <param name="conditionValues">Optional array of corresponding condition values for the fields specified in conditionFields.</param>
        /// <param name="VcurrConn">Optional existing SqlConnection to use; if null, a new connection will be created and managed 
        public List<InvestmentRenewVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
                 , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<InvestmentRenewVM> VMs = new List<InvestmentRenewVM>();
            InvestmentRenewVM vm;
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
inv.Id
,inv.InvestmentId
,inv.TransactionCode
,inv.InvestmentDate
,inv.ReferenceNo
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentValue
,inv.BankCharge
,isnull(inv.BankExciseDuty,0)BankExciseDuty
,inv.SourceTaxDeduct,inv.OtherCharge
,inv.Interest
,inv.Remarks
,inv.Post
,inv.IsActive
,inv.IsArchive
,inv.CreatedBy
,inv.CreatedAt
,inv.CreatedFrom
,inv.LastUpdateBy
,inv.LastUpdateAt
,inv.LastUpdateFrom
,isnull(inv.IsEncashed,0)IsEncashed

from InvestmentRenew inv
WHERE  1=1 AND inv.IsArchive = 0
";
                //InvestmentType
                //InvestmentTypeId
                //ReferenceNo

                if (Id > 0)
                {
                    sqlText += @" and inv.Id=@Id";
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

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                DataTable dtResult = new DataTable();

                dataAdapter.Fill(dtResult);


                //VMs = JsonConvert.DeserializeObject<List<InvestmentRenewVM>>(JsonConvert.SerializeObject(dtResult));
                foreach (DataRow dr in dtResult.Rows)
                {

                    vm = new InvestmentRenewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);

                    vm.InvestmentId = Convert.ToInt32(dr["InvestmentId"].ToString());
                    vm.TransactionCode = dr["TransactionCode"].ToString();
                    vm.InvestmentDate = Ordinary.StringToDate(dr["InvestmentDate"].ToString());
                    vm.ReferenceNo = dr["ReferenceNo"].ToString();
                    vm.FromDate = Ordinary.StringToDate(dr["FromDate"].ToString());
                    vm.ToDate = Ordinary.StringToDate(dr["ToDate"].ToString());
                    vm.MaturityDate = Ordinary.StringToDate(dr["MaturityDate"].ToString());
                    vm.InvestmentValue = Convert.ToDecimal(dr["InvestmentValue"].ToString());
                    vm.BankCharge = Convert.ToDecimal(dr["BankCharge"].ToString());
                    vm.BankExciseDuty = Convert.ToDecimal(dr["BankExciseDuty"].ToString());
                    vm.SourceTaxDeduct = Convert.ToDecimal(dr["SourceTaxDeduct"].ToString());
                    vm.OtherCharge = Convert.ToDecimal(dr["OtherCharge"].ToString());

                    vm.Interest = Convert.ToDecimal(dr["Interest"].ToString());


                    vm.IsEncashed = Convert.ToBoolean(dr["IsEncashed"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
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
                //}
                //dr.Close();
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

        //==================Insert =================
        /// <summary>
        /// Inserts a new Investment Renuew record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the Investment Renuew data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertInvestmentRenuew").
        /// </returns>
        public string[] Insert(InvestmentRenewVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertInvestment"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                #region Save



                #region Update Bank Charge
                sqlText = @"
                        Select top 1 * from InvestmentRenew i
                        where i.InvestmentId = @Id order by Id desc";
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                objComm.Parameters.AddWithValue("@Id", vm.InvestmentId);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                DataTable dtResult = new DataTable();
                dataAdapter.Fill(dtResult);

                if (dtResult.Rows.Count > 0)
                {
                    if (vm.IsEncashed == true)
                    {
                        vm.InvestmentVm.TotalAmount = Convert.ToDecimal(dtResult.Rows[0]["InvestmentValue"].ToString());
                        vm.Interest = Convert.ToDecimal(dtResult.Rows[0]["Interest"].ToString());
                        vm.InterestRate = Convert.ToDecimal(dtResult.Rows[0]["InterestRate"].ToString());
                        //vm.BankCharge = Convert.ToDecimal(dtResult.Rows[0]["BankCharge"].ToString());
                        vm.AIT = Convert.ToDecimal(dtResult.Rows[0]["AIT"].ToString());
                    }
                }

                #endregion

                vm.Id = _cDal.NextId("InvestmentRenew", currConn, transaction);
                if (vm != null)
                {
                    string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "InvestmentRenew", vm.InvestmentDate, currConn, transaction);

                    vm.TransactionCode = NewCode;

                    //////vm.TransactionCode = "INR-" + (vm.Id.ToString()).PadLeft(4, '0');

                    #region SqlText

                    sqlText = "  ";
                    sqlText += @" 
                        INSERT INTO InvestmentRenew(
                        Id
                        ,TransactionCode
                        ,InvestmentId                      
                        ,InvestmentDate
                        ,ReferenceNo
                        ,FromDate
                        ,ToDate
                        ,MaturityDate
                        ,InvestmentValue
                        ,BankCharge
                        ,BankExciseDuty
                        ,Interest
                        ,Post
                        ,TransType
                        ,SourceTaxDeduct
                        ,IsEncashed
                        ,OtherCharge,AditionAmount,EncashAmount,InterestRate,AIT
                        ,Remarks,IsActive,IsArchive,CreatedBy,CreatedAt,CreatedFrom
                        ) VALUES (
                        @Id
                        ,@TransactionCode
                        ,@InvestmentId                      
                        ,@InvestmentDate
                        ,@ReferenceNo
                        ,@FromDate
                        ,@ToDate
                        ,@MaturityDate
                        ,@InvestmentValue
                        ,@BankCharge
                        ,@BankExciseDuty
                        ,@Interest
                        ,@Post
                        ,@TransType
                        ,@SourceTaxDeduct
                        ,@IsEncashed
                        ,@OtherCharge,@AditionAmount,@EncashAmount,@InterestRate,@AIT
                        ,@Remarks,@IsActive,@IsArchive,@CreatedBy,@CreatedAt,@CreatedFrom
                        ) 
                        ";
                    #endregion

                    #region SqlExecution

                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Id", vm.Id);
                    cmdInsert.Parameters.AddWithValue("@TransactionCode", vm.InvestmentVm.TransactionCode);
                    cmdInsert.Parameters.AddWithValue("@InvestmentId", vm.InvestmentId);
                    cmdInsert.Parameters.AddWithValue("@InvestmentDate", Ordinary.DateToString(vm.InvestmentDate));
                    cmdInsert.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo);
                    if (vm.FromDate == null) { vm.FromDate = vm.InvestmentVm.FromDate; }
                    if (vm.ToDate == null) { vm.ToDate = vm.InvestmentVm.ToDate; }
                    if (vm.ToDate == null) { vm.MaturityDate = vm.InvestmentVm.MaturityDate; }
                    if (vm.InterestRate == 0) { vm.InterestRate = vm.InvestmentVm.InvestmentRate; }
                    if (vm.AIT == 0) { vm.AIT = vm.InvestmentVm.AIT; }
                    if (vm.Interest == 0) { vm.Interest = vm.InvestmentVm.TotalInterest; }
                    cmdInsert.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdInsert.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdInsert.Parameters.AddWithValue("@MaturityDate", Ordinary.DateToString(vm.MaturityDate));
                    if (vm.IsEncashed == true)
                    {
                        cmdInsert.Parameters.AddWithValue("@InvestmentValue", vm.InvestmentVm.InvestmentValue);
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@InvestmentValue", vm.InvestmentVm.TotalAmount);
                    }

                    cmdInsert.Parameters.AddWithValue("@BankCharge", vm.BankCharge);
                    cmdInsert.Parameters.AddWithValue("@BankExciseDuty", vm.BankExciseDuty.ToString() ?? "0");

                    decimal dateDiff = (Convert.ToDateTime(vm.ToDate) - Convert.ToDateTime(vm.FromDate)).Days;
                    SettingDAL _settingDal = new SettingDAL();
                    string AccruedByDay = _settingDal.settingValue("WWF", "AccruedByDay").Trim();
                    if (AccruedByDay == "N")
                    {
                        cmdInsert.Parameters.AddWithValue("@Interest", vm.Interest);
                    }
                    else
                    {
                        cmdInsert.Parameters.AddWithValue("@Interest", (((vm.InvestmentVm.TotalAmount / 100) * vm.InterestRate) / 365) * dateDiff);
                    }

                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", vm.CreatedAt);
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "WWF");
                    cmdInsert.Parameters.AddWithValue("@SourceTaxDeduct", vm.SourceTaxDeduct);
                    cmdInsert.Parameters.AddWithValue("@IsEncashed", vm.IsEncashed);
                    cmdInsert.Parameters.AddWithValue("@OtherCharge", vm.OtherCharge);
                    cmdInsert.Parameters.AddWithValue("@AditionAmount", vm.AditionAmount);
                    cmdInsert.Parameters.AddWithValue("@EncashAmount", vm.EncashAmount);
                    cmdInsert.Parameters.AddWithValue("@InterestRate", vm.InterestRate);
                    cmdInsert.Parameters.AddWithValue("@AIT", vm.AIT);



                    var exeRes = cmdInsert.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update InvestmentRenew.", "");
                    }

                    if (vm.IsEncashed == true)
                    {
                        sqlText = @"
                            Update Investments set IsEncashed = @IsEncashed 
                            where Id = @Id";
                        SqlCommand objUpdate = new SqlCommand(sqlText, currConn, transaction);
                        objUpdate.Parameters.AddWithValue("@Id", vm.InvestmentId);
                        objUpdate.Parameters.AddWithValue("@IsEncashed", vm.IsEncashed);
                        var update = objUpdate.ExecuteNonQuery();
                    }
                }

                #endregion
                else
                {
                    retResults[1] = "This Investment already used!";
                    throw new ArgumentNullException("Please Input Investment Value", "");
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
        /// <summary>
        /// Updates an existing Investment Renuew record in the database with values from the provided view model.
        /// Handles optional SQL connection and transaction to allow for usage in broader transaction scopes.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the updated Investment Renuew information.</param>
        /// <param name="VcurrConn">Optional external SQL connection. If null, a new connection will be created.</param>
        /// <param name="Vtransaction">Optional external SQL transaction. If null, a new transaction will be created and committed.</param>
        /// <returns>
        /// A string array containing:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Updated record ID,  
        /// [3] = The executed SQL query,  
        /// [4] = Exception message if any occurred,  
        /// [5] = Method name ("BankBranchUpdate").
        /// </returns>
        public string[] Update(InvestmentRenewVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Employee Investment Update"; //Method Name
            int transResult = 0;
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToInvestment"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE InvestmentRenew SET";
                    sqlText += " InvestmentId =@InvestmentId";
                    sqlText += " ,TransactionCode =@TransactionCode";
                    sqlText += " ,InvestmentDate =@InvestmentDate";
                    sqlText += " ,ReferenceNo =@ReferenceNo";
                    sqlText += " ,FromDate =@FromDate";
                    sqlText += " ,ToDate =@ToDate";
                    sqlText += " ,MaturityDate =@MaturityDate";
                    sqlText += " ,InvestmentValue =@InvestmentValue";
                    sqlText += " ,BankCharge =@BankCharge";
                    sqlText += " ,BankExciseDuty =@BankExciseDuty";
                    sqlText += " ,Interest =@Interest";
                    sqlText += " ,Post =@Post";
                    sqlText += " , Remarks=@Remarks";
                    sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , TransType=@TransType";
                    sqlText += " , SourceTaxDeduct=@SourceTaxDeduct";
                    sqlText += " , OtherCharge=@OtherCharge";
                    sqlText += " , AditionAmount=@AditionAmount";
                    sqlText += " , EncashAmount=@EncashAmount";



                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);

                    cmdUpdate.Parameters.AddWithValue("@InvestmentId", vm.InvestmentId);
                    cmdUpdate.Parameters.AddWithValue("@TransactionCode", vm.TransactionCode);
                    cmdUpdate.Parameters.AddWithValue("@InvestmentDate", Ordinary.DateToString(vm.InvestmentDate));
                    cmdUpdate.Parameters.AddWithValue("@ReferenceNo", vm.ReferenceNo);
                    cmdUpdate.Parameters.AddWithValue("@FromDate", Ordinary.DateToString(vm.FromDate));
                    cmdUpdate.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(vm.ToDate));
                    cmdUpdate.Parameters.AddWithValue("@MaturityDate", Ordinary.DateToString(vm.MaturityDate));
                    cmdUpdate.Parameters.AddWithValue("@InvestmentValue", vm.InvestmentValue);
                    cmdUpdate.Parameters.AddWithValue("@BankCharge", vm.BankCharge);
                    cmdUpdate.Parameters.AddWithValue("@BankExciseDuty", vm.BankExciseDuty.ToString() ?? "0");
                    cmdUpdate.Parameters.AddWithValue("@Interest", vm.Interest);

                    cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdUpdate.Parameters.AddWithValue("@IsActive", true);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "WWF");
                    cmdUpdate.Parameters.AddWithValue("@SourceTaxDeduct", vm.SourceTaxDeduct);
                    cmdUpdate.Parameters.AddWithValue("@OtherCharge", vm.OtherCharge);
                    cmdUpdate.Parameters.AddWithValue("@AditionAmount", vm.AditionAmount);
                    cmdUpdate.Parameters.AddWithValue("@EncashAmount", vm.EncashAmount);


                    cmdUpdate.Transaction = transaction;
                    var exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update InvestmentRenew.", "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Investment Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings


                }
                else
                {
                    throw new ArgumentNullException("Investment Update", "Could not found any item.");
                }

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Data Update Successfully.";
                retResults[2] = vm.Id.ToString();
                #endregion SuccessResult
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
           
        ////==================Post =================
        public string[] Post(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PostInvestment"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction
                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        retResults = _cDal.FieldPost("InvestmentRenew", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("InvestmentRenew Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Investment Information Post - Could not found any item.", "");
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Posted Successfully.";
                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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
        public string[] UpdateEncash(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "PostInvestment"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            int transResult = 0;
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
                if (transaction == null) { transaction = currConn.BeginTransaction("Post"); }
                #endregion open connection and transaction


                if (ids.Length >= 1)
                {
                    #region Update Settings
                    for (int i = 0; i < ids.Length - 1; i++)
                    {
                        DataTable dt = new DataTable();
                        DataTable dt1 = new DataTable();
                        string InvestmentId = "";
                        bool IsEncashed = false;

                        sqlText = @"
 
SELECT Id,isnull(IsEncashed,0)IsEncashed FROM Investments
WHERE ID IN(
SELECT InvestmentId FROM InvestmentRenew
WHERE ID=@Id)

";
                        SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                        da.SelectCommand.Parameters.AddWithValue("@Id", ids[i]);
                        da.SelectCommand.Transaction = transaction;
                        da.Fill(dt);

                        if (dt == null || dt.Rows.Count <= 0)
                        {
                            retResults[3] = sqlText;
                            retResults[1] = "No Data Found for Encash";

                            throw new ArgumentNullException(retResults[1], "");
                        }
                        else
                        {
                            InvestmentId = dt.Rows[0]["Id"].ToString();
                            IsEncashed = Convert.ToBoolean(dt.Rows[0]["IsEncashed"].ToString());
                            if (IsEncashed)
                            {
                                retResults[3] = sqlText;
                                retResults[1] = "This Investment already Encashed";
                                throw new ArgumentNullException(retResults[1], "");

                            }
                            else
                            {
                                sqlText = @"
 
                                SELECT * FROM InvestmentRenew
                                WHERE ID>@Id
                                and InvestmentId=@InvestmentId
";
                                da = new SqlDataAdapter(sqlText, currConn);
                                da.SelectCommand.Parameters.AddWithValue("@Id", ids[i]);
                                da.SelectCommand.Parameters.AddWithValue("@InvestmentId", InvestmentId);
                                da.SelectCommand.Transaction = transaction;
                                da.Fill(dt1);
                                if (dt1.Rows.Count > 0)
                                {
                                    retResults[3] = sqlText;
                                    retResults[1] = "You have another renewal for Enchasment";
                                    throw new ArgumentNullException(retResults[1], "");
                                }

                            }

                        }

                        sqlText = "";
                        sqlText = @"UPDATE InvestmentRenew SET IsEncashed=1
                                WHERE ID=@Id
                                UPDATE Investments SET IsEncashed=1
                                WHERE ID IN(
                                SELECT InvestmentId FROM InvestmentRenew
                                WHERE ID=@Id)";
                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Parameters.AddWithValue("@Id", ids[i]);
                        cmdUpdate.Transaction = transaction;
                        var exeRes = cmdUpdate.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                        if (transResult <= 0)
                        {
                            retResults[3] = sqlText;
                            retResults[1] = "Unexpected error to update Accounts.";
                            throw new ArgumentNullException(retResults[1], "");
                        }


                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Investment Information Encash - Could not found any item.", "");
                }

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Data Encash Successfully.";
                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
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



        ////==================Report=================
        public DataTable Report(InvestmentRenewVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
inv.Id
,inv.InvestmentTypeId
,inv.ReferenceNo
,eit.Name InvestmentType
,inv.InvestmentAddress
,inv.InvestmentDate
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentRate
,inv.InvestmentValue
,inv.Remarks
,inv.Post
from InvestmentRenew inv
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id
WHERE  1=1 AND  inv.IsArchive = 0
";
                //ReferenceNo
                //InvestmentTypeId
                //InvestmentType
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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.Fill(dt);
                string[] DtColumnChange = { "InvestmentDate", "FromDate", "ToDate", "MaturityDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, DtColumnChange);

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
            return dt;
        }


        ////==================Report=================
        public DataTable InvestmentStatementReport(InvestmentRenewVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
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
inv.Id
,inv.InvestmentTypeId
,inv.ReferenceNo
,eit.Name InvestmentType
,inv.InvestmentAddress
,inv.InvestmentDate
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentRate
,inv.InvestmentValue
,inv.Remarks
,inv.Post

,ISNULL(roi.ROIDate				,'NA')ROIDate
,ISNULL(roi.ROITotalValue		,0)ROITotalValue
,ISNULL(roi.ROIRate				,0)ROIRate
,ISNULL(roi.TotalInterestValue	,0)TotalInterestValue
,ISNULL(roi.Remarks				,'NA')ROIRemarks

from InvestmentRenew inv
WHERE  1=1 AND  inv.IsArchive = 0
";
                //ReferenceNo
                //InvestmentTypeId
                //InvestmentType
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

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

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
                        da.SelectCommand.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                da.Fill(dt);
                string[] DtColumnChange = { "ROIDate", "InvestmentDate", "FromDate", "ToDate", "MaturityDate" };
                dt = Ordinary.DtMultiColumnStringToDate(dt, DtColumnChange);

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
            return dt;
        }


        #endregion
    }
}
