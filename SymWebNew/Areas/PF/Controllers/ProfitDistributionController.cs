using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using OfficeOpenXml;
using SymWebUI.Areas.PF.Models;

namespace SymWebUI.Areas.PF.Controllers
{
    public class ProfitDistributionController : Controller
    {
        //
        // GET: /PF/ProfitDistribution/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ProfitDistributionRepo _repo = new ProfitDistributionRepo();
        ProfitDistributionDetailRepo _repoDetail = new ProfitDistributionDetailRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/ProfitDistribution/Index.cshtml");
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id
            //01     //DistributionDate
            //02     //TotalEmployeeContribution
            //03     //TotalEmployerContribution
            //04     //TotalProfit
            //05     //Post
            //06     //IsPaid



            #region Search and Filter Data
            string[] conditionFields = { "pd.TransType" };
            string[] conditionValues = { AreaTypeVM.TransType };
            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            IEnumerable<ProfitDistributionVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.DistributionDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.TotalEmployeeContribution.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.TotalEmployerContribution.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<ProfitDistributionVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? Ordinary.DateToString(c.DistributionDate) :
                sortColumnIndex == 2 && isSortable_2 ? c.TotalEmployeeContribution.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.TotalEmployerContribution.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.TotalProfit.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.IsPaid.ToString() :
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
                , c.DistributionDate
                , c.TotalEmployeeContribution.ToString()
                , c.TotalEmployerContribution.ToString()
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


