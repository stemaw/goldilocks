using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using CarChooser.Domain;
using LiteDB;


namespace CarChooser.Data
{
    public class CarRepository : IGetCars
    {
        private readonly string _db;

        public CarRepository(string db)
        {
            _db = db;
        }

        public Car GetCar(int id)
        {
            return AllCars().FirstOrDefault(c => c.Id == id);
        }

        public Car GetDefaultCar()
        {
            const int defaultId = 100;
            using (var db = new LiteDatabase(_db))
            {
                var col = db.GetCollection<Car>("cars");

                return col.FindById(defaultId);
            }
        }

        public IEnumerable<Car> GetCars(Func<Car, bool> predicate)
        {
            return AllCars().Where(predicate);
        }

        private readonly Object thisLock = new Object();
        private List<Car> _allCars;
        public List<Car> AllCars()
        {
            if (_allCars != null) return _allCars;
            lock (thisLock)
            {
                if (_allCars != null) return _allCars;

                using (var db = new LiteDatabase(_db))
                {
                    var col = db.GetCollection<Car>("cars");

                    _allCars = col.FindAll().ToList();
                }

                return _allCars;
            }
        }

        public IEnumerable<Car> AllCars(int skip, int take)
        {
            return AllCars().Skip(skip).Take(take);
        }

        public void Save(Car car)
        {
            var existing = AllCars().FirstOrDefault(c => c.Id == car.Id) != null;
            
            if (existing)
            {
                using (var db = new LiteDatabase(_db))
                {
                    var col = db.GetCollection<Car>("cars");

                    col.Update(car);
                }
            }
            else
            {
                AllCars().Add(car);

                using (var db = new LiteDatabase(_db))
                {
                    var col = db.GetCollection<Car>("cars");

                    col.Insert(car);
                }
            }
        }

        public Car GetCar(string manufacturer, string model, int yearFrom, int yearTo, string derivativeName)
        {
            return
                AllCars()
                    .FirstOrDefault(
                        c =>
                        c.Manufacturer.Name == manufacturer && c.Model == model && c.YearFrom == yearFrom && c.YearTo ==
                        yearTo && c.Name == derivativeName);
        }

        public Car GetModel(string manufacturer, string model, int yearFrom, int yearTo)
        {
            return
                AllCars()
                    .FirstOrDefault(
                        c =>
                        c.Manufacturer.Name == manufacturer && c.Model == model && c.YearFrom == yearFrom && c.YearTo ==
                        yearTo);
        }

        public bool UpdateManufacturerScore(string manufacturer, int score)
        {
            var cars = GetManufacturersCars(manufacturer);
            if (!cars.ToList().Any())
            {
                return false;
            }

            foreach (var car in cars)
            {
                car.Manufacturer.ReliabilityIndex = score;
                Save(car);
            }

            return true;

        }

        public bool UpdatePrestiage(string manufacturer, int score)
        {
            var cars = GetManufacturersCars(manufacturer);

            if (!cars.ToList().Any())
            {
                return false;
            }

            foreach (var car in cars)
            {
                car.Manufacturer.PrestigeIndex = score;
                Save(car);
            }

            return true;

        }

        private IEnumerable<Car> GetManufacturersCars(string manufacturer)
        {
            var cars = AllCars().Where(c => c.Manufacturer.Name.ToLower() == manufacturer.ToLower());
            if (!cars.ToList().Any())
            {
                if (manufacturer == "MG")
                {
                    cars = AllCars().Where(c => c.Manufacturer.Name == "MG");
                }
                else
                {
                    cars = AllCars().Where(c =>
                                           c.Manufacturer.Name.Length > 3 &&
                                           c.Manufacturer.Name.Substring(0, 3) == manufacturer.Substring(0, 3));
                }
            }
            return cars;
        }

        public void UpdateSales(Car car, int sales)
        {
            Save(car);
        }

        public void UpdateInsuranceGroup(Car car)
        {
            Save(car);
        }

        public void UpdateMpg(Car car)
        {
            Save(car);
        }

        public int GetNextModelId()
        {
            return AllCars().Max(c => c.ModelId) + 1;
        }

        public int GetNextDerivativeId()
        {
            return AllCars().Max(c => c.Id) + 1;
        }
    }
}
