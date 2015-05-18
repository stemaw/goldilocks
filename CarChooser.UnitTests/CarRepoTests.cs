using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Data;
using CarChooser.Domain;
using NUnit.Framework;

namespace CarChooser.UnitTests
{
    [TestFixture]
    public class CarRepoTests
    {
        //[Test]
        //public void ReId()
        //{
        //    var repo = new CarRepository();

        //    var cars = repo.AllCars();

        //    var i = 1;
        //    foreach (var car in cars)
        //    {
        //        car.Id = i;
        //        repo.Save(car);
        //        i++;
        //    }
        //}

        [Test]
        public void FindDupes()
        {
            var repo = new CarRepository();

            var cars = repo.AllCars();

            var results = from c in cars
                          group c by c.Manufacturer.Name + c.Model + c.YearFrom + c.YearTo into g
                          select new { Car = g.Key, Count = g.Count() };

            var dupes = results.Where(r => r.Count > 1).ToList();

            //var example = cars.Where(c => c.Manufacturer.Name + c.Model + c.YearFrom + c.YearTo == dupes.First().Car).ToList();

            Assert.That(dupes.Any() == false);
        }

        //[Test]
        //public void Migrate()
        //{
        //    new CarRepository().MigrateClean();
        //}
    }
}
