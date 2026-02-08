using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.WWF;
using SymRepository.Common;
using SymViewModel.WWF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SymReporting.WWF;
using Newtonsoft.Json;
using SymWebUI.Areas.WWF.Models;

namespace SymWebUI.Areas.WWF.Controllers
{
    public class LoanSattlementController : Controller
    {
        public LoanSattlementController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WWF/Loan/

        LoanSattlementRepo _repo = new LoanSattlementRepo();

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10010", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WWF/Home");
            }

            LoanSattlementVM vm = new LoanSattlementVM();
            vm.TransType = AreaTypePFVM.TransType;

            return View("~/Areas/WWF/Views/LoanSattlement/Index.cshtml",vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param)
        {

            #region Search and Filter Data

            string[] conditionFields = { "TransType" };
            string[] conditionValues = { AreaTypePFVM.TransType };

            var getAllData = _repo.SelectAll(0, conditionFields,conditionValues);
            IEnumerable<LoanSattlementVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransactionDate.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Amount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.InterestAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.ReferenceNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().Contains(param.sSearch.ToLower())
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
            Func<LoanSattlementVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TransactionDate :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Amount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.InterestAmount.ToString() :
                                                           sortColumnIndex == 4 && isSortable_5 ? c.Post.ToString() :
                                                           sortColumnIndex == 5 && isSortable_6 ? c.Remarks :
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
                , c.TransactionDate
                , c.Amount.ToString()
                , c.InterestAmount.ToString()
                , c.ReferenceNo               
                , c.Post ? "Posted" : "Not Posted"               
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
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "add").ToString();
            LoanSattlementVM vm = new LoanSattlementVM();

            vm.TransType = AreaTypePFVM.TransType;

            vm.Operation = "add";
            return View("~/Areas/WWF/Views/LoanSattlement/Create.cshtml", vm);
        }

        [HttpPost]
        public ActionResult CreateEdit(LoanSattlementVM vm)
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
                    result = _repo.Insert(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        vm.Id = 0;
                        return View("~/Areas/WWF/Views/LoanSattlement/Create.cshtml", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    result = _repo.Update(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("~/Areas/WWF/Views/LoanSattlement/Create.cshtml", vm);
                    }
                }
                else
                {
                    return View("~/Areas/WWF/Views/LoanSattlement/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data not Successfully";
                return View("~/Areas/WWF/Views/LoanSattlement/Create.cshtml", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            LoanSattlementVM vm = new LoanSattlementVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id.Split('~')[0]), null, null).FirstOrDefault();

            vm.Operation = "update";
            return View("~/Areas/WWF/Views/LoanSattlement/Create.cshtml", vm);
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




        [HttpGet]
        public ActionResult ReportView(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for Transaction Loan";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Loan Transactions";
                }
                dt.TableName = "dtLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WWF\\rptLoanSattlement.rpt";

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
                LoanSattlementVM LoanMonthlyPaymentvm = new LoanSattlementVM();
                PFReport report = new PFReport();
                LoanMonthlyPaymentvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = LoanMonthlyPaymentvm.Code;
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WWF/Views/LoanSattlement/reportVeiw.cshtml", vm);
               
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult LoanSattlementReport(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "Code", "Id", "TransactionDate>", "TransactionDate<","TransType" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for Transaction LoanMonthlyPayment";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "LoanMonthlyPayment Transactions";
                }
                dt.TableName = "dtLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WWF\\rptLoanSattlement.rpt";

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
