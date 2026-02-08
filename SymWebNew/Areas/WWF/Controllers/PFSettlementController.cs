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
using SymReporting.WWF;
using SymWebUI.Areas.WWF.Models;

namespace SymWebUI.Areas.WWF.Controllers
{
    public class PFSettlementController : Controller
    {
        public PFSettlementController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WWF/PFSettlement/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        PFSettlementRepo _repo = new PFSettlementRepo();
        PFSettlementDetailRepo _repoDetail = new PFSettlementDetailRepo();


        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WWF/Home");
            }
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all WWF Settlement information.
        /// </summary>      
        /// <returns>View containing WWF Settlement</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
           
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<PFSettlementVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.TotalPayableAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.AlreadyPaidAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.NetPayAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable8 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFSettlementVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.TotalPayableAmount.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.AlreadyPaidAmount.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.NetPayAmount.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.Post.ToString() :
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
                , c.EmpName
                , c.Designation
                , c.Department
                , c.TotalPayableAmount.ToString()
                , c.AlreadyPaidAmount.ToString()
                , c.NetPayAmount.ToString()
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
        public ActionResult IndexResignEmployee()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WWF/Home");
            }
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Resign Employee information.
        /// </summary>      
        /// <returns>View containing Resign Employee</returns>
        public ActionResult _indexResignEmployee(JQueryDataTableParamModel param)
        {
             #region Search and Filter Data
           
            PFSettlementRepo _repoPFSettlement = new PFSettlementRepo();
           

            string[] conditionFields = { "ve.BranchId" };
            string[] conditionValues = { Session["BranchId"].ToString() };

            var getAllData = _repoPFSettlement.SelectAll_ResignEmployee(conditionFields, conditionValues);
            IEnumerable<PFSettlementVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.EmpResignDate.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<PFSettlementVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.EmpResignDate) :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.EmployeeId)
                , c.Code
                , c.EmpName
                , c.Designation
                , c.Department
                , c.EmpResignDate
     
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
        public ActionResult Create(PFSettlementVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();

            vm = _repo.PreInsert(vm);

            vm.Operation = "add";
            return View(vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the creation and updating of WWF Settlement records based on the provided view model.
        /// </summary>
        /// <param name="vm">An instance of <see cref="PFSettlementVM"/> containing the settlement data and operation type ("add" or "update").</param>
        /// <returns>
        /// Redirects to the Edit view on successful save, or returns the Create view with the current model on failure.
        /// </returns>
        /// <remarks>
        /// - If the operation is "add", it sets the creation metadata, calls the repository's insert method, and redirects on success.  
        /// - If the operation is "update", it sets the update metadata, calls the repository's update method, and redirects.  
        /// - On exception, it logs the error and returns the Create view with a failure message.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(PFSettlementVM vm)
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
                    vm.BranchId = Session["BranchId"].ToString();
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Loads a WWF Settlement record and its associated details for editing based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the WWF Settlement record to be edited.</param>
        /// <returns>
        /// Returns the Create view pre-filled with the settlement and its detail data for editing.
        /// </returns>
        /// <remarks>
        /// - Sets the user's edit permission from the session.  
        /// - Retrieves the main WWF settlement record and its detail records from the repository.  
        /// - Sets the operation mode to "update" to distinguish between add and edit in the view.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            PFSettlementVM vm = new PFSettlementVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.detailVMs = _repoDetail.SelectByMasterId(Convert.ToInt32(id));

            vm.Operation = "update";
            return View("Create", vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Posts one or more WWF Settlemant records based on the provided IDs.  
        /// Updates the session permission and invokes the repository post method.
        /// </summary>
        /// <param name="ids">Tilde (~) separated string of WWF Settlemant fund IDs to post</param>
        /// <returns>
        /// A JSON result containing the post status message from the repository
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {


            string[] conditionFields = new string[5];
            string[] conditionValues = new string[5];
            string[] id = ids.Split('~');
            var getAllData = _repo.SelectAll(Convert.ToInt32(id[0]), conditionFields, conditionValues);
            if (getAllData != null && getAllData[0].Post == true)
            {
                string[] resultdata = new string[6];
                resultdata[0] = "Fail";
                resultdata[1] = "Data Already Posted";
                return Json(resultdata, JsonRequestBehavior.AllowGet);
                //return Json("Data already posted", JsonRequestBehavior.AllowGet);
            }
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string Id = ids[0].ToString();
            string[] result = new string[6];
            result = _repo.Post(Id);
            return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves a WWF Settlement record by its ID and returns it as JSON.
        /// </summary>
        /// <param name="id">The ID of the WWF Settlement record to retrieve.</param>
        /// <returns>
        /// A JsonResult containing the WWF Settlement data.
        /// </returns>
        /// <remarks>
        /// - Sets the edit permission for the current user session.  
        /// - Retrieves the WWF settlement data from the repository using the given ID.  
        /// - Used typically for AJAX calls to fetch WWF settlement details dynamically.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        public JsonResult GetPFSettlement(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            PFSettlementVM vm = new PFSettlementVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a PDF report for a specific WWF Settlement record.
        /// </summary>
        /// <param name="Id">The ID of the WWF Settlement record to generate the report for.</param>
        /// <returns>
        /// A PDF file rendered as an ActionResult containing the WWF Settlement report.
        /// </returns>
        /// <remarks>
        /// - Loads data for the specified WWF Settlement using the repository.  
        /// - If no data found, sets a default report header indicating empty results.  
        /// - Populates a DataSet with the data and loads the Crystal Report template.  
        /// - Injects company logo and report header into the report.  
        /// - Converts the report to a PDF and returns it.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ReportView(string Id)
        {
            try
            {
                string ReportHead = "";
                string rptLocation = "";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                PFSettlementVM vm = new PFSettlementVM();
                PFSettlementRepo _repo = new PFSettlementRepo();

                vm.Id = Convert.ToInt32(Id);
                table = _repo.Report(vm);
                ReportHead = "There are no data to Preview for WWF Settlement";
                if (table.Rows.Count > 0)
                {
                    ReportHead = "WWF Settlement Statement";
                }
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtPFSettlement";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WWF\\rptPFSettlement.rpt";


                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
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
                LastUpdateAt = DateTime.Now.ToString("yyyy/MM/dd/HH/mm"),
                LastUpdateBy = identity.Name,
                LastUpdateFrom = identity.WorkStationIP,
                CreatedAt = DateTime.Now.ToString("yyyy/MM/dd/HH/mm"),
                CreatedBy = identity.Name,
                CreatedFrom = identity.WorkStationIP,
                BranchId = BranchId
            };

            result = _repo.InsertAutoJournal("1", "4", Code, BranchId, vm);

            Session["result"] = result[0] + "~" + result[1];

            return View("~/Areas/WWF/Views/PFSettlement/Index.cshtml");
        }
   
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }
    
    }
}
