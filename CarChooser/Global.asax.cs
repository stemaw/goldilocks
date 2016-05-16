using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CarChooser.Domain.ScoreStrategies;
using CarChooser.Web.App_Start;

namespace CarChooser.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_OnStart()
        {
            Session["Scorer"] = new AdaptiveScorer();
        }
    }
}
