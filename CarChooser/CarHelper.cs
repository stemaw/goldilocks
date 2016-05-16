using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CarChooser.Domain;

namespace CarChooser.Web
{
    public static class CarHelper
    {
        public static string GetBestImage(int modelId, int derivativeId)
        {
            const string imageRoot = "/content/carimages/";
            var physicalRoot = HttpContext.Current.Server.MapPath(imageRoot);

            const string derivativeFormat = "{0}{1}-{2}.jpg";
            var derivativePath = string.Format(derivativeFormat, physicalRoot, modelId, derivativeId);

            if (System.IO.File.Exists(derivativePath)) return string.Format(derivativeFormat, imageRoot, modelId, derivativeId);

            const string modelFormat = "{0}{1}.jpg";
            var modelPath = string.Format(modelFormat, physicalRoot, modelId);

            if (System.IO.File.Exists(modelPath)) return string.Format(modelFormat, imageRoot, modelId);

            return imageRoot + "default.png";
        }

        public static string GetUrl(Car car)
        {
            return GetUrl(car.Manufacturer.Name, car.Model, car.Name, car.YearFrom, car.Id);
        }

        public static string GetUrl(string manufacturer, string model, string derivative, int yearFrom, int derivativeId)
        {
            return string.Format("{0}/{1}/{2}/{3}/{4}", manufacturer, model, HttpUtility.UrlEncode(derivative.Replace("/","-")), yearFrom, derivativeId).Replace(" ", "_");
        }

        public static string GetYear(int yearFrom, int yearTo)
        {
            if (yearTo == 0) return string.Format("({0} on)", To2DigitYear(yearFrom));

            return string.Format("({0} - {1})", To2DigitYear(yearFrom), To2DigitYear(yearTo));
        }

        private static string To2DigitYear(int year)
        {
            if (year.ToString().Length < 4) return year.ToString();
            return year.ToString().Substring(2, 2);
        }
    }
}
