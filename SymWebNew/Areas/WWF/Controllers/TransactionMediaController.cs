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
    public class TransactionMediaController : Controller
    {

        public TransactionMediaController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        // GET: /WWF/TransactionMedia/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        TransactionMediaRepo _repo = new TransactionMediaRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WWF/Home");
            }
            return View("~/Areas/WWF/Views/TransactionMedia/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Transcation Media information.
        /// </summary>      
        /// <returns>View containing Transcation Media</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
          
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<TransactionMediaVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.Remarks.ToString().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TransactionMediaVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Name :
                sortColumnIndex == 2 && isSortable_2 ? c.Remarks :
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
            TransactionMediaVM vm = new TransactionMediaVM();
            vm.Operation = "add";
            return PartialView("~/Areas/WWF/Views/TransactionMedia/Create.cshtml", vm);
        }

        /// <summary>
        /// Handles the creation or update of a TransactionMedia based on the operation type specified in the view model.
        /// Updates the respective fields (`CreatedAt`, `CreatedBy`, `CreatedFrom` for adding; `LastUpdateAt`, `LastUpdateBy`, `LastUpdateFrom` for updating) 
        /// before calling the insert or update method of the repository.
        /// </summary>
        /// <param name="vm">The view model containing the data for the transaction media to be created or updated.</param>
        /// <returns>
        /// A redirect to the Index action upon successful creation or update, or failure handling if an exception occurs.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(TransactionMediaVM vm)
        {
            string[] result = new string[6];
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
        /// Retrieves the details of a specific transaction media based on the provided ID, sets the operation type to "update",
        /// and returns a partial view for editing the transaction media.
        /// </summary>
        /// <param name="id">The ID of the transaction media to be edited.</param>
        /// <returns>
        /// A partial view containing the transaction media details for editing.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            TransactionMediaVM vm = new TransactionMediaVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return PartialView("~/Areas/WWF/Views/TransactionMedia/Create.cshtml", vm);
        }

        /// <summary>
        /// Deletes one or more transaction media entries based on the provided IDs.
        /// The method checks for permission to delete, updates the record with the current timestamp, 
        /// and returns the result of the deletion operation as a JSON response.
        /// </summary>
        /// <param name="ids">A string containing the IDs of the transaction media entries to be deleted, separated by '~'.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the deletion operation.
        /// </returns>
        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            TransactionMediaVM vm = new TransactionMediaVM();
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
