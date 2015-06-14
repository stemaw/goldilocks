using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CarChooser.Data;
using CarChooser.Domain;
using Coypu;

namespace DataImporter
{
    public class ParkersRipper
    {
        private BrowserSession Browser { get; set; }

        public ParkersRipper(BrowserSession browser)
        {
            Browser = browser;
        }

        public int CurrentManufacturer { get; set; }
        public int CurrentModel { get; set; }
        protected static string ReviewDropdownId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlAreaOfSite_Control";
        protected static string ManufacturerDropdownId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlManufacturer_Control";
        protected static string ModelDropdownId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlModel_Control";
        protected static string SearchButtonId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_btnSearch";
        protected static string PriceRangePanelId = "ctl00_contentHolder_topFullWidthContent_ctl01_pnlNewPriceRange";


        public void RipParkers(int manufacturerIndex, int modelIndex, bool hasOptionGroups)
        {
            CurrentManufacturer = manufacturerIndex;
            CurrentModel = modelIndex;

            if (Browser.Location.AbsoluteUri != "http://www.parkers.co.uk/cars/reviews/")
            {
                Browser.Visit("http://www.parkers.co.uk/cars/reviews/");
            }

            Browser.HasContent("Find a Review");

            var manufacturerName = SelectManufacturer(manufacturerIndex);
            string modelName = null;

            try
            {
                modelName = SelectModel(modelIndex, hasOptionGroups);
            }
            catch (Exception)
            {
                try
                {
                    manufacturerName = SelectManufacturer(manufacturerIndex);
                    modelName = SelectModel(modelIndex, hasOptionGroups);
                }
                catch (Exception)
                {
                    manufacturerIndex++;
                    modelIndex = 0;
                    
                    RipParkers(manufacturerIndex, modelIndex, hasOptionGroups);
                    return;
                }

            }

            var modelAndYear = GetModelAndYear(modelName);

            var existingCar = new CarRepository().GetModel(manufacturerName, modelAndYear.ModelName, modelAndYear.YearFrom, modelAndYear.YearTo);
            if (existingCar != null)
            {
                modelIndex++;
                RipParkers(manufacturerIndex, modelIndex, true);
                Console.WriteLine("Already had {0} {1}", manufacturerName, modelName);
                return;
            }

            SelectReviewOption();

            if (Browser.FindId(ReviewDropdownId).SelectedOption != "Read review")
            {
                modelIndex++;
                RipParkers(manufacturerIndex, modelIndex, true);
                Console.WriteLine("No review for {0} {1}", manufacturerName, modelName);
                return;
            }

            DoSearch();

            var carRatings = GetCarRatings();

            var price = GetPrice();

            var dimensions = GetDimensions();

            var performance = GetPerformance();

            foreach (var perf in performance)
            {
                var car = new Car
                    {
                        Id = 0,
                        Manufacturer = new Manufacturer
                            {
                                Name = manufacturerName,
                                Score = 50
                            },
                        Model = modelAndYear.ModelName,
                        Name = perf.Derivative,
                        PerformanceScore = Convert.ToInt32(carRatings["Performance"]*20),
                        ReliabilityScore = Convert.ToInt32(carRatings["Reliability"]*20),
                        SizeScore = CalculateSizeScore(dimensions),
                        PriceScore = CalculatePriceScore(price),
                        PrestigeScore = 50,
                        Price = price,
                        Length = dimensions.Length,
                        Width = dimensions.Width,
                        Height = dimensions.Height,
                        Ratings = carRatings,
                        YearFrom = modelAndYear.YearFrom,
                        YearTo = modelAndYear.YearTo
                    };

                new CarRepository().Save(car);
                
                Console.WriteLine("Successfully ripped {0} {1} : Manufacturer {2} Model {3}", car.Manufacturer.Name, car.Model, CurrentManufacturer, CurrentModel);

            }

            modelIndex++;

            
            RipParkers(manufacturerIndex, modelIndex, hasOptionGroups);
        }



        private static int CalculatePriceScore(decimal price)
        {
            if (price > 100000) return 100;

            return (int)Math.Floor((price / 100000) * 100);
        }

        private static int CalculateSizeScore(Dimension dimensions)
        {
            var lengthScore = dimensions.Length > 6000 ? 100 : (int)Math.Floor((dimensions.Length / 6000M) * 100);
            var widthScore = dimensions.Length > 2200 ? 100 : (int)Math.Floor((dimensions.Length / 2200M) * 100);
            var heightScore = dimensions.Length > 2000 ? 100 : (int)Math.Floor((dimensions.Length / 2000M) * 100);

            return (int)Math.Floor((lengthScore + widthScore + heightScore) / 3M);
        }

