using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineRecruitment_Main.Startup))]
namespace OnlineRecruitment_Main
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
