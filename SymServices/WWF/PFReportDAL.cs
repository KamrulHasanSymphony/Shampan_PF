using SymOrdinary;
using SymServices.Common;

using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SymServices.WWF
{
    public class PFReportDAL
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDal = new CommonDAL();
        #endregion
        public DataSet Report(PFReportVM vm)
        {
            #region Variables
            string sqlText = "";
            DataSet ds = new DataSet();
            #endregion
            try
            {
                ds = new DataSet();
                ds = PFBankStatement(vm);


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
            }
            #endregion
            return ds;
        }

        ////==================PFBankStatement=================
        public DataSet PFBankStatement(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
----declare @InvestmentId as int = 1

declare @StartDate as int = 20180101
declare @EndDate as int = 20191231

------------------------------BankStatement_CTE---------------------------------
--------------------------------------------------------------------------------
;WITH BankStatement
AS
(
SELECT BankBranchId
, TransactionType
, bd.DepositDate Date, bd.DepositAmount Dr, 0 Cr, 0 Balance
, Remarks Narration
FROM PFBankDeposits bd
where 1=1 AND bd.Post=1
and bd.DepositDate>=@StartDate and bd.DepositDate<=@EndDate
UNION ALL
SELECT 
'' AccountHead
, TransactionType
, w.WithdrawDate Date, 0 Dr, w.WithdrawAmount Cr, 0 Balance
, w.Remarks Narration
FROM Withdraws w

where 1=1 and w.Post=1
and w.WithdrawDate>=@StartDate and w.WithdrawDate<=@EndDate
)

--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
select * 
INTO #BalanceSheet
from BankStatement
ORDER by Date, Narration

declare @total int = 0
update #BalanceSheet set Balance = @total, @total = @total + (Dr-Cr) 

select 
row_number() over(order by bs.Date) as SL
,bs.TransactionType, bs.Date, bs.Dr, bs.Cr, bs.Balance, bs.Narration
from #BalanceSheet bs
ORDER BY bs.Date



--------------------------------------------------------------------------------
--------------------------------------------------------------------------------
select ISNULL(SUM(Dr),0) Dr, ISNULL(Sum(Cr),0) Cr, (ISNULL(SUM(Dr),0)-ISNULL(Sum(Cr),0)) Balance    from #BalanceSheet bs

drop table #BalanceSheet

";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;



                da.Fill(ds);
                DataTable dt = new DataTable();

                dt = ds.Tables[0];

                dt = Ordinary.DtColumnStringToDate(ds.Tables[0], "Date");

                ds.Tables[0].Clear();

                ds.Tables[0].Merge(dt);

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
            return ds;
        }

        public DataSet InvestmentStatement(PFParameterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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

;WITH InvestmentStatement
AS
(
SELECT InvestmentNameId
, 'Investment' TransactionType
, InvestmentDate TransactionDate, InvestmentValue DebitAmount, 0 CreditAmount, 0.0 BalanceAmount, 0 Gain
, Remarks Narration
FROM Investments inv
where 1=1 
AND inv.Post=1
UNION ALL
SELECT 
inv.InvestmentNameId
, 'ROI' TransactionType
, roi.ROIDate TransactionDate, 0 DebitAmount, roi.ROITotalValue CreditAmount, 0.0 BalanceAmount, roi.TotalInterestValue Gain
, roi.Remarks Narration
FROM ReturnOnInvestments roi
left outer join Investments inv ON roi.InvestmentId=inv.Id
where 1=1 
AND roi.Post=1
)



select ina.Name InvestementName
, ins.TransactionType, ins.TransactionDate, ins.DebitAmount, ins.CreditAmount, ins.BalanceAmount, ins.Gain, ins.Narration
from InvestmentStatement ins
LEFT OUTER JOIN InvestmentNames ina on ins.InvestmentNameId=ina.Id
ORDER BY ina.Name, ins.TransactionDate





";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;


                DataTable dt = new DataTable();

                da.Fill(dt);
                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");

                #region BalanceCalculation

                Ordinary.BalanceCalculationGroup(dt, "InvestementName");



                #endregion

                #region Summary
                DataView view = new DataView(dt);
                DataTable dtSummary = view.ToTable("Summary", false, "InvestementName", "DebitAmount", "CreditAmount", "BalanceAmount", "Gain");

                var varGroup = dtSummary.AsEnumerable()
                .GroupBy(r => Convert.ToString(r["InvestementName"]))
                .Select(g =>
                {
                    var row = dtSummary.NewRow();
                    row["InvestementName"] = g.Key;
                    row["DebitAmount"] = g.Sum(r => Convert.ToDecimal(r["DebitAmount"]));
                    row["CreditAmount"] = g.Sum(r => Convert.ToDecimal(r["CreditAmount"]));
                    row["BalanceAmount"] = g.Sum(r => Convert.ToDecimal(r["BalanceAmount"]));
                    row["Gain"] = g.Sum(r => Convert.ToDecimal(r["Gain"]));
                    return row;
                });

                dtSummary = varGroup.CopyToDataTable();

                foreach (DataRow dr in dtSummary.Rows)
                {
                    dr["BalanceAmount"] = Convert.ToDecimal(dr["DebitAmount"]) - Convert.ToDecimal(dr["CreditAmount"]);
                }

                #endregion

                #region DataSet

                ds.Tables.Add(dt);
                ds.Tables.Add(dtSummary);

                #endregion


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
            return ds;
        }
        
        public DataTable AccountStatement(PFParameterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

------declare @AccountId int =4

select 

GLs.TransactionCode, GLs.TransactionDate, GLs.Remarks, GLs.DebitAmount,GLs.CreditAmount, 0 BalanceAmount 

from GLTransactionDetails GLs
where 1=1 and AccountId=@AccountId
order by TransactionDate

";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@AccountId", vm.AccountId);

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");

                #region BalanceCalculation

                Ordinary.BalanceCalculation(dt);

                #endregion

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

        public DataTable AccountStatementBackup2(PFParameterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

------declare @AccountId int =18


select a.TransactionCode, a.TransactionDate, a.AccHead AccountHead, a.CreditAmount DebitAmount
, a.DebitAmount CreditAmount, 0 BalanceAmount, '' Remarks 
from 
(
select GLs.TransactionCode, GLs.TransactionDate, acc.Name AccHead,GLs.DebitAmount,GLs.CreditAmount 
from GLTransactionDetails GLS 
left outer join Accounts acc on acc.Id=GLS.AccountId
where 1=1 
and  gls.AccountId not in(@AccountId) 
and  gls.TransactionCode 
in ( select distinct TransactionCode from GLTransactionDetails where AccountId=@AccountId and IsSingle=1)

union all

select  GLs.TransactionCode, GLs.TransactionDate, acc.Name AccHead, case when  GLs.IsDr = 1 then a.TransactionAmount else 0 end DrAmount
, case when GLs.IsDr=0 then a.TransactionAmount else 0  end CrAmount 
from GLTransactionDetails GLs 
left outer join Accounts acc on acc.Id=GLS.AccountId
left outer join ( select * from GLTransactionDetails where AccountId=@AccountId and IsSingle=0 ) as a on a.TransactionCode=GLs.TransactionCode
where 1=1 
and GLs.IsSingle=1 
and  GLs.AccountId not in(@AccountId) 
and  GLs.TransactionCode 
in( select distinct TransactionCode from GLTransactionDetails where AccountId=@AccountId and IsSingle=0)


) as a
order by a.TransactionDate

";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@AccountId", vm.AccountId);

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");

                #region BalanceCalculation

                Ordinary.BalanceCalculation(dt);

                #endregion

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

        ////Jan-02-2020
        public DataTable AccountStatementBackup(PFParameterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
declare @IsBank bit=0
--------declare @AccountId int=3

select @IsBank=IsBank from Accounts
WHERE 1=1 AND Id = @AccountId

--------select @AccountId

------------------------------AccountStatement_CTE---------------------------------
--------------------------------------------------------------------------------
;WITH AccountStatement
AS
(

 

select * from 
(
select 'A' SL
,TransactionCode
,TransactionDate
,acc.Name AccountHead
,case when @IsBank=1 then DebitAmount else CreditAmount end DebitAmount
,case when @IsBank=1 then  CreditAmount else  DebitAmount end CreditAmount
, 0 BalanceAmount
, tdl.Remarks 
from GLTransactionDetails tdl
LEFT OUTER JOIN Accounts acc ON acc.Id=tdl.DrAccountIdForCredit
where AccountId=@AccountId and CreditAmount>0

union all

select 'B' SL
,TransactionCode
,TransactionDate
,acc.Name AccountHead
,case when @IsBank=1 then CreditAmount else  DebitAmount end DebitAmount
,case when @IsBank=1 then DebitAmount else  CreditAmount end CreditAmount
, 0 BalanceAmount
, tdl.Remarks 
from GLTransactionDetails tdl
LEFT OUTER JOIN Accounts acc ON acc.Id=tdl.AccountId
where TransactionCode in(
select distinct TransactionCode from GLTransactionDetails
where AccountId=@AccountId and DebitAmount>0)
and CreditAmount>0

 
) as a

----order by case when @IsBank=1 then  sl else sl end desc

)


--------------------------------------------------------------------------------
--------------------------------------------------------------------------------

select 
row_number() over(order by ast.TransactionDate) as SL
, ast.TransactionDate, ast.AccountHead, ast.Remarks, ast.DebitAmount, ast.CreditAmount, ast.BalanceAmount
from AccountStatement ast
ORDER BY ast.TransactionDate





";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@AccountId", vm.AccountId);

                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");

                #region BalanceCalculation

                Ordinary.BalanceCalculation(dt);

                #endregion

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
        
        public DataTable AccountLedgerReport(PFParameterVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                sqlText = @"------declare @AccountId int = 3
------declare @DateFrom varchar(14) = '20170101'
------declare @DateTo varchar(14) = '20171230'



select 
TransactionDate, TransactionCode, DebitAmount, CreditAmount, Remarks 


from GLTransactionDetails
where 1=1 and AccountId=@AccountId

";
                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    sqlText = sqlText + "  and TransactionDate>=@DateFrom";
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    sqlText = sqlText + "  and TransactionDate <=@DateTo";
                }

                sqlText = sqlText + " order by TransactionDate";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                da.SelectCommand.Parameters.AddWithValue("@AccountId", vm.AccountId);

                if (!string.IsNullOrWhiteSpace(vm.DateFrom))
                {
                    da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom));
                }

                if (!string.IsNullOrWhiteSpace(vm.DateTo))
                {
                    da.SelectCommand.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo));
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

        public DataTable EmployeeLoanClosed(string rType, string ToDate, string FromDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region sqlText

                sqlText += @"
SELECT ve.Code,ve.EmpName,
      [PrincipalAmount]     
      ,[InterestPolicy]
      ,[InterestRate]
      ,[InterestAmount]
      ,[TotalAmount]
      ,[NumberOfInstallment]    
      ,[StartDate]
      ,[EndDate]     
      ,[IsEarlySellte]
      ,[EarlySellteDate]
      ,[EarlySelltePrincipleAmount]
      ,[EarlySellteInterestAmount]
      ,[LoanNo]
  FROM [EmployeeLoan] el 
  Left Outer Join [ViewEmployeeInformation] ve on ve.EmployeeId=el.EmployeeId
  where el.IsArchive=1 and IsEarlySellte=0 and EarlySellteDate between CONVERT(DATETIME, @DateFrom, 112) and CONVERT(DATETIME, @ToDate, 112)
";
                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(FromDate));
                da.SelectCommand.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(ToDate));
                
                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "EarlySellteDate");
                dt = Ordinary.DtColumnStringToDate(dt, "StartDate");
                dt = Ordinary.DtColumnStringToDate(dt, "EndDate");

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

        public DataTable EmployeeLoanSettelment(string rType, string ToDate, string FromDate,string BranchId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region sqlText

                sqlText += @"
SELECT ve.Code,ve.EmpName,
      [PrincipalAmount]     
      ,[InterestPolicy]
      ,[InterestRate]
      ,[InterestAmount]
      ,[TotalAmount]
      ,[NumberOfInstallment]    
      ,[StartDate]
      ,[EndDate]     
      ,[IsEarlySellte]
      ,[EarlySellteDate]
      ,[EarlySelltePrincipleAmount]
      ,[EarlySellteInterestAmount]
      ,[LoanNo]
  FROM [EmployeeLoan] el 
  Left Outer Join [ViewEmployeeInformation] ve on ve.EmployeeId=el.EmployeeId
  where IsEarlySellte=1 and  EarlySellteDate between CONVERT(DATETIME, @DateFrom, 112) and CONVERT(DATETIME, @ToDate, 112) and el.BranchId=@BranchId
";
                #endregion
          
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;              
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(FromDate));
                da.SelectCommand.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(ToDate));
                da.SelectCommand.Parameters.AddWithValue("@BranchId", BranchId);


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "EarlySellteDate");
                dt = Ordinary.DtColumnStringToDate(dt, "StartDate");
                dt = Ordinary.DtColumnStringToDate(dt, "EndDate");
               
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

        public DataTable EmployeePFStatements(string rType, string EmployeeId, string ToDate, string FromDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region sqlText

                sqlText += @"

select * into #TempEmployeePF 


from 
(
select fyd.PeriodEnd TransactionDate, pfd.EmployeeId, pfd.EmployeePFValue EmployeeContribution, pfd.EmployeerPFValue EmployeerContribution
, 0 ProfitAmount,0 LoanAmount, 0 PaymentAmount, 'Contribution' TransactionType
from PFDetails pfd
left outer join HRMDB.dbo.FiscalYearDetail fyd on pfd.FiscalYearDetailId=fyd.Id
where 1=1 and EmployeeId=@EmployeeId and fyd.PeriodEnd <=@ToDate

union all
select pfd.DistributionDate, pfdd.EmployeeId, 0 EmployeeContribution, 0 EmployeerContribution
, pfdd.IndividualProfitValue ProfitAmount,0 LoanAmount, 0 PaymentAmount, 'Profit' TransactionType 
from ProfitDistributionDetails pfdd
left outer join ProfitDistributions pfd on pfdd.ProfitDistributionId=pfd.Id
where 1=1 and pfdd.EmployeeId=@EmployeeId and pfd.Post=1 and pfd.IsPaid=1 and pfd.DistributionDate <= @ToDate

union all
select StartDate, EmployeeId, 0 EmployeeContribution, 0 EmployeerContribution
, 0 ProfitAmount,TotalAmount LoanAmount, 0 PaymentAmount, 'Loan' TransactionType 
from HRMDB.dbo.EmployeeLoan loan
left outer join HRMDB.dbo.EnumLoanType elt on elt.id=loan.LoanType_E
where 1=1 and loan.EmployeeId=@EmployeeId and loan.IsApproved=1
and elt.Name = 'PF Loan' and loan.StartDate <= @ToDate

union all
select fyd.PeriodEnd, EmployeeId, 0 EmployeeContribution, 0 EmployeerContribution
, 0 ProfitAmount,0 LoanAmount, (LoanAmount+InterestAmount) PaymentAmount, 'Loan Payment' TransactionType 
from HRMDB.dbo.SalaryLoanDetail loan
left outer join HRMDB.dbo.EnumLoanType elt on elt.id=loan.LoanType_E
left outer join HRMDB.dbo.FiscalYearDetail fyd on loan.FiscalYearDetailId=fyd.Id
where 1=1 and loan.EmployeeId=@EmployeeId
and elt.Name = 'PF Loan' and fyd.PeriodEnd <= @ToDate
) as empPF
order by empPF.TransactionDate


";
                #endregion

                #region Summary

                if (rType == "Summary")
                {
                    sqlText += @" 

select 
 ve.EmpName EmployeeName
,ve.Code
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.Other1 OtherInfo
,ve.JoinDate

,empPF.TransactionDate
,empPF.EmployeeContribution
,empPF.EmployeerContribution
,empPF.ProfitAmount
,empPF.LoanAmount
,empPF.PaymentAmount
, 0 Balance
,empPF.TransactionType
from (

select 
EmployeeId
, @DateFrom TransactionDate
,sum(EmployeeContribution	 ) EmployeeContribution	
,sum(EmployeerContribution )   EmployeerContribution
,sum(ProfitAmount			 ) ProfitAmount			
,sum(LoanAmount			 )     LoanAmount			
,sum(PaymentAmount		 )     PaymentAmount	
, 0 Balance
, 'Opening' TransactionType
from #TempEmployeePF
where 1=1 and TransactionDate < @DateFrom 
group by EmployeeId

union all



select 
EmployeeId
, @ToDate TransactionDate
,sum(EmployeeContribution)EmployeeContribution
,sum(EmployeerContribution) EmployeerContribution
,sum(ProfitAmount) ProfitAmount
,sum(LoanAmount) LoanAmount
,sum(PaymentAmount) PaymentAmount
,0 Balance
,'Transactions'TransactionType

from #TempEmployeePF  where 1=1 and TransactionDate >= @DateFrom and  TransactionDate <= @ToDate  

group by EmployeeId

)as empPF

left outer join HRMDB.ViewEmployeeInformation ve on empPF.EmployeeId=ve.EmployeeId

";
                }
                #endregion


                #region Detail

                else
                {
                    sqlText += @" 

select 
 ve.EmpName EmployeeName
,ve.Code
,ve.Designation
,ve.Department
,ve.Section
,ve.Project
,ve.Other1 OtherInfo
,ve.JoinDate

,empPF.TransactionDate
,empPF.EmployeeContribution
,empPF.EmployeerContribution
,empPF.ProfitAmount
,empPF.LoanAmount
,empPF.PaymentAmount
, 0 Balance
,empPF.TransactionType
from 
(
select 
EmployeeId
, @DateFrom TransactionDate
,sum(EmployeeContribution	 ) EmployeeContribution	
,sum(EmployeerContribution )   EmployeerContribution
,sum(ProfitAmount			 ) ProfitAmount			
,sum(LoanAmount			 )     LoanAmount			
,sum(PaymentAmount		 )     PaymentAmount	
, 0 Balance
, 'Opening' TransactionType
from #TempEmployeePF
where 1=1 and TransactionDate < @DateFrom 
group by EmployeeId

union all

select 
EmployeeId
,TransactionDate
,EmployeeContribution
,EmployeerContribution
,ProfitAmount
,LoanAmount
,PaymentAmount
, 0 Balance
, TransactionType
from #TempEmployeePF
where 1=1 and TransactionDate >= @DateFrom and  TransactionDate <= @ToDate 

) as empPF
left outer join HRMDB.ViewEmployeeInformation ve on empPF.EmployeeId=ve.EmployeeId

";
                }
                #endregion

                sqlText += @" drop table #TempEmployeePF";

                

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(FromDate));
                da.SelectCommand.Parameters.AddWithValue("@ToDate", Ordinary.DateToString(ToDate));


                da.Fill(dt);

                dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");
                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");

                #region BalanceAmount

                decimal BalanceAmount = 0;
                decimal EmployeeContribution = 0;
                decimal EmployeerContribution = 0;
                decimal ProfitAmount = 0;
                decimal LoanAmount = 0;
                decimal PaymentAmount = 0;

                foreach (DataRow dr in dt.Rows)
                {

                    decimal DebitAmount = 0;
                    decimal CreditAmount = 0;
                    EmployeeContribution = Convert.ToDecimal(dr["EmployeeContribution"]);
                    EmployeerContribution = Convert.ToDecimal(dr["EmployeerContribution"]);
                    ProfitAmount = Convert.ToDecimal(dr["ProfitAmount"]);
                    LoanAmount = Convert.ToDecimal(dr["LoanAmount"]);
                    PaymentAmount = Convert.ToDecimal(dr["PaymentAmount"]);

                    DebitAmount = EmployeeContribution + EmployeerContribution + ProfitAmount + PaymentAmount;

                    CreditAmount = LoanAmount;

                    BalanceAmount = BalanceAmount + (DebitAmount - CreditAmount);
                    dr["Balance"] = BalanceAmount;

                }

                #endregion


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

        public DataTable EmployeePFSettlementReport(string EmployeeId, string ToDate, string FromDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region sqlText

                sqlText += @"
SELECT 
    ve.Code,
    ve.EmpName,
    ve.Department,
    ve.Designation,
    ve.Section,
    ve.Project,
    FORMAT(CAST(ve.JoinDate AS DATE), 'dd-MMM-yyyy') JoinDate,
    ps.NetPayAmount,
    ps.EmployerTotalContribution,
    ps.EmployeeTotalContribution,
    ps.EmployeeProfitValue,
    ps.EmployerProfitValue,
    SUM(ln.PrincipalAmount) AS PrincipalAmount,
    SUM(ln.InterestAmount) AS InterestAmount,
    SUM(CASE WHEN ln.IsPaid = 1 THEN ln.PrincipalAmount ELSE 0 END) AS PrincipalAmountPaid,
    SUM(CASE WHEN ln.IsPaid = 1 THEN ln.InterestAmount ELSE 0 END) AS InterestAmountPaid

FROM 
    PFSettlements ps
LEFT OUTER JOIN ViewEmployeeInformation ve 
    ON ve.EmployeeId = ps.EmployeeId
LEFT OUTER JOIN EmployeeLoanDetail ln 
    ON ln.EmployeeId = ve.EmployeeId
WHERE 
    ve.Code = @EmployeeId
GROUP BY
    ve.Code,
    ve.EmpName,
    ve.Department,
    ve.Designation,
    ve.Section,
    ve.Project,
    ve.JoinDate,
    ps.NetPayAmount,
    ps.EmployerTotalContribution,
    ps.EmployeeTotalContribution,
    ps.EmployeeProfitValue,
    ps.EmployerProfitValue;

                ";
                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                             
                da.Fill(dt);
                             
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
        public DataTable EmployeePFSettlementPayment(string Code, string ToDate, string FromDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            #endregion
            try
            {
                EmployeeInfoDAL edal = new EmployeeInfoDAL();
                string EmployeeId = edal.SelectEmpByCode(Code);

                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                
                #region sql statement

                #region sqlText

                sqlText += @"
                       SELECT 
                        ISNULL(SUM([EmployeeContribution])+
                        SUM([EmployerContribution])+
                        SUM([EmployeeProfit])+
                        SUM([EmployerProfit]),0) as Payment
                        FROM EmployeePFPayment where EmployeeId=@EmployeeId
                    ";
                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                da.Fill(dt);

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
        public DataTable PFInvestmentReportSyn(string Year, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region sqlText

                sqlText += @"
SELECT ivn.Id,ivn.Code,ivn.InvestmentDate,
ivn.Name as InvestmentName,
      fy.PeriodName Fyscalyear
      ,[TransactionDate]
      ,[ReferenceNo]
      ,[InvestmentValue]
      ,[AccruedMonth]
      ,[InterestRate]
      ,[AccruedInterest] 
      ,ia.AitInterest
      ,[NetInterest]
  FROM InvestmentAccrued ia
  Left Outer Join InvestmentNames ivn on ivn.Id=ia.InvestmentNameId
 Left Join  [FiscalYearDetail] fy on fy.Id=ia.FiscalYearDetailId
where InvestmentTypeId=3
 order by ivn.Id";
                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;           
                da.Fill(dt);

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

        public DataTable PFInvestmentReport(string Year, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                #region sqlText

                sqlText += @"

  Select  ivn.Name as InvestmentName,CONCAT('FY ', fy.Year-1, '-', fy.Year) as FyscalYear,
 FORMAT(CONVERT(DATE, iv.InvestmentDate, 112), 'dd-MM-yyyy') InvestmentDate,
 FORMAT(CONVERT(DATE, iv.MaturityDate, 112), 'dd-MM-yyyy') MaturityDate,
 iv.InvestmentValue,
  DATEDIFF(day, iv.InvestmentDate,iv.MaturityDate) Period,
  DATEDIFF(day, iv.InvestmentDate, DATEFROMPARTS(YEAR(iv.InvestmentDate), 6, 30)) PeriodJune30,  
  iv.InvestmentRate,ISNULL(iv.IsEncashed,0) IsEncashed, 0 as ExisDuty  
  from Investments iv 
  Left Join InvestmentNames ivn on ivn.Id=iv.InvestmentNameId
  Left Join  [FiscalYearDetail] fy on fy.Id=ivn.FiscalYearDetailId
  where iv.InvestmentTypeId=1 and fy.Year=@Year

";
                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@Year", Year);
           
                da.Fill(dt);


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
        public DataTable InvestmentDetailsReport(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
--declare @id varchar(100)='6'
 select * from (
select  
i.Id InvestmentId
,invn.Id InvestmentNameId
,invn.Name InvestmentName ,invt.Name InvestmentTypes 
,invn.Code 
,i.ReferenceNo
,i.InvestmentAddress
,i.InvestmentDate
,i.FromDate
,i.ToDate
,i.MaturityDate
,i.InvestmentRate
,i.InvestmentValue
,i.BankCharge
,b.Name as BankName
,br.BranchName
,Round(DATEDIFF(MONTH, i.FromDate, i.ToDate),0) AS Month
,(((i.InvestmentValue*i.InvestmentRate)/100)/12)*Round(DATEDIFF(MONTH, i.FromDate, i.ToDate),0) Interest
,0 BankExciseDuty, 'New' TransactionType
 from Investments i
left outer join InvestmentNames invn on i.InvestmentNameId=invn.Id
left outer join EnumInvestmentTypes invt on i.InvestmentTypeId=invt.Id
Left Outer Join BankNames b on b.Id=invn.BankNameId
Left Outer Join BankBranchs br on br.Id=invn.BankBranchId
where i.id=@id
And i.TransType=@TransType


union all
select    
i.InvestmentId InvestmentId
,0 InvestmentNameId
,'-'InvestmentName ,'-' InvestmentTypes
 ,i.TransactionCode Code
,i.ReferenceNo
,'-' InvestmentAddress
,i.InvestmentDate
,i.FromDate
,i.ToDate
,i.MaturityDate
,i.InterestRate InvestmentRate
,i.InvestmentValue,BankCharge
,'-' BankName
,'-' BranchName
,Round(DATEDIFF(MONTH, i.FromDate, i.ToDate),0) AS Month
,Interest,BankExciseDuty,case when IsEncashed=1 then 'Encashment' else 'Renew' end TransactionType
 from InvestmentRenew i
where i.InvestmentId=@id
And i.TransType=@TransType

) as a 
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
                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@id", vm.Id);
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);
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
                dt = Ordinary.DtColumnStringToDate(dt, "InvestmentDate");
                dt = Ordinary.DtColumnStringToDate(dt, "FromDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ToDate");
                dt = Ordinary.DtColumnStringToDate(dt, "MaturityDate");

                

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
            return dt;
        }

        public DataTable InvestmentNameDetailsReport(string InvestmentNameId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
                            Select * from InvestmentNameDetails where InvestmentNameId=@InvestmentNameId ";
                                
                #endregion SqlText
                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@InvestmentNameId", InvestmentNameId);
                            
                da.Fill(dt);
               
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
            return dt;
        }
        
        public DataTable InvestmentHeaderReport(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select invn.Name InvestmentName ,invt.Name InvestmentTypes,
i.Id
,i.TransactionCode
,i.InvestmentTypeId
,i.ReferenceNo
,i.InvestmentAddress
,i.InvestmentDate
,i.FromDate
,i.ToDate
,i.MaturityDate
,i.InvestmentRate
,i.InvestmentValue
,i.Remarks
 from Investments i
left outer join InvestmentNames invn on i.InvestmentNameId=invn.Id
left outer join EnumInvestmentTypes invt on i.InvestmentTypeId=invt.Id
 
WHERE  1=1   

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
                #endregion SqlText
                #region SqlExecution

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
                dt = Ordinary.DtColumnStringToDate(dt, "InvestmentDate");
                dt = Ordinary.DtColumnStringToDate(dt, "FromDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ToDate");
                dt = Ordinary.DtColumnStringToDate(dt, "MaturityDate");

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
            return dt;
        }
        
        public DataTable PFContributionReport(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select distinct pfh.Code, pf.FiscalYearDetailId,f.PeriodName, pf.ProjectId,p.Name ProjectName,pf.Remarks
,sum(pf.EmployeePFValue)EmployeeContribution
,sum(pf.EmployeerPFValue)MemberContribution
from PFDetails pf

left outer join PFHeader pfh on pfh.Id=pf.PFHeaderId
left outer join Project p on pf.ProjectId=p.Id
left outer join FiscalYearDetail f on pf.FiscalYearDetailId=f.Id
 
WHERE  1=1 
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
                #endregion SqlText
                #region SqlExecution
                sqlText += @" group by pfh.Code,pf.FiscalYearDetailId,f.PeriodName, pf.ProjectId,p.Name,pf.Remarks";
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
            return dt;
        }

        public DataTable InvestmentEncashment(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
--declare @id varchar(100)='6'
 select
 i.TransactionCode,i.InvestmentDate
 ,i.FromDate  ,irenew.ToDate
 ,irenew.MaturityDate
 ,it.Name TransactionType
 
 ,inn.Name InvesementName,i.ReferenceNo
 ,a.BankPortion,a.InvestmentValue ,a.Interest,a.BankCharge 
 from(
select distinct InvestmentId,sum(Bank)BankPortion,sum(InvestmentValue)InvestmentValue,sum(Interest)Interest,sum( BankCharge) BankCharge  from (
--select * from(
select Id InvestmentId ,InvestmentValue Bank,InvestmentValue , 0 Interest,0 BankCharge from Investments
where id=@id
And TransType=@TransType
and Post=1
union all
select  InvestmentId Id ,InvestmentRenew.InvestmentValue Bank,InvestmentRenew.InvestmentValue,0 Interest,0 BankCharge from InvestmentRenew
where InvestmentId=@id
And TransType=@TransType
and InvestmentRenew.IsEncashed=0
and InvestmentRenew.Post=1
union all
select  InvestmentId Id ,InvestmentRenew.InvestmentValue Bank,0 InvestmentValue,InvestmentRenew.Interest,InvestmentRenew.BankCharge from InvestmentRenew
where InvestmentId=@id
And TransType=@TransType

and InvestmentRenew.IsEncashed=1
and InvestmentRenew.Post=1
) as a
group by InvestmentId
) as a
left outer join Investments i on a.InvestmentId=i.Id
left outer join InvestmentNames inn on i.InvestmentNameId =inn.Id
left outer join EnumInvestmentTypes it on i.InvestmentTypeId=it.Id
left outer join InvestmentRenew irenew on i.id=irenew.InvestmentId and irenew.IsEncashed=1

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
                #endregion SqlText

                #region SqlExecution



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@id", vm.Id);
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);

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

                dt = Ordinary.DtColumnStringToDate(dt, "InvestmentDate");
                dt = Ordinary.DtColumnStringToDate(dt, "FromDate");
                dt = Ordinary.DtColumnStringToDate(dt, "ToDate");
                dt = Ordinary.DtColumnStringToDate(dt, "MaturityDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataSet InvestmentNameReport(int Id, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {

            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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
 
select i.Id InvestmentNamId
,i.Code
,i.Name
,i.Remarks
,it.Name InvestmentType
,i.InvestmentDate
,i.FromDate
,i.ToDate
,i.MaturityDate
,br.BranchName
,bn.Name BankName
,fd.PeriodName 
,i.AitInterest
 from InvestmentNames i
left outer join FiscalYearDetail fd on i.FiscalYearDetailId=fd.Id
left outer join EnumInvestmentTypes it on i.InvestmentTypeId=it.Id
 left outer join BankBranchs br on i.BankBranchId=br.Id
 left outer join BankNames bn on i.BankNameId=bn.Id

where i.Id=@InvestmentNameId

 select  
ia.InvestmentNameId
,fd.PeriodName 
,ia.TransactionDate
,ia.ReferenceNo
,ia.InvestmentValue
,ia.AccruedMonth
,ia.InterestRate
,ia.AccruedInterest
,ia.AitInterest
,ia.NetInterest
 from InvestmentAccrued ia
left outer join FiscalYearDetail fd on ia.FiscalYearDetailId=fd.Id
where ia.InvestmentNameId=@InvestmentNameId



 select  
InvestmentNameId
,FromMonth
,ToMonth
,InterestRate
,Remarks
 from InvestmentNameDetails
where InvestmentNameDetails.InvestmentNameId=@InvestmentNameId

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
                #endregion SqlText

                #region SqlExecution



                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@id", Id);
                da.SelectCommand.Parameters.AddWithValue("@InvestmentNameId", Id);

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

                da.Fill(ds);
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                dt = ds.Tables[0];
                dt1 = ds.Tables[1];

                dt = Ordinary.DtColumnStringToDate(ds.Tables[0], "InvestmentDate");
                dt = Ordinary.DtColumnStringToDate(ds.Tables[0], "FromDate");
                dt = Ordinary.DtColumnStringToDate(ds.Tables[0], "ToDate");
                dt = Ordinary.DtColumnStringToDate(ds.Tables[0], "MaturityDate");
                dt1 = Ordinary.DtColumnStringToDate(ds.Tables[1], "TransactionDate");

                //ds.Tables[0].Clear();
                //ds.Tables[1].Clear();

                ds.Tables[0].Merge(dt);
                ds.Tables[1].Merge(dt1);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }

        #region PF Report (25 Dec 2021)

        public DataTable BankCharge_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select bd.Code, bd.TransactionDate,bd.TotalValue,bd.Remarks
 from BankCharge bd

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by TransactionDate";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeeBreakMonthPF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,OpeningDate
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit
from EmployeeBreakMonthPF pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by e.Project,e.Department,e.Section,e.Code";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeeBreakMonthGF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
 
select 
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,OpeningDate
,EmployerContribution
,EmployerProfit

from GFEmployeeBreakMonth pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,EmployeeCode";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeePaymentGF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 

select distinct
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,PaymentDate
,EmployerContribution
,EmployerProfit

from GFEmployeePayment pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,EmployeeCode";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeePaymentPF_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 

select distinct
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,PaymentDate
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit
from EmployeePFPayment pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by   Project,Department,Section,Code";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");
                dt = Ordinary.DtColumnStringToDate(dt, "PaymentDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }
        public DataTable EmployeeForfeiture_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project

,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,OpeningDate
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit
from EmployeeForfeiture pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                        //sqlText += " AND " + conditionFields[i] + "=@" + cField;
                        sqlText += " AND CAST(" + conditionFields[i] + " AS NVARCHAR(50)) = @" + cField;

                    }
                }
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,Code";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable GFEmployeeForfeiture_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
 
select 
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,OpeningDate
,EmployerContribution
,EmployerProfit

from GFEmployeeForfeiture pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @"      order by Project,Department,Section,EmployeeCode";


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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeePFOpeinig_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project

,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,OpeningDate
,EmployeeContribution
,EmployerContribution
,EmployeeProfit
,EmployerProfit
from EmployeePFOpeinig pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,Code";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeeGFOpeinig_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select  
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,OpeningDate
,EmployerContribution
,EmployerProfit

from GFEmployeeOpeinig pfo
left outer join ViewEmployeeInformation e on pfo.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pfo.EmployeeId=eold.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,EmployeeCode";

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

                dt = Ordinary.DtColumnStringToDate(dt, "OpeningDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable EmployeeStatement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project

,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,pf.TransactionDate	
,pf.EmployeeContribution	
,pf.EmployerContribution	
,pf.EmployeeProfit	
,pf.EmployerProfit	
,pf.Total
, SUM(pf.Balance) OVER (partition BY e.Code
ORDER BY TransactionDate,SL ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Balance

,pf.TransType
,pf.Remarks
from (
select 'A'SL, OpeningDate TransactionDate,EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
,'Opening'TransType
,'Opening'Remarks
,isnull(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)+ isnull(EmployeeProfit,0)+isnull(EmployerProfit,0),0) Total
,isnull(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)+ isnull(EmployeeProfit,0)+isnull(EmployerProfit,0),0) Balance

from EmployeePFOpeinig
union all 
select 'B'SL, OpeningDate TransactionDate,EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit,EmployerProfit
,'BreakMonth'TransType
,isnull( EmployeeBreakMonthPF.Remarks,'Break Month')Remarks

,isnull(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)+ isnull(EmployeeProfit,0)+isnull(EmployerProfit,0),0) Total
,isnull(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)+ isnull(EmployeeProfit,0)+isnull(EmployerProfit,0),0) Balance
from EmployeeBreakMonthPF
union all 
select   'C'SL, fd.PeriodStart TransactionDate ,EmployeeId, EmployeePFValue EmployeeContribution, EmployeerPFValue,0 EmployeeProfit, 0 EmployerProfit
,'MonthlyContribution'TransType
,isnull( PFDetails.Remarks,'Monthly Contribution')Remarks

,isnull(isnull(EmployeePFValue,0)+ isnull(EmployeerPFValue,0),0) Total
,isnull(isnull(EmployeePFValue,0)+ isnull(EmployeerPFValue,0),0) Balance

from PFDetails
left outer join FiscalYearDetail fd on PFDetails.FiscalYearDetailId=fd.Id
union all 
select   'C'SL, DistributionDate TransactionDate ,EmployeeId, 0 EmployeeContribution, 0 EmployerContribution,EmployeeProfitDistribution EmployeeProfit,EmployeerProfitDistribution EmployerProfit
,'ProfitDistribution'TransType
,isnull( ProfitDistributionNew.Remarks,'Profit Distribution')Remarks

,isnull(isnull(EmployeeProfitDistribution,0)+ isnull(EmployeerProfitDistribution,0),0) Total
,isnull(isnull(EmployeeProfitDistribution,0)+ isnull(EmployeerProfitDistribution,0),0) Balance
from ProfitDistributionNew

union all 
select 'B'SL, PaymentDate TransactionDate,EmployeeId, -1*EmployeeContribution,-1* EmployerContribution, -1*EmployeeProfit,-1*EmployerProfit
,'Payment'TransType
,isnull( EmployeePFPayment.Remarks,'Payment')Remarks

,-1*isnull(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)+ isnull(EmployeeProfit,0)+isnull(EmployerProfit,0),0) Total
,-1*(isnull(isnull(EmployeeContribution,0)+ isnull(EmployerContribution,0)+ isnull(EmployeeProfit,0)+isnull(EmployerProfit,0),0)) Balance

from EmployeePFPayment

) as pf
left outer join ViewEmployeeInformation e on pf.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pf.EmployeeId=eold.EmployeeId


