using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.WPPF;
using SymViewModel.Common;
using SymViewModel.WPPF;
using SymWebUI.Areas.WPPF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class InvestmentNameController : Controller
    {
        public InvestmentNameController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/InvestmentName/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        InvestmentNameRepo _repo = new InvestmentNameRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }

            return View("~/Areas/WPPF/Views/InvestmentName/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment Name information.
        /// </summary>      
        /// <returns>View containing Investment Name Information</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {           
            #region Search and Filter Data
            string[] conditionFields = { "TransType", "BranchId" };
            string[] conditionValues = { AreaTypePFVM.TransType, Session["BranchId"].ToString() };
            var getAllData = _repo.SelectAll(0,conditionFields,conditionValues);
            IEnumerable<InvestmentNameVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Address.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<InvestmentNameVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.Address :
                sortColumnIndex == 4 && isSortable_4 ? c.Remarks :
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
                , c.Code      
                , c.Name      
                , c.Address 
                , c.Remarks  
     
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

        public ActionResult BlankItem(InvestmentNameDetailsVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {

                return PartialView("~/Areas/WPPF/Views/InvestmentName/_details.cshtml", vm);

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/WPPF/Views/InvestmentName/_details.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/WPPF/Views/InvestmentName/Create.cshtml", vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the creation and update of an Investment Name record based on the provided `InvestmentNameVM` object.  
        /// If the operation is "add", it inserts a new record, and if the operation is "update", it updates the existing record.
        /// </summary>
        /// <param name="vm">An instance of the `InvestmentNameVM` model containing the details of the Investment Name to be added or updated.</param>
        /// <returns>A view or redirection depending on the outcome of the add/update operation.</returns>
        /// <remarks>
        /// - For "add" operation, the record is inserted with creation details, and if successful, the user is redirected to the "Edit" view for the newly created record.
        /// - For "update" operation, the record is updated with modification details, and if successful, the user is redirected to the "Edit" view for the updated record.
        /// - If the operation is invalid or an exception occurs, the view is returned with an error message.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(InvestmentNameVM vm)
        {
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    vm.InvestmentDate = vm.FromDate;
                    vm.MaturityDate = vm.ToDate;
                    vm.BranchId = Session["BranchId"].ToString();
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/WPPF/Views/InvestmentName/Create.cshtml", vm);
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
                    return View("~/Areas/WPPF/Views/InvestmentName/Create.cshtml",vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/WPPF/Views/InvestmentName/Create.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.InvestmentNameDetails = _repo.SelectAllDetails(0, new[] { "InvestmentNameId" }, new[] { vm.Id.ToString() });
            vm.InvestmentAccrueds = new InvestmentAccruedRepo().SelectAll(0, new[] { "I.InvestmentNameId" }, new[] { vm.Id.ToString() });
            vm.Operation = "update";
            return View("~/Areas/WPPF/Views/InvestmentName/Create.cshtml", vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves and prepares data for the "Accrued Process" view for a specific Investment Name and Investment Type.
        /// This method checks user permissions, fetches relevant accrued data, and prepares the model for rendering.
        /// </summary>
        /// <param name="InvestmentNameId">The ID of the Investment Name to be used for filtering accrued data.</param>
        /// <param name="InvestmentTypeId">The ID of the Investment Type to be used for filtering accrued data.</param>
        /// <returns>A partial view for processing accrued data related to the specified Investment Name and Investment Type.</returns>
        /// <remarks>
        /// - The method checks if the user has permission to edit the accrued data using a specific role session.
        /// - It retrieves the accrued data for the given Investment Name and Investment Type from the repository.
        /// - The model is populated with the accrued data and passed to the partial view for display.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AccruedProcessView(string InvestmentNameId, string InvestmentTypeId)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.InvestmentAccrueds = new InvestmentAccruedRepo().SelectAll(0, new[] { "I.InvestmentNameId", "i.id" }, new[] { InvestmentNameId.ToString(), InvestmentTypeId });
            vm.Id = Convert.ToInt32(InvestmentNameId);
            vm.InvestmentTypeId = Convert.ToInt32(InvestmentTypeId);
            vm.Operation = "add";
            
            vm.InvestmentAccrued = vm.InvestmentAccrueds.FirstOrDefault();
            return PartialView("~/Areas/WPPF/Views/InvestmentName/AccruedProcessView.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult JournalEntry(string InterestAmount, string InvestmentNameId, string TransactionDate)
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

            result = _repo.InsertAutoJournal("1", "6", InterestAmount, InvestmentNameId,TransactionDate, BranchId, vm);

            Session["result"] = result[0] + "~" + result[1];

            return View("~/Areas/WPPF/Views/InvestmentName/Index.cshtml");
        }

        [HttpGet]
        public ActionResult LoanSattlementReportVeiw()
        {
            try
            {

                InvestmentNameVM vm = new InvestmentNameVM();
                vm.TransType = AreaTypePFVM.TransType;

                return PartialView("~/Areas/WPPF/Views/InvestmentName/EditProcess.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Processes the accrual data for the Investment Name based on the operation type (add or update).
        /// This method saves or updates accrued data based on the operation provided in the model and redirects the user accordingly.
        /// </summary>
        /// <param name="vm">The InvestmentNameVM object containing the investment details and the operation to perform.</param>
        /// <returns>A redirection to the "Edit" action with the investment ID if the operation is successful, or redirects back with a failure message if an error occurs.</returns>
        /// <remarks>
        /// - If the operation is "add", the method will attempt to add the accrued data and log the result.
        /// - If the operation is not "add", it will simply redirect back to the edit page.
        /// - If an error occurs, it will log the error details and show a failure message in the session.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AccruedProcess(InvestmentNameVM vm)
        {
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;                  
                    result =new InvestmentAccruedRepo().Process(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }
                    else
                    {
                        return RedirectToAction("Edit", new { id = vm.Id });
                    }
                }
                
                else
                {
                    return RedirectToAction("Edit", new { id = vm.Id });
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/WPPF/Views/InvestmentName/Create.cshtml", vm);
            }
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
            InvestmentNameVM vm = new InvestmentNameVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypePFVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvestmentNameInfoLoad(string Id = "0")
        {
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypePFVM.TransType;
            if (!string.IsNullOrWhiteSpace(Id))
            {
                vm = _repo.SelectAll(Convert.ToInt32(Id), null, null).FirstOrDefault();
            }
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a PDF report of investment GL transactions based on the provided investment ID.
        /// </summary>
        /// <param name="id">The investment Name ID for which the report should be generated</param>
        /// <returns>PDF file as ActionResult containing the investment Name report</returns>
         [HttpGet]
        public ActionResult ReportView(string id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds = new PFReportRepo().InvestmentNameReport(Convert.ToInt32(id));

             

                //string[] cFields = { "td.TransactionMasterId", "td.TransactionType" };
                //string[] cValues = { vm.Id.ToString(), vm.TransactionType };
                //dt = _repoGLTransactionDetail.Report(null, cFields, cValues);
                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                ds.Tables[0].TableName = "DtInvestmentName";
                ds.Tables[2].TableName = "DtInvestmentAccrued";
                ds.Tables[1].TableName = "DtInvestmentNameDetails";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptInvestmentRegister.rpt";

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
         /// Generates and downloads an Excel file containing the investment Name report.  
         /// Checks user permission before generating the file.
         /// </summary>
         /// <returns>Redirects to the Investment Name view after file is served or if access is denied</returns>
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
                
                 

                 dt = new PFReportRepo().InvestmentAccruedSummery();
               
                 ExcelPackage excel = new ExcelPackage();
                 string FileName = "Investment";            
                 var workSheet = excel.Workbook.Worksheets.Add("Investment Report");
                 CompanyRepo cRepo = new CompanyRepo();
                 CompanyVM comInfo = cRepo.SelectById(1);
                 string Line1 = comInfo.Name;
                 string Line2 = comInfo.Address;
                 string Line3 = "";

                 string[] ReportHeaders = new string[] { " WPPF Profit Distributions", Line1, Line2, Line3 };

                 ExcelSheetFormat(dt, workSheet, ReportHeaders);

                 using (var memoryStream = new MemoryStream())
                 {
                     Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                     Response.AddHeader("content-disposition", "attachment;  filename=" + FileName + ".xlsx");
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

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
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
        public JsonResult AccruedDelete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypePFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new InvestmentAccruedRepo().Delete(a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);

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
        public JsonResult AccruedPost(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            InvestmentNameVM vm = new InvestmentNameVM();
            vm.TransType = AreaTypePFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new InvestmentAccruedRepo().Post(a);

            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
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

    }
}
