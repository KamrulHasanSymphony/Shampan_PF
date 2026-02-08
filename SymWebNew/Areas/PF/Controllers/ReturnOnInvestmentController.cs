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
using SymReporting.PF;
using SymWebUI.Areas.PF.Models;


namespace SymWebUI.Areas.PF.Controllers
{
    public class ReturnOnInvestmentController : Controller
    {
        //
        // GET: /PF/ReturnOnInvestment/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ReturnOnInvestmentRepo _repo = new ReturnOnInvestmentRepo();
        ROIDetailRepo _repoDetail = new ROIDetailRepo();
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/ReturnOnInvestment/Index.cshtml");
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //InvestmentType
            //02     //ReferenceNo
            //03     //ROIDate
            //04     //ROIRate
            //05     //TotalInterestValue  
            //06     //Post
            //07     //IsBankDeposited


            #region Search and Filter Data
            string[] conditionFields = { "roi.TransType" };
            string[] conditionValues = { AreaTypeVM.TransType };

            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            IEnumerable<ReturnOnInvestmentVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.InvestmentType.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.ReferenceNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.ROIDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.ROIRate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.TotalInterestValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.IsBankDeposited.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ReturnOnInvestmentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.InvestmentType.ToString() :
                sortColumnIndex == 2 && isSortable_2 ? c.ReferenceNo.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? Ordinary.DateToString(c.ROIDate) :
                sortColumnIndex == 4 && isSortable_4 ? c.ROIRate.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.TotalInterestValue.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.IsBankDeposited.ToString() :
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
                , c.InvestmentType.ToString()    
                , c.ReferenceNo.ToString()    
                , c.ROIDate.ToString()          
                , c.ROIRate.ToString()          
                , c.TotalInterestValue.ToString()
                , c.Post ? "Posted":"Not Posted"              
                , c.IsBankDeposited?"Deposited":"Not Deposited"
     
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
        //////[HttpGet]
        public ActionResult Create(ReturnOnInvestmentVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            vm.TransType = AreaTypeVM.TransType;
            vm = _repo.PreInsert(vm);

            vm.Operation = "add";
            return View("~/Areas/PF/Views/ReturnOnInvestment/Create.cshtml",vm);
           
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ReturnOnInvestmentVM vm)
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
                        return View("~/Areas/PF/Views/ReturnOnInvestment/Create.cshtml", vm);
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
                    return View("~/Areas/PF/Views/ReturnOnInvestment/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/ReturnOnInvestment/Create.cshtml", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            ReturnOnInvestmentVM vm = new ReturnOnInvestmentVM();
            vm.TransType = AreaTypeVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.detailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));
            vm.Operation = "update";
            return View("~/Areas/PF/Views/ReturnOnInvestment/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            ReturnOnInvestmentVM vm = new ReturnOnInvestmentVM();
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
        public JsonResult GetReturnOnInvestment(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            ReturnOnInvestmentVM vm = new ReturnOnInvestmentVM();

            vm.TransType = AreaTypeVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/PF/Views/ReturnOnInvestment/Report.cshtml");
        }
        [HttpGet]
        public ActionResult ReportView(string dtFrom = "", string dtTo = "", string invId = "", string invTypeId = "")
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                ReturnOnInvestmentVM vm = new ReturnOnInvestmentVM();
                ReturnOnInvestmentRepo _repo = new ReturnOnInvestmentRepo();

                string[] conditionFields = { "roi.ROIDate>", "roi.ROIDate<", "roi.InvestmentId", "inv.InvestmentTypeId","roi.TransType"};
                string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), invId, invTypeId, AreaTypeVM.TransType };
                table = _repo.Report(vm, conditionFields, conditionValues);
                ReportHead = "There are no data to Preview for Return On Investment";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "Return On Investment List";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtReturnOnInvestment";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptReturnOnInvestment.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
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

        [HttpGet]
        public ActionResult ROI_GLTransactionReport(string id)
        {
            try
            {

                PFReportVM vm = new PFReport().ROI_GLTransactionReport(id);

                return File(vm.MemStream, "application/PDF");
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
