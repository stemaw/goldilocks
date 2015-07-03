using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CarChooser.Domain;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Npgsql;


namespace CarChooser.Data
{
    public class CarRepository : IGetCars
    {
        //private const string ConnectionString = @"Server=goldilocks.clstkqqph5qy.eu-west-1.rds.amazonaws.com;Port=5432;User Id=postgres;Password=postgres;Database=goldilocks;Timeout=60";
        // private const string ConnectionString = @"Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=gold;";

        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

        public Car GetCar(int id)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("select car from cars where id = " + id, conn);
                var result = (string)command.ExecuteScalar();
                return JsonConvert.DeserializeObject<Car>(result);
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

        private List<Car> _allCars;
        public List<Car> AllCars()
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
            var existing = AllCars().FirstOrDefault(c => c.Id == car.Id) != null;
            
            if (existing)
            {
                var json = JsonConvert.SerializeObject(car).Replace("'", "''");
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    var command =
                        new NpgsqlCommand(string.Format("UPDATE cars SET car='{0}' WHERE id = {1}", json, car.Id), conn);
                    
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                AllCars().Add(car);

                var json = JsonConvert.SerializeObject(car).Replace("'", "''");
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    var command =
                        new NpgsqlCommand(string.Format("INSERT INTO cars (id, car) VALUES ({0},'{1}')", car.Id, json), conn);
                    command.ExecuteNonQuery();
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
