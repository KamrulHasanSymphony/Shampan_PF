using SymOrdinary;
using SymRepository.Common;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Company/
        BranchRepo branchRepo = new BranchRepo();
        ShampanIdentityVM vm = new ShampanIdentityVM();
       
        
        public ActionResult Index()
        {
            List<BranchVM> company = branchRepo.SelectAll();
            return View(company);
        }
        public ActionResult Select(string Id)
        {
            int branchId;
            if (!int.TryParse(Id, out branchId))
            {
                Session["result"] = "Fail~Invalid branch.";
                return RedirectToAction("Index", "Home");
            }

            string errorMessage;
            if (!LoginContextHelper.TryCompleteBranchSelection(this, branchId, out errorMessage))
            {
                Session["result"] = "Fail~" + errorMessage;
                return RedirectToAction("Index", "Home");
            }

            return Redirect("/Common/Home");
        }

    }
}
