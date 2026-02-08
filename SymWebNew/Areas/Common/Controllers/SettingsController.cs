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
    public class SettingsController : Controller
    {
        //
        // GET: /Common/Settings/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        SettingRepo _repo = new SettingRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        /// <summary>
        /// Created: 10 Feb 2025  
        /// Created By: Md Torekul Islam  
        /// Retrieves all Department information.
        /// </summary>      
        /// <returns>View containing Department</returns>
        public ActionResult Index()
        {
            List<SettingsVM> vms = new List<SettingsVM>();
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_15", "index").ToString();
                vms = _repo.SettingsAll();

                return View(vms);
            }
            catch (Exception)
            {
                return View(vms);
            }
        }

        /// <summary>
        /// Handles the HTTP POST request to update an existing Settings's information.
        /// Sets metadata fields and attempts to update the Settings record using the repository.
        /// Logs error and sets session result in case of failure.
        /// </summary>
        /// <param name="vm">The <see cref="SettingsVM"/> containing the updated department data.</param>
        /// <param name="btn">Optional button parameter for extended functionality (currently unused).</param>
        /// <returns>
        /// A <see cref="RedirectToRouteResult"/> that redirects to the Index view.
        /// </returns>
        public ActionResult Edit(SettingsVM vm)
        {
            string[] result = new string[6];
            try
            {


                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_15", "edit").ToString();
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.LastUpdateAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.LastUpdateBy = Identity.Name;
                vm.LastUpdateFrom = Identity.WorkStationIP;
                result = new SettingRepo().settingsDataUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");

                //return Json(result[0] + "~" + result[1], JsonRequestBehavior.AllowGet);
                //throw;
            }

        }

    }
}
