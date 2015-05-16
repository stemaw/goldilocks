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
        private const string DatabasePath = @"C:\Users\ste_000\Documents\goldilocks\CarChooser.Data\CarData.db";
        //private const string DatabasePath = @"C:\Users\ste_000\Documents\visual studio 2012\Projects\CarChooser\Database\CarData.db";
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
                car = (Car) db.Ext().GetByID(dbId);
                car.Attractiveness = score;
                db.Store(car);
            }
        }

        public Car GetDefaultCar()
        {
            var random = new Random().Next(0, Count() - 1);
            return GetCar(random);
        }

        private static int Count()
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                return db.Query<Car>().Count(c => c.Id > 0);
            }
        }
        public IEnumerable<Car> GetCars(Func<Car, bool> predicate)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                return db.Query<Car>().Where(predicate).ToList();
            }
        }

        public IEnumerable<Car> AllCars()
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                return db.Query<Car>().ToList();
            }
        }

        public IEnumerable<Car> AllCars(int skip, int take)
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                return db.Query<Car>().Skip(skip).Take(take).ToList();
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

        // temporary method to deal with db40 being rubbish
        public void DeleteRubbish()
        {
            using (var db = Db4oEmbedded.OpenFile(DatabasePath))
            {
                var cars = db.Query<Car>().Where(c => c.Id == 0);
                foreach (var car in cars)
                {
                    var dbId = db.Ext().GetID(car);
                    var carToDelete = (Car)db.Ext().GetByID(dbId);
                    db.Delete(carToDelete);
                    db.Commit();
                }

            }
        }
    }
}