WHERE  1=1   
and Total<>0


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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,EmployeeCode,TransactionDate,pf.sl";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable PFEmployeeLedger(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
---declare @DateFrom   varchar(100)='20210702'
---declare @DatTo varchar(100)='20220701'
---declare @Employeeid varchar(100)='20220701'

select distinct pf.Employeeid
,CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation

,CASE
    WHEN e.JoinDate is null THEN eOld.JoinDate
    WHEN e.JoinDate = 'NA' THEN eOld.JoinDate
    ELSE e.JoinDate
END JoinDate
,isnull(OP.OP_EmployeeContribution	,0)OP_EmployeeContribution
,isnull(OP.OP_EmployerContribution	,0)OP_EmployerContribution
,isnull(OP.OP_EmployeeProfit		,0)OP_EmployeeProfit
,isnull(OP.OP_EmployerProfit		,0)OP_EmployerProfit
,isnull(MC.MC_EmployeeContribution	,0)MC_EmployeeContribution
,isnull(MC.MC_EmployerContribution	,0)MC_EmployerContribution
,isnull(pd.PD_EmployeeProfit		,0)PD_EmployeeProfit
,isnull(pd.PD_EmployerProfit		,0)PD_EmployerProfit
,isnull(PY.PY_EmployeeContribution	,0)PY_EmployeeContribution
,isnull(PY.PY_EmployerContribution	,0)PY_EmployerContribution
,isnull(PY.PY_EmployeeProfit		,0)PY_EmployeeProfit
,isnull(PY.PY_EmployerProfit		,0)PY_EmployerProfit

