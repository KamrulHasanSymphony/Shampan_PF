using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using SymOrdinary;
using SymReporting.PF;
using SymRepository.Common;
using SymRepository.PF;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.PF;
using SymWebUI.Areas.PF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace SymWebUI.Areas.PF.Controllers
{
    public class LeftController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        EmployeeLeftInformationRepo _repo = new EmployeeLeftInformationRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            return View(empVM);
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            var leftDateFilter = Convert.ToString(Request["sSearch_6"]);
            //Code
            //EmpName 
            //Department 
            //Designation
            //JoinDate
            //LeftDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            DateTime fromLeftDate = DateTime.MinValue;
            DateTime toLeftDate = DateTime.MaxValue;
            if (leftDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = leftDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(leftDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(leftDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = leftDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(leftDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(leftDateFilter.Split('~')[1]) : DateTime.MinValue;
            }


            #endregion Column Search


            EmployeeLeftInformationRepo _empLeftRepo = new EmployeeLeftInformationRepo();
            List<EmployeeLeftInformationVM> getAllData = new List<EmployeeLeftInformationVM>();

            IEnumerable<EmployeeLeftInformationVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string branchId = Session["BranchId"].ToString();
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = _empLeftRepo.SelectAll(branchId);
            }
            else
            {
                string[] conditionFields = { "el.EmployeeId" };
                string[] conditionValues = { Identit.EmployeeId };
                getAllData.Add(_empLeftRepo.SelectAll(branchId, 0, conditionFields, conditionValues).FirstOrDefault());

            }

            //Check whether the companies should be filtered by keyword
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
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.LeftDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~") || (leftDateFilter != "" && leftDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    &&
                                    (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    &&
                                    (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    &&
                                    (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                     &&
                                    (fromLeftDate == DateTime.MinValue || fromLeftDate <= Convert.ToDateTime(c.LeftDate))
                                    &&
                                    (toLeftDate == DateTime.MaxValue || toLeftDate >= Convert.ToDateTime(c.LeftDate))

                                    );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<EmployeeLeftInformationVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 6 && isSortable_6 ? Ordinary.DateToString(c.LeftDate) :
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
                , c.Department 
                , c.Designation
                , c.JoinDate
                , c.LeftDate
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


        public ActionResult _indexActiveEmployee(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            //Code
            //EmpName 
            //Department 
            //Designation
            //JoinDate

            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
            }
            #endregion Column Search


            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            List<EmployeeInfoVM> getAllData = new List<EmployeeInfoVM>();
            IEnumerable<EmployeeInfoVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string branchId = Session["BranchId"].ToString();
            if (identity.IsAdmin || identity.IsHRM)
            {
                getAllData = _empRepo.SelectAllActiveEmp(branchId);
            }
            else
            {
                getAllData.Add(_empRepo.SelectById(Identit.EmployeeId));

            }

            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.JoinDate.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || (joinDateFilter != "" && joinDateFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    &&
                                    (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    &&
                                    (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    &&
                                    (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.JoinDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.JoinDate))
                                );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
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
                , c.Department 
                , c.Designation
                , c.JoinDate
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



        [HttpGet]
        public ActionResult Left()
        {
            EmployeeInfoVM empVM = new EmployeeInfoVM();
            empVM.leftInformation = new EmployeeLeftInformationVM();
            return View(empVM);
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Left(EmployeeInfoVM evm, HttpPostedFileBase LeftInformationF)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_21", "process").ToString();
            EmployeeLeftInformationVM vm = new EmployeeLeftInformationVM();
            vm = evm.leftInformation;
            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                vm.FileName = LeftInformationF.FileName;
            }
            string[] retResults = new string[6];
            EmployeeLeftInformationRepo leftRepo = new EmployeeLeftInformationRepo();
            //vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.CreatedBy = identity.Name;
            //vm.CreatedFrom = identity.WorkStationIP;

            if (vm.Id > 0)
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                retResults = leftRepo.Update(vm);
            }
            else
            {

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                retResults = leftRepo.Insert(vm);
            }

            if (LeftInformationF != null && LeftInformationF.ContentLength > 0)
            {
                var path = Path.Combine(Server.MapPath("~/Files/LeftInformation"), retResults[2] + LeftInformationF.FileName);
                LeftInformationF.SaveAs(path);
            }
            //return Json(retResults[0] + "~" + retResults[1], JsonRequestBehavior.AllowGet);
            Session["result"] = retResults[0] + "~" + retResults[1];

            if (vm.Id > 0)
            {
                //return RedirectToAction("Left", "Left");
                return RedirectToAction("Index", "Left");
            }
            else
            {
                return RedirectToAction("Left", "Left");
                //return RedirectToAction("Index", "Left");
            }
        }

        public ActionResult leftdetailCreate(string id, string employeeId = "", string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeftInformationVM empleft = new EmployeeLeftInformationVM();
            EmployeeLeftInformationRepo empleftRepo = new EmployeeLeftInformationRepo();

            string[] conditionFields = { "el.EmployeeId", "ve.Code" };
            string[] conditionValues = { employeeId, empcode }; ;

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(empleft.EmployeeId))
                {
                    employeeId = empleft.EmployeeId;
                }
            }


            if (!string.IsNullOrEmpty(employeeId))
            {
                vm = repo.SelectByIdAll(employeeId);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructureAll(empcode, btn);
            }


            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    employeeId = vm.Id;
                }
            }


            empleft.EmployeeId = vm.Id;
            vm.leftInformation = empleft;
            return PartialView("_leftDetail", vm);
        }



        public ActionResult leftdetailEdit(string id, string employeeId = "", string empcode = "", string btn = "current")
        {
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeLeftInformationVM empleft = new EmployeeLeftInformationVM();
            EmployeeLeftInformationRepo empleftRepo = new EmployeeLeftInformationRepo();

            string[] conditionFields = { "el.EmployeeId", "ve.Code" };
            string[] conditionValues = { employeeId, empcode }; ;
            string branchId = Session["BranchId"].ToString();
            empleft = empleftRepo.SelectAll(branchId, Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();


            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(empleft.EmployeeId))
                {
                    employeeId = empleft.EmployeeId;
                }
            }


            if (!string.IsNullOrEmpty(employeeId))
            {
                vm = repo.SelectByIdAll(employeeId);
            }
            if (!string.IsNullOrWhiteSpace(empcode) && !string.IsNullOrWhiteSpace(btn))
            {
                vm = repo.SelectEmpStructureAll(empcode, btn);
            }


            if (string.IsNullOrWhiteSpace(employeeId))
            {
                if (!string.IsNullOrWhiteSpace(vm.Id))
                {
                    employeeId = vm.Id;
                }
            }


            empleft.EmployeeId = vm.Id;
            vm.leftInformation = empleft;
            return PartialView("_leftDetail", vm);
        }


        [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            EmployeeLeftInformationVM vm = new EmployeeLeftInformationVM();
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

