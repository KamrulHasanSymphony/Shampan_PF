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
    [Authorize]
    public class CompanyController : Controller
    {
        //
        // GET: /Common/Company/

        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        CompanyRepo compRepo = new CompanyRepo();

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Company information.
        /// </summary>      
        /// <returns>View containing Company</returns>
        public ActionResult Index()
        {
            return View(); 
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            var getAllData = compRepo.SelectAll();
            IEnumerable<CompanyVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredData = getAllData.Where(c =>
                    (c.Code ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Name ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Address ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.City ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.PostalCode ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Phone ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Mobile ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Fax ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    (c.Remarks ?? "").ToLower().Contains(param.sSearch.ToLower()) ||
                    c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }

            var displayedData = filteredData
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from c in displayedData
                         select new[]
                 {
                     Convert.ToString(c.Id),
                     c.Code,
                     c.Name,
                     c.Address,
                     c.City,
                     c.PostalCode,
                     c.Phone,
                     c.Mobile,
                     c.Fax,
                     c.Remarks,
                     c.IsActive ? "Yes" : "No"
                 };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays the view for creating a new entry. 
        /// This method checks the user's permission for the "add" action in the "1_7" role. 
        /// If the user does not have the appropriate permissions, they are redirected to a different page.
        /// </summary>
        /// <returns>
        /// The "Create" view, or a redirect to another page if the user lacks the required permissions.
        /// </returns>
        /// <remarks>
        /// This action method fetches the user's permission status for adding an entry (via the `SymRoleSession` method).
        /// If the user has the necessary permissions, the "Create" view is returned. 
        /// If the user does not have permission, they are redirected to a common home page.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Create()
        {
            CompanyVM company = compRepo.SelectAll().FirstOrDefault();
            if (company !=null)
            {
               // return RedirectToAction("Edit");
            }
            return View();
        }
        /// <summary>
        /// Creates a new branch entry. This method accepts a `BranchVM` model, populates some additional fields 
        /// such as `CreatedAt`, `CreatedBy`, and `CreatedFrom` before calling the repository's `Insert` method to save the data.
        /// If the operation is successful, the user is redirected to the "Index" page, otherwise an error message is logged and 
        /// the same view is returned with the model.
        /// </summary>
        /// <param name="BranchVM">The branch view model containing the data to be created.</param>
        /// <returns>
        /// If the insertion is successful, redirects to the "Index" action.
        /// If the insertion fails, logs the error and returns the same view with the model.
        /// </returns>
        /// <remarks>
        /// The method uses the `BranchRepo`'s `Insert` method to save the branch details. In case of an exception, 
        /// the error details are logged using the `FileLogger` class and a failure message is stored in the session.
        /// </remarks>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult CreateData(CompanyVM company)
        {
            string[] result = new string[6];
            company.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            company.CreatedBy = Ordinary.UserName;
            company.CreatedFrom = Ordinary.WorkStationIP;
            try
            {

                FiscalYearRepo fiscalYearRepo = new FiscalYearRepo();
                List<FiscalYearVM> fiscalYearLists = new List<FiscalYearVM>();
                fiscalYearLists = fiscalYearRepo.SelectAll(Convert.ToInt32(identity.BranchId));
                CompanyRepo compRepo = new CompanyRepo();          
                string yearStartDate = "";

                if (fiscalYearLists.Count > 0)
                {
                    yearStartDate = DateTime.Parse(fiscalYearLists.LastOrDefault().YearEnd).AddDays(1).ToString("dd-MMM-yyyy");
                    ViewBag.YearStart = "disabled";
                }
                else
                {
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
                FiscalYearVM newVM = DesignFiscalYear(vm);

                result = compRepo.Insert(company);               

                if(result[0]=="Success")
                {
                    result = new FiscalYearRepo().FiscalYearInsert(newVM);
                }
               
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(company);
            }
        }

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
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves Company data by ID.
        /// </summary>
        /// <param name="id">The ID of the Company to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="CompanyVM"/> to populate the edit form.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            CompanyVM company = compRepo.SelectById(id);
            return View(company);
        }

        /// <summary>
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves Bank Company by ID.
        /// </summary>
        /// <param name="id">The ID of the Company to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="CompanyVM"/> to populate the edit form.
        /// </returns>
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(CompanyVM company)
        { 
            string[] result = new string[6];            
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            company.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            company.LastUpdateBy = Ordinary.UserName;
            company.LastUpdateFrom = Ordinary.WorkStationIP;
            company.CurrentBranch=Convert.ToInt32(identity.BranchId);
            try
            {
                result = compRepo.Update(company);
                Session["result"] = result[0] + "~" + result[1];
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
            }
            try
            {
                company.Year= DateTime.Parse(company.YearStart).ToString("yyyy");
            }
            catch (Exception)
            {
            }
            return View(company);
        }
    }
}
