using System.Web.Mvc;

namespace SymWebUI.Areas.WWF
{
    public class WWFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WWF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WWF_default",
                "WWF/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
