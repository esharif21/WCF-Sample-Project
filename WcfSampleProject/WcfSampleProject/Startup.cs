using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WcfSampleProject.Startup))]
namespace WcfSampleProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
