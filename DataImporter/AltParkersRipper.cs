using System;
using System.Collections.Generic;
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
    public class AltParkersRipper
    {
        private readonly CarRepository _carRepository = new CarRepository();
        private readonly Prestige _prestige = new Prestige();
        private readonly ReliabilityIndex _reliability = new ReliabilityIndex();

        protected static string ManufacturerDropdownId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlManufacturer_Control";
        const string ParkersUrl = "http://www.parkers.co.uk/cars/reviews/";
        const string ModelSearchUrl = "http://www.parkers.co.uk/HttpHandlers/DataStrategyService.ashx?mthd=manuf-models-indented-pairs";
        const string GetSectionsUrl = "http://www.parkers.co.uk/HttpHandlers/RedirectUrlService.ashx";

        private void Pause()
        {
            var random = new Random();
            var delay = random.Next(100,1000);
            Thread.Sleep(delay);
        }

        public void RipParkers()
        {
            var doc = GetHtml(ParkersUrl);

            var manufacturersElement = doc.GetElementbyId(ManufacturerDropdownId);

            var options = manufacturersElement.SelectNodes("//option");

            var manufacturers = options.Select(o => new { Id = o.GetAttributeValue("value", 0), Name = o.InnerText });

            foreach (var manufacturer in manufacturers.Skip(9))
            {
                var modelsResponse = GetWebPostResponse(ModelSearchUrl,
                        string.Format("sourceId=ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlManufacturer_Control&targetId=ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlModel_Control&filterName=manuf&manuf={0}",
                            manufacturer.Id));

                var fullModelsResponse = JsonConvert.DeserializeObject<ModelsResponse>(modelsResponse);

                var models = fullModelsResponse.Models.Where(m => !m.Id.StartsWith("###"));

                foreach (var model in models)
                {
                    Pause();

                    var sectionResponse = GetWebPostResponse(GetSectionsUrl,
                                           string.Format(
                                               "sourceId=ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlModel_Control&targetId=ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlAreaOfSite_Control&filterName=model&model={0}&section=reviews",
                                               model.Id));

                    var sectionsResponse = JsonConvert.DeserializeObject<SectionResponse>(sectionResponse);

                    var reviewSection = sectionsResponse.Sections.FirstOrDefault(s => s.Name == "Read review");

                    if (reviewSection == null) continue;

                    reviewSection.Url = "http://www.parkers.co.uk" + reviewSection.Url;

                    var modelAndYear = GetModelAndYear(model.Name);

                    var factsSection = sectionsResponse.Sections.FirstOrDefault(s => s.Name == "Facts and Figures");

                    if (factsSection == null) continue;

                    factsSection.Url = "http://www.parkers.co.uk" + factsSection.Url;

                    var factsPage = GetHtml(factsSection.Url);

                    var derivatives = factsPage.DocumentNode.SelectNodes("//table[@class = 'infoTable']//tr");

                    foreach (var derivative in derivatives)
                    {
                        Pause();

                        if (derivative.FirstChild.Name == "th") continue;

                        var derivativeName = derivative.SelectSingleNode("td[1]").InnerText.Trim();

                        var car = _carRepository.GetCar(manufacturer.Name, modelAndYear.ModelName,
                                                        modelAndYear.YearFrom, modelAndYear.YearTo,
                                                        derivativeName) ??
                                  BuildNewCar(modelAndYear, manufacturer.Name, derivativeName);

                        GetReviewRatings(car, reviewSection);

                        GetFacts(derivative, car);

                        car.SizeScore = CalculateSizeScore(car);
                        car.PriceScore = CalculatePriceScore(car.Price);

                        _carRepository.Save(car);

                        Console.WriteLine("Successfully ripped {0} {1} {2}", car.Manufacturer.Name,
                                          car.Model, car.Name);
                    }
                }
            }
        }

        private static void GetFacts(HtmlNode derivative, Car car)
        {
            var factsDetailUrl =
                derivative.SelectSingleNode("td/p/a[text() = 'More Info']")
                          .GetAttributeValue("href", "null");

            factsDetailUrl = "http://www.parkers.co.uk" + factsDetailUrl;

            car.FactsPage = factsDetailUrl;

            var facts = GetHtml(factsDetailUrl);

            var price = GetElement("List Price", facts);
            if (!string.IsNullOrEmpty(price))
                car.Price = Convert.ToDecimal(price.Replace("£", "").Replace(",", ""));

            var mpg = GetElement("MPG", facts);

            if (!string.IsNullOrEmpty(mpg))
                car.Mpg = Convert.ToInt32(mpg.Replace(" mpg", ""));

            var engineSize = GetElement("Engine Size", facts);

            if (!string.IsNullOrEmpty(engineSize))
                car.EngineSize = Convert.ToInt32(engineSize.Replace(" cc", ""));

            var luggageCapacity = GetElement("Luggage Capacity", facts);

            if (!string.IsNullOrEmpty(luggageCapacity))
                car.LuggageCapacity = Convert.ToInt32(luggageCapacity.Replace(" litres", ""));

            var transmission = GetElement("Transmission", facts);

            if (!string.IsNullOrEmpty(transmission))
                car.Transmission = transmission;

            var insuranceGroup = GetElement("Insurance Group", facts);

            if (!string.IsNullOrEmpty(insuranceGroup))
                car.InsuranceGroup = Convert.ToInt32(insuranceGroup);

            var weight = GetElement("Weight", facts);

            if (!string.IsNullOrEmpty(weight))
                car.Weight = Convert.ToInt32(weight.Replace(" kg", ""));

            var cylinders = GetElement("Cylinders", facts);

            if (!string.IsNullOrEmpty(cylinders))
                car.Cylinders = Convert.ToInt32(cylinders);

            try
            {
                var emmissions = facts.DocumentNode.SelectSingleNode("//*[contains(text(),'g/km')]").InnerText;

                if (!string.IsNullOrEmpty(emmissions))
                    car.Emissions = Convert.ToInt32(emmissions.Replace(" g/km", ""));
            }
            catch (Exception)
            {
                Console.WriteLine("No emmissions found for {0}", car.Name);
            }
            
            var ved = GetElement("VED Band", facts);

            if (!string.IsNullOrEmpty(ved))
                car.VEDBand = ved.Trim();

            var torque = GetElement("Torque", facts);

            if (!string.IsNullOrEmpty(torque) && torque.Contains("Nm"))
                car.Torque = Convert.ToInt32(torque.Split('N')[0].Trim());

            var euroEmissions = GetElement("Euro Emissions Standard", facts);

            if (!string.IsNullOrEmpty(euroEmissions))
                car.EuroEmissionsStandard = Convert.ToInt32(euroEmissions);

            var length = GetElement("Length", facts);

            if (!string.IsNullOrEmpty(length))
                car.Length = Convert.ToInt32(length.Replace("mm", "").Trim());

            var width = GetElement("Width", facts);

            if (!string.IsNullOrEmpty(width))
                car.Width = Convert.ToInt32(width.Replace("mm", "").Trim());

            var height = GetElement("Height", facts);

            if (!string.IsNullOrEmpty(height))
                car.Height = Convert.ToInt32(height.Replace("mm", "").Trim());

            var acceleration = GetElement("0-60 mph", facts);

            if (!string.IsNullOrEmpty(acceleration))
                car.Acceleration = Convert.ToDecimal(acceleration.Replace("secs", "").Trim());

            var topSpeed = GetElement("Top Speed", facts);

            if (!string.IsNullOrEmpty(topSpeed))
                car.TopSpeed = Convert.ToInt32(topSpeed.Replace("mph", "").Trim());

            var power = GetElement("Power Output", facts);

            if (!string.IsNullOrEmpty(power))
                car.Power = Convert.ToInt32(power.Replace("bhp", "").Trim());
        }

        private void GetReviewRatings(Car car, Section reviewSection)
        {
            car.ReviewPage = reviewSection.Url;

            if (car.Ratings == null || !car.Ratings.Any())
            {
                var reviewPage = GetHtml(reviewSection.Url);

                car.Ratings = GetCarRatings(reviewPage);
            }
        }

        private Car BuildNewCar(ModelWithYear modelAndYear, string manufacturer, string derivative)
        {
            var otherModels = _carRepository.GetModel(manufacturer, modelAndYear.ModelName, modelAndYear.YearFrom,
                                                      modelAndYear.YearTo);

            var modelId = otherModels != null ? otherModels.ModelId : _carRepository.GetNextModelId();
            var sales = otherModels != null ? otherModels.Sales : 0;

            var derivativeId = _carRepository.GetNextDerivativeId();

            return new Car()
                {
                    ModelId = modelId,
                    Id = derivativeId,
                    Sales = sales,
                    Model = modelAndYear.ModelName,
                    YearFrom = modelAndYear.YearFrom,
                    YearTo = modelAndYear.YearTo,
                    Name = derivative,
                    Manufacturer = new Manufacturer()
                        {
                            Name = manufacturer,
                            PrestigeIndex =
                                _prestige.GetScores()
                                         .FirstOrDefault(m => m.Key == manufacturer)
                                         .Value,
                            ReliabilityIndex =
                                _reliability.GetScores()
                                            .FirstOrDefault(m => m.Key == manufacturer)
                                            .Value
                        }
                };

        }

        private static string GetElement(string elementName, HtmlDocument facts)
        {
            try
            {
                var result = facts.DocumentNode.SelectSingleNode(string.Format("//th[text()='{0}']/following::td[1]/p", elementName)).InnerText;

                return result.Trim() == "-" ? string.Empty : result.Trim();
            }
            catch (Exception)
            {
                return string.Empty;
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
        private static string GetWebPostResponse(string url, string postData)
        {
            var request = WebRequest.Create(url);

            ((HttpWebRequest)request).UserAgent =
                "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36";

            ((HttpWebRequest)request).Referer = "http://www.parkers.co.uk/cars/reviews/";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            ((HttpWebRequest)request).Accept = "application/json, text/javascript, */*; q=0.01";
            ((HttpWebRequest)request).Host = "www.parkers.co.uk";

            request.Method = "POST";

            var byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            var dataStream = request.GetRequestStream();


            request.Headers.Add("Cookie", "__sonar=10447369641459741403; _bs974da1db735a7522ae310107d0e069ab=1; _bs91936b7ed6df925de19e72cf82e1c2a3=1; _bsea50c48e0d281fca94526b0f02d82861=1; _bs4c9288285cbc95cb0e66ee37f41732b7=1; nmfp_evo5_PARKERS_popin=bmw; evo5_popin_instance=3616223; BSSID=c3ZyX3czX3d3d19wYXJrZXJzX2NvX3VrX21zaWlzcHJkZnJlZGEwNg==; __utmt_bauertracker=1; nmfirstparty=wmhyryx8pnysv; __utma=66691249.1171374821.1431171459.1433758498.1433760658.28; __utmb=66691249.2.10.1433760658; __utmc=66691249; __utmz=66691249.1431332827.6.2.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided); _s3_id.59.d3da=c349842388d76ff0.1433233318.3.1433760660.1433244106.; _s3_id.59.f493=af7959f638e0e7a4.1433312816.5.1433760660.1433758498.; _s3_ses.59.f493=*; nmfp_evo5_PARKERS=wmhyryx8pnysv|y8XSSXLlbbfm8ndU/27GQ0tqhJrxrOKYmD8BJu1/Bh20lLZKRjONyujiYIKcZgQ6Dx6h60plAdkDGPwwAuSDcc6kY5vQ9Jn9k2CDFNl6O7BdMltUooxR/YnA9+JqFNIWg5rqjkNPaqBONVCGLimIKUgCGIMbjmmQAVC/5oJcyVmAgg/zztrKJCXCZD7kq1c1tWXpj0kNuPnsDA8jqw8pWIRp6HmhxFvWuBGhDMH5eXkdLF+ZtbnUvA+q7wg0Y3C7DO8sr9w2YmDlNA6Mnr3TOEbQs3ns6FMO0Jka7DPKFVUKUvLiy3Mvtc+KXXO51cZsfP4Aszz1WlcxV/69LVJENSVKxlOzjTec7FG9511tzeMIKpVQ9zP+8rzkRUCGn+jcVeoD5tUwA3V9DnQUqxG+Nttq1mxGngf3sdchNMmMJCOF9uzv/4ZHVBOgPF5LJF0XyvRqNj2Vl8J94uZYj0HBAm8keKUvPERIsacr7Eg8MEJz1Y4aia9HQ/nGtSSgdfMgZXov+VPO69mlEd3byrS3nHcoKBgEY5cC5J60wFLUVumSgBUYQ2+WAGYMqGDHFglzgV1RAqkRtJ7muufBYYt8wQ==");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            var response = request.GetResponse();
            dataStream = response.GetResponseStream();
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

        private static int CalculatePriceScore(decimal price)
        {
            if (price > 100000) return 100;

            return (int)Math.Floor((price / 100000) * 100);
        }

        private static int CalculateSizeScore(Car car)
        {
            var lengthScore = car.Length > 6000 ? 100 : (int)Math.Floor((car.Length / 6000M) * 100);
            var widthScore = car.Width > 2200 ? 100 : (int)Math.Floor((car.Width / 2200M) * 100);
            var heightScore = car.Height > 2000 ? 100 : (int)Math.Floor((car.Height / 2000M) * 100);

            return (int)Math.Floor((lengthScore + widthScore + heightScore) / 3M);
        }

        private Dictionary<string, decimal> GetCarRatings(HtmlDocument reviewPage)
        {
            var ratingsPanel = reviewPage.GetElementbyId("pnlReviewRatings");

            var ratings = ratingsPanel.SelectNodes("//div[@class='ratingRow']");

            var carRatings = new Dictionary<string, decimal>();

            foreach (var rating in ratings)
            {
                var type = rating.SelectSingleNode("p[@class='title']/a").InnerText;
                var score = 0M;
                try
                {
                    score = StripScore(rating.SelectSingleNode("div/div[@class='rating']/div").InnerText);
                }
                catch (Exception)
                {
                }

                carRatings.Add(type, score);
            }
            return carRatings;
        }

        private static decimal StripScore(string text)
        {
            return decimal.Parse(text.Replace(" out of 5", ""));
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

    public class ModelsResponse
    {
        [JsonProperty("items")]
        public IEnumerable<Model> Models { get; set; }
    }

    public class Model
    {
        [JsonProperty("value")]
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Name { get; set; }
    }

    public class SectionResponse
    {
        [JsonProperty("items")]
        public IEnumerable<Section> Sections { get; set; }
    }

    public class Section
    {
        [JsonProperty("value")]
        public string Url { get; set; }

        [JsonProperty("text")]
        public string Name { get; set; }
    }
}