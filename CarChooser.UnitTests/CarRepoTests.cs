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
        [Test]
        public void ReId()
        {
            var repo = new CarRepository();

            var cars = repo.AllCars();

            var i = 1;
            foreach (var car in cars)
            {
                car.Id = i;
                repo.Save(car);
                i++;
            }
        }
    }
}
