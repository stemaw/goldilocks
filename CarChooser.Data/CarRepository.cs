using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CarChooser.Domain;
using Db4objects.Db4o;


namespace CarChooser.Data
{
    public class CarRepository : IGetCars
    {
        //private const string DatabasePath = @"CarData.db";
        private const string DatabasePath = @"C:\Users\ste_000\Documents\goldilocks\CarChooser.Data\CarData.db";
        //private const string DatabasePath2 = @"C:\Users\ste_000\Documents\goldilocks\CarChooser.Data\CarData3.db";
        //private static string DatabasePath
        //{
        //    get
        //    {
        //        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        //        var uri = new UriBuilder(codeBase);
        //        var path = Uri.UnescapeDataString(uri.Path);
        //        return Path.Combine(Path.GetDirectoryName(path), "CarData.db");
        //    }
        //}

        public Car GetCar(long currentCarId)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                return db.Query<Car>().FirstOrDefault(c => c.Id == currentCarId);
            }
        }

        public void UpdateAttractiveness(int id, int score)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var car = db.Query<Car>().FirstOrDefault(c => c.Id == id);
                var dbId = db.Ext().GetID(car);
                car = (Car)db.Ext().GetByID(dbId);
                car.Attractiveness = score;
                db.Store(car);
                db.Close();
            }
        }

        public Car GetDefaultCar()
        {
            var random = new Random().Next(0, Math.Max(Count() - 1, 1));
            return GetCar(random);
        }

        private static int Count()
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var count = db.Query<Car>().Count(c => c.Id > 0);
                db.Close();
                return count;
            }
        }
        public IEnumerable<Car> GetCars(Func<Car, bool> predicate)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var list = db.Query<Car>().Where(predicate).ToList();
                db.Close();
                return list;
            }
        }

        public IEnumerable<Car> AllCars()
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var allCars = db.Query<Car>().ToList();
                db.Close();
                return allCars;
            }
        }

        public IEnumerable<Car> AllCars(int skip, int take)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var allCars = db.Query<Car>().Skip(skip).Take(take).ToList();
                db.Close();
                return allCars;
            }
        }

        public void Save(Car car)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                if (car.Id == 0)
                {
                    car.Id = db.Query<Car>().Max(c => c.Id) + 1;
                    db.Store(car);
                }

                db.Store(car);
            }
        }

        public Car GetCar(string manufacturer, string model, int yearFrom, int yearTo)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                return db.Query<Car>().FirstOrDefault(c => c.Manufacturer.Name == manufacturer && c.Model == model
                    && c.YearFrom == yearFrom && c.YearTo == yearTo);
            }
        }

        public void Delete(Car car)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var dbId = db.Ext().GetID(car);
                car = (Car)db.Ext().GetByID(dbId);
                db.Delete(car);
                db.Commit();
            }
        }

        public bool UpdateManufacturerScore(string manufacturer, int score)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var cars = db.Query<Car>().Where(c => c.Manufacturer.Name.ToLower() == manufacturer.ToLower());
                if (!cars.ToList().Any())
                {
                    if (manufacturer == "MG")
                    {
                        cars = db.Query<Car>().Where(c => c.Manufacturer.Name == "MG");
                    }
                    else
                    {
                        cars =
                            db.Query<Car>()
                              .Where(
                                  c =>
                                  c.Manufacturer.Name.Length > 3 &&
                                  c.Manufacturer.Name.Substring(0, 3) == manufacturer.Substring(0, 3));
                    }
                }
                if (!cars.ToList().Any())
                {
                    return false;
                }

                foreach (var car in cars)
                {
                    var dbId = db.Ext().GetID(car);
                    var carToUpdate = (Car) db.Ext().GetByID(dbId);
                    carToUpdate.Manufacturer.ReliabilityIndex = score;
                    db.Store(carToUpdate);
                }

                return true;
            }
        }

        public bool UpdatePrestiage(string manufacturer, int score)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var cars = db.Query<Car>().Where(c => c.Manufacturer.Name.ToLower() == manufacturer.ToLower());
                if (!cars.ToList().Any())
                {
                    if (manufacturer == "MG")
                    {
                        cars = db.Query<Car>().Where(c => c.Manufacturer.Name == "MG");
                    }
                    else
                    {
                        cars =
                            db.Query<Car>()
                              .Where(
                                  c =>
                                  c.Manufacturer.Name.Length > 3 &&
                                  c.Manufacturer.Name.Substring(0, 3) == manufacturer.Substring(0, 3));
                    }
                }
                if (!cars.ToList().Any())
                {
                    return false;
                }

                foreach (var car in cars)
                {
                    var dbId = db.Ext().GetID(car);
                    var carToUpdate = (Car)db.Ext().GetByID(dbId);
                    carToUpdate.Manufacturer.PrestigeIndex = score;
                    db.Store(carToUpdate);
                }

                return true;
            }
        }

        public void UpdateSales(Car car, int sales)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var carToFind = db.Query<Car>().First(c => c.Id == car.Id);
                var dbId = db.Ext().GetID(carToFind);
                var carToUpdate = (Car)db.Ext().GetByID(dbId);
                carToUpdate.Sales = sales;
                db.Store(carToUpdate);
            }
        }
 
        //public void MigrateClean()
        //{
        //    var cars = AllCars();

        //    using (var db = Db4oEmbedded.OpenFile(DatabasePath2))
        //    {
        //        foreach (var car in cars)
        //        {
        //            if (car.Id == 0) continue;
                    
        //            var existing = db.Query<Car>().FirstOrDefault(c => car.ToString() == c.ToString());
                    
        //            if (existing == null)
        //            db.Store(car);
        //        }
        //    }
        //}
    }
}
