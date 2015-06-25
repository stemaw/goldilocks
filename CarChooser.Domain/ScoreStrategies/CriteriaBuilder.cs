using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CarChooser.Domain.ScoreStrategies
{
    public class CriteriaBuilder
    {
        public static Dictionary<string, double> Build(Car car)
        {
            var scoringProperties = GetScoringProperties();

            var characteristics = new Dictionary<string, double>();

            foreach (var property in scoringProperties)
            {
                var value = typeof(Car).GetProperty(property.Name).GetValue(car);
                characteristics.Add(property.Name, Convert.ToDouble(value));
            }

            return characteristics;
        }

        public static IEnumerable<PropertyInfo> GetScoringProperties()
        {
            var scoringProperties =
                typeof (Car).GetProperties().Where(p => p.GetCustomAttributes().Any(a => a is ScoreAttribute));

            return scoringProperties;
        }
    }
}