using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using SymOrdinary;
using SymReporting.PF;
using SymRepository.Common;

using SymRepository.PF;
using SymViewModel.Common;

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
    public class EmployeeInfoPFController : Controller
    {
        //
        // GET: /PF/EmployeeInfoPF/
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexRegnation()
        {
            return View();
        }
        /// <summary>
        /// Created: 01 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all employee information.
        /// </summary>      
        /// <returns>View containing employee information</returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            EmployeeInfoForPFRepo _Repo = new EmployeeInfoForPFRepo();

            #region Column Search
            var Id = Convert.ToString(Request["sSearch_0"]);
            var Code = Convert.ToString(Request["sSearch_1"]);
            var Name = Convert.ToString(Request["sSearch_2"]);
            var Department = Convert.ToString(Request["sSearch_3"]);
            var DateOfBirth = Convert.ToString(Request["sSearch_4"]);
            var JoinDate = Convert.ToString(Request["sSearch_5"]);
            #endregion Column Search
            #region Search and Filter Data

            string[] conditionFields = { "ve.BranchId" };
            string[] conditionValues = { Session["BranchId"].ToString() };

            var getAllData = _Repo.SelectAllEmployeeInfoForPF(conditionFields, conditionValues);
            IEnumerable<EmployeeInfoForPFVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                        isSearchable0 && c.Id.ToString().Contains(param.sSearch.ToLower())
                     || isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable4 && c.DateOfBirth.ToString().ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable5 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (Code != "" || Name != "" || Department != "" || DateOfBirth != "" || JoinDate != "")
            {
                filteredData = filteredData
                                .Where(c => (Code == "" || c.Code.ToLower().Contains(Code.ToLower()))
                                            && (Name == "" || c.Name.ToLower().Contains(Name.ToLower()))
                                            && (Department == "" || c.Department.ToLower().Contains(Department.ToLower()))
                                            && (DateOfBirth == "" || c.DateOfBirth.ToString().ToLower().Contains(DateOfBirth.ToLower()))
                                            && (JoinDate == "" || c.JoinDate.ToString().ToLower().Contains(JoinDate.ToLower()))
                                        );
            }
            #endregion Column Filtering

            var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoForPFVM, string> orderingFunction = (c =>
                sortColumnIndex == 0 && isSortable_0 ? c.Id.ToString() :
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.DateOfBirth.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.JoinDate.ToString() :
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "desc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)               
                ,c.Code              
                ,c.Name
                ,c.Department               
                ,c.DateOfBirth  
                ,c.JoinDate                                                                 
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

        public ActionResult _indexRegnation(JQueryDataTableParamModel param)
        {
            EmployeeInfoForPFRepo _Repo = new EmployeeInfoForPFRepo();

            #region Column Search
            var Id = Convert.ToString(Request["sSearch_0"]);
            var Code = Convert.ToString(Request["sSearch_1"]);
            var Name = Convert.ToString(Request["sSearch_2"]);
            var Department = Convert.ToString(Request["sSearch_3"]);
            var DateOfBirth = Convert.ToString(Request["sSearch_4"]);
            var JoinDate = Convert.ToString(Request["sSearch_5"]);
            #endregion Column Search
            #region Search and Filter Data

            string[] conditionFields = { "ve.BranchId" };
            string[] conditionValues = { Session["BranchId"].ToString() };

            var getAllData = _Repo.SelectRegnationEmployeeInfoForPF(conditionFields, conditionValues);
            IEnumerable<EmployeeInfoForPFVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable0 = Convert.ToBoolean(Request["bSearchable_0"]);
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                        isSearchable0 && c.Id.ToString().Contains(param.sSearch.ToLower())
                     || isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable2 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable4 && c.DateOfBirth.ToString().ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable5 && c.JoinDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (Code != "" || Name != "" || Department != "" || DateOfBirth != "" || JoinDate != "")
            {
                filteredData = filteredData
                                .Where(c => (Code == "" || c.Code.ToLower().Contains(Code.ToLower()))
                                            && (Name == "" || c.Name.ToLower().Contains(Name.ToLower()))
                                            && (Department == "" || c.Department.ToLower().Contains(Department.ToLower()))
                                            && (DateOfBirth == "" || c.DateOfBirth.ToString().ToLower().Contains(DateOfBirth.ToLower()))
                                            && (JoinDate == "" || c.JoinDate.ToString().ToLower().Contains(JoinDate.ToLower()))
                                        );
            }
            #endregion Column Filtering

            var isSortable_0 = Convert.ToBoolean(Request["bSortable_0"]);
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoForPFVM, string> orderingFunction = (c =>
                sortColumnIndex == 0 && isSortable_0 ? c.Id.ToString() :
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.Name :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.DateOfBirth.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.JoinDate.ToString() :
                "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "desc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                Convert.ToString(c.Id)               
                ,c.Code              
                ,c.Name
                ,c.Department               
                ,c.DateOfBirth  
                ,c.JoinDate                                                                 
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

        public ActionResult Create(EmployeeInfoForPFVM vm)
        {
            return View("Create", vm);
        }

        /// <summary>
        /// Created: 01 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the creation or editing of Employee Info for Provident Fund (PF).
        /// Sets metadata like CreatedAt, CreatedBy, and handles database persistence.
        /// </summary>
        /// <param name="vm">The EmployeeInfoForPFVM object containing the form data</param>
        /// <returns>Redirects to Index on success, otherwise returns the same view with the provided model</returns>

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Syncs employee PF-related data from an external API into the local system.
        /// Stores the result in session and redirects to the Index view.
        /// </summary>
        /// <returns>Redirects to the Index view after syncing is complete</returns>
        public ActionResult SyncFromAPI()
        {
            string[] result = new string[6];

            result = new EmployeeInfoForPFRepo().SelectFromAPIData();

            Session["result"] = result[0] + "~" + result[1];

            return RedirectToAction("Index");
        }
        public ActionResult ReActive(string Id)
        {
            string[] result = new string[6];

            try
            {

                result = new EmployeeInfoForPFRepo().ReActiveEmployeeInfoForPF(Id);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("IndexRegnation");

            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("IndexRegnation");
            }
        }
        public ActionResult Import()
        {
            return View();
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the upload and import of employee PF data from an Excel file.  
        /// Captures creation metadata, imports data through repository, and logs any exceptions.  
        /// </summary>
        /// <param name="vm">ViewModel containing uploaded Excel file and employee metadata</param>
        /// <returns>Redirects to the Index view after processing</returns>
        public ActionResult UploadExcel(EmployeeInfoForPFVM vm)
        {
            string[] result = new string[6];
            try
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = Session["BranchId"].ToString();

                result = new EmployeeInfoForPFRepo().ImportExcelFile(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Exports employee information to an Excel file based on filter criteria provided in the ViewModel.  
        /// The exported file is downloaded directly via the HTTP response.  
        /// </summary>
        /// <param name="VM">ViewModel containing export filter parameters</param>
        /// <returns>Redirects to Index after export is complete or if an error occurs</returns>
        [Authorize]
        public ActionResult ExportExcell(ExportImportVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            DataTable dt = new DataTable();
            ExcelPackage excel = new ExcelPackage();

            try
            {
                VM.BranchId = Session["BranchId"].ToString();
                ExportImportRepo _repo = new ExportImportRepo();

                dt = _repo.SelectEmployeeInfo(VM);

                #region Excel

                string filename = "Employee Data";
                var workSheet = excel.Workbook.Worksheets.Add("Employee Data");
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

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves employee PF information by ID and loads the edit view with the retrieved data.  
        /// </summary>
        /// <param name="id">The ID of the employee PF record to be edited</param>
        /// <returns>Returns the edit view populated with the employee PF data</returns>
        public ActionResult Edit(int id)
        {
            EmployeeInfoForPFRepo _Repo = new EmployeeInfoForPFRepo();
            EmployeeInfoForPFVM vm = new EmployeeInfoForPFVM();

            vm = _Repo.SelectById(id);
            return View(vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Deletes an employee PF record by its ID and returns to the Index view with a result message.  
        /// </summary>
        /// <param name="Id">The ID of the employee PF record to be deleted</param>
        /// <returns>Returns the Index view with a session message indicating success or failure</returns>
        public ActionResult Delete(int Id)
        {
            string[] result = new string[6];
            EmployeeInfoForPFRepo _Repo = new EmployeeInfoForPFRepo();
            EmployeeInfoForPFVM vm = new EmployeeInfoForPFVM();

            result = _Repo.DeleteEmployeeInfoForPF(Id);
            Session["result"] = result[0] + "~" + result[1];
            return View("Index");
        }

        [HttpPost]
        public ActionResult CreateEdit(EmployeeInfoForPFVM vm, HttpPostedFileBase file)
        {
            string[] result = new string[6];
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.IsActive = true;
            vm.BranchId = Session["BranchId"].ToString();

            try
            {
                EmployeeInfoForPFRepo repo = new EmployeeInfoForPFRepo();

                // 🧩 Preserve old photo if updating and no new file uploaded
                if (vm.Id > 0)
                {
                    var existing = repo.SelectById(vm.Id);
                    if (file == null || file.ContentLength == 0)
                    {
                        vm.PhotoName = existing.PhotoName;
                    }
                }

                // 🔹 Insert or Update
                result = repo.InsertEmployeeInfoForPF(vm);

                if (result[0] == "Success")
                {
                    int employeeId = vm.Id > 0 ? vm.Id : Convert.ToInt32(result[2]);

                    // 🔹 Only handle new file upload
                    if (file != null && file.ContentLength > 0)
                    {
                        var employee = repo.SelectById(employeeId);
                        string code = employee.Code ?? ("EMP_" + employeeId);
                        string extension = Path.GetExtension(file.FileName);
                        string photoName = code + extension;

                        var photoResult = repo.UpdatePhoto(employeeId, photoName);
                        if (photoResult[0] == "Success")
                        {
                            string dirPath = Server.MapPath("~/Files/EmployeeInfo");
                            if (!Directory.Exists(dirPath))
                                Directory.CreateDirectory(dirPath);

                            string filePath = Path.Combine(dirPath, photoName);
                            file.SaveAs(filePath);
                        }
                    }
                }

                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail~Data Not Successfully Saved!";
                FileLogger.Log(
                    result[0] + Environment.NewLine +
                    result[2] + Environment.NewLine +
                    result[5],
                    this.GetType().Name,
                    ex.Message + Environment.NewLine + result[3]
                );
                return View(vm);
            }
        }

        //////// Nominee Part ////////

        [HttpGet]
        public ActionResult Nominee(int EmployeeId, int Id)
        {
            EmployeeInfoForPFRepo _infoRepo = new EmployeeInfoForPFRepo();
            EmployeeInfoForPFVM vm = _infoRepo.SelectById(EmployeeId);

            if (Id != 0)
            {
                vm.nomineeVM = new EmployeeInfoForPFRepo().SelectByIdNominee(Id);

            }
            else
            {
                EmployeeNomineeVM nom = new EmployeeNomineeVM();
                nom.EmployeeId = EmployeeId.ToString();
                vm.nomineeVM = nom;
            }
            return PartialView("_editNominee", vm);
        }

        public ActionResult _indexNominee(JQueryDataTableParamVM param, string Id)
        {

            EmployeeInfoForPFRepo _empNoRepo = new EmployeeInfoForPFRepo();
            var getAllData = _empNoRepo.SelectAllByEmployeeNominee(Id);
            IEnumerable<EmployeeNomineeVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Relation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                isSearchable3 && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable4 && c.Phone.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeNomineeVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Relation :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.Mobile :
                                                           sortColumnIndex == 3 && isSortable_4 ? c.Phone :
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
                     , c.Name //+ "~" + Convert.ToString(c.Id)
                     , c.Relation
                     , c.Mobile
                     , c.Phone 
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
        [HttpPost]
        public ActionResult Nominee(EmployeeInfoForPFVM vm)
        {
            //Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_18", "add").ToString();

            string[] retResults = new string[6];
            string[] result = new string[6];
            EmployeeInfoForPFRepo empNonApp = new EmployeeInfoForPFRepo();
            EmployeeNomineeVM non = vm.nomineeVM;
            if (non.Id <= 0)
            {
                non.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                non.CreatedBy = identity.Name;
                non.CreatedFrom = identity.WorkStationIP;
                result = empNonApp.InsertNominee(non);
            }
            else
            {
                non.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                non.LastUpdateBy = identity.Name;
                non.LastUpdateFrom = identity.WorkStationIP;
                result = empNonApp.UpdateNominee(non);
            }
            var mgs = result[0] + "~" + result[1];

            Session["mgs"] = "mgs";
            return RedirectToAction("Edit", new { Id = non.EmployeeId, mgs = mgs });
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult NomineeDelete(string ids)
        {
            EmployeeInfoForPFRepo empNonApp = new EmployeeInfoForPFRepo();
            EmployeeNomineeVM vm = new EmployeeNomineeVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = empNonApp.DeleteNominee(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }


    }
}
