using System.Linq;
using System.Web;
using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public interface IMapCars
    {
        CarVM Map(Car car);
        Car Map(CarVM car);
    }

    public class CarMapper : IMapCars
    {
        private readonly IMapOfficialCarRatings _officialCarRatingsMapper;

        public CarMapper(IMapOfficialCarRatings officialCarRatingsMapper)
        {
            _officialCarRatingsMapper = officialCarRatingsMapper;
        }

        public CarVM Map(Car car)
        {
            if (car == null) return null;

            return new CarVM
                {
                    Id = car.Id,
                    ModelId = car.ModelId,
                    Model = car.Model,
                    Manufacturer = HttpUtility.HtmlDecode(car.Manufacturer.Name),
                    Ratings = car.Ratings.Select(r => _officialCarRatingsMapper.Map(r)),
                    Acceleration = car.Acceleration,
                    Derivative = car.Name,
                    Power = car.Power,
                    InsuranceGroup = car.InsuranceGroup,
                    TopSpeed = car.TopSpeed,
                    Height = car.Height,
                    Width = car.Width,
                    Length = car.Length,
                    YearFrom = car.YearFrom,
                    YearTo = car.YearTo,
                    Cylinders = car.Cylinders,
                    Emissions = car.Emissions,
                    EngineSize = car.EngineSize,
                    EuroEmissionsStandard = car.EuroEmissionsStandard,
                    LuggageCapacity = car.LuggageCapacity,
                    Mpg = car.Mpg,
                    Torque = car.Torque,
                    Transmission = car.Transmission,
                    Weight = car.Weight,
                    Price = car.Price,
                    ReviewPage = car.ReviewPage,
                };
        }

        public Car Map(CarVM car)
        {
            if (car == null) return null;

            return new Car
            {
                Id = (int)car.Id,
                ModelId = car.ModelId,
                Model = car.Model,
                Manufacturer = new Manufacturer() { Name = car.Manufacturer },
                Acceleration = car.Acceleration,
                Name = car.Derivative,
                Power = car.Power,
                InsuranceGroup = car.InsuranceGroup,
                TopSpeed = car.TopSpeed,
                Height = car.Height,
                Width = car.Width,
                Length = car.Length,
                YearFrom = car.YearFrom,
                YearTo = car.YearTo,
                Cylinders = car.Cylinders,
                Emissions = car.Emissions,
                EngineSize = car.EngineSize,
                EuroEmissionsStandard = car.EuroEmissionsStandard,
                LuggageCapacity = car.LuggageCapacity,
                Mpg = car.Mpg,
                Torque = car.Torque,
                Transmission = car.Transmission,
                Weight = car.Weight,
                Price = car.Price,
                ReviewPage = car.ReviewPage,
            };
        }
    }
}