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

namespace SymWebUI.Areas.BWWF.Controllers
{
    public class InvestmentRenewController : Controller
    {
        public InvestmentRenewController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /BWWF/Investment/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        InvestmentRenewRenewRepo _repo = new InvestmentRenewRenewRepo();
        //InvestmentDetailRepo _repoDetail = new InvestmentDetailRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int investmentId = 0)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            ViewBag.investmentId = investmentId;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }

            InvestmentRenewVM investmentRenewVm = new InvestmentRenewVM() { InvestmentId = investmentId };


            return View("~/Areas/BWWF/Views/InvestmentRenew/Index.cshtml",investmentRenewVm);
        }
        public ActionResult _index(JQueryDataTableParamModel param, int investmentId = 0)
        {
            //00     //Id 
            //01     //ReferenceNo
            //02     //InvestmentType
            //03     //InvestmentName
            //04     //InvestmentDate
            //05     //MaturityDate  
            //06     //InvestmentValue


            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0, new[] { "inv.InvestmentId", "inv.TransType" }, new[] { investmentId.ToString(), AreaTypePFVM.TransType });
            IEnumerable<InvestmentRenewVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.TransactionCode.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.InvestmentDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.ReferenceNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.IsEncashed.ToString().ToLower().Contains(param.sSearch.ToLower())
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

            Func<InvestmentRenewVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.TransactionCode :
                sortColumnIndex == 2 && isSortable_2 ? c.InvestmentDate :
                sortColumnIndex == 3 && isSortable_3 ? c.ReferenceNo :
                sortColumnIndex == 4 && isSortable_4 ? c.ReferenceNo :
                sortColumnIndex == 5 && isSortable_5 ? c.ReferenceNo :
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
                , c.TransactionCode            
                , c.InvestmentDate         
                , c.ReferenceNo         
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
        ////[HttpGet]
        public ActionResult Create(int investmentId = 0, string Encash = "F")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            //vm = _repo.PreInsert(vm);
            InvestmentRenewVM vm = new InvestmentRenewVM();
            vm.InvestmentId = investmentId;
            vm.TransType = AreaTypePFVM.TransType;
            InvestmentRepo investmentRepo = new InvestmentRepo();
            vm.InvestmentVm = investmentRepo.SelectInvstmetForRenual(investmentId).FirstOrDefault();
            vm.IsEncashed = Encash == "T" ? true : false;

            if (investmentId==0)
            {
                Session["result"] = "Fail~Please Select Investment!";

               return RedirectToAction("Index", new { id = "0"});
            }
            vm.Operation = "add";
            return View("~/Areas/BWWF/Views/InvestmentRenew/Create.cshtml", vm);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(InvestmentRenewVM vm)
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
                        return View("~/Areas/BWWF/Views/InvestmentRenew/Create.cshtml", vm);
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
                    return View("~/Areas/BWWF/Views/InvestmentRenew/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/BWWF/Views/InvestmentRenew/Create.cshtml", vm);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            InvestmentRenewVM vm = new InvestmentRenewVM();
           // vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            //vm.detailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));
            vm.InvestmentId = Convert.ToInt32(id);
            InvestmentRepo investmentRepo = new InvestmentRepo();
            //vm.TransType ="BWWF";
            //vm.InvestmentVm = investmentRepo.SelectAll(vm.InvestmentId).FirstOrDefault();
            vm.InvestmentVm = investmentRepo.SelectInvstmetForRenual(vm.InvestmentId).FirstOrDefault();

            vm.Operation = "update";
            return View("~/Areas/BWWF/Views/InvestmentRenew/Create.cshtml", vm);
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
        public JsonResult Encash(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.UpdateEncash(a);

            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetInvestment(string id)
        {
            InvestmentRenewVM vm = new InvestmentRenewVM();

            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            //////string investment = vm.InvestmentValue + "~" + vm.InvestmentRate;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InvestmentRenew(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                if (vm.ReportType == "InvestmentRenew")
                {
                    string[] cFields = { "inv.TransactionCode", "inv.Id", "inv.InvestmentDate>", "inv.InvestmentDate<", "inv.TransType" };
                    string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };
                    var Result = _repo.SelectAll(0, cFields, cValues);

                    dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));
                    ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                    if (dt.Rows.Count > 0)
                    {
                        ReportHead = "Bank Deposit GL Transactions";
                    }
                    dt.TableName = "dtInvestmentRenew";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptPFInvestmentRenew1.rpt";
                }
                else
                {
                    PFReportVM Reportvm = new PFReportVM();

                    string[] cFields = { "inv.TransactionCode", "inv.Id", "inv.InvestmentDate>", "inv.InvestmentDate<", "inv.TransType" };
                    string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                    var Result = _repo.SelectAll(0, cFields, cValues).FirstOrDefault();
                    Reportvm.Id = Result.InvestmentId;
                    Reportvm.TransType = AreaTypePFVM.TransType;

                    dt = new PFReportRepo().InvestmentEncashment(Reportvm, null, null);

                    ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                    if (dt.Rows.Count > 0)
                    {
                        ReportHead = "Investment Encashment Transactions";
                    }
                    dt.TableName = "dtInvestmentRenew";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestmentEncashment.rpt";
                }
                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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
    public ActionResult ReportView(int id)
    {
        try
        {
            string ReportHead = "";
            string rptLocation = "";
            ReportDocument doc = new ReportDocument();
            DataTable dt = new DataTable();
            var Result = _repo.SelectAll(Convert.ToInt32(id));

            dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

            //string[] cFields = { "td.TransactionMasterId", "td.TransactionType" };
            //string[] cValues = { vm.Id.ToString(), vm.TransactionType };
            //dt = _repoGLTransactionDetail.Report(null, cFields, cValues);
            ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
            if (dt.Rows.Count > 0)
            {
                ReportHead = "Bank Deposit GL Transactions";
            }
            dt.TableName = "dtInvestmentRenew";
            rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptPFInvestmentRenew1.rpt";

            doc.Load(rptLocation);
            doc.SetDataSource(dt);
            string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
            FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


            doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
            doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
            doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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
        public ActionResult InvestmentEncashment(string id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                PFReportVM Reportvm = new PFReportVM();
                InvestmentRenewVM vm = new InvestmentRenewVM();
                var Result = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                Reportvm.Id = Result.InvestmentId;

                 dt = new PFReportRepo().InvestmentEncashment(Reportvm, null, null);

                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Investment Encashment Transactions";
                }
                dt.TableName = "dtInvestmentRenew";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptInvestmentEncashment.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;


                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
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
                InvestmentRenewVM InvestmentRenewvm = new InvestmentRenewVM();
                PFReport report = new PFReport();
                InvestmentRenewvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = InvestmentRenewvm.TransactionCode;
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/BWWF/Views/InvestmentRenew/reportVeiw.cshtml", vm);
               
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
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo) };

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
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\\rptWithdraw.rpt";

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

        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
    }
}
