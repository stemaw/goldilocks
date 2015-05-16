using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface IManageCars
    {
        IEnumerable<Car> GetAllCars();
        IEnumerable<Car> GetAllCars(int pageNumber);
        bool UpdateCarAttractiveness(int id, int score);
    }
}