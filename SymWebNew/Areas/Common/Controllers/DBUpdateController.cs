using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Common.Controllers
{
    public class DBUpdateController : Controller
    {
        //
        // GET: /Common/DBUpdate/
        SymUserRoleRepo _reposur = new SymUserRoleRepo();
        ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DBUpdate()
        {
            try
            {
                Session["permission"] = _reposur.SymRoleSession(identity.UserId, "1_16", "process").ToString();
                string CompanyName = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

                string[] result = new string[6];

                #region Settings
                SettingsVM vm = new SettingsVM();
                SettingRepo _repo = new SettingRepo();
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss");
                vm.CreatedBy = Identity.Name;
                vm.CreatedFrom = Identity.WorkStationIP;
                vm.BranchId = Convert.ToInt32(Identity.BranchId);

                result = _repo.settingsDataInsert(vm, "PF", "IsWeightedAverageMonth", "Boolean", "Y");
                result = _repo.settingsDataInsert(vm, "PF", "IsProfitCalculation", "Boolean", "Y");
                result = _repo.settingsDataInsert(vm, "PF", "AccruedByDay", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "PFLoanRate", "FromSetting", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "PFLoanRate", "Upto12Month", "int", "5");
                result = _repo.settingsDataInsert(vm, "PFLoanRate", "GetterThen12Month", "int", "6");
                result = _repo.settingsDataInsert(vm, "PF", "FromDOJ", "Boolean", "N");
                result = _repo.settingsDataInsert(vm, "PF", "IsAutoJournal", "Boolean", "Y");
               

                #region HR-Payroll

                result = _repo.settingsDataInsert(vm, "HRM", "IsESSEditPermission", "string", "N");

                result = _repo.settingsDataInsert(vm, "AutoUser", "Employee", "string", "Y");
                result = _repo.settingsDataInsert(vm, "AutoPassword", "Employee", "string", "123456");                
                result = _repo.settingsDataInsert(vm, "AutoCode", "Employee", "Boolean", "Y");

                result = _repo.settingsDataInsert(vm, "PF", "FromPayroll", "Boolean", "Y");
                result = _repo.settingsDataInsert(vm, "PF", "YearDay", "Int", "365");


           
                #endregion


                #endregion Settings

                DBUpdateRepo comprepo = new DBUpdateRepo();

                #region PF Module

                comprepo.PF_DBUpdate();

                #endregion
                

                Session["result"] = result[0] + "~" + result[1];
                return Redirect("/Common/Home/");
            }
            catch (Exception e)
            {
                return Redirect("/Common/Home/");
            }
        }
    }
}
