using JQueryDataTables.Models;
using OfficeOpenXml;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Payroll;
using SymViewModel.Common;
using SymViewModel.Payroll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class GradeController : Controller
    {
        //
        // GET: /Common/Grade/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        GradeRepo _repo = new GradeRepo();
        #region Actions
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_11", "index").ToString();

            return View();
        }
        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Grade information.
        /// </summary>      
        /// <returns>View containing Grade</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var nameFilter = Convert.ToString(Request["sSearch_2"]);
            var minSalaryFilter = Convert.ToString(Request["sSearch_3"]);
            var maxSalaryFilter = Convert.ToString(Request["sSearch_4"]);
            //Code
            //Name
            //MinSalary
            //MaxSalary
            var minSalaryFrom = 0;
            var minSalaryTo = 0;
            if (minSalaryFilter.Contains('~'))
            {
                minSalaryFrom = minSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(minSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(minSalaryFilter.Split('~')[0]) : 0;
                minSalaryTo = minSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(minSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(minSalaryFilter.Split('~')[1]) : 0;
            }

            var maxSalaryFrom = 0;
            var maxSalaryTo = 0;
            if (maxSalaryFilter.Contains('~'))
            {
                maxSalaryFrom = maxSalaryFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(maxSalaryFilter.Split('~')[0]) == true ? Convert.ToInt32(maxSalaryFilter.Split('~')[0]) : 0;
                maxSalaryTo = maxSalaryFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(maxSalaryFilter.Split('~')[1]) == true ? Convert.ToInt32(maxSalaryFilter.Split('~')[1]) : 0;
            }

            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }
            #endregion Column Search

            #region Search and Filter Data

            var getAllData = _repo.SelectAll();
            IEnumerable<GradeVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.MinSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.MaxSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

             #endregion Search and Filter Data

            #region Column Filtering
            if (codeFilter != "" || nameFilter != ""|| (minSalaryFilter != "~" && minSalaryFilter != "") || (maxSalaryFilter != "~" && maxSalaryFilter != "") )
            {
                filteredData = filteredData
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                            &&
                                            (nameFilter == "" || c.Name.ToLower().Contains(nameFilter.ToLower()))
                                            &&
                                            (minSalaryFrom == 0 || minSalaryFrom <= Convert.ToInt32(c.MinSalary))
                                            &&
                                            (minSalaryTo == 0 || minSalaryTo >= Convert.ToInt32(c.MinSalary))
                                            &&
                                            (maxSalaryFrom == 0 || maxSalaryFrom <= Convert.ToInt32(c.MaxSalary))
                                            &&
                                            (maxSalaryTo == 0 || maxSalaryTo >= Convert.ToInt32(c.MaxSalary))
                                        
                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<GradeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.MinSalary.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.MaxSalary.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.Remarks :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies select new[] { 
                Convert.ToString(c.Id)
                , c.Code //+ "~" + Convert.ToString(c.Id)
                , c.Name
                , c.MinSalary.ToString()
                , c.MaxSalary.ToString()
                //, c.Remarks 
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

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_11", "add").ToString();
            return PartialView();
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(GradeVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = Identity.Name;
            vm.CreatedFrom = Identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(Identity.BranchId);
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
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_11", "edit").ToString();
            GradeVM vm = new GradeVM();
            vm = _repo.SelectById(id);
            return PartialView(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(GradeVM vm, string btn)
        {
            string[] result = new string[6];
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
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
        public ActionResult Delete(string id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_11", "delete").ToString();
            ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            GradeVM vm = new GradeVM();
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = Identity.Name;
            vm.LastUpdateFrom = Identity.WorkStationIP;
            vm.Id = id;
            try
            {
                //string[] result = new string[6];
                //result = _repo.Delete(vm);
                //Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Deleted";
                return RedirectToAction("Index");
            }

        }
        [Authorize(Roles = "Master,Admin,Account")]
        public ActionResult GradeDelete(string ids)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_11", "delete").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Common/Home");
            }
            GradeVM vm = new GradeVM();

            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion Actions

        public ActionResult Import()
        {
            return View();
        }
        public ActionResult ExportExcell(ExportImportVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            DataTable dt = new DataTable();
            ExcelPackage excel = new ExcelPackage();

            try
            {

                ExportImportRepo _repo = new ExportImportRepo();

                dt = _repo.SelectGradeInfo(VM);

                #region Excel

                string filename = "GradeInfo Data";
                var workSheet = excel.Workbook.Worksheets.Add("GradeInfo Data");
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                #endregion

            }
            catch (Exception ex)
            {
                Session["result"] = "Fail" + "~" + ex.Message.Replace("\r", "").Replace("\n", "");
                return RedirectToAction("Index");
            }

            finally { }
            return RedirectToAction("Index");

            // return Json(rVM, JsonRequestBehavior.AllowGet);
        }
     
    }
}
