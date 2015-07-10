using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CarChooser.Data;
using CarChooser.Web.Mappers;

namespace SiteMapGenerator
{
    class Program
    {
        //        <urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
        //  <url>
        //    <loc>http://www.goldilocker.co.uk/</loc>
        //    <lastmod>2015-06-16</lastmod>
        //    <changefreq>monthly</changefreq>
        //    <priority>0.8</priority>
        //  </url>
        //</urlset>
const string xmlns = @"http://www.sitemaps.org/schemas/sitemap/0.9";
        static void Main(string[] args)
        {
            var mapper = new CarMapper(new OfficialCarRatingsMapper());
            
            var cars = new CarRepository().AllCars();

            var doc = new XmlDocument();
            var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);
            
            var urlSetNode = doc.CreateElement(string.Empty, "urlset", xmlns);
            doc.AppendChild(urlSetNode);

            var homeNode = doc.CreateElement(string.Empty, "url", xmlns);
            urlSetNode.AppendChild(homeNode);

            AddTextNode("loc", "http://www.goldilocker.co.uk/", homeNode, doc);
            AddTextNode("lastmod", DateTime.Today.ToString("yyyy-MM-dd"), homeNode, doc);
            AddTextNode("changefreq", "monthly", homeNode, doc);
            AddTextNode("priority", "1", homeNode, doc);

            foreach (var car in cars)
            {
                var carvm = mapper.Map(car);
 
                var urlNode = doc.CreateElement(string.Empty, "url", xmlns);
                urlSetNode.AppendChild(urlNode);

                AddTextNode("loc", "http://www.goldilocker.co.uk/" + carvm.UrlName, urlNode, doc);
                AddTextNode("lastmod", DateTime.Today.ToString("yyyy-MM-dd"), urlNode, doc);
                AddTextNode("changefreq", "monthly", urlNode, doc);
                AddTextNode("priority", "0.2", urlNode, doc);
                
                //var element3 = doc.CreateElement(string.Empty, "loc", string.Empty);
                //var loc = doc.CreateTextNode("http://www.goldilocker.co.uk/" + carvm.UrlName);
                //element3.AppendChild(loc);
                //element2.AppendChild(element3);

                //var element4 = doc.CreateElement(string.Empty, "lastmod", string.Empty);
                //var lastmod = doc.CreateTextNode();
                //element4.AppendChild(lastmod);
                //element2.AppendChild(element4);

                //var element5 = doc.CreateElement(string.Empty, "changefreq", string.Empty);
                //var change = doc.CreateTextNode("monthly");
                //element5.AppendChild(change);
                //element2.AppendChild(element5);

                //var element6 = doc.CreateElement(string.Empty, "priority", string.Empty);
                //var priority = doc.CreateTextNode("0.1");
                //element6.AppendChild(priority);
                //element2.AppendChild(element6);
            }

            doc.Save("sitemap.xml");
        }

        private static void AddTextNode(string name, string value, XmlNode parent, XmlDocument doc)
        {
            var element = doc.CreateElement(string.Empty, name, xmlns);
            var textNode = doc.CreateTextNode(value);
            element.AppendChild(textNode);
            parent.AppendChild(element);
        }
    }
}
