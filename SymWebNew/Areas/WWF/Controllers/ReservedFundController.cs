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
using SymWebUI.Areas.WPPF.Models;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class ReservedFundController : Controller
    {
        //
        // GET: /WPPF/ReservedFund/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ReservedFundRepo _repo = new ReservedFundRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            return View("~/Areas/WPPF/Views/ReservedFund/Index.cshtml");
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //ReservedDate
            //02     //ReservedValue
            //03     //TransactionType
            //04     //Post

            #region Search and Filter Data
            string[] conditionFields = { "rf.TransType" };
            string[] conditionValues = { AreaTypeVM.TransType };
            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            IEnumerable<ReservedFundVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.ReservedDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.ReservedValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.TransactionType.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<ReservedFundVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? Ordinary.DateToString(c.ReservedDate) :
                sortColumnIndex == 2 && isSortable_2 ? c.ReservedValue.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.TransactionType :
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
                Convert.ToString(c.Id)
                , c.ReservedDate
                , c.ReservedValue.ToString()
                , c.TransactionType
                , c.Post?"Posted":"Not Posted"
                , c.PDFId.ToString()
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
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            ReservedFundVM vm = new ReservedFundVM();
            vm.TransType = AreaTypeVM.TransType;
            vm.Operation = "add";
            vm.TransactionType = "Other";

            return PartialView("~/Areas/WPPF/Views/ReservedFund/Create.cshtml",vm);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ReservedFundVM vm)
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
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return PartialView("~/Areas/WPPF/Views/ReservedFund/Create.cshtml", vm);
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
                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("~/Areas/WPPF/Views/ReservedFund/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return PartialView("~/Areas/WPPF/Views/ReservedFund/Create.cshtml", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            ReservedFundVM vm = new ReservedFundVM();
            vm.TransType = AreaTypeVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return PartialView("~/Areas/WPPF/Views/ReservedFund/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            ReservedFundVM vm = new ReservedFundVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypeVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Post(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/WPPF/Views/ReservedFund/Report.cshtml");
        }
        [HttpGet]
        public ActionResult ReportView(string dtFrom = "", string dtTo = "")
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                ReservedFundVM vm = new ReservedFundVM();
                ReservedFundRepo _repo = new ReservedFundRepo();

                string[] conditionFields = { "rf.ReservedDate>", "rf.ReservedDate<","rf.TransType" };
                string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), AreaTypeVM.TransType };
                table = _repo.Report(vm, conditionFields, conditionValues);
                ReportHead = "There are no data to Preview for Reserved Fund";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Reserved Fund List";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtReservedFund";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptReservedFund.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                //doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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


        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
    }
}
