using System.Web;
using System.Web.Mvc;

public class SessionCheckAttribute : ActionFilterAttribute
{
    private readonly string[] _requiredSessions;

    public SessionCheckAttribute(params string[] requiredSessions)
    {
        _requiredSessions = requiredSessions;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = HttpContext.Current.Session;

        // Skip Login page
        var url = filterContext.HttpContext.Request.RawUrl.ToLower();
        if (url.Contains("/Home/Index"))
            return;

        foreach (var key in _requiredSessions)
        {
            if (session[key] == null)
            {
                filterContext.Result = new RedirectResult("/Home/Index");
                return;
            }
        }

        base.OnActionExecuting(filterContext);
    }
}