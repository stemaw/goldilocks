//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using CarChooser.Data;
//using CarChooser.Domain;

//namespace DataImporter
//{
//    public class Licensing
//    {
//        private const string ExcelPath = @"C:\Users\ste_000\Documents\goldilocks\vehiclelicensing.xls";
//        private const string DatabasePath = @"C:\Users\ste_000\Documents\goldilocks\CarChooser.Data\SalesData.db";
//        private const string LogPath = @"C:\Users\ste_000\Documents\goldilocks\fuzzy.txt";
//        private readonly CarRepository _carRepo = new CarRepository();

//        private static void SaveSales(IEnumerable<SaleDetail> sales)
//        {
//            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
//            {
//                db.Store(sales);
//            }
//        }

//        private IEnumerable<SaleDetail> GetSales()
//        {
//            List<SaleDetail> result;
//            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
//            {
//                result = db.Query<SaleDetail>().ToList();
//            }

//            if (!result.Any())
//            {
//                result = ImportFromExcel();
//                SaveSales(result);
//            }

//            return result;
//        }

//        public void ImportSalesData()
//        {
//            var sales = GetSales();

//            var cars = GetCars();

//            foreach(var car in cars)
//            {
//                var salesForModel = GetMatchingSales(car, sales);

//                    if (!salesForModel.Any())
//                    {
//                        Console.WriteLine("No figures for {0} {1}", car.Manufacturer.Name, car.Model);
//                    }
//                    else
//                    {
//                        var salesForYear = salesForModel.Sum(s => s.Figures.Sum());
//                        car.Sales = (int)(salesForYear);
//                        try
//                        {
//                            _carRepo.UpdateSales(car, car.Sales);
//                            Console.WriteLine("Successfully updated car {0} {1} {2}", car.Manufacturer, car.Model, car.Id);
//                        }
//                        catch (Exception)
//                        {
//                            Console.WriteLine("Unable to update car {0} {1} {2}", car.Manufacturer, car.Model, car.Id);
//                        }
//                    }
//                }
//        }

//        private IEnumerable<SaleDetail> GetMatchingSales(Car car, IEnumerable<SaleDetail> sales)
//        {
//            var saleDetails = sales as SaleDetail[] ?? sales.ToArray();
            
//            var exactMatch =
//                saleDetails.Where(
//                    s =>
//                    s.Manufacturer.ToLower() == car.Manufacturer.Name.ToLower() &&
//                    s.Model.ToLower() == car.Model.ToLower());

//            if (exactMatch.Any()) return exactMatch;

//            var allForManufacturer =
//                saleDetails.Where(s => GetStart(s.Manufacturer) == GetStart(car.Manufacturer.Name));

//            var forManufacturer = allForManufacturer as SaleDetail[] ?? allForManufacturer.ToArray();
//            var models = forManufacturer.Select(m => m.Model).ToArray();

//             IEnumerable<string> fuzzyMatches;
//            if (car.Manufacturer.Name == "BMW" || car.Manufacturer.Name == "Mercedes-Benz")
//            {
//                fuzzyMatches = SeriesMatcher(models, car.Model);
//            }
//            else
//            {
//                fuzzyMatches = Fuzzy.FuzzyMatches(models, car.Model);    
//            }
            
//            SaveFuzzyDecision(car, fuzzyMatches);

//            return forManufacturer.Where(m => fuzzyMatches.Contains(m.Model));
//        }

//        private IEnumerable<string> SeriesMatcher(string[] allModels, string model)
//        {
//            if (model.Contains("Class") || model.Contains("series")) 
//            {
//                var series = model.Substring(0, 1);

//                if (series == "M")
//                {
//                    series = "ML";
//                }

//                var prefixLength = series.Length;
//                return allModels.Where(m => m.Substring(0, prefixLength) == series && Regex.IsMatch(m.Substring(prefixLength, 1), @"\d+"));
//            }

//            return Fuzzy.FuzzyMatches(allModels, model);
//        }

       
//        private void SaveFuzzyDecision(Car car, IEnumerable<string> fuzzyMatches)
//        {
//            File.AppendAllText(LogPath, string.Format("{0} {1} matched with {2} {3} {3}", car.Manufacturer.Name, car.Model, string.Join(Environment.NewLine, fuzzyMatches), Environment.NewLine));
//        }

//        private string GetStart(string value)
//        {
//            return value.ToLower().Substring(0, value.Length > 4 ? 4 : value.Length);
//        }

//        private static List<SaleDetail> ImportFromExcel()
//        {
//            var excel = new Microsoft.Office.Interop.Excel.Application();
//            var book = excel.Workbooks.Open(ExcelPath);

//            var sheet = (Microsoft.Office.Interop.Excel.Worksheet)book.Sheets[1];

//            var sales = new List<SaleDetail>();
//            const int rowCount = 37517;
//            const int firstRow = 8;

//            for (var i = firstRow; i < rowCount - 9; i++)
//            {
//                var values = (Array)sheet.Range["A" + i, "G" + i].Cells.Value;

//                sales.Add(new SaleDetail
//                    {
//                        Manufacturer = values.GetValue(1, 1).ToString(),
//                        Model = values.GetValue(1, 2).ToString(),
//                        Figures = new List<int?>
//                            {
//                                GetNullableInt(values.GetValue(1, 3)),
//                                GetNullableInt(values.GetValue(1, 5)),
//                                GetNullableInt(values.GetValue(1, 6)),
//                                GetNullableInt(values.GetValue(1, 7)),
//                            }
//                    });
//            }
//            return sales;
//        }

//        private static int? GetNullableInt(object excelValue)
//        {
//            if (excelValue == null) return null;

//            var value = excelValue.ToString();

//            if (string.IsNullOrEmpty(value)) return null;

//            return Convert.ToInt32(value);
//        }


//        private static IEnumerable<Car> GetCars()
//        {
//            return new CarRepository().AllCars();
//        }
//    }

//    internal class SaleDetail
//    {
//        public string Manufacturer { get; set; }
//        public string Model { get; set; }
//        public List<int?> Figures { get; set; }
//    }
//}
