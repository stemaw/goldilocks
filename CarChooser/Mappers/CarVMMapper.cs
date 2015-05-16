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
        private readonly IMapCarPerformanceFigures _performanceVMMapper;
        private readonly IMapCarRatings _carRatingsMapper;

        public CarVMMapper(IMapCarPerformanceFigures performanceVMMapper, IMapCarRatings carRatingsMapper)
        {
            _performanceVMMapper = performanceVMMapper;
            _carRatingsMapper = carRatingsMapper;
        }

        public CarVM Map(Car car)
        {
            return new CarVM
                {
                    Id = car.Id,
                    Model = car.Model,
                    Manufacturer = car.Manufacturer.Name,
                    Ratings = car.Ratings.Select(r => _carRatingsMapper.Map(r)),
                    Performance = car.PerformanceFigures.Select(cp => _performanceVMMapper.Map(cp)),
                    Height = car.Height,
                    Width = car.Width,
                    Length = car.Length,
                    YearFrom = car.YearFrom,
                    YearTo = car.YearTo,
                    Attractiveness = car.Attractiveness
                };
        }
    }
}