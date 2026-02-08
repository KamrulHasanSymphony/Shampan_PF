using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using OfficeOpenXml;

using SymViewModel.HRM;
using System.Data;
namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        //
        // GET: /Common/Branch/
        BranchRepo _repo = new BranchRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        BranchRepo branchRepo;
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_8", "index").ToString();

            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Branch information.
        /// </summary>      
        /// <returns>View containing Branch</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            branchRepo = new BranchRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var cityFilter = Convert.ToString(Request["sSearch_3"]);
            var mobileFilter = Convert.ToString(Request["sSearch_4"]);
            //Code
            //Name
            //City
            //Mobile



            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data


            var getAllData = branchRepo.SelectAll();
            IEnumerable<BranchVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c => 
                    isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.City.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || nameFilter != "" || cityFilter != "" || mobileFilter != "") 
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                                            &&
                                            (cityFilter == "" || c.City.ToLower().Contains(cityFilter.ToLower()))
                                            &&
                                            (mobileFilter == "" || c.Mobile.ToLower().Contains(mobileFilter.ToLower()))
                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<BranchVM, string> orderingFunction = (c => 
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.City :
                sortColumnIndex == 4 && isSortable_4 ? c.Mobile :
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
                , c.Name
                , c.City
                , c.Mobile
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
        /// Displays the view for creating a new entry. 
        /// This method checks the user's permission for the "add" action in the "1_7" role. 
        /// If the user does not have the appropriate permissions, they are redirected to a different page.
        /// </summary>
        /// <returns>
        /// The "Create" view, or a redirect to another page if the user lacks the required permissions.
        /// </returns>
        /// <remarks>
        /// This action method fetches the user's permission status for adding an entry (via the `SymRoleSession` method).
        /// If the user has the necessary permissions, the "Create" view is returned. 
        /// If the user does not have permission, they are redirected to a common home page.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "add").ToString();
            return View();
        }

        /// <summary>
        /// Creates a new branch entry. This method accepts a `BranchVM` model, populates some additional fields 
        /// such as `CreatedAt`, `CreatedBy`, and `CreatedFrom` before calling the repository's `Insert` method to save the data.
        /// If the operation is successful, the user is redirected to the "Index" page, otherwise an error message is logged and 
        /// the same view is returned with the model.
        /// </summary>
        /// <param name="BranchVM">The branch view model containing the data to be created.</param>
        /// <returns>
        /// If the insertion is successful, redirects to the "Index" action.
        /// If the insertion fails, logs the error and returns the same view with the model.
        /// </returns>
        /// <remarks>
        /// The method uses the `BranchRepo`'s `Insert` method to save the branch details. In case of an exception, 
        /// the error details are logged using the `FileLogger` class and a failure message is stored in the session.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(BranchVM BranchVM)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            BranchVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            BranchVM.CreatedBy = identity.Name;
            BranchVM.CreatedFrom = identity.WorkStationIP;
            try
            {
                
                result = new BranchRepo().Insert(BranchVM);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(BranchVM);
            }
        }

        /// <summary>
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves department data by ID.
        /// </summary>
        /// <param name="id">The ID of the Branch to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="BranchVM"/> to populate the edit form.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "edit").ToString();
            BranchVM vm = new BranchVM();
            vm = _repo.SelectById(id);
            return View(vm);
        }

        /// <summary>
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves Branch data by ID.
        /// </summary>
        /// <param name="id">The ID of the Branch to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="BranchVM"/> to populate the edit form.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(BranchVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                result = new BranchRepo().Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0] == "Fail")
                {
                    return View(vm);
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
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
        public JsonResult BranchDelete(string ids)
        {
            BranchVM vm = new BranchVM();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new BranchRepo().Delete(vm, a);
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

                dt = _repo.SelectBranchInfo(VM);

                #region Excel

                string filename = "BranchInfo Data";
                var workSheet = excel.Workbook.Worksheets.Add("BranchInfo Data");
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

        public ActionResult UploadExcel(BranchVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = new BranchRepo().ImportExcelFile(vm);
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
