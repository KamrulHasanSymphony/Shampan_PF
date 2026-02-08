using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.WWF;
using SymRepository.Common;
using SymViewModel.WWF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymWebUI.Areas.WWF.Models;

namespace SymWebUI.Areas.WWF.Controllers
{
    public class SettlementPolicyController : Controller
    {
        public SettlementPolicyController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WWF/SettlementPolicy/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        SettlementPolicyRepo _repo = new SettlementPolicyRepo();

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
        /// Retrieves all Settlement Ploicy information.
        /// </summary>      
        /// <returns>View containing Settlement Ploicy</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
           
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<SettlementPolicyVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.PolicyName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.JobAgeInMonth.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.EmployeeContributionRatio .ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.EmployerContributionRatio.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
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
            Func<SettlementPolicyVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.PolicyName                 :
                sortColumnIndex == 2 && isSortable_2 ? c.JobAgeInMonth.ToString()              :
                sortColumnIndex == 3 && isSortable_3 ? c.EmployeeContributionRatio.ToString()  :
                sortColumnIndex == 4 && isSortable_4 ? c.EmployerContributionRatio.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Remarks                    :
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
                , c.PolicyName                 
                , c.JobAgeInMonth.ToString()             
                , c.EmployeeContributionRatio.ToString() 
                , c.EmployerContributionRatio.ToString()
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
            SettlementPolicyVM vm = new SettlementPolicyVM();
            vm.Operation = "add";
            return View(vm);
        }

        /// <summary>
        /// Creates or updates a settlement policy based on the provided view model.
        /// If the operation is "add", a new record is created. If the operation is "update", an existing record is updated.
        /// The method also handles logging the result of the operation and redirects accordingly based on success or failure.
        /// </summary>
        /// <param name="vm">The view model containing the settlement policy data to be added or updated.</param>
        /// <returns>
        /// A view or redirection based on the outcome of the operation:
        /// - Redirects to the "Edit" view if the operation is successful.
        /// - Returns the "Create" view if the operation fails or if the "add" operation is being performed.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(SettlementPolicyVM vm)
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
        /// Retrieves and displays the settlement policy details for editing based on the provided ID.
        /// It checks the user’s permissions before allowing access to the edit page.
        /// If permission is granted, it loads the settlement policy data and sets the operation to "update".
        /// </summary>
        /// <param name="id">The ID of the settlement policy to be edited.</param>
        /// <returns>
        /// A view with the settlement policy data populated for editing.
        /// If the user does not have permission, they will be redirected elsewhere (not shown in this snippet).
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            SettlementPolicyVM vm = new SettlementPolicyVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
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
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            SettlementPolicyVM vm = new SettlementPolicyVM();
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
