using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.BWWF;
using SymViewModel.Common;
using SymViewModel.BWWF;
using SymWebUI.Areas.BWWF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.BWWF.Controllers
{
    public class BankBranchController : Controller
    {
        public BankBranchController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /BWWF/BankBranch/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        BankBranchRepo _repo = new BankBranchRepo();
       
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            return View("~/Areas/BWWF/Views/BankBranch/Index.cshtml");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Bank Name Branch information.
        /// </summary>      
        /// <returns>View containing Bank Branch Name</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
           
            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0, new string[] { "bb.TransType", "b.BranchId" }, new string[] { AreaTypePFVM.TransType, Session["BranchId"].ToString() });
            IEnumerable<BankBranchVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.BankName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.BranchName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.BranchAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.BankAccountType.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.BankAccountNo.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<BankBranchVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.BankName       :
                sortColumnIndex == 2 && isSortable_2 ? c.BranchName     :
                sortColumnIndex == 3 && isSortable_3 ? c.BranchAddress  :
                sortColumnIndex == 4 && isSortable_4 ? c.BankAccountType:
                sortColumnIndex == 5 && isSortable_5 ? c.BankAccountNo  :
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
                , c.BankName      
                , c.BranchName
                , c.BranchAddress 
                , c.BankAccountType
                , c.BankAccountNo  
     
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
            BankBranchVM vm = new BankBranchVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/BWWF/Views/BankBranch/Create.cshtml", vm);
        }

        /// <summary>
        /// Handles both creation and update operations for a bank branch record.
        /// </summary>
        /// <param name="vm">The <see cref="BankBranchVM"/> containing the data to be added or updated.</param>
        /// <returns>
        /// Redirects to the Edit view upon successful save; otherwise, returns to the Create view with the current model state.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(BankBranchVM vm)
        {
            string[] result = new string[6];
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;
                    vm.BranchId = Session["BranchId"].ToString();
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Edit", new { id = result[2] });
                    }
                    else
                    {
                        return View("~/Areas/BWWF/Views/BankBranch/Create.cshtml", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;

                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Edit", new { id = result[2] });
                }
                else
                {
                    return View("~/Areas/BWWF/Views/BankBranch/Create.cshtml", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("~/Areas/BWWF/Views/BankBranch/Create.cshtml", vm);
            }
        }

        /// <summary>
        /// Loads a bank branch record for editing based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the bank branch to be edited.</param>
        /// <returns>
        /// Returns the Create view pre-populated with the selected <see cref="BankBranchVM"/> data set to update mode.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            BankBranchVM vm = new BankBranchVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("~/Areas/BWWF/Views/BankBranch/Create.cshtml", vm);
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
            BankBranchVM vm = new BankBranchVM();
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
