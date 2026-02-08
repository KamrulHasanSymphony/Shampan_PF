using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.WPPF;
using SymRepository.Common;
using SymViewModel.WPPF;
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

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class EETransactionController : Controller
    {
        //
        // GET: /WPPF/EETransaction/

        EETransactionRepo _repo = new EETransactionRepo();
        EETransactionDetailRepo _detailRepo = new EETransactionDetailRepo();

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string tType)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10010", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            if (string.IsNullOrWhiteSpace(tType))
            {
                tType = "";
            }
            EETransactionVM vm = new EETransactionVM();
            vm.TransactionType = tType;
            return View(vm);
        }
        public ActionResult _index(JQueryDataTableParamVM param, string tType)
        {

            #region Search and Filter Data
            string[] conditionField = { "TransactionType", "BranchId" };
            string[] conditionValue = { tType, identity.BranchId };

            var getAllData = _repo.SelectAll(0, conditionField, conditionValue);
            IEnumerable<EETransactionVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransactionDateTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.GrandTotal.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
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
            Func<EETransactionVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.TransactionDateTime :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.GrandTotal.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Post.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)+"~"+ c.Post
                , c.Code
                , c.TransactionDateTime
                , c.GrandTotal.ToString()
                , c.Post ? "Posted" : "Not Posted"               
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
        public ViewResult BlankItem(int EEHeadId = 0, decimal STotal = 0)
        {
            EEHeadRepo prepo = new EEHeadRepo();
            EEHeadVM pVM = prepo.SelectAll(EEHeadId).FirstOrDefault();
            EETransactionDetailVM vm = new EETransactionDetailVM();
            vm.EEHeadId = EEHeadId;
            vm.EEHeadName = pVM.Name;
            vm.SubTotal = STotal;
            return View("_details", vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "add").ToString();
            EETransactionVM vm = new EETransactionVM();
            List<EETransactionDetailVM> EETransactionDVMs = new List<EETransactionDetailVM>();
            vm.eeTransactionDetailVMs = EETransactionDVMs;
            vm.TransactionType = tType;
            vm.Operation = "add";
            return View(vm);
        }

        [HttpPost]
        public ActionResult CreateEdit(EETransactionVM vm)
        {
            string[] result = new string[6];
            vm.BranchId = Convert.ToInt32(identity.BranchId);

            try
            {
                if (vm.Operation.ToLower() == "add")
                {

                    vm.IsPS = identity.IsESS;
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
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
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    if (result[0] == "Success")
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1];
                        return View("Create", vm);
                    }
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data not Successfully";
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            EETransactionVM vm = new EETransactionVM();
            vm = _repo.SelectAll(Convert.ToInt32(id), null, null).FirstOrDefault();

            List<EETransactionDetailVM> EETransactionDVMs = new List<EETransactionDetailVM>();
            EETransactionDVMs = _detailRepo.SelectAll(0, Convert.ToInt32(id), null, null, identity.IsESS);
            vm.eeTransactionDetailVMs = EETransactionDVMs;
            vm.TransactionType = "other";
            vm.Operation = "update";
            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "delete").ToString();
            EETransactionVM vm = new EETransactionVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
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
        public ActionResult Report(string tType)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            EETransactionVM vm = new EETransactionVM();
            vm.Operation = "report";
            vm.TransactionType = tType;
            return PartialView("_report", vm);
        }
        [HttpGet]
        public ActionResult ReportView(string Code, string TDF, string TDT, string RT, string tType, string branchId, string postStatus)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                int BranchId = 0;
                if (branchId == "-1")
                {
                    BranchId = 0;
                }
                else if (!string.IsNullOrWhiteSpace(branchId))
                {
                    BranchId = Convert.ToInt32(branchId);
                }
                else
                {
                    BranchId = Convert.ToInt32(identity.BranchId);
                }

                if (RT == "M")
                {
                    
                    EETransactionVM vm = new EETransactionVM();
                    EETransactionRepo _repo = new EETransactionRepo();

                    vm.IsPS = identity.IsESS;
                    vm.BranchId = BranchId;
                    vm.PostStatus = postStatus;
                    string[] conditionFields = { "tr.TransactionType", "tr.Code", "tr.TransactionDateTime>", "tr.TransactionDateTime<" };
                    string[] conditionValues = { tType, Code, Ordinary.DateToString(TDF), Ordinary.DateToString(TDT) };
                    
                    table = _repo.Report(vm, conditionFields, conditionValues);
                    ReportHead = "There are no data to Preview for Earning Expense (EETransaction)";
                    if (table.Rows.Count > 0)
                    {
                        ReportHead = "Earning Expense (EETransaction) List";
                    }
                    ds.Tables.Add(table);
                    ds.Tables[0].TableName = "dtEETransaction";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptEETransaction.rpt";
                }
                else if (RT == "D")
                {
                    EETransactionDetailVM dVM = new EETransactionDetailVM();
                    EETransactionDetailRepo _detailRepo = new EETransactionDetailRepo();
                    dVM.IsPS = identity.IsESS;
                    dVM.BranchId = BranchId;
                    dVM.PostStatus = postStatus;
                    string[] conditionFields = { "t.TransactionType", "t.Code", "t.TransactionDateTime>", "t.TransactionDateTime<" };
                    string[] conditionValues = { tType, Code, Ordinary.DateToString(TDF), Ordinary.DateToString(TDT) };
                    
                    table = _detailRepo.Report(dVM, conditionFields, conditionValues);

                    ReportHead = "There are no data to Preview for Earning Expense Transaction Detail";
                    if (table.Rows.Count > 0)
                    {
                        ReportHead = "Earning Expense Transaction Detail List";
                    }
                    ds.Tables.Add(table);
                    ds.Tables[0].TableName = "dtEETransactionDetail";
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptEETransactionDetail.rpt";
                }
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                //doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                //doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                //var branch = new SymRepository.Acc.BranchRepo().SelectAll(Convert.ToInt32(identity.BranchId)).FirstOrDefault();
                //doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + branch.Name + "'";
                //doc.DataDefinition.FormulaFields["BranchAddress"].Text = "'" + branch.Address + " (" + branch.ContactPersonTelephone + ")" + "'";

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
