using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository;
using SymRepository.Common;
using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class SymUserRollController : Controller
    {
        #region Decalre
        SymUserRoleRepo _Repo = new SymUserRoleRepo();
        UserInformationRepo _infoRepo = new UserInformationRepo();

        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        #endregion Decalre
        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_14", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var ModuleFilter = Convert.ToString(Request["sSearch_2"]);
            var MenuFilter = Convert.ToString(Request["sSearch_2"]);
            var IsIndexFilter = Convert.ToString(Request["sSearch_2"]);
            var IsAddFilter = Convert.ToString(Request["sSearch_2"]);
            var IsEditFilter = Convert.ToString(Request["sSearch_2"]);
            var IsDeleteFilter = Convert.ToString(Request["sSearch_2"]);
            var IsReportFilter = Convert.ToString(Request["sSearch_2"]);
            var IsProcessFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion Column Search
            #region Search and Filter Data
            var getAllData = _Repo.SelectSymUserRollAll();
            IEnumerable<SymUserRollVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);

                filteredData = getAllData
                   .Where(c => isSearchable2 && c.symArea.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.symController.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IsIndex.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IsAdd.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IsEdit.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IsDelete.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IsReport.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.IsProcess.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (ModuleFilter != "" || MenuFilter != "" || IsIndexFilter != "" || IsAddFilter != "" || IsEditFilter != "" || IsDeleteFilter != "" || IsReportFilter != "" || IsProcessFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (ModuleFilter == "" || c.symArea.ToString().ToLower().Contains(ModuleFilter.ToLower()))
                                            && (MenuFilter == "" || c.symController.ToString().ToLower().Contains(MenuFilter.ToLower()))
                                            && (IsIndexFilter == "" || c.IsIndex.ToString().ToLower().Contains(IsIndexFilter.ToLower()))
                                            && (IsAddFilter == "" || c.IsAdd.ToString().ToLower().Contains(IsAddFilter.ToLower()))
                                            && (IsEditFilter == "" || c.IsEdit.ToString().ToLower().Contains(IsEditFilter.ToLower()))
                                            && (IsDeleteFilter == "" || c.IsDelete.ToString().ToLower().Contains(IsDeleteFilter.ToLower()))
                                            && (IsReportFilter == "" || c.IsReport.ToString().ToLower().Contains(IsReportFilter.ToLower()))
                                            && (IsProcessFilter == "" || c.IsProcess.ToString().ToLower().Contains(IsProcessFilter.ToLower()))
                                        );
            }

            #endregion Column Filtering
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<SymUserRollVM, string> orderingFunction = (c => sortColumnIndex == 2 && isSortable_2 ? c.GroupId.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.symArea.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.symController.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsIndex.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsAdd.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsEdit.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsDelete.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsReport.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.IsProcess.ToString() :
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
                , c.symArea
                , c.symController
                ,Convert.ToString(c.IsIndex == true ? "Permited" : "Not Permited")
                ,Convert.ToString(c.IsAdd == true ? "Permited" : "Not Permited")
                ,Convert.ToString(c.IsEdit == true ? "Permited" : "Not Permited")
                ,Convert.ToString(c.IsDelete == true ? "Permited" : "Not Permited")
                ,Convert.ToString(c.IsReport == true ? "Permited" : "Not Permited")
                ,Convert.ToString(c.IsProcess == true ? "Permited" : "Not Permited")
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

        //public ActionResult _indexUserlist(JQueryDataTableParamVM param)
        //{
        //    #region Column Search
        //    var idFilter = Convert.ToString(Request["sSearch_0"]);
        //    var codeFilter = Convert.ToString(Request["sSearch_1"]);
        //    var empNameFilter = Convert.ToString(Request["sSearch_2"]);
        //    var departmentFilter = Convert.ToString(Request["sSearch_3"]);
        //    var designationFilter = Convert.ToString(Request["sSearch_4"]);
        //    var joinDateFilter = Convert.ToString(Request["sSearch_5"]);

        //    DateTime fromDate = DateTime.MinValue;
        //    DateTime toDate = DateTime.MaxValue;
        //    if (joinDateFilter.Contains('~'))
        //    {
        //        //Split date range filters with ~
        //        fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
        //        toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
        //    }


        //    var fromID = 0;
        //    var toID = 0;
        //    if (idFilter.Contains('~'))
        //    {
        //        //Split number range filters with ~
        //        fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
        //        toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
        //    }
        //    #endregion Column Search
        //    EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
        //    var getAllData = _empRepo.SelectAllActiveEmp();
        //    IEnumerable<EmployeeInfoVM> filteredData;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
        //        var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
        //        var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
        //        var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
        //        var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
        //        filteredData = getAllData.Where(c =>isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
        //            || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower()));
        //    }
        //    else
        //    {
        //        filteredData = getAllData;
        //    }
        //    #region Column Filtering
        //    if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
        //    {
        //        filteredData = filteredData
        //                        .Where(c =>
        //                            (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
        //                            &&(empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
        //                            &&(departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
        //                            &&(designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
        //                            &&(fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
        //                            &&(toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
        //                        );
        //    }

        //    #endregion Column Filtering

        //    var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
        //    var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
        //    var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    Func<EmployeeInfoVM, string> orderingFunction = (c =>
        //        sortColumnIndex == 1 && isSortable_1 ? c.Code :
        //        sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
        //        sortColumnIndex == 3 && isSortable_3 ? c.Department :
        //        sortColumnIndex == 4 && isSortable_4 ? c.Designation :
        //        sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
        //        "");
        //    var sortDirection = Request["sSortDir_0"]; // asc or desc
        //    if (sortDirection == "asc")
        //        filteredData = filteredData.OrderBy(orderingFunction);
        //    else
        //        filteredData = filteredData.OrderByDescending(orderingFunction);
        //    var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    var result = from c in displayedCompanies
        //                 select new[] { 
        //        Convert.ToString(c.Id)
        //        , c.Code
        //        , c.EmpName 
        //        , c.Department 
        //        , c.Designation
        //        , c.JoinDate
        //    };
        //    return Json(new
        //    {   sEcho = param.sEcho,
        //        iTotalRecords = getAllData.Count(),
        //        iTotalDisplayRecords = filteredData.Count(),
        //        aaData = result
        //    },JsonRequestBehavior.AllowGet);
        //}
        public ActionResult _indexGrouplist(JQueryDataTableParamVM param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var GroupNameFilter = Convert.ToString(Request["sSearch_1"]);
            var IsSuperFilter = Convert.ToString(Request["sSearch_2"]);
            #endregion Column Search

            UserGroupRepo _userGroupRepo = new UserGroupRepo();
            var getAllData = _userGroupRepo.SelectAll();
            IEnumerable<UserGroupVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                filteredData = getAllData.Where(c => isSearchable1 && c.GroupName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.IsSuper.ToString().ToLower().Contains(param.sSearch.ToLower())
                  );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (GroupNameFilter != "" || IsSuperFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                    (GroupNameFilter == "" || c.GroupName.ToLower().Contains(GroupNameFilter.ToLower()))
                                    && (IsSuperFilter == "" || c.IsSuper.ToString().ToLower().Contains(IsSuperFilter.ToLower()))
                                );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<UserGroupVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.GroupName :
                sortColumnIndex == 2 && isSortable_2 ? c.IsSuper.ToString() :
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
                , c.GroupName
                , c.IsSuper? "Super":"Not Super" 
            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_14", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(SymUserRollVM vm)
        {
            //SymUserDefaultRollVM vm = new SymUserDefaultRollVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            try
            {
                string[] result = new string[6];
                result = new SymUserRoleRepo().Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_14", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            SymUserRollVM vm = new SymUserRollVM();
            vm = _Repo.SelectById(id);
            return View(vm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(UserGroupVM vm)
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
        public ActionResult UserRollDetail(string empId, string Module)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_14", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            SymUserRollVM vm = new SymUserRollVM();
            vm.SymUserDefaultRollVMs = _Repo.UserRollDetail(empId, Module);
            return PartialView("_symUserRoll", vm.SymUserDefaultRollVMs);
        }
        public ActionResult SelectUserForRoll1(string Id)
        {
            SymUserRollVM vm = new SymUserRollVM();
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            string[] result = new string[6];
            vm.GroupId = Convert.ToInt32(Id);
            result = _Repo.SelectAllSymRollwithInsert(vm);
            UserGroupVM uservm = new UserGroupVM();
            uservm.SymUserRollVMs = _Repo.SelectAllSymUserRollByGroupId(Id);
            //vm.UserlogVM = _Repo.SelectUserById(Id);
            //vm.SymUserDefaultRollVMs = _Repo.SelectSymUserById(Id);
            //return View(new SymUserRollRepo().SelectAll());
            return PartialView("UserRoll", uservm);
        }
        public ActionResult SelectUserForRollfilter(string Id)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_14", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            UserGroupRepo _repoGroup = new UserGroupRepo();
            UserGroupVM groupVm = new UserGroupVM();
            groupVm = _repoGroup.SelectById(Id);
            return PartialView("UserRoll", groupVm);
        }
        public ActionResult SelectUserForRollSearch(string gid, string SymArea)
        {

            UserGroupVM uservm = new UserGroupVM();
            uservm.SymUserRollVMs = _Repo.SelectAllSymUserRollByGroupId(gid, SymArea);

            return View("SymUserRoll", uservm);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult SymUserRollDelete(string ids)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var permission = _reposur.SymRoleSession(identity.UserId, "1_14", "delete").ToString();
            Session["permission"] = permission;
            SymUserRollVM vm = new SymUserRollVM();

            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = new SymUserRoleRepo().Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
    }
}
