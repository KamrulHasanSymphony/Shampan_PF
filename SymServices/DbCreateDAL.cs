using SymOrdinary;
using SymphonySofttech.Utilities;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml.Linq;

namespace SymServices
{
    public class DbCreateDAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public string[] Insert(DbCreateVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            string sqlText = "";
            int Id = 0;

            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id.ToString();
            retResults[3] = sqlText;
            retResults[4] = "";
            retResults[5] = "InsertDB";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                // ===========================
                // Connection Handling
                // ===========================
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                else
                {
                    currConn = _dbsqlConnection.GetConnectionSys(vm);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }
                }

                // ===========================
                // STEP 1: CREATE DATABASE (No Transaction)
                // ===========================
                sqlText = "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '" + vm.DatabaseName + "') " + "CREATE DATABASE [" + vm.DatabaseName + "]";

                using (SqlCommand cmd = new SqlCommand(sqlText, currConn))
                {
                    cmd.ExecuteNonQuery();
                }

                // ===========================
                // STEP 2: CHANGE DATABASE
                // ===========================
                currConn.ChangeDatabase(vm.DatabaseName);

                // ===========================
                // STEP 3: START TRANSACTION (for table)
                // ===========================
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                else
                {
                    transaction = currConn.BeginTransaction();
                }

                // ===========================
                // STEP 4: CREATE TABLE
                // ===========================

                #region Views

                sqlText += @"CREATE VIEW [dbo].[ViewEmployeeInformation]
AS
SELECT        ei.Id, ei.Id AS EmployeeId, ei.Department AS DepartmentId, ei.Designation AS DesignationId, ei.Project AS ProjectId, ei.Section AS SectionId, ei.Code, ei.Name AS EmpName, ei.DateOfBirth, ei.JoinDate, ei.ResignDate, ei.Branch, 
                         ei.Grade, ISNULL(ei.GrossSalary, 0) AS GrossSalary, ISNULL(ei.BasicSalary, 0) AS BasicSalary, ei.PhotoName, ei.IsActive, ei.IsArchive, ei.LastUpdateAt, ei.LastUpdateBy, ei.LastUpdateFrom, ei.CreatedBy, ei.CreatedAt, 
                         ei.CreatedFrom, ei.Other1, ei.Remarks, ei.Email, d.Name AS Department, dg.Name AS Designation, s.Name AS Section, p.Name AS Project, ei.BranchId, br.Name AS UnitName, ei.ResignReason
FROM            dbo.EmployeeInfo AS ei LEFT OUTER JOIN
                         dbo.Department AS d ON ei.Department = d.Id LEFT OUTER JOIN
                         dbo.Designation AS dg ON ei.Designation = dg.Id LEFT OUTER JOIN
                         dbo.Section AS s ON ei.Section = s.Id LEFT OUTER JOIN
                         dbo.Project AS p ON ei.Project = p.Id LEFT OUTER JOIN
                         dbo.Branch AS br ON br.Id = ei.BranchId";

                sqlText += @"Create View [dbo].[View_LoanDetails] AS

SELECT        I.Id, I.BranchId, I.LoanType_E, I.EmployeeId, ve.EmpName, ve.Code, ve.Designation, ve.Department, ve.Section, I.IsFixed, I.InterestPolicy, I.InterestRate, I.TotalAmount, I.NumberOfInstallment, I.StartDate, I.EndDate, I.IsHold, 
                         I.IsApproved, I.ApplicationDate, I.ApprovedDate, I.RefundAmount, ISNULL(I.RefundDate, '') AS RefundDate, t.Name AS LoanType, I.LoanNo, ELD.Id AS Expr1, ELD.EmployeeLoanId, ELD.EmployeeId AS Expr2, 
                         ELD.InstallmentAmount, ELD.InstallmentPaidAmount, ELD.PaymentScheduleDate, ELD.PaymentDate, CASE WHEN ELD.IsHold = 0 THEN 'N' ELSE 'Y' END AS Expr3, CASE WHEN ELD.IsPaid = 0 THEN 'N' ELSE 'Y' END AS IsPaid,
                          CASE WHEN ELD.HaveDuplicate = 0 THEN 'N' ELSE 'Y' END AS HaveDuplicate, ELD.DuplicateID, ELD.Remarks, ELD.PrincipalAmount, ELD.InterestAmount, 
                         CASE WHEN IsPaid = 1 THEN ELD.PrincipalAmount ELSE 0 END AS PrincipalAmountPaid, CASE WHEN IsPaid = 1 THEN ELD.InterestAmount ELSE 0 END AS InterestAmountPaid
FROM            dbo.EmployeeLoanDetail AS ELD LEFT OUTER JOIN
                         dbo.EmployeeLoan AS I ON ELD.EmployeeLoanId = I.Id LEFT OUTER JOIN
                         dbo.ViewEmployeeInformation AS ve ON I.EmployeeId = ve.EmployeeId LEFT OUTER JOIN
                         dbo.EnumLoanType AS t ON t.Id = I.LoanType_E
WHERE        (ELD.IsArchive = 0)";

                sqlText += @"CREATE VIEW [dbo].[View_COA_New]
AS
SELECT        c.COAGroupId, cg.GroupSL, cg.Name AS COAGroupName, c.Id AS COAId, c.Code AS COACode, c.Name AS COAName, c.TransactionType, c.TransType, c.COASL, c.Nature, c.COAType, c.IsRetainedEarning, c.IsNetProfit, 
                         c.IsDepreciation, c.BranchId
FROM            dbo.COAs AS c LEFT OUTER JOIN
                         dbo.COAGroups AS cg ON c.COAGroupId = cg.Id


CREATE VIEW [dbo].[View_GLJournalDetailNew]
AS
SELECT        j.Code, j.TransactionDate, j.JournalType, j.TransType, j.TransactionType, c.COACode, c.COAName, c.Nature AS COANature, c.COAType, jd.DrAmount, jd.CrAmount, ISNULL(jd.DrAmount, 0) - ISNULL(jd.CrAmount, 0) 
                         AS TransactionAmount, c.COAId, j.Id AS GLJournalId, jd.Id AS GLJournalDetailId, j.IsYearClosing, c.IsNetProfit, c.IsRetainedEarning, c.BranchId
FROM            dbo.GLJournalDetails AS jd LEFT OUTER JOIN
                         dbo.GLJournals AS j ON jd.GLJournalId = j.Id LEFT OUTER JOIN
                         dbo.View_COA_New AS c ON jd.COAId = c.COAId";

                sqlText += @"CREATE VIEW [dbo].[View_IncomeStatement]
AS
SELECT TOP (100) PERCENT v.TypeOfReport, v.GroupType, v.GroupName, v.AccountCode, v.AccountName, v.Nature, v.TransType, CASE WHEN Nature = 'Dr' THEN NetChange ELSE 0 END AS Debit, 
                  CASE WHEN Nature = 'Cr' THEN NetChange * - 1 ELSE 0 END AS Credit, CASE WHEN Nature = 'Dr' THEN NetChange ELSE NetChange * - 1 END AS TransactionAmount, v.IsRetainedEarning, v.COAGroupId, v.COAGroupTypeId, 
                  v.COATypeOfReportId, v.COASL, v.GroupSL, v.GroupTypeSL, v.TypeOfReportSL, v.GroupTypeShortName, v.TypeOfReportShortName
FROM     dbo.TempNetChange AS v RIGHT OUTER JOIN
                      (SELECT DISTINCT COAId, MAX(RowSL) AS RowSL
                       FROM      dbo.TempNetChange
                       GROUP BY COAId) AS l ON l.COAId = v.COAId AND l.RowSL = v.RowSL
WHERE  (v.NetChange <> 0) AND (v.TypeOfReportShortName = 'IS')
ORDER BY v.COAId";

                sqlText += @"CREATE VIEW [dbo].[View_TrialBalance]
AS
SELECT TOP (100) PERCENT v.TypeOfReport, v.GroupType, v.GroupName AS COAGroupName, v.AccountCode, v.AccountName, v.Nature, v.TransType, CASE WHEN Nature = 'Dr' AND NetChange >= 0 THEN NetChange WHEN Nature = 'Cr' AND 
                  NetChange > 0 THEN NetChange * + 1 ELSE 0 END AS Debit, CASE WHEN Nature = 'Cr' THEN NetChange * - 1 WHEN Nature = 'Dr' AND NetChange < 0 THEN NetChange * - 1 ELSE 0 END AS Credit, 
                  CASE WHEN Nature = 'Dr' THEN NetChange ELSE NetChange * - 1 END AS TransactionAmount, v.IsRetainedEarning, v.COAGroupId, v.COAGroupTypeId, v.COATypeOfReportId, v.COASL, v.GroupSL, v.GroupTypeSL, v.TypeOfReportSL, 
                  v.GroupTypeShortName, v.TypeOfReportShortName
FROM     dbo.TempNetChange AS v RIGHT OUTER JOIN
                      (SELECT DISTINCT COAId, MAX(RowSL) AS RowSL
                       FROM      dbo.TempNetChange
                       GROUP BY COAId) AS l ON l.COAId = v.COAId AND l.RowSL = v.RowSL
WHERE  (v.NetChange <> 0)";

                sqlText += @"CREATE VIEW [dbo].[ViewEmployeeStatementGF]
AS
SELECT        EmployeeId, TransactionDate, EmployerContribution, EmployerProfit, Total, TransType, Remarks
FROM            (SELECT        'A' AS SL, '19000101' AS TransactionDate, EmployeeId, EmployerContribution, EmployerProfit, 'Opening' AS TransType, 'Opening' AS Remarks, ISNULL(EmployerContribution, 0) + ISNULL(EmployerProfit, 0) 
                                                    AS Total
                          FROM            dbo.GFEmployeeOpeinig
                          UNION ALL
                          SELECT        'B' AS SL, OpeningDate AS TransactionDate, EmployeeId, EmployerContribution, EmployerProfit, 'BreakMonth' AS TransType, ISNULL(Remarks, 'Break Month') AS Remarks, ISNULL(EmployerContribution, 0) 
                                                   + ISNULL(EmployerProfit, 0) AS Total
                          FROM            dbo.GFEmployeeBreakMonth
                          UNION ALL
                          SELECT        'C' AS SL, fd.PeriodStart AS TransactionDate, dbo.GFEmployeeProvisions.EmployeeId, dbo.GFEmployeeProvisions.ProvisionAmount, 0 AS EmployerProfit, 'MonthlyContribution' AS TransType, 
                                                   ISNULL(dbo.GFEmployeeProvisions.Remarks, 'Monthly Contribution') AS Remarks, ISNULL(ISNULL(dbo.GFEmployeeProvisions.ProvisionAmount, 0), 0) AS Total
                          FROM            dbo.GFEmployeeProvisions LEFT OUTER JOIN
                                                  FiscalYearDetail AS fd ON dbo.GFEmployeeProvisions.FiscalYearDetailId = fd.Id
                          UNION ALL
                          SELECT        'C' AS SL, fd.PeriodStart AS TransactionDate, GFEmployeeProvisions_1.EmployeeId, GFEmployeeProvisions_1.IncrementArrear, 0 AS EmployerProfit, 'IncrementArrear' AS TransType, 
                                                   ISNULL(GFEmployeeProvisions_1.Remarks, 'IncrementArrear') AS Remarks, ISNULL(ISNULL(GFEmployeeProvisions_1.ProvisionAmount, 0), 0) AS Total
                          FROM            dbo.GFEmployeeProvisions AS GFEmployeeProvisions_1 LEFT OUTER JOIN
                                               FiscalYearDetail AS fd ON GFEmployeeProvisions_1.FiscalYearDetailId = fd.Id
                          UNION ALL
                          SELECT        'C' AS SL, DistributionDate AS TransactionDate, EmployeeId, 0 AS EmployerContribution, EmployeerProfitDistribution AS EmployerProfit, 'ProfitDistribution' AS TransType, ISNULL(Remarks, 'Profit Distribution') 
                                                   AS Remarks, ISNULL(EmployeerProfitDistribution, 0) AS Total
                          FROM            dbo.GFProfitDistributionNew
                          UNION ALL
                          SELECT        'B' AS SL, PaymentDate AS TransactionDate, EmployeeId, 1 * EmployerContribution AS EmployerContribution, 1 * EmployerProfit AS EmployerProfit, 'Payment' AS TransType, ISNULL(Remarks, 'Payment') 
                                                   AS Remarks, ISNULL(EmployerContribution, 0) + ISNULL(EmployerProfit, 0) AS Total
                          FROM            dbo.GFEmployeePayment) AS pf
WHERE        (1 = 1) AND (Total <> 0)
";

                sqlText += @"Create view [dbo].[ViewEmployeeStatementPF] as
SELECT        EmployeeId, TransactionDate, EmployeeContribution, EmployerContribution, EmployeeProfit, EmployerProfit, Total, TransType, Remarks
FROM            (SELECT        'A' AS SL, '19000101' AS TransactionDate, EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit, EmployerProfit, 'Opening' AS TransType, 'Opening' AS Remarks, 
                                                    ISNULL(ISNULL(EmployeeContribution, 0) + ISNULL(EmployerContribution, 0) + ISNULL(EmployeeProfit, 0) + ISNULL(EmployerProfit, 0), 0) AS Total
                          FROM            dbo.EmployeePFOpeinig
                          UNION ALL
                          SELECT        'B' AS SL, OpeningDate AS TransactionDate, EmployeeId, EmployeeContribution, EmployerContribution, EmployeeProfit, EmployerProfit, 'BreakMonth' AS TransType, ISNULL(Remarks, 'Break Month') AS Remarks, 
                                                   ISNULL(ISNULL(EmployeeContribution, 0) + ISNULL(EmployerContribution, 0) + ISNULL(EmployeeProfit, 0) + ISNULL(EmployerProfit, 0), 0) AS Total
                          FROM            dbo.EmployeeBreakMonthPF
                          UNION ALL
                          SELECT        'C' AS SL, fd.PeriodStart AS TransactionDate, dbo.PFDetails.EmployeeId, dbo.PFDetails.EmployeePFValue AS EmployeeContribution, dbo.PFDetails.EmployeerPFValue, 0 AS EmployeeProfit, 0 AS EmployerProfit, 
                                                   'MonthlyContribution' AS TransType, ISNULL(dbo.PFDetails.Remarks, 'Monthly Contribution') AS Remarks, ISNULL(ISNULL(dbo.PFDetails.EmployeePFValue, 0) + ISNULL(dbo.PFDetails.EmployeerPFValue, 0), 0) 
                                                   AS Total
                          FROM            dbo.PFDetails LEFT OUTER JOIN
                                                   dbo.FiscalYearDetail AS fd ON dbo.PFDetails.FiscalYearDetailId = fd.Id
                          UNION ALL
                          SELECT        'C' AS SL, DistributionDate AS TransactionDate, EmployeeId, 0 AS EmployeeContribution, 0 AS EmployerContribution, EmployeeProfitDistribution AS EmployeeProfit, EmployeerProfitDistribution AS EmployerProfit, 
                                                   'ProfitDistribution' AS TransType, ISNULL(Remarks, 'Profit Distribution') AS Remarks, ISNULL(ISNULL(EmployeeProfitDistribution, 0) + ISNULL(EmployeerProfitDistribution, 0), 0) AS Total
                          FROM            dbo.ProfitDistributionNew
                          UNION ALL
                          SELECT        'B' AS SL, PaymentDate AS TransactionDate, EmployeeId, 1 * EmployeeContribution AS EmployeeContribution, 1 * EmployerContribution AS EmployerContribution, 1 * EmployeeProfit AS EmployeeProfit, 
                                                   1 * EmployerProfit AS EmployerProfit, 'Payment' AS TransType, ISNULL(Remarks, 'Payment') AS Remarks, 1 * ISNULL(ISNULL(EmployeeContribution, 0) + ISNULL(EmployerContribution, 0) + ISNULL(EmployeeProfit, 
                                                   0) + ISNULL(EmployerProfit, 0), 0) AS Total
                          FROM            dbo.EmployeePFPayment) AS pf
WHERE        (1 = 1) AND (Total <> 0)
";

                sqlText += @"Create View [dbo].[View_COA] AS

SELECT        cg.Name AS GroupName, cgt.Name AS GroupType, ctr.Name AS TypeOfReport, c.Name AS AccountName, c.Code AS AccountCode, c.Nature AS NatureXX, ISNULL(c.IsRetainedEarning, 0) AS IsRetainedEarning, c.TransType, 
                         ISNULL(c.OpeningBalance, 0) AS OpeningBalance, c.Id AS COAId, c.COAGroupId, cg.COAGroupTypeId, cg.Name AS COAGroupName, cg.COATypeOfReportId, ISNULL(c.COASL, 999) AS COASL, ISNULL(cg.GroupSL, 999) 
                         AS GroupSL, ISNULL(cgt.GroupTypeSL, 999) AS GroupTypeSL, ISNULL(ctr.TypeOfReportSL, 999) AS TypeOfReportSL, cgt.GroupTypeShortName, ctr.TypeOfReportShortName, cgt.COANature AS Nature
FROM            dbo.COAs AS c LEFT OUTER JOIN
                         dbo.COAGroups AS cg ON c.COAGroupId = cg.Id LEFT OUTER JOIN
                         dbo.COATypeOfReport AS ctr ON cg.COATypeOfReportId = ctr.Id LEFT OUTER JOIN
                         dbo.COAGroupType AS cgt ON cg.COAGroupTypeId = cgt.Id
";

                sqlText += @"CREATE VIEW [dbo].[View_GLJournalDetails]
AS
SELECT        c.AccountCode, c.AccountName, c.GroupName, c.GroupType, c.TypeOfReport, c.Nature, a.GLJournalId, a.GLJournalDetailId, a.Code, a.TransactionDate, a.JournalTypeId, a.JournalType, a.COAId, a.IsDr, a.DrAmount, a.CrAmount, 
                         a.Remarks, a.TransactionAmount, c.TransType, c.IsRetainedEarning, c.COAGroupId, c.COAGroupTypeId, c.COATypeOfReportId, c.COASL, c.GroupSL, c.GroupTypeSL, c.TypeOfReportSL, c.GroupTypeShortName, 
                         c.TypeOfReportShortName, a.BranchId
FROM            (SELECT        0 AS GLJournalId, 0 AS GLJournalDetailId, '' AS Code, '19000101' AS TransactionDate, 0 AS JournalTypeId, 'Opening' AS JournalType, Id AS COAId, CASE WHEN Nature = 'Dr' THEN 1 ELSE 0 END AS IsDr, 
                                                    0 AS DrAmount, 0 AS CrAmount, '' AS Remarks, CASE WHEN Nature = 'Dr' THEN OpeningBalance ELSE - 1 * OpeningBalance END AS TransactionAmount, '' AS BranchId
                          FROM            dbo.COAs
                          UNION ALL
                          SELECT        h.Id AS GLJournalId, d.Id AS GLJournalDetailId, h.Code, d.TransactionDate, d.JournalType AS JournalTypeId, 
                                                   CASE WHEN d .JournalType = 1 THEN 'JournalVoucher' WHEN d .JournalType = 2 THEN 'PaymentVoucher' WHEN d .JournalType = 3 THEN 'ReceiptVoucher' END AS JournalType, d.COAId, d.IsDr, d.DrAmount, 
                                                   d.CrAmount, ISNULL(ISNULL(d.Remarks, h.Remarks), '') AS Remarks, CASE WHEN IsDr = 1 THEN DrAmount ELSE - 1 * CrAmount END AS TransactionAmount, h.BranchId
                          FROM            dbo.GLJournalDetails AS d LEFT OUTER JOIN
                                                   dbo.GLJournals AS h ON d.GLJournalId = h.Id) AS a LEFT OUTER JOIN
                         dbo.View_COA AS c ON a.COAId = c.COAId";

