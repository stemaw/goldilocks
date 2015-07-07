using System;using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using CarChooser.Data;
using CarChooser.Domain;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace DataImporter
{
    public class ParkersPriceRipper
    {
        private readonly CarRepository _carRepository = new CarRepository();

        protected static string ManufacturerDropdownId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlManufacturer_Control";
        const string ParkersUrl = "http://www.parkers.co.uk/cars/reviews/";
        private void Pause()
        {
            var random = new Random();
            var delay = random.Next(100, 1000);
            Thread.Sleep(delay);
        }

        public void RipParkers()
        {
            var allCars = _carRepository.AllCars().Where(c => c.Price == 0);

            var models = from car in allCars
                         group car by car.ModelId
                             into g
                             select new { ModelId = g.Key, Derivates = g.ToList() };

            foreach (var model in models)
            {
                var firstMatch = allCars.First(c => c.ModelId == model.ModelId);
                
                Pause();
                
                var doc = GetHtml(firstMatch.ReviewPage + "facts-figures/");

                foreach (var derivative in model.Derivates)
                {
                    var matchingData =
                        doc.DocumentNode.SelectNodes(
                            string.Format("//table[@class = 'infoTable']//tr/td/p[text() = '{0}']",
                                          derivative.Name.Replace("'", "&quot;")))
                        ;

                    if (matchingData == null || !matchingData.Any()) continue;

                    var price =
                        matchingData.First().ParentNode.ParentNode.SelectNodes("td[2]")
                                    .First()
                                    .InnerText.Replace("£", "")
                                    .Replace(",", "");

                    decimal convertedPrice;
                    if (decimal.TryParse(price, out convertedPrice))
                    {
                        derivative.Price = Convert.ToDecimal(price);

                        _carRepository.Save(derivative);

                        Console.WriteLine("Successfully priced {0} {1} {2} £{3}", derivative.Manufacturer.Name,
                                          derivative.Model, derivative.Name, derivative.Price);
                    }
                }
            }
        }

        private static HtmlDocument GetHtml(string url)
        {
            var source = GetWebGetResponse(url);

            var doc = new HtmlDocument();
            HtmlNode.ElementsFlags.Remove("option");
            doc.LoadHtml(source);

            return doc;
        }

        private static string GetWebGetResponse(string url)
        {
            var request = WebRequest.Create(url);

            ((HttpWebRequest)request).UserAgent =
                "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36";

            ((HttpWebRequest)request).Referer = "http://www.parkers.co.uk/cars/reviews/";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            ((HttpWebRequest)request).Accept = "application/json, text/javascript, */*; q=0.01";

            request.Method = "GET";

            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        public Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public ModelWithYear GetModelAndYear(string model)
        {
            if (!model.Contains('('))
            {
                return new ModelWithYear() { ModelName = model, YearFrom = 0, YearTo = 0 };
            }

            var modelName = model.Split('(')[0].Trim();

            var firstBracket = model.IndexOf('(') + 1;
            var lastBracket = model.LastIndexOf(')');
            var modelYearString = model.Substring(firstBracket, lastBracket - firstBracket);

            var yearFrom = int.Parse(modelYearString.Substring(0, 2));

            yearFrom = BuildFullYear(yearFrom);

            var yearTo = 0;

            if (modelYearString.Contains('-'))
            {
                var dash = modelYearString.IndexOf('-') + 1;
                yearTo = int.Parse(modelYearString.Substring(dash, modelYearString.Length - dash).Trim());
                yearTo = BuildFullYear(yearTo);
            }

            return new ModelWithYear
            {
                ModelName = modelName,
                YearFrom = yearFrom,
                YearTo = yearTo
            };
        }

        private static int BuildFullYear(int yearFrom)
        {
            if (yearFrom > 15)
            {
                yearFrom = 1900 + yearFrom;
            }
            else
            {
                yearFrom = 2000 + yearFrom;
            }
            return yearFrom;
        }
    }

}