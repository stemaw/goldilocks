using System.Linq;
using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public interface IMapCarVMs
    {
        CarVM Map(Car car);
    }

    public class CarVMMapper : IMapCarVMs
    {
        private readonly IMapCarRatings _carRatingsMapper;

        public CarVMMapper(IMapCarRatings carRatingsMapper)
        {
            _carRatingsMapper = carRatingsMapper;
        }

        public CarVM Map(Car car)
        {
            return new CarVM
                {
                    Id = car.Id,
                    ModelId = car.ModelId,
                    Model = car.Model,
                    Manufacturer = car.Manufacturer.Name,
                    Ratings = car.Ratings.Select(r => _carRatingsMapper.Map(r)),
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
                    Price = car.Price
                };
        }
    }
}