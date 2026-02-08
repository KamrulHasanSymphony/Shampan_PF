using System.Web.Mvc;

namespace SymWebUI.Areas.Benefit
{
    public class BenefitAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Benefit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Benefit_default",
                "Benefit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