        [Authorize(Roles = "Admin")]
        public ActionResult IndexPreDistributionFund()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/ProfitDistribution/IndexPreDistributionFund.cshtml");

        }
        public ActionResult _indexPreDistributionFund(JQueryDataTableParamModel param)
        {
            //00     //Id
            //01     //FundingDate
            //02     //TotalFundingValue
            //03     //Post
            //04     //IsDistribute
            //05     //TransactionType



            #region Search and Filter Data
            string[] cFields = { "pdf.Post", "pdf.IsDistribute", "pdf.TransType" };
            string[] cValues = { "1", "0" ,AreaTypeVM.TransType};

            PreDistributionFundRepo _repoPreDistributionFund = new PreDistributionFundRepo();
            var getAllData = _repoPreDistributionFund.SelectAll(0, cFields, cValues);
            IEnumerable<PreDistributionFundVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.FundingDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.TotalFundingValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.IsDistribute.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.TransactionType.ToString().ToLower().Contains(param.sSearch.ToLower())

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

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PreDistributionFundVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? Ordinary.DateToString(c.FundingDate) :
                sortColumnIndex == 2 && isSortable_2 ? c.TotalFundingValue.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Post.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.IsDistribute.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.TransactionType.ToString() :

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
                , c.FundingDate
                , c.TotalFundingValue.ToString()
                , c.Post ?"Posted" :"Not Posted"
                , c.IsDistribute ?"Distributed":"Not Distributed"
                , c.TransactionType
     
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
        [HttpPost]
        ////public ActionResult Create(string ids, string TotalProfit)
        public ActionResult Create(ProfitDistributionVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            vm.Operation = "add";
            vm.TransactionType = "Other";
            vm.TransType = AreaTypeVM.TransType;
            vm = _repo.PreSelect(vm);



            if (string.IsNullOrWhiteSpace(vm.PreDistributionFundIds) || vm.TotalEmployeeContribution == 0 || vm.TotalEmployerContribution == 0 || vm.profitDistributionDetailVMs.Count() == 0)
            {
                Session["result"] = "Info" + "~" + "Found No Employee For Distribution!";
                return RedirectToAction("Index");
            }

            return View("~/Areas/PF/Views/ProfitDistribution/Create.cshtml",vm);

        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ProfitDistributionVM vm)
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
                    vm.TransType = AreaTypeVM.TransType;
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/PF/Views/ProfitDistribution/Create.cshtml", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypeVM.TransType;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("~/Areas/PF/Views/ProfitDistribution/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/ProfitDistribution/Create.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            ProfitDistributionVM vm = new ProfitDistributionVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.TransType = AreaTypeVM.TransType;
            vm.profitDistributionDetailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));
            vm.Operation = "update";
            return View("~/Areas/PF/Views/ProfitDistribution/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            ProfitDistributionVM vm = new ProfitDistributionVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypeVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Post(a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Payment(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Payment(a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult GetProfitDistribution(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            ProfitDistributionVM vm = new ProfitDistributionVM();

            vm.TransType = AreaTypeVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExcelReport(ProfitDistributionVM vm)
        {
            ////return null;
            string[] result = new string[6];

            DataTable dtMaster = new DataTable();
            DataTable dtDetail = new DataTable();
            try
            {
                #region Data Fetching
                {
                    ////string[] cFields = { "pd.Id" };
                    ////string[] cValues = { vm.Id.ToString() };
                    vm = _repo.SelectAll(vm.Id).FirstOrDefault();

                }


                {
                    string[] cFields = { "pd.Id" };
                    string[] cValues = { vm.Id.ToString() };
                    dtMaster = _repo.Report(null, cFields, cValues);
                }
                {
                    string[] cFields = { "pdd.ProfitDistributionId" };
                    string[] cValues = { vm.Id.ToString() };
                    dtDetail = _repo.Report_Detail(null, cFields, cValues);
                }
                #endregion

                #region Validations

                if (dtMaster == null || dtMaster.Rows.Count == 0 || dtDetail == null || dtDetail.Rows.Count == 0)
                {
                    result[1] = "No Data Found";
                    throw new ArgumentNullException("", result[1]);
                }
                #endregion



                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Detail");

                #region Row/Column

                int MasterRowStart = 1;

                int MasterRowCount = 0;
                MasterRowCount = dtMaster.Rows.Count;

                int MasterColumnCount = 0;
                MasterColumnCount = dtMaster.Columns.Count;


                int TableHeadRow = 0;
                TableHeadRow = MasterColumnCount + 1 + 3;

                int RowCount = 0;
                RowCount = dtDetail.Rows.Count;

                int ColumnCount = 0;
                ColumnCount = dtDetail.Columns.Count;

                int GrandTotalRow = 0;
                GrandTotalRow = TableHeadRow + RowCount + 1;




                #endregion

                #region Populate Excel Sheet
                int i = MasterRowStart;

                #region Column Name Sentence Case

                Ordinary.DtColumnNameSentenceCase(dtMaster);

                Ordinary.DtColumnNameSentenceCase(dtDetail);

                #endregion

                foreach (DataColumn dc in dtMaster.Columns)
                {
                    workSheet.Cells[i, 1].Value = dc.ColumnName;
                    workSheet.Cells[i, 2].Value = dtMaster.Rows[0][dc.ColumnName];
                    i++;
                }

                ////workSheet.Cells[1, 1].LoadFromDataTable(dtMaster, true);



                workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dtDetail, true);

                workSheet.Row(TableHeadRow).Style.WrapText = true;

                #endregion

                #region Format

                int colNumber = 0;

                foreach (DataColumn col in dtDetail.Columns)
                {
                    colNumber++;
                    if (col.DataType == typeof(DateTime))
                    {
                        workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy";
                    }
                    else if (col.DataType == typeof(Decimal))
                    {

                        workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                        if (col.ColumnName != "Service Length Month" && col.ColumnName != "Service Length Month Weight" && col.ColumnName != "Multiplication Factor")
                        {
                            #region Grand Total
                            workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                            #endregion

                        }
                    }

                }
                #endregion


                #region Download Process

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + "Profit Distribution: " + vm.PeriodNameFrom + " to " + vm.PeriodNameTo + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Success";
                result[1] = "Successful~Data Download";
                #endregion

            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = ex.Message;
            }
            finally
            {

            }

            Session["result"] = result[0] + "~" + result[1];

            return Redirect("Index");
        }


    }
}
