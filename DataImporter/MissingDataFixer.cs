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
            bool wasFixed;
            switch (name)
            {
                case "InsuranceGroup":
                    wasFixed = FixInsuranceGroup(car);
                    if (!wasFixed)
                    {
                        car.InsuranceGroup = int.Parse(Console.ReadLine());
                        _carRepository.Save(car);
                    }
                    break;
                case "LuggageCapacity":
                    wasFixed = FixLuggage(car);
                    if (!wasFixed)
                    {
                        car.LuggageCapacity = int.Parse(Console.ReadLine());
                        _carRepository.Save(car);
                    }
                    break;
                case "Width":
                    wasFixed = FixWidth(car);
                    if (!wasFixed)
                    {
                        car.Width = int.Parse(Console.ReadLine());
                        _carRepository.Save(car);
                    }
                    break;
                case "Height":
                    wasFixed = FixHeight(car);
                    if (!wasFixed)
                    {
                        car.Height = int.Parse(Console.ReadLine());
                        _carRepository.Save(car);
                    }
                    break;
                case "Length":
                    wasFixed = FixLength(car);
                    if (!wasFixed)
                    {
                        car.Length = int.Parse(Console.ReadLine());
                        _carRepository.Save(car);
                    }
                    break;
                case "Weight":
                    wasFixed = FixWeight(car);
                    if (!wasFixed)
                    {
                        car.Weight = int.Parse(Console.ReadLine());
                        _carRepository.Save(car);
                    }
                    break;
                case "Mpg":
                    car.Mpg = int.Parse(Console.ReadLine());
                    _carRepository.Save(car);
                    break;
                case "Acceleration":
                    car.Acceleration = decimal.Parse(Console.ReadLine());
                    _carRepository.Save(car);
                    break;
                case "TopSpeed":
                    car.TopSpeed = int.Parse(Console.ReadLine());
                    _carRepository.Save(car);
                    break;
                case "Emissions":
                    //car.Emissions = int.Parse(Console.ReadLine());
                    //_carRepository.Save(car);
                    break;
                case "Power":
                    car.Power = int.Parse(Console.ReadLine());
                    _carRepository.Save(car);
                    break;
                case "Cylinders":
                    car.Cylinders = int.Parse(Console.ReadLine());
                    _carRepository.Save(car);
                    break;
                case "Torque":
                    car.Torque = int.Parse(Console.ReadLine());
                    _carRepository.Save(car);
                    break;

            }
        }

        private bool FixInsuranceGroup(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.InsuranceGroup != 0);

            if (missing != null)
            {
                car.InsuranceGroup = missing.InsuranceGroup;
                _carRepository.Save(car);
                Console.WriteLine("Fixed InsuranceGroup for {0}", car.Name);
                return true;
            }

            return false;
        }

        private bool FixWidth(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Width != 0);

            if (missing != null)
            {
                car.Width = missing.Width;
                _carRepository.Save(car);
                Console.WriteLine("Fixed width for {0}", car.Name);
                return true;
            }

            return false;
        }

        private bool FixHeight(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Height != 0);

            if (missing != null)
            {
                car.Height = missing.Height;
                _carRepository.Save(car);
                Console.WriteLine("Fixed height for {0}", car.Name);
                return true;
            }

            return false;
        }
        private bool FixLength(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Length != 0);

            if (missing != null)
            {
                car.Length = missing.Length;
                _carRepository.Save(car);
                Console.WriteLine("Fixed length for {0}", car.Name);
                return true;
            }

            return false;
        }

        private bool FixWeight(Car car)
        {
            var missing =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.Weight != 0);

            if (missing != null)
            {
                car.Weight = missing.Weight;
                _carRepository.Save(car);
                Console.WriteLine("Fixed weight for {0}", car.Name);
                return true;
            }

            return false;
        }

        private bool FixLuggage(Car car)
        {
            var luggageForModel =
                _allCars.FirstOrDefault(c => c.ModelId == car.ModelId && c.LuggageCapacity != 0);

            if (luggageForModel != null)
            {
                car.LuggageCapacity = luggageForModel.LuggageCapacity;
                _carRepository.Save(car);
                Console.WriteLine("Fixed luggage for {0}", car.Name);
                return true;
            }

            return false;
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
