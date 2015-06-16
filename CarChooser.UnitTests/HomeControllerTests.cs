using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var controller = new HomeController(this, new SearchMapper(), new SearchVMMapper(new CarVMMapper(new CarRatingsMapper())), Mock.Of<DecisionRepository>());
            var result = controller.Index() as ViewResult;
            result.Model.Should().BeOfType<SearchResultVM>(); 
            
            var model = result.Model as SearchResultVM;
            
            model.CurrentCar.Manufacturer.Should().Be("Audi");
            model.CurrentCar.Model.Should().Be("A5");
        }

        public IEnumerable<Car> GetCar(Search search, IFilter carAdjudicator)
        {
            return new List<Car>{ new Car {Id = 1, Manufacturer = new Manufacturer {Name = "Audi", Score = 10}, Model = "A5"}};
        }
    }
}
