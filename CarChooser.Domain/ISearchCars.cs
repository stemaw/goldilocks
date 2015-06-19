using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface ISearchCars
    {
        IEnumerable<Car> GetCar(Search search, IFilter judge);
        Car GetCar(int id);
    }
}