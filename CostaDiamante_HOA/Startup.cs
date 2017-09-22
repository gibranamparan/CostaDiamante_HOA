using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CostaDiamante_HOA.Startup))]
namespace CostaDiamante_HOA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
