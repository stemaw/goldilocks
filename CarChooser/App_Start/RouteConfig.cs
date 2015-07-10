﻿using System.Web.Mvc;
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
        }
    }
}
