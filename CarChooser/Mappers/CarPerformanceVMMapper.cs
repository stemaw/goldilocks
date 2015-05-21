using CarChooser.Domain;
using CarChooser.Web.Models;

namespace CarChooser.Web.Mappers
{
    public interface IMapCarPerformanceFigures
    {
        PerformanceVM Map(Performance performance);
    }

    public class CarPerformanceVMMapper : IMapCarPerformanceFigures
    {
        public PerformanceVM Map(Performance performance)
        {
            return new PerformanceVM
                {
                    Derivative = performance.Derivative,
                    Acceleration = performance.Acceleration,
                    Power = performance.Power,
                    TopSpeed = performance.TopSpeed,
                    InsuranceGroup = performance.InsuranceGroup
                };
        }
    }
}