using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CarChooser.Data;
using CarChooser.Domain;
using Coypu;
using Coypu.Drivers.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DataImporter
{
    public class MPGRipper
    {
        private readonly BrowserSession _browser;
        protected static string ReviewDropdownId = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlAreaOfSite_Control";
        protected static string ManufacturerDropdown = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlManufacturer_Control";
        protected static string ModelDropdown = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_ddlModel_Control";
        protected static string FindButton = @"ctl00_contentHolder_topFullWidthContent_ctlQuickfindMiniManufacturerGrid_ctlManufacturerModelAreaDropdowns_btnSearch";
        private readonly CarRepository _carRepo;
        private string _windowTitle;


        public MPGRipper(BrowserSession browser)
        {
            _browser = browser;
            _carRepo = new CarRepository();
        }

        public void Rip()
        {
            var cars = _carRepo.AllCars().Where(c => c.Mpg == 0);

            var models = from car in cars
                         group car by car.ModelId
                             into g
                             select new { ModelId = g.Key, Derivates = g.ToList() };

            Console.WriteLine("{0} models without mpg etc..", models.Count());
            NavigateToReviews();

            _browser.HasContent("Find a Review");

            foreach (var model in models)
            {
                try
                {
                    var firstMatch = cars.First(c => c.ModelId == model.ModelId);

                    DoSearch(firstMatch);

                    _windowTitle = _browser.Title;
                    GetMpgs(cars);

                    _browser.GoBack();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed somewhere for {0} {1}{1} {2}", model, Environment.NewLine, ex);
                    NavigateToReviews();
                }
            }
        }

        private void NavigateToReviews()
        {
            if (_browser.Location.AbsoluteUri != "http://www.parkers.co.uk/cars/reviews/")
            {
                _browser.Visit("http://www.parkers.co.uk/cars/reviews/");
            }
        }

        private string GetElement(string elementName, BrowserWindow tab)
        {
            try
            {
                var result = tab.FindXPath(string.Format("//th[text()='{0}']/following::td[1]/p", elementName), new Options() { Timeout = TimeSpan.FromMilliseconds(100) }).Text;

                return result.Trim() == "-" ? string.Empty : result.Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void GetMpgs(IEnumerable<Car> derivatives)
        {
            var tables = _browser.FindAllCss(".infoTable");
            int attempts = 0;
            while (!tables.Any() || attempts > 10)
            {
                attempts++;
                Thread.Sleep(1000);
                tables = _browser.FindAllCss(".infoTable");
            }

            foreach (var table in tables)
            {
                var derivativeTable = table.FindAllCss("tbody tr:not(:first-child) ");

                foreach (var derivative in derivativeTable)
                {
                    ScrapeDerivative(derivatives, derivative);
                }
                //Parallel.ForEach(derivativeTable, d => ScrapeDerivative(derivatives, d));
            }
        }

        private void ScrapeDerivative(IEnumerable<Car> derivatives, SnapshotElementScope derivative)
        {


            var derivativeName = derivative.FindCss("td:nth-child(1)").Text;

            var matchingDerivative = derivatives.FirstOrDefault(d => d.Name.ToLower().Trim() == derivativeName.ToLower().Trim());

            if (matchingDerivative != null)
            {
                var tab = OpenTab("More Info", derivative, matchingDerivative.Id.ToString());

                var price = GetElement("List Price", tab);
                if (!string.IsNullOrEmpty(price))
                    matchingDerivative.Price = Convert.ToDecimal(price.Replace("£", "").Replace(",", ""));

                var mpg = GetElement("MPG", tab);

                if (!string.IsNullOrEmpty(mpg))
                    matchingDerivative.Mpg = Convert.ToInt32(mpg.Replace(" mpg", ""));

                var engineSize = GetElement("Engine Size", tab);

                if (!string.IsNullOrEmpty(engineSize))
                    matchingDerivative.EngineSize = Convert.ToInt32(engineSize.Replace(" cc", ""));

                var luggageCapacity = GetElement("Luggage Capacity", tab);

                if (!string.IsNullOrEmpty(luggageCapacity))
                    matchingDerivative.LuggageCapacity = Convert.ToInt32(luggageCapacity.Replace(" litres", ""));

                var transmission = GetElement("Transmission", tab);

                if (!string.IsNullOrEmpty(transmission))
                    matchingDerivative.Transmission = transmission;

                var insuranceGroup = GetElement("Insurance Group", tab);

                if (!string.IsNullOrEmpty(insuranceGroup))
                    matchingDerivative.InsuranceGroup = Convert.ToInt32(insuranceGroup);

                var weight = GetElement("Weight", tab);

                if (!string.IsNullOrEmpty(weight))
                    matchingDerivative.Weight = Convert.ToInt32(weight.Replace(" kg", ""));

                var cylinders = GetElement("Cylinders", tab);

                if (!string.IsNullOrEmpty(cylinders))
                    matchingDerivative.Cylinders = Convert.ToInt32(cylinders);

                var emmissions = tab.FindXPath("//*[contains(text(),'g/km')]").Text;

                if (!string.IsNullOrEmpty(emmissions))
                    matchingDerivative.Emissions = Convert.ToInt32(emmissions.Replace(" g/km", ""));

                var ved = GetElement("VED Band", tab);

                if (!string.IsNullOrEmpty(ved))
                    matchingDerivative.VEDBand = ved.Trim();

                var torque = GetElement("Torque", tab);

                if (!string.IsNullOrEmpty(torque) && torque.Contains("Nm"))
                    matchingDerivative.Torque = Convert.ToInt32(torque.Split('N')[0].Trim());

                var euroEmissions = GetElement("Euro Emissions Standard", tab);

                if (!string.IsNullOrEmpty(euroEmissions))
                    matchingDerivative.EuroEmissionsStandard = Convert.ToInt32(euroEmissions);

                var driver = (IWebDriver) _browser.Driver.Native;
                driver.Close();
                SwitchToMainWindow();

                UpdateCar(matchingDerivative);

                Console.WriteLine("{0} Mpg {1}", derivativeName, matchingDerivative.Mpg);
            }
            else
            {
                Console.WriteLine("Failed to find match for {0}", derivativeName);
            }
        }

        public void SwitchToMainWindow()
        {
            var driver = (IWebDriver)_browser.Driver.Native;
           // string mainWindowHandle = null;
            
            foreach (var handle in driver.WindowHandles)
            {
                driver.SwitchTo().Window(handle);
                if (driver.Title == _windowTitle)
                {
                    return;
                }
            }

            //driver.SwitchTo().Window(mainWindowHandle);

            throw new ArgumentException("Unable to find window with condition");
        }

       
        private BrowserWindow OpenTab(string linkText, SnapshotElementScope derivative, string name)
        {
            var url = derivative.FindLink(linkText)["href"];
            _browser.ExecuteScript(string.Format("window.open('{0}', '{1}'); ", url, name));
            return _browser.FindWindow(name);
        }

        private void UpdateCar(Car car)
        {
            _carRepo.UpdateMpg(car);
        }

        private void SelectReviewOption()
        {
            WaitForDropdownPopulation(ReviewDropdownId);

            var script = string.Format(@"$('#{0}>option:contains(Facts and Figures)').attr('selected', true);$('#{0}').trigger('change');", ReviewDropdownId);

            _browser.ExecuteScript(script);
        }

        private void DoSearch(Car car)
        {
            WaitForDropdownPopulation(ManufacturerDropdown);

            var script = string.Format(@"$('#{0} option:contains({1})').attr('selected', 'selected').trigger('change');",
                                       ManufacturerDropdown, car.Manufacturer.Name);

            _browser.ExecuteScript(script);

            WaitForDropdownPopulation(ModelDropdown);

            script = string.Format(@"$('#{0} option:contains({1})').attr('selected', 'selected').trigger('change');",
                                       ModelDropdown, BuildModelString(car));

            _browser.ExecuteScript(script);

            var modelName = _browser.FindId(ModelDropdown).SelectedOption;

            if (modelName == "Select a model") throw new Exception("Model not selected");

            SelectReviewOption();

            _browser.FindId(FindButton).Click();
        }
        private void WaitForDropdownPopulation(string dropdownId)
        {
            while (_browser.FindId(dropdownId).FindAllXPath("//option").Count() < 2) Thread.Sleep(30);
        }

        private string BuildModelString(Car car)
        {
            return new ModelWithYear { ModelName = car.Model, YearFrom = car.YearFrom, YearTo = car.YearTo }.ToString();
        }
    }
}