,isnull(OP.OP_EmployeeContribution	,0)
+isnull(MC.MC_EmployeeContribution	,0)
+isnull(-1*PY.PY_EmployeeContribution	,0) CL_EmployeeContribution


,isnull(OP.OP_EmployerContribution	,0)
+isnull(MC.MC_EmployerContribution	,0)
+isnull(-1*PY.PY_EmployerContribution	,0)CL_EmployerContribution

,isnull(OP.OP_EmployeeProfit		,0)
+isnull(pd.PD_EmployeeProfit		,0)
+isnull(-1*PY.PY_EmployeeProfit		,0)CL_EmployeeProfit


,isnull(OP.OP_EmployerProfit		,0)
+isnull(pd.PD_EmployerProfit		,0)
+isnull(-1*PY.PY_EmployerProfit		,0)CL_EmployerProfit

,isnull(OP.OP_EmployeeContribution	,0)
+isnull(OP.OP_EmployerContribution	,0)
+isnull(OP.OP_EmployeeProfit		,0)
+isnull(OP.OP_EmployerProfit		,0)
+isnull(MC.MC_EmployeeContribution	,0)
+isnull(MC.MC_EmployerContribution	,0)
+isnull(pd.PD_EmployeeProfit		,0)
+isnull(pd.PD_EmployerProfit		,0)
+isnull(-1*PY.PY_EmployeeContribution	,0)
+isnull(-1*PY.PY_EmployerContribution	,0)
+isnull(-1*PY.PY_EmployeeProfit		,0)
+isnull(-1*PY.PY_EmployerProfit		,0)Total
,isnull(loan.OutStandingLoanAmount,0) Loan

from ViewEmployeeStatementPF PF
left outer join 
(select distinct EmployeeId,SUM(EmployeeContribution)OP_EmployeeContribution,SUM(EmployerContribution)OP_EmployerContribution,SUM(EmployeeProfit)OP_EmployeeProfit,SUM(EmployerProfit)OP_EmployerProfit
,'Opening'TransType
  from ViewEmployeeStatementPF
where TransactionDate<@DateFrom
group by EmployeeId) OP on pf.EmployeeId=op.EmployeeId
left outer join 
(
select distinct EmployeeId,SUM(EmployeeContribution)MC_EmployeeContribution,SUM(EmployerContribution)MC_EmployerContribution
,'MonthlyContribution'TransType
  from ViewEmployeeStatementPF
where TransType in('MonthlyContribution' ,'BreakMonth' )
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId) MC on pf.EmployeeId=MC.EmployeeId
left outer join 
(
select distinct EmployeeId
,SUM(EmployeeProfit)PD_EmployeeProfit
,SUM(EmployerProfit)PD_EmployerProfit
,'ProfitDistribution'TransType
  from ViewEmployeeStatementPF
where TransType='ProfitDistribution' 
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId)PD on pf.EmployeeId=PD.EmployeeId
left outer join 
(
select distinct EmployeeId
,SUM(EmployeeContribution) PY_EmployeeContribution
,SUM(EmployerContribution) PY_EmployerContribution
,SUM(EmployeeProfit)       PY_EmployeeProfit
,SUM(EmployerProfit)       PY_EmployerProfit
,'Payment'TransType
  from ViewEmployeeStatementPF
where TransType='Payment' 
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId
)PY  on pf.EmployeeId=PY.EmployeeId

left outer join 
(select distinct EmployeeId,sum(PrincipalAmount)PrincipalAmount
,sum(InterestAmount)InterestAmount
,sum(PrincipalAmount)OutStandingLoanAmount
--+InterestAmount
from EmployeeLoanDetail
where 1=1
and PaymentScheduleDate>=@DateFrom and  PaymentScheduleDate<=@DatTo
and IsPaid=0
--and IsHold=0

--and EmployeeId=@Employeeid
group by EmployeeId) loan on pf.EmployeeId=loan.EmployeeId
left outer join ViewEmployeeInformation e  on pf.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pf.EmployeeId=eold.EmployeeId

