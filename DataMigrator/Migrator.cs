using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Domain;
using CarChooser.Domain.ScoreStrategies;
using LiteDB;
using NUnit.Framework;
using Newtonsoft.Json;
using Npgsql;

namespace DataMigrator
{
    [TestFixture]
    public class Migrator
    {
        private const string RemoteConnectionString = @"Server=goldilocks.clstkqqph5qy.eu-west-1.rds.amazonaws.com;Port=5432;User Id=postgres;Password=postgres;Database=goldilocks;";
        private const string LocalConnectionString = @"Server=localhost;Port=5432;User Id=postgres;Password=admin;Database=aws;";

        [Test]
        public void Migrate()
        {
            var cars = AllCars();

            //Delete();

            foreach (var car in cars)
            {
                StoreInLiteDb(car);
            }
        }

        private void StoreInLiteDb(Car car)
        {
            using (var db = new LiteDatabase(@"C:\Temp\HelpChoose.db"))
            {
                var col = db.GetCollection<Car>("cars");

                col.Insert(car);
            }
        }

        private IEnumerable<Car> AllCars()
        {
            var allCars = new List<Car>();
            using (var conn = new NpgsqlConnection(LocalConnectionString))
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

            return allCars;
        }

        private void Delete()
        {
            using (var conn = new NpgsqlConnection(RemoteConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand("DELETE FROM CARS", conn);
                command.ExecuteNonQuery();
            }
        }

        private void Store(Car car)
        {
            var json = JsonConvert.SerializeObject(car).Replace("'", "''");
            using (var conn = new NpgsqlConnection(RemoteConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(string.Format("INSERT INTO CARS(Id,Car) VALUES ({0},'{1}')", car.Id, json), conn);
                command.ExecuteNonQuery();
            }
        }

        private void Save(Car car)
        {
            var json = JsonConvert.SerializeObject(car).Replace("'", "''");
            using (var conn = new NpgsqlConnection(RemoteConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(string.Format("UPDATE cars SET car='{0}' WHERE id = {1}", json, car.Id), conn);
                command.ExecuteNonQuery();
            }
        }
    }
}
