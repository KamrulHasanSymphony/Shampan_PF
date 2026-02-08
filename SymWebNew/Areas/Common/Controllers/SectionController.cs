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
    public class SectionController : Controller
    {
        //
        // GET: /Common/Section/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        SectionRepo _repo = new SectionRepo();
        #region Actions
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_10", "index").ToString();
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Section information.
        /// </summary>      
        /// <returns>View containing Section</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
             #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_3"]);
            var remarksFilter = Convert.ToString(Request["sSearch_4"]);
            var isActiveFilter1 = isActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();
            IEnumerable<SectionVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                     isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable3 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                     );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || nameFilter != "" || isActiveFilter != "" || remarksFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                    && (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))
                    && (remarksFilter == "" || c.Remarks.ToLower().Contains(remarksFilter.ToLower()))
                    );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SectionVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.IsActive.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Remarks :
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
                , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
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

        /// <summary>
        /// Displays the partial view for creating a new record related to permission "1_10".
        /// The method retrieves the current user's permission for the "add" operation 
        /// and stores it in the session under the "permission" key.
        /// </summary>
        /// <returns>
        /// A <see cref="PartialViewResult"/> that renders the creation form.
        /// </returns>
        /// <remarks>
        /// If the user does not have the required permission, the view logic should ideally handle this 
        /// based on the session "permission" value. This method does not currently implement redirection 
        /// or validation logic for permission failure.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_10", "add").ToString();
            return PartialView();
        }

        /// <summary>
        /// Handles the HTTP POST request to create a new section entry.
        /// Populates metadata fields like CreatedAt, CreatedBy, CreatedFrom, and BranchId from the current user's identity.
        /// Attempts to insert the section record using the repository and redirects to the index page with a success or failure message.
        /// </summary>
        /// <param name="vm">The <see cref="SectionVM"/> containing data for the new section.</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> redirecting to the Index action, regardless of success or failure.
        /// On failure, a session variable "result" contains the error message and logs the exception.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(SectionVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(Identity.BranchId);
            try
            {
                
                result = _repo.Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Handles the HTTP GET request to retrieve and display the section data for editing.
        /// Sets the user's permission in the session based on their role.
        /// </summary>
        /// <param name="id">The unique identifier of the section to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the section data retrieved by ID.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_10", "edit").ToString();

            return PartialView(_repo.SelectById(id));
        }

        /// <summary>
        /// Handles the HTTP POST request to update an existing section.
        /// Updates metadata such as last update timestamp, user, and workstation IP.
        /// </summary>
        /// <param name="vm">The <see cref="SectionVM"/> containing the updated section information.</param>
        /// <param name="btn">The button identifier from the form, if applicable.</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> that redirects to the Index action after update.
        /// If an error occurs, logs the exception and also redirects to the Index action.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(SectionVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
            try
            {
                
                result = _repo.Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Updated";
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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_10", "delete").ToString();

            SectionVM vm = new SectionVM();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.Id = id;
            try
            {
                //string[] result = new string[6];
                //result = _repo.Delete(vm);
                //Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Deleted";
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
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult SectionDelete(string ids)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var permission = _reposur.SymRoleSession(identity.UserId, "1_10", "delete").ToString();
            Session["permission"] = permission;
            SectionVM vm = new SectionVM();
           
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion Actions

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

                dt = _repo.SelectSectionInfo(VM);

                #region Excel

                string filename = "SectionInfo Data";
                var workSheet = excel.Workbook.Worksheets.Add("SectionInfo Data");
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

        /// <summary>
        /// Handles the upload of an Excel file to import section data.
        /// Sets creation metadata and calls the repository method to process the file.
        /// </summary>
        /// <param name="vm">The <see cref="SectionVM"/> containing metadata and any data necessary for the import.</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> that redirects to the Index action after processing the Excel file.
        /// If an error occurs, logs the error and still redirects to the Index action.
        /// </returns>
        public ActionResult UploadExcel(SectionVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = new SectionRepo().ImportExcelFile(vm);
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
