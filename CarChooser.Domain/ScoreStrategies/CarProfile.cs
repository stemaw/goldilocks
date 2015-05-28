using System.Collections.Generic;

namespace CarChooser.Domain.ScoreStrategies
{
    public class CarProfile
    {
        public string Make { get; private set; }
        public Dictionary<string, double> Characteristics { get; set; }
        public string Model { get; private set; }

        public CarProfile(string make, string model, Dictionary<string, double> characteristics)
        {
            Make = make;
            Model = model;
            Characteristics = characteristics;
        }

        public static CarProfile From(Car car)
        {
            var characteristics = new Dictionary<string, double>();

/*            characteristics.Add("Top Speed", car.PerformanceFigures[0].TopSpeed);
            characteristics.Add("Power", car.PerformanceFigures[0].Power);
            characteristics.Add("Doors", car.GetDoorCount());*/

            // Review scores - Note, these are statistically unsound.
            characteristics.Add("Performance Review", car.PerformanceScore);
            characteristics.Add("Prestige Review", car.PrestigeScore);
            characteristics.Add("Reliability Review", car.ReliabilityScore);
            characteristics.Add("Attractiveness Review", car.AttractivenessScore);
            characteristics.Add("Size Review", car.SizeScore);
            characteristics.Add("Price Review", car.PriceScore);

            return new CarProfile(car.Manufacturer.Name, car.Model, characteristics);
        }
    }
}