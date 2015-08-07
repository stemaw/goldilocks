using System.Collections.Generic;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.Domain
{
    public interface IFilter
    {
        List<Car> Filter(List<Car> carOptions);
        double ScoreTheCar(CarProfile car);
    }
}