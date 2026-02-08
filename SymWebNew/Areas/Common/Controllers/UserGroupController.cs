using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
namespace SymWebUI.Areas.Common.Controllers
{
    public class UserGroupController : Controller
    {
        //
        // GET: /Common/UserGroup/
        UserGroupRepo _repo = new UserGroupRepo();
			SymUserRoleRepo _reposur = new SymUserRoleRepo();
			ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //#region Actions
        public ActionResult Index()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_70", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Common/Home");
               }
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all User Group information.
        /// </summary>      
        /// <returns>View containing User Group</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var GroupNameFilter = Convert.ToString(Request["sSearch_1"]);
            var isActiveFilter = Convert.ToString(Request["sSearch_2"]);
            var remarksFilter = Convert.ToString(Request["sSearch_3"]);
            var isActiveFilter1 = isActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<UserGroupVM> filteredData;
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
                       isSearchable1 && c.GroupName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.IsAdmin.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.IsHRM.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.IsAttendance.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.IsPayroll.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.IsTAX.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.IsPF.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.IsGF.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable9 && c.IsESS.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable10 && c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (GroupNameFilter != "" || isActiveFilter != "" || remarksFilter != "")
            {
                filteredData = filteredData.Where(c => 
                       (GroupNameFilter == "" || c.GroupName.ToLower().Contains(GroupNameFilter.ToLower()))
                    && (isActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(isActiveFilter1.ToLower()))
                    && (remarksFilter == "" || c.Remarks.ToLower().Contains(remarksFilter.ToLower()))
                    );
            }
            #endregion Column Filtering
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
            Func<UserGroupVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.GroupName :
                sortColumnIndex == 2 && isSortable_2 ? c.IsAdmin.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.IsHRM.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.IsAttendance.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.IsPayroll.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.IsTAX.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.IsPF.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.IsGF.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.IsESS.ToString() :
                sortColumnIndex == 10 && isSortable_10 ? c.IsActive.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                Convert.ToString(c.Id)
                , c.GroupName
                , Convert.ToString(c.IsAdmin == true ? "Yes" : "No") 
                , Convert.ToString(c.IsESS == true ? "Yes" : "No") 
                , Convert.ToString(c.IsHRM == true ? "Yes" : "No") 
                , Convert.ToString(c.IsAttendance == true ? "Yes" : "No") 
                , Convert.ToString(c.IsPayroll == true ? "Yes" : "No") 
                , Convert.ToString(c.IsTAX == true ? "Yes" : "No") 
                , Convert.ToString(c.IsPF == true ? "Yes" : "No") 
                , Convert.ToString(c.IsGF == true ? "Yes" : "No") 
                , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
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
        /// Checks user permission for creating a record, and redirects if the user does not have the necessary permissions.
        /// </summary>
        /// <returns>
        /// A <see cref="PartialViewResult"/> if the user has permission to create, otherwise a redirect to the home page.
        /// </returns>
        /// <remarks>
        /// This method checks if the user has permission to perform the "add" action for a specific role
        /// using the <see cref="SymRoleSession"/> method. If the permission is granted, it returns a partial view.
        /// If the user does not have permission, they are redirected to the home page.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_70", "add").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Common/Home");
               }
            return PartialView();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(UserGroupVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
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

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_70", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Common/Home");
               }
            UserGroupVM vm = new UserGroupVM();
            vm = _repo.SelectById(id);
            return PartialView(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(UserGroupVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
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

         [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Delete(string ids)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var permission = _reposur.SymRoleSession(identity.UserId, "1_70", "index").ToString();
            Session["permission"] = permission;
            UserGroupVM vm = new UserGroupVM();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

         public ActionResult SelectUserForRoll(string Id)
         {
             ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
             var permission = _reposur.SymRoleSession(identity.UserId, "1_70", "edit").ToString();
             Session["permission"] = permission;
             if (permission == "False")
             {
                 return Redirect("/Common/Home");
             }
             SymUserRollVM vm = new SymUserRollVM();
             SymUserRoleRepo _Repo = new SymUserRoleRepo();
             vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
             vm.CreatedBy = identity.Name;
             vm.CreatedFrom = identity.WorkStationIP;
             vm.BranchId = Convert.ToInt32(identity.BranchId);
             string[] result = new string[6];
             vm.GroupId = Convert.ToInt32(Id);
             result = _Repo.SelectAllSymRollwithInsert(vm);
             UserGroupVM uservm = new UserGroupVM();
             uservm = _repo.SelectById(Id);
             uservm.SymUserRollVMs = _Repo.SelectAllSymUserRollByGroupId(Id);
             //vm.UserlogVM = _Repo.SelectUserById(Id);
             //vm.SymUserDefaultRollVMs = _Repo.SelectSymUserById(Id);
             //return View(new SymUserRollRepo().SelectAll());
             return View("UserRoll", uservm);
         }

         public ActionResult SelectUserForRollSearch(string Id, string SymArea)
         {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_70", "edit").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Common/Home");
               }
             SymUserRoleRepo _Repo = new SymUserRoleRepo();
             UserGroupVM uservm = new UserGroupVM();
             uservm.SymUserRollVMs = _Repo.SelectAllSymUserRollByGroupId(Id, SymArea);
             return View("SymUserRoll", uservm);
         }

         [HttpPost]
         public ActionResult EditRoll(UserGroupVM vm)
         {
             string[] result = new string[6];
             ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
             vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
             vm.LastUpdateBy = identity.Name;
             vm.LastUpdateFrom = identity.WorkStationIP;
             try
             {
                 result = new SymUserRoleRepo().Update(vm);
                 Session["result"] = result[0] + "~" + result[1];
                 return RedirectToAction("Index");
             }
             catch (Exception)
             {
                 Session["result"] = "Fail~Data Not Succeessfully!";
                 FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                 return RedirectToAction("Index");
             }
         }
    }
}
