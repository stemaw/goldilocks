using System;
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
            var include = ((IsViable(car, "Performance Review"))
                        && (IsViable(car, "Prestige Review"))
                        && (IsViable(car, "Reliability Review"))
                        && (IsViable(car, "Attractiveness Review"))
                        && (IsViable(car, "Size Review"))
                        && (IsViable(car, "Price Review")) );

            return include;
        }

        private bool IsViable(CarProfile carProfile, string criteriaName)
        {
            try
            {
                return Math.Abs(_adaptiveScorer.Mean(criteriaName) - 
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
            var viableCars = carOptions.Where(c => IsViable(CarProfile.From(c))).Select( c => c).ToList();
            
            return viableCars;
        }
    }
}
