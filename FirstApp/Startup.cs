using ScanerApp.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ScanerApp.Startup))]
namespace ScanerApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
