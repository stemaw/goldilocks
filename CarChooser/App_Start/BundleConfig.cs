using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;


namespace CarChooser.Web.App_Start
{
    class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/js").Include(
                         "~/Scripts/angular.min.js",
                         "~/Scripts/angular-resource.min.js",
                         "~/Scripts/angular-route.min.js",
                         "~/Scripts/angular-touch.js",
                         "~/Content/angular-carousel.js",
                         "~/Scripts/App/Main.js"));

            bundles.Add(new StyleBundle("~/content/css").Include(
                         "~/Content/bootstrap.css",
                         "~/Content/grayscale.css",
                         "~/Content/CarChooser.css",
                         "~/Content/angular-carousel.css",
                         "~/Content/font-awesome/css/font-awesome.min.css"));
        }
    }
}
