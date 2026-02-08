using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;

using SymViewModel.Common;
using SymViewModel.HRM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Data;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using Excel;

namespace SymWebUI.Areas.Common.Controllers
{
    public class ExportImportController : Controller
    {
        //
        // GET: /Common/ExportImport/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all ExportImport information.
        /// </summary>      
        /// <returns>View containing ExportImport</returns>
        public ActionResult Index1()
        {
            EmployeeInfoVM vm = new EmployeeInfoVM();
            vm.Code = "100";
            vm.FullName = "100";
            return View(vm);
        }
        public ActionResult Index(string returnUrl)
        {

            if (!string.IsNullOrEmpty(Session["mgs"] as string))
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_18", "index").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/HRM/Home");
                }
                if (identity.IsESS)
                {
                    return Redirect("/hrm/employeeinfo/Edit/" + identity.EmployeeId);
                }
                List<EmployeeInfoVM> VMs = new List<EmployeeInfoVM>();
                EmployeeInfoVM vm = new EmployeeInfoVM();
                vm.ClientEmployeeIndex = new AppSettingsReader().GetValue("ClientEmployeeIndex", typeof(string)).ToString();
                VMs.Add(vm);
                return View(VMs);
            }
            catch (Exception)
            {
                return Redirect(new AppSettingsReader().GetValue("PAGENOTFOUND", typeof(string)).ToString());
            }
        }

        [Authorize]
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var sectionFilter = Convert.ToString(Request["sSearch_5"]);
            var projecttFilter = Convert.ToString(Request["sSearch_6"]);
            var LocationFilter = Convert.ToString(Request["sSearch_7"]);
            var ZoneFilter = Convert.ToString(Request["sSearch_8"]);
            var CostCenterFilter = Convert.ToString(Request["sSearch_9"]);

            //Code
            //EmpName 
            //Designation
            //Department 
            //Section
            //Projectt



            var fromID = 0;
            var toID = 0;
            if (idFilter.Contains('~'))
            {
                //Split number range filters with ~
                fromID = idFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[0]);
                toID = idFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(idFilter.Split('~')[1]);
            }

            #endregion Column Search

            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectActiveEmp();
            IEnumerable<EmployeeInfoVM> filteredData;
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
                var isSearchable9 = Convert.ToBoolean(Request["bSearchable_9"]);

                filteredData = getAllData
                   .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.Section.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.Project.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.Other1.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable8 && c.Other2.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable9 && c.Other3.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || sectionFilter != "" || projecttFilter != "" || LocationFilter != "" || ZoneFilter != "" | CostCenterFilter != "")
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (sectionFilter == "" || c.Section.ToLower().Contains(sectionFilter.ToLower()))
                                    && (projecttFilter == "" || c.Project.ToLower().Contains(projecttFilter.ToLower()))
                                    && (LocationFilter == "" || c.Other1.ToLower().Contains(LocationFilter.ToLower()))
                                    && (ZoneFilter == "" || c.Other2.ToLower().Contains(ZoneFilter.ToLower()))
                                    && (CostCenterFilter == "" || c.Other3.ToLower().Contains(CostCenterFilter.ToLower()))

                                );
            }
            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var isSortable_8 = Convert.ToBoolean(Request["bSortable_8"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_9"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.Section :
                sortColumnIndex == 6 && isSortable_6 ? c.Project :
                sortColumnIndex == 7 && isSortable_7 ? c.Other1 :
                sortColumnIndex == 8 && isSortable_8 ? c.Other2 :
                sortColumnIndex == 9 && isSortable_9 ? c.Other3 :

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
                  c.Id
                , c.Code
                , c.EmpName 
                , c.Designation
                , c.Department 
                , c.Section    
                , c.Project    
                , c.Other1
                , c.Other2
                , c.Other3
                //,c.Id
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
        
        public ActionResult Import(string returnUrl)
        {

            if (!string.IsNullOrEmpty(Session["mgs"] as string))
            {
                ViewBag.mgs = Request["mgs"];
                Session["mgs"] = "";
            }
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_18", "index").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/HRM/Home");
                }
                if (identity.IsESS)
                {
                    return Redirect("/hrm/employeeinfo/Edit/" + identity.EmployeeId);
                }


                return View();
            }
            catch (Exception)
            {
                return Redirect(new AppSettingsReader().GetValue("PAGENOTFOUND", typeof(string)).ToString());
            }
        }

        [Authorize]
        public ActionResult ExportExcell(ExportImportVM VM)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            DataTable dt = new DataTable();
            ExcelPackage excel = new ExcelPackage();


            try
            {

                ExportImportRepo _repo = new ExportImportRepo();

                switch (VM.Type)
                {
                    case "EmployeeInfo": dt = _repo.SelectEmployeeInfo(VM); break;
                    case "EmployeeLeaveStructure": dt = _repo.SelectEmpInfo(VM); break;
                    case "EmployeePersonalDetail": dt = _repo.SelectPersonalDetail(VM); break;
                    case "EmployeeJob": dt = _repo.SelectEmployeeJob(VM); break;
                    case "Asset": dt = _repo.SelectAsset(VM); break;
                    case "EmployeeEmergencyContact": dt = _repo.SelectEmployeeEmergencyContact(VM); break;
                    case "EmployeeEducation": dt = _repo.SelectEmployeeEducation(VM); break;
                    case "EmployeeProfessionalDegree": dt = _repo.SelectEmployeeProfessionalDegree(VM); break;
                    case "EmployeeLanguage": dt = _repo.SelectEmployeeLanguage(VM); break;
                    case "EmployeeExtraCurriculumActivities": dt = _repo.SelectEmployeeExtraCurriculumActivities(VM); break;
                    case "EmployeeImmigration": dt = _repo.SelectEmployeeImmigration(VM); break;
                    case "EmployeeTraining": dt = _repo.SelectEmployeeTraining(VM); break;
                    case "EmployeeTravel": dt = _repo.SelectEmployeeTravel(VM); break;
                    case "EmployeeNominee": dt = _repo.SelectEmployeeNominee(VM); break;
                    case "EmployeeDependent": dt = _repo.SelectEmployeeDependent(VM); break;
                    case "EmployeeLeftInformation": dt = _repo.SelectEmployeeLeftInformation(VM); break;
                    //case "EmployeeGroup": dt = _repo.SelectEmployeeGroup(VM); break;
                    case "Department": dt = _repo.SelectDepartment(VM); break;
                    case "Designation": dt = _repo.SelectDesignation(VM); break;
                    case "DesignationGroup": dt = _repo.SelectDesignationGroup(VM); break;
                    case "Bank": dt = _repo.SelectBank(VM); break;
                    case "Branch": dt = _repo.SelectBranch(VM); break;
                    case "EmployeePF": dt = _repo.SelectEmployeePF(VM); break;

                }
                #region Check Point

                SettingRepo _setDAL = new SettingRepo();
                string AutoCode = _setDAL.settingValue("AutoCode", "Employee").ToString() == "Y" ? "Y" : "N";
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                if (AutoCode == "Y" && VM.Type == "EmployeeInfo")
                {
                    if (dt.Columns.Contains("EmployeeCode"))
                    {
                        dt.Columns.Remove("EmployeeCode");
                    }
                }
                if (CompanyName.ToUpper() != "G4S")
                {
                    if (dt.Columns.Contains("EmployeeOtherId"))
                    {
                        dt.Columns.Remove("EmployeeOtherId");
                    }
                    if (dt.Columns.Contains("EmpJobType"))
                    {
                        dt.Columns.Remove("EmpJobType");
                    }
                    if (dt.Columns.Contains("EmpCategory"))
                    {
                        dt.Columns.Remove("EmpCategory");
                    }

                }
                #endregion
                #region Excel

                string filename = VM.Type + " Data";
                var workSheet = excel.Workbook.Worksheets.Add(VM.Type);
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

        public ActionResult ImportExcel(ExportImportVM VM)
        {
            string[] result = new string[6];
            try
            {
                ExportImportRepo _repo = new ExportImportRepo();
                SettingRepo _setDAL = new SettingRepo();

                EmployeeInfoVM vm = new EmployeeInfoVM();
                EmployeeJobVM Jobvm = new EmployeeJobVM();
                EmployeePersonalDetailVM PersonalDvm = new EmployeePersonalDetailVM();


                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                string AutoCode = _setDAL.settingValue("AutoCode", "Employee").ToString() == "Y" ? "Y" : "N";

                string tableName = "";
                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();

                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                #region file Copy i into Folder and read
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + VM.file.FileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                if (VM.file != null && VM.file.ContentLength > 0)
                {
                    VM.file.SaveAs(fullPath);
                }
                DataSet ds = new DataSet();
                //DataSet ds = Ordinary.UploadExcel(FullPath,fileName);
                System.Data.DataTable dt = new System.Data.DataTable();
                FileStream stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read);
                // We return the interface, so that
                IExcelDataReader reader = null;
                if (VM.file.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (VM.file.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();
                dt = ds.Tables[0];
                reader.Close();
                #endregion
                tableName = dt.TableName;
                if (tableName != VM.Type)
                {
                    result[0] = "Fail";
                    result[1] = "Select Profer Excel File for " + VM.Type + " Import";
                    result[2] = "";
                    result[3] = "";
                    result[4] = "ex";
                    result[5] = "ImportExcel"; //Method Name
                    // throw new ArgumentNullException("Select Profer Excel File for " + VM.Type + " Import", "");
                }
                if (dt.Rows.Count < 1)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Row in Excel file For Import";
                    result[2] = "";
                    result[3] = "";
                    result[4] = "ex";
                    result[5] = "ImportExcel"; //Method Name
                    throw new ArgumentNullException("No Data Row in Excel file For Import", "");
                }
                if (VM.Type == "EmployeeInfo")
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        vm = new EmployeeInfoVM();
                        Jobvm = new EmployeeJobVM();
                        PersonalDvm = new EmployeePersonalDetailVM();
                        vm.personalDetail = PersonalDvm;
                        vm.employeeJob = Jobvm;

                        vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                        vm.CreatedBy = identity.Name;
                        vm.CreatedFrom = identity.WorkStationIP;
                        vm.BranchId = Convert.ToInt32(identity.BranchId);
                        vm.IsActive = true;
                        if (AutoCode != "Y")
                        {
                            vm.Code = item["EmployeeCode"].ToString().Trim();
                        }
                        if (CompanyName.ToUpper() == "G4S")
                        {
                            vm.personalDetail.OtherId = item["EmployeeOtherId"].ToString().Trim();
                            vm.employeeJob.EmpJobType = item["EmpJobType"].ToString().Trim();
                            vm.employeeJob.EmpCategory = item["EmpCategory"].ToString().Trim();

                        }
                        vm.Salutation_E = item["Salutation_E"].ToString().Trim();
                        vm.MiddleName = item["MiddleName"].ToString().Trim();
                        vm.LastName = item["LastName"].ToString().Trim();
                        vm.Remarks = item["Remarks"].ToString().Trim();
                        result = new EmployeeInfoRepo().Insert(vm);
                    }
                }
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                switch (VM.Type)
                {
                    case "EmployeePersonalDetail": result = _repo.InsertPersonalDetail(dt); break;
                    case "EmployeeEmergencyContact": result = _repo.InsertEmployeeEmergencyContact(dt); break;
                    case "EmployeeJob": result = _repo.InsertEmployeeJob(dt); break;
                    case "EmployeeEducation": result = _repo.InsertEmployeeEducation(dt); break;
                    case "EmployeeLanguage": result = _repo.InsertEmployeeLanguage(dt); break;
                    case "EmployeeProfessionalDegree": result = _repo.InsertEmployeeProfessionalDegree(dt); break;
                    case "EmployeeExtraCurriculumActivities": result = _repo.InsertEmployeeExtraCurriculumActivities(dt); break;
                    //case "EmployeeExtraCurriculumActivities": dt = _repo.InsertEmployeeExtraCurriculumActivities(dt); break;
                    case "EmployeeImmigration": result = _repo.InsertEmployeeImmigration(dt); break;
                    case "EmployeeTraining": result = _repo.InsertEmployeeTraining(dt, vm); break;
                    case "EmployeeTravel": result = _repo.InsertEmployeeTravels(dt); break;
                    case "EmployeeNominee": result = _repo.InsertEmployeeNominee(dt); break;
                    case "EmployeeDependent": result = _repo.InsertEmployeeDependent(dt); break;
                    case "EmployeeAssets": result = _repo.InsertEmployeeAssets(dt); break;
                    case "EmployeeLeftInformation": result = _repo.InsertEmployeeLeftInformation(dt); break;
                    //case "EmployeeLeaveStructure": result = _repo.InsertEmployeeLeaveStructure(dt); break;
                    case "EmployeeLeaveStructure": result = _repo.InsertEmployeeLeaveStructure(dt); break;
                    case "Department": result = _repo.InsertDepartment(dt); break;
                    case "Designation": result = _repo.InsertDesignation(dt); break;
                    case "Bank": result = _repo.InsertBank(dt); break;
                    case "DesignationGroup": result = _repo.InsertDesignationGroup(dt); break;
                    case "EmployeePF": result = _repo.InsertEmployeePF(dt); break;

                }

                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Import");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Import");
            }

        }
    }
}
