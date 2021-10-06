using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SecondHandMarket.Web.Startup))]
namespace SecondHandMarket.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
