using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymRepository.WPPF;
using SymViewModel.WPPF;
using SymWebUI.Areas.WPPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.WPPF.Controllers
{
    public class AutoJournalSetupController : Controller
    {
        public AutoJournalSetupController()
        {
            ViewBag.TransType = AreaTypePFVM.TransType;
        }

        SymUserRoleRepo _repoSUR = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        AutoJournalSetupRepo _repo = new AutoJournalSetupRepo();

        public ActionResult Index()
        {
            string permission = _repoSUR.SymRoleSession(identity.UserId, "10003", "index").ToString();
            Session["permission"] = permission;
            if (permission == "False")
            {
                return Redirect("/WPPF/Home");
            }
            return View();
        }

       

        public ActionResult _index(JQueryDataTableParamModel param)
        {
            var getAllData = _repo.SelectAll();
            IEnumerable<AutoJournalSetupVM> filteredData;

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredData = getAllData.Where(c =>
                    c.JournalFor.ToLower().Contains(param.sSearch.ToLower()) ||
                    c.JournalName.ToLower().Contains(param.sSearch.ToLower()) ||
                    c.Nature.ToLower().Contains(param.sSearch.ToLower()) ||
                    c.COAName.ToString().ToLower().Contains(param.sSearch.ToLower()) ||
                    c.IsActive.ToString().ToLower().Contains(param.sSearch.ToLower())
                );
            }
            else
            {
                filteredData = getAllData;
            }

            var displayedCompanies = filteredData
                .Skip(param.iDisplayStart)
                .Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[]
                 {
                     Convert.ToString(c.Id),
                     c.JournalFor,
                     c.JournalName,
                     c.Nature,
                     c.COAName.ToString(),
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



        [Authorize(Roles = "Admin")]
        public ActionResult Create(AutoJournalSetupVM vm)
        {
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "10003", "add").ToString();

            vm.Operation = "add";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(AutoJournalSetupVM vm)
        {
            string[] result = new string[6];
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.CreatedBy = identity.Name;
                    vm.CreatedFrom = identity.WorkStationIP;
                    vm.BranchId = "1";
                    result = _repo.Insert(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    if (result[0].ToLower() == "success")
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                    vm.LastUpdateBy = identity.Name;
                    vm.LastUpdateFrom = identity.WorkStationIP;
                    result = _repo.Update(vm);
                    Session["result"] = result[0] + "~" + result[1];
                    return RedirectToAction("Index", new { id = result[2] });
                }
                else
                {
                    return View("Create", vm);
                }
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Data Not Succeessfully!";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "1_5", "edit").ToString();
            AutoJournalSetupVM vm = new AutoJournalSetupVM();
            vm = _repo.SelectById(id);
            vm.Operation = "Update";
            return View("Create",vm);
        }

        [Authorize(Roles = "Master,Admin,Account")]
        public JsonResult Delete(string ids)
        {
            AutoJournalSetupVM vm = new AutoJournalSetupVM();

            ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            Session["permission"] = _repoSUR.SymRoleSession(identity.UserId, "1_5", "delete").ToString();
            string[] a = ids.Split('~');
            string[] result = new string[6];

            vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
            vm.LastUpdateBy = identity.Name;
            vm.LastUpdateFrom = identity.WorkStationIP;
            result = _repo.Delete(a);
            return Json(result[1], JsonRequestBehavior.AllowGet);
        }

    }
}
