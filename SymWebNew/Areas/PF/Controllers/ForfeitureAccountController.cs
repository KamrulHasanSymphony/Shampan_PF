using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.PF;
using SymViewModel.PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.PF.Controllers
{
    public class ForfeitureAccountController : Controller
    {
        //
        // GET: /PF/ForfeitureAccount/

        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ForfeitureAccountRepo _repo = new ForfeitureAccountRepo();

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();
            IEnumerable<ForfeitureAccountVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.TotalForfeitValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<ForfeitureAccountVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.TotalForfeitValue.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
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
                , c.EmpName
                , c.Designation
                , c.Department
                , c.TotalForfeitValue.ToString()
                , c.Post ? "Posted" : "Not Posted"
     
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
        public ActionResult IndexResignEmployee()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }
        public ActionResult _indexResignEmployee(JQueryDataTableParamModel param)
        {
            //#region Search and Filter Data

            //ForfeitureAccountRepo _repoForfeitureAccount = new ForfeitureAccountRepo();
            //var getAllData = _repoForfeitureAccount.SelectAll_ResignEmployee();
            //IEnumerable<ForfeitureAccountVM> filteredData;
            //if (!string.IsNullOrEmpty(param.sSearch))
            //{
            //    var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
            //    var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
            //    var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
            //    var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
            //    var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
            //    filteredData = getAllData.Where(c =>
            //           isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
            //        || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
            //        || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
            //        || isSearchable4 && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
            //        || isSearchable5 && c.EmpResignDate.ToString().ToLower().Contains(param.sSearch.ToLower())
            //        );
            //}
            //else
            //{
            //    filteredData = getAllData;
            //}
            //#endregion Search and Filter Data
            //var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            //var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            //var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            //var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            //var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            //Func<ForfeitureAccountVM, string> orderingFunction = (c =>
            //    sortColumnIndex == 1 && isSortable_1 ? c.Code :
            //    sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
            //    sortColumnIndex == 3 && isSortable_3 ? c.Designation :
            //    sortColumnIndex == 4 && isSortable_4 ? c.Department :
            //    sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.EmpResignDate) :
            //    "");
            //var sortDirection = Request["sSortDir_0"]; // asc or desc
            //if (sortDirection == "asc")
            //    filteredData = filteredData.OrderBy(orderingFunction);
            //else
            //    filteredData = filteredData.OrderByDescending(orderingFunction);
            //var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            //var result = from c in displayedCompanies
            //             select new[] { 
            //    Convert.ToString(c.EmployeeId)
            //    , c.Code
            //    , c.EmpName
            //    , c.Designation
            //    , c.Department
            //    , c.EmpResignDate
     
            //};
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
        public ActionResult Create(ForfeitureAccountVM vm)
        {
            try
            {
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();
                vm = _repo.PreInsert(vm);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                vm.Operation = "add";
            }
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(ForfeitureAccountVM vm)
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "edit").ToString();
            ForfeitureAccountVM vm = new ForfeitureAccountVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            vm.Operation = "update";
            return View("Create", vm);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            ForfeitureAccountVM vm = new ForfeitureAccountVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult Post(string ids)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            result = _repo.Post(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