--where pf.EmployeeId=@EmployeeId
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
                #endregion SqlText

                #region SqlExecution
                if (vm.EmployeeId != null)
                {
                    sqlText += @"  And e.Code=@EmployeeId";
                }             

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom == null ? "1900/Jan/31" : vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DatTo", Ordinary.DateToString(vm.DateTo == null ? "2029/Dec/31" : vm.DateTo));
                if (vm.EmployeeId != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                }

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

                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable GFEmployeeLedger(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
--declare @DateFrom   varchar(100)='20210702'
--declare @DatTo varchar(100)='20220701'
--declare @Employeeid varchar(100)='20220701'

select distinct pf.Employeeid
,CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation

,CASE
    WHEN e.JoinDate is null THEN eOld.JoinDate
    WHEN e.JoinDate = 'NA' THEN eOld.JoinDate
    ELSE e.JoinDate
END JoinDate
,isnull(OP.OP_EmployerContribution	,0)OP_EmployerContribution
,isnull(OP.OP_EmployerProfit		,0)OP_EmployerProfit
,isnull(MC.MC_EmployerContribution	,0)MC_EmployerContribution
,isnull(IA.IncrementArrear,0)IncrementArrear
,isnull(pd.PD_EmployerProfit		,0)PD_EmployerProfit
,isnull(PY.PY_EmployerContribution	,0)PY_EmployerContribution
,isnull(PY.PY_EmployerProfit		,0)PY_EmployerProfit

,isnull(OP.OP_EmployerContribution	,0)
+isnull(MC.MC_EmployerContribution	,0)
+isnull(IA.IncrementArrear	,0)
+isnull(-1*PY.PY_EmployerContribution	,0)

CL_EmployerContribution
,isnull(OP.OP_EmployerProfit		,0)+isnull(-1*PY.PY_EmployerProfit		,0)CL_EmployerProfit

,isnull(OP.OP_EmployerContribution	,0)
+isnull(OP.OP_EmployerProfit		,0)
+isnull(MC.MC_EmployerContribution	,0)
+isnull(IA.IncrementArrear	,0)
+isnull(pd.PD_EmployerProfit		,0)
+isnull(-1*PY.PY_EmployerContribution	,0)
+isnull(-1*PY.PY_EmployerProfit		,0)Total
,0 Loan

from ViewEmployeeStatementGF PF
left outer join 
(
select distinct EmployeeId
,SUM(EmployerContribution)OP_EmployerContribution
,SUM(EmployerProfit)OP_EmployerProfit
,'Opening'TransType
  from ViewEmployeeStatementGF
where TransactionDate<@DateFrom
group by EmployeeId
) OP on pf.EmployeeId=op.EmployeeId
left outer join 
(
select distinct EmployeeId
,SUM(EmployerContribution)MC_EmployerContribution
,'MonthlyContribution'TransType
  from ViewEmployeeStatementGF
where TransType in('MonthlyContribution' ,'BreakMonth' )
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId) MC on pf.EmployeeId=MC.EmployeeId
left outer join 
(
select distinct EmployeeId
,SUM(EmployerContribution)IncrementArrear
,'IncrementArrear'TransType
  from ViewEmployeeStatementGF
where TransType in('IncrementArrear' ,'IncrementArrear' )
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId) IA on pf.EmployeeId=IA.EmployeeId

left outer join 
(
select distinct EmployeeId
,SUM(EmployerProfit)PD_EmployerProfit
,'ProfitDistribution'TransType
  from ViewEmployeeStatementGF
where TransType='ProfitDistribution' 
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId)PD on pf.EmployeeId=PD.EmployeeId
left outer join 
(
select distinct EmployeeId
,SUM(EmployerContribution) PY_EmployerContribution
,SUM(EmployerProfit)       PY_EmployerProfit
,'Payment'TransType
  from ViewEmployeeStatementGF
where TransType='Payment' 
and TransactionDate>=@DateFrom and  TransactionDate<=@DatTo
group by EmployeeId
)PY  on pf.EmployeeId=PY.EmployeeId
left outer join ViewEmployeeInformation e  on pf.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pf.EmployeeId=eold.EmployeeId

--where pf.EmployeeId='1_9'
 


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
                #endregion SqlText

                #region SqlExecution
                if (vm.EmployeeId != null)
                {
                    sqlText += @"  where pf.EmployeeId=@EmployeeId";
                }
                sqlText += @"  Order by Code";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom == null ? "1900/Jan/31" : vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@DatTo", Ordinary.DateToString(vm.DateTo == null ? "2029/Dec/31" : vm.DateTo));
                if (vm.EmployeeId != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                }

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

                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable GFEmployeeStatement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 

 
select 

CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
--e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,pf.TransactionDate	
,pf.EmployerContribution	
,pf.EmployerProfit	
,pf.Total
, SUM(pf.Balance) OVER (partition BY e.Code
ORDER BY TransactionDate,SL ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Balance

,pf.TransType
,pf.Remarks
from (
select 'A'SL, OpeningDate TransactionDate,EmployeeId,  EmployerContribution, EmployerProfit
,'Opening'TransType
,'Opening'Remarks
, isnull(EmployerContribution,0) +isnull(EmployerProfit,0) Total
, isnull(EmployerContribution,0) +isnull(EmployerProfit,0) Balance

from GFEmployeeOpeinig
union all 
select 'B'SL, OpeningDate TransactionDate,EmployeeId,  EmployerContribution, EmployerProfit
,'BreakMonth'TransType
,isnull( GFEmployeeBreakMonth.Remarks,'Break Month')Remarks

,isnull(  isnull(EmployerContribution,0) +isnull(EmployerProfit,0),0) Total
,isnull(  isnull(EmployerContribution,0) +isnull(EmployerProfit,0),0) Balance
from GFEmployeeBreakMonth
union all 
select   'C'SL, fd.PeriodStart TransactionDate ,EmployeeId, (ProvisionAmount+isnull(IncrementArrear,0)) ProvisionAmount,  0 EmployerProfit
,'MonthlyProvision'TransType
,isnull( GFEmployeeProvisions.Remarks,'Monthly Provision')Remarks

,isnull(isnull(ProvisionAmount,0)+isnull(IncrementArrear,0),0) Total
,isnull(isnull(ProvisionAmount,0)+isnull(IncrementArrear,0),0) Balance

from GFEmployeeProvisions
left outer join FiscalYearDetail fd on GFEmployeeProvisions.FiscalYearDetailId=fd.Id
union all 
select   'C'SL, DistributionDate TransactionDate ,EmployeeId, 0 EmployerContribution
,EmployeerProfitDistribution EmployerProfit
,'ProfitDistribution'TransType
,isnull( GFProfitDistributionNew.Remarks,'')Remarks

,isnull( isnull(EmployeerProfitDistribution,0),0) Total
,isnull( isnull(EmployeerProfitDistribution,0),0) Balance
from GFProfitDistributionNew

union all 
select 'B'SL, PaymentDate TransactionDate,EmployeeId, EmployerContribution,  EmployerProfit
,'Payment'TransType
,isnull( GFEmployeePayment.Remarks,'Payment')Remarks

,isnull( isnull(EmployerContribution,0)+isnull(EmployerProfit,0),0) Total
,-1*( isnull(EmployerContribution,0)+isnull(EmployerProfit,0)) Balance

from GFEmployeePayment

) as pf
left outer join ViewEmployeeInformation e on pf.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pf.EmployeeId=eold.EmployeeId


WHERE  1=1
and Total <>0
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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by Project,Department,Section,EmployeeCode,TransactionDate,pf.sl";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable Loan_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select  
 Code
,TransactionDate
,Amount
,InterestAmount
,Remarks
,ReferenceNo
,Post
 from Loan

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by TransactionDate";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable LoanMonthlyPayment_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select  
 Code
,TransactionDate
,Amount
,InterestAmount
,Remarks
,ReferenceNo
,Post
 from LoanMonthlyPayment

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by TransactionDate";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable LoanRepaymentToBank_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select  
 Code
,TransactionDate
,Amount
,InterestAmount
,Remarks
,ReferenceNo
,Post
 from LoanRepaymentToBank

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by TransactionDate";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable LoanSattlement_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select  
 Code
,TransactionDate
,Amount
,InterestAmount
,Remarks
,ReferenceNo
,Post
 from LoanSattlement

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by TransactionDate";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable PFBankDeposits_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select br.BranchName,tm.name TransactionMedium,
bd.Code,bd.DepositDate,bd.DepositAmount
 from PFBankDeposits bd
left outer join BankBranchs br on bd.BankBranchId=br.Id
left outer join TransactionMedias tm on bd.TransactionMediaId=tm.Id

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by DepositDate";

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

                dt = Ordinary.DtColumnStringToDate(dt, "DepositDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable PFContribution_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 

 
select 
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END Code
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,fd.PeriodName
,pd.EmployeePFValue
,pd.EmployeerPFValue

from PFDetails pd
left outer join ViewEmployeeInformation e on pd.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pd.EmployeeId=eold.EmployeeId
left outer join FiscalYearDetail fd on pd.FiscalYearDetailId=fd.Id

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by fd.PeriodId,Project,Department,Section,Code ";

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

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable GFContribution_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
CASE
    WHEN e.Code is null THEN eOld.Code
    WHEN e.Code = 'NA' THEN eOld.Code
    ELSE e.Code
END EmployeeCode
,CASE
    WHEN e.EmpName is null THEN eOld.EmpName
    WHEN e.EmpName = 'NA' THEN eOld.EmpName
    ELSE e.EmpName
END EmpName
,CASE
    WHEN e.Project is null THEN eOld.Project
    WHEN e.Project = 'NA' THEN eOld.Project
    ELSE e.Project
END Project
,CASE
    WHEN e.Section is null THEN eOld.Section
    WHEN e.Section = 'NA' THEN eOld.Section
    ELSE e.Section
END Section
,CASE
    WHEN e.Department is null THEN eOld.Department
    WHEN e.Department = 'NA' THEN eOld.Department
    ELSE e.Department
END Department

,CASE
    WHEN e.Designation is null THEN eOld.Designation
    WHEN e.Designation = 'NA' THEN eOld.Designation
    ELSE e.Designation
END Designation
,fd.PeriodName  ---, e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,pd.ProvisionAmount

from GFEmployeeProvisions pd
left outer join ViewEmployeeInformation e on pd.EmployeeId=e.EmployeeId
left outer join   ViewEmployeeInformation eOld  on pd.EmployeeId=eold.EmployeeId
left outer join FiscalYearDetail fd on pd.FiscalYearDetailId=fd.Id

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @"  order by EmployeeCode  ";

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

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable ProfitDistributionNew_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
e.Project,e.Department,e.Section,e.Code EmployeeCode, e.EmpName,e.Designation
,pdf.TotalValue TotalDistributeValue
,pd.DistributionDate
,pd.EmployeeContribution
,pd.EmployerContribution
,pd.EmployeeProfit
,pd.EmployerProfit
,pd.EmployeeContribution+pd.EmployerContribution+pd.EmployeeProfit+pd.EmployerProfit TotalContribution
,pd.MultiplicationFactor
,pd.EmployeeProfitDistribution
,pd.EmployeerProfitDistribution
,pd.TotalProfit
from ProfitDistributionNew pd
left outer join PreDistributionFunds pdf on pd.PreDistributionFundId=pdf.Id
left outer join ViewEmployeeInformation e on pd.EmployeeId=e.EmployeeId

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by e.Project,e.Department,e.Section,e.Code,DistributionDate ";

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

                dt = Ordinary.DtColumnStringToDate(dt, "DistributionDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable ReturnOnBankInterests_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select bd.Code,bd.TransactionDate,bd.TotalValue
 from ReturnOnBankInterests bd

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by TransactionDate ";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable Withdraws_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select br.BranchName,tm.name TransactionMedium,
bd.Code,bd.WithdrawDate,bd.WithdrawAmount
 from Withdraws bd
left outer join BankBranchs br on bd.BankBranchId=br.Id
left outer join TransactionMedias tm on bd.TransactionMediaId=tm.Id

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by WithdrawDate ";

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

                dt = Ordinary.DtColumnStringToDate(dt, "WithdrawDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable Voucher_Statement(string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
 
select 
j.Code, j.TransactionDate
,cg.Name COAGroupName
--,cg.GroupType
--,cg.TypeOfReport
,c.Name AccountName
,c.Code AccountCode,
jd.DrAmount, jd.CrAmount,jd.IsDr
,case 
when jd.JournalType=1 then 'JournalVoucher'
when jd.JournalType=2 then 'PaymentVoucher'
when jd.JournalType=3 then 'ReceiptVoucher' end VoucherName
  from GLJournalDetails jd
left outer join GLJournals j on jd.GLJournalId=j.Id
left outer join COAs c on jd.COAId=c.Id
left outer join COAGroups cg on c.COAGroupId=cg.Id

WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by j.Id,jd.IsDr desc ";

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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }


        public string[] Year_Closing(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            string sqlText = "";
            string[] retResults = new string[6];
            retResults[0] = "Fail";//Success or Fail
            retResults[1] = "Fail";// Success or Fail Message
            retResults[2] = "";// Return Id
            retResults[3] = sqlText; //  SQL Query
            retResults[4] = "ex"; //catch ex
            retResults[5] = "InsertAccount"; //Method Name

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataSet ds = new DataSet();
            #endregion

            //DateTime DateFrom = DateTime.ParseExact(vm.DateFrom, "dd-MMM-yyyy", null);
            //string newDateString = DateFrom.ToString("yyyyMMdd");
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



                #region SqlExecution


                ds = Year_ClosingData(vm, currConn, transaction);

                GLJournalVM jvm = new GLJournalVM();

                BaseEntity baseEnrey = new BaseEntity();

                DataTable dt = ds.Tables[0];
                DataTable dt1 = ds.Tables[1];
                if (dt1.Rows.Count > 0)
                {
                    List<GLJournalDetailVM> glvmList = new List<GLJournalDetailVM>();

                    decimal totalDrAmount = 0;
                    decimal totalCrAmount = 0;
                    jvm.Remarks = "Yearly Closing";
                    //jvm.TransactionDate = DateTime.Now.ToString("yyyyMMdd");
                    jvm.TransactionDate = Ordinary.StringToDate(dt.Rows[0]["ClosingDate"].ToString());
                    jvm.Post = true;
                    jvm.Operation = "Add";
                    jvm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                    jvm.CreatedBy = vm.BaseEntity.CreatedBy;
                    jvm.CreatedFrom = vm.BaseEntity.CreatedFrom;
                    jvm.TransType =vm.TransType;
                    jvm.IsYearClosing = true;
                    jvm.JournalType = 1;

                    GLJournalDetailVM glvm = new GLJournalDetailVM();


                    foreach (DataRow item in dt1.Rows)
                    {
                        glvm = new GLJournalDetailVM();
                        glvm.COAId = int.Parse(item["CoaId"].ToString());
                        if (item["Nature"].ToString() == "Cr")
                        {
                            glvm.DrAmount = Convert.ToDecimal(item["TransactionAmount"].ToString());
                            glvm.IsDr = true;
                            totalDrAmount += glvm.DrAmount;
                        }
                        if (item["Nature"].ToString() == "Dr")
                        {
                            glvm.CrAmount = Convert.ToDecimal(item["TransactionAmount"].ToString());
                            glvm.IsDr = false;
                            totalCrAmount += glvm.CrAmount;
                        }
                        //glvm.GLJournalId = 1;
                        glvm.TransactionDate = jvm.TransactionDate;
                        glvm.TransType = vm.TransType;

                        glvmList.Add(glvm);
                    }

                    decimal difference = totalDrAmount - totalCrAmount;

                    glvm = new GLJournalDetailVM();
                    glvm.COAId = Convert.ToInt32(dt.Rows[0][0].ToString());
                    glvm.CrAmount = difference;
                    glvm.IsDr = difference < 0 ? false : true;
                    //differenceEntry.GLJournalId = 1;
                    //differenceEntry.TransactionDate = DateTime.Now.ToString("yyyyMMdd");
                    glvm.TransType = vm.TransType; 
                    glvm.Post = jvm.Post;
                    glvm.TransactionDate = jvm.TransactionDate;
                    glvm.JournalType = jvm.JournalType;
                    glvm.IsYearClosing = jvm.IsYearClosing;
                    glvmList.Add(glvm);
                    jvm.GLJournalDetails = glvmList;
                    GLJournalDAL _glDal = new GLJournalDAL();
                    retResults = _glDal.Insert(jvm, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        retResults[3] = retResults[4];
                        throw new ArgumentNullException("Unexpected error to Insert GLJournals. Year Closing", "");
                    }
                    retResults = _glDal.InsertNP(vm.TransType, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        retResults[3] = retResults[4];
                        throw new ArgumentNullException("Unexpected error to Insert GLJournals. Year Closing", "");
                    }
                  //retResults=  _glDal.Insert(jvm,currConn,transaction);
                  //if (retResults[0] != "Success")
                  //{
                  //    retResults[3] = retResults[4];
                  //    throw new ArgumentNullException("Unexpected error to Insert GLJournals. Year Closing", "");
                  //}
                  //retResults=  _glDal.InsertNP(vm.TransType, currConn,transaction);
                  //if (retResults[0] != "Success")
                  //{
                  //    retResults[3] = retResults[4];
                  //    throw new ArgumentNullException("Unexpected error to Insert GLJournals. Year Closing", "");
                  //}
                }


                #endregion SqlExecution
                #region SuccessResult
                retResults[0] = "Success";
                retResults[1] = "Process Successfully.";
                retResults[2] = "Id";
                #endregion SuccessResult

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return retResults;
        }

        public DataSet Year_ClosingData(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
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

--declare @CloseYear as varchar(100);
--set @CloseYear='2023';
declare @ClosingDate as varchar(100);
declare @GLJournalId as varchar(100);


declare @RetainedEarningsCOAId as varchar(100)
select @RetainedEarningsCOAId=id from COAs  where isnull(IsRetainedEarning,0)=1  and isnull(TransType,'PF') in(@TransType)

select @ClosingDate=YearEnd from FiscalYear where Year=@CloseYear
select @RetainedEarningsCOAId RetainedEarningsCOAId,@ClosingDate ClosingDate

select @GLJournalId=Id from GLJournals where TransactionDate =@ClosingDate and IsYearClosing=1  and isnull(TransType,'PF') in(@TransType)
delete from GLJournalDetails where GLJournalId=@GLJournalId 
delete from GLJournals where Id=@GLJournalId 

select distinct t.TransType, CoaId, c.Nature, c.COAType ,case when c.COAType in ('Revenue') then  -1*isnull(Sum(TransactionAmount),0) 
else  isnull(Sum(TransactionAmount),0) end TransactionAmount from View_GLJournalDetailNew t
left outer join COAs c on t.coaid=c.Id
where  c.COAType in ('Revenue','Expense')
and transactionDate <@ClosingDate 
and isnull(t.TransType,'PF') in(@TransType)
group by t.TransType, CoaId, c.Nature, c.COAType 

";

                #endregion SqlText

                #region SqlExecution

                
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@CloseYear", vm.YearTo);
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);
                da.Fill(ds);


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }

        public DataSet View_TrialBalance(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            string[] retResults = new string[6];
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

                retResults = new PFReportDAL().TempNetChangeProcess(vm, currConn, transaction);

                #region sql statement

                #region SqlText

                sqlText = @"
 
select * from View_TrialBalance
WHERE  1=1   

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

                #endregion SqlText

                #region SqlExecution
                sqlText += @" order by  GroupTypeSL, GroupSL,COASL";
                if (true)
                {


                    sqlText += @"  
SELECT 'C' AS SL
,dbo.View_COA.COAId, '9999' AS AccountCode, 'Current Year Profit (Loss)' AS AccountName, 
 SUM(a_1.Credit) - SUM(a_1.Debit) AS TransactionAmount
FROM      (
SELECT v.AccountCode, v.AccountName
, CASE WHEN Nature = 'Dr' THEN NetChange ELSE 0 END AS Debit, 
    CASE WHEN Nature = 'Cr' THEN NetChange * - 1 ELSE 0 END AS Credit
FROM      dbo.TempNetChange AS v RIGHT OUTER JOIN
        (SELECT DISTINCT COAId, MAX(RowSL) AS RowSL
        FROM      dbo.TempNetChange
        GROUP BY COAId) AS l ON l.COAId = v.COAId AND l.RowSL = v.RowSL
WHERE   (v.NetChange <> 0) AND (v.TypeOfReportShortName = 'IS')
) 
AS a_1 CROSS JOIN
dbo.View_COA
WHERE   (dbo.View_COA.IsRetainedEarning = 1)
and View_COA.TransType='PF'
GROUP BY   dbo.View_COA.COAId
, dbo.View_COA.AccountCode, dbo.View_COA.AccountName 
 ";
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

                da.Fill(ds);

                //DataTable dt = ds.Tables[0];
                //dt = Ordinary.DtColumnStringToDate(dt, "TransactionDate");
                //ds.Tables.RemoveAt(0);
                //ds.Tables.Add(dt);


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }
        public DataTable View_IncomeStatement(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            string[] retResults = new string[6];
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
                retResults = new PFReportDAL().TempNetChangeProcess(vm, currConn, transaction);

                #region sql statement

                #region SqlText

                sqlText = @"
 
select * from View_IncomeStatement
WHERE  1=1   

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
                #endregion SqlText

                #region SqlExecution

                sqlText += @" order by  GroupTypeSL, GroupSL,COASL";



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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }
        public DataTable View_BalanceSheet(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            string[] retResults = new string[6];
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
                retResults = new PFReportDAL().TempNetChangeProcess(vm, currConn, transaction);

                #region sql statement

                #region SqlText

                sqlText = @"
 
select * from View_BalanceSheet
WHERE  1=1   

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

                #endregion SqlText

                #region SqlExecution

                //sqlText += @" order by GroupTypeSL,   GroupSL, GroupName,COASL,AccountName, RowSL ";
                sqlText += @" order by  GroupTypeSL, GroupSL,COASL";


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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }
        public DataTable View_NetChange(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, bool NotZero = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable dt = new DataTable();
            string[] retResults = new string[6];
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

                retResults = new PFReportDAL().TempNetChangeProcess(vm, currConn, transaction);


                #region sql statement

                #region SqlText

                sqlText = @"
 
select * from TempNetChange
WHERE  1=1   
";
                if (NotZero == false)
                {
                    sqlText += @" and TransactionAmount <>0       ";
                }
                if (vm.Id != 0)
                {
                    sqlText += @" and COAId=@COAId       ";
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
                sqlText += @" order by  GroupTypeSL, GroupSL,COASL";

                //sqlText += @" order by GroupTypeSL,   GroupSL, GroupName,COASL,AccountName, RowSL ";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;

                if (vm.Id != 0)
                {
                    da.SelectCommand.Parameters.AddWithValue("@COAId", vm.Id);
                }
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


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        #endregion

        public string[] TempNetChangeProcess(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
            retResults[5] = "InsertBankName"; //Method Name
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
delete from TempNetChange where BranchId=@BranchId";

                if (vm.ReportType != "TrialBalance")
                {
                    sqlText += @" 

select * into #TempNetChange from   ( 
select *  from View_NetChange
where TransactionDate<@DateFrom
and TransType=@TransType and BranchId=@BranchId
";
                    if (vm.ReportType == "TrialBalance")
                    {
                        sqlText += @" and  GroupSL not in('400','500')";

                    }
                    if (vm.Id != 0)
                    {
                        sqlText += @" and COAId=@COAId";
                    }

                    sqlText += @" ) as a";

                    sqlText += @" insert into TempNetChange(
RowSL
,AccountName
,AccountCode
,GroupName
,GroupType
,TypeOfReport
,Nature
,GLJournalDetailId
,GLJournalId
,Code
,TransactionDate
,JournalType
,COAId
,IsDr
,DrAmount
,CrAmount
,Remarks
,TransType
,TransactionAmount
,NetChange
,IsRetainedEarning
,COAGroupId
,COAGroupTypeId
,COATypeOfReportId
,COASL
,GroupSL
,GroupTypeSL
,TypeOfReportSL
,GroupTypeShortName
,TypeOfReportShortName
)

select 
b.RowSL
,b.AccountName
,b.AccountCode
,b.GroupName
,b.GroupType
,b.TypeOfReport
,b.Nature
,0 GLJournalDetailId
,0 GLJournalId
,''Code
,'19000101'TransactionDate
,'Opening'JournalType
,b.COAId
,b.IsDr
,0 DrAmount
,0 CrAmount
,''Remarks
,b.TransType
,b.TransactionAmount
,b.NetChange
,b.IsRetainedEarning
,b.COAGroupId
,b.COAGroupTypeId
,b.COATypeOfReportId
,b.COASL
,b.GroupSL
,b.GroupTypeSL
,b.TypeOfReportSL
,b.GroupTypeShortName
,b.TypeOfReportShortName
from View_NetChange as b
right outer join (select distinct COAId,MAX(RowSL)RowSL from #TempNetChange
group by COAId) as a
on a.COAId=b.COAId and a.RowSL=b.RowSL
where b.TransType=@TransType";

                    if (vm.Id != 0)
                    {
                        sqlText += @" and a.COAId=@COAId";
                    }

                    sqlText += @" and  b.GroupSL not in('400','500')";
                    sqlText += @" drop table #TempNetChange";

                }
                sqlText += @" insert into TempNetChange(
RowSL
,AccountName
,AccountCode
,GroupName
,GroupType
,TypeOfReport
,Nature
,GLJournalDetailId
,GLJournalId
,Code
,TransactionDate
,JournalType
,COAId
,IsDr
,DrAmount
,CrAmount
,Remarks
,TransType
,TransactionAmount
,NetChange
,IsRetainedEarning
,COAGroupId
,COAGroupTypeId
,COATypeOfReportId
,COASL
,GroupSL
,GroupTypeSL
,TypeOfReportSL
,GroupTypeShortName
,TypeOfReportShortName
,BranchId
)
select RowSL
,AccountName
,AccountCode
,GroupName
,GroupType
,TypeOfReport
,Nature
,GLJournalDetailId
,GLJournalId
,Code
,TransactionDate
,JournalType
,COAId
,IsDr
,DrAmount
,CrAmount
,Remarks
,TransType
,TransactionAmount
,NetChange
,IsRetainedEarning
,COAGroupId
,COAGroupTypeId
,COATypeOfReportId
,COASL
,GroupSL
,GroupTypeSL
,TypeOfReportSL
,GroupTypeShortName
,TypeOfReportShortName
,BranchId
from View_NetChange
where TransactionDate>=@DateFrom and TransactionDate<=@DateTo
and TransType=@TransType and BranchId=@BranchId";

                if (vm.Id != 0)
                {
                    sqlText += @" and COAId=@COAId";
                }


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);
                cmdInsert.Parameters.AddWithValue("@DateFrom", Ordinary.DateToString(vm.DateFrom == null ? "1900/Jan/31" : vm.DateFrom));
                cmdInsert.Parameters.AddWithValue("@DateTo", Ordinary.DateToString(vm.DateTo == null ? "2029/Dec/31" : vm.DateTo));
                cmdInsert.Parameters.AddWithValue("@TransType", vm.TransType ?? "PF");
                cmdInsert.Parameters.AddWithValue("@BranchId", vm.BranchId);
                if (vm.Id != 0)
                {
                    cmdInsert.Parameters.AddWithValue("@COAId", vm.Id);
                }

                var exeRes = cmdInsert.ExecuteNonQuery();
                transResult = Convert.ToInt32(exeRes);
                if (transResult <= 0)
                {
                    retResults[3] = sqlText;
                    throw new ArgumentNullException("Unexpected error to update BankNames.", "");
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

        public DataSet IFRSReportsX(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            string[] retResults = new string[6];
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

declare @NetProfitCOAId as varchar(100)

select @NetProfitCOAId=id from COAs where COAType in ('NetProfit')

create table #TempNetProfit(operationType nvarchar(100),COAType nvarchar(100),TransactionAmount decimal(18,4))

TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where transactionDate <@StartDate  
group by TransType, CoaId

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where transactionDate >=@StartDate and transactionDate <=@EndDate
group by TransType, CoaId

insert into #TempNetProfit(operationType,COAType,TransactionAmount)
select 'NetProfitOpening', c.COAType,sum( transactionAmount) transactionAmount    
from TempNetChangeNew t
left outer join COAs c on t.coaid=c.Id
where operationType='opening' and c.reportType in ('IS')
group by  c.COAType
insert into #TempNetProfit(operationType,COAType,TransactionAmount)
select 'NetProfitNetchange', c.COAType,sum( transactionAmount) transactionAmount    
from TempNetChangeNew t
left outer join COAs c on t.coaid=c.Id
where operationType='netchange' and c.reportType in ('IS')
group by  c.COAType

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select 'PF','NetProfitOpening',@NetProfitCOAId,sum(TransactionAmount)TransactionAmount from (
select -1* TransactionAmount TransactionAmount from #TempNetProfit
where COAType in('Revenue') and operationType in ('NetProfitOpening')
union all
select -1*TransactionAmount from #TempNetProfit
where COAType in('Expense')  and operationType in ('NetProfitOpening')
) as a

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select 'PF','NetProfitNetchange',@NetProfitCOAId,sum(TransactionAmount)TransactionAmount from (
select -1* TransactionAmount TransactionAmount from #TempNetProfit
where COAType in('Revenue') and operationType in ('NetProfitNetchange')
union all
select -1*TransactionAmount from #TempNetProfit
where COAType in('Expense')  and operationType in ('NetProfitNetchange')
) as a

--select * from TempNetChangeNew
select 
case
when c.COAType in('Asset') then '1' 
when c.COAType in('Members Fund and Liabilities') then '2' 
when c.COAType in('OwnersEquity') then '3' 
when c.COAType in('RetainedEarnings') then '4' 
when c.COAType in('Revenue') then '4' 
when c.COAType in('Expense') then '5' 
when c.COAType in('NetProfit') then '6' 
when c.COAType in('NetProfit') then '7' 
else 0 
end as SL 
,c.reportType, case when  c.COAType in('RetainedEarnings') then 'OwnersEquity' else c.COAType end  COAType, c.GroupSL, c.COAGroupName, c.COAId AS COAId, c.TransactionType, c.TransType, c.COASL,c.Nature, c.COACode, c.COAName
";
                if (vm.ReportType.ToLower() == "is")
                {
                    sqlText += @" ,case when c.COAType in('Revenue','Members Fund and Liabilities','OwnersEquity','RetainedEarnings') then -1*isnull(op.TransactionAmount,0) else isnull(op.TransactionAmount,0) end OpeningAmount 
,case when c.COAType in('Revenue','Members Fund and LiabilitiesX') then -1*isnull(t.TransactionAmount,0) else isnull(t.TransactionAmount,0) end NetChange 
,case when c.COAType in('Revenue','Members Fund and Liabilities','OwnersEquity','RetainedEarnings') then -1*(isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0)) else isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0) end ClosingAmount 
";
                }
                else if (vm.ReportType.ToLower() == "bs")
                {
                    sqlText += @" ,case when c.COAType in('Revenue','Members Fund and Liabilities','OwnersEquity','RetainedEarnings') then -1*isnull(op.TransactionAmount,0) else isnull(op.TransactionAmount,0) end OpeningAmount 
,case when c.COAType in('Revenue','Members Fund and LiabilitiesX') then -1*isnull(t.TransactionAmount,0) else isnull(t.TransactionAmount,0) end NetChange 
,case when c.COAType in('Revenue','Members Fund and Liabilities','OwnersEquity','RetainedEarnings') then -1*(isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0)) else isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0) end ClosingAmount 
";
                }
                else
                {
                    sqlText += @" ,isnull(op.TransactionAmount,0)OpeningAmount
,isnull(t.TransactionAmount,0)   NetChange 
,isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0) ClosingAmount";
                }


                sqlText += @" 
--,isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0) ClosingAmount
,isnull(n.TransactionAmount,0) NetProfitOpening,isnull(nn.TransactionAmount,0) NetProfitNetChange

