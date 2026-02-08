using System.Web.Mvc;

namespace SymWebUI.Areas.BWWF
{
    public class BWWFAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BWWF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BWWF_default",
                "BWWF/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
