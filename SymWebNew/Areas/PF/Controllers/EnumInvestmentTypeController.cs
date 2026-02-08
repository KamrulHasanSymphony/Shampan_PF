using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.PF;
using SymRepository.Common;
using SymViewModel.PF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymWebUI.Areas.PF.Models;

namespace SymWebUI.Areas.PF.Controllers
{
    public class EnumInvestmentTypeController : Controller
    {
        public EnumInvestmentTypeController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/EnumInvestmentType/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EnumInvestmentTypeRepo _repo = new EnumInvestmentTypeRepo();
        [Authorize(Roles = "Admin")]

        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/EnumInvestmentType/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Process Control information.
        /// </summary>      
        /// <returns>View containing Process Control</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {       
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<EnumInvestmentTypeVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Remarks.ToString().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EnumInvestmentTypeVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Name :
                sortColumnIndex == 2 && isSortable_2 ? c.IsActive.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
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
                , c.Name    
                , c.IsActive ? "Active":"Inactive" 
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
            EnumInvestmentTypeVM vm = new EnumInvestmentTypeVM();
            vm.Operation = "add";
            return PartialView("~/Areas/PF/Views/EnumInvestmentType/Create.cshtml", vm);
        }

        /// <summary>
        /// Handles the creation or update of an investment type based on the operation specified in the view model.
        /// Sets audit information such as creation or update time, user, and workstation IP.
        /// Displays a success or failure message and redirects to the Index view.
        /// </summary>
        /// <param name="vm">The view model containing investment type data and the operation type ("add" or "update").</param>
        /// <returns>
        /// Redirects to the Index view after performing the operation. 
        /// On exception, logs the error and still redirects to the Index view.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(EnumInvestmentTypeVM vm)
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
                    return RedirectToAction("Index");
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
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
        /// Retrieves the details of an investment type for editing based on the provided ID.
        /// Sets the operation type to "update" and returns the corresponding partial view with the populated model.
        /// Also checks user permissions to ensure the user has the right to edit.
        /// </summary>
        /// <param name="id">The ID of the investment type to be edited.</param>
        /// <returns>
        /// A partial view populated with the investment type data for editing.
        /// If the user does not have permission, the appropriate redirection should occur before this.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            EnumInvestmentTypeVM vm = new EnumInvestmentTypeVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return PartialView("~/Areas/PF/Views/EnumInvestmentType/Create.cshtml" , vm);
        }

        /// <summary>
        /// Deletes the investment types with the given IDs after validating user permissions for the "delete" operation.
        /// Updates the `LastUpdateAt`, `LastUpdateBy`, and `LastUpdateFrom` properties before calling the delete operation.
        /// </summary>
        /// <param name="ids">A string containing the IDs of the investment types to be deleted, separated by '~'.</param>
        /// <returns>
        /// A JSON response indicating the result of the delete operation, typically a message (success or failure).
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            EnumInvestmentTypeVM vm = new EnumInvestmentTypeVM();
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
