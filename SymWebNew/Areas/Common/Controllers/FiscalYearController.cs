using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class FiscalYearController : Controller
    {
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_13", "index").ToString();
            return View();
        }

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Fiscal Year information.
        /// </summary>      
        /// <returns>View containing Fiscal Year/returns>
        public ActionResult _index(JQueryDataTableParamModel param)
        {

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var yearFilter = Convert.ToString(Request["sSearch_1"]);
            var yearStartFilter = Convert.ToString(Request["sSearch_2"]);
            var yearEndFilter = Convert.ToString(Request["sSearch_3"]);

            //Year
            //YearStart
            //YearEnd 



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


            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            var getAllData = new FiscalYearRepo().SelectAll(Convert.ToInt32(identity.BranchId));
            IEnumerable<FiscalYearVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Year.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.YearStart.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.YearEnd.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable3 && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }


            #endregion Search and Filter Data

            #region Column Filtering
            if (yearFilter != "" || yearStartFilter == "" || yearEndFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (yearFilter == "" || c.Year.ToString().ToLower().Contains(yearFilter.ToLower()))
                                            &&
                                            (yearStartFilter == "" || c.YearStart.ToLower().Contains(yearStartFilter.ToLower()))
                                            &&
                                            (yearEndFilter == "" || c.YearEnd.Contains(yearEndFilter.ToLower()))

                                        );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<FiscalYearVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Year.ToString() :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.YearStart :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.YearEnd :
                                                           sortColumnIndex == 2 && isSortable_2 ? c.Remarks :
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
                , c.Year.ToString() //+ "~" + Convert.ToString(c.Id)
                , c.YearStart
                , c.YearEnd 
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

        /// <summary>
        /// Displays the fiscal year creation view.
        /// Initializes the start date of the new fiscal year based on the latest fiscal year end date or company start date.
        /// Prepares a list of 12 empty <see cref="FiscalYearDetailVM"/> items for monthly configuration.
        /// </summary>
        /// <returns>
        /// A <see cref="ViewResult"/> that renders the fiscal year creation form with default data.
        /// </returns>
        /// <remarks>
        /// Sets the ViewBag.YearStart flag as "disabled" if previous fiscal years exist, otherwise leaves it enabled.
        /// The fiscal year start date is calculated based on previous fiscal years or company setup.
        /// </remarks>
        [HttpGet]
        public ActionResult Create()
        {

            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_13", "add").ToString();

            FiscalYearRepo fiscalYearRepo = new FiscalYearRepo();
            List<FiscalYearVM> fiscalYearLists = new List<FiscalYearVM>();
            fiscalYearLists = fiscalYearRepo.SelectAll(Convert.ToInt32(identity.BranchId));
            string yearStartDate = "";
            if (fiscalYearLists.Count > 0)
            {
                yearStartDate = DateTime.Parse(fiscalYearLists.LastOrDefault().YearEnd).AddDays(1).ToString("dd-MMM-yyyy");
                ViewBag.YearStart = "disabled";
            }
            else
            {
                CompanyRepo compRepo = new CompanyRepo();
                CompanyVM company = compRepo.SelectById(Convert.ToInt32(identity.CompanyId));

                DateTime newDate = Convert.ToDateTime(Ordinary.StringToDate(company.YearStart));
                yearStartDate = newDate.ToString("dd-MMM-yyyy");
                ViewBag.YearStart = "";
            }

            FiscalYearVM vm = new FiscalYearVM();
            List<FiscalYearDetailVM> dvms = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM dvm;
            for (int i = 1; i < 13; i++)
            {
                dvm = new FiscalYearDetailVM();

                dvms.Add(dvm);
            }
            vm.FiscalYearDetailVM = dvms;
            vm.YearStart = yearStartDate;
            vm = DesignFiscalYear(vm);
            return View(vm);
        }

        /// <summary>
        /// Handles the submission of a new fiscal year creation request.
        /// Populates the fiscal year structure using <see cref="DesignFiscalYear"/> and inserts it into the database.
        /// </summary>
        /// <param name="vm">The <see cref="FiscalYearVM"/> object containing user input for the fiscal year.</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> to the Index action upon success or failure.
        /// </returns>
        /// <remarks>
        /// On success, stores the result message in the session and redirects to the Index.
        /// On failure, logs the error and stores the failure message in the session.
        /// </remarks>
        [HttpPost]
        public ActionResult Create(FiscalYearVM vm)
        {
            string[] result = new string[6];
            //ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

            FiscalYearRepo fiscalYearRepo = new FiscalYearRepo();
            //FiscalYearVM fiscalYearList = fiscalYearRepo.SelectAll(Convert.ToInt32(identity.BranchId)).LastOrDefault();
            //if (fiscalYearList!=null)
            //{
            //    vm.YearStart = DateTime.Parse(fiscalYearList.YearEnd).AddDays(1).ToString("dd-MMM-yyyy");
            //}
            //else
            //{
            //    //var values = Ordinary.DateToString(vm.YearStart);
            //    vm.YearStart = Ordinary.DateToString(vm.YearStart);// new DateTime(Convert.ToInt32(values.Substring(0, 4)), Convert.ToInt32(values.Substring(4, 2)), 01).ToString("dd-MMM-yyyy");
            //}

            FiscalYearVM newVM = DesignFiscalYear(vm);
            try
            {

                result = new FiscalYearRepo().FiscalYearInsert(newVM);
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

        /// <summary>
        /// Generates fiscal year details including start and end dates, period breakdown (monthly), and audit metadata.
        /// </summary>
        /// <param name="vm">The <see cref="FiscalYearVM"/> containing the initial year start date.</param>
        /// <returns>
        /// A populated <see cref="FiscalYearVM"/> with:
        /// - <c>YearEnd</c> calculated as one year minus one day from <c>YearStart</c>,
        /// - A list of 12 <see cref="FiscalYearDetailVM"/> representing each fiscal month,
        /// - Audit fields like <c>CreatedAt</c>, <c>CreatedBy</c>, etc., filled from the current user's identity.
        /// </returns>
        /// <remarks>
        /// Converts the provided <c>YearStart</c> to a <see cref="DateTime"/>, calculates the fiscal year range,
        /// and builds the monthly breakdown with start and end dates.
        /// </remarks>
        private FiscalYearVM DesignFiscalYear(FiscalYearVM vm)
        {
            var date = Ordinary.DateToString(vm.YearStart);
            DateTime start_date = new DateTime(Convert.ToInt32(date.Substring(0, 4)), Convert.ToInt32(date.Substring(4, 2)), Convert.ToInt32(date.Substring(6, 2)));
            vm.YearEnd = start_date.AddYears(1).AddDays(-1).ToString("dd-MMM-yyyy");
            vm.Year = Convert.ToInt32(start_date.AddYears(1).AddDays(-1).ToString("yyyy"));

            List<FiscalYearDetailVM> fvms = new List<FiscalYearDetailVM>();
            FiscalYearDetailVM fvm = new FiscalYearDetailVM();
            for (int i = 0; i < 12; i++)
            {
                fvm = new FiscalYearDetailVM();
                fvm.PeriodName = start_date.AddMonths(i).ToString("MMM-yy"); // start_date.AddMonths(i).ToString("MMMM") + "-" + vm.Year;
                fvm.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
                fvm.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
                fvms.Add(fvm);
            }
            //foreach (var item in vm.FiscalYearDetailVM)
            //{
            //    item.PeriodName = start_date.AddMonths(i).ToString("MMMM-yyyy"); // start_date.AddMonths(i).ToString("MMMM") + "-" + vm.Year;
            //    item.PeriodStart = start_date.AddMonths(i).ToString("dd-MMM-yyyy");
            //    item.PeriodEnd = start_date.AddMonths(i + 1).AddDays(-1).ToString("dd-MMM-yyyy");
            //    i++;
            //}
            vm.FiscalYearDetailVM = fvms;
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            vm.BranchId = Convert.ToInt32(identity.BranchId);
            return vm;
        }

        /// <summary>
        /// Loads the fiscal year data for editing based on the provided ID.
        /// </summary>
        /// <param name="Id">The ID of the fiscal year to be edited.</param>
        /// <returns>
        /// A <see cref="ViewResult"/> displaying the fiscal year data in the edit view.
        /// </returns>
        /// <remarks>
        /// Sets the session permission for edit access using the current user's identity,
        /// then retrieves the fiscal year details from the repository based on the given ID.
        /// </remarks
        [HttpGet]
        public ActionResult Edit(string Id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_13", "edit").ToString();
            return View(new FiscalYearRepo().SelectById(Id));
        }

        /// <summary>
        /// Handles the submission of the edited fiscal year data and updates it in the repository.
        /// </summary>
        /// <param name="vm">The <see cref="FiscalYearVM"/> model containing the updated fiscal year data.</param>
        /// <returns>
        /// A <see cref="RedirectToActionResult"/> that redirects to the "Index" action, showing the result of the update.
        /// </returns>
        /// <remarks>
        /// The method updates the fiscal year details in the repository, logging any errors and storing the update result in the session.
        /// It also updates the last update metadata (e.g., timestamp, user, workstation) before performing the update operation.
        /// </remarks>
        [HttpPost]
        public ActionResult Edit(FiscalYearVM vm)
        {
            string[] result = new string[6];
            try
            {
                ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = identity.Name;
                vm.LastUpdateFrom = identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(identity.BranchId);
                result = new FiscalYearRepo().FiscalYearUpdate(vm);
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

        public JsonResult Year(string term)
        {
            return Json(new FiscalYearRepo().Autocomplete(term), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks if the specified fiscal period is locked and returns the status with a message.
        /// </summary>
        /// <param name="FiscalYearDetailId">The ID of the fiscal year detail to check for lock status.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing a boolean indicating the lock status of the fiscal period,
        /// along with a message. The message indicates whether the fiscal period is locked or if an error occurred.
        /// </returns>
        /// <remarks>
        /// This method calls the repository to verify if the fiscal period is locked.
        /// If the period is locked, it returns a message indicating that the fiscal period is locked.
        /// In case of an exception, it returns the error message in the response.
        /// </remarks>
        [Authorize]
        public ActionResult FiscalPeriodLockCheck(int FiscalYearDetailId)
        {
            bool PeriodLock;
            string message="";
            try
            {

                PeriodLock = new FiscalYearRepo().FiscalPeriodLockCheck(FiscalYearDetailId);

                if (PeriodLock)
                {
                    message = "This Fiscal Period is Locked";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(true + "~" + message, JsonRequestBehavior.AllowGet);

            }

            return Json(PeriodLock + "~" + message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks if the fiscal period for a given date is locked and returns the status with a message.
        /// </summary>
        /// <param name="PeriodDate">The date representing the fiscal period to check for lock status, in "MM/dd/yyyy" format.</param>
        /// <returns>
        /// A <see cref="JsonResult"/> containing a boolean indicating the lock status of the fiscal period,
        /// along with a message. The message indicates whether the fiscal period is locked or if an error occurred.
        /// </returns>
        /// <remarks>
        /// This method converts the provided period date into a "MMM-yy" format, then queries the repository 
        /// to check if the fiscal period is locked. It returns a message that indicates whether the period is locked 
        /// or if an error occurs during the process.
        /// </remarks>
        [Authorize]
        public ActionResult FiscalPeriodLockCheck_PeriodDate(string PeriodDate)
        {
            bool PeriodLock;
            string message = "";

            try
            {
                string PeriodName = Convert.ToDateTime(PeriodDate).ToString("MMM-yy");

                string[] conditionFields = { "PeriodName" };
                string[] conditionValues = { PeriodName };

                PeriodLock = new FiscalYearRepo().SelectAll_FiscalYearDetail(0, conditionFields, conditionValues).FirstOrDefault().PeriodLock;

                if (PeriodLock)
                {
                    message = "This Fiscal Period is Locked";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(true + "~" + message, JsonRequestBehavior.AllowGet);

            }

            return Json(PeriodLock + "~" + message, JsonRequestBehavior.AllowGet);
        }

    }
}
