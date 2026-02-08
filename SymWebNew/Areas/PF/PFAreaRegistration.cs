using System.Web.Mvc;

namespace SymWebUI.Areas.PF
{
    public class PFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PF_default",
                "PF/{controller}/{action}/{id}",
                  new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.PF.Controllers" }
            );
        }
    }
}