from View_COA_New c
left outer join (select distinct CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('opening')  
group by CoaId
)Op on c.COAId=op.CoaId
left outer join (select distinct CoaId,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('NetChange')  
group by CoaId
)t on c.COAId=t.CoaId

left outer join (select distinct CoaId,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('NetProfitOpening')  
group by CoaId
)n on c.COAId=n.CoaId

left outer join (select distinct CoaId,isnull(Sum(TransactionAmount),0)TransactionAmount from TempNetChangeNew 
where OperationType in('NetProfitNetchange')  
group by CoaId
)nn on c.COAId=nn.CoaId

where isnull(op.TransactionAmount,0)+isnull(t.TransactionAmount,0)+isnull(n.TransactionAmount,0)+isnull(nn.TransactionAmount,0)<>0 

";
                if (vm.ReportType.ToLower() == "is")
                {

                    sqlText += @" and c.ReportType in('is','netprofit') ";
                }
                else if (vm.ReportType.ToLower() == "bs")
                {

                    sqlText += @" and c.ReportType in('bs','netprofit') ";
                }
                sqlText += @" 

order by SL,c.GroupSL,c.COASL 
--select * from TempNetChangeNew
drop table #TempNetProfit";

                #endregion SqlText

                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                //da.SelectCommand.Parameters.AddWithValue("@ReportType", vm.ReportType);
                da.SelectCommand.Parameters.AddWithValue("@StartDate", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@EndDate", Ordinary.DateToString(vm.DateTo));

                da.Fill(ds);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }
        public DataSet IFRSReports(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            DataSet ds = new DataSet();

            try
            {
                if (vm.ReportType.ToUpper() == "BS")
                {
                    ds = IFRSReportsBS(vm);
                }
                else if (vm.ReportType.ToUpper() == "IS")
                {
                    ds = IFRSReportsIS(vm);

                }
                else if (vm.ReportType.ToUpper() == "TB" || vm.ReportType.ToUpper() == "NC")
                {
                    ds = IFRSReportsTB(vm);

                }

            }

            #region catch

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + FieldDelimeter + ex.Message.ToString());
            }

            #endregion

            #region finally

            finally
            {

            }
            #endregion

            return ds;
        }

        public DataSet IFRSReportsTB(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            string[] retResults = new string[6];
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
--declare @MonthFrom as varchar(100)='1148'
--declare @MonthTo as varchar(100)='1159'
--declare @TransType as varchar(100)='PF' -- closing

declare @FirstStart as varchar(100)
declare @FirstEnd as varchar(10)
declare @LastStart as varchar(100)
declare @LastEnd as varchar(100)

declare @FirstYear as varchar(100)
declare @LastYear as varchar(100)
declare @FirstRetainedEarning as decimal(18,4)
declare @LastRetainedEarning as decimal(18,4)
declare @FirstNetProfit  as decimal(18,4)
declare @LastNetProfit as decimal(18,4)
declare @NetProfitCOAId as varchar(100)
declare @RetainedEarningCOAId as varchar(100)

declare @OpeningNetChange  as decimal(18,4)
declare @ClosingNetChange  as decimal(18,4)


create table #TempNetChangeNew(id int identity(1,1),TransType varchar(100),OperationType varchar(100),COAId varchar(100),TransactionAmount decimal(18,2))
select @RetainedEarningCOAId=id from COAs where IsRetainedEarning=1 and isnull(TransType,'PF') in(@TransType)
select @NetProfitCOAId=id from COAs where IsNetProfit=1 and isnull(TransType,'PF') in(@TransType)

select @LastEnd=PeriodEnd,@LastYear=[Year] from FiscalYearDetail where id=@MonthTo
select @LastStart=YearStart   from FiscalYear where [Year]=@LastYear
select @FirstEnd=PeriodStart,@FirstYear=[Year] from FiscalYearDetail where id=@MonthFrom
select @FirstStart=YearStart   from FiscalYear where [Year]=@FirstYear

if	(@FirstStart is NULL) begin set @FirstStart='19000101'; end
if	(@LastStart is NULL) begin set @LastStart='19000101'; end
if	(@FirstEnd is NULL) begin set @FirstEnd='29001231'; end
if	(@LastEnd is NULL) begin set @LastEnd='29001231'; end
if	(@LastYear is NULL) begin set @LastYear='2900'; end
if	(@FirstYear is NULL) begin set @FirstYear='1900'; end

select @LastRetainedEarning=isnull(RetainedEarning,0) from NetProfitYearEnds where [Year]=@LastYear
select @FirstRetainedEarning=isnull(RetainedEarning,0) from NetProfitYearEnds where [Year]=@FirstYear

 
select  @LastNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@LastStart and transactionDate <=@LastEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')

select  @FirstNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@FirstStart and transactionDate <=@FirstEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')
 
TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
--and transactionDate >=@FirstStart 
and transactionDate <@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Asset','Members Fund and Liabilities','OwnersEquity') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@FirstStart 
and transactionDate <@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Revenue','Expense') 
group by TransType, CoaId



insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@FirstEnd
and transactionDate <=@LastEnd
and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and COAType in ('Asset','Members Fund and Liabilities','OwnersEquity') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@FirstEnd 
and transactionDate <=@LastEnd
and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and COAType in ('Revenue','Expense') 
group by TransType, CoaId

 

select @OpeningNetChange=isnull(sum(TransactionAmount),0) from #TempNetChangeNew where OperationType in('opening')
select @ClosingNetChange=isnull(sum(TransactionAmount),0)   from #TempNetChangeNew where OperationType in('NetChange')


insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', @RetainedEarningCOAId ,case when @OpeningNetChange>0 then isnull(Sum(@FirstRetainedEarning),0) else 0 end

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'NetChange', @RetainedEarningCOAId , isnull(Sum(@LastRetainedEarning),0) 


insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount,OpeningAmount,NetChange,ClosingAmount)
select @TransType,''OperationType ,c.COAId
,0 TransactionAmount
,o.TransactionAmount OpeningAmount
,0 NetChange
,n.TransactionAmount ClosingAmount

