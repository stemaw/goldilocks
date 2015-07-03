using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface IManageCars
    {
        IEnumerable<Car> GetAllCars();
        IEnumerable<Car> GetAllCars(int pageNumber);
        void UpdateRatings(Car car);
        Car GetCar(int id);
    }
}