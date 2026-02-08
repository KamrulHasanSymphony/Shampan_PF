using OfficeOpenXml;
using SymOrdinary;
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
    public class EmployeePFPaymentController : Controller
    {
        public EmployeePFPaymentController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        EmployeePFPaymentRepo _eaRepo = new EmployeePFPaymentRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        //
        // GET: /PF/EmployeePFPayment/

        public ActionResult Index()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }

        public ActionResult _index(JQueryDataTableParamVM param)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var EmployeeNameFilter = Convert.ToString(Request["sSearch_2"]);
            var OpeningValueFilter = Convert.ToString(Request["sSearch_3"]);
            var OpeningDateFilter = Convert.ToString(Request["sSearch_4"]);
            var PostFilter = Convert.ToString(Request["sSearch_5"]);

            #endregion
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeePFPaymentRepo arerepo = new EmployeePFPaymentRepo();
            string branchId = Session["BranchId"].ToString();
            var getAllData = arerepo.SelectAll(branchId);
            IEnumerable<EmployeePFPaymentVM> filteredData;
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
                var isSearchable7 = Convert.ToBoolean(Request["bSearchable_7"]);
                var isSearchable8 = Convert.ToBoolean(Request["bSearchable_8"]);
                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.EmployeeContribution.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.EmployerContribution.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.EmployeeProfit.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable6 && c.EmployerProfit.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable7 && c.PaymentDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable8 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (codeFilter != "" || EmployeeNameFilter != "" || OpeningValueFilter != "" || OpeningDateFilter != "" || PostFilter != "")
            {
                filteredData = filteredData.Where(c =>
                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                    && (EmployeeNameFilter == "" || c.EmpName.ToLower().Contains(EmployeeNameFilter.ToLower()))
                    && (OpeningValueFilter == "" || c.EmployeeContribution.ToString().Contains(OpeningValueFilter.ToLower()))
                    && (OpeningValueFilter == "" || c.EmployerContribution.ToString().Contains(OpeningValueFilter.ToLower()))
                    && (OpeningValueFilter == "" || c.EmployeeProfit.ToString().Contains(OpeningValueFilter.ToLower()))
                    && (OpeningValueFilter == "" || c.EmployerProfit.ToString().Contains(OpeningValueFilter.ToLower()))
                    && (OpeningDateFilter == "" || c.PaymentDate.ToLower().Contains(OpeningDateFilter.ToLower()))
                    //&& (PostFilter == "" || c.Post.ToLower().Contains(PostFilter.ToLower()))

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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeePFPaymentVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.EmployeeContribution.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.EmployerContribution.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.EmployeeProfit.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.EmployerProfit.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.PaymentDate.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.Post.ToString() :
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
                             , c.EmployeeContribution.ToString() 
                             , c.EmployerContribution.ToString() 
                             , c.EmployeeProfit.ToString() 
                             , c.EmployerProfit.ToString() 
                             , c.PaymentDate.ToString()
                             , c.Post==true ? "Posted" : "Not Posted"
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

        public ActionResult SingleEmployeePFPaymentEdit(string PFPaymentId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            EmployeePFPaymentVM empPFPaymentvm = new EmployeePFPaymentVM();
            if (!string.IsNullOrWhiteSpace(PFPaymentId))
                empPFPaymentvm = _eaRepo.SelectByIdAll(PFPaymentId);
            EmployeeInfoVM vm = new EmployeeInfoVM();
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            //if (!string.IsNullOrWhiteSpace(PFPaymentId) && !string.IsNullOrWhiteSpace(empPFPaymentvm.Id))
            //{
            vm = repo.SelectById(empPFPaymentvm.EmployeeId);
            //}
            vm.empPFPaymentVM = empPFPaymentvm;
            Session["PFPaymentId"] = empPFPaymentvm.Id;
            return View(vm);
        }

        public ActionResult DetailCreate(string empcode = "", string btn = "current", string id = "0", string Operation = "")
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            string EmployeeId = "";
            EmployeeInfoRepo repo = new EmployeeInfoRepo();
            //EmployeeOtherEarningRepo arerepo = new EmployeeOtherEarningRepo();
            EmployeePFPaymentVM PFPaymentVM = new EmployeePFPaymentVM();
            EmployeeInfoVM vm = new EmployeeInfoVM();
            if (!string.IsNullOrEmpty(Session["PFPaymentId"] as string) && Session["PFPaymentId"] as string != "0")
            {
                string PFPaymentId = Session["PFPaymentId"] as string;
                PFPaymentVM = _eaRepo.SelectByIdAll(PFPaymentId);//find emp code
                vm = repo.AllSelectById(PFPaymentVM.EmployeeId);
                vm.empPFPaymentVM = PFPaymentVM;
                Session["PFPaymentId"] = "";
                // find exist earning date
            }
            else if (id != "0" && !string.IsNullOrWhiteSpace(id))
            {
                PFPaymentVM = _eaRepo.SelectById(id);//find emp code
                vm = repo.AllSelectById(PFPaymentVM.EmployeeId);
                vm.empPFPaymentVM = PFPaymentVM;
                // find exist earning date
            }
            else
            {
                vm = repo.SelectEmpForSearchAll(empcode, btn);

                if (!string.IsNullOrWhiteSpace(vm.EmployeeId))
                {
                    PFPaymentVM = _eaRepo.SelectByIdAll("", vm.EmployeeId);
                }

                if (vm.EmpName == null)
                {
                    vm.EmpName = "Employee Name";
                }
                else
                {
                    EmployeeId = vm.EmployeeId;
                }

                //svms = arerepo.SingleEmployeeEntry(EmployeeId, FiscalYearDetailId);
                vm.empPFPaymentVM = PFPaymentVM;
                vm.empPFPaymentVM.EmployeeId = EmployeeId;
                vm.Operation = Operation;
                if (vm.Operation.ToLower() == "add")
                {
                    vm.Id = null;
                    vm.empPFPaymentVM.Id = null;
                    vm.empPFPaymentVM.EmployeeContribution = 0;
                    vm.empPFPaymentVM.EmployerContribution = 0;
                    vm.empPFPaymentVM.EmployeeProfit = 0;
                    vm.empPFPaymentVM.EmployerProfit = 0;
                    vm.empPFPaymentVM.PaymentDate = null;
                    vm.empPFPaymentVM.Post = false;


                }
            }
            return PartialView("_detailCreate", vm);
        }

        [HttpPost]
        public ActionResult Create(EmployeeInfoVM empVM)
        {
            EmployeePFPaymentVM vm = new EmployeePFPaymentVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empPFPaymentVM;
                if (string.IsNullOrEmpty(vm.EmployeeId) && !string.IsNullOrEmpty(empVM.Id))
                {
                    vm.EmployeeId = empVM.Id;
                }
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;

                #region Get FiscalYearDetail Id
                DateTime newDate = Convert.ToDateTime(vm.PaymentDate);
                string sMonth = newDate.ToString("MMM");
                string sYear = newDate.ToString("yy");
                string PriodName = sMonth + "-" + sYear;


                var fydVM = new FiscalYearRepo().SelectAll_FiscalYearDetail(0, new[] { "PeriodName" }, new[] { PriodName })
                    .FirstOrDefault();
                vm.FiscalYearDetailId = fydVM.Id.ToString();
                #endregion
                vm.BranchId = Session["BranchId"].ToString();
                vm.EmployeeId = empVM.empPFPaymentVM.EmployeeId;
                result = _eaRepo.Insert(vm);

                return Json(result[0] + "~" + result[1] + "~" + result[2], JsonRequestBehavior.AllowGet);

                ////Session["result"] = result[0] + "~" + result[1];

                ////return Redirect("/PF/EmployeePFPayment/SingleEmployeePFPaymentEdit?PFPaymentId=" + result[2].ToString());

                //return RedirectToAction("SingleEmployeePFPaymentEdit", result[2]);

            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Update(EmployeeInfoVM empVM)
        {
            EmployeePFPaymentVM vm = new EmployeePFPaymentVM();
            string[] result = new string[6];
            try
            {
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm = empVM.empPFPaymentVM;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                //vm.FiscalYearDetailId = empVM.FiscalYearDetailId;

                #region Get FiscalYearDetail Id
                DateTime newDate = Convert.ToDateTime(vm.PaymentDate);
                string sMonth = newDate.ToString("MMM");
                string sYear = newDate.ToString("yy");
                string PriodName = sMonth + "-" + sYear;

                var fydVM = new FiscalYearRepo().SelectAll_FiscalYearDetail(0, new[] { "PeriodName" }, new[] { PriodName })
                    .FirstOrDefault();
                vm.FiscalYearDetailId = fydVM.Id.ToString();
                #endregion

                result = _eaRepo.Update(vm);
                ////if (result[0].ToLower() == "success" && btn.ToLower() != "save")
                ////{
                ////    result[1] = "Data Deleted Successfully";
                ////}
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Post(EmployeeInfoVM empVM)
        {
            EmployeePFPaymentVM vm = new EmployeePFPaymentVM();
            string[] result = new string[6];
            try
            {
                vm = empVM.empPFPaymentVM;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;

                result = _eaRepo.Post(vm);

                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult MultiplePost(string ids)
        {

            string[] strId = ids.Split('~');
            var employeePFPaymentVM = _eaRepo.SelectByIdAll(strId[0]);
            if (employeePFPaymentVM != null)
            {
                if (employeePFPaymentVM.Post == true)
                {
                    string[] resultpf = new string[6];
                    resultpf[0] = "Fail";
                    resultpf[1] = "Data Already Posted";
                    return Json(resultpf, JsonRequestBehavior.AllowGet);
                }
            }

            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            string[] result = new string[6];

            EmployeePFPaymentVM EarningVM = new EmployeePFPaymentVM();

            EarningVM.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            EarningVM.LastUpdateBy = identity.Name;
            EarningVM.LastUpdateFrom = identity.WorkStationIP;
            string[] a = ids.Split('~');
            result = _eaRepo.MultiplePost(EarningVM, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportEmployeePFPayment()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/PF/Home");
            }
            return View();
        }

        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            string[] result = new string[6];
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/PF/Home");
                }
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(fullPath);
                }
                ShampanIdentityVM vm = new ShampanIdentityVM();
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                result = _eaRepo.ImportExcelFile(fullPath, file.FileName, vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEmployeePFPayment");
                //return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeePFPayment");
            }
        }

        public ActionResult DownloadExcel_Employee(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
            , string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                EmployeePFPaymentVM vm = new EmployeePFPaymentVM();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/PF/Home");
                }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                string contentType = MimeMapping.GetMimeMapping(fullPath);
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                vm.DesignationId = DesignationId;
                vm.DepartmentId = DepartmentId;
                vm.SectionId = SectionId;
                vm.ProjectId = ProjectId;
                vm.Orderby = Orderby;
                vm.Code = CodeF;
                vm.CodeT = CodeT;

                dt = _eaRepo.ExportExcelFileFormEmployee(vm, fullPath, FileName);
                //exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "EmployeePFPayment" + "-" + DateTime.Now.ToString("yyyyMMdd");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEmployeePFPayment");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeePFPayment");
            }
        }


        public ActionResult DownloadExcel_Opening(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT
          , string Orderby = null)
        {
            DataTable dt = new DataTable();
            string[] result = new string[6];
            try
            {
                EmployeePFPaymentVM vm = new EmployeePFPaymentVM();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/PF/Home");
                }
                string FileName = "Download.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                string contentType = MimeMapping.GetMimeMapping(fullPath);
                //string fullPath = @"C:\";
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                vm.DesignationId = DesignationId;
                vm.DepartmentId = DepartmentId;
                vm.SectionId = SectionId;
                vm.ProjectId = ProjectId;
                vm.Orderby = Orderby;
                vm.Code = CodeF;
                vm.CodeT = CodeT;

                dt = _eaRepo.ExportExcelFileFormPFOpening(vm, fullPath, FileName);
                //exp(dt);
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                string filename = "EmployeePFPayment" + "-" + DateTime.Now.ToString("yyyyMMdd");
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportEmployeePFPayment");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("ImportEmployeePFPayment");
            }
        }




    }
}
