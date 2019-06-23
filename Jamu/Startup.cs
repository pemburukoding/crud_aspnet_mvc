using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jamu.Startup))]
namespace Jamu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
