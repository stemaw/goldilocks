using System;
using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface IGetCars
    {
        Car GetCar(int currentCarId);
        Car GetDefaultCar();
        IEnumerable<Car> GetCars(Func<Car, bool> predicate);
        List<Car> AllCars();
        IEnumerable<Car> AllCars(int skip, int take);
        void Save(Car car);
        void UpdateAttractiveness(int id, int score);
    }
}