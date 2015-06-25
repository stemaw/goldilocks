using System;
using System.Web.Mvc;
using CarChooser.Data;
using CarChooser.Domain;
using CarChooser.Web.Controllers;
using CarChooser.Web.Mappers;
using CarChooser.Web.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CarChooser.UnitTests
{
    [TestFixture]
    public class HomeControllerTests : ISearchCars
    {
        [Test]
        [Ignore]
        public void ShouldReturnDefaultCar()
        {
            var controller = new HomeController(this, new SearchMapper(new CarVMMapper(new CarRatingsMapper())), new SearchVMMapper(new CarVMMapper(new CarRatingsMapper())), Mock.Of<DecisionRepository>());
            var result = controller.Index() as ViewResult;
            result.Model.Should().BeOfType<SearchResultVM>(); 
            
            var model = result.Model as SearchResultVM;
            
            model.CurrentCar.Manufacturer.Should().Be("Audi");
            model.CurrentCar.Model.Should().Be("A5");
        }

        public Car GetCar(Search search, IFilter carAdjudicator)
        {
            return new Car {Id = 1, Manufacturer = new Manufacturer {Name = "Audi", Score = 10}, Model = "A5"};
        }

        public Car GetCar(int id)
        {
            throw new NotImplementedException();
        }
    }
}
