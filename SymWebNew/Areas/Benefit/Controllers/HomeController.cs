using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymWebUI.Areas.Benefit.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Benefit/Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PFHome()
        {
            return View();
        }
        public ActionResult GFHome()
        {
            return View();
        }

    }
}
