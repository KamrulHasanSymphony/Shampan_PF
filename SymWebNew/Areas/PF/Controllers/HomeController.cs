using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.PF.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /PF/Home/

        //public ActionResult Index()
        //{            
        //    return View();
        //}

        public ActionResult Index()
        {
            AdminInfoDashboardVM vm = new AdminInfoDashboardVM();
            PfInfoDashboardVM vmPf = new PfInfoDashboardVM();

            HomePageInfoDashboardRepo _Repo = new HomePageInfoDashboardRepo();
            BranchRepo BranchRepo = new BranchRepo();

            int BranchId = Convert.ToInt32(Session["BranchId"].ToString());

            BranchVM branch = BranchRepo.SelectById(Convert.ToInt32(BranchId));

            Session["BranchId"] = branch.Id;
            vm.BranchVM = branch;
            vmPf = _Repo.GetPfInfoDashboard();
            vm.PfInfoDashboardVMS = vmPf;
            Session["BranchName"] = vm.BranchVM.Name;

            return View("Index", vm);
        }

    }
}