        private IEnumerable<Performance> GetPerformance()
        {
            Browser.ClickLink("Facts & Figures");

            if (Browser.HasContent("performance"))
            {
                Browser.ClickLink("performance");
            }
            else if (Browser.HasContent("Performance"))
            {
                Browser.ClickLink("Performance");
            }
            else
            {
                Browser.Visit(Browser.Location.AbsoluteUri + "/performance");
            }

            var performanceTable = Browser.FindAllCss(".infoTable tbody tr:nth-child(2)");

            foreach (var derivative in performanceTable)
            {
                var derivativeName = derivative.FindCss("td:nth-child(1)").Text;
                var accelerationString = derivative.FindCss("td:nth-child(2)").Text;
                var topSpeedString = derivative.FindCss("td:nth-child(3)").Text;
                var powerString = derivative.FindCss("td:nth-child(4)").Text;

                var acceleration = accelerationString == "-" ? 10 : Convert.ToDecimal(accelerationString.TrimEnd("secs".ToCharArray()).Trim());
                var topSpeed = topSpeedString == "-" ? 150 : Convert.ToInt32(topSpeedString.TrimEnd("mph".ToCharArray()).Trim());
                var power = powerString == "-" ? 150 : Convert.ToInt32(powerString.TrimEnd("bhp".ToCharArray()).Trim());

                yield return new Performance()
                    {
                        Derivative = derivativeName,
                        Acceleration = acceleration,
                        TopSpeed = topSpeed,
                        Power = power
                    };
            }
        }

        private Dimension GetDimensions()
        {
            Browser.ClickLink("Facts & Figures");

            if (Browser.HasContent("dimensions"))
            {
                Browser.ClickLink("dimensions");
            }
            else if (Browser.HasContent("Dimensions"))
            {
                Browser.ClickLink("Dimensons");
            }
            else
            {
                Browser.Visit(Browser.Location.AbsoluteUri + "/dimensions");
            }

            var dimensionsTable = Browser.FindAllCss(".infoTable tbody tr:nth-child(2)").FirstOrDefault();

            if (dimensionsTable == null)
            {
                Thread.Sleep(5000);
                dimensionsTable = Browser.FindAllCss(".infoTable tbody tr:nth-child(2)").FirstOrDefault();
            }

            var lengthString = dimensionsTable.FindCss("td:nth-child(2)").Text;
            var widthString = dimensionsTable.FindCss("td:nth-child(3)").Text;
            var heightString = dimensionsTable.FindCss("td:nth-child(4)").Text;

            var length = lengthString == "-" ? 4000 : Convert.ToInt32(lengthString.TrimEnd("mm".ToCharArray()));
            var width = widthString == "-" ? 1800 : Convert.ToInt32(widthString.TrimEnd("mm".ToCharArray()));

            var height = heightString == "-" ? 1400 : Convert.ToInt32(heightString.TrimEnd("mm".ToCharArray()));

            Browser.GoBack();
            Browser.GoBack();

            return new Dimension() { Height = height, Width = width, Length = length };
        }

        private decimal GetPrice()
        {
            var prices = Browser.FindId(PriceRangePanelId).FindCss("p:nth-child(2)").Text;
            return ExtractPrice(prices);
        }

        private Dictionary<string, decimal> GetCarRatings()
        {
            Browser.HasContent("Parkers Rating");

            var ratingsPanel = Browser.FindId("pnlReviewRatings");

            var ratings = ratingsPanel.FindAllXPath("//div[@class='ratingRow']");

            var carRatings = new Dictionary<string, decimal>();

            foreach (var rating in ratings)
            {
                var type = rating.FindXPath("p[@class='title']/a").Text;
                var score = 0M;
                try
                {
                    score = StripScore(rating.FindXPath("div/div[@class='rating']/div").Text);
                }
                catch (Exception)
                {
                }

                carRatings.Add(type, score);
            }
            return carRatings;
        }

        private void DoSearch()
        {
            var script = string.Format(@"$('#{0}').trigger('click');", SearchButtonId);

            Browser.ExecuteScript(script);
        }

        private void SelectReviewOption()
        {
            WaitForDropdownPopulation(ReviewDropdownId);

            var script = string.Format(@"$('#{0}>option:eq(1)').attr('selected', true);$('#{0}').trigger('change');", ReviewDropdownId);

            Browser.ExecuteScript(script);
        }

        private string SelectModel(int modelIndex, bool hasOptionGroups)
        {
            WaitForDropdownPopulation(ModelDropdownId);

            string script;
            if (hasOptionGroups)
            {
                script = string.Format(
                    @"$('#{0}>optgroup>option:eq({1})').attr('selected', true);$('#{0}').trigger('change');",
                    ModelDropdownId, modelIndex);
            }
            else
            {
                script = string.Format(
                    @"$('#{0}>option:eq({1})').attr('selected', true);$('#{0}').trigger('change');",
                    ModelDropdownId, modelIndex);
            }

            Browser.ExecuteScript(script);

            var modelName = Browser.FindId(ModelDropdownId).SelectedOption;

            if (modelName == "Select a model") throw new Exception("Model not selected");

            return modelName;
        }

        private void WaitForDropdownPopulation(string dropdownId)
        {
            while (Browser.FindId(dropdownId).FindAllXPath("//option").Count() < 2) Thread.Sleep(30);
        }

        private string SelectManufacturer(int manufacturerIndex)
        {
            if (Browser.FindId(ManufacturerDropdownId).FindAllXPath("//option").Count() < manufacturerIndex)
            {
                throw new Exception("No more manufacturers");
            }

            var script = string.Format(@"$('#{0}>option:eq({1})').attr('selected', true);$('#{0}').trigger('change');",
                                       ManufacturerDropdownId, manufacturerIndex);

            Browser.ExecuteScript(script);

            var manufacturerName = Browser.FindId(ManufacturerDropdownId).SelectedOption;

            return manufacturerName;
        }

        private static decimal ExtractPrice(string priceRange)
        {
            var firstPrice = priceRange.Split('-')[0];
            return decimal.Parse(firstPrice.TrimStart('£').Replace(",", ""));
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
}