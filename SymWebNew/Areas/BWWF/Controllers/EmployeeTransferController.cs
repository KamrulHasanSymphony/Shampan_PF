using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json;
using OfficeOpenXml;
using SymOrdinary;
using SymReporting.BWWF;
using SymRepository.Common;
using SymRepository.BWWF;
using SymViewModel.Common;
using SymViewModel.HRM;
using SymViewModel.BWWF;
using SymWebUI.Areas.BWWF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.BWWF.Controllers
{
    public class EmployeeTransferController : Controller
    {
        public EmployeeTransferController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }

        EmployeeTransferRepo _eaRepo = new EmployeeTransferRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //
        // GET: /BWWF/EmployeePFOpeinig/

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            return View();
        }

        public ActionResult _index(JQueryDataTableParamVM param)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmpName = Convert.ToString(Request["sSearch_2"]);
            var FromBranchFilter = Convert.ToString(Request["sSearch_3"]);
            var ToBranchFilter = Convert.ToString(Request["sSearch_4"]);
            var TransferDateFilter = Convert.ToString(Request["sSearch_5"]);
        
            #endregion
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            var getAllData = _eaRepo.SelectAll();
            IEnumerable<EmployeeTransferVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);             
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.FromBranch.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.ToBranch.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.TransferDate.ToString().ToLower().Contains(param.sSearch.ToLower())                  
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmpName != "" || FromBranchFilter != "" || ToBranchFilter != "" || TransferDateFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (EmpName == "" || c.EmpName.ToLower().Contains(EmpName.ToLower()))
                    && (FromBranchFilter == "" || c.FromBranch.ToString().Contains(FromBranchFilter.ToLower()))
                    && (ToBranchFilter == "" || c.ToBranch.ToString().Contains(ToBranchFilter.ToLower()))
                    && (TransferDateFilter == "" || c.TransferDate.ToString().Contains(TransferDateFilter.ToLower()))                 
                    );
            }
            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeTransferVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.FromBranch.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.ToBranch.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.TransferDate.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] 
                         { 
                             Convert.ToString(c.Id)
                             ,c.Code
                             , c.EmpName
                             , c.FromBranch.ToString() 
                             , c.ToBranch.ToString() 
                             , c.TransferDate.ToString()                          
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

        public ActionResult SingleEmployeePFTransferEdit(string PFOpeinigId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            EmployeeTransferVM empPFForTransferVM = new EmployeeTransferVM();
            if (!string.IsNullOrWhiteSpace(PFOpeinigId))
                empPFForTransferVM = _eaRepo.SelectById(PFOpeinigId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            if (!string.IsNullOrWhiteSpace(PFOpeinigId) && !string.IsNullOrWhiteSpace(empPFForTransferVM.Id))
            {
                vm = repo.SelectById(empPFForTransferVM.Code);
            }
            //vm.empPFForTransferVM = empPFForTransferVM;
            Session["PFOpeinigId"] = empPFForTransferVM.Id;
            return View(vm);
        }

        public ActionResult DetailCreate(string empcode = "", string btn = "current", string id = "0")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            //EmployeeOtherEarningRepo arerepo = new EmployeeOtherEarningRepo();
            EmployeeTransferVM empPFForTransferVM = new EmployeeTransferVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(Session["PFOpeinigId"] as string) && Session["PFOpeinigId"] as string != "0")
            {
                string PFOpeinigId = Session["PFOpeinigId"] as string;
                empPFForTransferVM = _eaRepo.SelectById(PFOpeinigId);//find emp code
                vm = repo.SelectById(empPFForTransferVM.Code);
                //vm.empPFForTransferVM = empPFForTransferVM;
                Session["PFOpeinigId"] = "";
                // find exist earning date
            }
            else if (id != "0" && !string.IsNullOrWhiteSpace(id))
            {
                empPFForTransferVM = _eaRepo.SelectById(id);//find emp code
                vm = repo.SelectById(empPFForTransferVM.Code);
                //vm.empPFForTransferVM = empPFForTransferVM;
                // find exist earning date
            }
            else
            {
                vm = repo.SelectEmpForSearch(empcode, btn);

                if (!string.IsNullOrWhiteSpace(vm.Code))
                {
                    empPFForTransferVM = _eaRepo.SelectById("", vm.Code);
                }

                if (vm.EmpName == null)
                {
                    vm.EmpName = "Employee Name";
                }
                else
                {
                    EmployeeId = vm.Code;
                }

                //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
                //vm.empPFForTransferVM = empPFForTransferVM;
                vm.empPFForTransferVM.EmployeeId = EmployeeId;
            }
            return PartialView("_detailCreate", vm);
        }

        [HttpPost]
        public ActionResult Create(EmployeeInfoVM empVM)
        {
            EmployeeTransferVM vm = new EmployeeTransferVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //vm = empVM.empPFForTransferVM;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.Code = empVM.Code;

                result = _eaRepo.Insert(vm);

                return Json(result[0] + "~" + result[1] + "~" + result[2], JsonRequestBehavior.AllowGet);            

            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Update(EmployeeInfoVM empVM)
        {
            EmployeeTransferVM vm = new EmployeeTransferVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                //vm = empVM.empPFForTransferVM;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                //vm.FiscalYearDetailId = empVM.FiscalYearDetailId;

                result = _eaRepo.Update(vm);
            
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ImportEmployeePFOpeinig()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/BWWF/Home");
            }
            return View();
        }

    }
}
