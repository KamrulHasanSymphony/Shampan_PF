using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymViewModel.Common;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;

namespace SymWebUI.Areas.Common.Controllers
{
    public class DesignationGroupController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        DesignationGroupRepo _repo = new DesignationGroupRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;      
        // GET: /Common/DesignationGroup/

        public ActionResult Index()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_5", "index").ToString();
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Designation Group information.
        /// </summary>      
        /// <returns>View containing Designation Group</returns>
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
            IEnumerable<DesignationGroupVM> filteredData;

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
            Func<DesignationGroupVM, string> orderingFunction = (c =>
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
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)
                , c.Code//+"~"+c.Name+"~"+(c.IsActive  ? "Active" : "Inactive")+"~"+c.Remarks
                , c.Name
                , c.IsActive ? "Active" : "Inactive" 
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
        /// Loads the partial view for creating a new record (e.g., user or entity).
        /// Sets the user permission for the "add" action in the session based on the user's role.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="PartialViewResult"/> for the creation form.
        /// Redirects may occur if permission logic is handled elsewhere in the view or controller.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_5", "add").ToString();
            return PartialView();
        }

        /// <summary>
        /// Handles the creation of a new designation group record.
        /// This method sets the necessary audit fields like CreatedAt, CreatedBy, and CreatedFrom.
        /// It attempts to insert the data into the database, and based on the result, redirects to the "Index" action.
        /// </summary>
        /// <param name="vm">The DesignationGroupVM object containing the data for the new designation group.</param>
        /// <returns>
        /// Redirects to the "Index" action with a result message in the session, indicating the success or failure of the operation.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an issue during the insertion process, and logs the error.</exception>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(DesignationGroupVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
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
        /// Retrieves the details of a designation group by its ID and prepares it for editing.
        /// This method checks the user's permission to edit designation groups before loading the data.
        /// </summary>
        /// <param name="id">The unique identifier for the designation group to be edited.</param>
        /// <returns>
        /// Returns a partial view with the DesignationGroupVM object populated with the data for the selected designation group.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an issue while retrieving the designation group data.</exception>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_5", "edit").ToString();
            DesignationGroupVM vm = new DesignationGroupVM();
            vm = _repo.SelectById(id);
            return PartialView(vm);
        }

        /// <summary>
        /// Updates the details of an existing designation group based on the provided data.
        /// This method checks the user's session information and logs the update operation.
        /// If the update fails, an error is logged and the user is redirected back to the index page.
        /// </summary>
        /// <param name="vm">The DesignationGroupVM object containing the updated details of the designation group.</param>
        /// <param name="btn">The name of the button clicked during the update process (e.g., "Save").</param>
        /// <returns>
        /// Redirects to the "Index" action after attempting to update the designation group.
        /// The result of the update operation is stored in the session for further reference.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if the update process fails and an error is logged.</exception>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(DesignationGroupVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
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
        /// Deletes a designation group based on the provided ID.
        /// This method checks the user's session information and logs the delete operation.
        /// If the delete operation fails, an error is logged, and the user is redirected back to the index page.
        /// </summary>
        /// <param name="id">The ID of the designation group to be deleted.</param>
        /// <returns>
        /// Redirects to the "Index" action after attempting to delete the designation group.
        /// If the operation fails, an error message is stored in the session for further reference.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if the delete operation fails and an error is logged.</exception>
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(string id)
        {

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_5", "delete").ToString();
            DesignationGroupVM vm = new DesignationGroupVM();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.Id = id;
            try
            {
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Deleted";
                return RedirectToAction("Index");
            }

        }


        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult DepartmentDelete(string ids)
        {
            DesignationGroupVM vm = new DesignationGroupVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_5", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public JsonResult Areer(string value, string date, string employeeId)
        {

            return Json("successed", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Import()
        {
            return View();
        }

        /// <summary>
        /// Exports the Designation Group data to an Excel file based on the provided ExportImportVM.
        /// The data is retrieved from the database and written to an Excel file, which is then served to the user as an attachment.
        /// The method handles errors and sets session variables for feedback.
        /// </summary>
        /// <param name="VM">The view model containing the parameters for fetching the data to export, such as filters for the Designation Group info.</param>
        /// <returns>
        /// A file download response containing the exported Excel file. If an error occurs, the user is redirected to the Index page with a failure message.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an issue generating the Excel file or retrieving the data.</exception>
        public ActionResult ExportExcell(ExportImportVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            DataTable dt = new DataTable();
            ExcelPackage excel = new ExcelPackage();

            try
            {

                ExportImportRepo _repo = new ExportImportRepo();

                dt = _repo.SelectDesignationGroupInfo(VM);

                #region Excel

                string filename = "DesignationGroupInfo Data";
                var workSheet = excel.Workbook.Worksheets.Add("DesignationGroupInfo Data");
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
        /// Handles the upload and import of an Excel file containing Designation Group data. 
        /// The method processes the uploaded file and updates the database with the contents.
        /// If the operation is successful, the user is redirected to the Index page with a success message.
        /// If an error occurs, the user is redirected to the Index page with a failure message, and the error is logged.
        /// </summary>
        /// <param name="vm">The view model containing the uploaded Excel file and metadata such as creation time and user details.</param>
        /// <returns>
        /// A redirect to the Index action with a success or failure message stored in the session.
        /// </returns>
        /// <exception cref="Exception">Throws an exception if there is an issue processing the file or interacting with the database.</exception>
        public ActionResult UploadExcel(DesignationGroupVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                result = new DesignationGroupRepo().ImportExcelFile(vm);
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
