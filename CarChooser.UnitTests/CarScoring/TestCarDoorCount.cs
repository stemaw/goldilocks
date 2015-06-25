using CarChooser.Domain;
using NUnit.Framework;

namespace CarChooser.UnitTests.CarScoring
{
    public class TestCarDoorCount
    {
        [Test]
        public void WhenNameIs5DoorsThenDoorCOuntIsFive()
        {
            var car = new Car()
                      {
                          Model = "A1 Sportsback 1.2 TFSI SE 3d"
                      };

            Assert.That(car.DoorCount, Is.EqualTo(3));
        }
    }
}
