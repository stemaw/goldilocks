using System;
using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain;
using Newtonsoft.Json;
using Npgsql;


namespace CarChooser.Data
{
    public class CarRepository : IGetCars
    {
        //private const string ConnectionString = @"Server=goldilocks.clstkqqph5qy.eu-west-1.rds.amazonaws.com;Port=5432;User Id=postgres;Password=postgres;Database=goldilocks;Timeout=60";
        private const string ConnectionString = @"Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=gold;";

        public Car GetCar(long currentCarId)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("select car from cars where id = " + currentCarId, conn);
                var result = (string)command.ExecuteScalar();
                return JsonConvert.DeserializeObject<Car>(result);
            }
        }

        public void UpdateAttractiveness(int id, int score)
        {
            var car = GetCar(id);
            car.AttractivenessScore = score;

            var json = JsonConvert.SerializeObject(car);
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(string.Format("UPDATE cars SET car={0} WHERE id = {1}", json, id), conn);
                command.ExecuteNonQuery();
            }
        }


        public Car GetDefaultCar()
        {
            const int defaultId = 100;
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("select car from cars where id = " + defaultId, conn);
                var result = (string)command.ExecuteScalar();
                return JsonConvert.DeserializeObject<Car>(result);
            }
        }

        public IEnumerable<Car> GetCars(Func<Car, bool> predicate)
        {
            return AllCars().Where(predicate);
        }

        private IEnumerable<Car> _allCars;
        public IEnumerable<Car> AllCars()
        {
            if (_allCars != null) return _allCars;

            var allCars = new List<Car>();
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("select car from cars", conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var json = reader.GetString(0);
                    allCars.Add(JsonConvert.DeserializeObject<Car>(json));
                }
            }

            _allCars = allCars;
            return _allCars;
        }

        public IEnumerable<Car> AllCars(int skip, int take)
        {
            return AllCars().Skip(skip).Take(take);
        }

        public void Save(Car car)
        {
            var json = JsonConvert.SerializeObject(car).Replace("'","''");
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(string.Format("UPDATE cars SET car='{0}' WHERE id = {1}", json, car.Id), conn);
                command.ExecuteNonQuery();
            }
        }

        public Car GetCar(string manufacturer, string model, int yearFrom, int yearTo)
        {
            return
                AllCars()
                    .FirstOrDefault(
                        c =>
                        c.Manufacturer.Name == manufacturer && c.Model == model && c.YearFrom == yearTo && c.YearTo ==
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
    }
}
