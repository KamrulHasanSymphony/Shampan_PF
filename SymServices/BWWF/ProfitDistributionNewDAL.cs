using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SymOrdinary;
using SymServices.Common;
using SymViewModel.Common;
using SymViewModel.BWWF;

namespace SymServices.BWWF
{
    public class ProfitDistributionNewDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion


        //==================SelectAll=================
        /// <summary>
        /// Retrieves a list of Profit Distribuion from the database, optionally filtered by ID or additional conditions.
        /// Supports external SQL connection and transaction handling for reuse within larger operations.
        /// </summary>
        /// <param name="Id">Optional ID to filter a specific Profit Distribuion.</param>
        /// <param name="conditionFields">Array of column names to be used as additional filter conditions.</param>
        /// <param name="conditionValues">Array of values corresponding to each filter condition.</param>
        /// <param name="VcurrConn">An optional SQL connection. If not provided, a new connection is established.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If not provided, a new transaction is created and committed.</param>
        /// <returns>A list of <see cref="ProfitDistributionNewVM"/> representing the Profit Distribuion matching the criteria.</returns>
        public List<ProfitDistributionNewVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionNewVM> VMs = new List<ProfitDistributionNewVM>();
            ProfitDistributionNewVM vm;
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
                #region SqlText

                sqlText = @"
SELECT
 pd.Id
,fydFrom.PeriodName PeriodNameFrom

,pd.Id
,ve.Code
,ve.EmpName
,pd.PreDistributionFundId
,pd.EmployeeId
,pd.DistributionDate
,pd.FiscalYearDetailId
,pd.EmployeeContribution
,pd.EmployerContribution
,pd.EmployeeProfit
,pd.EmployerProfit
,pd.MultiplicationFactor
,pd.EmployeeProfitDistribution
,pd.EmployeerProfitDistribution
,pd.TotalProfit
,ISNULL(pd.IsPaid,0) IsPaid
,pd.Post
,pd.Remarks
,pd.IsActive
,pd.IsArchive
,pd.CreatedBy
,pd.CreatedAt
,pd.CreatedFrom
,pd.LastUpdateBy
,pd.LastUpdateAt
,pd.LastUpdateFrom

FROM ProfitDistributionNew pd
";
                sqlText = sqlText + @" LEFT OUTER JOIN [dbo].FiscalYearDetail fydFrom ON pd.FiscalYearDetailId=fydFrom.Id";
                sqlText = sqlText + @" LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON ve.EmployeeId=pd.EmployeeId";

                sqlText = sqlText + @" WHERE  1=1 AND pd.IsArchive = 0";


