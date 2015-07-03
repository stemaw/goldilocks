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
                    //FixIt(property.Name, car);
                }
            }
        }

        //private void FixIt(string name, Car car)
        //{
        //    switch (name)
        //    {
        //        case "Price":
        //            FixPrice(car);
        //    }
        //}

        //private void FixPrice(Car car)
        //{
            
        //}

        public static IEnumerable<PropertyInfo> GetScoringProperties()
        {
            return _scoringProperties ??
                   (_scoringProperties =
                    typeof(Car).GetProperties()
                                .Where(p => p.GetCustomAttributes().Any(a => a is ScoreAttribute)
                                && p.Name != "YearTo")
                                .ToList());
        }
    }
}
