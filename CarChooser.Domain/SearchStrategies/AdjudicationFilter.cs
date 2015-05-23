using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.Domain.SearchStrategies
{
    public class AdjudicationFilter : IPresentCars
    {
        private readonly AdaptiveScorer _adaptiveScorer;

        public AdjudicationFilter(AdaptiveScorer adaptiveScorer)
        {
            _adaptiveScorer = adaptiveScorer;
        }

        public bool IsViable(CarProfile car)
        {
            var include = ( (IsViable(car, "Top Speed"))
/*
                        && (IsViable(car, "2-Doors"))
                        && (IsViable(car, "3-Doors"))
                        && (IsViable(car, "4-Doors"))
                        && (IsViable(car, "5-Doors"))
*/
                        && (IsViable(car, "Power")) );

            return include;
        }

        private bool IsViable(CarProfile carProfile, string criteriaName)
        {
            try
            {
                return (_adaptiveScorer.Mean(criteriaName) - 
                    carProfile.Characteristics[criteriaName]) <
                       _adaptiveScorer.StandardDeviation(criteriaName);
            }
            catch (KeyNotFoundException)
            {
                return true;
            }
        }

        public List<Car> GetViableCarsFrom(List<Car> carOptions)
        {
            return carOptions.Where(c => IsViable(CarProfile.From(c))).Select( c => c).ToList();
        }
    }
}
