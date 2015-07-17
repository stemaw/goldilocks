using System;
using System.Xml;
using CarChooser.Data;
using CarChooser.Web.Mappers;

namespace SiteMapGenerator
{
    class Program
    {
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

            var quizNode = doc.CreateElement(string.Empty, "url", xmlns);
            urlSetNode.AppendChild(quizNode);

            AddTextNode("loc", "http://www.goldilocker.co.uk/Quiz", quizNode, doc);
            AddTextNode("lastmod", DateTime.Today.ToString("yyyy-MM-dd"), quizNode, doc);
            AddTextNode("changefreq", "monthly", quizNode, doc);
            AddTextNode("priority", "0.9", quizNode, doc);

            //foreach (var car in cars)
            //{
            //    var carvm = mapper.Map(car);
 
            //    var urlNode = doc.CreateElement(string.Empty, "url", xmlns);
            //    urlSetNode.AppendChild(urlNode);

            //    AddTextNode("loc", "http://www.goldilocker.co.uk/" + carvm.UrlName, urlNode, doc);
            //    AddTextNode("lastmod", DateTime.Today.ToString("yyyy-MM-dd"), urlNode, doc);
            //    AddTextNode("changefreq", "monthly", urlNode, doc);
            //    AddTextNode("priority", "0.2", urlNode, doc);
            //}

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
