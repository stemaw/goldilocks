using System;
using System.Threading.Tasks;
using CarChooser.Data;
using Coypu;
using Coypu.Drivers;

namespace DataImporter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var browser1 = Browser.SpinUpBrowser();
            var browser2 = Browser.SpinUpBrowser();

            var task1 = Task.Run(() => Parkers(browser2));
            var task2 = Task.Run(() => Images(browser1));

            Task.WaitAll(task1, task2);

            //Parkers(browser1);
        }

        private static void Images(BrowserSession browser)
        {
            var ripper = new GoogleImageRipper(browser);

            var cars = new CarRepository().AllCars();

            foreach (var car in cars)
            {
                try
                {
                    ripper.RipImage(car.Id, car.Manufacturer.Name,
                                new ModelWithYear { ModelName = car.Model, YearFrom = car.YearFrom, YearTo = car.YearTo });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static void Parkers(BrowserSession browser)
        {
            var ripper = new ParkersRipper(browser);

            try
            {
                ripper.RipParkers(66, 4);
            }
            catch (Exception ex)
            {
                var currentManufacturer = ripper.CurrentManufacturer;
                var modelIndex = ripper.CurrentModel++;

                Console.WriteLine(ex.ToString());

                browser.Dispose();
                browser = Browser.SpinUpBrowser();

                ripper = new ParkersRipper(browser);

                ripper.RipParkers(currentManufacturer, modelIndex);
            }
        }
    }
}