from View_COA_New c 
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew group by  COAId ) t on c.COAId=t.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='Opening'  group by  COAId) o on c.COAId=o.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='NetChange'  group by  COAId) n on c.COAId=n.COAId
  where isnull(c.TransType,'PF') in(@TransType)

select 
case when c.COAType in('Asset') then '1' 
when c.COAType in('Members Fund and Liabilities') then '2' 
when c.COAType in('OwnersEquity') then '3' 
when c.COAType in('Revenue') then '4' 
when c.COAType in('Expense') then '5' 
when c.COAType in('RetainedEarnings') then '6' 
when c.COAType in('NetProfit') then '7' 
when c.COAType in('NetProfit') then '8' 
else 0 
end as SL, c.GroupSL,c.COAGroupName,c.COASL,
c.Nature , c.COAType ,c.COAId,c.COACode,c.COAName 
,t.TransactionAmount 

,isnull(case when c.Nature in('Dr') and  t.ClosingAmount >=0 then  t.ClosingAmount 
when c.Nature in('Cr') and  t.ClosingAmount >0 then  t.ClosingAmount  
else 0 end,0) Dr
,isnull(case when c.Nature in('Cr') and  t.ClosingAmount <=0 then  -1*t.ClosingAmount 
when c.Nature in('Dr') and  t.ClosingAmount <0 then  -1* t.ClosingAmount 
else 0 end,0) Cr

,isnull(case when c.Nature in('Dr') and  t.OpeningAmount >=0 then  t.OpeningAmount 
when c.Nature in('Cr') and  t.OpeningAmount >0 then  t.OpeningAmount  
else 0 end,0) OpenDr
,isnull(case when c.Nature in('Cr') and  t.OpeningAmount <=0 then  -1*t.OpeningAmount 
when c.Nature in('Dr') and  t.OpeningAmount <0 then  -1* t.OpeningAmount 
else 0 end,0) OpenCr

,isnull(t.TransactionAmount,0)TransactionAmount
,isnull(t.OpeningAmount	   ,0)OpeningAmount	   
,isnull(t.NetChange		   ,0)NetChange		   
,isnull(t.ClosingAmount	   ,0)ClosingAmount	   

from View_COA_New c 
left outer join    TempNetChangeNew  t on c.COAId=t.COAId
where 1=1 
and (   t.ClosingAmount<>0)
 and isnull(c.TransType,'PF') in(@TransType)
 order by sl,GroupSL,COASL,COACode


   select @FirstStart FirstStart,@FirstEnd FirstEnd,@FirstYear  FirstYear,@LastStart LastStart,@LastEnd LastEnd,@LastYear LastYear,
isnull(@FirstRetainedEarning,0) FirstRetainedEarning,isnull(@FirstNetProfit,0) FirstNetProfit
,isnull(@LastRetainedEarning,0) LastRetainedEarning,isnull(@LastNetProfit,0) LastNetProfit


drop table #TempNetChangeNew
";

                #endregion SqlText
                sqlText = sqlText.Replace("HRMDB", vm.HRMDB);

                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                //da.SelectCommand.Parameters.AddWithValue("@StartYear", vm.YearFrom);
                //da.SelectCommand.Parameters.AddWithValue("@EndYear", vm.YearTo);
                da.SelectCommand.Parameters.AddWithValue("@MonthFrom", vm.MonthFrom);
                da.SelectCommand.Parameters.AddWithValue("@MonthTo", vm.MonthTo);
                //da.SelectCommand.Parameters.AddWithValue("MonthTo", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);

                da.Fill(ds);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }
        public DataSet IFRSReportsBS(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            string[] retResults = new string[6];
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





--declare @MonthTo as varchar(100)='1195'
--declare @TransType as varchar(100)='PF' -- closing


declare @FirstStart as varchar(100)
declare @FirstEnd as varchar(10)
declare @LastStart as varchar(100)
declare @LastEnd as varchar(100)

declare @FirstYear as varchar(100)
declare @LastYear as varchar(100)
declare @FirstRetainedEarning as decimal(18,4)
declare @LastRetainedEarning as decimal(18,4)
declare @FirstNetProfit  as decimal(18,4)
declare @LastNetProfit as decimal(18,4)
declare @NetProfitCOAId as varchar(100)
declare @RetainedEarningCOAId as varchar(100)


select @LastEnd=PeriodEnd,@LastYear=[Year] from FiscalYearDetail where id=@MonthTo

select @FirstStart=YearStart,@FirstYear=[Year]   from FiscalYear where [Year]=cast( @LastYear as int)-1
SELECT  @FirstEnd=CONVERT(VARCHAR(8),DATEADD(MONTH, -12, CONVERT(DATE, @LastEnd, 112)), 112)  
select @LastStart=YearStart  from FiscalYear where [Year]= @LastYear  

select @LastRetainedEarning=RetainedEarning from NetProfitYearEnds where Year=@LastYear
select @FirstRetainedEarning=RetainedEarning from NetProfitYearEnds where Year=@FirstYear

select @RetainedEarningCOAId=id from COAs where IsRetainedEarning=1 and isnull(TransType,'PF') in(@TransType)
select @NetProfitCOAId=id from COAs where IsNetProfit=1  and isnull(TransType,'PF') in(@TransType)
create table #TempNetChangeNew(id int identity(1,1),TransType varchar(100),OperationType varchar(100),COAId varchar(100),TransactionAmount decimal(18,2))

if	(@FirstStart is NULL) begin set @FirstStart='19000101'; end
if	(@LastStart is NULL) begin set @LastStart='19000101'; end
if	(@FirstEnd is NULL) begin set @FirstEnd='29001231'; end
if	(@LastEnd is NULL) begin set @LastEnd='29001231'; end
if	(@LastYear is NULL) begin set @LastYear='2900'; end
if	(@FirstYear is NULL) begin set @FirstYear='1900'; end

select  @LastNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@LastStart and transactionDate <=@LastEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')

select  @FirstNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@FirstStart and transactionDate <=@FirstEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')
 

TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
--and transactionDate >=@FirstStart 
and transactionDate <=@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Asset','Members Fund and Liabilities','OwnersEquity','Revenue') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', @NetProfitCOAId ,isnull(Sum(@FirstNetProfit),0) 

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', @RetainedEarningCOAId ,isnull(Sum(@FirstRetainedEarning),0) 

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
--and transactionDate >=@LastStart 
and transactionDate <=@LastEnd
and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and COAType in ('Asset','Members Fund and Liabilities','OwnersEquity','Revenue') 
group by TransType, CoaId

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'NetChange', @NetProfitCOAId ,isnull(Sum(@LastNetProfit),0) 

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'NetChange', @RetainedEarningCOAId ,isnull(Sum(@LastRetainedEarning),0) 

insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount,OpeningAmount,NetChange,ClosingAmount)
select @TransType,''OperationType ,c.COAId, isnull(t.TransactionAmount,0)TransactionAmount
,isnull(case when c.Nature in('Cr') and isnull(IsDepreciation,0)=0 then -1*o.TransactionAmount else o.TransactionAmount end,0) OpeningAmount
,isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0) NetChange
,isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0) ClosingAmount

from View_COA_New c 
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew group by  COAId ) t on c.COAId=t.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='Opening'  group by  COAId) o on c.COAId=o.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='NetChange'  group by  COAId) n on c.COAId=n.COAId
 where  isnull(c.TransType,'PF') in(@TransType)
