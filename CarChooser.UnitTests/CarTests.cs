using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Domain;
using NUnit.Framework;

namespace CarChooser.UnitTests
{
    [TestFixture]
    public class CarTests
    {
        [TestCase("Audi 4d", "Audi 4 door", 4)]
        [TestCase("Audi 3d", "Audi 3 door", 3)]
        [TestCase("Audi 2d", "Audi 2 door", 2)]
        public void LessCrypticShouldWork(string input, string expectedOutput, int expectedDoors)
        {
            var car = new Car()
                {
                    Name = input
                };

            Assert.True(car.DoorCount== expectedDoors, car.DoorCount.ToString());
            Assert.True(car.LessCrypticName == expectedOutput, car.LessCrypticName);
        }
    }
}
