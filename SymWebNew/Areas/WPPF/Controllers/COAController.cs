using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.WPPF;
using SymRepository.Common;
using SymViewModel.WPPF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymWebUI.Areas.WPPF.Models;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class COAController : Controller
    {
        public COAController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/COAs/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        COARepo _repo = new COARepo();
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "70001", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            return View("~/Areas/WPPF/Views/COA/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Chart of Accounts information.
        /// </summary>      
        /// <returns>View containing Chart of Accounts Name</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {           
            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0, new[] { "COAs.TransType" }, new[] { AreaTypePFVM.TransType });

            IEnumerable<COAVM> filteredData;
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
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.GroupName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.COAType.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Nature.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.IsRetainedEarning.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.IsNetProfit.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.IsDepreciation.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.COASL.ToString().Contains(param.sSearch.ToLower())
                    || isSearchable10 && c.IsActive.ToString().Contains(param.sSearch.ToLower())
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
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var isSortable_10 = Convert.ToBoolean(Request["bSortable_10"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<COAVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.GroupName :
                sortColumnIndex == 4 && isSortable_4 ? c.COAType.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Nature :
                sortColumnIndex == 6 && isSortable_6 ? c.IsRetainedEarning.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.IsNetProfit.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.IsDepreciation.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.COASL.ToString() :
                sortColumnIndex == 10 && isSortable_10 ? c.IsActive.ToString() :
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
                , c.Name               
                , c.GroupName.ToString()               
                , c.COAType.ToString()               
                , c.Nature.ToString()               
                , c.IsRetainedEarning.ToString()               
                , c.IsNetProfit.ToString()               
                , c.IsDepreciation.ToString()               
                , c.COASL.ToString()                   
                , c.IsActive? "Active":"Inactive"
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
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70001", "add").ToString();
            COAVM vm = new COAVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            //return PartialView(vm);
            return View("~/Areas/WPPF/Views/COA/Create.cshtml", vm);
        }

        /// <summary>
        /// Handles creation or update of a Chart of Accounts (COA) record.
        /// </summary>
        /// <param name="vm">The COA view model containing form data and operation type.</param>
        /// <returns>
        /// Redirects to the Index view after successful operation. 
        /// If an exception occurs, logs the error and also redirects to Index.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(COAVM vm)
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

                    //vm.BranchId = Convert.ToInt32(identity.BranchId);
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;

                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Loads the COA record for editing based on the given ID.
        /// </summary>
        /// <param name="id">The unique identifier of the COA record to edit.</param>
        /// <returns>
        /// Returns the Create view pre-populated with the selected COA data in update mode.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70001", "edit").ToString();
            COAVM vm = new COAVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("~/Areas/WPPF/Views/COA/Create.cshtml", vm);
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
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70001", "delete").ToString();
            COAVM vm = new COAVM();
            vm.TransType = AreaTypePFVM.TransType;
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
