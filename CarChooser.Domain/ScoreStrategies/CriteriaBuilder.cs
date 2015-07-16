using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CarChooser.Domain.ScoreStrategies
{
    public static class CriteriaBuilder
    {
        public static Dictionary<string, double> Build(Car car)
        {
            var scoringProperties = GetScoringProperties();

            var values = new Dictionary<string, double>
                {
                    {"Acceleration", Convert.ToDouble(car.Sales)},
                    {"TopSpeed", Convert.ToDouble(car.TopSpeed)},
                    {"Power", Convert.ToDouble(car.Power)},
                    {"InsuranceGroup", Convert.ToDouble(car.InsuranceGroup)},
                    {"Price", Convert.ToDouble(car.Price)},
                    {"Length", Convert.ToDouble(car.Length)},
                    {"Width", Convert.ToDouble(car.Width)},
                    {"Height", Convert.ToDouble(car.Height)},
                    {"YearFrom", Convert.ToDouble(car.YearFrom)},
                    {"YearTo", Convert.ToDouble(car.YearTo)},
                    {"LuggageCapacity", Convert.ToDouble(car.LuggageCapacity)},
                    {"Mpg", Convert.ToDouble(car.Mpg)},
                    {"Torque", Convert.ToDouble(car.Torque)},
                    {"Weight", Convert.ToDouble(car.Weight)},
                    {"Emissions", Convert.ToDouble(car.Emissions)},
                    {"EngineSize", Convert.ToDouble(car.EngineSize)},
                    {"DoorCount", Convert.ToDouble(car.DoorCount)},
                    {"Prestige", Convert.ToDouble(car.Manufacturer.PrestigeIndex)},
                    {"Reliability", Convert.ToDouble(car.Reliability)},
                };

            return scoringProperties.ToDictionary(property => property.Name, property => values[property.Name]);
        }

        private static IEnumerable<PropertyInfo> _scoringProperties;
 
        public static IEnumerable<PropertyInfo> GetScoringProperties()
        {
            return _scoringProperties ??
                   (_scoringProperties =
                    typeof (Car).GetProperties()
                                .Where(p => p.GetCustomAttributes().Any(a => a is ScoreAttribute))
                                .ToList());
        }
    }
}