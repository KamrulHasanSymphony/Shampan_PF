using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.BWWF;
using SymRepository.Common;
using SymViewModel.BWWF;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymWebUI.Areas.BWWF.Models;

namespace SymWebUI.Areas.BWWF.Controllers
{
    public class COASubGroupController : Controller
    {
        public COASubGroupController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /BWWF/COASubGroup/

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        COASubGroupRepo _repo = new COASubGroupRepo();
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "70001", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            return View("~/Areas/BWWF/Views/COASubGroup/Index.cshtml");
        }

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
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())                    
                    || isSearchable2 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.GroupName.ToString().Contains(param.sSearch.ToLower())
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
                sortColumnIndex == 4 && isSortable_4 ? c.GroupName.ToString() :
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
                ,c.GroupName
                , c.Name                                          
                , c.IsActive? "Active":"Inactive"
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
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70001", "add").ToString();
            COAGroupVM vm = new COAGroupVM();
            vm.TransType = AreaTypePFVM.TransType;
            vm.Operation = "add";
            return View("~/Areas/BWWF/Views/COASubGroup/Create.cshtml", vm);
        }



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
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMdd");
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
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMdd");
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


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "70001", "edit").ToString();
            COAGroupVM vm = new COAGroupVM();
            vm.TransType = AreaTypePFVM.TransType;

            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("~/Areas/BWWF/Views/COASubGroup/Create.cshtml", vm);
        }

    }
}