                if (Id > 0)
                {
                    sqlText += @" and pd.PreDistributionFundId=@Id";
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

                if (Id > 0)
                {
                    objComm.Parameters.AddWithValue("@Id", Id);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new ProfitDistributionNewVM();
                    vm.Id = Convert.ToInt32(dr["Id"]);
                    vm.PeriodNameFrom = dr["PeriodNameFrom"].ToString();
                    vm.EmployeeCode = dr["Code"].ToString();
                    vm.EmployeeName = dr["EmpName"].ToString();

                    //vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    //vm.PFDetailFiscalYearDetailIds = dr["PFDetailFiscalYearDetailIds"].ToString();
                    //vm.PreDistributionFundIds = dr["PreDistributionFundIds"].ToString();
                    //vm.DistributionDate = Ordinary.StringToDate(dr["DistributionDate"].ToString());
                    //vm.TotalEmployeeContribution = Convert.ToDecimal(dr["TotalEmployeeContribution"]);
                    //vm.TotalEmployerContribution = Convert.ToDecimal(dr["TotalEmployerContribution"]);
                    //vm.TotalProfit = Convert.ToDecimal(dr["TotalProfit"]);

                    //vm.FiscalYearDetailIdTo = Convert.ToInt32(dr["FiscalYearDetailIdTo"]);
                    //vm.TotalExpense = Convert.ToDecimal(dr["TotalExpense"]);
                    //vm.AvailableDistributionAmount = Convert.ToDecimal(dr["AvailableDistributionAmount"]);

                    //vm.TotalWeightedContribution = Convert.ToDecimal(dr["TotalWeightedContribution"]);
                    //vm.MultiplicationFactor = Convert.ToDecimal(dr["MultiplicationFactor"]);
                    vm.TotalProfit = Convert.ToDecimal(dr["TotalProfit"]);

                    vm.PreDistributionFundId = Convert.ToString(dr["PreDistributionFundId"]);
                    vm.EmployeeId = Convert.ToString(dr["PreDistributionFundId"]);
                    vm.DistributionDate = Ordinary.StringToDate(dr["DistributionDate"].ToString());
                    vm.FiscalYearDetailId = Convert.ToInt32(dr["FiscalYearDetailId"]);
                    vm.EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    vm.EmployerContribution = Convert.ToDecimal(dr["EmployerContribution"]);
                    vm.EmployeeProfit = Convert.ToDecimal(dr["EmployeeProfit"]);
                    vm.EmployerProfit = Convert.ToDecimal(dr["EmployerProfit"]);
                    vm.MultiplicationFactor = Convert.ToDecimal(dr["MultiplicationFactor"]);
                    vm.EmployeeProfitDistribution = Convert.ToDecimal(dr["EmployeeProfitDistribution"]);
                    vm.EmployeerProfitDistribution = Convert.ToDecimal(dr["EmployeerProfitDistribution"]);


                    vm.IsPaid = Convert.ToBoolean(dr["IsPaid"]);
                    //  vm.TransactionType = dr["TransactionType"].ToString();
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
                dr.Close();
                #endregion SqlExecution

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

        /// <summary>
        /// Inserts a new  Profit Distribuion record into the database with the provided details from the view model.
        /// Handles optional SQL connection and transaction management for use in broader transactional operations.
        /// </summary>
        /// <param name="vm">The <see cref="ProfitDistributionNewVM"/> containing the  Profit Distribuion data to insert.</param>
        /// <param name="VcurrConn">An optional external SQL connection. If null, a new connection is created.</param>
        /// <param name="Vtransaction">An optional SQL transaction. If null, a new transaction is created and committed.</param>
        /// <returns>
        /// A string array with the following structure:
        /// [0] = "Success" or "Fail",  
        /// [1] = Message describing the result,  
        /// [2] = Inserted record ID (if successful),  
        /// [3] = Executed SQL query,  
        /// [4] = Exception message (if any),  
        /// [5] = Method name ("InsertBankBranch").
        /// </returns>
        public ResultVM Process(ProfitDistributionNewVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<ProfitDistributionNewVM> VMs = new List<ProfitDistributionNewVM>();

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

                string Date = Convert.ToDateTime(vm.DistributionDate).ToString("yyyyMMdd");

                FiscalYearDetailVM fvm = new FiscalYearDetailVM();
                FiscalYearDAL fdal = new FiscalYearDAL();
                fvm = fdal.SelectAll_FiscalYearDetailByDate(Date);

                SettingDAL _settingDal = new SettingDAL();
                string IsWeightedAverageMonth = _settingDal.settingValue("BWWF", "IsWeightedAverageMonth").Trim();
                if (IsWeightedAverageMonth == "N")
                {

                    sqlText = @"

  SELECT 
    EmployeeId,
    ISNULL([1], 0) AS [11],
    ISNULL([2], 0) AS [10],
    ISNULL([3], 0) AS [9],
    ISNULL([4], 0) AS [8],
    ISNULL([5], 0) AS [7],
    ISNULL([6], 0) AS [6],
    ISNULL([7], 0) AS [5],
    ISNULL([8], 0) AS [4],
    ISNULL([9], 0) AS [3],
    ISNULL([10], 0) AS [2],
    ISNULL([11], 0) AS [1],
    ISNULL([12], 0) AS [0],
	  (
        ISNULL([1], 0) + ISNULL([2], 0) + ISNULL([3], 0) + ISNULL([4], 0) + ISNULL([5], 0) +
        ISNULL([6], 0) + ISNULL([7], 0) + ISNULL([8], 0) + ISNULL([9], 0) + ISNULL([10], 0) +
        ISNULL([11], 0) + ISNULL([12], 0)
    ) AS [TotalValue],
	(
	   (ISNULL([1], 0)*11 +ISNULL([2], 0)*10+ISNULL([3], 0)*9+ISNULL([4], 0)*8+ISNULL([5], 0)*7+ISNULL([6], 0)*6+ISNULL([7], 0)*5+ISNULL([8], 0)*4+ISNULL([9], 0)*3+ISNULL([10], 0)*2+ISNULL([11], 0)*1+ISNULL([12], 0)*0)/
	   ( ISNULL([1], 0) + ISNULL([2], 0) + ISNULL([3], 0) + ISNULL([4], 0) + ISNULL([5], 0) +
        ISNULL([6], 0) + ISNULL([7], 0) + ISNULL([8], 0) + ISNULL([9], 0) + ISNULL([10], 0) +
        ISNULL([11], 0) + ISNULL([12], 0)
		)
	) AS Month
FROM
(
    SELECT 
        EmployeeId,
        FiscalYearDetailId,
        (EmployeePFValue + EmployeerPFValue) AS TotalPF
    FROM PFDetails
) AS SourceTable
PIVOT
(
    SUM(TotalPF)
    FOR FiscalYearDetailId IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
) AS PivotTable
ORDER BY EmployeeId;

";
                    //                    sqlText = @"
                    //                    with cat as (
                    //                    select EmployeeId
                    //                    , sum(EmployeeContribution)EmployeeContribution
                    //                    , sum(EmployerContribution)EmployerContribution
                    //                    , sum(EmployeeProfit)EmployeeProfit
                    //                    , sum(EmployerProfit)EmployerProfit 
                    //                    , (sum(EmployeeContribution)+sum(EmployerContribution)+sum(EmployeeProfit)+sum(EmployerProfit)) TotalValue
                    //                    from (
                    //                    
                    //                    select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
                    //                    from EmployeePFOpeinig
                    //                    
                    //                    union all 
                    //                    
                    //                    select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
                    //                    from EmployeeBreakMonthPF
                    //                    
                    //                    union all 
                    //                    
                    //                    select  EmployeeId, EmployeePFValue EmployeeContribution, EmployeerPFValue,0 EmployeeProfit, 0 EmployerProfit
                    //                    from PFDetails
                    //                    
                    //                    union all 
                    //                    
                    //                    select  EmployeeId, 0 EmployeeContribution, 0 EmployerContribution,EmployeeProfitDistribution EmployeeProfit,EmployeerProfitDistribution EmployerProfit
                    //                    from ProfitDistributionNew
                    //                    )
                    //                    EmployeeProfits
                    //                    group by EmployeeId
                    //                    ) select c.*,ej.IsNoProfit from cat c
                    //                    Left Outer Join [EmployeeJob] ej on ej.EmployeeId=c.EmployeeId
                    //                    
                    //                    
                    //                    select  sum(EmployeeContribution)EmployeeContribution
                    //                    , sum(EmployerContribution)EmployerContribution
                    //                    , sum(EmployeeProfit)EmployeeProfit
                    //                    , sum(EmployerProfit)EmployerProfit 
                    //                    , (sum(EmployeeContribution)+sum(EmployerContribution)+sum(EmployeeProfit)+sum(EmployerProfit)) TotalValue
                    //                    from (
                    //                    
                    //                    select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
                    //                    from EmployeePFOpeinig
                    //                    
                    //                    union all 
                    //                    
                    //                    select EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
                    //                    from EmployeeBreakMonthPF
                    //                    
                    //                    union all 
                    //                    
                    //                    select  EmployeeId, EmployeePFValue EmployeeContribution, EmployeerPFValue,0 EmployeeProfit, 0EmployerProfit
                    //                    from PFDetails
                    //                    
                    //                    union all 
                    //                    
                    //                    select  EmployeeId, 0 EmployeeContribution, 0 EmployerContribution,EmployeeProfitDistribution EmployeeProfit,EmployeerProfitDistribution EmployerProfit
                    //                    from ProfitDistributionNew
                    //                    )
                    //                    EmployeeProfits
                    //                    
                    //                    ";
                }
                else
                {
                    sqlText = @"
                    -- Step 1: Define RankedPFDetails CTE
                    WITH RankedPFDetails AS (
                        SELECT 
                            pd.FiscalYearDetailId, 
                            pd.EmployeeId, 
                            pd.EmployeePFValue AS EmployeeContribution, 
                            pd.EmployeerPFValue AS EmployerContribution, 
                            0 AS EmployeeProfit, 
                            0 AS EmployerProfit,
                            ROW_NUMBER() OVER (PARTITION BY pd.EmployeeId ORDER BY pd.FiscalYearDetailId DESC) - 1 AS RowNum
                        FROM 
                            PFDetails pd
                        LEFT OUTER JOIN 
                            FiscalYearDetail fd 
                            ON fd.id = pd.FiscalYearDetailId
                       LEFT OUTER JOIN PFHeader ph on ph.Id=pd.PFHeaderId
                        WHERE 
                            fd.Year = @Year and ph.BranchId=@BranchId
                    )

                    -- Step 2: Use RankedPFDetails in the main query
                    , cat AS (
                        SELECT 
                            EmployeeId,
                            SUM(EmployeeContribution) AS EmployeeContribution,
                            SUM(EmployerContribution) AS EmployerContribution,
                            SUM(EmployeeProfit) AS EmployeeProfit,
                            SUM(EmployerProfit) AS EmployerProfit,
                            (SUM(EmployeeContribution) + SUM(EmployerContribution) + SUM(EmployeeProfit) + SUM(EmployerProfit)) AS TotalValue
                        FROM (
                            -- First Source: EmployeePFOpeinig
                            SELECT 
                                EmployeeId, 
                                  EmployeeContribution *12 EmployeeContribution, 
                                EmployerContribution * 12 EmployerContribution, 
                                EmployeeProfit, 
                                EmployerProfit
                            FROM EmployeePFOpeinig

                            UNION ALL

                            -- Second Source: EmployeeBreakMonthPF
                            SELECT 
                                EmployeeId, 
                                EmployeeContribution, 
                                EmployerContribution, 
                                EmployeeProfit, 
                                EmployerProfit
                            FROM EmployeeBreakMonthPF

                            UNION ALL

                            -- Third Source: RankedPFDetails
                            SELECT 
                                EmployeeId, 
                                EmployeeContribution * (RowNum + 1) AS EmployeeContribution,  
                                EmployerContribution * (RowNum + 1) AS EmployerContribution,  
                                EmployeeProfit, 
                                EmployerProfit
                            FROM RankedPFDetails

                            UNION ALL

                            -- Fourth Source: Profit Distribution New
                            SELECT 
                                EmployeeId, 
                                0 AS EmployeeContribution, 
                                0 AS EmployerContribution, 
                                EmployeeProfitDistribution AS EmployeeProfit, 
                                EmployeerProfitDistribution AS EmployerProfit
                            FROM ProfitDistributionNew
                        ) EmployeeProfits
                        GROUP BY EmployeeId
                    )

                    -- Step 3: Final Join with EmployeeJob
                    SELECT 
                        c.*, 
                        ej.IsNoProfit 
                    FROM 
                        cat c
                    LEFT OUTER JOIN 
                        EmployeeInfo ej 
                        ON ej.EmployeeId = c.EmployeeId;

                    -- Calculate Total
                    WITH RankedPFDetails AS (
                        SELECT 
                            pd.FiscalYearDetailId, 
                            pd.EmployeeId, 
                            pd.EmployeePFValue AS EmployeeContribution, 
                            pd.EmployeerPFValue AS EmployerContribution, 
                            0 AS EmployeeProfit, 
                            0 AS EmployerProfit,
                            ROW_NUMBER() OVER (PARTITION BY pd.EmployeeId ORDER BY pd.FiscalYearDetailId DESC) - 1 AS RowNum
                        FROM 
                            PFDetails pd
                        LEFT OUTER JOIN 
                            FiscalYearDetail fd 
                            ON fd.id = pd.FiscalYearDetailId
                         LEFT OUTER JOIN PFHeader ph on ph.Id=pd.PFHeaderId
                        WHERE 
                            fd.Year = @Year and ph.BranchId=@BranchId
                    )

                    -- Step 2: Aggregate data from multiple sources
                    , cat AS (
                        SELECT 
                            EmployeeId,
                            SUM(EmployeeContribution) AS TotalEmployeeContribution,
                            SUM(EmployerContribution) AS TotalEmployerContribution,
                            SUM(EmployeeProfit) AS TotalEmployeeProfit,
                            SUM(EmployerProfit) AS TotalEmployerProfit,
                            SUM(EmployeeContribution) + 
                            SUM(EmployerContribution) + 
                            SUM(EmployeeProfit) + 
                            SUM(EmployerProfit) AS TotalValue
                        FROM (
                            -- First Source: EmployeePFOpeinig
                            SELECT 
                                EmployeeId, 
                                 EmployeeContribution *12 EmployeeContribution, 
                                EmployerContribution * 12 EmployerContribution, 
                                EmployeeProfit, 
                                EmployerProfit
                            FROM EmployeePFOpeinig

                            UNION ALL

                            -- Second Source: EmployeeBreakMonthPF
                            SELECT 
                                EmployeeId, 
                                EmployeeContribution, 
                                EmployerContribution, 
                                EmployeeProfit, 
                                EmployerProfit
                            FROM EmployeeBreakMonthPF

                            UNION ALL

                            -- Third Source: RankedPFDetails
                            SELECT 
                                EmployeeId, 
                                EmployeeContribution * (RowNum + 1) AS EmployeeContribution,  
                                EmployerContribution * (RowNum + 1) AS EmployerContribution,  
                                EmployeeProfit, 
                                EmployerProfit
                            FROM RankedPFDetails

                            UNION ALL

                            -- Fourth Source: Profit Distribution New
                            SELECT 
                                EmployeeId, 
                                0 AS EmployeeContribution, 
                                0 AS EmployerContribution, 
                                EmployeeProfitDistribution AS EmployeeProfit, 
                                EmployeerProfitDistribution AS EmployerProfit
                            FROM ProfitDistributionNew
                        ) EmployeeProfits
                        GROUP BY EmployeeId
                    )

                    -- Step 3: Final Query with Sum of TotalValue Across All Employees
                    SELECT 
                        SUM(c.TotalEmployeeContribution) AS EmployeeContribution,
                        SUM(c.TotalEmployerContribution) AS EmployerContribution,
                        SUM(c.TotalEmployeeProfit) AS EmployeeProfit,
                        SUM(c.TotalEmployerProfit) AS EmployerProfit,
                        SUM(c.TotalValue) AS TotalValue
                    FROM 
                        cat c
                    LEFT OUTER JOIN 
                        EmployeeInfo ej 
                        ON ej.EmployeeId = c.EmployeeId;
                    ";
                }

                string checkExistData =
                    "Select Count(ID) from ProfitDistributionNew where PreDistributionFundId=@PreDistributionFundId";

                SqlCommand cmd = new SqlCommand(checkExistData, currConn, transaction);
                cmd.Parameters.AddWithValue("@PreDistributionFundId", vm.PreDistributionFundId);
                int count = (int)cmd.ExecuteScalar();

                if (count != 0)
                {
                    throw new Exception("Fund has been already distributed");

                }

                cmd.CommandText = sqlText;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@Year", fvm.Year);
                cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                int nextId = _cDal.NextId("ProfitDistributionNew", currConn, transaction);

                DataSet dtEmployeeDetails = new DataSet();
                adapter.Fill(dtEmployeeDetails);

                PreDistributionFundDAL preDistributionFundDAL = new PreDistributionFundDAL();
                PreDistributionFundVM preDistributionFund = preDistributionFundDAL
                    .SelectAll(Convert.ToInt32(vm.PreDistributionFundId)).FirstOrDefault();

                DataTable dtFinalDistribution = new DataTable();
                DataTable dtFinalDistributionNoProfit = new DataTable();

                dtFinalDistribution.Columns.Add("Id");
                dtFinalDistribution.Columns.Add("EmployeeId");

                dtFinalDistribution.Columns.Add("PreDistributionFundId");//
                dtFinalDistribution.Columns.Add("DistributionDate");//
                dtFinalDistribution.Columns.Add("FiscalYearDetailId");//
                dtFinalDistribution.Columns.Add("EmployeeContribution");//
                dtFinalDistribution.Columns.Add("EmployerContribution");//
                dtFinalDistribution.Columns.Add("EmployeeProfit");//
                dtFinalDistribution.Columns.Add("EmployerProfit");//
                dtFinalDistribution.Columns.Add("MultiplicationFactor");//
                dtFinalDistribution.Columns.Add("EmployeeProfitDistribution");//
                dtFinalDistribution.Columns.Add("EmployeerProfitDistribution");//
                dtFinalDistribution.Columns.Add("TotalProfit");//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "Post", DataType = typeof(bool) });//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "IsPaid", DataType = typeof(bool) });//
                dtFinalDistribution.Columns.Add("Remarks");//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "IsActive", DataType = typeof(bool) });//
                dtFinalDistribution.Columns.Add(new DataColumn() { ColumnName = "IsArchive", DataType = typeof(bool) });//

                //dtFinalDistribution.Columns.Add("IsActive");//
                //dtFinalDistribution.Columns.Add("IsArchive");//
                dtFinalDistribution.Columns.Add("CreatedBy");//
                dtFinalDistribution.Columns.Add("CreatedAt");//
                dtFinalDistribution.Columns.Add("CreatedFrom");//
                dtFinalDistribution.Columns.Add("LastUpdateBy");//
                dtFinalDistribution.Columns.Add("LastUpdateAt");//
                dtFinalDistribution.Columns.Add("LastUpdateFrom");//

                dtFinalDistributionNoProfit = dtFinalDistribution.Copy();

                _settingDal = new SettingDAL();
                string IsProfitCalculation = _settingDal.settingValue("BWWF", "IsProfitCalculation").Trim();

                if (IsWeightedAverageMonth == "N")
                {

                    decimal totalValueSum = 0;
                    if (dtEmployeeDetails.Tables.Count > 0 && dtEmployeeDetails.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dtEmployeeDetails.Tables[0].Rows)
                        {
                            if (row["TotalValue"] != DBNull.Value)
                            {
                                totalValueSum += Convert.ToDecimal(row["TotalValue"]) * 2 * Convert.ToDecimal(row["Month"]);
                            }
                        }
                    }

                    foreach (DataRow dataRow in dtEmployeeDetails.Tables[0].Rows)
                    {

                        decimal EmployeeProfit = 0;
                        decimal EmployerProfit = 0;

                        decimal totalProfit = (Convert.ToDecimal(preDistributionFund.TotalValue) / totalValueSum) * (Convert.ToDecimal(dataRow["TotalValue"]) * Convert.ToDecimal(dataRow["Month"]) * 2);
                        EmployeeProfit = totalProfit / 2;
                        EmployerProfit = totalProfit / 2;

                        decimal multiplicationFactor = Convert.ToDecimal(preDistributionFund.TotalValue) /
                                            Convert.ToDecimal(dtEmployeeDetails.Tables[0].Rows[0]["TotalValue"]);

                        if (IsProfitCalculation == "Y")
                        {
                            dtFinalDistribution.Rows.Add(nextId, dataRow["EmployeeId"], vm.PreDistributionFundId, Ordinary.DateToString(vm.DistributionDate), "0",
                        dataRow["TotalValue"], dataRow["TotalValue"], EmployeeProfit, EmployerProfit,
                        multiplicationFactor, EmployeeProfit, EmployerProfit, totalProfit, true, false, "-", true, false, "", "", "",
                        "", "", "");
                        }
                        else
                        {
                            totalProfit = 0;
                        }


                        nextId++;
                    }
                }
                else
                {
                    foreach (DataRow dataRow in dtEmployeeDetails.Tables[0].Rows)
                    {

                        decimal EmployeeProfit = Convert.ToDecimal(dataRow["EmployeeProfit"].ToString());
                        decimal EmployerProfit = Convert.ToDecimal(dataRow["EmployerProfit"].ToString());

                        decimal totalProfit = Convert.ToDecimal(dataRow["TotalValue"]) * Convert.ToDecimal(preDistributionFund.TotalValue) /
                                                         Convert.ToDecimal(dtEmployeeDetails.Tables[1].Rows[0]["TotalValue"]);
                        EmployeeProfit = totalProfit / 2;
                        EmployerProfit = totalProfit / 2;

                        decimal multiplicationFactor = Convert.ToDecimal(preDistributionFund.TotalValue) /
                                            Convert.ToDecimal(dtEmployeeDetails.Tables[1].Rows[0]["TotalValue"]);

                        if (IsProfitCalculation == "Y")
                        {

                            if (dataRow["IsNoProfit"].ToString() == "True")
                            {
                                dtFinalDistributionNoProfit.Rows.Add(nextId, dataRow["EmployeeId"], vm.PreDistributionFundId, Ordinary.DateToString(vm.DistributionDate), "0",
                                dataRow["EmployeeContribution"], dataRow["EmployerContribution"], dataRow["EmployeeProfit"],
                                dataRow["EmployerProfit"],
                                multiplicationFactor, EmployeeProfit, EmployerProfit, totalProfit, true, false, "-", true, false, "", "", "",
                                "", "", "");

                                totalProfit = 0;
                                EmployeeProfit = 0;
                                EmployerProfit = 0;

                            }
                            else
                            {
                                dtFinalDistribution.Rows.Add(nextId, dataRow["EmployeeId"], vm.PreDistributionFundId, Ordinary.DateToString(vm.DistributionDate), "0",
                            dataRow["EmployeeContribution"], dataRow["EmployerContribution"], EmployeeProfit, EmployerProfit,
                            multiplicationFactor, EmployeeProfit, EmployerProfit, totalProfit, true, false, "-", true, false, "", "", "",
                            "", "", "");
                            }
                        }
                        else
                        {
                            totalProfit = 0;
                        }


                        nextId++;
                    }

                }

                // update fiscal year detail Id

                string[] result = _cDal.BulkInsert("ProfitDistributionNew", dtFinalDistribution, currConn, transaction);
                string[] resultNoProfit = _cDal.BulkInsert("ProfitDistributionNoProfit", dtFinalDistributionNoProfit, currConn, transaction);

                string fiscalYearUpdate = @"
Update ProfitDistributionNew set FiscalYearDetailId = FiscalYearDetail.Id
from   FiscalYearDetail
where Format(cast(DistributionDate as datetime),'MMM-yy') = FiscalYearDetail.PeriodName";

                cmd.CommandText = fiscalYearUpdate;
                cmd.ExecuteNonQuery();

                string PreDistributionFundText = @"update PreDistributionFunds set IsDistribute = 1
                where Id = @Id";
                cmd.CommandText = PreDistributionFundText;
                cmd.Parameters.Add("@Id", vm.PreDistributionFundId);
                cmd.ExecuteNonQuery();


                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                ResultVM resultVm = new ResultVM
                {
                    Status = "Success",
                    Message = "Distribution Successful"
                };

                return resultVm;

            }
            #region catch

            catch (Exception ex)
            {
                throw ex;
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

        }

    }
}