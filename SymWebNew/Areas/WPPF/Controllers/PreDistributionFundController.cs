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
using SymReporting.WPPF;
using Newtonsoft.Json;
using SymWebUI.Areas.WPPF.Models;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class PreDistributionFundController : Controller
    {
        public PreDistributionFundController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/PreDistributionFund/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        PreDistributionFundRepo _repo = new PreDistributionFundRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            return View("~/Areas/WPPF/Views/PreDistributionFund/Index.cshtml");

        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment Name information.
        /// </summary>      
        /// <returns>View containing Pre Distribution Fund</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Search and Filter Data

            string[] conditionFields = { "pdf.TransType","pdf.BranchId" };
            string[] conditionValues = { AreaTypePFVM.TransType ,Session["BranchId"].ToString()};

            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);
            IEnumerable<PreDistributionFundVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
            

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.TransactionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.TotalValue.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<PreDistributionFundVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? Ordinary.DateToString(c.Code) :
                sortColumnIndex == 2 && isSortable_2 ? c.TransactionDate.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.TotalValue.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Remarks.ToString() :
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
                , c.TransactionDate.ToString()
                , c.TotalValue.ToString()
                , c.Remarks.ToString()

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
     
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();

            PreDistributionFundVM vm = new PreDistributionFundVM();
            vm.TransType = AreaTypePFVM.TransType;

            vm.Operation = "add";

            return View("~/Areas/WPPF/Views/PreDistributionFund/Create.cshtml",vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the creation and update of Pre-Distribution Fund records.  
        /// Determines the operation type (add/update) based on the `Operation` property of the `PreDistributionFundVM` view model.  
        /// Sets audit fields such as CreatedBy, CreatedAt, LastUpdateBy, etc., and invokes repository methods to persist data.  
        /// </summary>
        /// <param name="vm">The view model containing Pre-Distribution Fund data and operation type (add/update).</param>
        /// <returns>
        /// On successful insert/update, redirects to the Edit view with the new record's ID.  
        /// On failure or validation error, returns the Create view populated with the submitted data.
        /// </returns>
        /// <remarks>
        /// - On "add", the method inserts the record and sets creation metadata.
        /// - On "update", it updates the record and sets last updated metadata.
        /// - The `TransType` is assigned from a shared constant (`AreaTypePFVM.TransType`).  
        /// - If any exception occurs, an error is logged and the view is returned with failure status in the session.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(PreDistributionFundVM vm)
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

                    return View("~/Areas/WPPF/Views/PreDistributionFund/Create.cshtml", vm);

                }

                if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }

                return View("~/Areas/WPPF/Views/PreDistributionFund/Create.cshtml", vm);
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/WPPF/Views/PreDistributionFund/Create.cshtml", vm);
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Loads the Pre-Distribution Fund record for editing by ID and prepares the view model for update operation.  
        /// Also sets the user's permission in the session for role-based access.
        /// </summary>
        /// <param name="id">The unique identifier of the Pre-Distribution Fund record to be edited.</param>
        /// <returns>
        /// Returns the Create view (used for both add/edit) with the populated view model for update operation.
        /// </returns>
        /// <remarks>
        /// - Sets the user's permission using `SymRoleSession` with the screen ID "10003" and action "edit".  
        /// - Loads the record by ID and assigns the `Operation` as "update".  
        /// - Also sets the `TransType` value from a common constant (`AreaTypePFVM.TransType`).
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            PreDistributionFundVM vm = new PreDistributionFundVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("~/Areas/WPPF/Views/PreDistributionFund/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexReturnOnInvestment(string tType)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            PreDistributionFundVM vm = new PreDistributionFundVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.TransactionType = tType;
            return View("~/Areas/WPPF/Views/PreDistributionFund/IndexReturnOnInvestment.cshtml",vm);

        }


        [Authorize(Roles = "Admin")]
        public ActionResult IndexReturnOnBankInterest(string tType)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            PreDistributionFundVM vm = new PreDistributionFundVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.TransactionType = tType;
            return View("~/Areas/WPPF/Views/PreDistributionFund/IndexReturnOnBankInterest.cshtml",vm);

        }

        public ActionResult _indexReturnOnBankInterest(JQueryDataTableParamModel param)
        {
          
            #region Search and Filter Data
            ReturnOnBankInterestRepo _returnOnBankInterestRepo = new ReturnOnBankInterestRepo();

            string[] conditionFields = { "robi.TransType" };
            string[] conditionValues = { AreaTypePFVM.TransType };

            var getAllData = _returnOnBankInterestRepo.SelectAllNotTransferPDF(0, conditionFields, conditionValues);
            IEnumerable<ReturnOnBankInterestVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.BankBranchName.ToLower().Contains(param.sSearch.ToLower())
                    //|| isSearchable2 && c.ROBIDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    //|| isSearchable4 && c.TotalInterestValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<ReturnOnBankInterestVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.BankBranchName.ToString() :              
                sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :
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
                , c.BankBranchName.ToString()                 
                , c.Post ? "Posted":"Not Posted"              
     
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
        public ActionResult IndexReservedFund(string tType)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            PreDistributionFundVM vm = new PreDistributionFundVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.TransactionType = tType;
            return View("~/Areas/WPPF/Views/PreDistributionFund/IndexReturnOnBankInterest.cshtml",vm);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexForfeitureAccount(string tType)
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            PreDistributionFundVM vm = new PreDistributionFundVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.TransactionType = tType;
            return View("~/Areas/WPPF/Views/PreDistributionFund/IndexForfeitureAccount.cshtml",vm);

        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Deletes one or more investment records based on the provided IDs.  
        /// Sets user permission, prepares the view model, and invokes the repository to perform the deletion.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of Pre Disrtibutions fund record IDs to delete</param>
        /// <returns>
        /// A JSON result containing the deletion message returned from the repository
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            PreDistributionFundVM vm = new PreDistributionFundVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypePFVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Posts one or morePre Disrtibutions fun records based on the provided IDs.  
        /// Updates the session permission and invokes the repository post method.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of Pre Disrtibutions fund IDs to post</param>
        /// <returns>
        /// A JSON result containing the post status message from the repository
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            string[] conditionFields = new string[5];
            string[] conditionValues = new string[5];
            string[] id = ids.Split('~');
            var getAllData = _repo.SelectAll(Convert.ToInt32(id[0]) , conditionFields, conditionValues);
            if (getAllData != null && getAllData[0].Post == true)
            {
                string[] resultdata = new string[6];
                resultdata[0] = "Fail";
                resultdata[1] = "Data Already Posted";
                return Json(resultdata, JsonRequestBehavior.AllowGet);             
            }
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
            return View("~/Areas/WPPF/Views/PreDistributionFund/Report.cshtml");

        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates a PDF report for a Pre-Distribution Fund transaction based on the provided ID.
        /// </summary>
        /// <param name="id">The unique identifier of the Pre-Distribution Fund record for which the report is to be generated.</param>
        /// <returns>
        /// A PDF file result of the report rendered using Crystal Reports.
        /// </returns>
        /// <remarks>
        /// - Loads the report data from the database by filtering on `pdf.Id`.  
        /// - If data is found, it generates a report titled "Pre Distribution Fund Transactions"; otherwise, a default no-data message is set.  
        /// - Loads the Crystal Report (`rptPreDistributionFund.rpt`), sets the data source, and applies dynamic formula values such as company logo, name, address, and trans type.
        /// - Finally, renders the report as a PDF and returns the file result.
        /// </remarks>
        [HttpGet]
        public ActionResult ReportView(int id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "pdf.Id"};
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Pre Distribution Fund Transactions";
                }
                dt.TableName = "dtPreDistributionFund";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPreDistributionFund.rpt";

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
                PreDistributionFundVM PreDistributionFundvm = new PreDistributionFundVM();
                PFReport report = new PFReport();
                PreDistributionFundvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = PreDistributionFundvm.Code;
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WPPF/Views/PreDistributionFund/reportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates a PDF report for Pre-Distribution Fund transactions based on filter criteria from the view model.
        /// </summary>
        /// <param name="vm">An instance of <see cref="PFReportVM"/> containing filter parameters like Code, Id, DateFrom, and DateTo.</param>
        /// <returns>
        /// A PDF file result of the Pre-Distribution Fund report rendered using Crystal Reports.
        /// </returns>
        /// <remarks>
        /// - Constructs filtering conditions for the report based on the user-provided parameters.  
        /// - Retrieves the relevant data from the repository using those filters.  
        /// - If data is found, the report header is set to "Pre Distribution Fund Transactions", otherwise a no-data message is shown.  
        /// - Loads the Crystal Report template (`rptPreDistributionFund.rpt`), binds the data, and injects dynamic formula values like company logo, address, name, and trans type.  
        /// - Returns the rendered report as a downloadable PDF file.
        /// </remarks>
        [HttpPost]
        public ActionResult PreDistributionFundReport(PFReportVM vm)
        {


            try
            {
                string ReportHead = "";
                string rptLocation = "";
                PFReport report = new PFReport();


                string[] cFields = { "Code", "pdf.Id", "TransactionDate>", "TransactionDate<", "pdf.TransType"};
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };

                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                WithdrawVM Withdrawvm = new WithdrawVM();

                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Pre Distribution Fund Transactions";
                }
                dt.TableName = "dtPreDistributionFund";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPreDistributionFund.rpt";

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

            result = _repo.InsertAutoJournal("1", "4", Code, BranchId, vm);

            Session["result"] = result[0] + "~" + result[1];

            return View("~/Areas/WPPF/Views/PreDistributionFund/Index.cshtml");
        }
                       
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }


        [HttpGet]
        public ActionResult GetProfitValue(string date)
        {
            try
            {
                PFReportVM vm = new PFReportVM();
                FiscalYearDetailVM fvm = new FiscalYearDetailVM();
                DateTime parsedDate = DateTime.Parse(date);
                PFReportRepo _repo = new PFReportRepo();              
                DataTable dt = new DataTable();              
                DataSet ds = new DataSet();

                FiscalYearRepo fyRepo = new FiscalYearRepo();
               fvm = fyRepo.FiscalYearDetailByDate(date);
               vm.MonthTo = fvm.Id;
               vm.ReportType = "IS";
               vm.TransType = "WPPF";

                ds = _repo.IFRSReports(vm);
                dt = ds.Tables[1];

                PreDistributionFundVM pdvm = new PreDistributionFundVM();
                pdvm.TotalValue = Math.Abs(Convert.ToDecimal(dt.Rows[0]["LastNetProfit"])).ToString();

                return Json(new { success = true, totalValue = pdvm.TotalValue }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }

}

