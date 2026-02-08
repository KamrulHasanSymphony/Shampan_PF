using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;

using SymViewModel.Common;
using SymViewModel.HRM;
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
    public class DesignationController : Controller
    {
        //
        // GET: /HRM/Designation/

        DesignationRepo _repo = new DesignationRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        #region Actions
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_6", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Designation information.
        /// </summary>      
        /// <returns>View containing Designation</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var AttendenceBonusFilter = Convert.ToString(Request["sSearch_3"]);
            var EPZFilter = Convert.ToString(Request["sSearch_4"]);
            var OtherFilter = Convert.ToString(Request["sSearch_5"]);
            var IfterAmountFilter = Convert.ToString(Request["sSearch_6"]);
            var DinnerAmountFilter = Convert.ToString(Request["sSearch_7"]);
            var TiffinAmountFilter = Convert.ToString(Request["sSearch_8"]);
            var ETiffinAmountFilter = Convert.ToString(Request["sSearch_9"]);

            var OTAlloawanceFilter = Convert.ToString(Request["sSearch_10"]);
            var OTOrginalFilter = Convert.ToString(Request["sSearch_11"]);
            var OTBayerFilter = Convert.ToString(Request["sSearch_12"]);
            var ExtraOTFilter = Convert.ToString(Request["sSearch_13"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_14"]);
            var OrderNoFilter = Convert.ToString(Request["sSearch_15"]);
            var remarksFilter = Convert.ToString(Request["sSearch_16"]);
            #region From-To
            var AttendenceBonusFrom = 0;
            var AttendenceBonusTo = 0;
            if (AttendenceBonusFilter.Contains('~'))
            {
                AttendenceBonusFrom = AttendenceBonusFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(AttendenceBonusFilter.Split('~')[0]) == true ? Convert.ToInt32(AttendenceBonusFilter.Split('~')[0]) : 0;
                AttendenceBonusTo = AttendenceBonusFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(AttendenceBonusFilter.Split('~')[1]) == true ? Convert.ToInt32(AttendenceBonusFilter.Split('~')[1]) : 0;
            }
            var EPZFrom = 0;
            var EPZTo = 0;
            if (EPZFilter.Contains('~'))
            {
                EPZFrom = EPZFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(EPZFilter.Split('~')[0]) == true ? Convert.ToInt32(EPZFilter.Split('~')[0]) : 0;
                EPZTo = EPZFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(EPZFilter.Split('~')[1]) == true ? Convert.ToInt32(EPZFilter.Split('~')[1]) : 0;
            }
            var OtherFrom = 0;
            var OtherTo = 0;
            if (OtherFilter.Contains('~'))
            {
                OtherFrom = OtherFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(OtherFilter.Split('~')[0]) == true ? Convert.ToInt32(OtherFilter.Split('~')[0]) : 0;
                OtherTo = OtherFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(OtherFilter.Split('~')[1]) == true ? Convert.ToInt32(OtherFilter.Split('~')[1]) : 0;
            }
            var IfterAmountFrom = 0;
            var IfterAmountTo = 0;
            if (IfterAmountFilter.Contains('~'))
            {
                IfterAmountFrom = IfterAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(IfterAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(IfterAmountFilter.Split('~')[0]) : 0;
                IfterAmountTo = IfterAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(IfterAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(IfterAmountFilter.Split('~')[1]) : 0;
            }
            var DinnerAmountFrom = 0;
            var DinnerAmountTo = 0;
            if (DinnerAmountFilter.Contains('~'))
            {
                DinnerAmountFrom = DinnerAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(DinnerAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(DinnerAmountFilter.Split('~')[0]) : 0;
                DinnerAmountTo = DinnerAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(DinnerAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(DinnerAmountFilter.Split('~')[1]) : 0;
            }
            var TiffinAmountFrom = 0;
            var TiffinAmountTo = 0;
            if (TiffinAmountFilter.Contains('~'))
            {
                TiffinAmountFrom = TiffinAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(TiffinAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(TiffinAmountFilter.Split('~')[0]) : 0;
                TiffinAmountTo = TiffinAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(TiffinAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(TiffinAmountFilter.Split('~')[1]) : 0;
            }
            var ETiffinAmountFrom = 0;
            var ETiffinAmountTo = 0;
            if (ETiffinAmountFilter.Contains('~'))
            {
                ETiffinAmountFrom = ETiffinAmountFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(ETiffinAmountFilter.Split('~')[0]) == true ? Convert.ToInt32(ETiffinAmountFilter.Split('~')[0]) : 0;
                ETiffinAmountTo = ETiffinAmountFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(ETiffinAmountFilter.Split('~')[1]) == true ? Convert.ToInt32(ETiffinAmountFilter.Split('~')[1]) : 0;
            }
            #endregion From-To

            var OTAlloawanceFilter1 = OTAlloawanceFilter.ToLower() == "Y" ? true.ToString() : false.ToString();
            var OTOrginalFilter1 = OTOrginalFilter.ToLower() == "y" ? true.ToString() : false.ToString();
            var OTBayerFilter1 = OTBayerFilter.ToLower() == "y" ? true.ToString() : false.ToString();
            var ExtraOTFilter1 = ExtraOTFilter.ToLower() == "y" ? true.ToString() : false.ToString();
            var isActiveFilter1 = isActiveFilter.ToLower() == "Active" ? true.ToString() : false.ToString();
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();
            IEnumerable<DesignationVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);
                var isSearchable10 = Convert.ToBoolean(Request["bSearchable_10"]);
                var isSearchable11 = Convert.ToBoolean(Request["bSearchable_11"]);
                var isSearchable12 = Convert.ToBoolean(Request["bSearchable_12"]);
                var isSearchable13 = Convert.ToBoolean(Request["bSearchable_13"]);
                var isSearchable14 = Convert.ToBoolean(Request["bSearchable_14"]);
                var isSearchable15 = Convert.ToBoolean(Request["bSearchable_15"]);
                var isSearchable16 = Convert.ToBoolean(Request["bSearchable_16"]);


                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.AttendenceBonus.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.EPZ.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Other.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.IfterAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.DinnerAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.TiffinAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.ETiffinAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable10 && c.OTAlloawance.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable11 && c.OTOrginal.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable12 && c.OTBayer.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable13 && c.ExtraOT.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable14 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable15 && c.OrderNo.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable16 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || nameFilter != "" || AttendenceBonusFilter != "" || IfterAmountFilter != "" || EPZFilter != "" || ETiffinAmountFilter != "" || OtherFilter != ""
                || DinnerAmountFilter != "" || TiffinAmountFilter != "" || isActiveFilter != "" || remarksFilter != "" || OrderNoFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))                                                                                                             //1 
                    && (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))                                                                                                          //2 
                    && (AttendenceBonusFrom == 0 || AttendenceBonusFrom <= Convert.ToInt32(c.AttendenceBonus)) && (AttendenceBonusTo == 0 || AttendenceBonusTo >= Convert.ToInt32(c.AttendenceBonus)) //3 
                    && (EPZFrom == 0 || EPZFrom <= Convert.ToInt32(c.EPZ)) && (EPZTo == 0 || EPZTo >= Convert.ToInt32(c.EPZ))                                                                         //4 
                    && (ETiffinAmountFrom == 0 || ETiffinAmountFrom <= Convert.ToInt32(c.ETiffinAmount)) && (ETiffinAmountTo == 0 || ETiffinAmountTo >= Convert.ToInt32(c.ETiffinAmount))             //5 
                    && (IfterAmountFrom == 0 || IfterAmountFrom <= Convert.ToInt32(c.IfterAmount)) && (IfterAmountTo == 0 || IfterAmountTo >= Convert.ToInt32(c.IfterAmount))                         //6 
                    && (OtherFrom == 0 || OtherFrom <= Convert.ToInt32(c.Other)) && (OtherTo == 0 || OtherTo >= Convert.ToInt32(c.DinnerAmount))                                                      //7                                                             //7 
                    && (DinnerAmountTo == 0 || DinnerAmountTo >= Convert.ToInt32(c.Other)) && (DinnerAmountFrom == 0 || DinnerAmountFrom <= Convert.ToInt32(c.DinnerAmount))                          //8 
                    && (TiffinAmountFrom == 0 || TiffinAmountFrom <= Convert.ToInt32(c.TiffinAmount)) && (TiffinAmountTo == 0 || TiffinAmountTo >= Convert.ToInt32(c.TiffinAmount))                   //9 
                    && (OTAlloawanceFilter == "" || c.OTAlloawance.ToString().ToLower().Contains(OTAlloawanceFilter1.ToLower()))                                                                      //10
                    && (OTOrginalFilter == "" || c.OTOrginal.ToString().ToLower().Contains(OTOrginalFilter1.ToLower()))                                                                               //11
                    && (OTBayerFilter == "" || c.OTBayer.ToString().ToLower().Contains(OTBayerFilter1.ToLower()))                                                                                     //12
                    && (ExtraOTFilter == "" || c.ExtraOT.ToString().ToLower().Contains(ExtraOTFilter1.ToLower()))                                                                                     //13
                    && (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))                                                                                  //14
                    && (OrderNoFilter == "" || c.OrderNo.ToString().Contains(OrderNoFilter.ToLower()))                                                                                                 //15
                    && (remarksFilter == "" || c.Remarks.ToLower().Contains(remarksFilter.ToLower()))                                                                                                 //15
                    );
            }

            #endregion Column Filtering


            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1 "]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2 "]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3 "]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4 "]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5 "]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6 "]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7 "]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8 "]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9 "]);
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);
            var isSortable_11 = Convert.ToBoolean(Request["bSortable_11"]);
            var isSortable_12 = Convert.ToBoolean(Request["bSortable_12"]);
            var isSortable_13 = Convert.ToBoolean(Request["bSortable_13"]);
            var isSortable_14 = Convert.ToBoolean(Request["bSortable_14"]);
            var isSortable_15 = Convert.ToBoolean(Request["bSortable_15"]);
            var isSortable_16 = Convert.ToBoolean(Request["bSortable_16"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<DesignationVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.AttendenceBonus.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.EPZ.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Other.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.IfterAmount.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.ExtraOT.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.TiffinAmount.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.ETiffinAmount.ToString() :
                sortColumnIndex == 10 && isSortable_11 ? c.OTAlloawance.ToString() :
                sortColumnIndex == 11 && isSortable_11 ? c.OTOrginal.ToString() :
                sortColumnIndex == 12 && isSortable_12 ? c.OTBayer.ToString() :
                sortColumnIndex == 13 && isSortable_13 ? c.ExtraOT.ToString() :
                sortColumnIndex == 14 && isSortable_14 ? c.IsActive.ToString() :
                sortColumnIndex == 15 && isSortable_15 ? c.OrderNo.ToString() :
                sortColumnIndex == 16 && isSortable_16 ? c.Remarks :
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
                , c.Code                                                        //1 
                , c.Name                                                        //2 
                , c.AttendenceBonus.ToString()                                  //3 
                , c.EPZ.ToString()                                              //4 
                , c.Other.ToString()                                            //5 
                , c.IfterAmount.ToString()                                      //6 
                , c.DinnerAmount.ToString()                                     //7 
                , c.TiffinAmount.ToString()                                     //8 
                , c.ETiffinAmount.ToString()                                    //9 
                , Convert.ToString(c.OTAlloawance == true ? "Y" : "No")         //10
                , Convert.ToString(c.OTOrginal == true ? "Y" : "No")            //11
                , Convert.ToString(c.OTBayer == true ? "Y" : "No")              //12
                , Convert.ToString(c.ExtraOT == true ? "Y" : "No")              //13
                , Convert.ToString(c.IsActive == true ? "Active" : "Inactive")  //14
                , c.OrderNo.ToString()                                          //15
                , c.Remarks                                                     //16
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
        /// Displays the Create view for creating a new resource. 
        /// The method checks the user's permission to add a resource (based on their role) and 
        /// redirects to the home page if the user does not have permission.
        /// </summary>
        /// <returns>
        /// Returns the Create view if the user has the required permission; otherwise, redirects to the home page.
        /// </returns>
        /// <remarks>
        /// The permission check is performed using the user's role and a specific permission key ("1_6" for add).
        /// If the permission is not granted, the user is redirected to the home page.
        /// </remarks>
        [HttpGet]
        public ActionResult Create()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_6", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }

        /// <summary>
        /// Creates a new Designation based on the provided ViewModel (vm).
        /// The method sets the creation details, such as the timestamp, user, and branch ID.
        /// It then attempts to insert the Designation into the database. If successful, 
        /// it redirects to the Index action; otherwise, it logs the error and redirects to the Index action.
        /// </summary>
        /// <param name="vm">The DesignationViewModel containing the data to be saved.</param>
        /// <returns>
        /// Redirects to the Index action after attempting to insert the new designation.
        /// In case of an error, it redirects to the Index action with an error message in the session.
        /// </returns>
        /// <remarks>
        /// The method logs the result of the insertion operation in the session, and in case of failure, 
        /// it logs the error details in the file log for further investigation.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(DesignationVM vm)
        {
            string[] result = new string[6];
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
        /// Retrieves the Designation data based on the provided ID and prepares it for editing.
        /// The method checks user permissions before allowing access to the editing page.
        /// If the user does not have permission, they are redirected to the home page.
        /// </summary>
        /// <param name="id">The ID of the Designation to be edited.</param>
        /// <returns>
        /// A view containing the Designation data to be edited. If the user has permission, the method
        /// fetches the data for the specified ID and passes it to the view.
        /// If the user does not have permission, they are redirected to the home page.
        /// </returns>
        /// <remarks>
        /// The method retrieves the Designation data using the repository and passes the data to the view.
        /// It checks the user's permission to ensure they are allowed to edit the Designation.
        /// </remarks>
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_6", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            DesignationVM vm = new DesignationVM();
            vm = _repo.SelectById(id);
            return View(vm);
            //return PartialView(_repo.SelectById(id));
        }

        /// <summary>
        /// Updates the Designation data based on the provided model.
        /// This method processes the update request and logs the changes with timestamps, user details, and the branch ID.
        /// If the update is successful, the user is redirected to the "Index" page. In case of an error, the exception is logged and the user is redirected to the "Index" page with an error message.
        /// </summary>
        /// <param name="vm">The DesignationVM model containing the updated data for the Designation.</param>
        /// <param name="btn">A string representing the button clicked to trigger the update (e.g., "Save" or "Cancel").</param>
        /// <returns>
        /// A redirect to the "Index" page with a result status indicating the success or failure of the update.
        /// If the update is successful, a success message is shown; otherwise, an error message is logged and displayed.
        /// </returns>
        /// <remarks>
        /// The method captures the current timestamp, user name, and workstation IP for logging purposes.
        /// It calls the repository's `Update` method to perform the actual data update.
        /// Any exceptions encountered during the update process are logged with detailed information.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(DesignationVM vm, string btn)
        {

            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
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
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult Delete(string ids)
        {

            var permission = _reposur.SymRoleSession(identity.UserId, "1_6", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            DesignationVM vm = new DesignationVM();
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

        /// <summary>
        /// Exports the designation data to an Excel file and sends it to the client for download.
        /// This method fetches the designation data using the provided ExportImportVM model and then creates an Excel file from it.
        /// The file is returned as a download to the client with the appropriate content type and file name.
        /// If an error occurs during the export process, the error message is captured and displayed on the Index page.
        /// </summary>
        /// <param name="VM">The ExportImportVM model that contains the criteria for fetching the designation data.</param>
        /// <returns>
        /// A redirect to the "Index" page after the export process is completed, or an error message in case of failure.
        /// </returns>
        /// <remarks>
        /// The method uses the `ExportImportRepo.SelectDesignationInfo` method to fetch the data and uses the `EPPlus` library to generate an Excel file.
        /// The generated Excel file contains a sheet with the designation data, which is saved to a memory stream and sent to the client's browser as a downloadable file.
        /// If an error occurs, the exception message is logged and displayed on the "Index" page.
        /// </remarks>
        public ActionResult ExportExcell(ExportImportVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            DataTable dt = new DataTable();
            ExcelPackage excel = new ExcelPackage();

            try
            {

                ExportImportRepo _repo = new ExportImportRepo();

                dt = _repo.SelectDesignationInfo(VM);

                #region Excel

                string filename = "DesignationInfo Data";
                var workSheet = excel.Workbook.Worksheets.Add("DesignationInfo Data");
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
    }
}
