using System.Collections.Generic;
using CarChooser.Domain;
using CarChooser.Domain.ScoreStrategies;
using NUnit.Framework;

namespace CarChooser.UnitTests.CarFiltering
{
    [TestFixture]
    public class FilterRepositoryTests
    {
        [Test]
        public void WhenSuppliedWithCarsThenItIsMappedToCarProfile()
        {
            var car = new Car()
                {
                    Id = 132,
                    Manufacturer = new Manufacturer() {Name = "Audi", PrestigeIndex = 14, ReliabilityIndex = 65},
                    TopSpeed = 112,
                    Power = 84
                };

            var carProfile = CarProfile.From(car);

            Assert.That(carProfile.Characteristics["TopSpeed"], Is.EqualTo(112));
            Assert.That(carProfile.Characteristics["Power"], Is.EqualTo(84));
        }

    }
}
