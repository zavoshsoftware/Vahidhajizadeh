using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VahidHajizadeh.Startup))]
namespace VahidHajizadeh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
