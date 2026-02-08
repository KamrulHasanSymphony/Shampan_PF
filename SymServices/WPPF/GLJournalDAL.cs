using SymOrdinary;
using SymServices.Common;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace SymServices.WPPF
{
    public class GLJournalDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        #region Methods


        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of GLJournalVM records from the database based on the specified criteria.
        /// </summary>
        /// <param name="Id">Optional. The ID of a specific GLJournal record to retrieve. If 0, retrieves all records matching conditions.</param>
        /// <param name="conditionFields">Optional. Array of column names to be used as filter conditions.</param>
        /// <param name="conditionValues">Optional. Array of values corresponding to the conditionFields to filter the records.</param>
        /// <param name="VcurrConn">Optional. An existing SqlConnection to be used for the query; if null, a new connection is opened.</param>
        /// <param name="Vtransaction">Optional. An existing SqlTransaction to be used; if null, a new transaction is started for this operation.</param>
        /// <returns>A list of GLJournalVM objects that match the specified filters and are not archived.</returns>
        public List<GLJournalVM> SelectById(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
                  , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLJournalVM> VMs = new List<GLJournalVM>();
            GLJournalVM vm;
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
gl.Id
,gl.Code
,gl.TransactionDate
,jtt.Name TransactionTypeName
,gl.TransactionType

,isnull(gl.TransactionValue,0)TransactionValue
,gl.JournalType
,jt.Name JournalTypeName

,gl.Remarks
,gl.IsActive
,gl.IsArchive
,gl.CreatedBy
,gl.CreatedAt
,gl.CreatedFrom
,gl.LastUpdateBy
,gl.LastUpdateAt
,gl.LastUpdateFrom
,gl.Post
from GLJournals gl left outer join EnumJournalType jt on gl.JournalType = jt.Id
 left outer join EnumJournalTransactionType jtt on gl.TransactionType= jtt.Id

WHERE  1=1 AND IsArchive = 0
";

                if (Id > 0)
                {
                    sqlText += @" and gl.Id=@Id";
                }
                sqlText += " order by TransactionDate desc";               
               
                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);               
                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLJournalVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDate = Ordinary.StringToDate(dr["TransactionDate"].ToString());

                    vm.Remarks = dr["Remarks"].ToString();

                    vm.TransactionTypeName = dr["TransactionTypeName"].ToString();
                    vm.TransactionType = Convert.ToInt32(dr["TransactionType"]);
                    vm.TransactionValue = Convert.ToDecimal(dr["TransactionValue"]);

                    vm.JournalType = Convert.ToInt32(dr["JournalType"]);
                    vm.JournalTypeName = dr["JournalTypeName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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

        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of GLJournalDetailVM records from the database with optional filtering by Id and additional conditions.
        /// </summary>
        /// <param name="Id">Optional identifier to filter the results by the GLJournalDetail Id. Default is 0 which means no Id filtering.</param>
        /// <param name="conditionFields">Optional array of field names to apply additional filtering conditions.</param>
        /// <param name="conditionValues">Optional array of field values corresponding to conditionFields for filtering.</param>
        /// <param name="VcurrConn">Optional existing SqlConnection to use. If null, a new connection will be created and managed.</param>
        /// <param name="Vtransaction">Optional existing SqlTransaction to use. If null, a new transaction will be started and committed within the method.</param>
        /// <returns>Returns a list of GLJournalDetailVM objects matching the specified conditions.</returns>
        public List<GLJournalDetailVM> SelectAllDetails(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
                  , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLJournalDetailVM> VMs = new List<GLJournalDetailVM>();
            GLJournalDetailVM vm;
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
gd.Id
,gd.GLJournalId
,gd.COAId
,d.Name AccountName
,gd.IsDr
,gd.DrAmount
,gd.CrAmount
,gd.TransactionDate
,gd.Remarks

from GLJournalDetails gd
left outer join COAs d on gd.COAId = d.id 

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
                    vm = new GLJournalDetailVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.AccountName = dr["AccountName"].ToString();
                    vm.GLJournalId = Convert.ToInt32(dr["GLJournalId"].ToString());
                    vm.COAId = Convert.ToInt32(dr["COAId"].ToString());
                    vm.IsDr = Convert.ToBoolean(dr["IsDr"].ToString());
                    vm.DrAmount = Convert.ToDecimal(dr["DrAmount"].ToString());
                    vm.CrAmount = Convert.ToDecimal(dr["CrAmount"].ToString());

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
        //==================Insert =================
        /// <summary>
        /// Inserts a new GLJournal record along with its details into the database.
        /// Manages database connection and transaction if not provided externally.
        /// Generates and assigns a code based on JournalType and transaction details.
        /// Rolls back transaction and returns failure result if any error occurs during insertion.
        /// </summary>
        /// <param name="vm">The GLJournalVM object containing journal and journal details data to insert.</param>
        /// <param name="VcurrConn">Optional existing SQL connection to use for this operation.</param>
        /// <param name="Vtransaction">Optional existing SQL transaction to use for this operation.</param>
        /// <returns>
        /// An array of strings containing:
        /// [0] - "Success" or "Fail" indicating the operation result.
        /// [1] - Message describing the result.
        /// [2] - The ID of the inserted GLJournal record (if successful).
        /// [3] - The SQL query executed or error context.
        /// [4] - Exception message if failed.
        /// [5] - Name of the method ("InsertAccount").
        /// </returns>
        public string[] Insert(GLJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertAccount"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable dt = new DataTable();
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


                vm.Id = _cDal.NextId("GLJournals", currConn, transaction);

                if (vm.Code != null)
                {
                    string glcode = "SELECT * FROM GLJournals WHERE Code = @Code";
                    SqlCommand cmd = new SqlCommand(glcode, currConn);
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Transaction = transaction;
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                
                sqlText = "select isnull(count(id),0)+1 FROM  GLJournals where TransactionType=@TransactionType";
                SqlCommand cmd2 = new SqlCommand(sqlText, currConn);
                cmd2.Transaction = transaction;
                cmd2.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                var idExecuteScalar = cmd2.ExecuteScalar();
                int nextId = Convert.ToInt32(idExecuteScalar);

                if (vm.Code == null || vm.Code == "0")
                {
                  
                    if (vm.JournalType == 1)
                    {
                        string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "JournalVoucher", vm.TransactionDate, currConn, transaction);
                        vm.Code = NewCode;                    
                    }
                    else if (vm.JournalType == 2)
                    {
                        string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "PaymentVoucher", vm.TransactionDate, currConn, transaction);
                        vm.Code = NewCode;                     
                    }
                    else if (vm.JournalType == 3)
                    {
                        string NewCode = new CommonDAL().CodeGenerationPF(vm.TransType, "ReceiptVoucher", vm.TransactionDate, currConn, transaction);
                        vm.Code = NewCode;                     
                    }
                }             
                if (vm != null)
                {
                    sqlText = "  ";
                    sqlText += @" 
                        Delete from GLJournals where Code=@code;                         
                        INSERT INTO GLJournals(
                        Code
                        ,TransactionDate
                        ,TransactionType
                        ,JournalType
                        ,Remarks
                        ,IsActive
                        ,IsArchive
                        ,CreatedBy
                        ,CreatedAt
                        ,CreatedFrom
                        ,Post
                        ,TransType
                        ,IsYearClosing,BranchId

                        ) VALUES (
                        @Code
                        ,@TransactionDate
                        ,@TransactionType
                        ,@JournalType
                        ,@Remarks
                        ,@IsActive
                        ,@IsArchive
                        ,@CreatedBy
                        ,@CreatedAt
                        ,@CreatedFrom
                        ,@Post
                        ,@TransType
                        ,@IsYearClosing,@BranchId
                        ) 
                        select scope_Identity();
                        ";
                    SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                    cmdInsert.Parameters.AddWithValue("@Code", vm.Code);
                    cmdInsert.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.TransactionDate));
                    cmdInsert.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdInsert.Parameters.AddWithValue("@JournalType", vm.JournalType);
                    cmdInsert.Parameters.AddWithValue("@IsYearClosing", vm.IsYearClosing);
                    cmdInsert.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    cmdInsert.Parameters.AddWithValue("@IsActive", true);
                    cmdInsert.Parameters.AddWithValue("@IsArchive", false);
                    cmdInsert.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmdInsert.Parameters.AddWithValue("@CreatedAt", Ordinary.DateToString(vm.CreatedAt));
                    cmdInsert.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmdInsert.Parameters.AddWithValue("@Post", false);
                    cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");
                    cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);



                    var exeRes = cmdInsert.ExecuteScalar();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLJournals.", "");
                    }

                    vm.Id = transResult;
                    if (vm.GLJournalDetails.Count > 0)
                    {
                        retResults = DeleteJournaltDetails(vm, currConn, transaction);
                        if (retResults[0] == "Success")
                        {
                            retResults = InsertDetails(vm, currConn, transaction);
                        }
                        if (retResults[0] != "Success")
                        {
                            retResults[3] = retResults[4];
                            throw new ArgumentNullException("Unexpected error to update GLJournals.", "");
                        }
                    }
                    else
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLJournals.", "");
                    }



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

                if(dt.Rows.Count>0)
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Journal Already Created!!";
                }
                else
                {
                    retResults[0] = "Success";
                    retResults[1] = "Journal Created Successfully.";
                }              

                retResults[2] = transResult.ToString();
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

        /// <summary>
        /// Inserts or updates net profit year-end data based on the specified transaction type.
        /// Executes a SQL script that calculates and stores net profit and retained earnings for each fiscal year.
        /// Handles connection and transaction management with optional external SqlConnection and SqlTransaction.
        /// </summary>
        /// <param name="TransType">The transaction type identifier used in the SQL query to filter data (e.g., "PF", "GF").</param>
        /// <param name="VcurrConn">An optional existing SqlConnection to use; if null, a new connection will be created.</param>
        /// <param name="Vtransaction">An optional existing SqlTransaction to use; if null, a new transaction will be started.</param>
        /// <returns>
        /// A string array containing the results:
        /// [0] = "Success" or "Fail" indicating the operation outcome,
        /// [1] = Message describing the success or failure,
        /// [2] = The number of affected rows as a string,
        /// [3] = The executed SQL query text,
        /// [4] = Exception message if any occurred,
        /// [5] = The method name ("InsertAccount").
        /// </returns>
        public string[] InsertNP(string TransType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertAccount"; //Method Name
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

                sqlText = "  ";
                sqlText += @" 
declare @YearStart as varchar(100);
declare @YearEnd as varchar(100);
declare @Year as varchar(100);
--declare @TransType as varchar(100)='PF' -- closing

declare @RetainedEarningsCOAId as varchar(100)
select @RetainedEarningsCOAId=id from COAs  where isnull(IsRetainedEarning,0)=1   and isnull(TransType,'PF') in(@TransType)

create table #FiscalYear(Id int identity(1,1),[Year] varchar(100),YearStart varchar(100),YearEnd varchar(100))

insert into #FiscalYear([Year],YearStart,YearEnd)
select [Year],YearStart,YearEnd from HRMDB.dbo.FiscalYear



TRUNCATE TABLE NetProfitYearEnds; 
DBCC CHECKIDENT ('NetProfitYearEnds', RESEED, 1);

DECLARE @Counter INT 
SET @Counter=1
WHILE ( @Counter <= (select count(id) from #FiscalYear) )
BEGIN
select @Year=[Year],@YearStart=YearStart ,@YearEnd=YearEnd from #FiscalYear where id=@Counter
 
 insert into NetProfitYearEnds(TransType,[Year],YearStart,YearEnd,COAId,CoaType,TransactionAmount,NetProfit,RetainedEarning)
 select @TransType,@Year,@YearStart,@YearEnd,0,'',0 TransactionAmount,sum(NetProfit)NetProfit,sum(RetainedEarning)RetainedEarning from 
 (
select  isnull(Sum(TransactionAmount),0)NetProfit, 0 RetainedEarning
from View_GLJournalDetailNew 
where transactionDate >=@YearStart and transactionDate <=@YearEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')
union all
select 0 NetProfit,isnull(Sum(TransactionAmount),0)TransactionAmount 
from View_GLJournalDetailNew 
where transactionDate >=@YearStart and transactionDate <= @YearEnd  and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAId in (@RetainedEarningsCOAId)
) as a

    SET @Counter  = @Counter  + 1
END

SET @Counter=1
WHILE ( @Counter <= (select count(id) from NetProfitYearEnds) )
BEGIN
declare @NetProfit as decimal(18,4)=0;
declare @RetainedEarning  as decimal(18,4)=0;

select @NetProfit=NetProfit,@RetainedEarning=RetainedEarning from NetProfitYearEnds where id=@Counter-1
update NetProfitYearEnds set RetainedEarning=RetainedEarning+@RetainedEarning+@NetProfit where id=@Counter

    SET @Counter  = @Counter  + 1
END

--select * from NetProfitYearEnds

drop table #FiscalYear
";

                
                sqlText = sqlText.Replace("NetProfitYearEnds", TransType.ToLower() == "gf" ? "NetProfitGFYearEnds" : "NetProfitYearEnds");

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@TransType", TransType);

                var exeRes = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Unexpected error", "");
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
                retResults[2] = transResult.ToString();
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
        /// Updates a GLJournal record and its related details in the database. 
        /// If no external connection or transaction is provided, a new connection and transaction are created and managed 
        public string[] Update(GLJournalVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Update"; //Method Name
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
                if (transaction == null) { transaction = currConn.BeginTransaction("UpdateToAccount"); }
                #endregion open connection and transaction

                if (vm != null)
                {
                    #region Update Settings
                    sqlText = "";
                    sqlText = "UPDATE GLJournals SET";
                    sqlText += "  Post=@Post";
                    sqlText += " , TransactionType=@TransactionType";
                    sqlText += " , TransactionDate=@TransactionDate";
                    sqlText += " , Remarks=@Remarks";
                    //sqlText += " , IsActive=@IsActive";
                    sqlText += " , LastUpdateBy=@LastUpdateBy";
                    sqlText += " , LastUpdateAt=@LastUpdateAt";
                    sqlText += " , LastUpdateFrom=@LastUpdateFrom";
                    sqlText += " , TransType=@TransType";

                    sqlText += " WHERE Id=@Id";
                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", vm.Id);
                    cmdUpdate.Parameters.AddWithValue("@Post", vm.Post);
                    cmdUpdate.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmdUpdate.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(vm.TransactionDate));
                    cmdUpdate.Parameters.AddWithValue("@Remarks", vm.Remarks ?? Convert.DBNull);
                    //cmdUpdate.Parameters.AddWithValue("@IsActive", vm.IsActive);

                    cmdUpdate.Parameters.AddWithValue("@LastUpdateBy", vm.LastUpdateBy);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateAt", vm.LastUpdateAt);
                    cmdUpdate.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmdUpdate.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");

                    cmdUpdate.Transaction = transaction;
                    int exeRes = cmdUpdate.ExecuteNonQuery();
                    transResult = Convert.ToInt32(exeRes);
                    if (transResult <= 0)
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLJournals.", "");
                    }

                    string deleteDetails = "delete from GLJournalDetails where GLJournalId = @GLJournalId";
                    cmdUpdate.CommandText = deleteDetails;
                    cmdUpdate.Parameters.AddWithValue("@GLJournalId", vm.Id);
                    cmdUpdate.ExecuteNonQuery();

                    if (vm.GLJournalDetails.Count > 0)
                    {
                        InsertDetails(vm, currConn, transaction);
                    }
                    else
                    {
                        retResults[3] = sqlText;
                        throw new ArgumentNullException("Unexpected error to update GLJournals.", "");
                    }

                    retResults[2] = vm.Id.ToString();// Return Id
                    retResults[3] = sqlText; //  SQL Query
                    #region Commit
                    if (transResult <= 0)
                    {
                        // throw new ArgumentNullException("Account Update", vm.BranchId + " could not updated.");
                    }
                    #endregion Commit
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("Account Update", "Could not found any item.");
                }
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Data Update Successfully.";
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

        /// <summary>
        /// Inserts GLJournal details into the database. Optionally accepts an existing SQL connection and transaction.
        /// Handles connection and transaction management if not provided.
        /// Updates the GLJournals header with the sum of DrAmount after inserting details.
        /// </summary>
        /// <param name="GLJournalVM">The GLJournal view model containing journal details to insert.</param>
        /// <param name="VcurrConn">Optional: Existing SQL connection to use.</param>
        /// <param name="Vtransaction">Optional: Existing SQL transaction to use.</param>
        /// <returns>
        /// An array of strings containing:
        /// [0] - Status ("Success" or "Fail")
        /// [1] - Status message or error message
        /// [2] - Return Id (currently always 0)
        /// [3] - The executed SQL query string
        /// [4] - Exception message if any
        /// [5] - Method name identifier ("GLJournalDetailVM")
        /// </returns>
        public string[] DeleteJournaltDetails(GLJournalVM GLJournalVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int NextId = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = NextId.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLJournalDetailVM"; //Method Name
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

                List<GLJournalDetailVM> VMs = GLJournalVM.GLJournalDetails;
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
Delete from GLJournalDetails where GLJournalId=@GLJournalId
";
                    #endregion SqlText

                    #region SqlExecution
                    foreach (GLJournalDetailVM vm in VMs)
                    {

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@GLJournalId", GLJournalVM.Id);                    
                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    #endregion SqlExecution                   
                }
                else
                {
                    retResults[1] = "No Detail Found!";
                    throw new ArgumentNullException(retResults[1], "");
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
                retResults[2] = NextId.ToString();
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
        public string[] InsertDetails(GLJournalVM GLJournalVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Initializ
            string sqlText = "";
            int NextId = 0;
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = NextId.ToString();// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "GLJournalDetailVM"; //Method Name
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

                List<GLJournalDetailVM> VMs = GLJournalVM.GLJournalDetails;
                if (VMs != null && VMs.Count > 0)
                {
                    #region SqlText
                    sqlText = "  ";
                    sqlText += @" 
INSERT INTO GLJournalDetails (
 GLJournalId
,COAId
,IsDr
,DrAmount
,CrAmount
,TransactionDate
,TransactionType
,JournalType
,Remarks
,TransType
,IsYearClosing


) VALUES (
@GLJournalId
,@COAId
,@IsDr
,@DrAmount
,@CrAmount
,@TransactionDate
,@TransactionType
,@JournalType
,@Remarks
,@TransType
,@IsYearClosing
) 
";
                    #endregion SqlText

                    #region SqlExecution
                    foreach (GLJournalDetailVM vm in VMs)
                    {

                        SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                        cmdInsert.Parameters.AddWithValue("@GLJournalId", GLJournalVM.Id);
                        cmdInsert.Parameters.AddWithValue("@COAId", vm.COAId);
                        cmdInsert.Parameters.AddWithValue("@DrAmount", vm.DrAmount);
                        cmdInsert.Parameters.AddWithValue("@CrAmount", vm.CrAmount);
                        cmdInsert.Parameters.AddWithValue("@IsDr", vm.DrAmount > 0 ? 1 : 0);

                        cmdInsert.Parameters.AddWithValue("@TransactionDate", Ordinary.DateToString(GLJournalVM.TransactionDate));
                        cmdInsert.Parameters.AddWithValue("@TransactionType", GLJournalVM.TransactionType);
                        cmdInsert.Parameters.AddWithValue("@JournalType", GLJournalVM.JournalType);
                        cmdInsert.Parameters.AddWithValue("@IsYearClosing", GLJournalVM.IsYearClosing);
                        cmdInsert.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(vm.Remarks) ? Convert.DBNull : vm.Remarks);
                        cmdInsert.Parameters.AddWithValue("@TransType", GLJournalVM.TransType ?? "PF");

                        var exeRes = cmdInsert.ExecuteNonQuery();
                        transResult = Convert.ToInt32(exeRes);
                    }
                    #endregion SqlExecution
                    #region MyRegion
                    string headerUpdate = @"update  GLJournals set TransactionValue=d.DrAmount
                                        from 
                                        (select  distinct GLJournalId,sum(DrAmount)DrAmount from GLJournalDetails
                                        where GLJournalId=@Id
                                        group by GLJournalId) d
                                        where GLJournals.Id=d.GLJournalId
                                        and id =@Id";

                    SqlCommand cmdUpdate = new SqlCommand(headerUpdate, currConn);
                    cmdUpdate.Parameters.AddWithValue("@Id", GLJournalVM.Id);
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.ExecuteNonQuery();


                    #endregion
                }
                else
                {
                    retResults[1] = "No Detail Found!";
                    throw new ArgumentNullException(retResults[1], "");
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
                retResults[2] = NextId.ToString();
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
        /// <summary>
        /// Posts the records identified by the provided IDs to the database, optionally using an existing SQL connection and transaction.
        /// Commits the transaction if it is created within this method.
        /// </summary>
        /// <param name="ids">An array of record IDs to be posted.</param>
        /// <param name="VcurrConn">An optional existing SQL connection to use.</param>
        /// <param name="Vtransaction">An optional existing SQL transaction to use.</param>
        /// <returns>
        /// A string array containing:
        /// [0] - "Success" or "Fail" indicating the operation result;
        /// [1] - Success or error message;
        /// [2] - The return Id as string (default "0");
        /// [3] - The SQL query text (default "sqlText");
        /// [4] - Exception message if failed;
        /// [5] - The method name ("Loan").
        /// </returns>
        public string[] Post(string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "Loan"; //Method Name
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
                        retResults = _cDal.FieldPost("GLJournals", "Id", ids[i], currConn, transaction);
                        if (retResults[0].ToLower() == "fail")
                        {
                            throw new ArgumentNullException("GLJournals Post", ids[i] + " could not Post.");
                        }
                    }
                    #endregion Update Settings
                }
                else
                {
                    throw new ArgumentNullException("GLJournals Post - Could not found any item.", "");
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


        ////==================Report=================
        /// <summary>
        /// Generates a DataTable report of GL journal entries with optional filtering conditions, using the provided transaction and connection if specified.
        /// </summary>
        /// <param name="vm">The GLJournalVM view model instance (currently unused in method body).</param>
        /// <param name="conditionFields">An array of field names to be used as filter conditions in the SQL WHERE clause.</param>
        /// <param name="conditionValues">An array of corresponding values for the filter conditions.</param>
        /// <param name="VcurrConn">An optional existing SQL connection to use; if null, a new connection is opened.</param>
        /// <param name="Vtransaction">An optional existing SQL transaction to use; if null, transaction management is handled 
        public DataTable Report(GLJournalVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
select h.Id, h.Code TransactionCode,h.TransactionDate, jtt.Name TransactionType,h.JournalType,isnull(h.Remarks,'-')HeaderNarration,h.Post
,c.Name AccountName,c.Code AccountCode,  d.IsDr,d.DrAmount DebitAmount,d.CrAmount CreditAmount,isnull(d.Remarks,'-') DetailNarration
 from GLJournalDetails d
left outer join GLJournals h on d.GLJournalId=h.Id
left outer join COAs c on d.COAId=c.Id
left outer join EnumJournalTransactionType jtt on h.TransactionType=jtt.Id
left outer join EnumJournalType jt on h.JournalType=jt.Id
where 1=1 
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

                sqlText += @" order by IsDr desc,d.Id";

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
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");

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


        public List<GLJournalVM> SelectAll(int JournalType, string[] conditionFields = null, string[] conditionValues = null
                 , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<GLJournalVM> VMs = new List<GLJournalVM>();
            GLJournalVM vm;
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
gl.Id
,gl.Code
,gl.TransactionDate
,jtt.Name TransactionTypeName
,gl.TransactionType

,isnull(gl.TransactionValue,0)TransactionValue
,gl.JournalType
,jt.Name JournalTypeName

,gl.Remarks
,gl.IsActive
,gl.IsArchive
,gl.CreatedBy
,gl.CreatedAt
,gl.CreatedFrom
,gl.LastUpdateBy
,gl.LastUpdateAt
,gl.LastUpdateFrom
,gl.Post
from GLJournals gl left outer join EnumJournalType jt on gl.JournalType = jt.Id
 left outer join EnumJournalTransactionType jtt on gl.TransactionType= jtt.Id

WHERE  1=1 AND IsArchive = 0
";

                if (JournalType > 0)
                {
                    sqlText += @" and gl.JournalType=@Id";
                }
                sqlText += " order by TransactionDate desc";

                SqlCommand objComm = new SqlCommand(sqlText, currConn, transaction);
                if (JournalType > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", JournalType);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new GLJournalVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.Post = Convert.ToBoolean(dr["Post"]);
                    vm.Code = dr["Code"].ToString();
                    vm.TransactionDate = Ordinary.StringToDate(dr["TransactionDate"].ToString());

                    vm.Remarks = dr["Remarks"].ToString();

                    vm.TransactionTypeName = dr["TransactionTypeName"].ToString();
                    vm.TransactionType = Convert.ToInt32(dr["TransactionType"]);
                    vm.TransactionValue = Convert.ToDecimal(dr["TransactionValue"]);

                    vm.JournalType = Convert.ToInt32(dr["JournalType"]);
                    vm.JournalTypeName = dr["JournalTypeName"].ToString();
                    vm.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    vm.IsArchive = Convert.ToBoolean(dr["IsArchive"]);
                    vm.CreatedAt = Ordinary.StringToDate(dr["CreatedAt"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedFrom = dr["CreatedFrom"].ToString();
                    vm.LastUpdateAt = Ordinary.StringToDate(dr["LastUpdateAt"].ToString());
                    vm.LastUpdateBy = dr["LastUpdateBy"].ToString();
                    vm.LastUpdateFrom = dr["LastUpdateFrom"].ToString();

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
        #endregion
    }
}
