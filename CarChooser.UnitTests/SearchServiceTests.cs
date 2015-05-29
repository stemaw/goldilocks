using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Domain;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CarChooser.UnitTests
{
    [TestFixture]
    public class SearchServiceTests : IGetCars
    {
        [TestCase(1, RejectionReasons.TooExpensive, "Ceed")]
        [TestCase(2, RejectionReasons.TooCommon, "320d")]
        public void ItShouldFindADifferentCar(int currentCarId, RejectionReasons reason, string expectedModel)
        {
            var service = new SearchService(this);
            var result = service.GetCar(new Search {CurrentCarId = currentCarId, RejectionReason = reason}, Mock.Of<IPresentCars>());

            result.Model.Should().Be(expectedModel);
        }

        public IEnumerable<Car> GetCars(Func<Car, bool> predicate)
        {
            return AllCars().Where(predicate);
        }

        public IEnumerable<Car> AllCars()
        {
            return new List<Car>
                {
                    new Car{ Id = 1, Manufacturer = new Manufacturer{ Name = "BMW", Score = 0}, Model = "320d", PriceScore = 50, PrestigeScore = 60, SizeScore = 60},
                        
                    new Car{Id = 2, Manufacturer = new Manufacturer{ Name = "Kia", Score = 0}, Model = "Ceed", PriceScore = 20, PrestigeScore = 20, SizeScore = 40}

                };
        }

        public IEnumerable<Car> AllCars(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public void Save(Car car)
        {
            throw new NotImplementedException();
        }

        public void UpdateAttractiveness(int id, int score)
        {
            throw new NotImplementedException();
        }

        public Car GetCar(long currentCarId)
        {
            return AllCars().First(c => c.Id == currentCarId);
        }

        public Car GetDefaultCar()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Car> GetCars(Search search)
        {
            throw new NotImplementedException();
        }
    }
}
