using System;
using System.Collections.Generic;

namespace CarChooser.Domain.ScoreStrategies
{
    public class CarProfile
    {
        public string Make { get; private set; }
        public Dictionary<string, double> Characteristics { get; set; }
        public string Model { get; private set; }
        public Car OriginalCar { get; private set; }

        public CarProfile(string make, string model, Dictionary<string, double> characteristics)
        {
            Make = make;
            Model = model;
            Characteristics = characteristics;
        }

        public static CarProfile From(Car car)
        {
            var characteristics = new Dictionary<string, double>
                                  {
                                      {"Performance Review", car.PerformanceScore},
                                      {"Prestige Review", car.Manufacturer.PrestigeIndex},
                                      {"Reliability Review", car.Manufacturer.ReliabilityIndex},
                                      {"Sales", car.Sales},
                                      {"Size Review", car.SizeScore},
                                      {"Price Review", car.PriceScore},
                                      {"Acceleration", (double) car.Acceleration},
                                      {"TopSpeed", car.TopSpeed},
                                      {"Power", car.Power},
                                      {"InsuranceGroup", car.InsuranceGroup},
                                      {"Price", (double) car.Price},
                                      {"Length", car.Length},
                                      {"Width", car.Width},
                                      {"Height", car.Height},
                                      {"YearFrom", car.YearFrom},
                                      {"YearTo", car.YearTo}
                                  };


            var carProfile = new CarProfile(car.Manufacturer.Name, car.Model, characteristics);

            carProfile.OriginalCar = car;

            return carProfile;
        }
    }
}