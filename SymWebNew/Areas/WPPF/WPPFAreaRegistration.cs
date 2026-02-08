using System.Web.Mvc;

namespace SymWebUI.Areas.WPPF
{
    public class WPPFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WPPF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WPPF_default",
                "WPPF/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
