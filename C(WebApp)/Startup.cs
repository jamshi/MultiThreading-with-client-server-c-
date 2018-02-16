using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(JBP.WebApp.Startup))]
namespace JBP.WebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}