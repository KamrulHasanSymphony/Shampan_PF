using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.BWWF;
using SymViewModel.BWWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymReporting.BWWF;
using Newtonsoft.Json;
using SymWebUI.Areas.BWWF.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SymWebUI.Areas.BWWF.Controllers
{
    public class InvestmentController : Controller
    {
        public InvestmentController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /BWWF/Investment/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        InvestmentRepo _repo = new InvestmentRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            return View("~/Areas/BWWF/Views/Investment/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment information.
        /// </summary>      
        /// <returns>View containing Investment Information</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //ReferenceNo
            //02     //InvestmentType
            //03     //InvestmentName
            //04     //InvestmentDate
            //05     //MaturityDate  
            //06     //InvestmentValue


            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0, new string[] { "inv.TransType", "ina.BranchId" }, new string[] { AreaTypePFVM.TransType, Session["BranchId"].ToString() });
            IEnumerable<InvestmentVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ReferenceNo.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.TransactionCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.InvestmentType.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.InvestmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.InvestmentDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.MaturityDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.InvestmentValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.IsEncashed.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<InvestmentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.ReferenceNo :
                sortColumnIndex == 2 && isSortable_2 ? c.TransactionCode :
                sortColumnIndex == 3 && isSortable_3 ? c.InvestmentType :
                sortColumnIndex == 4 && isSortable_4 ? c.InvestmentName :
                sortColumnIndex == 5 && isSortable_5 ? c.InvestmentDate :
                sortColumnIndex == 6 && isSortable_6 ? c.MaturityDate :
                sortColumnIndex == 7 && isSortable_7 ? c.InvestmentValue.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.Post.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.IsEncashed.ToString() :               
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)
                , c.ReferenceNo            
                , c.TransactionCode            
                , c.InvestmentType         
                , c.InvestmentName         
                , c.InvestmentDate            
                , c.MaturityDate              
                , c.InvestmentValue.ToString()
                , c.Post ? "Posted" : "Not Posted"               
                , c.IsEncashed ? "Encashed" : "Not Encashed"               
     
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
        [Authorize(Roles = "Admin")]
       

        public ActionResult Create(InvestmentVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            //vm = _repo.PreInsert(vm);
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/BWWF/Views/Investment/Create.cshtml", vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles creation and updating of investment records.  
        /// Sets audit fields and determines whether to insert a new record or update an existing one.
        /// </summary>
        /// <param name="vm">The Investment view model containing user input and operation type</param>
        /// <returns>
        /// Redirects to the Edit view on success, or redisplays the Create view with validation messages on failure
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(InvestmentVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    vm.BranchId = Session["BranchId"].ToString();

                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/BWWF/Views/Investment/Create.cshtml", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;

                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("~/Areas/BWWF/Views/Investment/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/BWWF/Views/Investment/Create.cshtml", vm);
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Loads an existing investment record for editing based on the given ID.  
        /// Sets the user’s permission in session and fetches data from the repository.
        /// </summary>
        /// <param name="id">The ID of the investment record to edit (as string, converted to int)</param>
        /// <returns>
        /// Returns the investment data pre-filled in the Create view for update operations
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentVM vm = new InvestmentVM();
            vm.TransType = AreaTypePFVM.TransType;

            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            vm.Operation = "update";
            return View("~/Areas/BWWF/Views/Investment/Create.cshtml", vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Deletes one or more investment records based on the provided IDs.  
        /// Sets user permission, prepares the view model, and invokes the repository to perform the deletion.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of investment record IDs to delete</param>
        /// <returns>
        /// A JSON result containing the deletion message returned from the repository
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentVM vm = new InvestmentVM();
            vm.TransType = AreaTypePFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Posts one or more investment records based on the provided IDs.  
        /// Updates the session permission and invokes the repository post method.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of investment record IDs to post</param>
        /// <returns>
        /// A JSON result containing the post status message from the repository
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Post(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves a specific investment record by ID and returns it as JSON.
        /// </summary>
        /// <param name="id">The ID of the investment record to retrieve</param>
        /// <returns>
        /// A JSON result containing the investment view model (InvestmentVM)
        /// </returns>
        public JsonResult GetInvestment(string id)
        {
            InvestmentVM vm = new InvestmentVM();
            vm.TransType = AreaTypePFVM.TransType;

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            //////string investment = vm.InvestmentValue + "~" + vm.InvestmentRate;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/BWWF/Views/Investment/Report.cshtml");
        }
      
        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a PDF report of investment GL transactions based on the provided investment ID.
        /// </summary>
        /// <param name="id">The investment ID for which the report should be generated</param>
        /// <returns>PDF file as ActionResult containing the investment report</returns>
        [HttpGet]
        public ActionResult ReportView(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();

                string[] cFields = { "i.Id"};
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                    dt = new PFReportRepo().InvestmentHeaderReport(null, cFields, cValues);
               
                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }

                    dt.TableName = "dtInvestmentHeader";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestmentNew.rpt";
              

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and downloads an Excel file containing the investment summary report.  
        /// Checks user permission before generating the file.
        /// </summary>
        /// <returns>Redirects to the InvestmentName view after file is served or if access is denied</returns>
        public ActionResult DownloadExcel()
        {
            string[] result = new string[6];
            DataTable dt = new DataTable();
            try
            {
                SymUserRoleRepo _reposur = new SymUserRoleRepo();
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Common/Home");
                }


                dt = new PFReportRepo().InvestmentSummery();

                ExcelPackage excel = new ExcelPackage();
                string FileName = "InvestmentSummery";
                var workSheet = excel.Workbook.Worksheets.Add("Investment Report");


                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string line1 = comInfo.Name;
                string line2 = comInfo.Address;


                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";


                string[] ReportHeaders = new string[]
                    {
                        "Investment Summary Report",
                        line1,
                        line2,
                        "Branch: " + branchName   
                    };


                ExcelSheetFormat(dt, workSheet, ReportHeaders);


                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment; filename=" + FileName + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                result[0] = "Success";
                result[1] = "Data Saved Successfully!";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("InvestmentName");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("InvestmentName");
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

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Returns a partial view displaying the report view of a specific investment based on its ID.
        /// Retrieves investment information using the provided ID and prepares the PFReportVM.
        /// </summary>
        /// <param name="id">The investment ID for which the report view is to be generated.</param>
        /// <returns>A PartialView containing the report view model.</returns>
        [HttpGet]
        public ActionResult reportVeiw(int id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                InvestmentVM Investmentvm = new InvestmentVM();
                Investmentvm.TransType = AreaTypePFVM.TransType;
                PFReport report = new PFReport();
                Investmentvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = Investmentvm.TransactionCode;
                return PartialView("~/Areas/BWWF/Views/Investment/reportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a PDF Crystal Report for Investment GL Transactions based on provided filter criteria.  
        /// It builds the report using transaction details such as transaction code, ID, investment date range, and type.
        /// </summary>
        /// <param name="vm">An instance of PFReportVM containing filtering criteria like Code, Id, DateFrom, and DateTo.</param>
        /// <returns>A PDF file as an ActionResult containing the Investment GL Transaction Report.</returns>
        [HttpPost]
        public ActionResult Investment_GLTransactionReport(PFReportVM vm)
        {


            try
            {
                string ReportHead = "";
                string rptLocation = "";
                vm.TransType = AreaTypePFVM.TransType;
                PFReport report = new PFReport();


                string[] cFields = { "inv.TransactionCode", "inv.Id", "inv.InvestmentDate>", "inv.InvestmentDate<", "inv.TransactionType" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo),AreaTypePFVM.TransType };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();
                Withdrawvm.TransType = AreaTypePFVM.TransType;

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

              

                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }
                dt.TableName = "dtInvestmentEncashment";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestment.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a PDF Crystal Report for Investment Details based on provided filter criteria.  
        /// Filters include transaction code, ID, and investment date range.
        /// </summary>
        /// <param name="vm">An instance of PFReportVM containing filtering criteria like Code, Id, DateFrom, and DateTo.</param>
        /// <returns>A PDF file as an ActionResult containing the Investment Details Report.</returns>
        [HttpPost]
        public ActionResult Investment_DetailsReport(PFReportVM vm)
        {


            try
            {
                string ReportHead = "";
                string rptLocation = "";
                vm.TransType = AreaTypePFVM.TransType;

                PFReport report = new PFReport();


                string[] cFields = { "inv.TransactionCode", "inv.Id", "inv.InvestmentDate>", "inv.InvestmentDate<" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                 dt = new PFReportRepo().InvestmentDetailsReport(vm, cFields, cValues);




                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }
                dt.TableName = "dtInvestment";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestmentDetails.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a PDF Crystal Report for Investment based on filtering criteria and report type.  
        /// This method handles two types of reports: Header and Detailed. The report content varies based on the `ReportType` parameter.
        /// </summary>
        /// <param name="vm">An instance of PFReportVM containing filtering criteria like Code, Id, DateFrom, DateTo, and ReportType.</param>
        /// <returns>A PDF file as an ActionResult containing the Investment Report.</returns>
         [HttpPost]
        public ActionResult InvestmentReport(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                vm.TransType = AreaTypePFVM.TransType;
                PFReport report = new PFReport();

                string[] cFields = { "i.TransactionCode", "i.Id", "i.InvestmentDate>", "i.InvestmentDate<" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();

                WithdrawVM Withdrawvm = new WithdrawVM();
                if (vm.ReportType=="H")
                {
                    dt = new PFReportRepo().InvestmentHeaderReport(vm, cFields, cValues);
                    ds.Tables.Add(dt);
                }
                else
                {
                    vm.Id = vm.Id;
                    dt = new PFReportRepo().InvestmentDetailsReport(vm, null,null);
                  
                    dt1 = new PFReportRepo().InvestmentNameDetailsReport(dt.Rows[0]["InvestmentNameId"].ToString());
                  
                }

                ReportHead = "There are no data to Preview for GL Transaction for Investment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment GL Transactions";
                }

                if (vm.ReportType == "H")
                {
                    dt.TableName = "dtInvestmentHeader";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestmentNew.rpt";
                }
                else
                {
                    ds.Tables.Add(dt);
                    ds.Tables.Add(dt1);                   
                    dt.TableName = "dtInvestmentDetails";
                    dt1.TableName = "dtInvestmentNameDetails";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestmentDetails.rpt";
                }
               

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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

         /// <summary>
         /// Created: 13 Apr 2025  
         /// Created By: Md Torekul Islam  
         /// Generates and returns a PDF Crystal Report for the Contribution GL Transactions based on the provided `Id`.  
         /// The report is generated using the `PFReportVM` and filters data based on `PFHeaderId`.
         /// </summary>
         /// <param name="Id">The identifier used to filter data for the BWWF Contribution report.</param>
         /// <returns>A PDF file as an ActionResult containing the BWWF Contribution Report.</returns>
        [HttpGet]
        public ActionResult PFContributionReport(string Id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();
                //vm.Id = 1;
                PFReportVM vm=new PFReportVM();
                vm.TransType = AreaTypePFVM.TransType;

                string[] cFields = { "PFHeaderId" };
                string[] cValues = { "1" };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                dt = new PFReportRepo().PFContributionReport(vm, cFields, cValues);
                ReportHead = "There are no data to Preview for GL Transaction for Contribution";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Contribution GL Transactions";
                }
                dt.TableName = "dtPFContribution";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptPFContribution.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult JournalEntry(string Code)
        {
            string[] result = new string[6];

            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();


            string BranchId = Session["BranchId"].ToString();

            ShampanIdentityVM vm = new ShampanIdentityVM
            {
                LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                LastUpdateBy = identity.Name,
                LastUpdateFrom = identity.WorkStationIP,
                CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CreatedBy = identity.Name,
                CreatedFrom = identity.WorkStationIP,
                BranchId = BranchId
            };

            result = _repo.InsertAutoJournal("1", "2", Code, BranchId, vm);

            Session["result"] = result[0] + "~" + result[1];

            return View("~/Areas/BWWF/Views/Investment/Index.cshtml");
        }

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

    }
}
