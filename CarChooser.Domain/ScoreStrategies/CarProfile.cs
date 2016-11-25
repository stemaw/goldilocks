using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CarChooser.Domain.ScoreStrategies
{
    public class CarProfile
    {
        public string Make { get; private set; }
        public Dictionary<string, double> Characteristics { get; set; }
        public string Model { get; private set; }
        public Car OriginalCar { get; set; }

        public CarProfile(string make, string model, Dictionary<string, double> characteristics)
        {
            Make = make;
            Model = model;
            Characteristics = characteristics;
        }

        public static CarProfile From(Car car)
        {
            var characteristics = CriteriaBuilder.Build(car);

            var carProfile = new CarProfile(car.Manufacturer.Name, car.Model, characteristics);

            carProfile.OriginalCar = car;

            return carProfile;
        }
    }
}