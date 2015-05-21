using System;
using System.Linq;
using System.Threading;
using CarChooser.Data;
using CarChooser.Domain;
using Coypu;

namespace DataImporter
{
    public class InsuranceGroup
    {
        private readonly BrowserSession _browser;

        private const string ManufacturerDropdown = "ctl00_contentHolder_topFullWidthContent_ucInsuranceGroupCriteria_ddlManufacturer_Control";

        private const string ModelDropdown = "ctl00_contentHolder_topFullWidthContent_ucInsuranceGroupCriteria_ddlModel_Control";

        private const string FindButton = "ctl00_contentHolder_topFullWidthContent_ucInsuranceGroupCriteria_btnFindGroup";

        private readonly CarRepository _carRepo = new CarRepository();

        public long CurrentCar;

        public InsuranceGroup(BrowserSession browser)
        {
            _browser = browser;
        }

        public void RipInsuranceGroup(int startingId)
        {
            const string url = "http://www.parkers.co.uk/cars/insurance/car-insurance-groups/";

            _browser.Visit(url);

            CurrentCar = startingId;

            var cars = _carRepo.AllCars().Skip(startingId);

            foreach (var car in cars)
            {
                try
                {
                    DoSearch(car);

                    if (GetGroups(car))
                    {
                        UpdateCar(car);
                    }
                    else
                    {
                        Console.WriteLine("Failed to get groups for {0}", car.ToString());
                    }

                    _browser.GoBack();

                    Console.WriteLine(CurrentCar++);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed somewhere for {0} {1}{1} {2}", car, Environment.NewLine, ex);
                }

            }
        }

        private void UpdateCar(Car car)
        {
            _carRepo.UpdateInsuranceGroup(car);
        }

        private bool GetGroups(Car car)
        {
            var tables = _browser.FindAllCss(".infoTable");
            int attempts = 0;
            while (!tables.Any() || attempts > 10)
            {
                attempts++;
                Thread.Sleep(1000);
                tables = _browser.FindAllCss(".infoTable tbody tr:nth-child(2)");
            }

            bool updated = false;

            foreach (var table in tables)
            {
                var performanceTable = table.FindAllCss("tbody tr:nth-child(2)");
                
                foreach (var derivative in performanceTable)
                {
                    var derivativeName = derivative.FindCss("td:nth-child(1)").Text;
                    var insuranceGroupText = derivative.FindCss("td:nth-child(2)").Text;

                    int insuranceGroup = 0;

                    int.TryParse(insuranceGroupText, out insuranceGroup);

                    var matchingDerivative = car.PerformanceFigures.FirstOrDefault(p => p.Derivative == derivativeName);

                    if (matchingDerivative != null)
                    {
                        matchingDerivative.InsuranceGroup = insuranceGroup;
                        Console.WriteLine("{0} Ins {1}", derivativeName, insuranceGroup);
                        updated = true;
                    }
                }
            }
            return updated;
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
