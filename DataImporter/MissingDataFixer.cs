using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Data;
using CarChooser.Domain;
using CarChooser.Domain.ScoreStrategies;

namespace DataImporter
{
    public class MissingDataFixer
    {
        private readonly CarRepository _carRepository;
        private static IEnumerable<PropertyInfo> _scoringProperties;
        List<Car> _allCars;

        public MissingDataFixer()
        {
            _carRepository = new CarRepository();
        }
        public void FixMissingData()
        {
            _allCars = _carRepository.AllCars();
            var carType = typeof (Car);

            var missingData = new Dictionary<Car, List<string>>();

            foreach (var car in _allCars)
            {
                foreach (var property in GetScoringProperties().Where(property => Convert.ToInt32(carType.GetProperty(property.Name).GetValue(car)) == 0))
                {
                    if (missingData.ContainsKey(car))
                    {
                        missingData[car].Add(property.Name);
                    }
                    else
                    {
                        missingData.Add(car, new List<string>() { property.Name });
                    }
                   FixIt(property.Name, car);
                }
            }

            Console.WriteLine("Missing {0}", missingData.Values.Sum(v => v.Count));

            //var missingAttributes = missingData.SelectMany(d => d.Value).SelectMany(v => v).Distinct();

            //Console.WriteLine("Missing {0}", string.Join(missingAttributes, ";"));
            
        }

        private void FixIt(string name, Car car)
        {
            switch (name)
            {
                case "LuggageCapacity":
                    FixLuggage(car);
                    break;
                case "Width":
                    FixWidth(car);
                    break;
                case "Height":
                    FixHeight(car);
                    break;
                case "Length":
                    FixLength(car);
                    break;
                case "Weight":
                    FixWeight(car);
                    break;
            }
        }

        private void FixWidth(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Width != 0);

            if (missing != null)
            {
                car.Width = missing.Width;
                _carRepository.Save(car);
                Console.WriteLine("Fixed width for {0}", car.Name);
            }
        }

        private void FixHeight(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Height != 0);

            if (missing != null)
            {
                car.Height = missing.Height;
                _carRepository.Save(car);
                Console.WriteLine("Fixed height for {0}", car.Name);
            }
        }
        private void FixLength(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Length != 0);

            if (missing != null)
            {
                car.Length = missing.Length;
                _carRepository.Save(car);
                Console.WriteLine("Fixed length for {0}", car.Name);
            }
        }

        private void FixWeight(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Weight != 0);

            if (missing != null)
            {
                car.Weight = missing.Weight;
                _carRepository.Save(car);
                Console.WriteLine("Fixed weight for {0}", car.Name);
            }
        }

        private void FixLuggage(Car car)
        {
            var luggageForModel =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.LuggageCapacity != 0);

            if (luggageForModel != null)
            {
                car.LuggageCapacity = luggageForModel.LuggageCapacity;
                _carRepository.Save(car);
                Console.WriteLine("Fixed luggage for {0}", car.Name);
            }
        }

        //private void FixPrice(Car car)
        //{
            
        //}

        public static IEnumerable<PropertyInfo> GetScoringProperties()
        {
            return _scoringProperties ??
                   (_scoringProperties =
                    typeof(Car).GetProperties()
                                .Where(p => p.GetCustomAttributes().Any(a => a is ScoreAttribute)
                                && p.Name != "YearTo"
                                && p.Name != "Price")
                                .ToList());
        }
    }
}
