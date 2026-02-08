using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SymWebUI.Startup))]
namespace SymWebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
