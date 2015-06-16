using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using CarChooser.Domain;
using Coypu;

namespace DataImporter
{
    public class GoogleImageRipper
    {
        private BrowserSession Browser { get; set; }
        private readonly List<string> _exclude;
        private string[] _files
            ;

        public GoogleImageRipper(BrowserSession browser)
        {
            Browser = browser;
            
            _exclude = new List<string>
                {
                    "site:http://zombiedrive.com/",
                    "interior"
                };
        }

        
        public void RipImage(Car car, bool modelLevel)
        {
            if (_files == null)
                _files = Directory.GetFiles(@"C:\Users\ste_000\Documents\goldilocks\CarChooser\Content\CarImages\");
            
            Thread.Sleep(200);

            var idToUse = car.ModelId.ToString();

            if (!modelLevel)
            {
                idToUse = car.ModelId + "-" + car.Id;
            }

            var imageName = GetImageName(idToUse);

            if (File.Exists(imageName)) return;

            if (modelLevel)
            {
                var match = _files.FirstOrDefault(f => f.Replace(@"C:\Users\ste_000\Documents\goldilocks\CarChooser\Content\CarImages\","").StartsWith(idToUse + "-"));
                if (match != null)
                {
                    File.Copy(match, imageName);
                }
            }
            retry:           
            var url =
                string.Format(
                    "https://www.google.co.uk/search?tbs=isc:black%2Cic:trans%2Cisz:l&tbm=isch&q={0}+{1}+{2}+{3}+-{4}&cad=h#q={0}+{1}+{2}+{3}+-{4}&tbs=isz:l&tbm=isch&tbas=0",
                    car.Manufacturer.Name, car.Model, ReplaceStuff(car.LessCrypticName), car.YearFrom, string.Join("+-", _exclude));
            //var url =
            //    string.Format(
            //        "https://www.google.co.uk/search?q={0}+{1}+{2}&espv=2&biw=1745&bih=814&source=lnms&tbm=isch&sa=X&ei=j_tNVdvjN8OC7gaxvIE4&ved=0CAYQ_AUoAQ#tbs=isc:black%2Cic:trans%2Cisz:l&tbm=isch&q={0}+{1}+{2}",
            //        make, model.ModelName, model.YearFrom);

            Browser.Visit(url);

            Browser.FindCss("#rg_s > div:nth-child(1) > a > img").Click();

            Browser.ClickLink("View image");

            string imageUrl;
            string siteUrl = null;
            try
            {
                siteUrl = Browser.Location.Host;

                imageUrl = Browser.FindXPath("//img")["src"];
            }
            catch (Exception)
            {
                var siteToExclude = "site:" + siteUrl;
                if (!_exclude.Contains(siteToExclude))
                {
                    _exclude.Add(siteToExclude);
                }

                Browser.GoBack();

                goto retry;
            }
            
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(imageUrl, imageName);
            }
            Console.WriteLine("Successfully found image for {0} {1} {2}", car.Manufacturer.Name, car.Model, car.LessCrypticName);
        }

        private string ReplaceStuff(string lessCrypticName)
        {
            return Regex.Replace(lessCrypticName, @"\([^)]+\)", "");
        }

        private static string GetImageName(string id)
        {
            return string.Format(
                @"C:\Users\ste_000\Documents\goldilocks\CarChooser\Content\CarImages\{0}.jpg",
                id);
        }
    }
}