using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SymOrdinary;

namespace SymWebUI.Areas.Config.Controllers
{
    public class OrdinaryController : Controller
    {
        //
        // GET: /Config/Home/

        public JsonResult DateFormating(string val)
        {
          var date= Ordinary.DateFormating(val);
          return Json(date, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            return View();
        }

    }
}
