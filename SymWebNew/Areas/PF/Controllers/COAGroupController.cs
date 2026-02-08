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
    public class COAGroupController : Controller
    {
        public COAGroupController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /PF/COAs/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        COAGroupRepo _repo = new COAGroupRepo();
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "70001", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View("~/Areas/PF/Views/COAGroup/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Chart of Accounts Group information.
        /// </summary>      
        /// <returns>View containing Chart of Accounts Group Name</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
          
            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0, new[] { "COAGroups.TransType" }, new[] { AreaTypePFVM.TransType });

            IEnumerable<COAGroupVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);                
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())                    
                    || isSearchable2 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.GroupSL.ToString().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<COAGroupVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Name :             
                sortColumnIndex == 2 && isSortable_2 ? c.IsActive.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Remarks :
                sortColumnIndex == 4 && isSortable_4 ? c.GroupSL.ToString() :
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
                , c.IsActive? "Active":"Inactive"
                , c.Remarks            
                , c.GroupSL.ToString()            
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
            COAGroupVM vm = new COAGroupVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/PF/Views/COAGroup/Create.cshtml", vm);
        }

        /// <summary>
        /// Handles creation or update of a COA Group record based on the operation type.
        /// </summary>
        /// <param name="vm">The COAGroupVM view model containing form data and operation type ("add" or "update").</param>
        /// <returns>
        /// Redirects to the Index view after the operation. 
        /// If an error occurs or the operation type is invalid, redirects to the Index with a failure message.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(COAGroupVM vm)
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
        /// Retrieves a COA Group record for editing based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the COA Group to be edited.</param>
        /// <returns>
        /// Returns the Create view (used for both add/edit) populated with the existing COA Group data
        /// and sets the operation mode to "update".
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70001", "edit").ToString();
            COAGroupVM vm = new COAGroupVM();
            vm.TransType = AreaTypePFVM.TransType;

            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("~/Areas/PF/Views/COAGroup/Create.cshtml", vm);
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
            COAGroupVM vm = new COAGroupVM();
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
