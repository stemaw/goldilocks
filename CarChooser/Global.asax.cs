using System.Web.Mvc;
using System.Web.Routing;
using CarChooser.Web.App_Start;

namespace CarChooser.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
