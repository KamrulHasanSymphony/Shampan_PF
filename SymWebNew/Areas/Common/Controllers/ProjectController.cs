using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        //
        // GET: /Common/Project/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        ProjectRepo proRepo = new ProjectRepo();
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_9", "index").ToString();
//            return View(proRepo.SelectAll());
            return View();
        }
        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Project information.
        /// </summary>      
        /// <returns>View containing Project</returns>
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name)
        {
            #region Column Search
            //var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var contactPersonFilter = Convert.ToString(Request["sSearch_3"]);
            var mobileFilter = Convert.ToString(Request["sSearch_4"]);
            var manpowerFilter = Convert.ToString(Request["sSearch_5"]);
            var startdateFilter = Convert.ToString(Request["sSearch_6"]);
            //Code
            //Name
            //ContactPerson
            //Mobile
            //ManpowerRequired
            //Startdate
            DateTime fromStartDate = DateTime.MinValue;
            DateTime toStartDate = DateTime.MaxValue;
            if (startdateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromStartDate = startdateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(startdateFilter.Split('~')[0]) == true ? Convert.ToDateTime(startdateFilter.Split('~')[0]) : DateTime.MinValue;
                toStartDate = startdateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(startdateFilter.Split('~')[1]) == true ? Convert.ToDateTime(startdateFilter.Split('~')[1]) : DateTime.MinValue;
            }
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = proRepo.SelectAll();
            IEnumerable<ProjectVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData.Where(c => 
                    isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.ContactPerson.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.ManpowerRequired.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.Startdate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (codeFilter != "" || nameFilter != "" || contactPersonFilter != "" || mobileFilter != "" || manpowerFilter != "" || (startdateFilter != "" && startdateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                                            &&
                                            (contactPersonFilter == "" || c.ContactPerson.ToLower().Contains(contactPersonFilter.ToLower()))
                                            &&
                                            (mobileFilter == "" || c.Mobile.ToLower().Contains(mobileFilter.ToLower()))
                                            &&
                                            (manpowerFilter == "" || c.ManpowerRequired.ToString().ToLower().Contains(manpowerFilter.ToLower()))
                                            &&
                                            (fromStartDate == DateTime.MinValue || fromStartDate <= Convert.ToDateTime(c.Startdate))
                                            &&
                                            (toStartDate == DateTime.MaxValue || toStartDate >= Convert.ToDateTime(c.Startdate))
                                        );
            }
            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ProjectVM, string> orderingFunction = (c => 
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.ContactPerson :
                sortColumnIndex == 4 && isSortable_4 ? c.Mobile :
                sortColumnIndex == 5 && isSortable_5 ? c.ManpowerRequired.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? Ordinary.DateToString(c.Startdate) :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                Convert.ToString(c.Id)
                , c.Code
                , c.Name //+ "~" + Convert.ToString(c.Id)
                , c.ContactPerson
                , c.Mobile
                , c.ManpowerRequired.ToString()
                , c.Startdate.ToString()
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

        /// <summary>
        /// Renders the partial view for creating a new designation group.
        /// Retrieves and sets the user's permission in the session for adding a designation group.
        /// </summary>
        /// <returns>
        /// A <see cref="PartialViewResult"/> that renders the Create view.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_9", "add").ToString();
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request to create a new department.
        /// Populates audit fields and attempts to insert the department data using the repository.
        /// Logs and stores any exception-related details in session and log file.
        /// </summary>
        /// <param name="vm">The <see cref="ProjectVM"/> view model containing the new department data.</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> that redirects to the Index action.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(ProjectVM projectVM)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            projectVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            projectVM.CreatedBy = identity.Name;
            projectVM.CreatedFrom = identity.WorkStationIP;
            projectVM.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                result = proRepo.Insert(projectVM);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves Project data by ID.
        /// </summary>
        /// <param name="id">The ID of the Project to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="ProjectVM"/> to populate the edit form.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_9", "edit").ToString();
            return View(proRepo.SelectById(id));
        }

        /// <summary>
        /// Handles the HTTP POST request to update an existing Project's information.
        /// Sets metadata fields and attempts to update the Project record using the repository.
        /// Logs error and sets session result in case of failure.
        /// </summary>
        /// <param name="vm">The <see cref="ProjectVM"/> containing the updated department data.</param>
        /// <param name="btn">Optional button parameter for extended functionality (currently unused).</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> that redirects to the Index view.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(ProjectVM projectVM)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            projectVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            projectVM.LastUpdateBy = identity.Name;
            projectVM.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                result = proRepo.Update(projectVM);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Deletes settlement policies based on the provided IDs. 
        /// Checks the user's permission before performing the delete operation.
        /// </summary>
        /// <param name="ids">A string containing the IDs of the settlement policies to be deleted, separated by a '~' character.</param>
        /// <returns>
        /// A JSON result containing the status message (e.g., success or failure) of the delete operation.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult ProjectDelete(string ids)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_9", "delete").ToString();
            ProjectVM vm = new ProjectVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = proRepo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        public ActionResult Import()
        {
            return View();
        }

        public ActionResult ExportExcell(ExportImportVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            DataTable dt = new DataTable();
            ExcelPackage excel = new ExcelPackage();

            try
            {

                ExportImportRepo _repo = new ExportImportRepo();

                dt = _repo.SelectProjectInfo(VM);

                #region Excel

                string filename = "ProjectInfo Data";
                var workSheet = excel.Workbook.Worksheets.Add("ProjectInfo Data");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                #endregion

            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");
                return RedirectToAction("Index");
            }

            finally { }
            return RedirectToAction("Index");

            // return Json(rVM, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UploadExcel(ProjectVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = new ProjectRepo().ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }
    }
}
