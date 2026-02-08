using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.WWF;
using SymViewModel.WWF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using Newtonsoft.Json;
using SymReporting.WWF;
using SymWebUI.Areas.WWF.Models;
namespace SymWebUI.Areas.WWF.Controllers
{
    public class PFBankDepositController : Controller
    {
        public PFBankDepositController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WWF/PFBankDeposit/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        PFBankDepositRepo _repo = new PFBankDepositRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WWF/Home");
            }
            return View("~/Areas/WWF/Views/PFBankDeposit/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Investment Name information.
        /// </summary>      
        /// <returns>View containing WWF Bank Deposit</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
        
            #region Search and Filter Data

            string[] conditionFields = { "pfbd.TransType" };
            string[] conditionValues = { AreaTypePFVM.TransType };

            var getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

            IEnumerable<PFBankDepositVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.Code.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.DepositDate.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.DepositAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.BankBranchName.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<PFBankDepositVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.DepositDate) :
                sortColumnIndex == 3 && isSortable_3 ? c.DepositAmount.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.BankBranchName :
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
                Convert.ToString(c.Id)
                , c.Code
                , c.DepositDate
                , c.DepositAmount.ToString()
                , c.BankBranchName
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
        ////[HttpGet]
        public ActionResult Create(PFBankDepositVM vm)////string tType = "")
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();

           
                vm.TransactionType = "Other";
           

            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/WWF/Views/PFBankDeposit/Create.cshtml",vm);

        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the creation and editing of PFBankDeposit entries. Based on the operation type ("add" or "update"), it either inserts a new record or updates an existing one.
        /// This action method updates relevant metadata such as creation and update timestamps, user details, and transaction type before interacting with the repository.
        /// </summary>
        /// <param name="vm">The ViewModel representing the bank deposit details that will be created or updated.</param>
        /// <returns>A redirection to the "Edit" view of the corresponding entry if the operation is successful, or a re-rendering of the current "Create" view if the operation fails.</returns>
        /// <remarks>
        /// - The method distinguishes between "add" and "update" operations and sets the corresponding metadata, such as `CreatedAt`, `CreatedBy`, `LastUpdateAt`, and `LastUpdateBy`.
        /// - If the insert or update operation succeeds, the user is redirected to the edit page of the created/updated entry.
        /// - If an error occurs, a failure message is logged, and the user is shown the "Create" view again.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(PFBankDepositVM vm)
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
                        return View("~/Areas/WWF/Views/PFBankDeposit/Create.cshtml", vm);
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
                    return View("~/Areas/WWF/Views/PFBankDeposit/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/WWF/Views/PFBankDeposit/Create.cshtml", vm);
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves the data of a specific PFBankDeposit record for editing. The record is selected based on the provided `id`, and its details are passed to the "Create" view for further modifications.
        /// This action ensures that the user has the necessary permissions to edit the record before proceeding.
        /// </summary>
        /// <param name="id">The ID of the PFBankDeposit record to be edited.</param>
        /// <returns>The "Create" view with the populated ViewModel for editing the selected PFBankDeposit record.</returns>
        /// <remarks>
        /// - This method checks the user's permission before allowing access to the edit functionality.
        /// - It retrieves the specific PFBankDeposit record from the repository and sets the `Operation` property to "update" for indicating an edit operation.
        /// - If the record is found, it is passed to the "Create" view for the user to modify.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            PFBankDepositVM vm = new PFBankDepositVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.TransType = AreaTypePFVM.TransType;

            vm.Operation = "update";
            return View("~/Areas/WWF/Views/PFBankDeposit/Create.cshtml", vm);
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
            PFBankDepositVM vm = new PFBankDepositVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.TransType = AreaTypePFVM.TransType;
            result = _repo.Delete(vm, a);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Posts one or more Bank Deposit records based on the provided IDs.  
        /// Updates the session permission and invokes the repository post method.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of Bank Deposit record IDs to post</param>
        /// <returns>
        /// A JSON result containing the post status message from the repository
        /// </returns>
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
        public JsonResult GetPFBankDeposit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            PFBankDepositVM vm = new PFBankDepositVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View("~/Areas/WWF/Views/PFBankDeposit/Report.cshtml");
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a report preview for Bank Deposit GL Transactions. The report is based on the `id` parameter and retrieves the corresponding data for bank deposits. If data is found, the report is generated and returned as a PDF. If no data is found, a message indicating no data for the preview is shown.
        /// </summary>
        /// <param name="id">The ID of the Bank Deposit record to generate the report for. If the ID is 0, all records are considered for the report generation.</param>
        /// <returns>A PDF report of Bank Deposit GL Transactions, or a message indicating no data if no records are found.</returns>
        /// <remarks>
        /// - This method retrieves Bank Deposit data based on the provided ID and generates a report using Crystal Reports.
        /// - The report includes fields like company logo, transaction type, and a custom header based on the retrieved data.
        /// - If no records are found, the report header indicates "There are no data to Preview for GL Transaction for Bank Deposit".
        /// </remarks>
        [HttpGet]
        public ActionResult ReportView(int id)
        {

            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();

                //PFBankDepositVM vm = new PFBankDepositVM();
                string[] cFields = {  "pfbd.Id" };
                string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };
                var Result = _repo.SelectAll(0, cFields, cValues);

                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));


                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtPFBankDeposits";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WWF\\rptBankDeposit.rpt";

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
                PFBankDepositVM PFBankDepositvm = new PFBankDepositVM();
                PFReport report = new PFReport();
                PFBankDepositvm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
                vm.Id = id;
                vm.Code = PFBankDepositvm.Code;
                vm.TransType = AreaTypePFVM.TransType;
                return PartialView("~/Areas/WWF/Views/PFBankDeposit/reportVeiw.cshtml", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a report preview for Bank Deposit GL Transactions based on filter criteria provided in the `PFReportVM` view model. The report is customized with company information and filters based on deposit date range, transaction type, and other parameters. If data is found, the report is generated and returned as a PDF; if no data is found, a message indicating no data is shown.
        /// </summary>
        /// <param name="vm">A view model containing filter criteria for the report, including Code, Id, Deposit Date range (`DateFrom`, `DateTo`), and Transaction Type.</param>
        /// <returns>A PDF report of Bank Deposit GL Transactions, or a message indicating no data if no records match the criteria.</returns>
        /// <remarks>
        /// - This method generates a report for Bank Deposit GL transactions, using criteria like deposit code, date range, and transaction type.
        /// - If the provided filter results in data, the report is generated using a Crystal Reports template located in the project directory.
        /// - The report header includes details like the company logo, company name, and address from the `CompanyRepo`.
        /// </remarks>
        [HttpPost]
        public ActionResult PFBankDepositReport(PFReportVM vm)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable dt = new DataTable();
                
                //PFBankDepositVM vm = new PFBankDepositVM();
                string[] cFields = { "pfbd.Code", "pfbd.Id", "pfbd.DepositDate>", "pfbd.DepositDate<", "pfbd.TransType" };
                string[] cValues = { vm.Code, vm.Id.ToString() == "0" ? "" : vm.Id.ToString(), Ordinary.DateToString(vm.DateFrom), Ordinary.DateToString(vm.DateTo), AreaTypePFVM.TransType };
                var Result = _repo.SelectAll(0, cFields, cValues);

               dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

                
                ReportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                if (dt.Rows.Count > 0)
                {
                    ReportHead = "Bank Deposit GL Transactions";
                }
                dt.TableName = "dtPFBankDeposits";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WWF\\rptBankDeposit.rpt";

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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
