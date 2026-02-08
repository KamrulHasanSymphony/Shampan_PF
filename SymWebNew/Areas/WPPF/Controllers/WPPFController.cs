using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymReporting.WPPF;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Newtonsoft.Json;
using SymWebUI.Areas.WPPF.Models;
using SymRepository.Payroll;
using System.Web;
using SymRepository.Enum;
using SymViewModel.Enum;
using System.Configuration;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class WPPFController : Controller
    {
        public WPPFController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/WPPF/
        WPPFRepo _repo = new WPPFRepo();
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public ActionResult IndexFiscalPeriod(string EmployeeId = "", string fydid = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;        

            return View();
        }

        public ActionResult _indexFiscalPeriod(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "", bool isWWF = false)
        {
            WPPFRepo _repo = new WPPFRepo();
            List<PFHeaderVM> getAllData = new List<PFHeaderVM>();
            IEnumerable<PFHeaderVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string[] conditionFields = { "pfd.EmployeeId", "pfd.FiscalYearDetailId" };
            string[] conditionValues = { EmployeeId, fydid };
            string TransType = AreaTypeWPPFVM.TransType;
            getAllData = _repo.SelectFiscalPeriodHeader(TransType, conditionFields, conditionValues);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                // Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
            

                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.ProjectName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.TotalPF.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                       
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
          

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFHeaderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.ProjectName :
                sortColumnIndex == 3 && isSortable_3 ? c.FiscalPeriod :
                sortColumnIndex == 4 && isSortable_4 ? c.TotalPF.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :
               
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                     c.Id.ToString()
                    , c.Code
                    , c.ProjectName
                    , c.FiscalPeriod
                    , c.TotalPF.ToString()
                    , c.Post ? "Yes" : "No"
                 };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
             JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(decimal? TotalProfit, string fydid, int? FiscalYear)
        {
            if (string.IsNullOrEmpty(fydid))
                return View();

            if (TotalProfit == null || TotalProfit <= 0)
                return Json("Fail~Invalid Total Profit", JsonRequestBehavior.AllowGet);
           
            ShampanIdentityVM vm = new ShampanIdentityVM
            {
                CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CreatedBy = identity.Name,
                CreatedFrom = identity.WorkStationIP,
                BranchId = Session["BranchId"].ToString()
            };

            string TransType = AreaTypeWPPFVM.TransType;
            WPPFRepo repo = new WPPFRepo();
            string[] result = repo.Insert(TotalProfit, fydid, FiscalYear,TransType, vm);

            string mgs = result[0] + "~" + result[1];
            Session["result"] = mgs;

            return Json(mgs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProfitDistribution(string code = "")
        {
            try
            {

                ViewBag.code = code;


                WPPFRepo _repo = new WPPFRepo();
                string[] conditionFields = { "b.Code" };
                string[] conditionValues = { code.ToString() };

                string TransType = AreaTypeWPPFVM.TransType;
                var pfh = _repo.SelectProfitDistribution(TransType, conditionFields, conditionValues).FirstOrDefault();
                ViewBag.PeriodName = pfh.FiscalPeriod + " (" + pfh.ProjectName + " )";

                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult _indexProfitDistribution(JQueryDataTableParamModel param, string code = "")
        {

            WPPFRepo _repo = new WPPFRepo();
            List<PFHeaderVM> getAllData = new List<PFHeaderVM>();
            IEnumerable<PFHeaderVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string[] conditionFields = { "b.Code" };
            string[] conditionValues = { code.ToString() };
            string TransType = AreaTypeWPPFVM.TransType;
            getAllData = _repo.SelectProfitDistribution(TransType, conditionFields, conditionValues);

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.ProjectName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.TotalPF.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFHeaderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.ProjectName :
                sortColumnIndex == 3 && isSortable_3 ? c.TotalPF.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Post.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 //c.Id.ToString()
                 c.Code
                , c.ProjectName
                ,c.TotalPF.ToString()
                , c.Post?"Yes":"No"
            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
             JsonRequestBehavior.AllowGet);
        }

        public JsonResult Post(string ids)
        {
            try
            {
                // Fetch user permission for editing the PF Header based on role and permissions
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

                // Check if the user has permission; if not, handle appropriately
                if (Session["permission"].ToString() == "False")
                {
                    return Json("Permission Denied", JsonRequestBehavior.AllowGet);
                }

                // Initialize the PFHeaderVM and set the Id from the provided string (split by ~)
                PFHeaderVM headerVm = new PFHeaderVM
                {
                    Id = Convert.ToInt32(ids.Split('~')[0])
                };
                string TransType = AreaTypeWPPFVM.TransType;
                // Prepare the result variable to hold the outcome of the post operation
                string[] result = _repo.PostHeader(TransType, headerVm);

                // Return the second result element (status message) as the response
                return Json(result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json("Error: {ex.Message}", JsonRequestBehavior.AllowGet);
            }
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        public ActionResult AllWPPFReport(PFHeaderVM vm)
        {
            string[] result = new string[6];
            try
            {
                #region Try

                #region Objects and Variables

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _repoSUR.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                string TransType = AreaTypeWPPFVM.TransType;
                // RepoCall → load all WPPF headers without filtering
                WPPFRepo _repo = new WPPFRepo();
                var getAllData = _repo.SelectAll(TransType);

                dt = Ordinary.ListToDataTable(getAllData.ToList());

                #endregion


                #region Report Call

                EnumReportRepo _reportRepo = new EnumReportRepo();
             
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = "";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                              @"Files\ReportFiles\PF\rptWPPFReport.rpt";

                doc.Load(rptLocation);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
           
                #endregion

                dt.TableName = "dtWPPFReport";

                doc.SetDataSource(dt);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;

                #endregion Try
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];

                FileLogger.Log("WPPFReport", this.GetType().Name,
                               ex.Message + Environment.NewLine + ex.StackTrace);

                return View();
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
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Left;
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

                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                        workSheet.Cells[TableHeadRow + 1, colNumber]
                                                                            .Address + ":" +
                                                                        workSheet.Cells[(TableHeadRow + RowCount),
                                                                            colNumber].Address + ")";

                    #endregion
                }
            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)]
                .Style
                .Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style
                .Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

            #endregion
        }   

        public ActionResult DownloadAllWPPFReport()
        {
            string[] result = new string[6];
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try
            {
                #region Objects and Variables

                ReportDocument doc = new ReportDocument();

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _repoSUR.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string FileName = "WPPF Profit Value Report.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";

                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                #endregion


                #region Pull Data
                string TransType = AreaTypeWPPFVM.TransType;
                WPPFRepo _repo = new WPPFRepo();
                var getAllData = _repo.SelectAll(TransType);

                dt = Ordinary.ListToDataTable(getAllData.ToList());

                // Remove unwanted columns
                string[] toRemove =
{
    "Id",
    "FiscalYearDetailId",
    "DistributionDate",
    "EmployeePFValue",
    "EmployeerPFValue",
    "TotalEmployeeValue",
    "Post",
    "Remarks",
    "IsActive",
    "IsArchive",
    "CreatedBy",
    "CreatedAt",
    "CreatedFrom",
    "LastUpdateBy",
    "LastUpdateAt",
    "LastUpdateFrom",
    "TotalEmployerValue",
    "Operation",
    "ProjectId",
    "PeriodStart",
    "PeriodEnd",
    "TransType",
    "DistributedValue"
};


                foreach (string col in toRemove)
                {
                    if (dt.Columns.Contains(col))
                        dt.Columns.Remove(col);
                }

                // Rename safely
                if (dt.Columns.Contains("Code"))
                    dt.Columns["Code"].ColumnName = "Code";
                if (dt.Columns.Contains("ProjectName"))
                    dt.Columns["ProjectName"].ColumnName = "Project Name";
                if (dt.Columns.Contains("FiscalPeriod"))
                    dt.Columns["FiscalPeriod"].ColumnName = "Fiscal Period";

                if (dt.Columns.Contains("TotalPF"))
                    dt.Columns["TotalPF"].ColumnName = "Total Profit";

                #endregion


                #region Validations

                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return View();
                }

                #endregion


                #region Report Info

                string filename = "WPPF Profit Value Excel Report";

                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);

                string Line1 = comInfo.Name;
                string Line2 = comInfo.Address;
                string Line3 = "";

                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                #endregion


                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                ExcelSheetFormat(dt, workSheet, ReportHeaders);

                #endregion


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


                #region Redirect

                result[0] = "Success";
                result[1] = "Successful~Excel Downloaded";

                Session["result"] = result[0] + "~" + result[1];
                return Redirect("/PF/WPPF/IndexFiscalPeriod");

                #endregion
            }
            catch (Exception e)
            {
                Session["result"] = "Fail~Process Fail";

                FileLogger.Log("DownloadAllWPPFReport", this.GetType().Name, e.Message + Environment.NewLine + e.StackTrace);

                return Redirect("/PF/WPPF/IndexFiscalPeriod");
            }
        }

        public ActionResult AllWPPFProfitDistributionReport(string code = "")
        {
            string[] result = new string[6];
            try
            {
                #region Try

                #region Objects and Variables

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _repoSUR.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                
                WPPFRepo _repo = new WPPFRepo();

                string[] conditionFields = { "b.Code" };
                string[] conditionValues = { code };
                string TransType = AreaTypeWPPFVM.TransType;
                var getAllData = _repo.SelectProfitDistribution(TransType, conditionFields, conditionValues);

                dt = Ordinary.ListToDataTable(getAllData.ToList());

                #endregion


                #region Report Call

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = AppDomain.CurrentDomain.BaseDirectory +
                                     @"Files\ReportFiles\PF\rptWPPFProfitDistribution.rpt";

                doc.Load(rptLocation);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";

                #endregion

                dt.TableName = "dtWPPFProfitDistribution";

                doc.SetDataSource(dt);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;

                #endregion Try
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];

                FileLogger.Log("WPPFProfitDistributionReport", this.GetType().Name,
                               ex.Message + Environment.NewLine + ex.StackTrace);

                return View();
            }
        }

        public ActionResult DownloadAllWPPFProfitDistributionReport(string code = "")
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();

            try
            {
                #region Objects and Variables

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _repoSUR.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                string FileName = "WPPF Profit Distribution Report.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";

                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                #endregion


                #region Pull Data

                WPPFRepo _repo = new WPPFRepo();

                // If 'code' is provided, filter by it, otherwise fetch all data
                string[] conditionFields = { "b.Code" };
                string[] conditionValues = { code };
                string TransType = AreaTypeWPPFVM.TransType;
                var getAllData = _repo.SelectProfitDistribution(TransType, conditionFields, conditionValues);
                dt = Ordinary.ListToDataTable(getAllData.ToList());


                // Remove unwanted columns
                var toRemove = new string[]
                        {
                            "Id",
    "FiscalYearDetailId",
    "DistributionDate",
    "EmployeePFValue",
    "EmployeerPFValue",
    "TotalEmployeeValue",
    "Post",
    "Remarks",
    "IsActive",
    "IsArchive",
    "CreatedBy",
    "CreatedAt",
    "CreatedFrom",
    "LastUpdateBy",
    "LastUpdateAt",
    "LastUpdateFrom",
    "TotalEmployerValue",
    "Operation",
    "ProjectId",
    "PeriodStart",
    "PeriodEnd",
    "TransType",
    "DistributedValue",
    "FiscalPeriod"
                        };

                foreach (string col in toRemove)
                {
                    if (dt.Columns.Contains(col))
                        dt.Columns.Remove(col);
                }

                // Rename columns for Excel
                dt.Columns["Code"].ColumnName = "Code";
                dt.Columns["ProjectName"].ColumnName = "Employee Name";
                dt.Columns["TotalPF"].ColumnName = "Employee Profit";

                #endregion


                #region Validations

                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];
                    return View();
                }

                #endregion


                #region Excel Header Setup
                string filename = "WPPF Profit Distribution_ExcelReport";
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);

                string Line1 = comInfo.Name;
                string Line2 = comInfo.Address;
                string Line3 = "";

                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                #endregion


                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                ExcelSheetFormat(dt, workSheet, ReportHeaders);

                #endregion


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


                #region Redirect

                result[0] = "Success";
                result[1] = "Successful~Excel Downloaded";
                Session["result"] = result[0] + "~" + result[1];

                return Redirect("/PF/WPPF/ProfitDistribution");

                #endregion
            }
            catch (Exception e)
            {
                Session["result"] = "Fail~Process Fail";
                FileLogger.Log("DownloadAllWPPFProfitDistributionReport", this.GetType().Name, e.Message);
                return Redirect("/PF/WPPF/ProfitDistribution");
            }
        }
    }
}
