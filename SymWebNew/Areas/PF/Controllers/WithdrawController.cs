using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.PF;
using SymRepository.Common;
using SymViewModel.PF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using SymReporting.PF;
using Newtonsoft.Json;
using SymWebUI.Areas.PF.Models;
namespace SymWebUI.Areas.PF.Controllers
{
    public class WithdrawController : Controller
    {
        public WithdrawController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/Withdraw/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        WithdrawRepo _repo = new WithdrawRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/Withdraw/Index.cshtml");

        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            //00     //Id 
            //01     //WithdrawDate
            //02     //WithdrawAmount
            //03     //TransactionType
            //04     //Post

            #region Search and Filter Data

            string[] conditionFields = { "w.TransType" };
            string[] conditionValues = { AreaTypePFVM.TransType };
            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            IEnumerable<WithdrawVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.WithdrawDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.WithdrawAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<WithdrawVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code.ToString() :
                sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.WithdrawDate) :
                sortColumnIndex == 3 && isSortable_3 ? c.WithdrawAmount.ToString() :
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
                , c.Code 
                , c.WithdrawDate 
                , c.WithdrawAmount.ToString()
                , c.Post?"Posted":"Not Posted"
     
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
            WithdrawVM vm = new WithdrawVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/PF/Views/Withdraw/Create.cshtml",vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(WithdrawVM vm)
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

                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/PF/Views/Withdraw/Create.cshtml", vm);
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
                    return View("~/Areas/PF/Views/Withdraw/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/PF/Views/Withdraw/Create.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            WithdrawVM vm = new WithdrawVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            //vm.detailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));

            vm.Operation = "update";
            return View("~/Areas/PF/Views/Withdraw/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            WithdrawVM vm = new WithdrawVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypePFVM.TransType;

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
        public JsonResult GetAvailableBalance(WithdrawVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            vm.TransType = AreaTypePFVM.TransType;

            vm = _repo.SelectAvailableBalance(vm);

            return Json(vm, JsonRequestBehavior.AllowGet);

            ////vm.
        }

        [Authorize(Roles = "Admin")]
        public JsonResult GetWithdraw(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            WithdrawVM vm = new WithdrawVM();

            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/PF/Views/Withdraw/Report.cshtml");
        }

        //[HttpGet]
        //public ActionResult ReportView(string dtFrom = "", string dtTo = "", string bankBranchId = "")
        //{
        //    try
        //    {
        //        string ReportHead = "";
        //        string rptLocation = "";
        //        ReportDocument doc = new ReportDocument();
        //        DataTable table = new DataTable();
        //        DataSet ds = new DataSet();
        //        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        //        WithdrawVM vm = new WithdrawVM();
        //        WithdrawRepo _repo = new WithdrawRepo();

        //        string[] conditionFields = { "w.WithdrawDate>", "w.WithdrawDate<", "w.AccountId" };
        //        string[] conditionValues = { Ordinary.DateToString(dtFrom), Ordinary.DateToString(dtTo), bankBranchId };
        //        table = _repo.Report(vm, conditionFields, conditionValues);
        //        ReportHead = "There are no data to Preview for Withdraw ";
        //        if (table.Rows.Count > 0)
        //        {
        //            ReportHead = "Withdraw List";
        //        }
        //        ds.Tables.Add(table);
        //        ds.Tables[0].TableName = "dtWithdraw";
        //        rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptWithdraw.rpt";

        //        doc.Load(rptLocation);
        //        doc.SetDataSource(ds);
        //        string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
        //        doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
        //        doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
        //        //doc.DataDefinition.FormulaFields["frmGroupBy"].Text = "'" + groupBy + "'";
        //        var rpt = RenderReportAsPDF(doc);
        //        doc.Close();
        //        return rpt;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpGet]
        public ActionResult ReportView(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "w.Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();
                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtWithdraw";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptWithdraw.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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

        [HttpGet]
        public ActionResult reportVeiw(int id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                WithdrawVM Withdrawvm = new WithdrawVM();
                PFReport report = new PFReport();
                string[] cFields = { "w.Id", };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                Withdrawvm = _repo.SelectAll(0,cFields,cValues).FirstOrDefault();
                vm.Id = id;
                vm.Code = Withdrawvm.Code;
                return PartialView("~/Areas/PF/Views/Withdraw/reportVeiw.cshtml", vm);


            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Withdraw_GLTransactionReport(PFReportVM vm)
        {
           

            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "w.Code", "w.Id", "w.WithdrawDate>", "w.WithdrawDate<", "w.TransType" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

                
                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtWithdraw";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\PF\\rptWithdraw.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
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



        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

    }
}
