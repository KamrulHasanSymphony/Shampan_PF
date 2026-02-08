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
    public class AppraisalQuestionsController : Controller
    {
        //
        // GET: /Common/AppraisalQuestions/

        AppraisalQuestionsRepo _repo = new AppraisalQuestionsRepo();
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        AppraisalQuestionsRepo appraisalQuestionsRepo;
        public ActionResult Index()
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_8", "index").ToString();
            return View();
        }

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            appraisalQuestionsRepo = new AppraisalQuestionsRepo();

            #region Column Search
            var idFilter = Convert.ToString(Request["sSearch_0"]);
            var questionFilter = Convert.ToString(Request["sSearch_1"]);
            var departmentFilter = Convert.ToString(Request["sSearch_2"]);
            //var cityFilter = Convert.ToString(Request["sSearch_3"]);
            //var mobileFilter = Convert.ToString(Request["sSearch_4"]);
            //Code
            //Name
            //City
            //Mobile

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

            var getAllData = appraisalQuestionsRepo.SelectAll();
            IEnumerable<AppraisalQuestionsVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);

                filteredData = getAllData.Where(c =>
                        isSearchable1 && c.DepartmentId.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable2 && c.Question.ToLower().Contains(param.sSearch.ToLower())
                     || isSearchable3 && c.IsUser
                     || isSearchable4 && c.IsSupervisor
                    
                    );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            #region Column Filtering
            if (questionFilter != "")
            {
                filteredData = filteredData
                                .Where(c => (departmentFilter == "" || c.DepartmentId.ToLower().Contains(departmentFilter.ToLower())
                                     &&
                                    (questionFilter == "" || c.Question.ToLower().Contains(questionFilter.ToLower()))
                                   )
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
            //var isSortable_9 = Convert.ToBoolean(Request["bSortable_9"]);
            var isSortable_9 = Convert.ToBoolean(Request["bSortable_10"]);


            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<AppraisalQuestionsVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.DepartmentId :
                sortColumnIndex == 2 && isSortable_2 ? c.Question  :
                sortColumnIndex == 3 && isSortable_3 ? Convert.ToString (c.IsUser) :
                sortColumnIndex == 4 && isSortable_4 ? Convert.ToString(c.IsSupervisor) :
                sortColumnIndex == 5 && isSortable_5 ? Convert.ToString(c.IsDepartmentHead) :
                sortColumnIndex == 6 && isSortable_6 ? Convert.ToString(c.IsManagement) :
                sortColumnIndex == 7 && isSortable_7 ? Convert.ToString(c.IsHR) :

                sortColumnIndex == 8 && isSortable_8 ? Convert.ToString(c.FeedBackYear) :
                //sortColumnIndex == 9 && isSortable_9 ? Convert.ToString(c.FeedBackMonthId) :
                sortColumnIndex == 9 && isSortable_9 ? Convert.ToString(c.PeriodName) :

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
                ,c.DepartmentId
                , c.Question
                ,Convert.ToString(c.IsUser)
                ,Convert.ToString( c.IsSupervisor)
                ,Convert.ToString( c.IsDepartmentHead)
                ,Convert.ToString( c.IsManagement)
                ,Convert.ToString( c.IsHR)
                ,Convert.ToString( c.FeedBackYear)
                //,Convert.ToString( c.FeedBackMonthId)
                , c.PeriodName
                
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
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "add").ToString();
            return View();
        }
        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Create(AppraisalQuestionsVM appraisalQuestionsVM)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //BranchVM.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //BranchVM.CreatedBy = identity.Name;
            //BranchVM.CreatedFrom = identity.WorkStationIP;
            try
            {
                result = new AppraisalQuestionsRepo().Insert(appraisalQuestionsVM);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Edit", new { @id = result[2] });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(appraisalQuestionsVM);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "edit").ToString();
            AppraisalQuestionsVM vm = new AppraisalQuestionsVM();
            vm = _repo.SelectById(id);
            return View(vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        [HttpPost]
        public ActionResult Edit(AppraisalQuestionsVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            try
            {
                result = new AppraisalQuestionsRepo().Update(vm);
                Session["result"] = result[0] + "~" + result[1];
                if (result[0] == "Fail")
                {
                    return View(vm);
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Edit", new { @id = result[2] });
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View(vm);
            }
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult AppraisalQuestionsDelete(string ids)
        {
            try
            {
                 AppraisalQuestionsVM vm = new AppraisalQuestionsVM();
            Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_7", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            //vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            //vm.LastUpdateBy = identity.Name;
            //vm.LastUpdateFrom = identity.WorkStationIP;
            result = new AppraisalQuestionsRepo().Delete(vm, a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }

    }
}
