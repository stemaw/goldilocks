using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface ISearchCars
    {
        Car GetCar(Search search, IFilter judge);
    }

    public interface IFilter
    {
        List<Car> Filter(List<Car> carOptions);
    }
}