using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Seminar.Startup))]
namespace Seminar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
