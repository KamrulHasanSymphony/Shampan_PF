using CrystalDecisions.CrystalReports.Engine;
using JQueryDataTables.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.Enum;

using SymRepository.Loan;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymViewModel.HRM;
using SymViewModel.Loan;
//using SymWebUI.Areas.Payroll.Reports.PayrollEntry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SymReporting.BWWF;
using Newtonsoft.Json;
using SymWebUI.Areas.BWWF.Models;

using SymServices.Common;

namespace SymWebUI.Areas.BWWF.Controllers
{
    public class LoanController : Controller
    {
        //
        // GET: /HRM/Loan/
				SymUserRoleRepo _reposur = new SymUserRoleRepo();
				ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index(string id)
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/Payroll/Home");
               }
           
            if (!(identity.IsAdmin || identity.IsPayroll))
            {
                id = identity.EmployeeId;
            }
            EmployeeInfoRepo _infoRepo = new EmployeeInfoRepo();
            EmployeeInfoVM empVm = _infoRepo.SelectById(id);
            empVm.Id = id;
        
            return View(empVm);
        }
       
        public ActionResult AllLoan()
        {
               var permission= _reposur.SymRoleSession(identity.UserId, "1_51", "index").ToString();
               Session["permission"] = permission;
               if (permission=="False")
               {
                   return Redirect("/BWWF/Home");
               }
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all BWWF Loan information.
        /// </summary>      
        /// <returns>View containing BWWF Loan</returns>
        public ActionResult _index(JQueryDataTableParamModel param, string code, string name, string employeeId)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLoanRepo _loanRepo = new EmployeeLoanRepo();
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var loanTypeFilter = Convert.ToString(Request["sSearch_5"]);
            var totalAmountFilter = Convert.ToString(Request["sSearch_6"]);
            var startDateFilter = Convert.ToString(Request["sSearch_7"]);
            var PrincipalAmountFilter = Convert.ToString(Request["sSearch_8"]);
            var InterestAmountFilter = Convert.ToString(Request["sSearch_9"]);          
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (startDateFilter.Contains('~'))
            {
                //Split date range filters with ~
                fromDate = startDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Convert.ToDateTime(startDateFilter.Split('~')[0]);
                toDate = startDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Convert.ToDateTime(startDateFilter.Split('~')[1]);
            }
            var amountFrom = 0;
            var amountTo = 0;
            if (totalAmountFilter.Contains('~'))
            {
                amountFrom = totalAmountFilter.Split('~')[0] == "" ? 0 : Convert.ToInt32(totalAmountFilter.Split('~')[0]);
                amountTo = totalAmountFilter.Split('~')[1] == "" ? 0 : Convert.ToInt32(totalAmountFilter.Split('~')[1]);
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
            var getAllData = _loanRepo.SelectAll(Convert.ToInt32(Session["BranchId"].ToString()), employeeId);

            IEnumerable<EmployeeLoanVM> filteredData;
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
                       || isSearchable3 && c.Department.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable4 && c.Designation.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable5 && c.LoanType.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable6 && c.StartDate.ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable7 && c.PrincipalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable8 && c.InterestAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       || isSearchable9 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredData = getAllData;
            }
            #endregion Search and Filter Data
            #region Column Filtering
            if (codeFilter != "" || empNameFilter != "" || departmentFilter != "" || designationFilter != "" || loanTypeFilter != "" || (totalAmountFilter != "" && totalAmountFilter != "~") || (startDateFilter != "" && startDateFilter != "~"))
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
                                    (loanTypeFilter == "" || c.LoanType.ToLower().Contains(loanTypeFilter.ToLower()))
                                    &&
                                    (fromDate == DateTime.MinValue || fromDate <= Convert.ToDateTime(c.StartDate))
                                    &&
                                    (toDate == DateTime.MaxValue || toDate >= Convert.ToDateTime(c.StartDate))
                                    &&
                                    (amountFrom == 0 || amountFrom <= Convert.ToInt32(c.TotalAmount))
                                    &&
                                    (amountTo == 0 || amountTo >= Convert.ToInt32(c.TotalAmount))
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
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeLoanVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? c.LoanType :
                sortColumnIndex == 6 && isSortable_6 ? Ordinary.DateToString(c.StartDate) :
                sortColumnIndex == 7 && isSortable_7 ? c.PrincipalAmount.ToString() :
                sortColumnIndex == 8 && isSortable_8 ? c.InterestAmount.ToString() :
                sortColumnIndex == 9 && isSortable_9 ? c.TotalAmount.ToString() :
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
                , c.EmpName 
                , c.Department 
                , c.Designation , c.LoanType// + "~" + Convert.ToString(c.Id)
                , c.PrincipalAmount.ToString()
                , c.InterestAmount.ToString()
                , c.TotalAmount.ToString()
                , c.StartDate
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
        [HttpGet]
        public ActionResult Create(string employeeId)
        {          
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }              
            EmployeeLoanVM vm = new EmployeeLoanVM();
            EmployeeVM emp = new EmployeeInfoRepo().EmployeeInfo(employeeId);
            vm.EmployeeId = employeeId;
            vm.Employee = emp.FullName;
            vm.Code = emp.Code;
            vm.Project = emp.Project;
            vm.Section = emp.Section;
            vm.Department = emp.Department;
            vm.Designation = emp.Designation;
            return View(vm);
        }

        /// <summary>
        /// Creates a new employee loan record and calculates loan details based on the provided parameters. 
        /// The method handles both "Reduce" and other interest policies, computes the necessary amounts for each installment, 
        /// and stores the loan information into the database.
        /// </summary>
        /// <param name="vm">The view model containing employee loan information, including loan principal, interest rate, number of installments, etc.</param>
        /// <returns>
        /// Redirects to the "AllLoan" action on success or returns the same view with error message if something goes wrong.
        /// </returns>
        /// <remarks>
        /// This method calculates loan installments for the employee using different interest policies. 
        /// It uses the "Reduce" method for calculating principal and interest amounts, while for other policies, it divides the total loan 
        /// and interest amounts equally across all installments.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is any error while processing the loan creation or inserting into the database.</exception>
        [HttpPost]
        public ActionResult Create(EmployeeLoanVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                SettingRepo sRepo = new SettingRepo();
                bool FromSetting = Convert.ToBoolean(sRepo.settingValue("PFLoanRate", "FromSetting") == "Y" ? true : false);
                int Upto12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "Upto12Month"));              
                int GetterThen12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "GetterThen12Month"));

                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId.ToString();
                vm.BranchId = Session["BranchId"].ToString();
                vm.IsHold = false;
                decimal cPAmount = vm.PrincipalAmount;
                decimal dPAmount = 0;
                decimal dIAmount = 0;
                decimal dTAmount = 0;
                int iLoop = 1;

                var fractionValue = "0";
            
                List<MonthCalculation> monthCalculations = Ordinary.MonthCalculation(Ordinary.DateToString(vm.StartDate), vm.NumberOfInstallment);
                List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
                EmployeeLoanDetailVM loanDetail;

                var amount = vm.PrincipalAmount / vm.NumberOfInstallment;
                var actualValue = amount.ToString().Split('.')[0];

                if (amount == Math.Round(amount))
                {
                    fractionValue = "0";
                }
                else
                {
                    fractionValue = amount.ToString().Split('.')[1];
                }
                var totalCount = vm.NumberOfInstallment - 1;


                #region  Reduce policy
                double annualInterestRate = Convert.ToDouble(vm.InterestRate / 100); // Annual interest rate (10%)
                int loanTermMonths = vm.NumberOfInstallment; // Loan term in months
                // Calculate monthly interest rate

                double monthlyInterestRate = annualInterestRate / 12;

                double PrincipleAmount = 0;
                PrincipleAmount = Convert.ToDouble(cPAmount);

                // Calculate installment amount manually
                double installmentValue = (PrincipleAmount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, loanTermMonths)) / (Math.Pow(1 + monthlyInterestRate, loanTermMonths) - 1);
                #endregion

                for (int i = 0; i < monthCalculations.Count; i++)
                {
                    int rowNumber = i + 1;

                    if (vm.InterestPolicy == "Reduce")
                    {
                        
                        double NewPrincipleAmount = Math.Round(PrincipleAmount);

                        double InterestAmount = Math.Round(NewPrincipleAmount * annualInterestRate / 12);
                        double PrinciipleWithInterestAmount = Math.Round(PrincipleAmount + InterestAmount);
                        double Deposit = Math.Round(installmentValue);
                        if (i == totalCount)
                        {
                            Deposit = Math.Round(PrinciipleWithInterestAmount);

                        }
                        double InterestPart = Math.Round(InterestAmount);
                        double PrinciplePart = Math.Round(Deposit - InterestPart);


                        PrincipleAmount = Math.Round(PrinciipleWithInterestAmount - installmentValue);

                        dPAmount = Convert.ToDecimal(PrinciplePart);
                        dIAmount = Convert.ToDecimal(InterestPart);
                        dTAmount = Convert.ToDecimal(Deposit);

                    }
                    else
                    {
                        dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                        dTAmount = dPAmount + dIAmount;
                    }

                    if (vm.InterestPolicy != "Reduce")
                    {
                        if (totalCount == i)
                        {
                            dPAmount = vm.PrincipalAmount - (Convert.ToInt32(actualValue) * (vm.NumberOfInstallment - 1));

                            dIAmount = Convert.ToDecimal(dIAmount);
                            var val = dIAmount.ToString().Split('.')[0];

                            dIAmount = Convert.ToInt32(vm.InterestAmount) - (Convert.ToInt32(val) * (vm.NumberOfInstallment - 1));
                        }
                        else
                        {
                            dPAmount = Math.Floor(dPAmount);
                            dIAmount = Math.Floor(dIAmount);
                        }
                    }
                    dTAmount = dPAmount + dIAmount;

                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.PrincipalAmount = dPAmount;
                    loanDetail.InterestAmount = dIAmount;
                    loanDetail.InstallmentAmount = dTAmount;
                    loanDetail.InstallmentPaidAmount = 0;
                    loanDetail.PaymentScheduleDate = monthCalculations[i].StartDate;
                    loanDetail.PaymentDate = "";
                    loanDetail.IsHold = false;
                    loanDetail.IsPaid = false;
                    loanDetail.Remarks = "";
                    loanDetail.InstallmentSLNo = rowNumber;
                    loanDetails.Add(loanDetail);
                    vm.EndDate = monthCalculations[i].StartDate;
                }
                
                vm.employeeLoanDetails = loanDetails;

                result = new EmployeeLoanRepo().EmployeeLoanInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index", new { id = vm .EmployeeId});
                return RedirectToAction("AllLoan");
            }
            catch (Exception ex)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            return View(vm);
        }

        public JsonResult MonthCalculation(string date, int number)
        {
            return Json(Ordinary.MonthCalculation(Ordinary.DateToString(date), number), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult EmployeeInfoForLoan()
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "add").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View("_Employee");
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Employee Information for BWWF Loan.
        /// </summary>      
        /// <returns>View containing Employee Information for BWWF Loan</returns>
        [Authorize]
        public ActionResult _EmployeeInfoForLoan(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var codeFilter = Convert.ToString(Request["sSearch_1"]);
            var empNameFilter = Convert.ToString(Request["sSearch_2"]);
            var departmentFilter = Convert.ToString(Request["sSearch_3"]);
            var designationFilter = Convert.ToString(Request["sSearch_4"]);
            var joinDateFilter = Convert.ToString(Request["sSearch_5"]);
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MaxValue;
            if (joinDateFilter.Contains('~'))
            {
                fromDate = joinDateFilter.Split('~')[0] == "" ? DateTime.MinValue : Ordinary.IsDate(joinDateFilter.Split('~')[0]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[0]) : DateTime.MinValue;
                toDate = joinDateFilter.Split('~')[1] == "" ? DateTime.MaxValue : Ordinary.IsDate(joinDateFilter.Split('~')[1]) == true ? Convert.ToDateTime(joinDateFilter.Split('~')[1]) : DateTime.MinValue;
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
            EmployeeInfoRepo _empRepo = new EmployeeInfoRepo();
            var getAllData = _empRepo.SelectAll(Session["BranchId"].ToString());
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
                                .Where(c => (codeFilter == "" || c.Code.ToLower().Contains(codeFilter.ToLower()))
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
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EmployeeInfoVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.Code :
                sortColumnIndex == 2 && isSortable_2 ? c.EmpName :
                sortColumnIndex == 3 && isSortable_3 ? c.Department :
                sortColumnIndex == 4 && isSortable_4 ? c.Designation :
                sortColumnIndex == 5 && isSortable_5 ? Ordinary.DateToString(c.JoinDate) :
                sortColumnIndex == 6 && isSortable_6 ? c.Remarks :
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
                , c.Code //+ "~" + Convert.ToString(c.Id) 
                , c.EmpName 
                , c.Department 
                , c.Designation 
                , c.JoinDate.ToString()      
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
        public ActionResult Edit(string loanId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            vm = loanRepo.SelectLoan(loanId);

            foreach (var item in vm.employeeLoanDetails.Take(1))
            {
                vm.TotalDuePrincipalAmount = item.TotalDuePrincipalAmount;
                vm.TotalDueInterestAmount = item.TotalDueInterestAmount;
                vm.SettlementAmount = item.SettlementAmount;
                vm.NoofInstallment = item.NoofInstallment;
            }
            return View(vm);
        }

        /// <summary>
        /// Edits an existing employee loan record or saves a new loan record based on the provided view model.
        /// This method handles both "Save" and "Update" operations for employee loans. It calculates the loan details 
        /// for each installment based on the specified interest policy and updates the loan record in the database.
        /// </summary>
        /// <param name="vm">The view model containing the updated employee loan information, including loan principal, interest rate, number of installments, etc.</param>
        /// <param name="Save">Indicates whether the operation is "Save" or "Update".</param>
        /// <returns>
        /// Redirects to the "AllLoan" action after successfully saving or updating the loan, or returns an error message in the session if the operation fails.
        /// </returns>
        /// <remarks>
        /// The method supports two operations: 
        /// 1. **Save**: A new loan record is created with the calculated installment details based on the interest policy ("Reduce" or others).
        /// 2. **Update**: An existing loan record is updated with the new data, and its installments are recalculated as needed.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is any error while saving or updating the loan record.</exception>

        [HttpPost]
        public ActionResult Edit(EmployeeLoanVM vm, string Save)
        {
            Save = vm.Operation;
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (Save == "Save")
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
                vm.IsHold = false;
                decimal cPAmount = vm.PrincipalAmount;
                decimal dPAmount = 0;
                decimal dIAmount = 0;
                decimal dTAmount = 0;
                List<MonthCalculation> monthCalculations = Ordinary.MonthCalculation(Ordinary.DateToString(vm.StartDate), vm.NumberOfInstallment);
                List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
                EmployeeLoanDetailVM loanDetail;

                var amount = vm.PrincipalAmount / vm.NumberOfInstallment;
                var actualValue = amount.ToString().Split('.')[0];
                var fractionValue = amount.ToString().Split('.')[1];
                var totalCount = vm.NumberOfInstallment - 1;
                #region  Reduce policy
                double annualInterestRate = Convert.ToDouble(vm.InterestRate / 100); // Annual interest rate (10%)
                int loanTermMonths = 36; // Loan term in months
                // Calculate monthly interest rate

                double monthlyInterestRate = annualInterestRate / 12;

                double PrincipleAmount = 0;
                PrincipleAmount = Convert.ToDouble(cPAmount);

                // Calculate installment amount manually
                double installmentValue = (PrincipleAmount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, loanTermMonths)) / (Math.Pow(1 + monthlyInterestRate, loanTermMonths) - 1);
                #endregion

                for (int i = 0; i < monthCalculations.Count; i++)
                {
                    if (vm.InterestPolicy == "Reduce")
                    {                      
                        double NewPrincipleAmount = Math.Round(PrincipleAmount);

                        double InterestAmount = Math.Round(NewPrincipleAmount * annualInterestRate / 12);
                        double PrinciipleWithInterestAmount = Math.Round(PrincipleAmount + InterestAmount);
                        double Deposit = Math.Round(installmentValue);
                        if (i == totalCount)
                        {
                            Deposit = Math.Round(PrinciipleWithInterestAmount);

                        }
                        double InterestPart = Math.Round(InterestAmount);
                        double PrinciplePart = Math.Round(Deposit - InterestPart);


                        PrincipleAmount = Math.Round(PrinciipleWithInterestAmount - installmentValue);

                        dPAmount = Convert.ToDecimal(PrinciplePart);
                        dIAmount = Convert.ToDecimal(InterestPart);
                        dTAmount = Convert.ToDecimal(Deposit);

                    }
                    else
                    {
                        dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                        dTAmount = dPAmount + dIAmount;
                    }
                    if (vm.InterestPolicy != "Reduce")
                    {
                        if (totalCount == i)
                        {
                            dPAmount = vm.PrincipalAmount - (Convert.ToInt32(actualValue) * (vm.NumberOfInstallment - 1));

                            dIAmount = Convert.ToDecimal(dIAmount);
                            var val = dIAmount.ToString().Split('.')[0];

                            dIAmount = Convert.ToInt32(vm.InterestAmount) - (Convert.ToInt32(val) * (vm.NumberOfInstallment - 1));

                        }
                        else
                        {
                            dPAmount = Math.Floor(dPAmount);
                            dIAmount = Math.Floor(dIAmount);
                        }
                    }

                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.PrincipalAmount = dPAmount;
                    loanDetail.InterestAmount = dIAmount;
                    loanDetail.InstallmentAmount = dTAmount;
                    loanDetail.InstallmentPaidAmount = 0;
                    loanDetail.PaymentScheduleDate = monthCalculations[i].StartDate;
                    loanDetail.PaymentDate = "";
                    loanDetail.IsHold = false;
                    loanDetail.IsPaid = false;
                    loanDetail.Remarks = "";
                    loanDetails.Add(loanDetail);
                    vm.EndDate = monthCalculations[i].StartDate;
                }

                vm.employeeLoanDetails = loanDetails;
                result = new EmployeeLoanRepo().EmployeeLoanInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("AllLoan");
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
                EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
                result = loanRepo.EmployeeLoanUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                //var mgs = result[0] + "~" + result[1];
                return RedirectToAction("AllLoan");
                //return Json(mgs, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult PaidEdit(string Id, string emploanId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanDetailVM vm = new EmployeeLoanDetailVM();
            vm = loanRepo.SelectEmployeeLoan(Id, emploanId);
            return PartialView("PaidEdit", vm);
        }

        /// <summary>
        /// Updates the payment status of an existing employee loan record.
        /// This method is used to modify the payment details of an employee loan, such as marking installments as paid.
        /// It updates the loan's payment details in the database and redirects to the "Edit" view for the given loan.
        /// </summary>
        /// <param name="vm">The view model containing the employee loan information, including the loan ID and payment details to be updated.</param>
        /// <returns>
        /// Redirects to the "Edit" view of the loan after successfully updating the payment details, or redirects with an error message if the operation fails.
        /// </returns>
        /// <remarks>
        /// The method performs the following actions:
        /// 1. Updates the loan payment details (e.g., marking installments as paid) based on the provided `vm` (view model).
        /// 2. Updates the metadata fields like `LastUpdateAt`, `LastUpdateBy`, and `LastUpdateFrom` with the current date and user information.
        /// 3. If the update is successful, it redirects to the loan's edit view. If there is an error, an error message is logged, and the user is redirected to the edit view with a failure message.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is any error while updating the payment status of the loan.</exception>

        [HttpPost]
        public ActionResult PaidEdit(EmployeeLoanDetailVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
                EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
                result = loanRepo.PaidEdit(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index", new { id = vm .EmployeeId});
                return RedirectToAction("Edit", new { loanId = vm.EmployeeLoanId });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Edit", new { loanId = vm.EmployeeLoanId });
            }
        }

        public ActionResult employeeLoanDetails(EmployeeLoanDetailVM vm)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            return View(vm);
        }

        /// <summary>
        /// Displays the details of a specific employee loan based on the provided loan ID.
        /// This method checks if the current user has permission to view/edit the loan details. If the user lacks permission, they are redirected to the home page.
        /// </summary>
        /// <param name="loanId">The unique identifier of the employee loan whose details are to be displayed.</param>
        /// <returns>
        /// Returns the view containing the details of the specified employee loan. If the user does not have permission, they are redirected to the home page.
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Checks the current user's permission to view/edit the loan details.
        /// 2. If the permission is "False", the user is redirected to the home page.
        /// 3. If the permission is valid, it retrieves the loan details for the given `loanId` and passes the data to the view.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is an error while retrieving the loan details from the repository.</exception>

        [HttpGet]
        public ActionResult Details(string loanId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            vm = loanRepo.SelectLoan(loanId);
            return View(vm);
        }

        /// <summary>
        /// Updates the loan details for a specific employee based on the provided parameters.
        /// This method checks if the current user has permission to edit the loan details. If the user lacks permission, they are redirected to the home page.
        /// </summary>
        /// <param name="loanDetailsId">The unique identifier of the employee loan detail that is being updated.</param>
        /// <param name="employeeId">The unique identifier of the employee whose loan details are being updated.</param>
        /// <param name="loanId">The unique identifier of the employee loan.</param>
        /// <param name="ishold">A flag indicating whether the loan is held.</param>
        /// <param name="HaveDuplicate">A flag indicating whether the loan has duplicate details.</param>
        /// <param name="remarks">Additional remarks regarding the loan update.</param>
        /// <returns>
        /// A JSON response containing the result of the loan detail update operation.
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Checks the current user's permission to edit the loan details.
        /// 2. If the permission is "False", the user is redirected to the home page.
        /// 3. Updates the loan details based on the provided parameters and the current user's identity.
        /// 4. Calls the `EmployeeLoanUpdate2` method in the repository to save the updates.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is an error while updating the loan details in the repository.</exception>

        public ActionResult LoanUpdate(string loanDetailsId, string employeeId, string loanId, bool ishold, bool HaveDuplicate, string remarks)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLoanDetailVM loanDetail = new EmployeeLoanDetailVM();
            loanDetail.EmployeeLoanId = loanId;
            loanDetail.Id = Convert.ToInt32(loanDetailsId);
            loanDetail.EmployeeId = employeeId;
            loanDetail.IsHold = ishold;
            loanDetail.Remarks = remarks;
            loanDetail.HaveDuplicate = HaveDuplicate;
            loanDetail.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            loanDetail.LastUpdateBy = identity.Name;
            loanDetail.LastUpdateFrom = identity.WorkStationIP;
            //if (ishold==true)
            //     loanDetail.triger ="hold";
            if (HaveDuplicate == true)
                loanDetail.triger = "duplicate";
            else
                loanDetail.triger = "hold";
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            string[] result = new string[6];
            result = loanRepo.EmployeeLoanUpdate2(loanDetail);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the loan details for a specific employee based on the provided parameters.
        /// This method checks if the current user has permission to edit the loan details. If the user lacks permission, they are redirected to the home page.
        /// </summary>
        /// <param name="loanDetailsId">The unique identifier of the employee loan detail that is being updated.</param>
        /// <param name="employeeId">The unique identifier of the employee whose loan details are being updated.</param>
        /// <param name="loanId">The unique identifier of the employee loan.</param>
        /// <param name="ishold">A flag indicating whether the loan is held.</param>
        /// <param name="HaveDuplicate">A flag indicating whether the loan has duplicate details.</param>
        /// <param name="remarks">Additional remarks regarding the loan update.</param>
        /// <returns>
        /// A JSON response containing the result of the loan detail update operation.
        /// </returns>
        /// <remarks>
        /// The method performs the following steps:
        /// 1. Checks the current user's permission to edit the loan details.
        /// 2. If the permission is "False", the user is redirected to the home page.
        /// 3. Updates the loan details based on the provided parameters and the current user's identity.
        /// 4. Calls the `EmployeeLoanUpdate2` method in the repository to save the updates.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is an error while updating the loan details in the repository.</exception>

        public ActionResult LoanUpdateDetail(string loanDetailsId, decimal principalAmount, decimal interestAmount, decimal installmentAmount, string remarks)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            EmployeeLoanDetailVM loanDetail = new EmployeeLoanDetailVM();
            loanDetail.Id = Convert.ToInt32(loanDetailsId);
            loanDetail.InterestAmount = interestAmount;
            loanDetail.PrincipalAmount = principalAmount;
            loanDetail.InstallmentAmount = installmentAmount;
            loanDetail.Remarks = remarks;
            loanDetail.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            loanDetail.LastUpdateBy = identity.Name;
            loanDetail.LastUpdateFrom = identity.WorkStationIP;
            loanDetail.triger = "update";
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            string[] result = new string[6];
            result = loanRepo.EmployeeLoanUpdate2(loanDetail);
            return Json(result, JsonRequestBehavior.AllowGet);
        }      
     
        [HttpGet]
        public ActionResult UpdateSettelment(string loanId, decimal TotalDuePrincipalAmount, decimal TotalDueInterestAmount, string EarlySellteDate)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            string[] result = new string[6];
            result = loanRepo.UpdateSettelment(loanId, TotalDuePrincipalAmount, TotalDueInterestAmount, EarlySellteDate);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ApproveSettelment(string loanId, string EarlySellteDate)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            string[] result = new string[6];
            result = loanRepo.ApprovedSettelment(loanId, EarlySellteDate);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the settlement of an employee loan by retrieving the loan details for settlement
        /// based on the provided loan ID and checking user permissions.
        /// </summary>
        /// <param name="loanId">The loan ID used to fetch the loan details for settlement.</param>
        /// <returns>Returns the settlement view with the loan details if authorized; otherwise, redirects to the Payroll Home.</returns>
        [HttpGet]
        public ActionResult Settlement(string loanId)
        {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_51", "edit").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            vm = loanRepo.SelectLoanForSettelment(loanId);

            foreach (var item in vm.employeeLoanDetails.Take(1))
            {
                vm.TotalDuePrincipalAmount = item.TotalDuePrincipalAmount;
                vm.TotalDueInterestAmount = item.TotalDueInterestAmount;
                vm.SettlementAmount = item.SettlementAmount;
                vm.NoofInstallment = item.NoofInstallment;
            }
            return View(vm);
        }

        /// <summary>
        /// Created: 13 Apr 2025  
        /// Created By: Md Torekul Islam  
        /// Handles the loan settlement process, either saving a new loan settlement or updating an existing one,
        /// based on the provided loan data.
        /// </summary>
        /// <param name="vm">The view model containing the loan settlement data.</param>
        /// <param name="Save">A string value indicating whether the action is to "Save" (create) or update an existing loan settlement.</param>
        /// <returns>Redirects to the "AllLoan" view with a result message after the operation is completed.</returns>
        [HttpPost]
        public ActionResult Settlement(EmployeeLoanVM vm, string Save)
        {
            Save = vm.Operation;
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (Save == "Save")
            {
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = identity.Name;
                vm.CreatedFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
                vm.IsHold = false;
                decimal cPAmount = vm.PrincipalAmount;
                decimal dPAmount = 0;
                decimal dIAmount = 0;
                decimal dTAmount = 0;
                List<MonthCalculation> monthCalculations = Ordinary.MonthCalculation(Ordinary.DateToString(vm.StartDate), vm.NumberOfInstallment);
                List<EmployeeLoanDetailVM> loanDetails = new List<EmployeeLoanDetailVM>();
                EmployeeLoanDetailVM loanDetail;

                var amount = vm.PrincipalAmount / vm.NumberOfInstallment;
                var actualValue = amount.ToString().Split('.')[0];
                var fractionValue = amount.ToString().Split('.')[1];
                var totalCount = vm.NumberOfInstallment - 1;
                #region  Reduce policy
                double annualInterestRate = Convert.ToDouble(vm.InterestRate / 100); // Annual interest rate (10%)
                int loanTermMonths = 36; // Loan term in months
                // Calculate monthly interest rate

                double monthlyInterestRate = annualInterestRate / 12;

                double PrincipleAmount = 0;
                PrincipleAmount = Convert.ToDouble(cPAmount);

                // Calculate installment amount manually
                double installmentValue = (PrincipleAmount * monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, loanTermMonths)) / (Math.Pow(1 + monthlyInterestRate, loanTermMonths) - 1);
                #endregion

                for (int i = 0; i < monthCalculations.Count; i++)
                {
                    if (vm.InterestPolicy == "Reduce")
                    {
                       
                        double NewPrincipleAmount = Math.Round(PrincipleAmount);

                        double InterestAmount = Math.Round(NewPrincipleAmount * annualInterestRate / 12);
                        double PrinciipleWithInterestAmount = Math.Round(PrincipleAmount + InterestAmount);
                        double Deposit = Math.Round(installmentValue);
                        if (i == totalCount)
                        {
                            Deposit = Math.Round(PrinciipleWithInterestAmount);

                        }
                        double InterestPart = Math.Round(InterestAmount);
                        double PrinciplePart = Math.Round(Deposit - InterestPart);


                        PrincipleAmount = Math.Round(PrinciipleWithInterestAmount - installmentValue);

                        dPAmount = Convert.ToDecimal(PrinciplePart);
                        dIAmount = Convert.ToDecimal(InterestPart);
                        dTAmount = Convert.ToDecimal(Deposit);

                    }
                    else
                    {
                        dPAmount = vm.PrincipalAmount / vm.NumberOfInstallment;
                        dIAmount = vm.InterestAmount / vm.NumberOfInstallment;
                        dTAmount = dPAmount + dIAmount;
                    }
                    if (vm.InterestPolicy != "Reduce")
                    {
                        if (totalCount == i)
                        {
                            dPAmount = vm.PrincipalAmount - (Convert.ToInt32(actualValue) * (vm.NumberOfInstallment - 1));

                            dIAmount = Convert.ToDecimal(dIAmount);
                            var val = dIAmount.ToString().Split('.')[0];

                            dIAmount = Convert.ToInt32(vm.InterestAmount) - (Convert.ToInt32(val) * (vm.NumberOfInstallment - 1));

                        }
                        else
                        {
                            dPAmount = Math.Floor(dPAmount);
                            dIAmount = Math.Floor(dIAmount);
                        }
                    }

                    loanDetail = new EmployeeLoanDetailVM();
                    loanDetail.PrincipalAmount = dPAmount;
                    loanDetail.InterestAmount = dIAmount;
                    loanDetail.InstallmentAmount = dTAmount;
                    loanDetail.InstallmentPaidAmount = 0;
                    loanDetail.PaymentScheduleDate = monthCalculations[i].StartDate;
                    loanDetail.PaymentDate = "";
                    loanDetail.IsHold = false;
                    loanDetail.IsPaid = false;
                    loanDetail.Remarks = "";
                    loanDetails.Add(loanDetail);
                    vm.EndDate = monthCalculations[i].StartDate;
                }

                vm.employeeLoanDetails = loanDetails;
                result = new EmployeeLoanRepo().EmployeeLoanInsert(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("AllLoan");
            }
            else
            {
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = identity.BranchId;
                EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
                result = loanRepo.EmployeeLoanUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                //var mgs = result[0] + "~" + result[1];
                return RedirectToAction("AllLoan");
                //return Json(mgs, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Generates and downloads a PDF report for the loan schedule of members based on 
        /// various filter parameters such as project, department, section, designation, and date range.
        /// </summary>
        /// <param name="ProjectId">The ID of the project to filter the loan data.</param>
        /// <param name="DepartmentId">The ID of the department to filter the loan data.</param>
        /// <param name="SectionId">The ID of the section to filter the loan data.</param>
        /// <param name="DesignationId">The ID of the designation to filter the loan data.</param>
        /// <param name="CodeF">The code from which to filter the loan data.</param>
        /// <param name="CodeT">The code to which to filter the loan data.</param>
        /// <param name="view">Specifies whether to view the report or download it.</param>
        /// <param name="FromDate">The start date to filter loan data.</param>
        /// <param name="ToDate">The end date to filter loan data.</param>
        /// <returns>A PDF report of the loan schedule for members.</returns>
        public ActionResult ScheduleofLoanMember(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string view,string FromDate,string ToDate)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                string BranchId = Session["BranchId"].ToString(); 
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string vFromDate = "";
                string vToDate = "";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                if (FromDate != "0_0" && FromDate != "0" && FromDate != "" && FromDate != "null" && FromDate != null)
                {
                    DateTime date = DateTime.ParseExact(FromDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    string formattedDate = date.ToString("yyyyMMdd");
                    vFromDate = formattedDate;
                  
                }
                if (ToDate != "0_0" && ToDate != "0" && ToDate != "" && ToDate != "null" && ToDate != null)
                {
                    DateTime date = DateTime.ParseExact(ToDate, "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    string formattedDate = date.ToString("yyyyMMdd");
                    vToDate = formattedDate;                   
                }
                ReportDocument doc = new ReportDocument();
                EmployeeLoanRepo _repo = new EmployeeLoanRepo();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();
                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                List<EmployeeLoanDetailVM> getAllData = new List<EmployeeLoanDetailVM>();
                //var BranchId = Convert.ToInt32(identity.BranchId);
                //var getAllData = _repo.SelectAll(BranchId);
                string rptLocation = "";
                string ReportHead = "";
                getAllData = _repo.SelectAllForReport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, BranchId);
                ReportHead = "There are no data to Preview for Loan Statement";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Loan Statement";
                }
                table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\rptLoanStatement.rpt";

                getAllData = _repo.SelectAllForSLMPeport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, vFromDate, vToDate, BranchId);
                ReportHead = "There are no data to Preview for Schedule Loan Member";
                if (getAllData.Count > 0)
                {
                    if(FromDate!="")
                    {
                        ReportHead = "Schedule of Loan Member (Date of " + FromDate + " to " + ToDate + ")";
                    }
                    else
                    {
                        ReportHead = "Schedule of Loan Member";
                    }                    
                }
                table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\rptScheduleofLoanMember.rpt";

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
                //doc = new rptLoanStatement();
                //doc.SetDataSource(ds);
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
        /// Generates an Employee Loan Report based on the provided parameters and renders it as a PDF.
        /// </summary>
        /// <param name="ProjectId">The project ID used to filter the loan data.</param>
        /// <param name="DepartmentId">The department ID used to filter the loan data.</param>
        /// <param name="SectionId">The section ID used to filter the loan data.</param>
        /// <param name="DesignationId">The designation ID used to filter the loan data.</param>
        /// <param name="CodeF">The code filter starting value.</param>
        /// <param name="CodeT">The code filter ending value.</param>
        /// <param name="view">If the view is set to "Y" or is empty, the method renders the view, otherwise it generates the report.</param>
        /// <returns>A PDF report of employee loans or a view.</returns>
        public ActionResult EmployeeLoanReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string view)
        {
            try
            {
            var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
            Session["permission"] = permission;
            string BranchId = Session["BranchId"].ToString();
            if (permission == "False")
            {
                return Redirect("/Payroll/Home");
            }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }               
                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;
                }
                ReportDocument doc = new ReportDocument();
                EmployeeLoanRepo _repo = new EmployeeLoanRepo();
                DataTable table = new DataTable();
                DataSet ds = new DataSet();

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                List<EmployeeLoanDetailVM> getAllData = new List<EmployeeLoanDetailVM>();              
                string rptLocation = "";
                string ReportHead = "";
                getAllData = _repo.SelectAllForReport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT, BranchId);
                ReportHead = "There are no data to Preview for Loan Statement";
                if (getAllData.Count > 0)
                {
                    ReportHead = "Loan Statement";
                }
                table = new DataTable();
                table = Ordinary.ListToDataTable(getAllData.ToList());
                ds = new DataSet();
                ds.Tables.Add(table);
                ds.Tables[0].TableName = "dtEmpLoan";
                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Payroll\PayrollEntry\rptLoanStatement.rpt";
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.Load(rptLocation);
                doc.SetDataSource(ds);
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                //doc = new rptLoanStatement();
                //doc.SetDataSource(ds);
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
        /// Generates a loan report (Summary, Individual, or Statement) based on filtering parameters for project, department, section, designation, and date range.
        /// </summary>
        /// <param name="ProjectId">The project ID to filter data.</param>
        /// <param name="DepartmentId">The department ID to filter data.</param>
        /// <param name="SectionId">The section ID to filter data.</param>
        /// <param name="DesignationId">The designation ID to filter data.</param>
        /// <param name="CodeF">The starting code for filtering.</param>
        /// <param name="CodeT">The ending code for filtering.</param>
        /// <param name="view">If "Y", the report will be displayed; otherwise, it will be generated.</param>
        /// <param name="Rtype">The type of report to generate (Summary, Individual, or Statement).</param>
        /// <param name="DateFrom">The start date for filtering data.</param>
        /// <param name="DateTo">The end date for filtering data.</param>
        /// <returns>A PDF report based on the specified filters and report type.</returns>
        public ActionResult EmployeeLoanStatementReport(string ProjectId, string DepartmentId, string SectionId, string DesignationId, string CodeF, string CodeT, string view, string Rtype, string DateFrom, string DateTo)
        {
            try
            {
                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;

                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }
                //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrWhiteSpace(view) || view == "Y")
                {
                    return View();
                }
                string rptLocation = "";
                string ReportHead = "";

                string vProjectId = "0_0";
                string vDepartmentId = "0_0";
                string vSectionId = "0_0";
                string vDesignationId = "0_0";
                string vCodeF = "0_0";
                string vCodeT = "0_0";
                string projectParam = "[All]";
                string deptParam = "[All]";
                string secParam = "[All]";
                string desigParam = "[All]";
                string codeFParam = "[All]";
                string codeTParam = "[All]";
                if (ProjectId != "0_0" && ProjectId != "0" && ProjectId != "" && ProjectId != "null" && ProjectId != null)
                {
                    vProjectId = ProjectId;
                    ProjectRepo pRepo = new ProjectRepo();
                    projectParam = pRepo.SelectById(ProjectId).Name;
                }
                if (DepartmentId != "0_0" && DepartmentId != "0" && DepartmentId != "" && DepartmentId != "null" && DepartmentId != null)
                {
                    vDepartmentId = DepartmentId;
                    DepartmentRepo dRepo = new DepartmentRepo();
                    deptParam = dRepo.SelectById(DepartmentId).Name;
                }
                if (SectionId != "0_0" && SectionId != "0" && SectionId != "" && SectionId != "null" && SectionId != null)
                {
                    vSectionId = SectionId;
                    SectionRepo sRepo = new SectionRepo();
                    secParam = sRepo.SelectById(SectionId).Name;
                }
                if (DesignationId != "0_0" && DesignationId != "0" && DesignationId != "" && DesignationId != "null" && DesignationId != null)
                {
                    vDesignationId = DesignationId;
                    DesignationRepo desRepo = new DesignationRepo();
                    desigParam = desRepo.SelectById(DesignationId).Name;
                }
                if (CodeF != "0_0" && CodeF != "0" && CodeF != "" && CodeF != "null" && CodeF != null)
                {
                    vCodeF = CodeF;
                    codeFParam = vCodeF;
                }
                if (CodeT != "0_0" && CodeT != "0" && CodeT != "" && CodeT != "null" && CodeT != null)
                {
                    vCodeT = CodeT;
                    codeTParam = vCodeT;                   
                }

                ReportDocument doc = new ReportDocument();
                EmployeeLoanRepo _repo = new EmployeeLoanRepo();
                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                List<EmployeeLoanDetailVM> getAllData = new List<EmployeeLoanDetailVM>();
                string companyLogo = "";
                string BranchId = Session["BranchId"].ToString();

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                if (Rtype == "Summary")
                {
                    string FromDate = (Convert.ToDateTime(DateFrom.ToString())).ToString("yyyyMMdd");
                    string ToDate = (Convert.ToDateTime(DateTo.ToString())).ToString("yyyyMMdd");


                    string[] cFields = { "PaymentScheduleDate>", "PaymentScheduleDate<", "BranchId" };
                    string[] cValues = { FromDate, ToDate, codeFParam, codeTParam, BranchId };

                    dt = _repo.GetSummeryLoanData(cFields, cValues);

                    ReportHead = "There are no data to Preview for Loan Summary";
                    if (dt.Rows.Count > 0)
                    {
                        DateTime startdatte = DateTime.Now;
                        DateTime enddatte = DateTime.Now;
                        if (DateFrom != "" && DateTo != "")
                        {
                            startdatte = Convert.ToDateTime(DateFrom);
                            enddatte = Convert.ToDateTime(DateTo);
                        }
                        ReportHead = "Loan Summary Report (" + startdatte.ToString("yyyy") + "-" + enddatte.ToString("yy") + ")";                       
                    }
                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\rptAllLoanSummary.rpt";

                    doc.Load(rptLocation);
                    doc.SetDataSource(dt);
                    companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                    //doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
                    
                }
                if (Rtype == "Individual")
                {
                    string FromDate = (Convert.ToDateTime(DateFrom.ToString())).ToString("yyyyMMdd");
                    string ToDate = (Convert.ToDateTime(DateTo.ToString())).ToString("yyyyMMdd");

                    string[] cFields = { "PaymentScheduleDate>", "PaymentScheduleDate<", "BranchId" };
                    string[] cValues = { FromDate, ToDate, codeFParam, codeTParam, BranchId };
                    
                    dt = _repo.GetIndividualLoanData(cFields, cValues);

                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\rptLoanEGCB_Individual.rpt";
                    ReportHead = "There are no data to Preview for Loan Individual";
                    if (dt.Rows.Count > 0)
                    {
                        DateTime startdatte = DateTime.Now;
                        DateTime enddatte = DateTime.Now;
                        if (DateFrom != "" && DateTo != "")
                        {
                            startdatte = Convert.ToDateTime(DateFrom);
                            enddatte = Convert.ToDateTime(DateTo);
                        }
                        ReportHead = "Loan Individual Report (" + startdatte.ToString("yyyy") + "-" + enddatte.ToString("yy") + ")";
                    }
                    doc.Load(rptLocation);
                    doc.SetDataSource(dt);
                    companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                    doc.DataDefinition.FormulaFields["PrincipalPaid"].Text = dt.Rows[0]["PrincipalPaid"].ToString();
                    doc.DataDefinition.FormulaFields["InterestPaid"].Text = dt.Rows[0]["InterestPaid"].ToString();
                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                    //doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
                                     
                }
                if (Rtype == "Statement")
                {
                    getAllData = _repo.SelectLoanStatementForReport(vProjectId, vDepartmentId, vSectionId, vDesignationId, vCodeF, vCodeT);
                    dt = Ordinary.ListToDataTable(getAllData.ToList());

                    rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\rptAllLoanStatement.rpt";
                    ReportHead = "There are no data to Preview for Loan Statement";
                    if (getAllData.Count > 0)
                    {
                        DateTime startdatte = DateTime.Now;
                        DateTime enddatte = DateTime.Now; 
                        if(DateFrom!="" && DateTo !="")
                        {
                            startdatte = Convert.ToDateTime(DateFrom);
                            enddatte = Convert.ToDateTime(DateTo);
                        }                       
                        ReportHead = "Loan Statement Report ("+startdatte.ToString("yyyy")+"-"+enddatte.ToString("yy")+")";
                    }

                    ds = new DataSet();
                    ds.Tables.Add(dt);
                    ds.Tables[0].TableName = "dtEmpLoan";
                    companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                    doc.Load(rptLocation);
                    doc.SetDataSource(ds);
                    doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                    doc.DataDefinition.FormulaFields["ReportHead"].Text = "'" + ReportHead + "'";
                    doc.DataDefinition.FormulaFields["projectParam"].Text = "'" + projectParam + "'";
                    doc.DataDefinition.FormulaFields["deptParam"].Text = "'" + deptParam + "'";
                    doc.DataDefinition.FormulaFields["secParam"].Text = "'" + secParam + "'";
                    doc.DataDefinition.FormulaFields["desigParam"].Text = "'" + desigParam + "'";
                    doc.DataDefinition.FormulaFields["codeFParam"].Text = "'" + codeFParam + "'";
                    doc.DataDefinition.FormulaFields["codeTParam"].Text = "'" + codeTParam + "'";
                    doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";
                    doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                    doc.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"].ToString() + "'";
                    //doc = new rptLoanStatement();
                    //doc.SetDataSource(ds);                 
                           
                }

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;    
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        private FileStreamResult RenderReportAsPDF(ReportDocument rptDoc)
        {
            Stream stream = rptDoc.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            return File(stream, "application/PDF");
        }

        [Authorize(Roles = "Admin")]
         [Authorize(Roles = "Admin")]
        public JsonResult Delete(string ids)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "10003", "delete").ToString();
            EmployeeLoanVM vm = new EmployeeLoanVM();
            EmployeeLoanRepo _repo = new EmployeeLoanRepo();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Retrieves the Provident Fund (BWWF) loan balance and applicable interest rates 
        /// for a specific employee loan as of the provided application date.
        /// The method gathers configuration values from settings, calculates interest rates 
        /// based on loan duration, and returns the result as JSON.
        /// </summary>
        /// <param name="ApplicationDate">The date for which the BWWF balance should be calculated. If null or empty, the current date is used.</param>
        /// <param name="emploanId">The unique identifier of the employee loan.</param>
        /// <returns>A JSON object containing BWWF balance, interest rate details, and rate settings.</returns>        [HttpGet]
        public JsonResult PFBalance(string ApplicationDate, string emploanId)
        {
           
            DataTable dt = new DataTable();
            string date = ApplicationDate;
            if (string.IsNullOrWhiteSpace(ApplicationDate))
            {
                date = DateTime.Now.ToString("dd-MMM-yyyy");
            }

            EmployeeLoanVM vm = new EmployeeLoanVM();
            SettingRepo sRepo = new SettingRepo();
            EmployeeLoanRepo loanRepo = new EmployeeLoanRepo();
            dt = loanRepo.getBalance(date, emploanId);


            bool FromSetting = Convert.ToBoolean(sRepo.settingValue("PFLoanRate", "FromSetting") == "Y" ? true : false);
            int Upto12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "Upto12Month"));
            int GetterThen12Month = Convert.ToInt32(sRepo.settingValue("PFLoanRate", "GetterThen12Month"));

            if (dt != null && dt.Rows.Count > 0)
            {
                vm.PFBalance = Convert.ToDecimal(dt.Rows[0]["Balance"].ToString());
            }
            string AvailableRate = sRepo.settingValue("PFLoan", "AvailableRate");

            vm.AvailableRate = Convert.ToDecimal(AvailableRate);

            vm.FromSetting = FromSetting;
            vm.InterestRate = Upto12Month;
            vm.InterestRate1 = GetterThen12Month;

            vm.PFBalance = 999999999;


            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generates and returns a PDF report of loan transactions for the specified loan ID.
        /// The report is dynamically loaded based on company name and includes company details 
        /// and transaction data. If no data is found, a placeholder header is used.
        /// </summary>
        /// <param name="id">The unique identifier of the loan transaction to be viewed in the report.</param>
        /// <returns>A PDF file rendered as a report containing loan transaction details.</returns>

         [HttpGet]
         public ActionResult ReportView(string id)
         {
             try
             {            
                 string ReportHead = "";
                 string rptLocation = "";


                 string[] cFields = { "I.Id" };
                 string[] cValues = { id.ToString() == "0" ? "" : id.ToString() };

                 ReportDocument doc = new ReportDocument();
                 DataTable dt = new DataTable();
                 EmployeeLoanRepo _repo = new EmployeeLoanRepo();


                 dt = _repo.GetData( cFields, cValues);
                                

                 ReportHead = "There are no data to Preview for Transaction Loan";
                 if (dt.Rows.Count > 0)
                 {
                     ReportHead = "Loan Transactions";
                 }
                 dt.TableName = "dtLoan";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\BWWF\rptLoanEGCB.rpt";               

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                doc.Load(rptLocation);
                doc.SetDataSource(dt);
                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                FormulaFieldDefinitions ffds = doc.DataDefinition.FormulaFields;

                doc.DataDefinition.FormulaFields["PrincipalPaid"].Text = dt.Rows[0]["PrincipalPaid"].ToString();
                doc.DataDefinition.FormulaFields["InterestPaid"].Text = dt.Rows[0]["InterestPaid"].ToString();
                doc.DataDefinition.FormulaFields["ReportHeaderA4"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";

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
         /// Generates and returns a PDF report containing all loan records for the current branch.
         /// The report is selected based on predefined report configuration, and includes company details.
         /// Access is restricted based on user permissions.
         /// </summary>
         /// <param name="vm">The view model containing loan filter parameters (currently unused).</param>
         /// <returns>
         /// A PDF file rendered as a report if access is granted and data is available; 
         /// otherwise redirects to the Payroll Home or returns a default view on failure.
         /// </returns>

        public ActionResult AllLoanReport(EmployeeLoanVM vm)
        {
            string[] result = new string[6];
            try
            {
                #region Try

                #region Objects and Variables

                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                var permission = _reposur.SymRoleSession(identity.UserId, "1_55", "report").ToString();
                Session["permission"] = permission;
                if (permission == "False")
                {
                    return Redirect("/Payroll/Home");
                }

                ReportDocument doc = new ReportDocument();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                EmployeeLoanRepo _loanRepo = new EmployeeLoanRepo();
                var getAllData = _loanRepo.SelectAll(Convert.ToInt32(identity.BranchId), "");

                dt = Ordinary.ListToDataTable(getAllData.ToList());

                #endregion

                #region Report Call

                EnumReportRepo _reportRepo = new EnumReportRepo();
                List<EnumReportVM> enumReportVMs = new List<EnumReportVM>();

                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "LoanReport", "LoanReport2" };
                enumReportVMs = _reportRepo.SelectAll(0, conFields, conValues);

                string ReportFileName = enumReportVMs.FirstOrDefault().ReportFileName;
                string ReportName = enumReportVMs.FirstOrDefault().Name;

                CompanyRepo _CompanyRepo = new CompanyRepo();
                CompanyVM cvm = _CompanyRepo.SelectAll().FirstOrDefault();

                string rptLocation = "";

                rptLocation = AppDomain.CurrentDomain.BaseDirectory + @"Files\ReportFiles\Loan\" + ReportFileName + ".rpt";

                doc.Load(rptLocation);

                string companyLogo = AppDomain.CurrentDomain.BaseDirectory + "Images\\COMPANYLOGO.png";
                doc.DataDefinition.FormulaFields["CompanyLogo"].Text = "'" + companyLogo + "'";
                doc.DataDefinition.FormulaFields["Address"].Text = "'" + cvm.Address + "'";
                doc.DataDefinition.FormulaFields["CompanyName"].Text = "'" + cvm.Name + "'";


                #endregion

                dt.TableName = "dtLoanReport";

                doc.SetDataSource(dt);

                var rpt = RenderReportAsPDF(doc);
                doc.Close();
                return rpt;

                #endregion Try
            }
            catch (Exception ex)
            {
                result[0] = "Fail";
                result[1] = "Process Fail";
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log("LoanReport", this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace);

                return View();
            }
        }

        /// <summary>
        /// Exports all loan data for the current branch as an Excel (.xlsx) file and prompts the user to download it.
        /// The export excludes several internal or non-relevant columns. Report metadata like company name and address 
        /// are added to the Excel file. User access is validated before proceeding.
        /// </summary>
        /// <param name="vm">The view model containing loan filter parameters (currently unused).</param>
        /// <returns>
        /// An Excel file stream download if the user has permission and data is available;
        /// otherwise redirects to the SalarySheet view or Payroll Home on permission failure.
        /// </returns>
        public ActionResult DownloadAllLoanReport(EmployeeLoanVM vm)
         {
             string[] result = new string[6];
             DataSet ds = new DataSet();
             DataTable dt = new DataTable();
             List<string> ProjectIdList = new List<string>();
             try
             {
                 #region Objects and Variables

                 ReportDocument doc = new ReportDocument();

                 string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                 var permission = _reposur.SymRoleSession(identity.UserId, "1_42", "add").ToString();
                 Session["permission"] = permission;
                 if (permission == "False")
                 {
                     return Redirect("/Payroll/Home");
                 }

                 string FileName = "Download.xls";
                 string fullPath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\";
                 //string fullPath = @"C:\";
                 if (System.IO.File.Exists(fullPath + FileName))
                 {
                     System.IO.File.Delete(fullPath + FileName);
                 }
                 #endregion

              
                 #region Pull Data

                 EmployeeLoanRepo _loanRepo = new EmployeeLoanRepo();
                 var getAllData = _loanRepo.SelectAll(Convert.ToInt32(identity.BranchId), "");

                 dt = Ordinary.ListToDataTable(getAllData.ToList());

                 var toRemove = new string[] {  "Operation","PFBalance","AvailableRate","Id","LoanType_E","EmployeeId","NumberOfInstallment"
                                                ,"PeriodName","ApplicationDate","ApprovedDate","IsApproved","EndDate","IsHold","Remarks"
                                                ,"IsActive","IsArchive","CreatedBy","CreatedAt","CreatedFrom","LastUpdateBy","LastUpdateAt","LastUpdateFrom","Project"
                                                ,"Section","employeeLoanDetails","InterestPolicy","IsFixed","InterestRate","InterestRate1","BranchId","Employee"
                                                ,"FiscalYearDetailId","RefundAmount","RefundDate","FromSetting","TotalDuePrincipalAmount","TotalDueInterestAmount"
                                                ,"SettlementAmount","NoofInstallment","IsEarlySellte","EarlySellteDate"
                                            };

                //List<string> oldColumnNames = new List<string> { "EmpName", "LoanTypeName", "PrincipalAmount", "InterestAmount", "InstallmentAmount" };
                //List<string> newColumnNames = new List<string> { "Employee Name", "Type Name", "Principal Amount", "Interest Amount", "Installment Amount" };
                //dt = Ordinary.DtColumnNameChangeList(dt, oldColumnNames, newColumnNames);

                foreach (string col in toRemove)
                {
                    dt.Columns.Remove(col);
                }


                #endregion

                #region Validations

                if (dt.Rows.Count == 0)
                {
                    result[0] = "Fail";
                    result[1] = "No Data Found";
                    Session["result"] = result[0] + "~" + result[1];

                    return View();
                }

                #endregion

                #region Report Call

                string filename = "";


                string[] conFields = { "ReportType", "ReportId" };
                string[] conValues = { "LoanReport", "LoanReport1" };
                List<EnumReportVM> enumReportVMs = new EnumReportRepo().SelectAll(0, conFields, conValues);

                string ReportName = enumReportVMs.FirstOrDefault().Name;
                filename = ReportName;

                #endregion

                #region Prepare Excel

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

                //if (CompanyName.ToUpper() == "TIB" || CompanyName.ToUpper() == "G4S")
                //{
                CompanyRepo cRepo = new CompanyRepo();
                CompanyVM comInfo = cRepo.SelectById(1);
                string Line1 = comInfo.Name; // "BRAC EPL STOCK BROKERAGE LIMITED";
                string
                    Line2 = comInfo
                        .Address; // "SYMPHONY, PLOT NO. S.E (F)- 9 (3RD FLOOR), ROAD- 142, GULSHAN-1, DHAKA-1212 ";
                string Line3 = "";

                string[] ReportHeaders = new string[] { Line1, Line2, Line3 };

                #endregion

                #region Excel Download

                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=" + filename + ".xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

                #endregion

                #region Redirect

                result[0] = "Success";
                result[1] = "Successful~Data Download";

                Session["result"] = result[0] + "~" + result[1];
                return Redirect("SalarySheet");

                #endregion
            }
            catch (Exception e)
            {
                Session["result"] = result[0] + "~" + result[1];
                FileLogger.Log(
                    result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine +
                    result[5].ToString(), this.GetType().Name,
                    result[4].ToString() + Environment.NewLine + result[3].ToString());
                return Redirect("SalarySheet");
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
                workSheet.Cells[i + 1, 1, (i + 1), ColumnCount].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Left;
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

                    workSheet.Cells[GrandTotalRow, colNumber].Formula = "=Sum(" +
                                                                        workSheet.Cells[TableHeadRow + 1, colNumber]
                                                                            .Address + ":" +
                                                                        workSheet.Cells[(TableHeadRow + RowCount),
                                                                            colNumber].Address + ")";

                    #endregion
                }
            }

            workSheet.Cells[TableHeadRow, 1, TableHeadRow, ColumnCount].Style.Font.Bold = true;
            workSheet.Cells[GrandTotalRow, 1, GrandTotalRow, ColumnCount].Style.Font.Bold = true;

            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount - 1)] + (TableHeadRow + RowCount + 2)]
                .Style
                .Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[
                    "A" + (TableHeadRow) + ":" + Ordinary.Alphabet[(ColumnCount)] + (TableHeadRow + RowCount + 1)].Style
                .Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[GrandTotalRow, 1].LoadFromText("Grand Total");

            #endregion
        }   
    }
}
