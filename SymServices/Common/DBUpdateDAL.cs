using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SymServices.Common
{
    public class DBUpdateDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        #endregion
        #region DB Migrate

    
        public string[] PF_DBUpdate(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                #region Table Add

                retResults = PF_DatabaseTableChanges(currConn, transaction);

                #endregion

                #region Field Add

                retResults = PF_DBTableFieldAdd("COAGroups", "GroupSL", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "COASL", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "IsRetainedEarning", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "IsNetProfit", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "IsDepreciation", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("COAs", "COAType", "nvarchar(100)", true, currConn, transaction);
             

                retResults = PF_DBTableFieldAdd("GLJournals", "IsYearClosing", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("GLJournalDetails", "IsYearClosing", "bit", true, currConn, transaction);



                ////Type: bit - int - decimal(18, 2) - nvarchar(50) ////true = allow null
                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "IsFixed", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("InvestmentNames", "AitInterest", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("InvestmentAccrued", "AitInterest", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("InvestmentAccrued", "NetInterest", "decimal(18, 2)", true, currConn, transaction);


                retResults = PF_DBTableFieldAdd("PFBankDeposits", "ReferenceNo", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "TransactionMediaId", "nvarchar(200)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "ReferenceNo", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "TransactionMediaId", "nvarchar(200)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Investments", "InvestmentNameId", "int", true, currConn, transaction);



                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "ActualInterestAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "ServiceChargeAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnBankInterests", "ActualInterestAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnBankInterests", "ServiceChargeAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "PFStartDate", "nvarchar(14)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "PFEndDate", "nvarchar(14)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "TotalPayableAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "AlreadyPaidAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFSettlements", "NetPayAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "IsPaid", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IsPaid", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "TransactionType", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("Withdraws", "Post", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnInvestments", "IsBankDeposited", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ReturnOnBankInterests", "IsBankDeposited", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "ReferenceId", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFDetails", "IsBankDeposited", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "TransactionType", "nvarchar(100)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("PFBankDeposits", "Post", "bit", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ForfeitureAccounts", "TotalForfeitValue", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "FiscalYearDetailIdTo", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "TotalExpense", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "AvailableDistributionAmount", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "MultiplicationFactor", "decimal(18, 9)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributions", "TotalWeightedContribution", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "FiscalYearDetailIdTo", "int", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IndividualTotalContribution", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "ServiceLengthMonthWeight", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IndividualWeightedContribution", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "MultiplicationFactor", "decimal(18, 9)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "IndividualProfitValue", "decimal(18, 2)", true, currConn, transaction);
                retResults = PF_DBTableFieldAdd("ProfitDistributionDetails", "ServiceLengthMonth", "decimal(18, 2)", true, currConn, transaction);


                #endregion

                #region Foreign Key

                retResults = PF_DBTableForeignKeyAdd("Investments", "InvestmentNames", "InvestmentNameId", "Id", currConn, transaction);

                retResults = PF_DBTableForeignKeyAdd("PFBankDeposits", "TransactionMedias", "TransactionMediaId", "Id", currConn, transaction);

                retResults = PF_DBTableForeignKeyAdd("Withdraws", "TransactionMedias", "TransactionMediaId", "Id", currConn, transaction);

                #endregion


                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            #region Catch and Finally

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        public string[] PF_DatabaseTableChanges(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

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
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                string sqlText = "";

                #region InvestmentNames
                sqlText = " ";
                sqlText = @"


CREATE TABLE [dbo].[InvestmentNames](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Address] [nvarchar](200) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_InvestmentNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

";

                #endregion
                retResults = NewTableAdd("InvestmentNames", sqlText, currConn, transaction);

                #region TransactionMedias
                sqlText = " ";
                sqlText = @"

CREATE TABLE [dbo].[TransactionMedias](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
 CONSTRAINT [PK_TransactionMedias] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


";

                #endregion
                retResults = NewTableAdd("TransactionMedias", sqlText, currConn, transaction);

                #region EmployeeForFeiture_New
                sqlText = " ";
                sqlText = @"

                    CREATE TABLE [dbo].[EmployeeForFeiture_New](
	                    [Id] [int] NOT NULL,
	                    [EmployeeId] [nvarchar](20) NOT NULL,
	                    [ForFeitureDate] [nvarchar](14) NULL,
	                    [EmployeeContribution] [decimal](18, 2) NULL,
	                    [EmployerContribution] [decimal](18, 2) NULL,
	                    [EmployeeProfit] [decimal](18, 2) NULL,
	                    [EmployerProfit] [decimal](18, 2) NULL,
	                    [Post] [bit] NOT NULL,
	                    [Remarks] [nvarchar](500) NULL,
	                    [IsActive] [bit] NOT NULL,
	                    [IsArchive] [bit] NOT NULL,
	                    [CreatedBy] [nvarchar](20) NOT NULL,
	                    [CreatedAt] [nvarchar](14) NOT NULL,
	                    [CreatedFrom] [nvarchar](50) NOT NULL,
	                    [LastUpdateBy] [nvarchar](20) NULL,
	                    [LastUpdateAt] [nvarchar](14) NULL,
	                    [LastUpdateFrom] [nvarchar](50) NULL
                    ) ON [PRIMARY]

                    ";
                #endregion
                retResults = NewTableAdd("EmployeeForFeiture_New", sqlText, currConn, transaction);

                #region PFLoanDetail
                sqlText = " ";
                sqlText = @"
                        CREATE TABLE [dbo].[PFLoanDetail](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [EmployeeLoanId] [varchar](50) NOT NULL,
	                    [EmployeeId] [nvarchar](20) NOT NULL,
	                    [InstallmentAmount] [decimal](18, 2) NOT NULL,
	                    [InstallmentPaidAmount] [decimal](18, 2) NOT NULL,
	                    [PaymentScheduleDate] [nvarchar](20) NOT NULL,
	                    [PaymentDate] [nvarchar](20) NULL,
	                    [IsHold] [bit] NOT NULL,
	                    [IsManual] [bit] NULL,
	                    [IsPaid] [bit] NOT NULL,
	                    [Remarks] [nvarchar](500) NULL,
	                    [IsActive] [bit] NOT NULL,
	                    [IsArchive] [bit] NOT NULL,
	                    [CreatedBy] [nvarchar](20) NOT NULL,
	                    [CreatedAt] [nvarchar](14) NOT NULL,
	                    [CreatedFrom] [nvarchar](50) NOT NULL,
	                    [LastUpdateBy] [nvarchar](20) NULL,
	                    [LastUpdateAt] [nvarchar](14) NULL,
	                    [LastUpdateFrom] [nvarchar](50) NULL,
	                    [PrincipalAmount] [decimal](18, 3) NOT NULL,
	                    [InterestAmount] [decimal](18, 3) NOT NULL,
	                    [HaveDuplicate] [bit] NULL,
	                    [DuplicateID] [int] NULL
	                    ) ON [PRIMARY]

                    ";
             #endregion
                retResults = NewTableAdd("PFLoanDetail", sqlText, currConn, transaction);

                #region NetProfitYearEnds

               sqlText = " ";
                            sqlText = @"

            CREATE TABLE [dbo].[NetProfitYearEnds](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [TransType] [nvarchar](50) NULL,
	            [Year] [varchar](50) NULL,
	            [YearStart] [varchar](50) NULL,
	            [YearEnd] [varchar](50) NULL,
	            [COAId] [int] NULL,
	            [COAType] [varchar](50) NULL,
	            [TransactionAmount] [decimal](18, 4) NULL,
	            [NetProfit] [decimal](18, 4) NULL,
	            [RetainedEarning] [decimal](18, 4) NULL,
             CONSTRAINT [pk_NetProfitYearEnds] PRIMARY KEY CLUSTERED (	[Id] ASC)
            )
            ";
                  #endregion
                retResults = NewTableAdd("NetProfitYearEnds", sqlText, currConn, transaction);

                #region NetProfitGFYearEnds

                sqlText = " ";
                sqlText = @"
            CREATE TABLE [dbo].[NetProfitGFYearEnds](
	            [Id] [int] IDENTITY(1,1) NOT NULL,
	            [TransType] [nvarchar](50) NULL,
	            [Year] [varchar](50) NULL,
	            [YearStart] [varchar](50) NULL,
	            [YearEnd] [varchar](50) NULL,
	            [COAId] [int] NULL,
	            [COAType] [varchar](50) NULL,
	            [TransactionAmount] [decimal](18, 4) NULL,
	            [NetProfit] [decimal](18, 4) NULL,
	            [RetainedEarning] [decimal](18, 4) NULL,
             CONSTRAINT [pk_NetProfitGFYearEnds] PRIMARY KEY CLUSTERED (	[Id] ASC)
            )

            ";           

                #endregion             
                retResults = NewTableAdd("NetProfitGFYearEnds", sqlText, currConn, transaction);

                #region ProfitDistributionNoProfit

                sqlText = " ";
                sqlText = @"
                   CREATE TABLE [dbo].[ProfitDistributionNoProfit](
	                [Id] [int] NOT NULL,
	                [PreDistributionFundId] [nvarchar](200) NOT NULL,
	                [EmployeeId] [nvarchar](20) NOT NULL,
	                [DistributionDate] [nvarchar](14) NOT NULL,
	                [FiscalYearDetailId] [int] NOT NULL,
	                [EmployeeContribution] [decimal](18, 2) NULL,
	                [EmployerContribution] [decimal](18, 2) NULL,
	                [EmployeeProfit] [decimal](18, 2) NULL,
	                [EmployerProfit] [decimal](18, 2) NULL,
	                [MultiplicationFactor] [decimal](18, 9) NULL,
	                [EmployeeProfitDistribution] [decimal](18, 2) NULL,
	                [EmployeerProfitDistribution] [decimal](18, 2) NULL,
	                [TotalProfit] [decimal](18, 2) NOT NULL,
	                [Post] [bit] NOT NULL,
	                [IsPaid] [bit] NULL,
	                [Remarks] [nvarchar](500) NULL,
	                [IsActive] [bit] NOT NULL,
	                [IsArchive] [bit] NOT NULL,
	                [CreatedBy] [nvarchar](20) NOT NULL,
	                [CreatedAt] [nvarchar](14) NOT NULL,
	                [CreatedFrom] [nvarchar](50) NOT NULL,
	                [LastUpdateBy] [nvarchar](20) NULL,
	                [LastUpdateAt] [nvarchar](14) NULL,
	                [LastUpdateFrom] [nvarchar](50) NULL,
	                [TransactionType] [varchar](100) NULL,
	                [TransType] [varchar](100) NULL
	                ) ON [PRIMARY]
                GO
            ";

                #endregion
                retResults = NewTableAdd("ProfitDistributionNoProfit", sqlText, currConn, transaction);

        
                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                #endregion

                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";

            }

            #region Catch and Finally

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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

        public string[] NewTableAdd(string TableName, string createQuery, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name

            string sqlText = "";
            int transResult = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

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
                #endregion New open connection and transaction
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection();
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrWhiteSpace(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }
                else if (string.IsNullOrWhiteSpace(createQuery))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(2)");

                }

                #endregion Validation

                #region Prefetch

                sqlText = "";

                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";

                sqlText += " BEGIN";
                sqlText += " " + createQuery;
                sqlText += " END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                cmdPrefetch.Transaction = transaction;
                transResult = cmdPrefetch.ExecuteNonQuery();

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }



                #endregion Prefetch

                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";

            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
            }
            #endregion

            return retResults;
        }

        public string[] DBTableForeignKeyAdd(string ForeignTable, string PrimaryTable, string ForeignField, string PrimaryField, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                sqlText = "";
                sqlText += @" 
IF NOT EXISTS (SELECT * 
  FROM sys.foreign_keys 
   WHERE object_id = OBJECT_ID(N'dbo.FK_paramForeignTable_paramPrimaryTable')
   AND parent_object_id = OBJECT_ID(N'dbo.paramPrimaryTable')
)


ALTER TABLE [dbo].[paramForeignTable]  WITH CHECK ADD  CONSTRAINT [FK_paramForeignTable_paramPrimaryTable] FOREIGN KEY([paramForeignField])
REFERENCES [dbo].[paramPrimaryTable] ([paramPrimaryField])

ALTER TABLE [dbo].[paramForeignTable] CHECK CONSTRAINT [FK_paramForeignTable_paramPrimaryTable]


";
                sqlText = Regex.Replace(sqlText, "paramForeignTable", ForeignTable);
                sqlText = Regex.Replace(sqlText, "paramPrimaryTable", PrimaryTable);
                sqlText = Regex.Replace(sqlText, "paramForeignField", ForeignField);
                sqlText = Regex.Replace(sqlText, "paramPrimaryField", PrimaryField);

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] DBTableAdd(string TableName, string FieldName, string DataType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText += " IF  NOT EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";
                sqlText += " BEGIN";
                sqlText += " CREATE TABLE " + TableName + "( " + FieldName + " " + DataType + " null) ";
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " if not exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                if (NullType == true)
                {
                    sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + " NULL DEFAULT 0 ;";
                }
                else
                {
                    sqlText += " ALTER TABLE " + TableName + " ADD " + FieldName + " " + DataType + " NOT NULL DEFAULT 0 ;";
                }
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] DBTableFieldAlter(string TableName, string FieldName, string DataType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                else if (string.IsNullOrEmpty(DataType))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (data type)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " ALTER TABLE " + TableName + " ALTER COLUMN " + FieldName + "   " + DataType + "";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] DBTableFieldRemove(string TableName, string FieldName, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction
                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (objects)");
                }
                else if (string.IsNullOrEmpty(FieldName))
                {
                    throw new ArgumentNullException("DB Migrate", "Unable to alter db by (columns)");
                }
                #endregion Validation
                sqlText = "";
                sqlText += " if exists(select * from sys.columns ";
                sqlText += " where Name = N'" + FieldName + "' and Object_ID = Object_ID(N'" + TableName + "'))   ";
                sqlText += " begin";
                sqlText += " ALTER TABLE " + TableName + " DROP COLUMN " + FieldName;
                sqlText += " END";
                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                retResults[0] = "Success";
                retResults[1] = "DB Migrate Successfully.";
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] TAX_DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction("DeleteToCreditCard"); }
                #endregion open connection and transaction

                retResults = DBTableFieldAdd(TableName, FieldName, DataType, NullType, currConn, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] PF_DBTableFieldAdd(string TableName, string FieldName, string DataType, bool NullType, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            string sqlText = "";
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
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                retResults = DBTableFieldAdd(TableName, FieldName, DataType, NullType, currConn, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public string[] PF_DBTableForeignKeyAdd(string ForeignTable, string PrimaryTable, string ForeignField, string PrimaryField, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "0";// Return Id
            retResults[3] = "sqlText"; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "DBMigration"; //Method Name
            SqlConnection currConn = null;
            SqlTransaction transaction = null; try
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
                if (transaction == null) { transaction = currConn.BeginTransaction(""); }
                #endregion open connection and transaction

                #region Execution

                retResults = DBTableForeignKeyAdd(ForeignTable, PrimaryTable, ForeignField, PrimaryField, currConn, transaction);
                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message; //catch ex
                if (Vtransaction == null) { if (Vtransaction == null) { transaction.Rollback(); } }
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
            return retResults;
        }

        public int NewTableExistCheck(string TableName, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            #region Initializ
            string sqlText = "";
            int transResult = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(1)");

                }

                //else if (string.IsNullOrEmpty(DataType))
                //{
                //    throw new ArgumentNullException("TransactionCode", "Unable to Create ID(3)");

                //}

                #endregion Validation
                #region open connection and transaction
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }

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
                #region Prefetch

                sqlText = "";

                sqlText += " IF  EXISTS (SELECT * FROM sys.objects ";
                sqlText += " WHERE object_id = OBJECT_ID(N'" + TableName + "') AND type in (N'U'))";

                sqlText += " BEGIN Select 1 END";
                sqlText += " else BEGIN Select 0 END";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);

                //cmdPrefetch.ExecuteScalar();
                cmdPrefetch.Transaction = transaction;
                transResult = (int)cmdPrefetch.ExecuteScalar();

                #endregion Prefetch
                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit
            }

            #endregion try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("CommonDAL", "NewTableExistCheck", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("CommonDAL", "NewTableExistCheck", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

            return transResult;
        }


        #endregion
    }
}
