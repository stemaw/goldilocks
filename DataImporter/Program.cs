using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

            //FixRubbishImages();

            //GetManufacturerLogos();

            //var carRepository = new CarRepository();

            //var allCars = carRepository.AllCars();
            //foreach (var car in allCars)
            //{
            //    //new DescriptionGenerator().Generate(car);
            //    car.Description = null;
            //    //WordWrap(car.Description);
            //    //Thread.Sleep(1000);
            //    carRepository.Save(car);
            //}

            new MissingDataFixer().FixMissingData();

            //new ParkersPriceRipper().RipParkers();

            //new AltParkersRipper().RipParkers();
            //new ReliabilityIndex().FindManufacturersWithNoRating();
            //new ReliabilityIndex().FixManufacturersWithNoRating();

            Console.WriteLine("Done");
            Console.ReadKey();

            //Parkers(browser1);
        }

        private static void GetManufacturerLogos()
        {
        retry:

            BrowserSession browser = null;
            try
            {
                browser = Browser.SpinUpBrowser();
                new GoogleLogoRipper(browser).RipLogos(manualMode:true);
            }
            catch (Exception)
            {
                browser.Dispose();
                goto retry;
            }
        }

        private static void FixRubbishImages()
        {
            retry:

            BrowserSession browser = null;
            try
            {
                browser = Browser.SpinUpBrowser();
                RubbishImages(browser);
            }
            catch (Exception)
            {
                browser.Dispose();
                goto retry;
            }
        }

        public static void WordWrap(string paragraph)
        {
            paragraph = new Regex(@" {2,}").Replace(paragraph.Trim(), @" ");
            var left = Console.CursorLeft; var top = Console.CursorTop; var lines = new List<string>();
            for (var i = 0; paragraph.Length > 0; i++)
            {
                lines.Add(paragraph.Substring(0, Math.Min(Console.WindowWidth, paragraph.Length)));
                var length = lines[i].LastIndexOf(" ", StringComparison.Ordinal);
                if (length > 0) lines[i] = lines[i].Remove(length);
                paragraph = paragraph.Substring(Math.Min(lines[i].Length + 1, paragraph.Length));
                Console.SetCursorPosition(left, top + i); Console.WriteLine(lines[i]);
            }
        }

        private static void RubbishImages(BrowserSession browser)
        {
             var ripper = new GoogleImageRipper(browser);
            var reports = new ReportRepository().GetReports("rubbishImage");

            var allCars = new CarRepository().AllCars();
            var modelLevel = true;
            foreach (var report in reports)
            {
                var car = allCars.FirstOrDefault(c => c.Id == report);

                if (car == null) continue;
                
                try
                {
                    ripper.RipImage(car, modelLevel, true, true);
                    new ReportRepository().DeleteReport("rubbishImage", car.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        private static void Images(BrowserSession browser)
        {
            var ripper = new GoogleImageRipper(browser);

            var modelLevel = true;

            var allCars = new CarRepository().AllCars();

            if (modelLevel)
            {
                var models = from car in allCars
                             group car by car.ModelId
                                 into g
                                 select new { ModelId = g.Key, Derivates = g.ToList() };

                foreach (var model in models)
                {
                    var car = allCars.First(c => c.ModelId == model.ModelId);

                    {
                        try
                        {
                            ripper.RipImage(car, modelLevel, true);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                    }
                }
            }
            else
            {
                foreach (var car in allCars)
                {
                    try
                    {
                        ripper.RipImage(car, modelLevel, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
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

        private static void ModelFixer()
        {
            var cars = new CarRepository().AllCars();

            var bmws = cars.Where(c => c.Manufacturer.Name == "BMW").ToList();

            Console.WriteLine(bmws.Count);
        }
    }
}
