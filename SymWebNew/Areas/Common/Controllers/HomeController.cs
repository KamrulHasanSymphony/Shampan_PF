using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace SymWebUI.Areas.Common.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //GET: /Common/Home/    


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

        public ActionResult PFInfo()
        {
            PfInfoDashboardVM vmPf = new PfInfoDashboardVM();
            HomePageInfoDashboardRepo _Repo = new HomePageInfoDashboardRepo();

            vmPf = _Repo.GetPfInfoDashboard();

            var ch = new chartPF
            {
                TotalPerson = vmPf.TotalPerson,
                PFValue = vmPf.PFValue,
            };

            return Json(ch, JsonRequestBehavior.AllowGet);
        }

        public class chart
        {
            public string Present { get; set; }
            public string Absent { get; set; }
            public string Late { get; set; }
            public string InMiss { get; set; }
        }

        public class charthrm
        {
            public decimal TotalEmployee { get; set; }
            public decimal InactiveEmployee { get; set; }
            public decimal NotApplicable { get; set; }
            public decimal Male { get; set; }
            public decimal Female { get; set; }
        }

        public class chartpayroll
        {
            public decimal TotalPerson { get; set; }
            public decimal GrossSalary { get; set; }       
        }
        public class chartTax
        {
            public decimal TotalPerson { get; set; }
            public decimal TaxValue { get; set; }
        }
        public class chartPF
        {
            public decimal TotalPerson { get; set; }
            public decimal PFValue { get; set; }
        }
        public class chartGF
        {
            public decimal TotalPerson { get; set; }
            public decimal GFValue { get; set; }
        }
    }
}
