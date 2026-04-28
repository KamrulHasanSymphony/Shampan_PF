using SymOrdinary;
using SymViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymRepository;

namespace SymWebUI.Controllers
{
     [AllowAnonymous]
    public class DbCreateController : Controller
    {
        //
        // GET: /DbCreate/
       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DbCreateVM vm)
        {
            string[] result = new string[6];
            
            try
            {
                result = new DbCreateRepo().Insert(vm);
                Session["result"] = result[0] + "~" + result[1];
                //return RedirectToAction("Index");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                Session["result"] = "Fail~Database Not Save Succeessfully";
                FileLogger.Log(result[0].ToString() + Environment.NewLine + result[2].ToString() + Environment.NewLine + result[5].ToString(), this.GetType().Name, result[4].ToString() + Environment.NewLine + result[3].ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult TestConnection(DbCreateVM vm)
        {
            string[] result = new string[6];

            try
            {
                result = new DbCreateRepo().TestConnection(vm);

                ViewBag.Message = result[0] + "~" + result[1];

                return View("Index", vm);
            }
            catch (Exception)
            {
                ViewBag.Message = "Fail~Database Not Save Successfully";

                return View("Index", vm); 
            }
        }

    }
}
