using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimpleWebStore.Startup))]
namespace SimpleWebStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
