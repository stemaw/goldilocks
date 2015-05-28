using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface ISearchCars
    {
        Car GetCar(Search search, IPresentCars carAdjudicator);
    }

    public interface IPresentCars
    {
        List<Car> GetViableCarsFrom(List<Car> carOptions);
    }
}