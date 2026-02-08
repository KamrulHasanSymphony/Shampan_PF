using SymOrdinary;
using SymRepository.WPPF;
using SymRepository.Common;
using SymViewModel.WPPF;
using SymViewModel.Common;
using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using SymReporting.WPPF;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymWebUI.Areas.WPPF.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class PFReportController : Controller
    {
        public PFReportController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/PFReport/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        private GLJournalRepo _glJournalRepo = new GLJournalRepo();

        public ActionResult Index()
        {
            return View("~/Areas/WPPF/Views/PFReport/Index.cshtml");

        }

        /// <summary>
        /// Displays the Provident Fund Bank Statement Report view.
        /// </summary>
        /// <returns>
        /// Returns the view for the WPPF Bank Statement Report after verifying user permissions.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult PFBankStatementReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "report").ToString();
            return View("~/Areas/WPPF/Views/PFReport/PFBankStatementReport.cshtml");

        }

        /// <summary>
        /// Generates and returns a PDF report for the Provident Fund Bank Statement.
        /// </summary>
        /// <param name="dtFrom">The start date of the report range (optional).</param>
        /// <param name="dtTo">The end date of the report range (optional).</param>
        /// <param name="bankBranchId">The ID of the bank branch to filter the report (optional).</param>
        /// <returns>Returns a PDF file containing the WPPF Bank Statement report.</returns>
        /// <exception cref="Exception">Throws any exceptions that occur during report generation.</exception>
        [HttpGet]
        public ActionResult PFBankStatementReportView(string dtFrom = "", string dtTo = "", string bankBranchId = "")
        {
            try
            {
                //PFBankDeposit
                //Withdraw
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dtPFBankDeposit = new DataTable();
                DataTable dtWithdraw = new DataTable();
                DataSet ds = new DataSet();

                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                PFBankDepositVM pfbdVM = new PFBankDepositVM();
                PFBankDepositRepo _pfbdRepo = new PFBankDepositRepo();

                if (string.IsNullOrWhiteSpace(dtFrom))
                {
                    dtFrom = DateTime.MinValue.ToString();
                }

                if (string.IsNullOrWhiteSpace(dtTo))
                {
                    dtTo = DateTime.MaxValue.ToString();
                }

                pfbdVM.DateFrom = dtFrom;
                pfbdVM.DateTo = dtTo;
                pfbdVM.BankBranchId = Convert.ToInt32(bankBranchId);
                pfbdVM.TransType = AreaTypePFVM.TransType;

                dtPFBankDeposit = _pfbdRepo.PFBankStatementReport(pfbdVM);

                ReportHead = "There are no data to Preview for WPPF Bank Statement";
                if (dtPFBankDeposit.Rows.Count > 0)
                {
                    ReportHead = AreaTypePFVM.TransType+" Bank Statement";
                }

                ds.Tables.Add(dtPFBankDeposit);
                ds.Tables[0].TableName = "dtPFBankStatement";

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPFBankStatement.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
        
        public ActionResult AccountStatement()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/WPPF/Views/PFReport/AccountStatement.cshtml");

        }

        public ActionResult PFInvestmentReportView()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/WPPF/Views/PFReport/PFInvestmentReport.cshtml");

        }

        public ActionResult PFInvestmentRepor(string ToDate, string Type)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFReport rpt = new PFReport();
                string BranchName = Session["BranchName"].ToString();

                if(Type=="FDR")
                {
                    vm = rpt.PFInvestmentRepor(ToDate, BranchName);
                }
                else
                {
                    vm = rpt.PFInvestmentReporSyn(ToDate, BranchName);
                }

                return File(vm.MemStream, "application/PDF");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult EmployeePFStatement()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/WPPF/Views/PFReport/EmployeePFStatement.cshtml");

        }

        public ActionResult EmployeePFStatements(string rType, string EmployeeId,string ToDate,string FromDate)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFReport rpt = new PFReport();

                vm = rpt.EmployeePFStatements(rType,  EmployeeId, ToDate,FromDate);

                return File(vm.MemStream, "application/PDF");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult EmployeePFSettlementReport()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/WPPF/Views/PFReport/EmployeePFSettlementReport.cshtml");

        }

        public ActionResult EmployeePFSettlementReports(string EmployeeId, string ToDate, string FromDate)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFReport rpt = new PFReport();
                string BranchName = Session["BranchName"].ToString();

                vm = rpt.EmployeePFSettlementReport(EmployeeId, ToDate, FromDate, BranchName);

                return File(vm.MemStream, "application/PDF");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult EmployeeLoanSettelmentView()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/WPPF/Views/PFReport/EmployeeLoanSettelment.cshtml");

        }

        public ActionResult EmployeeLoanClosedView()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            return View("~/Areas/WPPF/Views/PFReport/EmployeeLoanClosed.cshtml");

        }
    
        #region Excel Reports

        public ActionResult InvestmentStatement(PFParameterVM paramVM)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFExcel xcl = new PFExcel();
                vm.TransType = AreaTypePFVM.TransType;
                vm = xcl.InvestmentStatement(paramVM);

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + vm.FileName + ".xlsx");
                vm.MemStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();

                return Redirect("InvestmentStatement");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Report(PFReportVM vm)
        {

            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {

                return null;

            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("Report");
            }
        }

        #endregion
        
        [HttpGet]
        public ActionResult BankChargeReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/BankChargeReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeBreakMonthPFReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/EmployeeBreakMonthPFReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeePFPaymentReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/EmployeePFPaymentReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeForfeitureReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/EmployeeForfeitureReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeePFOpeinigReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/EmployeePFOpeinigReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/EmployeeReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EmployeeLedgerReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/EmployeeLedgerReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/LoanReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanMonthlyPaymentReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/LoanMonthlyPaymentReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanRepaymentToBankReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/LoanRepaymentToBankReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult LoanSattlementReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/LoanSattlementReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult PFBankDepositsReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/PFBankDepositsReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult PFContributionReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/PFContributionReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult ProfitDistributionNewReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/ProfitDistributionNewReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult ReturnOnBankInterestsReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/ReturnOnBankInterestsReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult WithdrawsReportVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/WithdrawsReportVeiw.cshtml", vm);


            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Voucher_StatementReportVeiw(string JournalType="1")
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.JournalType = JournalType;
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/Voucher_StatementReportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult YearClosingReportVeiw(string JournalType = "1")
        {
            try
            {
                PFReportVM vm = new PFReportVM();
                vm.JournalType = JournalType;
                vm.TransType = AreaTypePFVM.TransType;
                return View (vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Year_Closing(string fydid)
        {
            PFReportVM vm = new PFReportVM();
            string[] result = new string[6];
            try
            {
                vm.YearTo = fydid;
                 vm.BaseEntity.CreatedBy = identity.Name;
                 vm.BaseEntity.CreatedFrom = identity.WorkStationIP;
                 vm.TransType = AreaTypePFVM.TransType;

                 PFReportRepo _repo = new PFReportRepo();
                result = _repo.YearClosing(vm);
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        private void ExcelSheetFormat(DataTable dt, ExcelWorksheet workSheet, string[] ReportHeaders)
        {


            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;

            int InWordsRow = 0;
            InWordsRow = GrandTotalRow + 1;

            int SignatureSpaceRow = 0;
            SignatureSpaceRow = InWordsRow + 1;

            int SignatureRow = 0;
            SignatureRow = InWordsRow + 4;
            workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);
            #region Format

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };



            for (int i = 0; i < ReportHeaders.Length; i++)
            {
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

            }
            int colNumber = 0;

            foreach (DataColumn col in dt.Columns)
            {
                colNumber++;
                if (col.DataType == typeof(DateTime))
                {
                    workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                }
                else if (col.DataType == typeof(Decimal))
                {

                    workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    #region Grand Total
                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                    #endregion
                }

            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
            #endregion
        }

        [HttpPost]
        public ActionResult BankCharge_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "bd.TransactionDate>", "bd.TransactionDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.BankCharge_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType+" BankCharge_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " Bank Charge Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Bank Charge Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeeBreakMonthPF_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.EmployeeBreakMonthPF_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "EmployeeBreakMonth"+AreaTypePFVM.TransType+"_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeeBreakMonth" + AreaTypePFVM.TransType + "_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee Break Month "+ AreaTypePFVM.TransType +" Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeePaymentPF_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "e.EmployeeId", "pfo.PaymentDate>", "pfo.PaymentDate<" };
                string[] conditionValues = { vm.EmployeeId,Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                DataTable dt = _repo.EmployeePaymentPF_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "EmployeePayment" + AreaTypePFVM.TransType + "_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeePayment" + AreaTypePFVM.TransType + "_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee Payment " + AreaTypePFVM.TransType + " Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public ActionResult EmployeeForfeiture_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "e.Code" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.EmployeeForfeiture_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "EmployeeForfeiture_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeeForfeiture_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee Forfeiture Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeePFOpeinig_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId };

                DataTable dt = _repo.EmployeePFOpeinig_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = "Employee WPPF Opening Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeePFOpeinig_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { "Employee WPPF Opening Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult EmployeeStatement(PFReportVM vm)
        {
            try
            {
                string rptLocation = "";
               

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "e.Code", "pf.TransactionDate>", "pf.TransactionDate<" };
                string[] conditionValues = { vm.EmployeeId, Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo)};
                DateTime startdate = Convert.ToDateTime(vm.DateFrom);
                DateTime enddate = Convert.ToDateTime(vm.DateTo);

                string ReportHead = "Employee Statement " + startdate.ToString("yyyy") + " - " + enddate.ToString("yy") + " ";

                DataTable dt = _repo.EmployeeStatement(conditionFields, conditionValues);
                if(vm.ReportType=="Excel")
                { 
                #region Excel

                string filename = "EmployeeStatement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("EmployeeStatement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;




                string[] ReportHeaders = new string[] { "Employee Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion
                }
                else
                {
                    CompanyRepo _CompanyRepo = new CompanyRepo();
                    CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                    ReportDocument doc = new ReportDocument();
                    dt.TableName = "dtEmployeeStatement";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptEmployeeStatementDetails.rpt";

                    doc.Load(rptLocation);
                    doc.SetDataSource(dt);
                    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                    doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                    doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
                    var rpt = RenderReportAsPDF(doc);
                    doc.Close();
                    return rpt;
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generates and returns an Employee Ledger report as a PDF or Excel file based on the provided parameters.
        /// </summary>
        /// <param name="vm">The PFReportVM object containing report parameters such as ReportType, DateFrom, and DateTo.</param>
        /// <returns>
        /// Returns a PDF file containing the Employee Ledger report or triggers an Excel file download depending on the selected ReportType.
        /// </returns>
        /// <exception cref="Exception">Throws any exceptions that occur during report generation or file export.</exception>

        [HttpPost]
        public ActionResult EmployeeLedger(PFReportVM vm)
        {
            try
            {
                string rptLocation = "";

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = {  };
                string[] conditionValues = { };
                DateTime startdate = Convert.ToDateTime(vm.DateFrom);
                DateTime enddate = Convert.ToDateTime(vm.DateTo);
                string ReportHead = "Member’s Fund Position (" + startdate.ToString("yyyy") + "-"+enddate.ToString("yy")+")";
                DataTable dt = _repo.PFEmployeeLedger(vm, conditionFields, conditionValues);
                if (vm.ReportType == "Excel")
                {
                    dt.Columns.Remove("Loan");

                    #region Excel

                    string filename = "EmployeeStatement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                    ExcelPackage excel = new ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("EmployeeStatement");
                    CompanyRepo cRepo = new CompanyRepo();
                    CompanyVM comInfo = cRepo.SelectById(1);
                    string Line1 = comInfo.Name;
                    string Line2 = comInfo.Address;
                    //string Line3 = "";
                    string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";



                    string[] ReportHeaders = new string[] { "Employee Statement", Line1, Line2, "Branch: " + branchName };

                    ExcelSheetFormat(dt, workSheet, ReportHeaders);
                    #region Excel Download

                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                    #endregion
                    return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                    #endregion
                }
                else
                {
                    ReportDocument doc = new ReportDocument();
                    dt.TableName = "dtEmployeeLedger";
                    if (vm.ReportType == "Individual")
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\RptPFIndividualLedger.rpt";
                    }
                    else
                    {
                        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPFEmployeeLedger.rpt";
                    }
                    CompanyRepo _CompanyRepo = new CompanyRepo();
                    CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                    doc.Load(rptLocation);
                    doc.SetDataSource(dt);
                    string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                    doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";

                    if (vm.DateFrom != null && vm.DateTo != null)
                    {
                        doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                        doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";

                    }
                    var rpt = RenderReportAsPDF(doc);
                    doc.Close();
                    return rpt;
                }
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Loan_Statement(PFReportVM vm)
        {
            try
            {

                vm.TransType = AreaTypePFVM.TransType;
                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "TransactionDate>", "TransactionDate<", "TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.Loan_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " Loan_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " Loan_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Loan Statement", Line1, Line2, Line3 };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanMonthlyPayment_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "TransactionDate>", "TransactionDate<", "TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) , AreaTypePFVM.TransType};

                DataTable dt = _repo.LoanMonthlyPayment_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + "LoanMonthlyPayment_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " LoanMonthlyPayment_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Loan Monthly Payment Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanRepaymentToBank_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();
                string[] conditionFields = { "TransactionDate>", "TransactionDate<", "TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.LoanRepaymentToBank_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " LoanRepaymentToBank_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " LoanRepaymentToBank_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Loan Repayment To Bank Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanSattlement_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "TransactionDate>", "TransactionDate<" ,"TransType"};
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.LoanSattlement_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " LoanSattlement_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " LoanSattlement_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Loan Sattlement Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult PFBankDeposits_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();


                string[] conditionFields = { "bd.DepositDate>", "bd.DepositDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.PFBankDeposits_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " BankDeposits_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " BankDeposits_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Bank Deposits Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult PFContribution_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "e.EmployeeId" };
                string[] conditionValues = { vm.EmployeeId};

                DataTable dt = _repo.PFContribution_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " Contribution_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " Contribution_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Contribution Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ProfitDistributionNew_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "pd.DistributionDate>", "pd.DistributionDate<", "pd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.ProfitDistributionNew_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " ProfitDistributionNew_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " ProfitDistributionNew_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Profit Distribution Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ReturnOnBankInterests_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "bd.TransactionDate>", "bd.TransactionDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.ReturnOnBankInterests_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " ReturnOnBankInterests_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " ReturnOnBankInterests_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Return On Bank Interests Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Withdraws_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = { "bd.WithdrawDate>", "bd.WithdrawDate<", "bd.TransType" };
                string[] conditionValues = { Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                DataTable dt = _repo.Withdraws_Statement(conditionFields, conditionValues);

                #region Excel

                string filename = AreaTypePFVM.TransType + " Withdraws_Statement " + "-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(AreaTypePFVM.TransType + " Withdraws_Statement");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { AreaTypePFVM.TransType + " Withdraws Statement", Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Voucher_Statement(PFReportVM vm)
        {
            try
            {

                PFReportRepo _repo = new PFReportRepo();

                string[] conditionFields = new string[] { "jd.JournalType", "jd.TransType", "jd.COAId" };
                string[] conditionValues = new string[] { vm.JournalType, AreaTypePFVM.TransType, vm.Id.ToString() == "0" ? null : vm.Id.ToString() };

                DataTable dt = _repo.Voucher_Statement(conditionFields, conditionValues);

                #region Excel

                string StatementName = "";
                if (vm.JournalType == "1")
                {
                    StatementName = AreaTypePFVM.TransType + " JournalVoucher";
                }
                else if (vm.JournalType == "2")
                {
                    StatementName = AreaTypePFVM.TransType + " PaymentVoucher";
                }
                else if (vm.JournalType == "3")
                {
                    StatementName = AreaTypePFVM.TransType + " ReceiptVoucher";
                }

                string filename = StatementName + " -" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(StatementName);
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;// "BRAC EPL STOCK BROKERAGE LIMITED";
                string Line2 = comInfo.Address;// "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                int LeftColumn = 5;
                int CenterColumn = 5;

                string[] ReportHeaders = new string[] { StatementName, Line1, Line2, "Branch: " + branchName };

                ExcelSheetFormat(dt, workSheet, ReportHeaders);
                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                #endregion
                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");
                #endregion

                //return File(vm.MemStream, "application/PDF");
            }

            catch (Exception)
            {
                throw;
            }
        }

         [HttpGet]
        public ActionResult TrialBalanceVeiw()
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PFReport/TrialBalanceVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

         [HttpPost]
         public ActionResult TrialBalanceReport(PFReportVM vm)
         {

             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 vm.TransType = AreaTypePFVM.TransType;
              
                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypePFVM.TransType };

                 FiscalYearRepo fyrepo = new FiscalYearRepo();
                 var tempYear = fyrepo.SelectByYear(Convert.ToInt32(vm.FiscalYear));
                 vm.DateFrom = tempYear.YearStart;
                 vm.DateTo = tempYear.YearEnd;
                vm.TransType = "WPPF";

                 ReportDocument doc = new ReportDocument();
                 DataSet ds = new DataSet();
                 DataTable dt = new DataTable();

                 string TransactionAmount = "0";

                
                 ds = _repo.FRReports(vm);
                              
                 DataRow[] filteredRows = ds.Tables[0].Select("GroupSL = '8888'");
                 foreach (DataRow row in filteredRows)
                 {
                     TransactionAmount = row["TransactionAmount"].ToString();  // Access the filtered rows here

                     TransactionAmount = Ordinary.ParseDecimal(TransactionAmount);
                 }

                 foreach (DataRow row in filteredRows)
                 {
                     row.Delete();
                 }
                 ds.Tables[0].AcceptChanges();
                 if (false)
                 {
                     ds = _repo.View_TrialBalance(vm, cFields, cValues);
                     TransactionAmount = "0";
                     if (ds.Tables[1].Rows.Count > 0)
                     {
                         TransactionAmount = ds.Tables[1].Rows[0]["TransactionAmount"].ToString();
                     }
                 }
                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     ReportHead = "TrialBalance";
                 }
                 ds.Tables[0].TableName = "dtFinancialReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptTrialBalance.rpt";

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();


                 doc.Load(rptLocation);
                 doc.SetDataSource(ds);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["TransactionAmount"].Text = "'" + TransactionAmount + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                 doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";


                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult IncomeStatementVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypePFVM.TransType;
                 return PartialView("~/Areas/WPPF/Views/PFReport/IncomeStatementVeiw.cshtml", vm);

             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpPost]
         public ActionResult IncomeStatementReport(PFReportVM vm)
         {

              try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 vm.TransType = AreaTypePFVM.TransType;

                 FiscalYearRepo fyrepo = new FiscalYearRepo();
                 var tempYear = fyrepo.SelectByYear(Convert.ToInt32(vm.FiscalYear));
                 vm.DateFrom = tempYear.YearStart;
                 vm.DateTo = tempYear.YearEnd;

                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypePFVM.TransType };

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();
                 DataSet ds = new DataSet();
                                  
                 ds = _repo.FRReports(vm);
                 dt = ds.Tables[0];
                

                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                                ReportHead = "There are no data to Preview for TrialBalance";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "IncomeStatement";
                 }
                 dt.TableName = "dtFinancialReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptIncomeStatement.rpt";

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                 doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult IFRSReportView(string ReportType)
         {
             try
             {
                 PFReportVM vm = new PFReportVM();
                 vm.ReportType = ReportType;
                 vm.TransType = "WPPF";
                 vm.TransType = AreaTypePFVM.TransType;
                 return View(vm);
                 //return PartialView("~/Areas/WPPF/Views/PFReport/IFRSReporView.cshtml", vm);
             }
             catch (Exception)
             {
                 throw;
             }
         }

         /// <summary>
         /// Generates and returns an IFRS report as a PDF based on the selected report type:
         /// Balance Sheet (BS), Income Statement (IS), Trial Balance (TB), or Net Change (NC).
         /// </summary>
         /// <param name="vm">The PFReportVM object containing report parameters such as TransType, ReportType, and fiscal date range.</param>
         /// <returns>Returns a PDF file containing the generated IFRS report.</returns>
         /// <exception cref="Exception">Throws any exceptions that occur during report generation.</exception>

         [HttpPost]
         public ActionResult IFRSReport(PFReportVM vm)
         {

             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 string fileName = "rptIFRSReportTB.rpt";
                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();
                 DataTable dt1 = new DataTable();
                 DataSet ds = new DataSet();
                 vm.TransType = AreaTypePFVM.TransType;
               
                 ds = _repo.IFRSReports(vm);
                 dt = ds.Tables[0];
              
                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();
                  FiscalYearRepo fyrepo = new FiscalYearRepo();

                  //vm.DateFrom = fyrepo.FYPeriodDetail(vm.MonthFrom).FirstOrDefault().PeriodName;
                  //vm.DateTo = fyrepo.FYPeriodDetail(vm.MonthTo).FirstOrDefault().PeriodName;

                 ReportHead = "";
                 if (dt.Rows.Count > 0)
                 {
                     if(vm.ReportType.ToUpper()=="BS")
                     {
                         //ReportHead = "Balance Sheet";
                         fileName = "rptIFRSReportBS.rpt";
                        dt1 = ds.Tables[1];
                        vm.DateFrom = Ordinary.StringToDate(dt1.Rows[0]["FirstEnd"].ToString());
                        vm.YearFrom = dt1.Rows[0]["FirstYear"].ToString();
                        if (vm.YearFrom=="1900")
                        {
                            vm.YearFrom = (Convert.ToInt32(dt1.Rows[0]["LastYear"]) - 1).ToString();
                        }
                        vm.DateTo = Ordinary.StringToDate(dt1.Rows[0]["LastEnd"].ToString());
                        vm.YearTo = dt1.Rows[0]["LastYear"].ToString();

                     }
                     else if (vm.ReportType.ToUpper() == "IS")
                     {
                         //ReportHead = "Income Statement";
                         fileName = "rptIFRSReportIS.rpt";
                         dt1 = ds.Tables[1];
                         string json = JsonConvert.SerializeObject(dt1);
                         vm.PFReport1VMs = JsonConvert.DeserializeObject<List<PFReport1VM>>(json);
                         vm.PFReport1VM = vm.PFReport1VMs.FirstOrDefault();
                         if (vm.YearFrom == "1900" || vm.YearFrom == null)
                         {
                             vm.YearFrom = (Convert.ToInt32(dt1.Rows[0]["LastYear"]) - 1).ToString();
                         }
                         vm.DateFrom = Ordinary.StringToDate(dt1.Rows[0]["FirstEnd"].ToString()); 
                         vm.DateTo = Ordinary.StringToDate(dt1.Rows[0]["LastEnd"].ToString());
                         vm.YearTo = dt1.Rows[0]["LastYear"].ToString();

                         vm.PFReport1VM.FirstNetProfit = Math.Abs(vm.PFReport1VM.FirstNetProfit);
                         vm.PFReport1VM.LastNetProfit = Math.Abs(vm.PFReport1VM.LastNetProfit);
                     }
                     else if (vm.ReportType.ToUpper() == "TB" || vm.ReportType.ToUpper() == "NC")                     
                     {
                         //ReportHead = "Trial Balance";
                         fileName = "rptIFRSReportTB.rpt";
                         if (vm.ReportType.ToUpper() == "NC")
                         {
                             fileName = "rptIFRSReportNC.rpt";
                             
                         }

                         dt1 = ds.Tables[1];
                         string json = JsonConvert.SerializeObject(dt1);
                         vm.PFReport1VMs = JsonConvert.DeserializeObject<List<PFReport1VM>>(json);
                         vm.PFReport1VM = vm.PFReport1VMs.FirstOrDefault();
                         vm.DateFrom = Ordinary.StringToDate(dt1.Rows[0]["FirstEnd"].ToString());
                         vm.YearFrom = dt1.Rows[0]["FirstYear"].ToString();

                         vm.DateTo = Ordinary.StringToDate(dt1.Rows[0]["LastEnd"].ToString());
                         vm.YearTo = dt1.Rows[0]["LastYear"].ToString();

                         //vm.PFReport1VM.FirstNetProfit = Math.Abs(vm.PFReport1VM.FirstNetProfit);
                         //vm.PFReport1VM.LastNetProfit = Math.Abs(vm.PFReport1VM.LastNetProfit);
                     }
                     
                 }
                 dt.TableName = "dtIFRSReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\" + fileName;

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions crFormulaF = doc.DataDefinition.FormulaFields;

                 CommonFormMethod _vCommonFormMethod = new CommonFormMethod();
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "FirstNetProfit", vm.PFReport1VM.FirstNetProfit.ToString("#,##0.00"));
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "LastNetProfit", vm.PFReport1VM.LastNetProfit.ToString("#,##0.00"));

                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "ReportHeaderA4", companyLogo);
                 //_vCommonFormMethod.FormulaField(doc, crFormulaF, "ReportHead", ReportHead);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "TransType", AreaTypePFVM.TransType);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "DateFrom", vm.DateFrom);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "DateTo", vm.DateTo);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "YearFrom", vm.YearFrom);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "YearTo", vm.YearTo);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "Address", cvm.Address);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "CompanyName", cvm.Name);
                 _vCommonFormMethod.FormulaField(doc, crFormulaF, "BranchName", Session["BranchName"].ToString());

                 
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult BalanceSheetVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypePFVM.TransType;
                 return PartialView("~/Areas/WPPF/Views/PFReport/BalanceSheetVeiw.cshtml", vm);

             }
             catch (Exception)
             {
                 throw;
             }
         }

         /// <summary>
         /// Generates and returns a PDF Balance Sheet report for the specified fiscal year and transaction type.
         /// </summary>
         /// <param name="vm">The PFReportVM view model containing the FiscalYear and other filtering parameters.</param>
         /// <returns>Returns a PDF file containing the Balance Sheet report.</returns>
         /// <exception cref="Exception">Throws any exceptions that occur during report generation.</exception>

         [HttpPost]
         public ActionResult BalanceSheetReport(PFReportVM vm)
         {


             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();
                 vm.TransType = AreaTypePFVM.TransType;
             
                 string[] cFields = { "TransType" };
                 string[] cValues = { AreaTypePFVM.TransType };
                 vm.TransType = AreaTypePFVM.TransType;

                 FiscalYearRepo fyrepo = new FiscalYearRepo();
                 var tempYear = fyrepo.SelectByYear(Convert.ToInt32(vm.FiscalYear));
                 vm.DateFrom = tempYear.YearStart;
                 vm.DateTo = tempYear.YearEnd;             

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();
                 DataSet ds = new DataSet();

                
                 ds = _repo.FRReports(vm);
                 dt = ds.Tables[0];

                 //dt = _repo.View_BalanceSheet(vm,cFields, cValues);
                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();


                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "BalanceSheet";
                 }
                 dt.TableName = "dtFinancialReport";
                 rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptBalanceSheet.rpt";

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                 //string DateFrom = vm.DateFrom ?? "";

                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                 doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult LedgerVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypePFVM.TransType;
                 return PartialView("~/Areas/WPPF/Views/PFReport/LedgerVeiw.cshtml", vm);
             }
             catch (Exception)
             {
                 throw;
             }
         }

         /// <summary>
         /// Generates and returns a PDF report for the WPPF Ledger, including net change data within a specified date range.
         /// </summary>
         /// <param name="vm">The PFReportVM view model containing filtering parameters like DateFrom, DateTo, and others.</param>
         /// <returns>Returns a PDF file containing the WPPF Ledger report.</returns>
         /// <exception cref="Exception">Throws any exceptions that occur during report generation.</exception>
         [HttpPost]
         public ActionResult LedgerReport(PFReportVM vm)
         {


             try
             {
                 PFReportRepo _repo = new PFReportRepo();
                 string ReportHead = "";
                 string rptLocation = "";
                 PFReport report = new PFReport();

                 vm.TransType = AreaTypePFVM.TransType;
                 string[] cFields = { "TransType","BranchId" };
                 string[] cValues = { AreaTypePFVM.TransType ,Session["BranchId"].ToString()};
                 vm.BranchId = Session["BranchId"].ToString();
                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();


                 dt = _repo.View_NetChange(vm,cFields, cValues);

                 //dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));
                 CompanyRepo _CompanyRepo = new CompanyRepo();
                 CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                 ReportHead = "There are no data to Preview for TrialBalance";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "LedgerReport";
                 }
                 dt.TableName = "dtLedger";
                 if (vm.Id == null || vm.Id == 0)
                 {
                     rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptLedgerMultiple.rpt";
                 }
                 else
                 {
                     rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptLedger.rpt";
                 }                 

                 doc.Load(rptLocation);
                 doc.SetDataSource(dt);
                 string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                 FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                 doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                 doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                 doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                 doc.DataDefinition.FormulaFields["DateFrom"].Text = "'" + vm.DateFrom + "'";
                 doc.DataDefinition.FormulaFields["DateTo"].Text = "'" + vm.DateTo + "'";
                 doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                 doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                 doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";

                 //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                 var rpt = RenderReportAsPDF(doc);
                 doc.Close();
                 return rpt;
             }
             catch (Exception)
             {
                 throw;
             }
         }

         public ActionResult EmployeeLoanSettelment(string rType, string ToDate, string FromDate)
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 PFReport rpt = new PFReport();
                 string BranchId = Session["BranchId"].ToString();
                 string BranchName = Session["BranchName"].ToString();
                 vm = rpt.EmployeeLoanSettelment(rType, ToDate, FromDate, BranchId, BranchName);

                 return File(vm.MemStream, "application/PDF");
             }
             catch (Exception)
             {
                 throw;
             }
         }       

         public ActionResult EmployeeLoanClosed(string rType, string EmployeeId, string ToDate, string FromDate)
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 PFReport rpt = new PFReport();
                 string BranchName = Session["BranchName"].ToString();
                 vm = rpt.EmployeeLoanClosed(rType, ToDate, FromDate, BranchName);

                 return File(vm.MemStream, "application/PDF");
             }
             catch (Exception)
             {
                 throw;
             }
         }

         [HttpGet]
         public ActionResult AllEmployeeReportVeiw()
         {
             try
             {

                 PFReportVM vm = new PFReportVM();
                 vm.TransType = AreaTypePFVM.TransType;
                 return PartialView("~/Areas/WPPF/Views/PFReport/AllEmployeeReportVeiw.cshtml", vm);

             }
             catch (Exception)
             {
                 throw;
             }
         }


         [HttpPost]
         public ActionResult AllEmployee(PFReportVM vm)
         {
             try
             {
                 vm.BranchId = Session["BranchId"].ToString();
                 string rptLocation = "";

                 PFReportRepo _repo = new PFReportRepo();

                 string[] conditionFields = { };
                 string[] conditionValues = { };
           
                 string ReportHead = "Employee Image";
                 DataTable dt = _repo.PFAllEmployee(vm, conditionFields, conditionValues);
                 
                ReportDocument doc = new ReportDocument();

                if (vm.EmployeeId == null)
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptEmployeeImageInfo.rpt";
                }
                else
                {
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptIndividualEmployeeImageInfo.rpt";
                }
             
              

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;                    
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
              
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
             }

             catch (Exception)
             {
                 throw;
             }
         }

    }
}