select 
case when c.COAType in('Asset') then '1' 
when c.COAType in('Members Fund and Liabilities') then '2' 
when c.COAType in('OwnersEquity') then '3' 
when c.COAType in('RetainedEarnings') then '4' 
when c.COAType in('Revenue') then '4' 
when c.COAType in('Expense') then '5' 
when c.COAType in('NetProfit') then '6' 
when c.COAType in('NetProfit') then '7' 
else 0 
end as SL, c.GroupSL,c.COAGroupName,c.COASL,
c.Nature , c.COAType ,c.COACode,c.COAName ,
isnull(case when c.Nature in('Dr') and  t.TransactionAmount >=0 then  t.TransactionAmount 
when c.Nature in('Cr') and  t.TransactionAmount >0 then  t.TransactionAmount  
else 0 end,0) Dr
,isnull(case when c.Nature in('Cr') and  t.TransactionAmount <=0 then  -1*t.TransactionAmount 
when c.Nature in('Dr') and  t.TransactionAmount <0 then  -1* t.TransactionAmount 
else 0 end,0) Cr
,t.TransactionAmount
,t.OpeningAmount
,t.NetChange
,t.ClosingAmount

from View_COA_New c 
left outer join    TempNetChangeNew  t on c.COAId=t.COAId
where 1=1 and GroupSL<>6
and (t.TransactionAmount<>0 or t.OpeningAmount<>0 or t.NetChange<>0 or t.ClosingAmount<>0)
and c.COAType in ('Asset','Members Fund and Liabilities','OwnersEquity','Revenue') 
 and isnull(c.TransType,'PF') in(@TransType)
 order by sl,GroupSL,COASL,COACode

select @FirstStart FirstStart,@FirstEnd FirstEnd,@FirstYear  FirstYear,@LastStart LastStart,@LastEnd LastEnd,@LastYear LastYear,
isnull(@FirstRetainedEarning,0) FirstRetainedEarning,isnull(@FirstNetProfit,0) FirstNetProfit
,isnull(@LastRetainedEarning,0) LastRetainedEarning,isnull(@LastNetProfit,0) LastNetProfit

drop table #TempNetChangeNew

";




                #endregion SqlText
                sqlText = sqlText.Replace("HRMDB", vm.HRMDB);

                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                //da.SelectCommand.Parameters.AddWithValue("@StartYear", vm.YearFrom);
                //da.SelectCommand.Parameters.AddWithValue("@EndYear", vm.YearTo);
                //da.SelectCommand.Parameters.AddWithValue("@MonthFrom", vm.MonthFrom);
                da.SelectCommand.Parameters.AddWithValue("@MonthTo", vm.MonthTo);
                //da.SelectCommand.Parameters.AddWithValue("MonthTo", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);

                da.Fill(ds);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }

        public DataSet IFRSReportsIS(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            string[] retResults = new string[6];
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

--declare @MonthTo as varchar(100)='1171'
--declare @TransType as varchar(100)='PF' -- closing


declare @FirstStart as varchar(100)
declare @FirstEnd as varchar(10)
declare @LastStart as varchar(100)
declare @LastEnd as varchar(100)

declare @FirstYear as varchar(100)
declare @LastYear as varchar(100)
declare @FirstRetainedEarning as decimal(18,4)
declare @LastRetainedEarning as decimal(18,4)
declare @FirstNetProfit  as decimal(18,4)
declare @LastNetProfit as decimal(18,4)
declare @NetProfitCOAId as varchar(100)
declare @RetainedEarningCOAId as varchar(100)


select @LastEnd=PeriodEnd,@LastYear=[Year] from HRMDB.dbo.FiscalYearDetail where id=@MonthTo

select @FirstStart=YearStart,@FirstYear=[Year]   from HRMDB.dbo.FiscalYear where [Year]=cast( @LastYear as int)-1
SELECT  @FirstEnd=CONVERT(VARCHAR(8),DATEADD(MONTH, -12, CONVERT(DATE, @LastEnd, 112)), 112)  
select @LastStart=YearStart  from HRMDB.dbo.FiscalYear where [Year]= @LastYear  

select @LastRetainedEarning=RetainedEarning from NetProfitYearEnds where Year=@LastYear
select @FirstRetainedEarning=RetainedEarning from NetProfitYearEnds where Year=@FirstYear

select @RetainedEarningCOAId=id from COAs where IsRetainedEarning=1  and isnull(TransType,'PF') in(@TransType)
select @NetProfitCOAId=id from COAs where IsNetProfit=1  and isnull(TransType,'PF') in(@TransType)
create table #TempNetChangeNew(id int identity(1,1),TransType varchar(100),OperationType varchar(100),COAId varchar(100),TransactionAmount decimal(18,2))

if	(@FirstStart is NULL) begin set @FirstStart='19000101'; end
if	(@LastStart is NULL) begin set @LastStart='19000101'; end
if	(@FirstEnd is NULL) begin set @FirstEnd='29001231'; end
if	(@LastEnd is NULL) begin set @LastEnd='29001231'; end
if	(@LastYear is NULL) begin set @LastYear='2900'; end
if	(@FirstYear is NULL) begin set @FirstYear='1900'; end

select  @LastNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@LastStart and transactionDate <=@LastEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')

select  @FirstNetProfit=isnull(Sum(TransactionAmount),0)  
from View_GLJournalDetailNew 
where transactionDate >=@FirstStart and transactionDate <=@FirstEnd and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)  and COAType in ('Revenue','Expense')
 

TRUNCATE TABLE TempNetChangeNew; 
DBCC CHECKIDENT ('TempNetChangeNew', RESEED, 1);

insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct @TransType,'Opening', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@FirstStart 
and transactionDate <=@FirstEnd 
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and isnull(IsRetainedEarning,0)=0
and COAType in ('Revenue','Expense') 
group by TransType, CoaId


insert into #TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount)
select distinct TransType,'NetChange', CoaId ,isnull(Sum(TransactionAmount),0)TransactionAmount from View_GLJournalDetailNew 
where 1=1
and transactionDate >=@LastStart 
and transactionDate <=@LastEnd
and isnull(IsRetainedEarning,0)=0
and isnull(IsYearClosing,0)=0 
and TransType in(@TransType)
and COAType in ('Revenue','Expense') 
group by TransType, CoaId


insert into TempNetChangeNew(TransType,OperationType,COAId,TransactionAmount,OpeningAmount,NetChange,ClosingAmount)
select @TransType,''OperationType ,c.COAId, isnull(t.TransactionAmount,0)TransactionAmount
,isnull(case when c.Nature in('Cr') and isnull(IsDepreciation,0)=0 then -1*o.TransactionAmount else o.TransactionAmount end,0) OpeningAmount
,isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0) NetChange
,isnull(case when c.Nature in('Cr')  and isnull(IsDepreciation,0)=0 then -1*n.TransactionAmount else n.TransactionAmount end,0) ClosingAmount

