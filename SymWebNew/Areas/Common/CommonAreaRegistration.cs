using System.Web.Mvc;

namespace SymWebUI.Areas.Common
{
    public class CommonAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Common_default",
                "Common/{controller}/{action}/{id}",
              new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.Common.Controllers" }
            );
        }
    }
}
