using System;
using System.Collections.Generic;
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

        public long CurrentModel;

        public InsuranceGroup(BrowserSession browser)
        {
            _browser = browser;
        }

        public void RipInsuranceGroup(int startingId)
        {
            const string url = "http://www.parkers.co.uk/cars/insurance/car-insurance-groups/";

            _browser.Visit(url);
            
            CurrentModel = startingId;

            var cars = _carRepo.AllCars().Where(c => c.InsuranceGroup == 0).ToList();

            var models = from car in cars
                         group car by car.ModelId
                         into g
                         select new {ModelId = g.Key, Derivates = g.ToList()};

            foreach (var model in models)
            {
                try
                {
                    var firstMatch = cars.First(c => c.ModelId == model.ModelId);

                    DoSearch(firstMatch);

                    GetGroups(model.Derivates);
                    
                    _browser.GoBack();

                    Console.WriteLine(CurrentModel++);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed somewhere for {0} {1}{1} {2}", model, Environment.NewLine, ex);
                }

            }
        }
        
        private void UpdateCar(Car car)
        {
            _carRepo.UpdateInsuranceGroup(car);
        }

        private void GetGroups(IEnumerable<Car> derivatives)
        {
            var tables = _browser.FindAllCss(".infoTable").ToList();
            int attempts = 0;
            while (!tables.Any() || attempts > 10)
            {
                attempts++;
                Thread.Sleep(1000);
                tables = _browser.FindAllCss(".infoTable tbody tr:nth-child(2)").ToList();
            }

            foreach (var table in tables)
            {
                var performanceTable = table.FindAllCss("tbody tr:nth-child(2)");
                
                foreach (var derivative in performanceTable)
                {
                    var derivativeName = derivative.FindCss("td:nth-child(1)").Text;
                    var insuranceGroupText = derivative.FindCss("td:nth-child(2)").Text;

                    int insuranceGroup = 0;

                    int.TryParse(insuranceGroupText, out insuranceGroup);

                    var matchingDerivative = derivatives.FirstOrDefault(d => d.Name.ToLower().Trim() == derivativeName.ToLower().Trim());

                    if (matchingDerivative != null)
                    {
                        matchingDerivative.InsuranceGroup = insuranceGroup;
                        
                        UpdateCar(matchingDerivative);

                        Console.WriteLine("{0} Ins {1}", derivativeName, insuranceGroup);
                    }
                    else
                    {
                        Console.WriteLine("Failed to find match for {0}", derivativeName);    
                    }
                }
            }
            
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
