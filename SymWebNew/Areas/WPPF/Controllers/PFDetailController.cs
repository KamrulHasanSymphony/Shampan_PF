using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.WPPF;
using SymViewModel.WPPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using SymViewModel.Common;
using SymReporting.WPPF;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Newtonsoft.Json;
using SymWebUI.Areas.WPPF.Models;
using SymRepository.Payroll;
using System.Web;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class PFDetailController : Controller
    {
        public PFDetailController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }
        //
        // GET: /WPPF/PFDetail/
        PFDetailRepo _repo = new PFDetailRepo();
        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        /// <summary>
        /// Created: 01 Mar 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves Fiscal Period Header data (Id, FiscalYearId) and processes WPPF details for the specified Fiscal Period.
        /// </summary>
        /// <param name="id">Fiscal Period Header ID</param>
        /// <returns>View containing Fiscal Period details</returns>
        public ActionResult Index(int id)
        {
            try
            {
                // Set the ID to ViewBag for potential use in the view
                ViewBag.id = id;

                // Initialize the repository to interact with the database
                PFDetailRepo _repo = new PFDetailRepo();
                string[] conditionFields = { "pfd.Id" };
                string[] conditionValues = { id.ToString() };

                // Fetch the Fiscal Period Header data
                var pfh = _repo.SelectFiscalPeriodHeader(conditionFields, conditionValues).FirstOrDefault(); // Call PFDetailRepo and PFDetailDAL().SelectFiscalPeriodHeader. Id as parameter.

                // Set PeriodName for the view if data is found
                ViewBag.PeriodName = pfh.FiscalPeriod + " (" + pfh.ProjectName + " )";

                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        /// <summary>
        /// Created: 01 Mar 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves Fiscal Period Header data (Id, FiscalYearId) and processes WPPF details for the specified Fiscal Period.
        /// </summary>
        /// <param name="id">Fiscal Period Header ID</param>
        /// <returns>View containing Fiscal Period details</returns>
        public ActionResult _index(JQueryDataTableParamModel param, int id)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);

            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var designationFilter = Convert.ToString(Request["sSearch_3"]);
            var departmentFilter = Convert.ToString(Request["sSearch_4"]);
            var EmployeePFValueFilter = Convert.ToString(Request["sSearch_5"]);
            var EmployeerPFValueFilter = Convert.ToString(Request["sSearch_6"]);
            var EmployeePFValueFrom = 0;
            var EmployeePFValueTo = 0;

            if (EmployeePFValueFilter.Contains('~'))
            {
                EmployeePFValueFrom = EmployeePFValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(EmployeePFValueFilter.Split('~')[0]) == true ? Convert.ToInt32(EmployeePFValueFilter.Split('~')[0]) : 0;
                EmployeePFValueTo = EmployeePFValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(EmployeePFValueFilter.Split('~')[1]) == true ? Convert.ToInt32(EmployeePFValueFilter.Split('~')[1]) : 0;
            }
            var EmployeerPFValueFrom = 0;
            var EmployeerPFValueTo = 0;
            if (EmployeerPFValueFilter.Contains('~'))
            {
                EmployeerPFValueFrom = EmployeerPFValueFilter.Split('~')[0] == "" ? 0 : Ordinary.IsInteger(EmployeerPFValueFilter.Split('~')[0]) == true ? Convert.ToInt32(EmployeerPFValueFilter.Split('~')[0]) : 0;
                EmployeerPFValueTo = EmployeerPFValueFilter.Split('~')[1] == "" ? 0 : Ordinary.IsInteger(EmployeerPFValueFilter.Split('~')[1]) == true ? Convert.ToInt32(EmployeerPFValueFilter.Split('~')[1]) : 0;
            }

            #endregion Column Search

            PFDetailRepo _repo = new PFDetailRepo();
            List<PFDetailVM> getAllData = new List<PFDetailVM>();
            IEnumerable<PFDetailVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string[] conditionFields = { "pfd.PFHeaderId", "ve.BranchId" };
            string[] conditionValues = { id.ToString(), Session["BranchId"].ToString() };

            getAllData = _repo.SelectEmployeeList(conditionFields, conditionValues);         // Its call PFDetailRepo and PFDetailDAL().SelectEmployeeList. PFHeaderId as parameter. 

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
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.BasicSalary.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.EmployeePFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.EmployeerPFValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            if (codeFilter != "" || empNameFilter != "" || designationFilter != "" || departmentFilter != "" || (EmployeePFValueFilter != "" && EmployeePFValueFilter != "~") || (EmployeerPFValueFilter != "" && EmployeerPFValueFilter != "~"))
            {
                filteredData = filteredData
                                .Where(c =>
                                    (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
                                    && (empNameFilter == "" || c.EmpName.ToLower().Contains(empNameFilter.ToLower()))
                                    && (designationFilter == "" || c.Designation.ToString().ToLower().Contains(designationFilter.ToLower()))
                                    && (departmentFilter == "" || c.Department.ToLower().Contains(departmentFilter.ToLower()))
                                    && (EmployeePFValueFrom == 0 || EmployeePFValueFrom <= Convert.ToInt32(c.EmployeePFValue))
                                    && (EmployeePFValueTo == 0 || EmployeePFValueTo >= Convert.ToInt32(c.EmployeePFValue))
                                    && (EmployeerPFValueFrom == 0 || EmployeerPFValueFrom <= Convert.ToInt32(c.EmployeerPFValue))
                                    && (EmployeerPFValueTo == 0 || EmployeerPFValueTo >= Convert.ToInt32(c.EmployeerPFValue))
                                );
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFDetailVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation :
                sortColumnIndex == 4 && isSortable_4 ? c.Department :
                sortColumnIndex == 5 && isSortable_5 ? c.BasicSalary.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.EmployeePFValue.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.EmployeerPFValue.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = displayedCompanies.Select(c => new[]
            {
                //Convert.ToString(c.Id)
                "", c.Code, c.EmpName, c.Designation, c.Department, c.BasicSalary.ToString(), c.EmployeePFValue.ToString(),
                c.EmployeerPFValue.ToString(),
            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult IndexFiscalPeriod(string EmployeeId = "", string fydid = "")
        {
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.fydid = fydid;

            return View();
        }
        /// <summary>
        /// Created: 01 Mar 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves Fiscal Period Header data (Id, FiscalYearId) and processes WPPF details for the specified Fiscal Period.
        /// </summary>
        /// <param name="id">Fiscal Period Header ID</param>
        /// <param name="EmployeeId"> EmployeeId</param>
        /// <returns>View containing Fiscal Period details</returns>
        public ActionResult _indexFiscalPeriod(JQueryDataTableParamModel param, string EmployeeId = "", string fydid = "")
        {

            PFDetailRepo _repo = new PFDetailRepo();
            List<PFHeaderVM> getAllData = new List<PFHeaderVM>();
            IEnumerable<PFHeaderVM> filteredData;
            ShampanIdentity Identit = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            string[] conditionFields = { "pfd.EmployeeId", "pfd.FiscalYearDetailId", "pfd.BranchId" };
            string[] conditionValues = { EmployeeId, fydid, Session["BranchId"].ToString() };

            getAllData = _repo.SelectFiscalPeriodHeader(conditionFields, conditionValues);      

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
                filteredData = getAllData
                    .Where(c =>
                          isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable2 && c.ProjectName.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable3 && c.FiscalPeriod.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.TotalEmployeeValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.TotalEmployerValue.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.TotalPF.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var isSortable_7 = Convert.ToBoolean(Request["bSortable_7"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<PFHeaderVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.ProjectName :
                sortColumnIndex == 3 && isSortable_3 ? c.PeriodStart :
                sortColumnIndex == 4 && isSortable_4 ? c.TotalEmployeeValue.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.TotalEmployerValue.ToString() :
                sortColumnIndex == 6 && isSortable_6 ? c.TotalPF.ToString() :
                sortColumnIndex == 7 && isSortable_7 ? c.Post.ToString() :
                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                 c.Id.ToString()
                , c.Code
                , c.ProjectName
                , c.FiscalPeriod
                ,c.TotalEmployeeValue.ToString()
                ,c.TotalEmployerValue.ToString()
                ,c.TotalPF.ToString()
                , c.Post?"Yes":"No"
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
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the processing of WPPF details by interacting with the PFDetailRepo.
        /// </summary>
        /// <param name="fydid">Fiscal Year Detail ID</param>
        /// <param name="ProjectId">Project ID</param>
        /// <param name="chkAll">Flag to check if all records should be processed</param>
        /// <returns>JSON result indicating the outcome of the process</returns>
        public ActionResult PFProcess(string fydid = "", string ProjectId = "", string chkAll = "")
        {
            // Validate that the fiscal year detail ID is provided
            if (string.IsNullOrWhiteSpace(fydid))
            {
                return View(); // Return the view if fydid is missing
            }

            // Initialize result and message variables
            string[] result = new string[6];
            string mgs = "";

            // Create a new instance of ShampanIdentityVM and populate it with metadata
            ShampanIdentityVM vm = new ShampanIdentityVM
            {
                CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CreatedBy = identity.Name,
                CreatedFrom = identity.WorkStationIP
            };
            string TransType = AreaTypePFVM.TransType;
            // Initialize the PFDetailRepo and call the PFProcess method
            PFDetailRepo _repo = new PFDetailRepo();
            result = _repo.PFProcess(fydid, ProjectId, chkAll, vm);

            // Concatenate the result for messaging and store it in the session
            mgs = result[0] + "~" + result[1];
            Session["result"] = mgs;

            // Return the result as a JSON response
            return Json(mgs, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the posting of WPPF Header data, retrieves permission, and returns the result in JSON format.
        /// </summary>
        /// <param name="ids">A string containing the IDs used to fetch WPPF Header details</param>
        /// <returns>JSON result with the status message from the post operation</returns>
        public JsonResult Post(string ids)
        {
            try
            {
                // Fetch user permission for editing the WPPF Header based on role and permissions
                Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

                // Check if the user has permission; if not, handle appropriately
                if (Session["permission"].ToString() == "False")
                {
                    return Json("Permission Denied", JsonRequestBehavior.AllowGet);
                }

                // Initialize the PFHeaderVM and set the Id from the provided string (split by ~)
                PFHeaderVM headerVm = new PFHeaderVM
                {
                    Id = Convert.ToInt32(ids.Split('~')[0])
                };

                // Prepare the result variable to hold the outcome of the post operation
                string[] result = _repo.PostHeader(headerVm);

                // Return the second result element (status message) as the response
                return Json(result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json("Error: {ex.Message}", JsonRequestBehavior.AllowGet);
            }
        }
        
        [Authorize(Roles = "Admin")]
        public JsonResult GetPFDetail(string fydid)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "edit").ToString();

            PFDetailVM vm = new PFDetailVM();

            string[] cFields = { "pfd.FiscalYearDetailId" };
            string[] cValues = { fydid };

            // Select all pf data by fiscal year
            vm = _repo.SelectAll(0, cFields, cValues).FirstOrDefault();

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Report()
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10010", "report").ToString();
            return View();
        }
        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a Crystal Report PDF for WPPF Detail, filtered by the provided criteria.
        /// </summary>
        /// <param name="fydid">Fiscal Year Detail ID used for filtering the report</param>
        /// <param name="empCodeFrom">Employee code range start for filtering the report</param>
        /// <param name="empCodeTo">Employee code range end for filtering the report</param>
        /// <param name="groupBy">Grouping criteria for the report (if any)</param>
        /// <returns>PDF file containing the WPPF Detail report</returns>
        public ActionResult ReportView(string fydid = "", string empCodeFrom = "", string empCodeTo = "", string groupBy = "")
        {
            try
            {
                // Initialize report variables
                string reportHead = "There are no data to Preview for WPPF Detail";
                string rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPFDetail.rpt";
                ReportDocument doc = new ReportDocument();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();

                // Retrieve company information for report header
                CompanyRepo companyRepo = new CompanyRepo();
                CompanyVM company = companyRepo.SelectAll().FirstOrDefault();

                // Initialize repository for PFDetail report generation
                PFDetailRepo pfRepo = new PFDetailRepo();

                // Set report filtering conditions
                string[] conditionFields = { "ve.Code>", "ve.Code<", "pfd.FiscalYearDetailId" };
                string[] conditionValues = { empCodeFrom, empCodeTo, fydid };

                // Fetch the report data based on the conditions
                table = pfRepo.Report(new PFDetailVM()); // Call PFDetailRepo and PFDetailDAL().Report method

                // Update report header based on data availability
                if (table.Rows.Count > 0)
                {
                    reportHead = "WPPF Detail List";
                }

                // Add data to the DataSet for the report
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtPFDetail";

                // Load the Crystal Report template and set data source
                doc.Load(rptLocation);
                doc.SetDataSource(ds);

                // Set formula fields (company information, transaction type, etc.)
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'{AreaTypePFVM.TransType}'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'{company.Address}'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'{company.Name}'";

                // Render and return the PDF report
                var rpt = RenderReportAsPDF(doc);
                doc.Close();

                return rpt;
            }
            catch (Exception ex)
            {
                // Handle any exceptions with an informative error message
                throw new Exception("An error occurred while generating the WPPF Detail report.", ex);
            }
        }
        
        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a Crystal Report PDF detailing WPPF Contributions (Bank Deposit GL Transactions) 
        /// for a specific WPPF Header ID. Fetches employee-specific data and renders the report.
        /// </summary>
        /// <param name="id">PFHeaderId used to filter the contribution data for specific WPPF Header</param>
        /// <returns>PDF file containing the WPPF contribution details</returns>
        public ActionResult PFContributionDetailsReport(int id)
        {
            try
            {
                // Initialize report variables
                string reportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                string rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPFContributionDetails.rpt";
                ReportDocument doc = new ReportDocument();
                DataTable dt;

                // Set condition for report data using PFHeaderId
                string[] conditionFields = { "pfd.PFHeaderId", "ve.BranchId" };
                string[] conditionValues = { id.ToString(), Session["BranchId"].ToString() };

                // Retrieve data using the repository method (fetches list of employees)
                var result = _repo.SelectEmployeeList(conditionFields, conditionValues); // Calls PFDetailRepo and PFDetailDAL().SelectEmployeeList
                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(result));

                // Update report header if data exists
                if (dt.Rows.Count > 0)
                {
                    reportHead = "Bank Deposit GL Transactions";
                }

                // Set the DataTable's table name for reporting
                dt.TableName = "dtPFDetail";

                // Load the Crystal Report template
                doc.Load(rptLocation);
                doc.SetDataSource(dt);

                // Load company logo and details for report
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                CompanyVM cvm = new CompanyRepo().SelectAll().FirstOrDefault();

                // Set formula fields (company information, report header, transaction type)
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'{companyLogo}'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'{reportHead}'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'{AreaTypePFVM.TransType}'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'{cvm.Address}'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'{cvm.Name}'";

                // Render and return PDF report
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                throw new Exception("An error occurred while generating the WPPF Contribution Details Report.", ex);
            }
        }
        
        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a Crystal Report PDF summarizing WPPF Employer Provisions (Bank Deposit GL Transactions).
        /// Retrieves contribution data for the given PFHeaderId, applies formatting, and renders the report.
        /// </summary>
        /// <param name="id">PFHeaderId used to filter employer provision data</param>
        /// <returns>PDF file containing the employer WPPF contribution summary</returns>
        [HttpGet]  ///Here's a version that mentions the method type more clearly.
        public ActionResult PFContributionSummarysReport(int id)
        {
            try
            {
                // Initialize variables
                string reportHead = "There are no data to Preview for GL Transaction for Bank Deposit";
                string rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\PFEmployeeContributionSummary.rpt";
                ReportDocument doc = new ReportDocument();
                DataTable dt;

                // Set condition for report data
                string[] conditionFields = { "ts.PFHeaderId", "ph.BranchId" };
                string[] conditionValues = { id.ToString(), Session["BranchId"].ToString() };

                // Retrieve data using repo (calls PFDetailDAL().PFEmployersProvisionsReport internally)
                dt = _repo.PFEmployersProvisionsReport(conditionFields, conditionValues);

                if (dt.Rows.Count > 0)
                {
                    reportHead = "Bank Deposit GL Transactions";
                }

                dt.TableName = "EmployersProvisions";

                // Load and bind data to Crystal Report
                doc.Load(rptLocation);
                doc.SetDataSource(dt);

                // Load company logo and details
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                CompanyVM cvm = new CompanyRepo().SelectAll().FirstOrDefault();

                // Set formula fields for the report
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'{companyLogo}'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'{reportHead}'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'{cvm.Address}'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'{cvm.Name}'";

                // Render and return PDF
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        ///  Created: 01 Mar 2025 
        /// Created By : Md Torekul Islam
        /// Downloads the WPPF (Provident Fund) employee contribution details as an Excel file
        /// for the specified PFHeader ID.
        /// I retrieve data from the database, design it for excel, and convert the datatable to excel and Download as excel file.
        /// </summary>
        /// <param name="id">The PFHeader ID used to filter employee contribution data.</param>
        /// <returns>Redirects to the WPPF detail index after Excel download.</returns>
        [HttpGet]  ///Here's a version that mentions the method type more clearly.
        public ActionResult PFContributionDetailsDownload(int id)
        {
            try
            {

                // Initialize the Crystal Reports document and a DataTable
                ReportDocument doc = new ReportDocument();
                System.Data.DataTable dt = new System.Data.DataTable();

                // Define filter conditions to retrieve WPPF contribution details
                string[] conditionFields = { "pfd.PFHeaderId", "ve.BranchId" };
                string[] conditionValues = { id.ToString(), Session["BranchId"].ToString() };

                // Fetch data using repository method (PFDetailRepo and PFDetailDAL)
                var Result = _repo.SelectEmployeeList(conditionFields, conditionValues);

                // Convert the result to a DataTable
                dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Result));

                // Convert specific columns from string to decimal
                string[] DecimalColumnNames = { "BasicSalary", "EmployeePFValue", "EmployeerPFValue" };
                dt = Ordinary.DtColumnStringToDecimal(dt, DecimalColumnNames);

                // Filter and reorder the required columns
                var dataView = new DataView(dt);
                dt = dataView.ToTable(true, "Code", "EmpName", "Designation", "Department",
                     "BasicSalary", "EmployeePFValue", "EmployeerPFValue");

                #region Excel Generation

                string StatementName = "WPPF Employee Contribution Details";
                string filename = StatementName + " -" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

                // Create Excel package using EPPlus
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add(StatementName);

                // Get company info for Excel header
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);

                string Line1 = comInfo.Name;
                string Line2 = comInfo.Address;

                // Prepare report headers
                string[] ReportHeaders = new string[] { Line1, Line2, StatementName };

                // Format and load the DataTable into Excel worksheet
                ExcelSheetFormat(dt, workSheet, ReportHeaders);

                #region Excel File Download

                // Write the Excel file to the HTTP response stream
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End(); // Ends the response
                }

                #endregion

                return Redirect("WPPF/PFDetail/index");

                #endregion
            }
            catch (Exception)
            {
                // Rethrow exception (consider logging in real scenarios)
                throw;
            }
        }
        
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Downloads the WPPF (Provident Fund) summary report as an Excel file  
        /// based on the selected fiscal year detail ID, report type, and project ID.  
        /// </summary>
        /// <param name="fydid">The fiscal year detail ID and Project ID combined as a tilde (~) separated string.</param>
        /// <param name="rType">The report type to determine the format or view of the WPPF summary.</param>
        /// <param name="ProjectId">The project identifier to filter WPPF summary data (overridden by parsed fydid).</param>
        /// <returns>Redirects to the WPPF detail fiscal period index after Excel file is downloaded.</returns>
        [HttpGet]
        public ActionResult PFReportSummaryDetail(string fydid, string rType, string ProjectId)
        {
            try
            {
                // Initialize the ViewModel and Report objects
                PFReportVM vm = new PFReportVM();
                PFReport report = new PFReport();

                // Split the fydid input to extract actual fiscal year detail ID and project ID
                string[] parts = fydid.Split('~');
                fydid = parts[0];
                ProjectId = parts.Length > 1 ? parts[1] : null;

                // Retrieve the WPPF summary report data based on parameters
                vm = report.PFReportSummaryDetail(fydid, rType, ProjectId);

                #region Excel Generation

                string filename = "WPPF Contribution " + rType;

                // Create a new Excel package and worksheet using EPPlus
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("WPPF Contribution " + rType);

                // Retrieve company information to include in the Excel header
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);

                string Line1 = comInfo.Name;
                string Line2 = comInfo.Address;
                string Line3 = "";

                // Define Excel headers
                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                // Format and load the DataTable into Excel worksheet
                ExcelSheetFormat(vm.DataTable, workSheet, ReportHeaders);

                #region Excel File Download

                // Write the Excel file to the HTTP response stream
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End(); // Ends the response to complete file download
                }

                #endregion

                return Redirect("WPPF/PFDetail/IndexFiscalPeriod");

                #endregion
            }
            catch (Exception)
            {
                // Consider logging the exception details here for debugging
                throw;
            }
        }
        
        private void ExcelSheetFormat(DataTable dt, ExcelWorksheet workSheet, string[] ReportHeaders)
        {


            int TableHeadRow = 0;
            TableHeadRow = ReportHeaders.Length + 2;

            int RowCount = 0;
            RowCount = dt.Rows.Count;

            int ColumnCount = 0;
            ColumnCount = dt.Columns.Count;

            int GrandTotalRow = 0;
            GrandTotalRow = TableHeadRow + RowCount + 1;

            int InWordsRow = 0;
            InWordsRow = GrandTotalRow + 1;

            int SignatureSpaceRow = 0;
            SignatureSpaceRow = InWordsRow + 1;

            int SignatureRow = 0;
            SignatureRow = InWordsRow + 4;
            workSheet.Cells[TableHeadRow, 1].LoadFromDataTable(dt, true);
            #region Format

            var format = new OfficeOpenXml.ExcelTextFormat();
            format.Delimiter = '~';
            format.TextQualifier = '"';
            format.DataTypes = new[] { eDataTypes.String };



            for (int i = 0; i < ReportHeaders.Length; i++)
            {
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Merge = true;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.Font.Size = 16 - i;
                workSheet.Cells[i + 1, 1].LoadFromText(ReportHeaders[i], format);

            }
            int colNumber = 0;

            foreach (DataColumn col in dt.Columns)
            {
                colNumber++;
                if (col.DataType == typeof(DateTime))
                {
                    workSheet.Column(colNumber).Style.Numberformat.Format = "dd-MMM-yyyy hh:mm:ss AM/PM";
                }
                else if (col.DataType == typeof(Decimal))
                {

                    workSheet.Column(colNumber).Style.Numberformat.Format = "#,##0.00_);[Red](#,##0.00)";

                    #region Grand Total
                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" + workSheet.Cells[TableHeadRow + 1, colNumber].Address + ":" + workSheet.Cells[(TableHeadRow + RowCount), colNumber].Address + ")";
                    #endregion
                }

            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells["A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");
            #endregion
        }

        [HttpGet]
        public ActionResult reportVeiw(string id)
        {
            try
            {

                PFReportVM vm = new PFReportVM();
                PFDetailVM PFDetailvm = new PFDetailVM();
                PFReport report = new PFReport();
                string[] cFields = { "pfd.PFHeaderId" };
                string[] cValues = { id };
                PFDetailvm = _repo.SelectAll(0, cFields, cValues).FirstOrDefault();
                vm.PFHeaderId = Convert.ToInt32(PFDetailvm.PFHeaderId);
                vm.Code = PFDetailvm.Code;
                return PartialView("reportVeiw", vm);

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Generates and returns a Crystal Report PDF for Provident Fund Contribution based on selected filters.
        /// Loads data, sets company info and logos, and renders the report as a downloadable PDF.
        /// </summary>
        /// <param name="vm">PFReportVM containing filter values like PFHeaderId.</param>
        /// <returns>PDF file containing the WPPF Contribution report.</returns>
        public ActionResult PFContributionReport(PFReportVM vm)
        {
            try
            {
                // Initialize values
                string reportHead = "There are no data to Preview for GL Transaction for Contribution";
                string rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\WPPF\\rptPFContribution.rpt";
                ReportDocument doc = new ReportDocument();
                DataTable dt;

                // Prepare filter fields and values
                string[] cFields = { "PFHeaderId" };
                string[] cValues = { vm.PFHeaderId.ToString() == "0" ? "" : vm.PFHeaderId.ToString() };

                // Get data for the report
                PFReport report = new PFReport();
                dt = new PFReportRepo().PFContributionReport(vm, cFields, cValues);

                if (dt.Rows.Count > 0)
                {
                    reportHead = "Contribution GL Transactions";
                }

                dt.TableName = "dtPFContribution";

                // Load and bind data to Crystal Report
                doc.Load(rptLocation);
                doc.SetDataSource(dt);

                // Load company info and set formula fields
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                CompanyVM cvm = new CompanyRepo().SelectAll().FirstOrDefault();

                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + reportHead + "'";
                doc.DataDefinition.FormulaFields["TransType"].Text = "'" + AreaTypePFVM.TransType + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";

                // Render and return the report as PDF
                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public ActionResult ImportExportPF()
        {
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            return View();
        }

        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Downloads Provident Fund (WPPF) details as an Excel file based on filtering parameters  
        /// like Project, Department, Section, Designation, Employee Code range, and Fiscal Period.
        /// </summary>
        /// <param name="ProjectId">Filter by project ID.</param>
        /// <param name="DepartmentId">Filter by department ID.</param>
        /// <param name="SectionId">Filter by section ID.</param>
        /// <param name="DesignationId">Filter by designation ID.</param>
        /// <param name="CodeF">Start of employee code range (From).</param>
        /// <param name="CodeT">End of employee code range (To).</param>
        /// <param name="fid">Fiscal Period ID.</param>
        /// <param name="ETId">Employee Type ID (optional).</param>
        /// <param name="Orderby">Column name for ordering the result set.</param>
        /// <returns>Redirects to ImportExportPF view after downloading the Excel file.</returns>
        [HttpGet]
        public ActionResult DownloadPFDetailsExcel(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, int fid = 0, int ETId = 0, string Orderby = null)
        {
            // Initialize dependencies and variables
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            PFDetailRepo _repo = new PFDetailRepo();
            DataTable dt = new DataTable();
            string[] result = new string[6];

            try
            {
                // Check if user has permission to add (Role ID "1_38")
                var permission = _reposur.SymRoleSession(identity.UserId, "1_38", "add").ToString();
                Session["permission"] = permission;

                if (permission == "False")
                {
                    return Redirect("/WPPF/Home");
                }

                // Define file information and location
                string FileName = "DownloadPFContribution.xls";
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                string contentType = MimeMapping.GetMimeMapping(fullPath);
                string BranchId = Session["BranchId"].ToString();
                // Delete existing file if already exists
                if (System.IO.File.Exists(fullPath + FileName))
                {
                    System.IO.File.Delete(fullPath + FileName);
                }

                // Fetch WPPF contribution data from repository
                dt = _repo.ExportExcelFilePF(fullPath, FileName, ProjectId, DepartmentId, SectionId, DesignationId, CodeF, CodeT, fid, Orderby, BranchId);

                // Create Excel package using EPPlus
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Contribution");

                // Load datatable into Excel worksheet
                workSheet.Cells[1, 1].LoadFromDataTable(dt, true);

                // Define filename based on fiscal period
                string filename = "WPPF of" + "-" + dt.Rows[0]["Fiscal Period"].ToString();

                // Write the Excel file to the response stream for download
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                // Set success status in session
                result[0] = "Successfull";
                result[1] = "Successful~Data Download";
                Session["result"] = result[0] + "~" + result[1];

                return RedirectToAction("ImportExportPF");
            }
            catch (Exception)
            {
                // Handle and log exception, then redirect
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine +
                    result[2].ToString() + Environment.NewLine +
                    result[5].ToString(),
                    this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());

                return RedirectToAction("ImportExportPF");
            }
        }

        /// <summary>
        /// Created: 10 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Imports Provident Fund (WPPF) detail data from an uploaded Excel file and inserts it into the system.
        /// Validates user permission, saves the file to server, and processes it through repository logic.
        /// </summary>
        /// <param name="file">The uploaded Excel file containing WPPF details.</param>
        /// <param name="FYDId">Fiscal Year Detail ID.</param>
        /// <param name="PId">Project ID.</param>
        /// <returns>Redirects to ImportExportPF view after import operation.</returns>
        [HttpPost]
        public ActionResult ImportPFDetailExcel(HttpPostedFileBase file, int FYDId = 0, string PId = "")
        {
            // Initialize required repositories
            SymUserRoleRepo _reposur = new SymUserRoleRepo();
            PFDetailRepo _repo = new PFDetailRepo();

            // Prepare result message container
            string[] result = new string[6];

            try
            {
                // Check user permission to add (Role ID: 1_31)
                var permission = _reposur.SymRoleSession(identity.UserId, "1_31", "add").ToString();
                Session["permission"] = permission;

                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                // Define file storage path
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + file.FileName;

                // Delete file if it already exists
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Save uploaded file to server
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(fullPath);
                }

                // Set audit metadata for import
                ShampanIdentityVM vm = new ShampanIdentityVM
                {
                    LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    LastUpdateBy = identity.Name,
                    LastUpdateFrom = identity.WorkStationIP,
                    CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CreatedBy = identity.Name,
                    CreatedFrom = identity.WorkStationIP,
                    BranchId = Session["BranchId"].ToString()
                };

                // Call repository method to import the Excel data
                result = _repo.ImportExcelFile(fullPath, file.FileName, vm, FYDId, PId);

                // Store result in session and redirect
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("ImportExportPF");

                // Optionally redirect to: return RedirectToAction("OpeningBalance");
            }
            catch (Exception)
            {
                // On failure, log result and redirect to import page
                Session["result"] = result[0] + "~" + result[1];

                // Optional: Log exception details
                // FileLogger.Log(result[0] + Environment.NewLine + result[2] + Environment.NewLine + result[5], this.GetType().Name, result[4] + Environment.NewLine + result[3]);

                return RedirectToAction("ImportExportPF");
            }
        }

        public ActionResult InsertAutoJournal(string TransactionMonth, string TransactionForm, string TransactionCode)
        {
            string[] result = new string[6];
            string BranchId = Session["BranchId"].ToString();

            ShampanIdentityVM vm = new ShampanIdentityVM
            {
                LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                LastUpdateBy = identity.Name,
                LastUpdateFrom = identity.WorkStationIP,
                CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss"),
                CreatedBy = identity.Name,
                CreatedFrom = identity.WorkStationIP,
                BranchId = Session["BranchId"].ToString()
            };

            result = _repo.InsertAutoJournal(TransactionMonth, TransactionForm, TransactionCode, BranchId, vm);
            Session["result"] = result[0] + "~" + result[1];

            return View("~/Areas/WPPF/Views/PFDetail/IndexFiscalPeriod.cshtml");
        }

    }
}
