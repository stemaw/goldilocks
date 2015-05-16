using System;
using System.IO;
using System.Net;
using Coypu;

namespace DataImporter
{
    public class GoogleImageRipper
    {
        private BrowserSession Browser { get; set; }

        public GoogleImageRipper(BrowserSession browser)
        {
            Browser = browser;
        }

        public void RipImage(long id, string make, ModelWithYear model)
        {
            if (File.Exists(GetImageName(id))) return;

            var url =
                string.Format(
                    "https://www.google.co.uk/search?tbs=isc:black%2Cic:trans%2Cisz:l&tbm=isch&q={0}+{1}+{2}&cad=h#q={0}+{1}+{2}&tbs=isz:l&tbm=isch&tbas=0",
                    make, model.ModelName, model.YearFrom);
            //var url =
            //    string.Format(
            //        "https://www.google.co.uk/search?q={0}+{1}+{2}&espv=2&biw=1745&bih=814&source=lnms&tbm=isch&sa=X&ei=j_tNVdvjN8OC7gaxvIE4&ved=0CAYQ_AUoAQ#tbs=isc:black%2Cic:trans%2Cisz:l&tbm=isch&q={0}+{1}+{2}",
            //        make, model.ModelName, model.YearFrom);

            Browser.Visit(url);

            Browser.FindCss("#rg_s > div:nth-child(1) > a > img").Click();

            Browser.ClickLink("View image");

            string imageUrl;
            try
            {
                imageUrl = Browser.FindXPath("//img")["src"];
            }
            catch (Exception)
            {
                Browser.GoBack();

                Browser.FindCss("#rg_s > div:nth-child(2) > a > img").Click();

                Browser.ClickLink("View image");

                imageUrl = Browser.FindXPath("//img")["src"];
            }
            
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(imageUrl, GetImageName(id));
            }
            Console.WriteLine("Successfully found image for {0} {1}", make, model.ModelName);
        }

        private static string GetImageName(long id)
        {
            return string.Format(
                @"C:\Users\ste_000\Documents\Visual Studio 2012\Projects\CarChooser\CarChooser\Content\CarImages\{0}.jpg",
                id);
        }
    }
}