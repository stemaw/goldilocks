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
            //var browser1 = Browser.SpinUpBrowser();
            //var browser2 = Browser.SpinUpBrowser();

            //var task1 = Task.Run(() => Parkers(browser2));
            //var task2 = Task.Run(() => Images(browser1));

            //Task.WaitAll(task1, task2);

           // Images(browser1);

            //new Prestige().ImportManufacturerScores();
            //new ReliabilityIndex().ImportManufacturerScores();
            //new Licensing().ImportSalesData();

            //new InsuranceGroup(browser1).RipInsuranceGroup(0);
#if DEBUG            
            new AltParkersRipper().RipParkers();
#else
    retry:

            BrowserSession browser = null;
            try
            {
                browser = Browser.SpinUpBrowser();
                Images(browser);
            }
            catch (Exception)
            {
                browser.Dispose();
                goto retry;
            }
            
#endif
            Console.WriteLine("Done");
            Console.ReadKey();

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
                    ripper.RipImage(car);
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
                ripper.RipParkers(70, 1, false);
            }
            catch (Exception ex)
            {
                var currentManufacturer = ripper.CurrentManufacturer;
                var modelIndex = ripper.CurrentModel++;

                Console.WriteLine(ex.ToString());

                browser.Dispose();
                browser = Browser.SpinUpBrowser();

                ripper = new ParkersRipper(browser);

                ripper.RipParkers(currentManufacturer, modelIndex, false);
            }
        }
    }
}