from View_COA_New c 
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew group by  COAId ) t on c.COAId=t.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='Opening'  group by  COAId) o on c.COAId=o.COAId
left outer join (select distinct COAId,sum(TransactionAmount)TransactionAmount from #TempNetChangeNew where operationType='NetChange'  group by  COAId) n on c.COAId=n.COAId
  where isnull(c.TransType,'PF') in(@TransType)

select 
case when c.COAType in('Asset') then '1' 
when c.COAType in('Members Fund and Liabilities') then '2' 
when c.COAType in('Revenue') then '3' 
when c.COAType in('Expense') then '4' 
when c.COAType in('OwnersEquity') then '5' 
when c.COAType in('RetainedEarnings') then '6' 
when c.COAType in('NetProfit') then '7' 
when c.COAType in('NetProfit') then '8' 
else 0 
end as SL, c.GroupSL,c.COAGroupName,c.COASL,
c.Nature , c.COAType ,c.COACode,c.COAName ,
isnull(case when c.Nature in('Dr') and  t.TransactionAmount >=0 then  t.TransactionAmount 
when c.Nature in('Cr') and  t.TransactionAmount >0 then  t.TransactionAmount  
else 0 end,0) Dr
,isnull(case when c.Nature in('Cr') and  t.TransactionAmount <=0 then  -1*t.TransactionAmount 
when c.Nature in('Dr') and  t.TransactionAmount <0 then  -1* t.TransactionAmount 
else 0 end,0) Cr
,t.TransactionAmount
,t.OpeningAmount
,t.NetChange
,t.ClosingAmount

from View_COA_New c 
left outer join    TempNetChangeNew  t on c.COAId=t.COAId
where 1=1 
and (t.TransactionAmount<>0 or t.OpeningAmount<>0 or t.NetChange<>0 or t.ClosingAmount<>0)
and c.COAType in ('Revenue','Expense','OwnersEquity') 
 and isnull(c.TransType,'PF') in(@TransType)
 order by sl,GroupSL,COASL,COACode

   select @FirstStart FirstStart,@FirstEnd FirstEnd,@FirstYear  FirstYear,@LastStart LastStart,@LastEnd LastEnd,@LastYear LastYear,
isnull(@FirstRetainedEarning,0) FirstRetainedEarning,isnull(@FirstNetProfit,0) FirstNetProfit
,isnull(@LastRetainedEarning,0) LastRetainedEarning,isnull(@LastNetProfit,0) LastNetProfit

drop table #TempNetChangeNew





";




                #endregion SqlText
                sqlText = sqlText.Replace("HRMDB", vm.HRMDB);

                #region SqlExecution
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                //da.SelectCommand.Parameters.AddWithValue("@StartYear", vm.YearFrom);
                //da.SelectCommand.Parameters.AddWithValue("@EndYear", vm.YearTo);
                //da.SelectCommand.Parameters.AddWithValue("@MonthFrom", vm.MonthFrom);
                da.SelectCommand.Parameters.AddWithValue("@MonthTo", vm.MonthTo);
                //da.SelectCommand.Parameters.AddWithValue("MonthTo", Ordinary.DateToString(vm.DateFrom));
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);

                da.Fill(ds);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }




        public DataSet FRReports(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataSet ds = new DataSet();
            string[] retResults = new string[6];
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
                retResults = new PFReportDAL().TempNetChangeProcess(vm, currConn, transaction);

                #region sql statement

                #region SqlText

                sqlText = @"
 

--declare @FiscalYear int
--declare @transType varchar(10)
--set @FiscalYear='2023'
--set @transType='PF'

declare @YearStart varchar(10)
declare @YearEnd varchar(10)
declare @NextYearRetainedEarning decimal(18,4)=0
declare @RetainedEarningId varchar(10)
declare @NetIncomeId varchar(10)

select @YearStart =YearStart,@YearEnd =YearEnd from HRMDB.dbo.FiscalYear where [Year]=@FiscalYear
select @RetainedEarningId=Id from COAs where COASL='9999' and TransType=@transType
select @NetIncomeId=Id from COAs where COASL='8888' and TransType=@transType

delete from YearClosing where FiscalYear=@FiscalYear and transType=@transType
insert into YearClosing(transType,COAId,ClosingAmount,FiscalYear)
select distinct @transType, COAId, RunningTotal RunningTotal,@FiscalYear 
from(
SELECT COAId , SUM( TransactionAmount) 
OVER (PARTITION BY COAId ORDER BY COAId) AS RunningTotal
FROM View_GLJournalDetails
where  TransType=@transType
and TransactionDate>=@YearStart and TransactionDate<=@YearEnd
) as a
where RunningTotal<>0

insert into YearClosing(transType,COAId,ClosingAmount,FiscalYear)
 select @transType,@NetIncomeId, sum( ClosingAmount)ClosingAmount,@FiscalYear from (
select abs(sum(ClosingAmount))ClosingAmount from YearClosing
where coaid in(
select COAId from View_COA where TransType=@transType and  GroupSL='400'
)
and FiscalYear=@FiscalYear

union all
select -1*abs(sum(ClosingAmount))ClosingAmount from YearClosing
where coaid in(
select COAId from View_COA where  TransType=@transType and GroupSL='500'
)
and FiscalYear=@FiscalYear

) as a

select @NextYearRetainedEarning=ClosingAmount from YearClosing where FiscalYear=(@FiscalYear-1) and 
COAId in(select Id from COAs where COASL='8888' and TransType=@transType) and TransType=@transType

select @NextYearRetainedEarning=@NextYearRetainedEarning+(ClosingAmount) 
from YearClosing where FiscalYear=(@FiscalYear-1)
and  COAId in(select Id from COAs where COASL='9999' and TransType=@transType)  and TransType=@transType

if(@NextYearRetainedEarning<>0) begin
insert into YearClosing(transType,COAId,ClosingAmount,FiscalYear)
 select @transType,@RetainedEarningId, @NextYearRetainedEarning,@FiscalYear
 end 

insert into YearClosing(transType,COAId,ClosingAmount,FiscalYear)
select @transType,COAId,ClosingAmount,@FiscalYear from YearClosing where FiscalYear=(@FiscalYear-1)
and  COAId in(select COAId from View_COA where GroupSL not in('400','500','9999','8888') and TransType=@transType)

update YearClosing set ClosingAmount =-1*ClosingAmount where 
COAId in(
select COAId from 
 View_COA where transType=@transType and groupsl in('9999'))
 and  FiscalYear=@FiscalYear

delete  from YearClosing where isnull(closingAmount,0)=0 and  FiscalYear=@FiscalYear

 ";

                sqlText += @"
select  
 y.COAId
 ,c.TypeOfReport
,c.GroupType
,c.GroupName COAGroupName
,isnull(c.AccountCode,'9999')AccountCode
,isnull(c.AccountName,'9999')AccountName
,c.Nature
,@TransType TransType
,case	when (GroupSL in('9999') and   ClosingAmount<0) then abs(ClosingAmount)
		when (GroupSL not in('9999') and Nature='Dr' and   ClosingAmount>=0) then abs(ClosingAmount)
		when (GroupSL not in('9999') and Nature='Cr' and   ClosingAmount>0) then abs(ClosingAmount) else 0  end Debit
,case	when (GroupSL in('9999') and   ClosingAmount>=0) then  abs(ClosingAmount)
	when (GroupSL not in('9999') and Nature='Cr' and   ClosingAmount<=0) then  abs(ClosingAmount)
		when (GroupSL not in('9999') and Nature='Dr' and   ClosingAmount<0) then abs(ClosingAmount) else 0  end Credit
,ClosingAmount
";
                if (vm.ReportType == "IncomeStatement")
                {
                    sqlText += @" , abs(ClosingAmount) TransactionAmount";
                }
                else if (vm.ReportType == "BalanceSheet")
                {
                    sqlText += @" 
,case when c.GroupSL in('8888','9999') then  ClosingAmount 
when Nature='Dr' and ClosingAmount>=0 then ClosingAmount
when Nature='Dr' and ClosingAmount<0 then ClosingAmount
when Nature='Cr' and ClosingAmount<=0 then abs(ClosingAmount)
when Nature='Cr' and ClosingAmount>0 then ClosingAmount
else ClosingAmount
end  TransactionAmount

--,case when c.GroupSL not in('8888','9999') then abs( ClosingAmount) else ClosingAmount end  TransactionAmount
";
                }
                else
                {
                    sqlText += @" , ClosingAmount TransactionAmount";
                }

                sqlText += @"

,c.IsRetainedEarning
,c.COAGroupId
,c.COAGroupTypeId
,c.COATypeOfReportId
,c.COASL
,isnull(c.GroupSL,'9999')GroupSL
,c.GroupTypeSL
,c.TypeOfReportSL
,c.GroupTypeShortName
,c.TypeOfReportShortName
  from (
 select distinct FiscalYear,  y.TransType, y.COAId,sum(ClosingAmount)ClosingAmount from YearClosing y
where  y.TransType=@transType  and FiscalYear=@FiscalYear
group by FiscalYear,  y.TransType, y.COAId
) as y
left outer join View_COA c on y.COAId=c.COAId and  y.TransType=c.TransType 
where  y.TransType=@transType  and FiscalYear=@FiscalYear

";
                if (vm.ReportType == "IncomeStatement")
                {
                    sqlText += @" and isnull(c.GroupSL,'9999') in('400','500')";
                }
                else if (vm.ReportType == "BalanceSheet")
                {
                    sqlText += @" and isnull(c.GroupSL,'9999') not in('400','500')  and c.TypeOfReportShortName in('BS') ";
                }
                sqlText += @"

order by FiscalYear,  y.TransType,case when COASL='9999' then 'D' when COASL='8888' then 'E' else 'C' end asc ,c.AccountCode asc,Nature desc


";

                #endregion SqlText

                #region SqlExecution
                

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.Parameters.AddWithValue("@TransType", vm.TransType);
                da.SelectCommand.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear);

                da.Fill(ds);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return ds;
        }

        public DataTable InvestmentAccruedSummery()
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @" 
                        select 
                        n.Code
                        ,n.Name [Investment Name]
                        ,it.Name  [Investment Type]
                        ,b.Name [Bank Name]
                       ,FORMAT(CONVERT(DATE, n.InvestmentDate, 112), 'dd-MMM-yyyy') [Investment Date]                      
                        ,FORMAT(CONVERT(DATE, n.MaturityDate, 112), 'dd-MMM-yyyy') [Maturity Date]
                        ,n.AitInterest as [AIT Rate]
                        ,fd.PeriodName [Period Name] 
                        ,ia.ReferenceNo [Reference No]
                        ,ia.InvestmentValue AS [Investment Value]
                        ,ia.AccruedMonth [Accrued Month]
                        ,ia.InterestRate [Interest Rate]
                        ,ia.AccruedInterest AS [Accrued Interest]
                        ,ia.AitInterest AS [AIT]
                        ,ia.NetInterest AS [Net Interest]
                         from InvestmentAccrued ia
                         Left Outer Join InvestmentNames n on n.Id=ia.InvestmentNameId
                         Left Outer Join EnumInvestmentTypes it on it.Id=n.InvestmentTypeId
                         Left Outer Join BankNames b on b.Id=n.BankNameId
                        left outer join HRMDB.dbo.FiscalYearDetail fd on ia.FiscalYearDetailId=fd.Id
                        order by n.Code";
                
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.Fill(dt);
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }

        public DataTable InvestmentSummery()
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @"
SELECT
inv.Id
,inv.TransactionCode
,inv.ReferenceNo
,ina.Name InvestmentName
,eit.Name InvestmentType
,inv.InvestmentDate
,inv.FromDate
,inv.ToDate
,inv.MaturityDate
,inv.InvestmentRate
,inv.InvestmentValue AS InvestmentValue
,isnull(inv.IsEncashed,0)IsEncashed
, DATEDIFF(month, CONVERT(DATE, CONVERT(CHAR(8), inv.FromDate), 112), CONVERT(DATE, CONVERT(CHAR(8), inv.ToDate), 112)) AS InvestmenMonths
,(inv.InvestmentValue * inv.InvestmentRate) / 100.00 AS TotalInterest
,(inv.InvestmentValue + ((inv.InvestmentValue * inv.InvestmentRate) / 100.00)) AS TotalAmount
from Investments inv
LEFT OUTER JOIN EnumInvestmentTypes eit ON inv.InvestmentTypeId = eit.Id
LEFT OUTER JOIN InvestmentNames ina ON inv.InvestmentNameId = ina.Id
WHERE  1=1 AND inv.IsArchive = 0
 AND inv.TransType='PF'";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.Fill(dt);
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }

        public DataTable DataExportForSunTemplatePF(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
DECLARE @PeriodName AS varchar(100)
DECLARE @PeriodYear AS varchar(100)
DECLARE @PeriodEnd AS varchar(100)

SELECT @PeriodName=UPPER(LEFT(PeriodName, 3)), @PeriodYear=[Year],@PeriodEnd=PeriodEnd
FROM FiscalYearDetail
WHERE Id =@FiscalYearDetailId;
with cat as(
SELECT
    CASE 
        WHEN upvt.[Transaction Reference] = 'PROVIDENT FUND-EMPLYR CONT' THEN 'C' + e.DCode
    END as [Account code],
    'PF ' + @PeriodName + '-' + @PeriodYear + ' -' + s.EmpName AS Description,
      @PeriodName + '-' + @PeriodYear as [Transaction Reference],
    e.Code as [Extra Reference],
    CONVERT(varchar, CAST(@PeriodEnd AS date), 106) AS [Transaction Date],
    'BDT' as [Currency Code],
    upvt.[Transaction Amount],
    upvt.[Transaction Amount] as [Base Amount],
    s.Branch as [BRANCHE T1],
    s.DptCode as [ACTIVITY T2],
    e.TPNCode as [CLIENT/SUPPLIER T3],
    '' as [VAT TAX],
    e.DCode as [EMPLOYEE CODE T5],
    '' as [JOB FILE T6],
    '' as [CASH FLOW T7],
    '' as [REAL CUSTOMER T8],
    '' as [T9]
FROM 
   [dbo].View_TIBSalary s
LEFT OUTER JOIN 
    [dbo].ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN 
    [dbo].EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
CROSS APPLY (
    VALUES (
        'PROVIDENT FUND-EMPLYR CONT', 
        s.PFEmployer * 1
    )
) AS upvt ([Transaction Reference], [Transaction Amount])
WHERE 
    s.FiscalYearDetailId = @FiscalYearDetailId 
    AND upvt.[Transaction Amount] <> 0
    AND s.IsHold = 0

UNION ALL

SELECT
    'E' + e.DCode as [Account code],
    'PF ' + @PeriodName + '-' + @PeriodYear + ' -' + s.EmpName AS Description,
       @PeriodName + '-' + @PeriodYear as [Transaction Reference],
    e.Code as [Extra Reference],
    CONVERT(varchar, CAST(@PeriodEnd AS date), 106) AS [Transaction Date],
    'BDT' as [Currency Code],
    upvt.[Transaction Amount],
    upvt.[Transaction Amount] as [Base Amount],
    s.Branch as [BRANCHE T1],
    s.DptCode as [ACTIVITY T2],
    e.TPNCode as [CLIENT/SUPPLIER T3],
    '' as [VAT TAX],
    e.DCode as [EMPLOYEE CODE T5],
    '' as [JOB FILE T6],
    '' as [CASH FLOW T7],
    '' as [REAL CUSTOMER T8],
    '' as [T9]
FROM 
   [dbo].View_TIBSalary s
LEFT OUTER JOIN 
    [dbo].ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
LEFT OUTER JOIN 
   [dbo].EmployeePersonalDetail p ON s.EmployeeId = p.EmployeeId
CROSS APPLY (
    VALUES (
        'PROVIDENT FUND-EMPLYR CONT', 
        s.PFEmployer * 1
    )
) AS upvt ([Transaction Reference], [Transaction Amount])
WHERE 
    s.FiscalYearDetailId = @FiscalYearDetailId 
    AND upvt.[Transaction Amount] <> 0
    AND s.IsHold = 0
    AND upvt.[Transaction Reference] = 'PROVIDENT FUND-EMPLYR CONT'
	) 
	Select * from cat

	Union All
  Select 'CN0001' AS [Account code],
    'Total Provident Fund Contribution' AS Description,
    NULL AS [Transaction Reference],
    NULL AS [Extra Reference],
    NULL AS [Transaction Date],
    NULL AS [Currency Code],
    SUM(CAST(-[Transaction Amount] AS decimal(18, 2))) AS [Transaction Amount],
    SUM(CAST(-[Transaction Amount] AS decimal(18, 2))) AS [Base Amount],
    NULL AS [BRANCHE T1],
    NULL AS [ACTIVITY T2],
    NULL AS [CLIENT/SUPPLIER T3],
    NULL AS [VAT TAX],
    NULL AS [EMPLOYEE CODE T5],
    NULL AS [JOB FILE T6],
    NULL AS [CASH FLOW T7],
    NULL AS [REAL CUSTOMER T8],
    NULL AS [T9] from cat
";

                #region More Conditions

                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;
                da.SelectCommand.Parameters.AddWithValue("@FiscalYearDetailId", vm.MonthFrom);

                da.Fill(dt);

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

        public DataTable DataExportForSunTemplatePFProfit(PFReportVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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
        WITH cat AS (
            SELECT
                'PE' + e.DCode AS [Account code],
                'P-EC ' + e.EmpName AS Description,
                'PROFIT TF ' AS [Transaction Reference],
                e.Code AS [Extra Reference],
                @DateTo AS [Transaction Date],
                'BDT' AS [Currency Code],
                s.TotalProfit AS [Transaction Amount],
                s.TotalProfit AS [Base Amount],
                e.Branch AS [BRANCHE T1],
                e.DptCode AS [ACTIVITY T2],
                e.TPNCode AS [CLIENT/SUPPLIER T3],
                '' AS [VAT TAX],
                e.DCode AS [EMPLOYEE CODE T5],
                '' AS [JOB FILE T6],
                '' AS [CASH FLOW T7],
                '' AS [REAL CUSTOMER T8],
                '' AS [T9]
            FROM 
                 [ProfitDistributionNew] s
            LEFT OUTER JOIN 
                [dbo].ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
            WHERE 
                s.TotalProfit <> 0 AND s.DistributionDate BETWEEN @DateFrom AND @DateTo

            UNION ALL 

            SELECT
                'PC' + e.DCode AS [Account code],
                'P-CC ' + e.EmpName AS Description,
                'PROFIT TF ' AS [Transaction Reference],
                e.Code AS [Extra Reference],
                @DateTo AS [Transaction Date],
                'BDT' AS [Currency Code],
                s.TotalProfit AS [Transaction Amount],
                s.TotalProfit AS [Base Amount],
                e.Branch AS [BRANCHE T1],
                e.DptCode AS [ACTIVITY T2],
                e.TPNCode AS [CLIENT/SUPPLIER T3],
                '' AS [VAT TAX],
                e.DCode AS [EMPLOYEE CODE T5],
                '' AS [JOB FILE T6],
                '' AS [CASH FLOW T7],
                '' AS [REAL CUSTOMER T8],
                '' AS [T9]
            FROM 
                 [ProfitDistributionNew] s
            LEFT OUTER JOIN 
                [dbo].ViewEmployeeInformation e ON s.EmployeeId = e.EmployeeId
            WHERE 
                s.TotalProfit <> 0 AND s.DistributionDate BETWEEN @DateFrom AND @DateTo

        UNION ALL

        SELECT 
             'PF' + e.DCode AS [Account code],
                'P-FR ' + e.EmpName AS Description,
                'PROFIT TF ' AS [Transaction Reference],
                e.Code AS [Extra Reference],
                '20240531' AS [Transaction Date],
                'BDT' AS [Currency Code],
                f.EmployerContribution *-1 AS [Transaction Amount],
                f.EmployerContribution *-1 AS [Base Amount],
                e.Branch AS [BRANCHE T1],
                e.DptCode AS [ACTIVITY T2],
                e.TPNCode AS [CLIENT/SUPPLIER T3],
                '' AS [VAT TAX],
                e.DCode AS [EMPLOYEE CODE T5],
                '' AS [JOB FILE T6],
                '' AS [CASH FLOW T7],
                '' AS [REAL CUSTOMER T8],
                '' AS [T9]
        FROM 
            EmployeeForfeiture f
			   LEFT OUTER JOIN 
                [dbo].ViewEmployeeInformation e ON f.EmployeeId = e.EmployeeId
        ) 
        SELECT 
            *
        FROM 
            cat

        UNION ALL

        SELECT 
            '888888' AS [Account code],
            'PROFIT TF ' AS Description,
            'PROFIT TF ' AS [Transaction Reference],
            NULL AS [Extra Reference],
            NULL AS [Transaction Date],
            NULL AS [Currency Code],
            SUM(-[Transaction Amount]) AS [Transaction Amount],
            SUM(-[Base Amount]) AS [Base Amount],
            NULL AS [BRANCHE T1],
            NULL AS [ACTIVITY T2],
            NULL AS [CLIENT/SUPPLIER T3],
            NULL AS [VAT TAX],
            NULL AS [EMPLOYEE CODE T5],
            NULL AS [JOB FILE T6],
            NULL AS [CASH FLOW T7],
            NULL AS [REAL CUSTOMER T8],
            NULL AS [T9] 
        FROM 
            cat
";

                #region More Conditions

                #endregion

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;
                da.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom);
                da.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo);

                da.Fill(dt);

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


        public DataTable PFAllEmployee(PFReportVM vm, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
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

                sqlText = @"select b.Name AS Department, c.Name AS Designation,a.* from EmployeeInfo a
left join Department b on a.Department = b.Id
left join Designation c on a.Designation = c.Id
where a.IsActive =1 
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
                #endregion SqlText

                #region SqlExecution
                if (vm.EmployeeId != null)
                {
                    sqlText += @"  And a.Code=@EmployeeId";
                }
                if (vm.BranchId != null)
                {
                    sqlText += @" And  a.BranchId=@BranchId";
                }      
                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;             
                if (vm.EmployeeId != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@EmployeeId", vm.EmployeeId);

                }
                if (vm.BranchId != null)
                {                   
                    da.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }      
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

                dt = Ordinary.DtColumnStringToDate(dt, "JoinDate");


                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
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
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dt;
        }

        public DataTable ProfitDistributionSummery()
        {
            string[] retResults = new string[6];
            DataTable dt = new DataTable();
            SqlTransaction transaction = null;
            try
            {
                #region Variables
                SqlConnection currConn = null;
                string sqlText = "";
                #endregion
                #region open connection and transaction
                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                if (transaction == null) { transaction = currConn.BeginTransaction("ExportExcelFile"); }
                #endregion open connection and transaction
                #region DataRead From DB
                #region sql statement
                sqlText = @"
SELECT
ve.Code
,ve.EmpName
,pd.DistributionDate
,pd.TotalProfit
FROM ProfitDistributionNew pd
LEFT OUTER JOIN [dbo].FiscalYearDetail fydFrom ON pd.FiscalYearDetailId=fydFrom.Id
LEFT OUTER JOIN [dbo].ViewEmployeeInformation ve ON ve.EmployeeId=pd.EmployeeId
WHERE  1=1 AND pd.IsArchive = 0
 ";

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.Fill(dt);
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                throw ex;
            }
            return dt;
        }

    }
}
