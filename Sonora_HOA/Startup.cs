using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sonora_HOA.Startup))]
namespace Sonora_HOA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