                sqlText += @"CREATE VIEW [dbo].[View_NetChange]
AS
SELECT        ROW_NUMBER() OVER (Partition BY j.COAId
ORDER BY j.COAId) AS RowSL, j.AccountName, j.AccountCode, j.GroupName GroupName, GroupType, TypeOfReport, Nature, j.GLJournalDetailId, j.GLJournalId, j.Code, j.TransactionDate, j.JournalType, j.COAId, j.IsDr, j.DrAmount, 
j.CrAmount, j.Remarks, j.TransType, j.TransactionAmount, SUM(j.TransactionAmount) OVER (partition BY j.COAId
ORDER BY j.COAId, transactionDAte ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS NetChange, j.IsRetainedEarning, j.COAGroupId, j.COAGroupTypeId, j.COATypeOfReportId, j.COASL, j.GroupSL, j.GroupTypeSL, 
j.TypeOfReportSL, j.GroupTypeShortName, j.TypeOfReportShortName,j.BranchId
FROM            View_GLJournalDetails j";

                #endregion Views

                #region Tables

                sqlText = @"
CREATE TABLE [dbo].[Branch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Address] [varchar](500) NULL,
	[District] [nvarchar](200) NULL,
	[Division] [nvarchar](200) NULL,
	[Country] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[PostalCode] [varchar](50) NULL,
	[Phone] [nvarchar](100) NULL,
	[Mobile] [nvarchar](100) NULL,
	[Fax] [nvarchar](100) NULL,
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


CREATE TABLE [dbo].[Project](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Startdate] [nvarchar](14) NULL,
	[EndDate] [nvarchar](14) NULL,
	[ManpowerRequired] [int] NOT NULL,
	[ContactPerson] [nvarchar](500) NULL,
	[ContactPersonDesignation] [nvarchar](500) NULL,
	[Address] [varchar](500) NULL,
	[District] [nvarchar](200) NULL,
	[Division] [nvarchar](200) NULL,
	[Country] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[PostalCode] [varchar](50) NULL,
	[Phone] [nvarchar](100) NULL,
	[Mobile] [nvarchar](100) NULL,
	[Fax] [nvarchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [varchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [varchar](50) NULL,
	[OrderNo] [int] NULL
	) ON [PRIMARY]

CREATE TABLE [dbo].[Department](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[OrderNo] [int] NULL,
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


CREATE TABLE [dbo].[Designation](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
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
	[AttendenceBonus] [decimal](18, 2) NULL,
	[EPZ] [decimal](18, 2) NULL,
	[Other] [decimal](18, 2) NULL,
	[DinnerAmount] [decimal](18, 2) NULL,
	[IfterAmount] [decimal](18, 2) NULL,
	[TiffinAmount] [decimal](18, 2) NULL,
	[ETiffinAmount] [decimal](18, 2) NULL,
	[OTAlloawance] [bit] NULL,
	[OTOrginal] [bit] NULL,
	[OTBayer] [bit] NULL,
	[ExtraOT] [bit] NULL,
	[PriorityLevel] [int] NULL,
	[OrderNo] [int] NULL,
	[DesignationGroupId] [nvarchar](20) NULL,
	[GradeId] [nvarchar](50) NULL,
	[HospitalPlanC1] [nvarchar](50) NULL,
	[HospitalPlanC2] [nvarchar](50) NULL,
	[HospitalPlanC3] [nvarchar](50) NULL,
	[HospitalPlanC4] [nvarchar](50) NULL,
	[HospitalPlanC5] [nvarchar](50) NULL,
	[DeathCoveragePlanC6] [nvarchar](50) NULL,
	[MaternityPlanC7] [nvarchar](50) NULL,
	[MaternityPlanC8] [nvarchar](50) NULL,
	[MaternityPlanC9] [nvarchar](50) NULL,
	[EntitlementC1] [nvarchar](50) NULL,
	[EntitlementC2] [nvarchar](50) NULL,
	[EntitlementC3] [nvarchar](50) NULL,
	[EntitlementC4] [nvarchar](50) NULL,
	[EntitlementC5] [nvarchar](50) NULL,
	[MobileExpenseC1] [nvarchar](50) NULL,
	[MobileExpenseC2] [nvarchar](50) NULL,
	[MobileExpenseC3] [nvarchar](50) NULL,
	[MobileExpenseC4] [nvarchar](50) NULL,
	[InternationalTravelC1] [nvarchar](50) NULL,
	[InternationalTravelC2] [nvarchar](50) NULL,
	[InternationalTravelC3] [nvarchar](50) NULL,
	[DomesticlTravelC1] [nvarchar](50) NULL,
	[DomesticTravelC2] [nvarchar](50) NULL,
	[DomesticTravelC3] [nvarchar](50) NULL,
	[DomesticTravelC4] [nvarchar](50) NULL,
	[DomesticTravelC5] [nvarchar](50) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[Section](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[OrderNo] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [varchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [varchar](50) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Department] [nvarchar](200) NULL,
	[Designation] [nvarchar](200) NULL,
	[Project] [nvarchar](200) NULL,
	[Section] [nvarchar](150) NULL,
	[DateOfBirth] [nvarchar](150) NULL,
	[JoinDate] [nvarchar](150) NULL,
	[ResignDate] [nvarchar](150) NULL,
	[Remarks] [nvarchar](150) NULL,
	[IsActive] [bit] NULL,
	[IsArchive] [bit] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedAt] [nvarchar](14) NULL,
	[CreatedFrom] [nvarchar](50) NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[PhotoName] [nvarchar](50) NULL,
	[NomineeDateofBirth] [nvarchar](14) NULL,
	[NomineeName] [varchar](500) NULL,
	[NomineeRelation] [nvarchar](200) NULL,
	[NomineeAddress] [varchar](500) NULL,
	[NomineeDistrict] [nvarchar](200) NULL,
	[NomineeDivision] [nvarchar](200) NULL,
	[NomineeCountry] [nvarchar](200) NULL,
	[NomineeCity] [nvarchar](200) NULL,
	[NomineePostalCode] [varchar](50) NULL,
	[NomineePhone] [nvarchar](100) NULL,
	[NomineeMobile] [nvarchar](100) NULL,
	[NomineeBirthCertificateNo] [nvarchar](50) NULL,
	[NomineeFax] [nvarchar](100) NULL,
	[NomineeFileName] [nchar](50) NULL,
	[NomineeRemarks] [nvarchar](500) NULL,
	[NomineeNID] [nvarchar](50) NULL,
	[GrossSalary] [decimal](18, 2) NULL,
	[BasicSalary] [decimal](18, 2) NULL,
	[LeftDate] [nvarchar](14) NULL,
	[Grade] [nvarchar](200) NULL,
	[Branch] [nvarchar](200) NULL,
	[ProjectId] [nvarchar](20) NULL,
	[SectionId] [nvarchar](20) NULL,
	[DepartmentId] [nvarchar](20) NULL,
	[DesignationId] [nvarchar](20) NULL,
	[Other1] [nvarchar](200) NULL,
	[EmployeeId] [nvarchar](20) NULL,
	[EmpName] [nvarchar](601) NULL,
	[Email] [nvarchar](50) NULL,
	[ContactNo] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[IsNoProfit] [bit] NULL,
	[BranchId] [nvarchar](50) NULL,
	[ResignReason] [nvarchar](50) NULL,
	[OfficialContactNo] [nvarchar](50) NULL,
	[EmployeeNID] [nvarchar](50) NULL,
	[EmployeeTIN] [nvarchar](50) NULL,
	[FathersName] [nvarchar](250) NULL,
	[MothersName] [nvarchar](250) NULL,
	[SpouseName] [nvarchar](250) NULL,
	[EmployeeBankAccountNumber] [nvarchar](50) NULL,
	[PresentAddress] [nvarchar](250) NULL,
	[ParmanentAdderss] [nvarchar](250) NULL,
	[NomineeBankAccountNumber] [nvarchar](50) NULL,
	[NomineeShare] [nvarchar](50) NULL,
	[EmployeeBankNameId] [int] NULL,
	[NomineeBankNameId] [int] NULL,
	[IsProfit] [bit] NULL,
	[IsNoInterest] [bit] NULL
	) ON [PRIMARY]




CREATE TABLE [dbo].[EmployeeLoan](
	[Id] [varchar](50) NOT NULL,
	[BranchId] [nvarchar](50) NOT NULL,
	[LoanType_E] [nvarchar](200) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[PrincipalAmount] [decimal](18, 3) NOT NULL,
	[IsFixed] [bit] NOT NULL,
	[InterestPolicy] [varchar](50) NULL,
	[InterestRate] [decimal](18, 3) NULL,
	[InterestAmount] [decimal](18, 3) NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[NumberOfInstallment] [int] NOT NULL,
	[ApprovedDate] [nvarchar](14) NULL,
	[StartDate] [nvarchar](14) NOT NULL,
	[EndDate] [nvarchar](14) NOT NULL,
	[ApplicationDate] [nvarchar](14) NULL,
	[IsApproved] [bit] NULL,
	[IsHold] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[RefundAmount] [decimal](18, 2) NULL,
	[RefundDate] [nvarchar](14) NULL,
	[PayrollProcessDate] [varchar](100) NULL,
	[IsEarlySellte] [bit] NULL,
	[EarlySellteDate] [varchar](100) NULL,
	[EarlySelltePrincipleAmount] [decimal](18, 2) NULL,
	[EarlySellteInterestAmount] [decimal](18, 2) NULL,
	[LoanNo] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeLoanDetail](
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
	[DuplicateID] [int] NULL,
	[InstallmentSLNo] [int] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EnumLoanType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[GLAccountCode] [varchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsInterest] [bit] NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL
) ON [PRIMARY]





CREATE TABLE [dbo].[COAGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupSL] [int] NULL,
	[Code] [varchar](500) NULL,
	[Name] [varchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[GroupType] [varchar](100) NULL,
	[ReportType] [varchar](100) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL,
	[COATypeOfReportId] [int] NULL,
	[COAGroupTypeId] [int] NULL,
	[GroupNature] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[COAs](
	[Id] [int] NOT NULL,
	[COASL] [int] NULL,
	[COAGroupId] [int] NULL,
	[Code] [varchar](500) NULL,
	[Name] [varchar](500) NULL,
	[Nature] [varchar](10) NULL,
	[COAType] [varchar](100) NULL,
	[ReportType] [varchar](100) NULL,
	[OpeningBalance] [decimal](18, 2) NULL,
	[TransType] [varchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsRetainedEarning] [bit] NULL,
	[IsNetProfit] [bit] NULL,
	[IsDepreciation] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[BranchId] [nvarchar](50) NULL
) ON [PRIMARY]





CREATE TABLE [dbo].[GLJournals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[JournalType] [int] NULL,
	[TransactionType] [int] NULL,
	[TransactionValue] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL,
	[IsYearClosing] [bit] NULL,
	[BranchId] [nvarchar](50) NULL,
	[SourceId] [int] NULL,
	[Source] [nvarchar](250) NULL,
	[IsApprove] [bit] NULL,
	[ApprovedBy] [nvarchar](50) NULL,
	[ApprovedAt] [nvarchar](50) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[GLJournalDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GLJournalId] [int] NULL,
	[COAId] [int] NULL,
	[TransactionDate] [int] NULL,
	[TransactionType] [int] NULL,
	[JournalType] [int] NULL,
	[IsDr] [bit] NULL,
	[DrAmount] [decimal](18, 3) NULL,
	[CrAmount] [decimal](18, 3) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransType] [varchar](100) NULL,
	[IsYearClosing] [bit] NULL
	) ON [PRIMARY]





CREATE TABLE [dbo].[TempNetChange](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RowSL] [int] NOT NULL,
	[AccountName] [nvarchar](500) NULL,
	[AccountCode] [nvarchar](500) NULL,
	[GroupName] [nvarchar](500) NULL,
	[GroupType] [nvarchar](500) NULL,
	[TypeOfReport] [nvarchar](500) NULL,
	[Nature] [nvarchar](2) NULL,
	[GLJournalDetailId] [int] NULL,
	[GLJournalId] [int] NULL,
	[Code] [nvarchar](500) NULL,
	[TransactionDate] [nvarchar](50) NULL,
	[JournalType] [nvarchar](50) NULL,
	[COAId] [int] NULL,
	[IsDr] [bit] NULL,
	[DrAmount] [decimal](18, 2) NULL,
	[CrAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransType] [nvarchar](10) NULL,
	[TransactionAmount] [decimal](18, 2) NULL,
	[NetChange] [decimal](18, 2) NULL,
	[IsRetainedEarning] [bit] NULL,
	[COAGroupId] [int] NULL,
	[COAGroupTypeId] [int] NULL,
	[COATypeOfReportId] [int] NULL,
	[COASL] [int] NULL,
	[GroupSL] [int] NULL,
	[GroupTypeSL] [int] NULL,
	[TypeOfReportSL] [int] NULL,
	[GroupTypeShortName] [nvarchar](100) NULL,
	[TypeOfReportShortName] [nvarchar](100) NULL,
	[BranchId] [nvarchar](50) NULL
	) ON [PRIMARY]








CREATE TABLE [dbo].[FiscalYearDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FiscalYearId] [nvarchar](20) NOT NULL,
	[Year] [int] NOT NULL,
	[PeriodName] [varchar](50) NULL,
	[PeriodStart] [nvarchar](20) NULL,
	[PeriodEnd] [nvarchar](20) NULL,
	[PeriodLock] [bit] NULL,
	[PayrollLock] [bit] NULL,
	[PFLock] [bit] NULL,
	[TAXLock] [bit] NULL,
	[LoanLock] [bit] NULL,
	[SagePostComplete] [bit] NULL,
	[Remarks] [nvarchar](500) NULL,
	[PeriodId] [int] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[GFEmployeeBreakMonth](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OpeningDate] [nvarchar](14) NULL,
	[EmployerContribution] [decimal](18, 2) NULL,
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


CREATE TABLE [dbo].[GFEmployeeOpeinig](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OpeningDate] [nvarchar](14) NULL,
	[EmployerContribution] [decimal](18, 2) NULL,
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


CREATE TABLE [dbo].[GFEmployeePayment](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[PaymentDate] [nvarchar](14) NULL,
	[EmployerContribution] [decimal](18, 2) NULL,
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
	[LastUpdateFrom] [nvarchar](50) NULL,
	[FiscalYearDetailId] [int] NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[GFEmployeeProvisions](
	[Id] [int] NOT NULL,
	[GFHeaderId] [int] NULL,
	[FiscalYearDetailId] [int] NULL,
	[EmployeeId] [nvarchar](20) NULL,
	[ProjectId] [nvarchar](20) NULL,
	[DepartmentId] [nvarchar](20) NULL,
	[SectionId] [nvarchar](20) NULL,
	[DesignationId] [nvarchar](20) NULL,
	[JoinDate] [nvarchar](14) NULL,
	[GrossAmount] [decimal](18, 2) NULL,
	[BasicAmount] [decimal](18, 2) NULL,
	[ProvisionAmount] [decimal](18, 2) NULL,
	[IncrementArrear] [decimal](18, 2) NULL,
	[GFPolicyId] [int] NULL,
	[MultipicationFactor] [decimal](18, 2) NULL,
	[JobMonth] [int] NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[Post] [bit] NULL,
	[FiscalYearDetailStartDate] [nvarchar](20) NULL,
	[IsBreakMonth] [bit] NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[GFProfitDistributionNew](
	[Id] [int] NOT NULL,
	[GFPreDistributionFundId] [nvarchar](200) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[DistributionDate] [nvarchar](14) NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[EmployerContribution] [decimal](18, 2) NULL,
	[EmployerProfit] [decimal](18, 2) NULL,
	[MultiplicationFactor] [decimal](18, 9) NULL,
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
	[LastUpdateFrom] [nvarchar](50) NULL
	) ON [PRIMARY]





CREATE TABLE [dbo].[ProfitDistributionNew](
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

CREATE TABLE [dbo].[EmployeeBreakMonthPF](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OpeningDate] [nvarchar](14) NULL,
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


CREATE TABLE [dbo].[EmployeePFPayment](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[PaymentDate] [nvarchar](14) NULL,
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
	[LastUpdateFrom] [nvarchar](50) NULL,
	[FiscalYearDetailId] [int] NULL,
	[BranchId] [nvarchar](10) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeePFOpeinig](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OpeningDate] [nvarchar](14) NULL,
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


CREATE TABLE [dbo].[PFDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PFHeaderId] [int] NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[PFStructureId] [nvarchar](20) NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[EmployeePFValue] [decimal](18, 2) NOT NULL,
	[EmployeerPFValue] [decimal](18, 2) NOT NULL,
	[Post] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[BasicSalary] [decimal](18, 2) NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[IsDistribute] [bit] NULL,
	[IsBankDeposited] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]





CREATE TABLE [dbo].[COATypeOfReport](
	[Id] [int] NOT NULL,
	[TypeOfReportSL] [int] NULL,
	[TypeOfReportShortName] [varchar](500) NULL,
	[Name] [varchar](500) NULL,
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


CREATE TABLE [dbo].[COAGroupType](
	[Id] [int] NOT NULL,
	[GroupTypeSL] [int] NULL,
	[GroupTypeShortName] [varchar](500) NULL,
	[COANature] [varchar](2) NULL,
	[Name] [varchar](500) NULL,
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












CREATE TABLE [dbo].[Asset](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
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
	[IsVehicle] [bit] NULL,
	[RegNo] [nvarchar](100) NULL,
	[EngineNo] [nvarchar](100) NULL,
	[ChassisNo] [nvarchar](100) NULL,
	[Model] [nvarchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[AutoJournalSetup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JournalFor] [varchar](500) NULL,
	[JournalName] [varchar](500) NULL,
	[Nature] [varchar](500) NULL,
	[GroupName] [nvarchar](500) NULL,
	[COAID] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[BranchId] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[Bank](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[BankBranchs](
	[Id] [int] NOT NULL,
	[BankId] [int] NOT NULL,
	[BranchName] [nvarchar](200) NOT NULL,
	[BranchAddress] [nvarchar](200) NOT NULL,
	[BankAccountType] [nvarchar](200) NOT NULL,
	[BankAccountNo] [nvarchar](200) NOT NULL,
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
	[TransType] [varchar](100) NULL,
	[BranchId] [varchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[BankCharge](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](20) NULL,
	[BankBranchId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NOT NULL,
	[TotalValue] [decimal](18, 2) NOT NULL,
	[Post] [bit] NOT NULL,
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


CREATE TABLE [dbo].[BankNames](
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
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL,
	[BranchId] [varchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[COAsBackup](
	[Id] [int] NOT NULL,
	[COASL] [int] NULL,
	[COAGroupId] [int] NULL,
	[Code] [varchar](500) NULL,
	[Name] [varchar](500) NULL,
	[Nature] [varchar](10) NULL,
	[OpeningBalance] [decimal](18, 2) NULL,
	[TransType] [varchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsRetainedEarning] [bit] NULL,
	[TransactionType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[COAType](
	[Id] [int] NOT NULL,
	[Name] [varchar](500) NULL,
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


CREATE TABLE [dbo].[CodeGenerations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CYear] [int] NULL,
	[TransactionTypeGroup] [nvarchar](50) NULL,
	[TransactionType] [nvarchar](50) NULL,
	[Prefix] [nvarchar](50) NULL,
	[LastNumber] [int] NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[Company](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Address] [varchar](500) NULL,
	[District] [nvarchar](200) NULL,
	[Division] [nvarchar](200) NULL,
	[Country] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[PostalCode] [varchar](50) NULL,
	[Phone] [nvarchar](100) NULL,
	[Mobile] [nvarchar](100) NULL,
	[Fax] [nvarchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[TaxId] [nvarchar](50) NULL,
	[RegistrationNumber] [nvarchar](50) NULL,
	[Mail] [nvarchar](50) NULL,
	[NumberOfEmployees] [int] NOT NULL,
	[YearStart] [nvarchar](20) NULL,
	[Year] [nvarchar](20) NULL,
	[VATNo] [nvarchar](20) NOT NULL,
	[LogoName] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[DesignationGroup](
	[Id] [nvarchar](20) NOT NULL,
	[Serial] [int] NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EEHeads](
	[Id] [int] NOT NULL,
	[Name] [varchar](120) NULL,
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


CREATE TABLE [dbo].[EETransactionDetails](
	[Id] [int] NOT NULL,
	[BranchId] [int] NULL,
	[SL] [int] NULL,
	[EETransactionId] [int] NOT NULL,
	[EEHeadId] [int] NULL,
	[TransactionDateTime] [nvarchar](14) NULL,
	[SubTotal] [decimal](25, 9) NULL,
	[ReferenceNo1] [varchar](50) NULL,
	[ReferenceNo2] [varchar](50) NULL,
	[ReferenceNo3] [varchar](50) NULL,
	[Post] [bit] NULL,
	[TransactionType] [varchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsPS] [bit] NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EETransactions](
	[Id] [int] NOT NULL,
	[BranchId] [int] NULL,
	[Code] [varchar](20) NOT NULL,
	[EEHeadId] [int] NULL,
	[TransactionDateTime] [nvarchar](14) NULL,
	[GrandTotal] [decimal](25, 9) NULL,
	[ReferenceNo1] [varchar](50) NULL,
	[ReferenceNo2] [varchar](50) NULL,
	[ReferenceNo3] [varchar](50) NULL,
	[Post] [bit] NULL,
	[TransactionType] [varchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsPS] [bit] NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeCompensatoryLeave](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[LeaveYear] [int] NOT NULL,
	[LeaveType_E] [nvarchar](200) NOT NULL,
	[FromDate] [nvarchar](14) NOT NULL,
	[ToDate] [nvarchar](14) NOT NULL,
	[TotalLeave] [decimal](18, 1) NOT NULL,
	[ApprovedBy] [nvarchar](20) NULL,
	[ApproveDate] [nvarchar](14) NULL,
	[IsApprove] [bit] NOT NULL,
	[RejectedBy] [nvarchar](20) NULL,
	[RejectDate] [nvarchar](14) NULL,
	[IsReject] [bit] NULL,
	[IsHalfDay] [bit] NOT NULL,
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


CREATE TABLE [dbo].[EmployeeFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[EmployeePersonalDetail_NIDFile] [nvarchar](150) NULL,
	[EmployeePersonalDetail_PassportFile] [nvarchar](150) NULL,
	[EmployeePersonalDetail_Fingerprint] [nvarchar](150) NULL,
	[EmployeePersonalDetail_VaccineFile1] [nvarchar](150) NULL,
	[EmployeePersonalDetail_VaccineFiles2] [nvarchar](150) NULL,
	[EmployeePersonalDetail_VaccineFile3] [nvarchar](150) NULL,
	[EmployeeInfo_PhotoName] [nvarchar](150) NULL,
	[EmployeePersonalDetail_DisabilityFile] [nvarchar](150) NULL,
	[EmployeePersonalDetail_Signature] [nvarchar](150) NULL,
	[EmployeeNominee_VaccineFile1] [nvarchar](150) NULL,
	[EmployeeNominee_VaccineFile2] [nvarchar](150) NULL,
	[EmployeeNominee_VaccineFile3] [nvarchar](150) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsArchive] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[EmployeePersonalDetail_TINFiles] [nvarchar](150) NULL,
	[SignatureFiles] [nvarchar](150) NULL,
	[FileName] [nvarchar](150) NULL,
	[Employeedependent_VaccineFile3] [nvarchar](150) NULL,
	[Employeedependent_VaccineFile2] [nvarchar](150) NULL,
	[Employeedependent_VaccineFile1] [nvarchar](150) NULL,
	[Extra_FileName] [nvarchar](150) NULL,
	[Experience_Certificate] [nvarchar](150) NULL,
	[Lng_Achivement] [nvarchar](150) NULL,
	[edu_Certificate] [nvarchar](150) NULL,
	[PassportVisa] [nvarchar](150) NULL,
	[BillVoucher] [nvarchar](150) NULL,
	[AssetFileName] [nvarchar](150) NULL,
	[Certificate] [nvarchar](150) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeForfeiture](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OpeningDate] [nvarchar](14) NULL,
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


CREATE TABLE [dbo].[EmployeeJob](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[JoinDate] [nvarchar](14) NOT NULL,
	[LeftDate] [nvarchar](14) NULL,
	[ProbationEnd] [nvarchar](14) NULL,
	[DateOfPermanent] [nvarchar](14) NULL,
	[EmploymentStatus_E] [nvarchar](200) NOT NULL,
	[EmploymentType_E] [nvarchar](200) NOT NULL,
	[Supervisor] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [varchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [varchar](50) NULL,
	[IsPermanent] [bit] NOT NULL,
	[StructureGroupId] [nvarchar](20) NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[BasicSalary] [decimal](18, 2) NOT NULL,
	[BankInfo] [nvarchar](500) NULL,
	[BankAccountNo] [nvarchar](50) NULL,
	[ProbationMonth] [int] NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[BankPayAmount] [decimal](18, 2) NULL,
	[Other1] [nvarchar](200) NULL,
	[Other2] [nvarchar](200) NULL,
	[Other3] [nvarchar](200) NULL,
	[Other4] [nvarchar](200) NULL,
	[Other5] [nvarchar](200) NULL,
	[AccountType] [nvarchar](50) NULL,
	[IsJobBefore] [bit] NULL,
	[FirstHoliday] [nvarchar](10) NULL,
	[SecondHoliday] [nvarchar](10) NULL,
	[Other1Id] [int] NULL,
	[Other2Id] [int] NULL,
	[Other3Id] [int] NULL,
	[Other4Id] [int] NULL,
	[Other5Id] [int] NULL,
	[ExtraOT] [bit] NULL,
	[OTBayer] [bit] NULL,
	[OTOrginal] [bit] NULL,
	[OTAlloawance] [bit] NULL,
	[AttendenceBonus] [decimal](18, 2) NULL,
	[GFStartFrom] [nvarchar](20) NULL,
	[BankAccountName] [nvarchar](200) NULL,
	[Routing_No] [nvarchar](200) NULL,
	[IsTAXApplicable] [bit] NULL,
	[IsCarTAXApplicable] [bit] NULL,
	[ExtendedProbationMonth] [nvarchar](50) NULL,
	[IsPFApplicable] [bit] NULL,
	[IsGFApplicable] [bit] NULL,
	[IsInactive] [bit] NULL,
	[FromDate] [nvarchar](14) NULL,
	[ToDate] [nvarchar](14) NULL,
	[ContrExDate] [nvarchar](50) NULL,
	[Extentionyn] [nvarchar](50) NULL,
	[secondExDate] [nvarchar](50) NULL,
	[fristExDate] [nvarchar](50) NULL,
	[RetirementDate] [nvarchar](50) NULL,
	[IsBuild] [bit] NULL,
	[EmpJobType] [nvarchar](50) NULL,
	[EmpCategory] [nvarchar](50) NULL,
	[Force] [nvarchar](50) NULL,
	[Rank] [nvarchar](50) NULL,
	[Duration] [nvarchar](50) NULL,
	[Retirement] [nvarchar](50) NULL,
	[DotedLineReport] [nvarchar](50) NULL,
	[IsSalalryProcess] [bit] NULL,
	[IsNoProfit] [bit] NULL,
	[IsRebate] [bit] NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeLeftInformation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NULL,
	[LeftType_E] [nvarchar](200) NULL,
	[EntryLeftDate] [nvarchar](14) NULL,
	[LeftDate] [nvarchar](14) NULL,
	[FileName] [nchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NULL,
	[IsArchive] [bit] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedAt] [nvarchar](14) NULL,
	[CreatedFrom] [nvarchar](50) NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsSalalryProcess] [bit] NULL,
	[BranchId] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeNominee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NULL,
	[DateofBirth] [nvarchar](14) NULL,
	[Name] [varchar](500) NULL,
	[Relation] [nvarchar](200) NULL,
	[Address] [varchar](500) NULL,
	[District] [nvarchar](200) NULL,
	[Division] [nvarchar](200) NULL,
	[Country] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[PostalCode] [varchar](50) NULL,
	[Phone] [nvarchar](100) NULL,
	[Mobile] [nvarchar](100) NULL,
	[BirthCertificateNo] [nvarchar](50) NULL,
	[Fax] [nvarchar](100) NULL,
	[FileName] [nchar](50) NULL,
	[Passport] [nvarchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NULL,
	[IsArchive] [bit] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedAt] [nvarchar](14) NULL,
	[CreatedFrom] [nvarchar](50) NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[NID] [nvarchar](50) NULL,
	[IsVaccineDose1Complete] [bit] NULL,
	[PostOffice] [nvarchar](50) NULL,
	[VaccineDose1Date] [nvarchar](14) NULL,
	[VaccineDose1Name] [nvarchar](50) NULL,
	[IsVaccineDose2Complete] [bit] NULL,
	[VaccineDose2Date] [nvarchar](14) NULL,
	[VaccineDose2Name] [nvarchar](50) NULL,
	[IsVaccineDose3Complete] [bit] NULL,
	[VaccineDose3Date] [nvarchar](14) NULL,
	[VaccineDose3Name] [nvarchar](50) NULL,
	[VaccineFile3] [nvarchar](150) NULL,
	[VaccineFiles2] [nvarchar](50) NULL,
	[VaccineFile1] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeePersonalDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OtherId] [nvarchar](20) NULL,
	[FatherName] [nvarchar](200) NULL,
	[MotherName] [nvarchar](200) NULL,
	[SpouseName] [nvarchar](200) NULL,
	[PersonalContactNo] [nvarchar](200) NULL,
	[CorporateContactNo] [nvarchar](200) NULL,
	[CorporateContactLimit] [decimal](18, 2) NULL,
	[Gender_E] [nvarchar](200) NULL,
	[MaritalStatus_E] [nvarchar](200) NULL,
	[Nationality_E] [nvarchar](200) NULL,
	[DateOfBirth] [nvarchar](14) NULL,
	[NickName] [nvarchar](200) NULL,
	[Smoker] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[NID] [nvarchar](50) NULL,
	[Signature] [nvarchar](50) NULL,
	[NIDFile] [nvarchar](50) NULL,
	[PassportNumber] [nvarchar](50) NULL,
	[ExpiryDate] [nvarchar](14) NULL,
	[Religion] [nvarchar](50) NULL,
	[TIN] [nvarchar](50) NULL,
	[IsDisable] [bit] NULL,
	[KindsOfDisability] [nvarchar](150) NULL,
	[DisabilityFile] [nvarchar](50) NULL,
	[PassportFile] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[BloodGroup_E] [nvarchar](20) NULL,
	[PlaceOfBirth] [nvarchar](100) NULL,
	[MarriageDate] [nvarchar](14) NULL,
	[SpouseProfession] [nvarchar](100) NULL,
	[SpouseDateOfBirth] [nvarchar](14) NULL,
	[SpouseBloodGroup] [nvarchar](20) NULL,
	[HRMSCode] [nvarchar](50) NULL,
	[WDCode] [nvarchar](50) NULL,
	[TPNCode] [nvarchar](50) NULL,
	[PersonalEmail] [nvarchar](50) NULL,
	[IsVaccineDose1Complete] [bit] NULL,
	[VaccineDose1Date] [nvarchar](14) NULL,
	[VaccineDose1Name] [nvarchar](50) NULL,
	[IsVaccineDose2Complete] [bit] NULL,
	[VaccineDose2Date] [nvarchar](14) NULL,
	[VaccineDose2Name] [nvarchar](50) NULL,
	[IsVaccineDose3Complete] [bit] NULL,
	[VaccineDose3Date] [nvarchar](14) NULL,
	[VaccineDose3Name] [nvarchar](50) NULL,
	[Fingerprint] [nvarchar](50) NULL,
	[FingerprintFile] [nvarchar](50) NULL,
	[TINFile] [nvarchar](50) NULL,
	[NoChildren] [nvarchar](50) NULL,
	[Heightft] [nvarchar](50) NULL,
	[HeightIn] [nvarchar](50) NULL,
	[Weight] [nvarchar](50) NULL,
	[ChestIn] [nvarchar](50) NULL,
	[VaccineFile3] [nvarchar](50) NULL,
	[VaccineFile2] [nvarchar](50) NULL,
	[VaccineFiles2] [nvarchar](50) NULL,
	[VaccineFile1] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeePresentAddress](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[Address] [varchar](500) NULL,
	[District] [nvarchar](200) NULL,
	[Division] [nvarchar](200) NULL,
	[Country] [nvarchar](200) NULL,
	[City] [nvarchar](200) NULL,
	[PostalCode] [varchar](50) NULL,
	[Phone] [nvarchar](100) NULL,
	[Mobile] [nvarchar](100) NULL,
	[Fax] [nvarchar](100) NULL,
	[Remarks] [nvarchar](500) NULL,
	[FileName] [nchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[PostOffice] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EmployeeProfessionalDegree](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[Degree_E] [nvarchar](200) NOT NULL,
	[Institute] [nvarchar](500) NULL,
	[YearOfPassing] [nvarchar](4) NULL,
	[IsLast] [bit] NULL,
	[FileName] [nchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[Marks] [numeric](18, 2) NULL,
	[TotalYear] [int] NULL,
	[Level] [nvarchar](50) NULL
	) ON [PRIMARY]

CREATE TABLE [dbo].[EmployeeTransfer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeCode] [nvarchar](20) NOT NULL,
	[FromBranch] [int] NOT NULL,
	[ToBranch] [int] NOT NULL,
	[TransferDate] [nvarchar](30) NOT NULL,
	[Remarks] [nvarchar](250) NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[CreatedAt] [nvarchar](50) NULL,
	[CreatedFrom] [nvarchar](50) NULL,
	[LastUpdateBy] [nvarchar](50) NULL,
	[LastUpdateAt] [nvarchar](50) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EnumCountry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsContact] [bit] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EnumDistrict](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Country_E] [nvarchar](200) NOT NULL,
	[Division_E] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EnumDivision](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Country_E] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EnumEMPCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EnumEmpJobType](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Type] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EnumEmploymentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EnumInvestmentTypes](
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
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EnumJournalFor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActice] [bit] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EnumJournalTransactionType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NULL,
	[NameTrim] [nvarchar](200) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[EnumJournalType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActice] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[EnumLeaveType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsWithoutPay] [bit] NULL,
	[LType] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsRegular] [bit] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[EnumLeftType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
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


CREATE TABLE [dbo].[EnumOderBy](
	[Id] [nvarchar](200) NOT NULL,
	[Module] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
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


CREATE TABLE [dbo].[EnumProfessionalDegree](
	[Id] [int] NOT NULL,
	[ProfessionalDegrees] [nvarchar](200) NULL,
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


CREATE TABLE [dbo].[EnumReport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportId] [varchar](50) NULL,
	[ReportName] [varchar](500) NULL,
	[ReportType] [varchar](500) NULL,
	[ReportFileName] [varchar](500) NULL,
	[IsVisible] [bit] NULL,
	[Remarks] [nvarchar](500) NULL,
	[ReportSL] [int] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[FiscalYear](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[Year] [int] NOT NULL,
	[FiscalYear] [nvarchar](50) NULL,
	[YearStart] [nvarchar](20) NULL,
	[YearEnd] [nvarchar](20) NULL,
	[YearLock] [bit] NULL,
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


CREATE TABLE [dbo].[ForfeitureAccounts](
	[Id] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[ForfeitDate] [nvarchar](14) NOT NULL,
	[ForfeitValue] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsTransferPDF] [bit] NULL,
	[Post] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[TotalForfeitValue] [decimal](18, 2) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[GFEmployeeForfeiture](
	[Id] [int] NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[OpeningDate] [nvarchar](14) NULL,
	[EmployerContribution] [decimal](18, 2) NULL,
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


CREATE TABLE [dbo].[GFEmployeeSettlements](
	[Id] [int] NOT NULL,
	[GFPolicyId] [int] NOT NULL,
	[PolicyJobDurationYearFrom] [int] NULL,
	[PolicyJobDurationYearTo] [int] NULL,
	[PolicyMultipicationFactor] [decimal](18, 2) NULL,
	[PolicyIsFixed] [bit] NULL,
	[PolicyLastBasicMultipication] [decimal](18, 2) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[JoinDate] [nvarchar](14) NOT NULL,
	[LeftDate] [nvarchar](14) NOT NULL,
	[TotalJobDurationYear] [int] NULL,
	[LastGross] [decimal](18, 2) NOT NULL,
	[LastBasic] [decimal](18, 2) NOT NULL,
	[SettlementDate] [nvarchar](14) NOT NULL,
	[GFValue] [decimal](18, 2) NOT NULL,
	[ServiceCharge] [decimal](18, 2) NOT NULL,
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


CREATE TABLE [dbo].[GFHeader](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[ProvisionAmount] [decimal](18, 2) NOT NULL,
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


CREATE TABLE [dbo].[GFPolicies](
	[Id] [int] NOT NULL,
	[PolicyName] [nvarchar](200) NOT NULL,
	[JobDurationYearFrom] [int] NULL,
	[JobDurationYearTo] [int] NULL,
	[MultipicationFactor] [decimal](18, 2) NULL,
	[IsFixed] [bit] NULL,
	[LastBasicMultipication] [decimal](18, 2) NOT NULL,
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

CREATE TABLE [dbo].[Grade](
	[Id] [nvarchar](20) NOT NULL,
	[SL] [int] NULL,
	[BranchId] [int] NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[MinSalary] [decimal](18, 2) NULL,
	[MaxSalary] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsHouseRentFactorFromBasic] [bit] NULL,
	[IsTAFactorFromBasic] [bit] NULL,
	[TAFactor] [bit] NULL,
	[IsMedicalFactorFromBasic] [bit] NULL,
	[Area] [nvarchar](20) NULL,
	[GradeNo] [int] NULL,
	[CurrentBasic] [decimal](18, 5) NULL,
	[BasicNextYearFactor] [decimal](18, 5) NULL,
	[BasicNextStepFactor] [decimal](18, 5) NULL,
	[HouseRentFactor] [decimal](18, 5) NULL,
	[MedicalFactor] [decimal](18, 5) NULL,
	[IsFixedHouseRent] [bit] NULL,
	[HouseRentAllowance] [decimal](18, 5) NULL,
	[IsFixedSpecialAllowance] [bit] NULL,
	[SpecialAllowance] [decimal](18, 5) NULL,
	[LowerLimit] [decimal](18, 5) NULL,
	[MedianLimit] [decimal](18, 5) NULL,
	[UpperLimit] [decimal](18, 5) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[InvestmentAccrued](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvestmentNameId] [int] NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NOT NULL,
	[ReferenceNo] [nvarchar](500) NOT NULL,
	[InvestmentValue] [decimal](18, 2) NOT NULL,
	[AccruedMonth] [int] NULL,
	[InterestRate] [decimal](18, 2) NOT NULL,
	[AccruedInterest] [decimal](18, 2) NOT NULL,
	[Post] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL,
	[AitInterest] [decimal](18, 2) NULL,
	[NetInterest] [decimal](18, 2) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[InvestmentDetails](
	[Id] [int] NOT NULL,
	[InvestmentId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[AccountId] [int] NOT NULL,
	[DebitAmount] [decimal](18, 2) NULL,
	[CreditAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[InvestmentNameDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvestmentNameId] [int] NOT NULL,
	[FromMonth] [nvarchar](14) NOT NULL,
	[ToMonth] [nvarchar](14) NOT NULL,
	[InterestRate] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


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
	[Code] [nvarchar](200) NULL,
	[InvestmentTypeId] [int] NULL,
	[InvestmentDate] [nvarchar](14) NULL,
	[FromDate] [nvarchar](14) NULL,
	[ToDate] [nvarchar](14) NULL,
	[MaturityDate] [nvarchar](14) NULL,
	[BankBranchId] [int] NULL,
	[BankNameId] [int] NULL,
	[FiscalYearDetailId] [int] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL,
	[AitInterest] [decimal](18, 2) NULL,
	[BranchId] [nvarchar](50) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[InvestmentRenew](
	[Id] [int] NOT NULL,
	[InvestmentId] [int] NOT NULL,
	[TransactionCode] [nvarchar](500) NULL,
	[InvestmentDate] [nvarchar](14) NOT NULL,
	[ReferenceNo] [nvarchar](500) NOT NULL,
	[FromDate] [nvarchar](14) NOT NULL,
	[ToDate] [nvarchar](14) NOT NULL,
	[MaturityDate] [nvarchar](14) NOT NULL,
	[InvestmentValue] [decimal](18, 2) NOT NULL,
	[BankCharge] [decimal](18, 2) NOT NULL,
	[BankExciseDuty] [decimal](18, 2) NULL,
	[Interest] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Post] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsEncashed] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL,
	[SourceTaxDeduct] [decimal](18, 2) NULL,
	[OtherCharge] [decimal](18, 2) NULL,
	[AditionAmount] [decimal](18, 2) NULL,
	[EncashAmount] [decimal](18, 2) NULL,
	[InterestRate] [decimal](18, 2) NULL,
	[AIT] [decimal](18, 2) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[Investments](
	[Id] [int] NOT NULL,
	[TransactionCode] [nvarchar](500) NULL,
	[InvestmentTypeId] [int] NULL,
	[TransactionType] [nvarchar](500) NULL,
	[ReferenceNo] [nvarchar](500) NULL,
	[InvestmentAddress] [nvarchar](500) NULL,
	[InvestmentDate] [nvarchar](14) NOT NULL,
	[FromDate] [nvarchar](14) NOT NULL,
	[ToDate] [nvarchar](14) NOT NULL,
	[MaturityDate] [nvarchar](14) NOT NULL,
	[InvestmentRate] [decimal](18, 2) NOT NULL,
	[InvestmentValue] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Post] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[InvestmentNameId] [int] NULL,
	[ReferenceId] [varchar](500) NULL,
	[IsEncashed] [bit] NULL,
	[TransType] [varchar](100) NULL,
	[BankCharge] [decimal](18, 2) NULL,
	[IsApprove] [bit] NULL,
	[ApprovedBy] [nvarchar](20) NULL,
	[ApprovedAt] [nvarchar](20) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[LeaveSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [varchar](100) NULL,
	[LeaveDate] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[Loan](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[Amount] [decimal](18, 2) NULL,
	[InterestAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[ReferenceNo] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[LoanMonthlyPayment](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[Amount] [decimal](18, 2) NULL,
	[InterestAmount] [decimal](18, 2) NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[ReferenceNo] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[LoanRepaymentToBank](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[Amount] [decimal](18, 2) NULL,
	[InterestAmount] [decimal](18, 2) NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[ReferenceNo] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[LoanSattlement](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[Amount] [decimal](18, 2) NULL,
	[InterestAmount] [decimal](18, 2) NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[ReferenceNo] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


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
	[RetainedEarning] [decimal](18, 4) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[NetProfitGFYearEnds1](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransType] [nvarchar](50) NULL,
	[Year] [varchar](50) NULL,
	[YearStart] [varchar](50) NULL,
	[YearEnd] [varchar](50) NULL,
	[COAId] [int] NULL,
	[COAType] [varchar](50) NULL,
	[TransactionAmount] [decimal](18, 4) NULL,
	[NetProfit] [decimal](18, 4) NULL,
	[RetainedEarning] [decimal](18, 4) NULL
	) ON [PRIMARY]


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
	[RetainedEarning] [decimal](18, 4) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[O_GLTransactionDetails](
	[Id] [int] NOT NULL,
	[TransactionCode] [nvarchar](50) NULL,
	[TransactionMasterId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[TransactionType] [varchar](50) NULL,
	[AccountId] [int] NOT NULL,
	[IsDr] [bit] NULL,
	[IsSingle] [bit] NULL,
	[DebitAmount] [decimal](25, 9) NULL,
	[CreditAmount] [decimal](25, 9) NULL,
	[TransactionAmount] [decimal](18, 2) NOT NULL,
	[DrAccountIdForCredit] [int] NULL,
	[CrAccountIdForDebit] [int] NULL,
	[Post] [bit] NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[PostedBy] [nvarchar](20) NULL,
	[PostedAt] [nvarchar](14) NULL,
	[PostedFrom] [nvarchar](50) NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[O_ROIDetails](
	[Id] [int] NOT NULL,
	[ROIId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[AccountId] [int] NOT NULL,
	[DebitAmount] [decimal](18, 2) NULL,
	[CreditAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[o_WithdrawDetails1](
	[Id] [int] NOT NULL,
	[WithdrawId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[AccountId] [int] NOT NULL,
	[DebitAmount] [decimal](18, 2) NULL,
	[CreditAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[o_WithdrawTypes](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](200) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[AccountType] [nvarchar](200) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[OProfitDistributionOfBankInterests](
	[Id] [int] NOT NULL,
	[ROBIId] [int] NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[DistributionDate] [nvarchar](14) NOT NULL,
	[TotalInterestValue] [decimal](18, 2) NOT NULL,
	[SelfInterestValue] [decimal](18, 2) NOT NULL,
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


CREATE TABLE [dbo].[OProfitDistributionOfInvestments](
	[Id] [int] NOT NULL,
	[ROIId] [int] NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[DistributionDate] [nvarchar](14) NOT NULL,
	[TotalProfitValue] [decimal](18, 2) NOT NULL,
	[SelfProfitValue] [decimal](18, 2) NOT NULL,
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


CREATE TABLE [dbo].[OProfitDistributionOfReservedfunds](
	[Id] [int] NOT NULL,
	[RFIId] [int] NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[DistributionDate] [nvarchar](14) NOT NULL,
	[TotalValue] [decimal](18, 2) NOT NULL,
	[SelfValue] [decimal](18, 2) NOT NULL,
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


CREATE TABLE [dbo].[PFBankDepositDetails](
	[Id] [int] NOT NULL,
	[PFBankDepositId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[AccountId] [int] NOT NULL,
	[DebitAmount] [decimal](18, 2) NULL,
	[CreditAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[PFBankDeposits](
	[Id] [int] NOT NULL,
	[Code] [varchar](50) NULL,
	[FiscalYearDetailId] [int] NULL,
	[DepositAmount] [decimal](18, 2) NULL,
	[TotalEmployeePFValue] [decimal](18, 2) NULL,
	[TotalEmployeerPFValue] [decimal](18, 2) NULL,
	[DepositDate] [nvarchar](14) NULL,
	[BankBranchId] [int] NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[ReferenceNo] [nvarchar](100) NULL,
	[TransactionMediaId] [nvarchar](200) NULL,
	[ReferenceId] [int] NULL,
	[TransactionType] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[PFHeader](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[EmployeePFValue] [decimal](18, 2) NOT NULL,
	[EmployeerPFValue] [decimal](18, 2) NOT NULL,
	[Post] [bit] NOT NULL,
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
	[TransType] [varchar](100) NULL,
	[BranchId] [nvarchar](50) NULL,
	[IsApprove] [bit] NULL,
	[ApproveBy] [nvarchar](50) NULL,
	[ApproveAt] [nvarchar](50) NULL
	) ON [PRIMARY]


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


CREATE TABLE [dbo].[PFSettlementDetails](
	[Id] [int] NOT NULL,
	[PFSettlementId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NULL,
	[AccountId] [int] NOT NULL,
	[DebitAmount] [decimal](18, 2) NULL,
	[CreditAmount] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[TransactionType] [nvarchar](100) NULL,
	[Post] [bit] NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[PFSettlements](
	[Id] [int] NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[TransactionCode] [nvarchar](50) NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[EmployeeProfitValue] [decimal](18, 2) NOT NULL,
	[EmployerProfitValue] [decimal](18, 2) NOT NULL,
	[EmployeeTotalContribution] [decimal](18, 2) NULL,
	[EmployerTotalContribution] [decimal](18, 2) NULL,
	[EmpDOJ] [nvarchar](20) NOT NULL,
	[EmpResignDate] [nvarchar](20) NOT NULL,
	[SettlementDate] [nvarchar](20) NOT NULL,
	[SettlementPolicyId] [int] NOT NULL,
	[JobAgeInMonth] [decimal](18, 2) NOT NULL,
	[EmployeeContributionRatio] [decimal](18, 2) NULL,
	[EmployerContributionRatio] [decimal](18, 2) NULL,
	[EmployeeProfitRatio] [decimal](18, 2) NULL,
	[EmployerProfitRatio] [decimal](18, 2) NULL,
	[EmployeeActualContribution] [decimal](18, 2) NULL,
	[EmployerActualContribution] [decimal](18, 2) NULL,
	[EmployeeActualProfitValue] [decimal](18, 2) NULL,
	[EmployerActualProfitValue] [decimal](18, 2) NULL,
	[Post] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[TotalPayableAmount] [decimal](18, 2) NULL,
	[AlreadyPaidAmount] [decimal](18, 2) NULL,
	[NetPayAmount] [decimal](18, 2) NULL,
	[PFStartDate] [nvarchar](14) NULL,
	[PFEndDate] [nvarchar](14) NULL,
	[EmployeeContributionForfeitValue] [decimal](18, 2) NULL,
	[EmployeeProfitForfeitValue] [decimal](18, 2) NULL,
	[EmployerContributionForfeitValue] [decimal](18, 2) NULL,
	[EmployerProfitForfeitValue] [decimal](18, 2) NULL,
	[TotalForfeitValue] [decimal](18, 2) NULL,
	[ProvidentFundAmount] [decimal](18, 2) NULL,
	[TransactionType] [nvarchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[PreDistributionFunds](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](500) NOT NULL,
	[TransactionDate] [nvarchar](14) NOT NULL,
	[TotalValue] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](500) NULL,
	[Post] [bit] NOT NULL,
	[IsDistribute] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL,
	[BranchId] [varchar](50) NULL,
	[IsApprove] [bit] NULL,
	[ApprovedBy] [nvarchar](20) NULL,
	[ApprovedAt] [nvarchar](20) NULL) ON [PRIMARY]


CREATE TABLE [dbo].[ProfitDistributionDetails](
	[Id] [int] NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[ProfitDistributionId] [int] NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[EmployeeProfitValue] [decimal](18, 2) NOT NULL,
	[EmployerProfitValue] [decimal](18, 2) NOT NULL,
	[EmployeeTotalContribution] [decimal](18, 2) NULL,
	[EmployerTotalContribution] [decimal](18, 2) NULL,
	[Post] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsPaid] [bit] NULL,
	[FiscalYearDetailIdTo] [int] NULL,
	[IndividualTotalContribution] [decimal](18, 2) NULL,
	[ServiceLengthMonthWeight] [decimal](18, 2) NULL,
	[IndividualWeightedContribution] [decimal](18, 2) NULL,
	[MultiplicationFactor] [decimal](18, 9) NULL,
	[IndividualProfitValue] [decimal](18, 2) NULL,
	[ServiceLengthMonth] [decimal](18, 2) NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


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


CREATE TABLE [dbo].[ProfitDistributions](
	[Id] [int] NOT NULL,
	[PFDetailFiscalYearDetailIds] [nvarchar](200) NULL,
	[PreDistributionFundIds] [nvarchar](200) NOT NULL,
	[DistributionDate] [nvarchar](14) NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[TotalEmployeeContribution] [decimal](18, 2) NULL,
	[TotalEmployerContribution] [decimal](18, 2) NULL,
	[TotalProfit] [decimal](18, 2) NOT NULL,
	[TransactionType] [varchar](50) NULL,
	[Remarks] [nvarchar](500) NULL,
	[Post] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsPaid] [bit] NULL,
	[FiscalYearDetailIdTo] [int] NULL,
	[TotalExpense] [decimal](18, 2) NULL,
	[AvailableDistributionAmount] [decimal](18, 2) NULL,
	[MultiplicationFactor] [decimal](18, 9) NULL,
	[TotalWeightedContribution] [decimal](18, 2) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[RawDataSummaryPF](
	[EmployeeId] [nvarchar](255) NULL,
	[Code] [nvarchar](255) NULL,
	[PFAmount] [decimal](18, 2) NULL,
	[PeriodName] [nvarchar](255) NULL,
	[FiscalYearDetailId] [int] NULL,
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[ReservedFunds](
	[Id] [int] NOT NULL,
	[ReservedDate] [nvarchar](14) NOT NULL,
	[ReservedValue] [decimal](18, 2) NOT NULL,
	[PDFId] [int] NULL,
	[Remarks] [nvarchar](500) NULL,
	[Post] [bit] NOT NULL,
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


CREATE TABLE [dbo].[ReturnOnBankInterests](
	[Id] [int] NOT NULL,
	[Code] [nvarchar](20) NULL,
	[BankBranchId] [int] NOT NULL,
	[TransactionDate] [nvarchar](14) NOT NULL,
	[TotalValue] [decimal](18, 2) NOT NULL,
	[Post] [bit] NOT NULL,
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
	[TransType] [varchar](100) NULL,
	[ActualInterestAmount] [decimal](18, 2) NULL,
	[ServiceChargeAmount] [decimal](18, 2) NULL,
	[IsBankDeposited] [bit] NULL) ON [PRIMARY]


CREATE TABLE [dbo].[ReturnOnInvestments](
	[Id] [int] NOT NULL,
	[TransactionCode] [nvarchar](500) NULL,
	[TransactionType] [nvarchar](500) NULL,
	[ReferenceId] [nvarchar](500) NULL,
	[InvestmentId] [int] NOT NULL,
	[InvestmentTypeId] [int] NULL,
	[ROIDate] [nvarchar](14) NOT NULL,
	[ROIRate] [decimal](18, 2) NOT NULL,
	[ROITotalValue] [decimal](18, 2) NOT NULL,
	[TotalInterestValue] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[Post] [bit] NOT NULL,
	[IsTransferPDF] [bit] NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsFixed] [bit] NULL,
	[ActualInterestAmount] [decimal](18, 2) NULL,
	[ServiceChargeAmount] [decimal](18, 2) NULL,
	[IsBankDeposited] [bit] NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[SalaryPFDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FiscalYearDetailId] [int] NOT NULL,
	[PFStructureId] [nvarchar](20) NOT NULL,
	[ProjectId] [nvarchar](20) NOT NULL,
	[DepartmentId] [nvarchar](20) NOT NULL,
	[SectionId] [nvarchar](20) NOT NULL,
	[DesignationId] [nvarchar](20) NOT NULL,
	[EmployeeId] [nvarchar](20) NOT NULL,
	[GradeId] [nvarchar](20) NULL,
	[PFValue] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[BasicSalary] [decimal](18, 2) NOT NULL,
	[GrossSalary] [decimal](18, 2) NOT NULL,
	[EmployeeStatus] [varchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[Setting](
	[Id] [varchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[SettingGroup] [varchar](120) NULL,
	[SettingName] [varchar](120) NULL,
	[SettingValue] [nvarchar](500) NULL,
	[SettingType] [varchar](120) NULL,
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


CREATE TABLE [dbo].[SettlementPolicies](
	[Id] [int] NOT NULL,
	[PolicyName] [nvarchar](500) NOT NULL,
	[JobAgeInMonth] [decimal](18, 2) NOT NULL,
	[EmployeeContributionRatio] [decimal](18, 2) NOT NULL,
	[EmployerContributionRatio] [decimal](18, 2) NULL,
	[EmployeeProfitRatio] [decimal](18, 2) NULL,
	[EmployerProfitRatio] [decimal](18, 2) NULL,
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


CREATE TABLE [dbo].[SymUserDefaultRoll](
	[Id] [nvarchar](20) NOT NULL,
	[BranchId] [int] NOT NULL,
	[symArea] [nvarchar](100) NULL,
	[symController] [nvarchar](100) NULL,
	[IsIndex] [bit] NOT NULL,
	[IsAdd] [bit] NOT NULL,
	[IsEdit] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[IsReport] [bit] NOT NULL,
	[IsProcess] [bit] NOT NULL,
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


CREATE TABLE [dbo].[SymUserRoll](
	[Id] [nvarchar](20) NOT NULL,
	[DefaultRollId] [nvarchar](20) NULL,
	[BranchId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[IsIndex] [bit] NOT NULL,
	[IsAdd] [bit] NOT NULL,
	[IsEdit] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[IsReport] [bit] NOT NULL,
	[IsProcess] [bit] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedAt] [nvarchar](14) NULL,
	[CreatedFrom] [nvarchar](50) NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[TempNetChangeNew](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransType] [nvarchar](50) NULL,
	[OperationType] [nvarchar](50) NULL,
	[COAId] [int] NULL,
	[TransactionAmount] [decimal](18, 4) NULL,
	[OpeningAmount] [decimal](18, 4) NULL,
	[NetChange] [decimal](18, 4) NULL,
	[ClosingAmount] [decimal](18, 4) NULL
	) ON [PRIMARY]


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
	[TransactionType] [varchar](100) NULL,
	[TransType] [varchar](100) NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[User](
	[Id] [nvarchar](50) NOT NULL,
	[GroupId] [int] NULL,
	[FullName] [nchar](100) NULL,
	[Email] [nchar](100) NULL,
	[LogId] [nchar](50) NOT NULL,
	[Password] [nchar](50) NOT NULL,
	[VerificationCode] [nchar](20) NULL,
	[BranchId] [int] NOT NULL,
	[EmployeeId] [nvarchar](50) NOT NULL,
	[IsAdmin] [bit] NULL,
	[IsActive] [bit] NULL,
	[IsVerified] [bit] NULL,
	[IsArchived] [bit] NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](20) NOT NULL,
	[CreatedFrom] [varchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [varchar](50) NULL,
	[IsApprove] [bit] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[UserGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](100) NULL,
	[IsSuper] [bit] NULL,
	[IsESS] [bit] NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[IsAdmin] [bit] NULL,
	[IsHRM] [bit] NULL,
	[IsAttendance] [bit] NULL,
	[IsPayroll] [bit] NULL,
	[IsTAX] [bit] NULL,
	[IsPF] [bit] NULL,
	[IsGF] [bit] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[UserRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BranchId] [int] NOT NULL,
	[UserInfoId] [nvarchar](128) NULL,
	[RoleInfoId] [nvarchar](128) NULL,
	[IsArchived] [bit] NOT NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[Withdraws](
	[Id] [int] NOT NULL,
	[IsInvested] [bit] NULL,
	[Code] [nvarchar](50) NULL,
	[WithdrawAmount] [decimal](18, 2) NULL,
	[WithdrawDate] [nvarchar](14) NULL,
	[BankBranchId] [int] NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsArchive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedAt] [nvarchar](14) NOT NULL,
	[CreatedFrom] [nvarchar](50) NOT NULL,
	[LastUpdateBy] [nvarchar](20) NULL,
	[LastUpdateAt] [nvarchar](14) NULL,
	[LastUpdateFrom] [nvarchar](50) NULL,
	[Post] [bit] NULL,
	[TransactionType] [nvarchar](100) NULL,
	[ReferenceNo] [nvarchar](100) NULL,
	[TransactionMediaId] [nvarchar](200) NULL,
	[TransactionTypeId] [int] NULL,
	[TransType] [varchar](100) NULL
	) ON [PRIMARY]


CREATE TABLE [dbo].[YearClosing](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransType] [nvarchar](10) NULL,
	[COAId] [int] NULL,
	[ClosingAmount] [decimal](18, 4) NULL,
	[FiscalYear] [nvarchar](4) NULL
	) ON [PRIMARY]

";
                #endregion Tables

                #region Default Data Insert
                // Default Data
                sqlText += @"
                


SET IDENTITY_INSERT [dbo].[AutoJournalSetup] ON 

INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (1, N'1', N'Contribution Employee', N'Cr', N'2', 18, 1, 0, N'admin', N'20250928', N'', N'admin', N'20260201105557', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (2, N'1', N'Contribution Employer', N'Cr', N'2', 17, 1, 0, N'admin', N'20250928', N'', N'admin', N'20260201105606', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (3, N'1', N'Receivable- Company', N'Dr', N'1', 10, 1, 0, N'admin', N'20250928', N'', N'admin', N'20260201105613', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (4, N'3', N'Profit Employee', N'Cr', N'9', 20, 1, 0, N'Admin', N'20251005121208', N'', N'admin', N'20260209115119', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (5, N'3', N'Profit Employer', N'Cr', N'9', 19, 1, 0, N'Admin', N'20251005121227', N'', N'admin', N'20260209115142', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (6, N'3', N'Income Summery', N'Dr', N'9', 349, 1, 0, N'Admin', N'20251005121243', N'', N'admin', N'20260201105850', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (7, N'6', N'Investment Accrued', N'Dr', N'1', 31, 1, 0, N'Admin', N'20251028133822', N'', N'admin', N'20260201105913', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (8, N'6', N'Investment Accrued', N'Cr', N'12', 16, 1, 0, N'Admin', N'20251028133932', N'', N'admin', N'20260201105933', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (9, N'4', N'Contribution Employer', N'Dr', N'1', 17, 1, 0, N'Admin', N'20251105222608', N'', N'admin', N'20260201110130', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (10, N'4', N'Contribution Employee', N'Dr', N'1', 18, 1, 0, N'Admin', N'20251105222646', N'', N'admin', N'20260201110138', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (11, N'4', N'Profit Employee', N'Dr', N'1', 20, 1, 0, N'Admin', N'20251105222731', N'', N'admin', N'20260201110153', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (12, N'4', N'Profit Employer', N'Dr', N'2', 19, 1, 0, N'Admin', N'20251105222750', N'', N'admin', N'20260209153530', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (13, N'4', N'PF Settlement', N'Cr', N'2', 357, 1, 0, N'Admin', N'20251105222826', N'', N'admin', N'20260209152012', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (14, N'2', N'Investment', N'Dr', N'2', 3, 1, 0, N'Admin', N'20251217122443', N'', NULL, NULL, NULL, N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (15, N'2', N'Investment', N'Cr', N'2', 21, 1, 0, N'Admin', N'20251217122521', N'', NULL, NULL, NULL, N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (18, N'1', N'Contribution Employee', N'Cr', N'2', 65, 1, 0, N'admin', N'20250928', N'', N'admin', N'20260201105502', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (19, N'1', N'Contribution Employer', N'Cr', N'2', 64, 1, 0, N'admin', N'20250928', N'', N'admin', N'20260201094914', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (20, N'1', N'Receivable- Company', N'Dr', N'1', 56, 1, 0, N'admin', N'20250928', N'', N'admin', N'20260201111408', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (21, N'3', N'Profit Employee', N'Cr', N'2', 67, 1, 0, N'Admin', N'20251005121208', N'', N'admin', N'20260201094942', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (22, N'3', N'Profit Employer', N'Cr', N'2', 66, 1, 0, N'Admin', N'20251005121227', N'', N'admin', N'20260201094958', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (23, N'3', N'Income Summery', N'Dr', N'9', 354, 1, 0, N'Admin', N'20251005121243', N'', N'admin', N'20260201095014', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (24, N'6', N'Investment Accrued', N'Dr', N'1', 78, 1, 0, N'Admin', N'20251028133822', N'', N'admin', N'20260201095055', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (25, N'6', N'Investment Accrued', N'Cr', N'12', 63, 1, 0, N'Admin', N'20251028133932', N'', N'admin', N'20260201095112', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (26, N'4', N'Contribution Employer', N'Dr', N'1', 64, 1, 0, N'Admin', N'20251105222608', N'', N'admin', N'20260201095634', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (27, N'4', N'Contribution Employee', N'Dr', N'1', 65, 1, 0, N'Admin', N'20251105222646', N'', N'admin', N'20260201095642', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (28, N'4', N'Profit Employee', N'Dr', N'1', 67, 1, 0, N'Admin', N'20251105222731', N'', N'admin', N'20260201095719', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (29, N'4', N'Profit Employer', N'Dr', N'1', 66, 1, 0, N'Admin', N'20251105222750', N'', N'admin', N'20260201095655', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (30, N'4', N'PF Settlement', N'Cr', N'12', 63, 1, 0, N'Admin', N'20251105222826', N'', N'admin', N'20260201095735', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (31, N'2', N'Investment', N'Dr', N'1', 366, 1, 0, N'Admin', N'20251217122443', N'', N'admin', N'20260210155637', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (32, N'2', N'Investment', N'Cr', N'2', 63, 1, 0, N'Admin', N'20251217122521', N'', N'admin', N'20260210155715', N'', N'15')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (33, N'2', N'Investment Journal', N'Dr', N'1', 16, 1, 0, N'admin', N'20260405132615', N'', N'admin', N'20260405133937', N'', N'10')
INSERT [dbo].[AutoJournalSetup] ([Id], [JournalFor], [JournalName], [Nature], [GroupName], [COAID], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [BranchId]) VALUES (34, N'2', N'Investment Journal', N'Cr', N'1', 15, 1, 0, N'admin', N'20260405132704', N'', NULL, NULL, NULL, N'10')
SET IDENTITY_INSERT [dbo].[AutoJournalSetup] OFF

INSERT [dbo].[Bank] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_10', 1, N'IBBL', N'Islami Bank Bangladesh Ltd.', N'Testing', 1, 0, N'admin', N'20260113171154', N'', NULL, NULL, NULL)

INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (1, 1, N'Gulshan Branch ', N'Gulshan Dhaka', N'Savings', N'111123232300', NULL, 0, 1, N'Admin', N'20250322095119', N'', N'Admin', N'20250908152629', N'', N'PF', N'PF', N'10')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (2, 2, N'Tejgaon', N'MTB Square, 210/A/1 Tejgaon Industrial Area, Tejgaon, Dhaka 1215', N'SND', N'1310000152563', N'Agami Fashions Limited Employees (Contributory) Provident Fund


', 1, 0, N'Admin', N'20250908152925', N'', N'Admin', N'20250908181215', N'', NULL, N'PF', N'14')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (3, 3, N'Tejgaon', N'MTB Square, 210/A/1 Tejgaon Industrial Area, Tejgaon, Dhaka 1215
', N'SND', N'1310000152563', N'Agami Fashions Limited Employees (Contributory) Provident Fund
', 1, 0, N'Admin', N'20250908180753', N'', NULL, NULL, NULL, NULL, N'PF', N'10')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (10, 12, N'IPDC-HO', N'Dhaka', N'FDR', N'1001251000060483', NULL, 1, 0, N'admin', N'20260209165602', N'', N'admin', N'20260209165724', N'', NULL, N'PF', N'15')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (6, 3, N'Tejgaon', N'gg', N'SND', N'1310000152545', NULL, 1, 0, N'Admin', N'20250908181420', N'', NULL, NULL, NULL, NULL, N'PF', N'15')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (8, 10, N'Dhaka', N'Dhaka', N'FDR', N'1001251000060476', NULL, 1, 0, N'Admin', N'20251020130901', N'', NULL, NULL, NULL, NULL, N'PF', N'10')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (9, 11, N'Panthapath', N'Tridhara Tower', N'FDR', N'@#$%^&*(ERTYUIO$R%', NULL, 0, 1, N'admin', N'20260115152358', N'', N'admin', N'20260208122703', N'', NULL, N'PF', N'10')
INSERT [dbo].[BankBranchs] ([Id], [BankId], [BranchName], [BranchAddress], [BankAccountType], [BankAccountNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (7, 4, N'Gulshan Branch ', N'Gulshan Branch ', N'Savings', N'111123232300', NULL, 1, 0, N'Admin', N'20250908183908', N'', NULL, NULL, NULL, NULL, N'PF', N'15')

INSERT [dbo].[BankCharge] ([Id], [Code], [BankBranchId], [TransactionDate], [TotalValue], [Post], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (1, N'BKC-0002/102025', 6, N'20251008', CAST(1000.00 AS Decimal(18, 2)), 0, NULL, 1, 0, N'Admin', N'20251030163924', N'', NULL, NULL, NULL, NULL, N'PF')
INSERT [dbo].[BankCharge] ([Id], [Code], [BankBranchId], [TransactionDate], [TotalValue], [Post], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (2, N'BKC-0003/012026', 9, N'20260119', CAST(0.00 AS Decimal(18, 2)), 0, NULL, 1, 0, N'admin', N'20260119114924', N'', NULL, NULL, NULL, NULL, N'PF')


INSERT [dbo].[BankNames] ([Id], [Name], [Address], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType], [BranchId]) VALUES (10, N'IPDC Finance PLC.', N'Dhaka', NULL, 1, 0, N'Admin', N'20251020130711', N'', NULL, NULL, NULL, NULL, N'PF', N'10')

SET IDENTITY_INSERT [dbo].[Branch] ON 

INSERT [dbo].[Branch] ([Id], [CompanyId], [Code], [Name], [Address], [District], [Division], [Country], [City], [PostalCode], [Phone], [Mobile], [Fax], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (10, 1, N'B1003', N'SSL', NULL, NULL, NULL, NULL, N'Dhaka', N'1207', N'02-9611894-5', N'2154251', N'02-9673916', N'A', 1, 0, N'Admin', N'20250322145032', N'', N'Admin', N'20250907113441', N'')
SET IDENTITY_INSERT [dbo].[Branch] OFF

SET IDENTITY_INSERT [dbo].[COAGroups] ON 

INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (1, 0, N'10200', N'Current Asset', NULL, 1, 0, N'-', N'-', N'-', N'Admin', N'20250325140149', N'', N'Asset', N'BS', N'PF', N'PF', 1, 1, N'Dr')
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (2, 2, N'10100', N'Members Fund and Liabilities', NULL, 1, 0, N'-', N'-', N'-', N'Admin', N'20250908183059', N'', N'Asset', N'BS', N'PF', N'PF', 1, 1, N'Dr')
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (3, 4, N'20100', N'Non-Current Assest', NULL, 1, 0, N'-', N'-', N'-', N'-', N'-', N'-', N'Members Fund and Liabilities', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (5, 5, NULL, N'Expense', NULL, 1, 0, N'-', N'-', N'-', N'Admin', N'20250908183109', N'', N'Members Fund and Liabilities', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (7, 7, NULL, N'Revenue', NULL, 1, 0, N'-', N'-', N'-', N'Admin', N'20250908183130', N'', N'Revenue', N'IS', N'PF', N'PF', 1, 1, N'Cr')
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (8, 8, NULL, N'Non-Operating Income', NULL, 1, 0, N'-', N'-', N'-', N'Admin', N'20250908183220', N'', N'Revenue', N'IS', N'PF', N'PF', 1, 1, N'Cr')
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (9, 9999, NULL, N'Retained Earnings', NULL, 0, 0, N'-', N'-', N'-', N'ADMIN', N'20240815173541', N'', N'RetainedEarnings', N'RetainedEarnings', N'PF', N'PF', 1, 1, N'Dr')
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (10, 4, N'20300', N'Members Funds', NULL, 1, 0, N'-', N'-', N'-', N'Admin', N'20250908183744', N'', N'Members'' Fund and Liabilities', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (12, 7, N'50000', N'Non-Current Liabilities', NULL, 1, 0, N'-', N'-', N'-', N'-', N'-', N'-', N'Expense', N'IS', N'PF', N'PF', 1, 1, N'Cr')
INSERT [dbo].[COAGroups] ([Id], [GroupSL], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [GroupType], [ReportType], [TransactionType], [TransType], [COATypeOfReportId], [COAGroupTypeId], [GroupNature]) VALUES (13, 100, NULL, N'Testing', NULL, 1, 0, N'admin', N'20260118093532', N'', N'admin', N'20260118093545', N'', NULL, NULL, NULL, N'PF', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[COAGroups] OFF

INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (1, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127152849', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (2, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127152921', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (3, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127152932', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (4, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127152944', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (5, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127152955', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (6, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153006', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (7, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153015', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (8, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153041', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (9, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153101', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (10, 1, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260124170249', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (11, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153126', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (12, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153145', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (13, 0, 1, N'10205101', N'AFL-IPDC-HO-FDR-1001251000060476', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127151210', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (14, 0, 1, N'10205102', N'AFL-IPDC-HO-FDR-1001251000063711', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127151356', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (15, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153215', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (16, 0, 1, N'10208101', N'AFL-MTBPLC-TEJ-SND-1310000152563', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127152832', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (17, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127155006', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (18, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127154947', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (19, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260210155534', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (20, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260209144454', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (21, 0, 12, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153257', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (22, 0, 2, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127151457', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (23, 0, 12, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153330', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (24, 0, 2, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127151525', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (25, 0, 12, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153416', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (26, 0, 12, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153539', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (27, 0, 12, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153555', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (28, 0, 12, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153525', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (29, 0, 7, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153753', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (30, 0, 7, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153811', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (31, 0, 7, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153822', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (32, 0, 7, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153831', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (33, 0, 7, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153855', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (34, 0, 7, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153906', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (35, 0, 7, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153920', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (36, 0, 7, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153935', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (37, 0, 7, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127153952', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (38, 0, 12, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127154725', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (39, 0, 12, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127154715', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (40, 0, 12, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127154622', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (41, 0, 12, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127154657', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (42, 0, 12, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260127154643', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (358, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (46, 0, 1, N'1400001', N'Receivable Company', N'Dr', N'Asset', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'20260128151355', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (47, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (48, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (49, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (50, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (51, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (52, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (53, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (54, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (55, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (56, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (57, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (58, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (59, 0, 1, N'10205101', N'AWL-IPDC-HO-FDR-1001251000060483', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (60, 0, 1, N'10205102', N'AWL-IPDC-HO-FDR-1001251000063707', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (61, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (62, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (63, 0, 1, N'10208101', N'AWL-MTBPLC-TEJ-SND-1310000152545', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (64, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (65, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (66, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (67, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (68, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (69, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (70, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (71, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (72, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (73, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (74, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (75, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (76, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (77, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (78, 0, 7, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'20260216112948', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (79, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (80, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (81, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (82, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (83, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (84, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (85, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (86, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (87, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (88, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (89, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (90, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (91, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (92, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (93, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (94, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (95, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (96, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (97, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (98, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (99, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (100, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (101, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (102, 0, 1, N'10205101', N'DFL-IPDC-HO-FDR-1001251000060443', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')

INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (103, 0, 1, N'10205102', N'DFL-IPDC-HO-FDR-1001251000063701', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (104, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (105, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (106, 0, 1, N'10208101', N'DFL-MTBPLC-TEJ-SND-1310000152527', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (107, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (108, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (109, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (110, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (111, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (112, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (113, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (114, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (115, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (116, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (117, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (118, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (119, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (120, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (121, 0, 4, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (122, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (123, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (124, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (125, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (126, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (127, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (128, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (129, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (130, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (131, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (132, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (133, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (134, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (135, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (136, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (137, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (138, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (139, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (140, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (141, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (142, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (143, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (144, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (145, 0, 1, N'10205101', N'DITECH-IPDC-HO-FDR-1001251000060467', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (146, 0, 1, N'10205102', N'DITECH-IPDC-HO-FDR-1001251000063700', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (147, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (148, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (149, 0, 1, N'10208101', N'DITECH-MTBPLC-TEJ-SND-1310000152572', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (150, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (151, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (152, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (153, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (154, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (155, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (156, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (157, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (158, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (159, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (160, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (161, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (162, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (163, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (164, 0, 4, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (165, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (166, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (167, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (168, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (169, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (170, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (171, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (172, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (173, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (174, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (175, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (176, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (177, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (178, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (179, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (180, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (181, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (182, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (183, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (184, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (185, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (186, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (187, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (188, 0, 1, N'10205101', N'DIVC-IPDC-HO-FDR-1001251000060452', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (189, 0, 1, N'10205102', N'DIVC-IPDC-HO-FDR-1001251000063703', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (190, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (191, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (192, 0, 1, N'10208101', N'DIVC-MTBPLC-TEJ-SND-1310000152554', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (193, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (194, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (195, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (196, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (197, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (198, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (199, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (200, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (201, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (202, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')

INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (203, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (204, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (205, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (206, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (207, 0, 4, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (208, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (209, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (210, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (211, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (212, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (213, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (214, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (215, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (216, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (217, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (218, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (219, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (220, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (221, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (222, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (223, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (224, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (225, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (226, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (227, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (228, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (229, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (230, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (231, 0, 1, N'10205101', N'DRL-IPDC-HO-FDR-1001251000060460', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (232, 0, 1, N'10205102', N'DRL-IPDC-HO-FDR-1001251000063702', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (233, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (234, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (235, 0, 1, N'10208101', N'DRL-MTBPLC-TEJ-SND-1310000172685', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (236, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (237, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (238, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (239, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (240, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (241, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (242, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (243, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (244, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (245, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (246, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (247, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (248, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (249, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (250, 0, 4, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (251, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (252, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (253, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (254, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (255, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (256, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (257, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (258, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (259, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (260, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (261, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (262, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (263, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (264, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (265, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (266, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (267, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (268, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (269, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (270, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (271, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (272, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (273, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (274, 0, 1, N'10205101', N'DGL-IPDC-HO-FDR-1001251000060439', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (275, 0, 1, N'10205102', N'DGL-IPDC-HO-FDR-1001251000063712', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (276, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (277, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (278, 0, 1, N'10208101', N'DGL-MTBPLC-TEJ-SND-1310000152474', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (279, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (280, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (281, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (282, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (283, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (284, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (285, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (286, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (287, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (288, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (289, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (290, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (291, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (292, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (293, 0, 4, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (294, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (295, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (296, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (297, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (298, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (299, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (300, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (301, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (302, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')

INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (303, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (304, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (305, 0, 1, N'10201101', N'AIT- Corporate', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (306, 0, 1, N'10201102', N'AIT- Bond', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (307, 0, 1, N'10201103', N'AIT- Dividend', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (308, 0, 1, N'10201104', N'AIT- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (309, 0, 1, N'10201105', N'AIT- Debentures', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (310, 0, 1, N'10201106', N'AIT- Securites', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (311, 0, 1, N'10201107', N'AIT- Bank Interest', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (312, 0, 1, N'10202101', N'Advance to Employee', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (313, 0, 1, N'10203101', N'Receivable- Others ', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (314, 0, 1, N'10203102', N'Receivable- Company', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (315, 0, 1, N'10204101', N'Interest Receivable- FDR', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (316, 0, 1, N'10204102', N'Interest Receivable- Securities', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (317, 0, 1, N'10205101', N'GGL-OBPLC-KarwanBazar-FDR-0124130003965', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (318, 0, 1, N'10205102', N'GGL-IPDC-HO-FDR-1001251000060554', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (319, 0, 1, N'10205103', N'GGL-IPDC-HO-FDR-1001251000063706', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (320, 0, 1, N'10206101', N'', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (321, 0, 1, N'10207101', N'Petty Cash-HO', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (322, 0, 1, N'10208101', N'GGL-MTBPLC-TEJ-SND-1310000152509', N'Dr', N'Asset', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (323, 0, 2, N'20101101', N'Employers'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (324, 0, 2, N'20101102', N'Employees'' Contribution', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (325, 0, 2, N'20101103', N'Employers'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (326, 0, 2, N'20101104', N'Employees'' Profit', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (327, 0, 3, N'30101101', N'Deferred Tax Liability', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (328, 0, 3, N'30201101', N'Others Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (329, 0, 3, N'30201102', N'Payable to Outgoing Member', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (330, 0, 3, N'30201103', N'Payable to Company', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (331, 0, 3, N'30202101', N'Income Tax Payable', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (332, 0, 3, N'30203101', N'Provision for Finance Expenses', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (333, 0, 3, N'30203102', N'Provision for Bad Debts', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (334, 0, 3, N'30203103', N'Provision for Income Tax', N'Cr', N'Members Fund and Liabilities', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (335, 0, 4, N'40101101', N'Interest Income on Bond', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (336, 0, 4, N'40101102', N'Dividend Income on Shares', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (337, 0, 4, N'40101103', N'Interest Income on FDR', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (338, 0, 4, N'40101104', N'Interest Income on Debentures', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (339, 0, 4, N'40101105', N'Interest Income on Securities', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (340, 0, 4, N'40101106', N'Interest Income on SND', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (341, 0, 4, N'40101107', N'Interest Income on Bank Deposits', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (342, 0, 4, N'40101108', N'Interest on Employees'' Loan', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (343, 0, 4, N'40201101', N'Miscellaneous Income-Others', N'Cr', N'Revenue', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (344, 0, 5, N'50101101', N'Bad Debts Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (345, 0, 5, N'50102101', N'Miscellaneous Expenses', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (346, 0, 5, N'50103101', N'Bank Charge', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (347, 0, 5, N'50104101', N'Foreign Currency Fluctuation Loss/(Gain)', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (348, 0, 5, N'50105101', N'Income Taxes Expenses-Corporate', N'Dr', N'Expense', N'BS', CAST(0.00 AS Decimal(18, 2)), N'PF', N'NA', 1, 0, N'Admin', N'19000101', N'Local', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (349, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (350, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (351, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (352, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (353, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (354, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (355, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (356, 0, 9, N'99999', N'Income Summery', N'Cr', N'OwnersEquity', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02601E+13', N'', N'admin', N'2.02601E+13', N'', 0, 0, 0, N'PF', N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (357, 0, 2, N'30201104', N'Settlement Account ', N'Cr', N'Members Fund and Liabilities
', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'20260209151918', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'10')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (359, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'11')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (360, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'12')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (361, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'13')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (362, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'14')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (363, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'15')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (364, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'16')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (365, 0, 2, N'30201104', N'Settlement Account', N'Cr', N'Members Fund and Liabilities', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'2.02602E+13', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'17')
INSERT [dbo].[COAs] ([Id], [COASL], [COAGroupId], [Code], [Name], [Nature], [COAType], [ReportType], [OpeningBalance], [TransType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsRetainedEarning], [IsNetProfit], [IsDepreciation], [TransactionType], [BranchId]) VALUES (366, 0, 1, N'10205108', N'AWL-IPDC-HO-FDR-1001251000060483', N'Dr', N'Asset', NULL, NULL, N'PF', NULL, 1, 0, N'admin', N'20260210155408', N'', NULL, NULL, NULL, 0, 0, 0, NULL, N'15')


INSERT [dbo].[COAType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (1, N'Asset', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')
INSERT [dbo].[COAType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (2, N'Members Fund and Liabilities
', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')
INSERT [dbo].[COAType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (3, N'OwnersEquity', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')
INSERT [dbo].[COAType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (4, N'Revenue', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')
INSERT [dbo].[COAType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (5, N'Expense', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')

INSERT [dbo].[COATypeOfReport] ([Id], [TypeOfReportSL], [TypeOfReportShortName], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (1, 100, N'BS', N'Balance Sheet', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')
INSERT [dbo].[COATypeOfReport] ([Id], [TypeOfReportSL], [TypeOfReportShortName], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (2, 500, N'IS', N'Income Statement', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')
INSERT [dbo].[COATypeOfReport] ([Id], [TypeOfReportSL], [TypeOfReportShortName], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (3, 300, N'RE', N'Retained Earning', N'', 1, 0, N'Admin', N'19000101', N'Local', NULL, NULL, NULL, N'PF', N'PF')

SET IDENTITY_INSERT [dbo].[CodeGenerations] ON 

INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (1, 2026, N'PF', N'PFContribution', N'PFC', 146, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (2, 2026, N'PF', N'PFContribution', N'PFC', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (3, 2026, N'PF', N'BankDeposit', N'BKD', 4, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (4, 2025, N'PF', N'BankWithdraw', N'BKW', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (5, 2025, N'PF', N'ReturnOnBankInterest', N'RBI', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (6, 2025, N'PF', N'BankCharge', N'BKC', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (7, 2025, N'PF', N'Investment', N'INV', 7, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (8, 2025, N'PF', N'InvestmentRenew', N'INR', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (9, 2025, N'PF', N'Loan', N'LON', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (10, 2025, N'PF', N'MonthlyLoanPayment', N'MLP', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (11, 2025, N'PF', N'LoanRepaymentToBank', N'LPB', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (12, 2025, N'PF', N'EarlySattlement', N'LES', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (13, 2025, N'PF', N'Forfeiture', N'FFR', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (14, 2025, N'PF', N'JournalVoucher', N'JV', 143, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (15, 2025, N'PF', N'PaymentVoucher', N'PV', 15, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (16, 2025, N'PF', N'ReceiptVoucher', N'RV', 38, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (17, 2025, N'PF', N'PreDistributionFund', N'PDF', 10, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (18, 2025, N'GF', N'GFContribution', N'GFC', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (19, 2025, N'GF', N'BankDeposit', N'BKD', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (20, 2025, N'GF', N'BankWithdraw', N'BKW', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (21, 2025, N'GF', N'ReturnOnBankInterest', N'RBI', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (22, 2025, N'GF', N'BankCharge', N'BKC', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (23, 2025, N'GF', N'Investment', N'INV', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (24, 2025, N'GF', N'InvestmentRenew', N'INR', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (25, 2025, N'GF', N'Loan', N'LON', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (26, 2025, N'GF', N'MonthlyLoanPayment', N'MLP', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (27, 2025, N'GF', N'LoanRepaymentToBank', N'LPB', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (28, 2025, N'GF', N'EarlySattlement', N'LES', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (29, 2025, N'GF', N'Forfeiture', N'FFR', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (30, 2025, N'GF', N'JournalVoucher', N'JV', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (31, 2025, N'GF', N'PaymentVoucher', N'PV', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (32, 2025, N'GF', N'ReceiptVoucher', N'RV', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (33, 2025, N'GF', N'PreDistributionFund', N'PDF', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (42, 2026, N'PF', N'PFContribution', N'PFC', 30, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (43, 2026, N'PF', N'PFContribution', N'PFC', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (44, 2026, N'PF', N'BankDeposit', N'BKD', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (45, 2026, N'PF', N'BankWithdraw', N'BKW', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (46, 2026, N'PF', N'ReturnOnBankInterest', N'RBI', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (47, 2026, N'PF', N'BankCharge', N'BKC', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (48, 2026, N'PF', N'Investment', N'INV', 7, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (49, 2026, N'PF', N'InvestmentRenew', N'INR', 8, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (50, 2026, N'PF', N'Loan', N'LON', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (51, 2026, N'PF', N'MonthlyLoanPayment', N'MLP', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (52, 2026, N'PF', N'LoanRepaymentToBank', N'LPB', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (53, 2026, N'PF', N'EarlySattlement', N'LES', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (54, 2026, N'PF', N'Forfeiture', N'FFR', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (55, 2026, N'PF', N'JournalVoucher', N'JV', 39, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (56, 2026, N'PF', N'PaymentVoucher', N'PV', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (57, 2026, N'PF', N'ReceiptVoucher', N'RV', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (58, 2026, N'PF', N'PreDistributionFund', N'PDF', 11, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (59, 2026, N'GF', N'GFContribution', N'GFC', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (60, 2026, N'GF', N'BankDeposit', N'BKD', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (61, 2026, N'GF', N'BankWithdraw', N'BKW', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (62, 2026, N'GF', N'ReturnOnBankInterest', N'RBI', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (63, 2026, N'GF', N'BankCharge', N'BKC', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (64, 2026, N'GF', N'Investment', N'INV', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (65, 2026, N'GF', N'InvestmentRenew', N'INR', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (66, 2026, N'GF', N'Loan', N'LON', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (67, 2026, N'GF', N'MonthlyLoanPayment', N'MLP', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (68, 2026, N'GF', N'LoanRepaymentToBank', N'LPB', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (69, 2026, N'GF', N'EarlySattlement', N'LES', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (70, 2026, N'GF', N'Forfeiture', N'FFR', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (71, 2026, N'GF', N'JournalVoucher', N'JV', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (72, 2026, N'GF', N'PaymentVoucher', N'PV', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (73, 2026, N'GF', N'ReceiptVoucher', N'RV', 1, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (74, 2026, N'GF', N'PreDistributionFund', N'PDF', 0, N'GF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (98, 0, N'PF', N'JournalVoucher', N'JV', 3, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (99, 0, N'PF', N'JournalVoucher', N'JV', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (100, 0, N'PF', N'JournalVoucher', N'JV', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (101, 0, N'PF', N'JournalVoucher', N'JV', 0, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (102, 2027, N'PF', N'JournalVoucher', N'JV', 1, N'PF')
INSERT [dbo].[CodeGenerations] ([Id], [CYear], [TransactionTypeGroup], [TransactionType], [Prefix], [LastNumber], [TransType]) VALUES (109, 2027, N'PF', N'JournalVoucher', N'JV', 0, N'PF')
SET IDENTITY_INSERT [dbo].[CodeGenerations] OFF

SET IDENTITY_INSERT [dbo].[Company] ON 

INSERT [dbo].[Company] ([Id], [Code], [Name], [Address], [District], [Division], [Country], [City], [PostalCode], [Phone], [Mobile], [Fax], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TaxId], [RegistrationNumber], [Mail], [NumberOfEmployees], [YearStart], [Year], [VATNo], [LogoName]) VALUES (14, N'C001', N'Infrastructure Development Company Limited (IDCOL)', N'dhaka', NULL, NULL, NULL, N'Dhaka faruque', N'347', N'0198999999', NULL, N'A', NULL, 1, 0, N'admin', N'20260106154637', N'192.168.15.100', N'admin', N'20260119150655', N'192.168.15.100', N'Tax id', N'001976146-0402', N'test', 1500, N'20240701', N'2025', N'1', NULL)
SET IDENTITY_INSERT [dbo].[Company] OFF

INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_1', 1, N'3', N'Commercial', 7, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907113931', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_35', 1, N'10', N'Front Office', 1, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907114151', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_36', 1, N'4', N'Legal', 2, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907113904', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_37', 1, N'1', N'Merchandising', 3, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907113810', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_38', 1, N'8', N'Support & Service', 4, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907114032', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_39', 1, N'11', N'Accounts & Finance', 5, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907114206', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_40', 1, N'9', N'Technical', 6, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907114048', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_41', 1, N'5', N'Internal Audit', 11, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907113923', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_42', 1, N'7', N'HR & Admin', 8, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907114010', N'')
INSERT [dbo].[Department] ([Id], [BranchId], [Code], [Name], [OrderNo], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_43', 1, N'2', N'Procurement', 9, NULL, 1, 0, N'Admin', N'20250320095411', N'', N'Admin', N'20250907165937', N'')

INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_1', 1, N'HoPS', N'Head of Priority Services', NULL, 1, 1, N'Admin', N'19000101', N'local', N'ADMIN', N'20210228114849', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_10', 1, N'', N'In-Charge, Aparajita Female Trading Booth-Gulshan', N'', 1, 1, N'Admin', N'19000101', N'local', N'ADMIN', N'20210228114849', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_100', 1, N'IMP-E', N'Executive -VAT & ERP', NULL, 1, 1, N'ADMIN', N'20210228121138', N'', N'Admin', N'20250907164546', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_101', 1, N'11', N'MTO', NULL, 1, 0, N'ADMIN', N'20210228121221', N'', N'Admin', N'20250907164803', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, 0, 0, NULL, N'1', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_102', 1, N'DV-S', N'Sr. Software Developer', NULL, 1, 1, N'ADMIN', N'20210228121254', N'', N'Admin', N'20250907164712', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_103', 1, N'DV-D', N'Software Developer', NULL, 1, 1, N'ADMIN', N'20210228121334', N'', N'Admin', N'20250907164712', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_104', 1, N'A&A', N'Jr. Executive Admin & Accounts', NULL, 1, 1, N'ADMIN', N'20210228121409', N'', N'Admin', N'20250907164603', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Designation] ([Id], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [AttendenceBonus], [EPZ], [Other], [DinnerAmount], [IfterAmount], [TiffinAmount], [ETiffinAmount], [OTAlloawance], [OTOrginal], [OTBayer], [ExtraOT], [PriorityLevel], [OrderNo], [DesignationGroupId], [GradeId], [HospitalPlanC1], [HospitalPlanC2], [HospitalPlanC3], [HospitalPlanC4], [HospitalPlanC5], [DeathCoveragePlanC6], [MaternityPlanC7], [MaternityPlanC8], [MaternityPlanC9], [EntitlementC1], [EntitlementC2], [EntitlementC3], [EntitlementC4], [EntitlementC5], [MobileExpenseC1], [MobileExpenseC2], [MobileExpenseC3], [MobileExpenseC4], [InternationalTravelC1], [InternationalTravelC2], [InternationalTravelC3], [DomesticlTravelC1], [DomesticTravelC2], [DomesticTravelC3], [DomesticTravelC4], [DomesticTravelC5]) VALUES (N'1_105', 1, N'GD', N'Graphics designer', NULL, 1, 1, N'ADMIN', N'20210228121437', N'', N'Admin', N'20250907164557', N'', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[DesignationGroup] ([Id], [Serial], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1', 1, 1, N'All', N'All', NULL, 1, 0, N'admin', N'1900/01/01', N'local', N'admin', N'1900/01/01', N'local')
INSERT [dbo].[DesignationGroup] ([Id], [Serial], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_10', 10, 1, N'MGT-1', N'Chairman', NULL, 0, 1, N'ADMIN', N'20230323174617', N'', N'Admin', N'20250908143233', N'')
INSERT [dbo].[DesignationGroup] ([Id], [Serial], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_11', 11, 1, N'MGT- ERP', N'Director & ERP Consultant', NULL, 0, 1, N'ADMIN', N'20230507120227', N'', N'Admin', N'20250908143233', N'')
INSERT [dbo].[DesignationGroup] ([Id], [Serial], [BranchId], [Code], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_12', 12, 1, N'VAT-1', N'VAT & Tax', NULL, 0, 1, N'ADMIN', N'20230622145117', N'', N'Admin', N'20250908143241', N'')


SET IDENTITY_INSERT [dbo].[EmployeeInfo] ON 

INSERT [dbo].[EmployeeInfo] ([Id], [Code], [Name], [Department], [Designation], [Project], [Section], [DateOfBirth], [JoinDate], [ResignDate], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [PhotoName], [NomineeDateofBirth], [NomineeName], [NomineeRelation], [NomineeAddress], [NomineeDistrict], [NomineeDivision], [NomineeCountry], [NomineeCity], [NomineePostalCode], [NomineePhone], [NomineeMobile], [NomineeBirthCertificateNo], [NomineeFax], [NomineeFileName], [NomineeRemarks], [NomineeNID], [GrossSalary], [BasicSalary], [LeftDate], [Grade], [Branch], [ProjectId], [SectionId], [DepartmentId], [DesignationId], [Other1], [EmployeeId], [EmpName], [Email], [ContactNo], [Status], [IsNoProfit], [BranchId], [ResignReason], [OfficialContactNo], [EmployeeNID], [EmployeeTIN], [FathersName], [MothersName], [SpouseName], [EmployeeBankAccountNumber], [PresentAddress], [ParmanentAdderss], [NomineeBankAccountNumber], [NomineeShare], [EmployeeBankNameId], [NomineeBankNameId], [IsProfit], [IsNoInterest]) VALUES (1, N'101', N'Raisul Islam', N'1_39', N'1_199', N'1_1', N'1_12', N'1/1/1970 12:00:00 AM', N'1/1/2009 12:00:00 AM', NULL, N'', 1, 0, N'', N'', N'', N'', N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(286000.00 AS Decimal(18, 2)), CAST(143000.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', N'', NULL, NULL, N'10', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[EmployeeInfo] OFF



SET IDENTITY_INSERT [dbo].[EnumCountry] ON 

INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (1, N'World', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (2, N'China', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (3, N'India', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (4, N'United States', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (5, N'Indonesia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (6, N'Brazil', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (7, N'Pakistan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (8, N'Nigeria', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (9, N'Bangladesh', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'ADMIN', N'20170928', N'182.48.89.41~192.168.15.22', 1)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (10, N'Russia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (11, N'Japan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (12, N'Mexico', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (13, N'Philippines', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (14, N'Vietnam', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (15, N'Ethiopia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (16, N'Egypt', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (17, N'Germany', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (18, N'Iran', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (19, N'Turkey', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (20, N'Democratic Republic of the Congo', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (21, N'Thailand', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (22, N'France', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (23, N'United Kingdom', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (24, N'Italy', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (25, N'Burma', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (26, N'South Africa', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (27, N'South Korea', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (28, N'Colombia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (29, N'Spain', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (30, N'Ukraine', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (31, N'Tanzania', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (32, N'Kenya', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (33, N'Argentina', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (34, N'Algeria', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (35, N'Poland', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (36, N'Sudan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (37, N'Uganda', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (38, N'Canada', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (39, N'Iraq', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (40, N'Morocco', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (41, N'Peru', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (42, N'Uzbekistan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (43, N'Saudi Arabia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (44, N'Malaysia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (45, N'Venezuela', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (46, N'Nepal', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (47, N'Afghanistan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'ADMIN               ', N'20170108', N'192.168.15.2', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (48, N'Yemen', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (49, N'North Korea', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (50, N'Ghana', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (51, N'Mozambique', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (52, N'Taiwan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (53, N'Australia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (54, N'Ivory Coast', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (55, N'Syria', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (56, N'Madagascar', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (57, N'Angola', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (58, N'Cameroon', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (59, N'Sri Lanka', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (60, N'Romania', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (61, N'Burkina Faso', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (62, N'Niger', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (63, N'Kazakhstan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (64, N'Netherlands', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (65, N'Chile', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (66, N'Malawi', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (67, N'Ecuador', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (68, N'Guatemala', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (69, N'Mali', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (70, N'Cambodia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (71, N'Senegal', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (72, N'Zambia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (73, N'Zimbabwe', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (74, N'Chad', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (75, N'South Sudan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (76, N'Belgium', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (77, N'Cuba', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (78, N'Tunisia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (79, N'Guinea', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (80, N'Greece', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (81, N'Portugal', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (82, N'Rwanda', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (83, N'Czech Republic', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (84, N'Somalia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (85, N'Haiti', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (86, N'Benin', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (87, N'Burundi', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (88, N'Bolivia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (89, N'Hungary', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (90, N'Sweden', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (91, N'Belarus', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (92, N'Dominican Republic', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (93, N'Azerbaijan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (94, N'Honduras', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (95, N'Austria', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (96, N'United Arab Emirates', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (97, N'Israel', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (98, N'Switzerland', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (99, N'Tajikistan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)

INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (100, N'Bulgaria', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (101, N'Hong Kong (China)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (102, N'Serbia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (103, N'Papua New Guinea', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (104, N'Paraguay', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (105, N'Laos', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (106, N'Jordan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (107, N'El Salvador', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (108, N'Eritrea', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (109, N'Libya', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (110, N'Togo', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (111, N'Sierra Leone', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (112, N'Nicaragua', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (113, N'Kyrgyzstan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (114, N'Denmark', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (115, N'Finland', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (116, N'Slovakia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (117, N'Singapore', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (118, N'Turkmenistan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (119, N'Norway', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (120, N'Lebanon', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (121, N'Costa Rica', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (122, N'Central African Republic', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (123, N'Ireland', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (124, N'Georgia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (125, N'New Zealand', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (126, N'Republic of the Congo', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (127, N'Palestine', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (128, N'Liberia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (129, N'Croatia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (130, N'Oman', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (131, N'Bosnia and Herzegovina', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (132, N'Puerto Rico', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (133, N'Kuwait', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (134, N'Moldov', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (135, N'Mauritania', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (136, N'Panama', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (137, N'Uruguay', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (138, N'Armenia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (139, N'Lithuania', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (140, N'Albania', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (141, N'Mongolia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (142, N'Jamaica', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (143, N'Namibia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (144, N'Lesotho', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (145, N'Qatar', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (146, N'Macedonia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (147, N'Slovenia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (148, N'Botswana', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (149, N'Latvia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (150, N'Gambia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (151, N'Kosovo', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (152, N'Guinea-Bissau', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (153, N'Gabon', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (154, N'Equatorial Guinea', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (155, N'Trinidad and Tobago', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (156, N'Estonia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (157, N'Mauritius', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (158, N'Swaziland', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (159, N'Bahrain', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (160, N'Timor-Leste', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (161, N'Djibouti', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (162, N'Cyprus', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (163, N'Fiji', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (164, N'Reunion (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (165, N'Guyana', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (166, N'Comoros', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (167, N'Bhutan', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (168, N'Montenegro', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (169, N'Macau (China)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (170, N'Solomon Islands', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (171, N'Western Sahara', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (172, N'Luxembourg', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (173, N'Suriname', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (174, N'Cape Verde', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (175, N'Malta', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (176, N'Guadeloupe (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (177, N'Martinique (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (178, N'Brunei', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (179, N'Bahamas', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (180, N'Iceland', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (181, N'Maldives', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (182, N'Belize', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (183, N'Barbados', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (184, N'French Polynesia (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (185, N'Vanuatu', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (186, N'New Caledonia (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (187, N'French Guiana (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (188, N'Mayotte (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (189, N'Samoa', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (190, N'Sao Tom and Principe', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (191, N'Saint Lucia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (192, N'Guam (USA)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (193, N'Curacao (Netherlands)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (194, N'Saint Vincent and the Grenadines', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (195, N'Kiribati', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (196, N'United States Virgin Islands (USA)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (197, N'Grenada', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (198, N'Tonga', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (199, N'Aruba (Netherlands)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)

INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (200, N'Federated States of Micronesia', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (201, N'Jersey (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (202, N'Seychelles', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (203, N'Antigua and Barbuda', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (204, N'Isle of Man (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (205, N'Andorra', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (206, N'Dominica', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (207, N'Bermuda (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (208, N'Guernsey (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (209, N'Greenland (Denmark)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (210, N'Marshall Islands', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (211, N'American Samoa (USA)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (212, N'Cayman Islands (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (213, N'Saint Kitts and Nevis', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (214, N'Northern Mariana Islands (USA)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (215, N'Faroe Islands (Denmark)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (216, N'Sint Maarten (Netherlands)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (217, N'Saint Martin (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (218, N'Liechtenstein', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (219, N'Monaco', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (220, N'San Marino', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (221, N'Turks and Caicos Islands (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (222, N'Gibraltar (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (223, N'British Virgin Islands (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (224, N'Aland Islands (Finland)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (225, N'Caribbean Netherlands (Netherlands)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (226, N'Palau', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (227, N'Cook Islands (NZ)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (228, N'Anguilla (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (229, N'Wallis and Futuna (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (230, N'Tuvalu', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (231, N'Nauru', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (232, N'Saint Barthelemy (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (233, N'Saint Pierre and Miquelon (France)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (234, N'Montserrat (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (235, N'Saint Helena, Ascension and Tristan da Cunha (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (236, N'Svalbard and Jan Mayen (Norway)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (237, N'Falkland Islands (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (238, N'Norfolk Island (Australia)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (239, N'Christmas Island (Australia)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (240, N'Niue (NZ)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (241, N'Tokelau (NZ)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (242, N'Vatican City', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (243, N'Cocos (Keeling) Islands (Australia)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
INSERT [dbo].[EnumCountry] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsContact]) VALUES (244, N'Pitcairn Islands (UK)', N'NA', 1, 0, N'admin', N'01/01/2016', N'local', N'admin', N'01/01/2016', N'local', 0)
SET IDENTITY_INSERT [dbo].[EnumCountry] OFF

SET IDENTITY_INSERT [dbo].[EnumDistrict] ON 

INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (1, N'Rajshahi', N'Bangladesh', N'Rajshahi', N'Rajshahi', 1, 0, N'1', N'1', N'1', N'1', N'1', NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (3, N'Dhaka', N'Bangladesh', N'Dhaka', N'Dhaka', 1, 0, N'1', N'1', N'1', N'1', N'1', NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (4, N'Dhaka', N'Bangladesh', N'Dhaka', NULL, 1, 1, N'ADMIN               ', N'20160411', N'192.168.15.2', N'ADMIN               ', N'20160411', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (5, N'Faridpur', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (6, N'Gopalganj', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (7, N'Gazipur', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (8, N'Jamalpur', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (9, N'Kishoreganj', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (10, N'Madaripur', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (11, N'Manikganj', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (12, N'Munshiganj', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (13, N'Mymensingh', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (14, N'Narayanganj', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (15, N'	Narsingdi', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (16, N'Netrakona', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (17, N'Rajbari', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (18, N'Shariatpur', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (19, N'Sherpur', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (20, N'Tangail', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (21, N'Rangamati', N'Bangladesh', N'Chittagoang', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (22, N'Bagerhat ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (23, N'Chuadanga ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (24, N'Jessore', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (25, N'Jhenaidah ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (26, N'Khulna ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (27, N'Kushtia ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (28, N'Magura ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (29, N'Meherpur', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (30, N'Narail ', N'Bangladesh', N'Dhaka', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (31, N'Satkhira ', N'Bangladesh', N'Khulna', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (32, N'Barguna ', N'Bangladesh', N'Barisal', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (33, N'Barisal District', N'Bangladesh', N'Barisal', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (34, N'Bhola ', N'Bangladesh', N'Barisal', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (35, N'Jhalokati ', N'Bangladesh', N'Barisal', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (36, N'Patuakhali ', N'Bangladesh', N'Barisal', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (37, N'Pirojpur ', N'Bangladesh', N'Barisal', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (38, N'Bandarban ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (39, N'Brahmanbaria ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (40, N'Chandpur ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (41, N'Chittagong District', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (42, N'Comilla ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (43, N'Cox''s Bazar ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (44, N'Feni ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (45, N'Khagrachhari ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (46, N'Lakshmipur ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (47, N'Noakhali ', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (48, N'Rangamati ', N'Bangladesh', N'Chittagoang', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (49, N'Rangamati District', N'Bangladesh', N'Chittagoang', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (50, N'Rangamati District', N'Bangladesh', N'Chittagoang', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (51, N'Rangamati', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (52, N'Bogra ', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (53, N'Joypurhat ', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (54, N'Naogaon ', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (55, N'Natore ', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (56, N'Chapainawabganj', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (57, N'Pabna ', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (58, N'Sirajganj ', N'Bangladesh', N'Rajshahi', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (59, N'Dinajpur ', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (60, N'Gaibandha ', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (61, N'Kurigram ', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (62, N'Lalmonirhat ', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (63, N'Nilphamari', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (64, N'Panchagarh ', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160413', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (65, N'Rangpur ', N'Bangladesh', N'Rajshahi', NULL, 1, 1, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (66, N'Dinajpur', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (67, N'Gaibandha ', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (68, N'Kurigram ', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (69, N'Lalmonirhat ', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (70, N'Thakurgaon ', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (71, N'Habiganj ', N'Bangladesh', N'Sylhet', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (72, N'Moulvibazar', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (73, N'Sunamganj ', N'Bangladesh', N'Sylhet', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (74, N'Sylhet ', N'Bangladesh', N'Sylhet', NULL, 1, 0, N'ADMIN               ', N'20160413', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (75, N'Rangpur', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (76, N'Dinajpur', N'Bangladesh', N'	Rangpur', NULL, 1, 1, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160417', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (77, N'Kurigram', N'Bangladesh', N'	Rangpur', NULL, 1, 1, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (78, N'Gaibandha', N'Bangladesh', N'	Rangpur', NULL, 1, 1, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160417', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (79, N'Nilphamari', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (80, N'Panchagarh', N'Bangladesh', N'Rangpur', NULL, 1, 0, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (81, N'Thakurgaon', N'Bangladesh', N'	Rangpur', NULL, 1, 1, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (82, N'Lalmonirhat.', N'Bangladesh', N'	Rangpur', NULL, 1, 1, N'ADMIN               ', N'20160417', N'192.168.15.2', N'ADMIN               ', N'20160514', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (83, N'Lalmonirhat', N'Bangladesh', N'Rangpur', NULL, 1, 1, N'ADMIN               ', N'20160514', N'192.168.15.2', N'ADMIN               ', N'20160608', N'192.168.15.2')
INSERT [dbo].[EnumDistrict] ([Id], [Name], [Country_E], [Division_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (84, N'Chittagong', N'Bangladesh', N'Chittagoang', NULL, 1, 0, N'ADMIN               ', N'20161203', N'192.168.15.2', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[EnumDistrict] OFF

SET IDENTITY_INSERT [dbo].[EnumDivision] ON 

INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (1, N'Dhaka', N'Bangladesh', N'a', 1, 0, N'1', N'1', N'1', N'ADMIN               ', N'20160411', N'192.168.15.2')
INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (2, N'Rajshahi', N'Bangladesh', N'a', 1, 0, N'1', N'1', N'1', NULL, NULL, NULL)
INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (3, N'Khulna', N'Bangladesh', N'a', 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (4, N'Chittagoang', N'Bangladesh', N'a', 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', N'ADMIN               ', N'20160411', N'192.168.15.2')
INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (5, N'Barisal', N'Bangladesh', N'Br', 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', N'ADMIN               ', N'20160411', N'192.168.15.2')
INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (6, N'Rangpur', N'Bangladesh', N'Rangpur', 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', N'ADMIN               ', N'20160514', N'192.168.15.2')
INSERT [dbo].[EnumDivision] ([Id], [Name], [Country_E], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (7, N'Sylhet', N'Bangladesh', N'SL', 1, 0, N'ADMIN               ', N'20160411', N'192.168.15.2', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[EnumDivision] OFF

INSERT [dbo].[EnumInvestmentTypes] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (1, N'FDR', N'FDR', 1, 0, N'Admin', N'20250904124257', N'', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[EnumInvestmentTypes] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (2, N'Testing', N'test', 1, 0, N'admin', N'20260118100850', N'', NULL, NULL, NULL, NULL, NULL)

SET IDENTITY_INSERT [dbo].[EnumJournalFor] ON 

INSERT [dbo].[EnumJournalFor] ([Id], [Name], [Remarks], [IsActice]) VALUES (1, N'Contribution', NULL, 1)
INSERT [dbo].[EnumJournalFor] ([Id], [Name], [Remarks], [IsActice]) VALUES (2, N'Investment', NULL, 1)
INSERT [dbo].[EnumJournalFor] ([Id], [Name], [Remarks], [IsActice]) VALUES (3, N'Profit Distributions', NULL, 1)
INSERT [dbo].[EnumJournalFor] ([Id], [Name], [Remarks], [IsActice]) VALUES (4, N'PF Settlement', NULL, 1)
INSERT [dbo].[EnumJournalFor] ([Id], [Name], [Remarks], [IsActice]) VALUES (5, N'Year Closing', NULL, 1)
INSERT [dbo].[EnumJournalFor] ([Id], [Name], [Remarks], [IsActice]) VALUES (6, N'Investment Accrued', NULL, 1)
SET IDENTITY_INSERT [dbo].[EnumJournalFor] OFF

SET IDENTITY_INSERT [dbo].[EnumLeftType] ON 

INSERT [dbo].[EnumLeftType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (2, N'Resignation', N'Resignation', 1, 0, N'admin', N'01/01/2016', N'local', N'Nj                  ', N'20160323', N'192.168.15.2')
INSERT [dbo].[EnumLeftType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (3, N'Termination', N'Termination', 1, 0, N'admin', N'01/01/2016', N'local', N'Nj                  ', N'20160323', N'192.168.15.2')
INSERT [dbo].[EnumLeftType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (4, N'Retirement', N'Retirement', 1, 0, N'admin', N'01/01/2016', N'local', N'Nj                  ', N'20160323', N'192.168.15.2')
INSERT [dbo].[EnumLeftType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (5, N'Terminate', N'Terminate', 1, 0, N'Nj                  ', N'20160323', N'192.168.15.2', NULL, NULL, NULL)
INSERT [dbo].[EnumLeftType] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (6, N'Inactive', NULL, 1, 0, N'admin', N'20170402', N'182.48.67.50~2001:0:9d38:6abd:1cf4:21e8:3f57:f02d', NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[EnumLeftType] OFF

SET IDENTITY_INSERT [dbo].[EnumLoanType] ON 

INSERT [dbo].[EnumLoanType] ([Id], [Name], [GLAccountCode], [Remarks], [IsInterest], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (10, N'PF Loan', N'-', NULL, 0, 1, 0, N'ADMIN               ', N'20161207', N'192.168.15.2', NULL, N'20170327', N'182.48.67.50~192.168.15.206')
INSERT [dbo].[EnumLoanType] ([Id], [Name], [GLAccountCode], [Remarks], [IsInterest], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (11, N'Medical Loan', N'-', NULL, 0, 1, 0, N'ADMIN               ', N'20161207', N'192.168.15.2', NULL, N'20170327', N'182.48.67.50~192.168.15.206')
INSERT [dbo].[EnumLoanType] ([Id], [Name], [GLAccountCode], [Remarks], [IsInterest], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (12, N'Car Loan', N'-', NULL, 0, 1, 0, N'ADMIN               ', N'20161207', N'192.168.15.2', NULL, N'20170327', N'182.48.67.50~192.168.15.206')
INSERT [dbo].[EnumLoanType] ([Id], [Name], [GLAccountCode], [Remarks], [IsInterest], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (13, N'Salary Loan', N'-', NULL, 0, 1, 0, N'ADMIN               ', N'20161207', N'192.168.15.2', NULL, N'20170327', N'182.48.67.50~192.168.15.206')
SET IDENTITY_INSERT [dbo].[EnumLoanType] OFF

INSERT [dbo].[EnumOderBy] ([Id], [Module], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'CODE', N'Salary', N'CODE', NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[EnumOderBy] ([Id], [Module], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'DCG', N'Salary', N'DEPT>CODE>GRADE', NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[EnumOderBy] ([Id], [Module], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'DDC', N'Salary', N'DEPT>DOJ>CODE', NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[EnumOderBy] ([Id], [Module], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'DGC', N'Salary', N'DEPT>GRADE>CODE', NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[EnumOderBy] ([Id], [Module], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'DGDC', N'Salary', N'DEPT>GRADE>DOJ>CODE', NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)


INSERT [dbo].[Grade] ([Id], [SL], [BranchId], [Code], [Name], [MinSalary], [MaxSalary], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsHouseRentFactorFromBasic], [IsTAFactorFromBasic], [TAFactor], [IsMedicalFactorFromBasic], [Area], [GradeNo], [CurrentBasic], [BasicNextYearFactor], [BasicNextStepFactor], [HouseRentFactor], [MedicalFactor], [IsFixedHouseRent], [HouseRentAllowance], [IsFixedSpecialAllowance], [SpecialAllowance], [LowerLimit], [MedianLimit], [UpperLimit]) VALUES (N'1', 1, 1, N'N/A', N'N/A', CAST(0.00 AS Decimal(18, 2)), CAST(1000000.00 AS Decimal(18, 2)), NULL, 1, 0, N'Admin', N'20250907', N'', N'Admin', N'20250907', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[Project] ([Id], [BranchId], [Code], [Name], [Startdate], [EndDate], [ManpowerRequired], [ContactPerson], [ContactPersonDesignation], [Address], [District], [Division], [Country], [City], [PostalCode], [Phone], [Mobile], [Fax], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [OrderNo]) VALUES (N'1_1', 1, N'0001', N'IDCOL', N'19000101', N'19000101', 0, NULL, NULL, N'Symphony Tower, Plot No.S.E(F)-9 (3rd Floor), Road No.142,Gulshan-1,Dhaka-1212', NULL, NULL, NULL, N'Gulshan-1', NULL, NULL, NULL, NULL, NULL, 1, 0, N'Admin', N'20250322171912', N'', N'Admin', N'20250907171735', N'', NULL)
INSERT [dbo].[Project] ([Id], [BranchId], [Code], [Name], [Startdate], [EndDate], [ManpowerRequired], [ContactPerson], [ContactPersonDesignation], [Address], [District], [Division], [Country], [City], [PostalCode], [Phone], [Mobile], [Fax], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [OrderNo]) VALUES (N'1_2', 1, N'SYM-CTG', N'Chittagong', N'', N'', 0, N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, 1, N'Admin', N'20250322171912', N'', N'Admin', N'20250907171622', N'', NULL)
INSERT [dbo].[Project] ([Id], [BranchId], [Code], [Name], [Startdate], [EndDate], [ManpowerRequired], [ContactPerson], [ContactPersonDesignation], [Address], [District], [Division], [Country], [City], [PostalCode], [Phone], [Mobile], [Fax], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [OrderNo]) VALUES (N'1_3', 1, N'T-001', N'Testing', N'01-Jan-2026', N'31-Jan-2026', 2, N'Md Sabbir Alam', NULL, N'Colonel hat', NULL, NULL, NULL, N'Chittagong', N'DFGH#$%^&*()', N'dfg#$%^&*(', N'RFHJ#$%^&*()_', NULL, NULL, 0, 1, N'admin', N'20260113180817', N'', N'admin', N'20260405120853', N'', NULL)

INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_1', 1, N'2', N'Procurement', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907171932', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_10', 1, N'5', N'Internal Audit', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172024', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_11', 1, N'7', N'HR & Admin', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172319', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_12', 1, N'12', N'Accounts & Finance', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172224', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_13', 1, N'13', N'Manager', N'Manager', 13, 1, 0, N'Admin', N'20250907174523', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_14', 1, N'17', N'Senior Executive', NULL, 17, 1, 0, N'Admin', N'20250907174946', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_15', 1, N'15', N'Creative', NULL, 0, 1, 0, N'Admin', N'20250907181232', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_16', 1, N'16', N'Brands & Strategy', NULL, 0, 1, 0, N'Admin', N'20250907181243', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_17', 1, N'17', N'Marketing', NULL, 0, 1, 0, N'Admin', N'20250907181255', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_18', 1, N'18', N'Business Development', NULL, 0, 1, 0, N'Admin', N'20250907181319', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_19', 1, N'22', N'Business Development & Strategy', NULL, 0, 1, 0, N'Admin', N'20250907181329', N'', N'Admin', N'20250908132305', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_2', 1, N'4', N'Legal', NULL, 5, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172004', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_20', 1, N'19', N'Brand & Social Media', NULL, 0, 1, 0, N'Admin', N'20250907181341', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_21', 1, N'21', N'Quality', NULL, 0, 1, 0, N'Admin', N'20250907183529', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_22', 1, N'14', N'General', NULL, 0, 1, 0, N'Admin', N'20250908132223', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_23', 1, N'23', N'Information Technology', NULL, 0, 1, 0, N'Admin', N'20250908132328', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_24', 1, N'24', N'Admin', NULL, 0, 1, 0, N'Admin', N'20250908132338', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_25', 1, N'25', N'Planning', NULL, 0, 1, 0, N'Admin', N'20250908132349', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_26', 1, N'26', N'Enterprise Sales', NULL, 0, 1, 0, N'Admin', N'20250908134653', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_27', 1, N'27', N'Pre- Sales', NULL, 0, 1, 0, N'Admin', N'20250908134703', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_28', 1, N'28', N'Product Research, Design & Development', NULL, 0, 1, 0, N'Admin', N'20250908134713', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_29', 1, N'29', N'Cyber Security', NULL, 0, 1, 0, N'Admin', N'20250908134722', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_3', 1, N'8', N'Support & Service', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172305', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_30', 1, N'30', N'Supply Chain', NULL, 0, 1, 0, N'Admin', N'20250908134732', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_31', 1, N'31', N'HR', NULL, 0, 1, 0, N'Admin', N'20250908134741', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_32', 1, N'32', N'Admin, Protocol & Transport', NULL, 0, 1, 0, N'Admin', N'20250908144835', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_33', 1, N'33', N'Sourcing', NULL, 0, 1, 0, N'Admin', N'20250908144845', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_34', 1, N'34', N'Corporate Brand', NULL, 0, 1, 0, N'Admin', N'20250908144854', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_35', 1, N'35', N'Corporate Affairs', NULL, 0, 1, 0, N'Admin', N'20250908144906', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_36', 1, N'36', N'Maintenance', NULL, 0, 1, 0, N'Admin', N'20250908144917', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_37', 1, N'37', N'Management Information Systems', NULL, 0, 1, 0, N'Admin', N'20250908144930', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_38', 1, N'38', N'Industrial Engineering', NULL, 0, 1, 0, N'Admin', N'20250908144942', N'', NULL, NULL, NULL)
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_39', 1, N'SQA', N'Testing', NULL, 0, 0, 1, N'admin', N'20260113113148', N'', N'admin', N'20260113113536', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_4', 1, N'1', N'Merchandising', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172312', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_5', 1, N'9', N'Technical', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172145', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_6', 1, N'11', N'Sustainability', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172258', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_7', 1, N'10', N'Front Office', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172203', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_8', 1, N'6', N'Operation', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907172045', N'')
INSERT [dbo].[Section] ([Id], [BranchId], [Code], [Name], [Remarks], [OrderNo], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_9', 1, N'3', N'Commercial', NULL, 0, 1, 0, N'Admin', N'20250322153045', N'', N'Admin', N'20250907171948', N'')

INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_1', 0, N'PF', N'IsWeightedAverageMonth', N'N', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', N'Admin', N'20251028184919', N'')
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_2', 1, N'PF', N'IsProfitCalculation', N'Y', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_3', 0, N'PF', N'AccruedByDay', N'Y', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', N'Admin', N'20251023130010', N'')
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_4', 1, N'PFLoanRate', N'FromSetting', N'N', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_5', 1, N'PFLoanRate', N'Upto12Month', N'5', N'int', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_6', 1, N'PFLoanRate', N'GetterThen12Month', N'6', N'int', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_7', 1, N'PF', N'FromDOJ', N'N', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_8', 1, N'HRM', N'IsESSEditPermission', N'N', N'string', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_9', 1, N'AutoUser', N'Employee', N'Y', N'string', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_10', 1, N'AutoPassword', N'Employee', N'123456', N'string', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_11', 1, N'AutoCode', N'Employee', N'Y', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_12', 1, N'PF', N'FromPayroll', N'Y', N'Boolean', NULL, 1, 0, N'Admin', N'20251023125937', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_13', 0, N'PF', N'YearDay', N'360', N'Int', NULL, 1, 0, N'Admin', N'20251023125937', N'', N'Admin', N'20251023125956', N'')
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_14', 1, N'PF', N'EntitleDate', N'20240701', N'varchar(14)', NULL, 1, 0, N'ADMIN', N'20200308153952', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_15', 1, N'PF', N'AutoJournal', N'Y', N'Boolean', NULL, 1, 0, N'Admin', N'20251028135520', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_16', 1, N'PF', N'IsAutoJournal', N'Y', N'Boolean', NULL, 1, 0, N'Admin', N'20251028135640', N'', NULL, NULL, NULL)
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_17', 0, N'PF', N'IsContributionNotSame', N'Y', N'Boolean', NULL, 1, 0, N'admin', N'20260113112809', N'', N'admin', N'20260329110614', N'')
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_73', 0, N'PFLoan', N'AvailableRate', N'100', N'decimal', NULL, 1, 0, N'ADMIN', N'20200322125449', N'', N'admin', N'20260329120925', N'')
INSERT [dbo].[Setting] ([Id], [BranchId], [SettingGroup], [SettingName], [SettingValue], [SettingType], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_20', 1, N'PF', N'CashCOAId', N'22', N'int', NULL, 1, 0, N'admin', N'20260113122239', N'', NULL, NULL, NULL)

INSERT [dbo].[SettlementPolicies] ([Id], [PolicyName], [JobAgeInMonth], [EmployeeContributionRatio], [EmployerContributionRatio], [EmployeeProfitRatio], [EmployerProfitRatio], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (1, N'Policy 1', CAST(48.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, 1, 0, N'Admin', N'20250904124415', N'', N'Admin', N'20250929211637', N'', NULL, NULL)
INSERT [dbo].[SettlementPolicies] ([Id], [PolicyName], [JobAgeInMonth], [EmployeeContributionRatio], [EmployerContributionRatio], [EmployeeProfitRatio], [EmployerProfitRatio], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (2, N'Policy 2', CAST(60.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, 1, 0, N'Admin', N'20250904124442', N'', N'Admin', N'20251007032349', N'', NULL, NULL)
INSERT [dbo].[SettlementPolicies] ([Id], [PolicyName], [JobAgeInMonth], [EmployeeContributionRatio], [EmployerContributionRatio], [EmployeeProfitRatio], [EmployerProfitRatio], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (3, N'Policy 3', CAST(61.01 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), NULL, 1, 0, N'Admin', N'20250929211739', N'', N'Admin', N'20251007032404', N'', NULL, NULL)

INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_1', 1, N'Admin', N'User', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_2', 1, N'Admin', N'Setup(Department)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_3', 1, N'Admin', N'Setup(Designation)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_4', 1, N'Admin', N'Setup(Bank)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_5', 1, N'Admin', N'Setup(Branch)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_6', 1, N'Admin', N'Setup(Project)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_7', 1, N'Admin', N'Setup(Section)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_8', 1, N'Admin', N'Setup(Salary Grade)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_9', 1, N'Admin', N'Setup(Holiday)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_10', 1, N'Admin', N'Setup(Fiscal Year)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_11', 1, N'Admin', N'Setup(SymUserRoll)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_12', 1, N'Admin', N'Setup(Settings)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)
INSERT [dbo].[SymUserDefaultRoll] ([Id], [BranchId], [symArea], [symController], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_13', 1, N'Admin', N'Setup(DB Update)', 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'19000101', N'local', NULL, NULL, NULL)

INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_1', N'1_1', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_2', N'1_2', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_3', N'1_3', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_4', N'1_4', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_5', N'1_5', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_6', N'1_6', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_7', N'1_7', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_8', N'1_8', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_9', N'1_9', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_10', N'1_10', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_11', N'1_11', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_12', N'1_12', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_13', N'1_13', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_14', N'1_14', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_15', N'1_15', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_16', N'1_16', 1, 2, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145117', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_17', N'1_1', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_18', N'1_2', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_19', N'1_3', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_20', N'1_4', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_21', N'1_5', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_22', N'1_6', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_23', N'1_7', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_24', N'1_8', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_25', N'1_9', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_26', N'1_10', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_27', N'1_11', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_28', N'1_12', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_29', N'1_13', 1, 3, 1, 1, 1, 1, 0, 1, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_30', N'1_14', 1, 3, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_31', N'1_15', 1, 3, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_32', N'1_16', 1, 3, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20251224145145', N'', N'admin', N'20260405103629', N'')
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_33', N'1_1', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_34', N'1_2', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_35', N'1_3', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_36', N'1_4', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_37', N'1_5', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_38', N'1_6', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_39', N'1_7', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_40', N'1_8', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_41', N'1_9', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_42', N'1_10', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_43', N'1_11', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_44', N'1_12', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)
INSERT [dbo].[SymUserRoll] ([Id], [DefaultRollId], [BranchId], [GroupId], [IsIndex], [IsAdd], [IsEdit], [IsDelete], [IsReport], [IsProcess], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom]) VALUES (N'1_45', N'1_13', 1, 4, 0, 0, 0, 0, 0, 0, NULL, 1, 0, N'admin', N'20260114125120', N'', NULL, NULL, NULL)


INSERT [dbo].[TransactionMedias] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (1, N'bKash', N'bKash', 1, 0, N'Admin', N'20250904124317', N'', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[TransactionMedias] ([Id], [Name], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [TransactionType], [TransType]) VALUES (2, N'Banking', NULL, 1, 0, N'admin', N'20260118101228', N'', NULL, NULL, NULL, NULL, NULL)

INSERT [dbo].[User] ([Id], [GroupId], [FullName], [Email], [LogId], [Password], [VerificationCode], [BranchId], [EmployeeId], [IsAdmin], [IsActive], [IsVerified], [IsArchived], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsApprove]) VALUES (N'1_3', 2, N'Farhan Sadique                                                                                      ', N'example@gmail.com                                                                                   ', N'admin                                             ', N'b1JXpoXo6qdggBV0qXQnBw==                          ', NULL, 1, N'6', 1, 1, 1, 0, N'Admin', N'', N'', NULL, NULL, NULL, 1)

SET IDENTITY_INSERT [dbo].[UserGroup] ON 

INSERT [dbo].[UserGroup] ([Id], [GroupName], [IsSuper], [IsESS], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsAdmin], [IsHRM], [IsAttendance], [IsPayroll], [IsTAX], [IsPF], [IsGF]) VALUES (2, N'Admin', 1, 0, NULL, 1, 0, N'Admin', N'11', N'11', N'Admin', N'20240825124719', N'', 1, 1, 0, 0, 0, 0, 1)
INSERT [dbo].[UserGroup] ([Id], [GroupName], [IsSuper], [IsESS], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsAdmin], [IsHRM], [IsAttendance], [IsPayroll], [IsTAX], [IsPF], [IsGF]) VALUES (3, N'ESS', 0, 1, N'12', 1, 0, N'ADMIN', N'20170422', N'182.48.67.50~192.168.15.23', N'admin', N'20260402190157', N'', 0, 0, 0, 0, 0, 1, 0)
INSERT [dbo].[UserGroup] ([Id], [GroupName], [IsSuper], [IsESS], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [IsAdmin], [IsHRM], [IsAttendance], [IsPayroll], [IsTAX], [IsPF], [IsGF]) VALUES (4, N'Tester', NULL, 0, NULL, 1, 0, N'admin', N'20260114114817', N'', N'admin', N'20260114114832', N'', 1, 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT [dbo].[UserGroup] OFF

SET IDENTITY_INSERT [dbo].[UserRoles] ON 

INSERT [dbo].[UserRoles] ([Id], [BranchId], [UserInfoId], [RoleInfoId], [IsArchived]) VALUES (1, 1, N'1_2', N'Admin', 0)
SET IDENTITY_INSERT [dbo].[UserRoles] OFF

INSERT [dbo].[Withdraws] ([Id], [IsInvested], [Code], [WithdrawAmount], [WithdrawDate], [BankBranchId], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [Post], [TransactionType], [ReferenceNo], [TransactionMediaId], [TransactionTypeId], [TransType]) VALUES (1, 0, N'BKW-0002/102025', CAST(1000.00 AS Decimal(18, 2)), N'20251014', 4, N'test', 1, 0, N'Admin', N'20251030155829', N'', NULL, NULL, NULL, 0, NULL, N'F1st Renew', N'1', 0, N'PF')
INSERT [dbo].[Withdraws] ([Id], [IsInvested], [Code], [WithdrawAmount], [WithdrawDate], [BankBranchId], [Remarks], [IsActive], [IsArchive], [CreatedBy], [CreatedAt], [CreatedFrom], [LastUpdateBy], [LastUpdateAt], [LastUpdateFrom], [Post], [TransactionType], [ReferenceNo], [TransactionMediaId], [TransactionTypeId], [TransType]) VALUES (2, 0, N'BKW-0003/012026', CAST(5000.00 AS Decimal(18, 2)), N'20260119', 9, NULL, 1, 0, N'admin', N'20260119113119', N'', N'admin', N'20260119114115', N'', 0, NULL, N'001', N'1', 0, N'PF')


                
                ";
                #endregion Default Data

                
                using (SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }

                // ===========================
                // COMMIT
                // ===========================
                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                retResults[0] = "Success";
                retResults[1] = "Database & Table Created Successfully.";
                retResults[2] = vm.Id;

                // XML Save Call
                bool xmlSaved = SaveToSuperFile(vm);

                if (!xmlSaved)
                {
                    retResults[1] += " But XML Save Failed!";
                }
                return retResults;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message;

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;

        public static bool SaveToSuperFile(DbCreateVM vm)
        {
            bool result = false;



            try
            {
                string filePath = HostingEnvironment.MapPath("~/Files/SuperInformation.xml");

                XDocument xDoc;

                // যদি file থাকে
                if (File.Exists(filePath))
                {
                    xDoc = XDocument.Load(filePath);
                }
                else
                {
                    // Root create
                    xDoc = new XDocument(new XElement("Super"));
                }
                              

                // New Node
                XElement newNode = new XElement("SuperInfo",
                    new XAttribute("tom", Converter.DESEncrypt(PassPhrase, EnKey, vm.LogIN)),
                    new XElement("jery", Converter.DESEncrypt(PassPhrase, EnKey, vm.Password)),
                    new XElement("mini", Converter.DESEncrypt(PassPhrase, EnKey, vm.ServerName)),
                    new XElement("doremon", Converter.DESEncrypt(PassPhrase, EnKey, vm.DatabaseName)),
                    new XElement("DateTime", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"))
                );

                // Add to root
                xDoc.Root.Add(newNode);

                // Save
                xDoc.Save(filePath);

                result = true;
            }
            catch (Exception ex)
            {
                // logging করতে পারো
                result = false;
            }

            return result;
        }

        public string[] TestConnection(DbCreateVM vm, SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            string sqlText = "";
            int Id = 0;

            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = Id.ToString();
            retResults[3] = sqlText;
            retResults[4] = "";
            retResults[5] = "InsertDB";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            try
            {
                if (VcurrConn != null)
                {
                    currConn = VcurrConn;
                }
                else
                {
                    currConn = _dbsqlConnection.GetConnectionSys(vm);

                    if (currConn == null)
                    {
                        retResults[1] = "Fail";
                    }
                    else
                    {
                        retResults[0] = "Success";
                        retResults[1] = "Database connect successfully";
                    }
                }
                return retResults;
            }
            catch (Exception ex)
            {
                retResults[0] = "Fail";
                retResults[4] = ex.Message;

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }

                return retResults;
            }
            finally
            {
                if (VcurrConn == null && currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }
    }
}
