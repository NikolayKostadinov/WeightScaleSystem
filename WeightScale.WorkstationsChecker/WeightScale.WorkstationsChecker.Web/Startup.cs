using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeightScale.WorkstationsChecker.Web.Startup))]
namespace WeightScale.WorkstationsChecker.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
