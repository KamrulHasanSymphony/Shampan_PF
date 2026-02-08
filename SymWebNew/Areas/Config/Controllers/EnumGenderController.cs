using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymRepository.Enum;
using SymViewModel.Common;
using SymRepository.Enum;
using SymViewModel.Common;
using SymViewModel.Enum;
using SymOrdinary;
using System.Threading;
using SymRepository.Common;

namespace SymWebUI.Areas.Config.Controllers
{
    [Authorize]
    public class EnumGenderController : Controller
    {
        //
        // GET: /Enum/EnumGender/
         SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        EnumGenderRepo _repo = new EnumGenderRepo();
        #region Actions
        public ActionResult Index()
        {            
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "index").ToString();

            return View();
        }
        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Enum Gender information.
        /// </summary>      
        /// <returns>View containing Enam Gender</returns>
        public ActionResult _index(JQueryDataTableParamVM param, string code, string name)
        {
            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var NameFilter = Convert.ToString(Request["sSearch_1"]);
            var IsActiveFilter = Convert.ToString(Request["sSearch_2"]);
            var remarksFilter = Convert.ToString(Request["sSearch_3"]);

            var IsActiveFilter1 = IsActiveFilter.ToLower() == "active" ? true.ToString() : false.ToString();
            #endregion Column Search

            var getAllData = _repo.SelectAll();
            IEnumerable<EnumGenderVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);

                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               isSearchable2 && c.Remarks.ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredData = getAllData;
            }
            #region Column Filtering
            if (NameFilter != "" || IsActiveFilter != "" || remarksFilter != "")   

            {
                filteredData = filteredData
                                .Where(c =>
                                           (NameFilter == "" || c.Name.ToLower().Contains(NameFilter.ToLower()))
                                           &&
                                           (IsActiveFilter == "" || c.IsActive.ToString().ToLower().Contains(IsActiveFilter1.ToLower()))
                                            &&
                                           (remarksFilter == "" || c.Remarks.ToString().ToLower().Contains(remarksFilter.ToLower()))
                                           );
            }

            #endregion Column Filtering

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<EnumGenderVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Name :
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
                             , c.Name
                             , Convert.ToString(c.IsActive == true ? "Active" : "Inactive") 
                             , c.Remarks
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
        [HttpGet]
        public ActionResult Create()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "add").ToString();

            return PartialView();
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
        /// The method uses the `BankRepo`'s `Insert` method to save the branch details. In case of an exception, 
        /// the error details are logged using the `FileLogger` class and a failure message is stored in the session.
        /// </remarks>
        [HttpPost]
        public ActionResult Create(EnumGenderVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.CreatedBy = identity.Name;
            vm.CreatedFrom = identity.WorkStationIP;
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

        /// <summary>
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves EnumGender data by ID.
        /// </summary>
        /// <param name="id">The ID of the EnumGender to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="EnumGenderVM"/> to populate the edit form.
        /// </returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "edit").ToString();

            return PartialView(_repo.SelectById(id));
        }

        /// <summary>
        /// Handles the HTTP GET request to load the edit view for a specific department.
        /// Checks user permission and retrieves EnumGender data by ID.
        /// </summary>
        /// <param name="id">The ID of the EnumGender to be edited.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> containing the <see cref="EnumGenderVM"/> to populate the edit form.
        /// </returns>
        [HttpPost]
        public ActionResult Edit(EnumGenderVM vm, string btn)
        {         
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
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

        /// <summary>
        /// Deletes settlement policies based on the provided IDs. 
        /// Checks the user's permission before performing the delete operation.
        /// </summary>
        /// <param name="ids">A string containing the IDs of the settlement policies to be deleted, separated by a '~' character.</param>
        /// <returns>
        /// A JSON result containing the status message (e.g., success or failure) of the delete operation.
        /// </returns>
        public JsonResult Delete(string ids)
        {
            EnumGenderVM vm = new EnumGenderVM();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_17", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];
            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }
        #endregion Actions

    }
}
