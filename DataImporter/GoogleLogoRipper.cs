using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using CarChooser.Data;
using CarChooser.Domain;
using Coypu;

namespace DataImporter
{
    public class GoogleLogoRipper
    {
        private BrowserSession Browser { get; set; }
        private readonly List<string> _exclude;
        private string[] _files;
        readonly static string _imageFolder = @"C:\gitcode\goldilocks\CarChooser\Content\ManufacturerLogos\";
        
        public GoogleLogoRipper(BrowserSession browser)
        {
            Browser = browser;
            
            _exclude = new List<string>
                {
                    //"site:http://zombiedrive.com/",
                    //"site:http://boldride.com/",
                    //"site:http://netcarshow.com/",
                    //"interior"
                };
        }

        public void RipLogos(bool manualMode = false, bool forceUpdate = false)
        {
            var manufacturers = new CarRepository().AllCars().GroupBy(c => c.Manufacturer.Name).Select(g => g.Key);

            foreach (var m in manufacturers)
            {
                RipImage(m, manualMode, forceUpdate);
            }
        }
        
        private void RipImage(string manufacturer, bool manualMode, bool forceUpdate = false)
        {
            if (_files == null)
            {
                _files = Directory.GetFiles(_imageFolder);
            }

            Thread.Sleep(200);

            var imageName = GetImageName(manufacturer);

            if (!forceUpdate && File.Exists(imageName)) return;

            retry:           
            var url =
                string.Format(
                    "https://www.google.co.uk/search?tbs=%2Cic:trans%2Cisz:l&tbm=isch&q={0}+{1}&cad=h#q={0}&tbas=0&tbs=ic:trans,isz:lt,islt:qsvga&tbm=isch",
                    HttpUtility.HtmlDecode(manufacturer) + " logo", string.Join("+-", _exclude));
            
            Browser.Visit(url);

            if (!manualMode)
            {
                Browser.FindCss("#rg_s > div:nth-child(1) > a > img").Click();

                Browser.ClickLink("View image");
            }
            else
            {
                var currentHost = Browser.Location.Host;
                while (Browser.Location.Host == currentHost)
                {
                    // wait for manual selection
                    Thread.Sleep(1000);
                }
            }
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
            Console.WriteLine("Successfully found logo for {0}", manufacturer);
        }

        private static string GetImageName(string id)
        {
            return string.Format(_imageFolder + "{0}.png", id);
        }
    }
}