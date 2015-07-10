using System.Web.Mvc;
using System.Web.Routing;

namespace CarChooser.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "CarPages",
                url: "{controller}/{action}/{pageNumber}",
                defaults: new { controller = "Car", action = "Get", pageNumber = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SpecificModel",
                url: "{manufacturer}/{model}/{derivative}/{year}/{carId}",
                defaults: new { controller = "Home", action = "Get" }
            );

            routes.MapRoute(
                name: "Manufacturers",
                url: "manufacturers",
                defaults: new { controller = "Home", action = "GetManufacturers" }
            );

            routes.MapRoute(
                name: "Models",
                url: "models/{manufacturer}",
                defaults: new { controller = "Home", action = "GetModels" }
            );

            routes.MapRoute(
                name: "Derivatives",
                url: "derivatives/{model}",
                defaults: new { controller = "Home", action = "GetDerivatives" }
            );
        }
    }
}
