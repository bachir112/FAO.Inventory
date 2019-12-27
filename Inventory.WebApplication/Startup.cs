using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Inventory.WebApplication.Startup))]
namespace Inventory.WebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
