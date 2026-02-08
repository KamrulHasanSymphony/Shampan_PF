using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.WPPF;
using SymViewModel.Common;
using SymViewModel.WPPF;
using SymWebUI.Areas.WPPF.Models;
using OfficeOpenXml.Style;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class ProfitDistributionNewController : Controller
    {
        public ProfitDistributionNewController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/ProfitDistribution/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ProfitDistributionNewRepo _repo = new ProfitDistributionNewRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(string PreDistributionFundId)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            Session["PreDistributionFundId"] = PreDistributionFundId;
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id
            //01     //DistributionDate
            //04     //TotalProfit
            //05     //Post
            //06     //IsPaid

            #region Search and Filter Data
            int PreDistributionFundId = Convert.ToInt32(Session["PreDistributionFundId"].ToString());
            var getAllData = _repo.SelectAll(PreDistributionFundId);
            IEnumerable<ProfitDistributionNewVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData.Where(c =>
                    
                       isSearchable1 && c.EmployeeCode.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmployeeName.ToString().ToLower().Contains(param.sSearch.ToLower())

                    || isSearchable3 && c.DistributionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.TotalProfit.ToString().ToLower().Contains(param.sSearch.ToLower())

                    || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.IsPaid.ToString().ToLower().Contains(param.sSearch.ToLower())
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

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProfitDistributionNewVM, string> orderingFunction = (c =>

                sortColumnIndex == 1 && isSortable_1 ? c.EmployeeCode :
                sortColumnIndex == 2 && isSortable_2 ? c.EmployeeName.ToString() :

                sortColumnIndex == 3 && isSortable_3 ? Ordinary.DateToString(c.DistributionDate) :
                sortColumnIndex == 4 && isSortable_4 ? c.TotalProfit.ToString() :

                sortColumnIndex == 4 && isSortable_4 ? c.Post.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.IsPaid.ToString() :
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
                    ,c.EmployeeCode
                    ,c.EmployeeName
                    , c.DistributionDate
                    , c.TotalProfit.ToString()
                    , c.Post ? "Posted" : "Not Posted"
                    , c.IsPaid ? "Paid" : "Not Paid"
     
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




        public ActionResult Process(int PreDistributionFundId = 0)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            PreDistributionFundRepo preDistributionFundRepo = new PreDistributionFundRepo();

            ProfitDistributionNewVM vm = new ProfitDistributionNewVM();
            vm.Operation = "add";
            vm.PreDistributionFundId = PreDistributionFundId.ToString();
          
            vm.PreDistributionFund = preDistributionFundRepo.SelectAll(PreDistributionFundId).FirstOrDefault();

            return View(vm);
        }        

        
        public ActionResult ProcessDistribution(ProfitDistributionNewVM vm)
        {
            try
            {
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
                vm.BranchId = Session["BranchId"].ToString();

                ResultVM result = _repo.Process(vm);

                Session["result"] = result.Status + "~" + result.Message;
            }
            catch (Exception e)
            {
                Session["result"] = "Fail" + "~" + e.Message;

            }

            return RedirectToAction("Process", new { vm.PreDistributionFundId });

        }

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

                dt = new PFReportRepo().ProfitDistributionSummery();
                ExcelPackage excel = new ExcelPackage();
                string FileName = "ProfitDistributionSummery";
                var workSheet = excel.Workbook.Worksheets.Add("Profit Distribution Report");
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name;
                string Line2 = comInfo.Address;
                //string Line3 = "";
                string branchName = Session["BranchName"] != null ? Session["BranchName"].ToString() : "N/A";

                string[] ReportHeaders = new string[] { "Profit Distribution Summery", Line1, Line2, "Branch: " + branchName };

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
                return RedirectToAction("ProfitDistributionNew");
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ProfitDistributionNew");
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
    }
}