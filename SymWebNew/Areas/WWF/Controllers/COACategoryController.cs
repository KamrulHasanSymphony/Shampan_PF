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
    public class COACategoryController : Controller
    {
        public COACategoryController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        COACategoryRepo _repo = new COACategoryRepo();

        //
        // GET: /WWF/COACategory/
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "70002", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WWF/Home");
            }
            return View("~/Areas/WWF/Views/COACategory/Index.cshtml");
        }

        // DataTables ajax
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Search and Filter Data
            var getAllData = _repo.SelectAll(0);

            IEnumerable<COAGroupVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]); 
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]); 
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]); 
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]); 
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]); 

                string s = param.sSearch.ToLower();
                filteredData = getAllData.Where(c =>
                       (isSearchable1 && (c.GroupName ?? "").ToLower().Contains(s))
                    || (isSearchable2 && (c.SubGroupName ?? "").ToLower().Contains(s))
                    || (isSearchable3 && (c.Name ?? "").ToLower().Contains(s))
                    || (isSearchable4 && (c.IsActive.ToString().ToLower().Contains(s)))
                    || (isSearchable5 && (c.Remarks ?? "").ToLower().Contains(s))
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion

            // Sorting
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<COAGroupVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.GroupName :
                sortColumnIndex == 2 && isSortable_2 ? c.SubGroupName :
                sortColumnIndex == 3 && isSortable_3 ? c.Name :
                sortColumnIndex == 4 && isSortable_4 ? c.IsActive.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Remarks :
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayed = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayed
                         select new[] {
                             Convert.ToString(c.Id),
                             c.GroupName,
                             c.SubGroupName,
                             c.Name,
                             c.IsActive ? "Active" : "Inactive",
                             c.Remarks
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
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "add").ToString();
            COAGroupVM vm = new COAGroupVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";

            return View("~/Areas/WWF/Views/COACategory/Create.cshtml", vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(COAGroupVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identityLocal = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
                    vm.CreatedBy = identityLocal.Name;
                    vm.CreatedFrom = identityLocal.WorkStationIP;
                    vm.TransType = AreaTypePFVM.TransType;

                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index");
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
                    vm.LastUpdateBy = identityLocal.Name;
                    vm.LastUpdateFrom = identityLocal.WorkStationIP;
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
            catch (Exception ex)
            {
                Session["result"] = "Fail~Data Not Successfully!";
                // Log more info if needed
                FileLogger.Log((result != null ? result[0].ToString() : "") + Environment.NewLine + (result != null ? result[2].ToString() : ""), this.GetType().Name, (result != null ? result[4].ToString() : "") + Environment.NewLine + ex.ToString());
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70002", "edit").ToString();
            COAGroupVM vm = new COAGroupVM();
            vm.TransType = AreaTypePFVM.TransType;

            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            if (vm == null)
            {
                Session["result"] = "Fail~Record not found!";
                return RedirectToAction("Index");
            }
            vm.Operation = "update";

            return View("~/Areas/WWF/Views/COACategory/Create.cshtml", vm);
        }
    }
}
