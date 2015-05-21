using System.Collections.Generic;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.Domain.SearchStrategies
{
    public class AdjudicationFilter
    {
        private readonly AdaptiveScorer _adaptiveScorer;

        public AdjudicationFilter(AdaptiveScorer adaptiveScorer)
        {
            _adaptiveScorer = adaptiveScorer;
        }

        public bool IsIncluded(CarProfile car)
        {
            var include = ( (MeetsCriteria(car, "Top Speed"))
                        && (MeetsCriteria(car, "2-Doors"))
                        && (MeetsCriteria(car, "3-Doors"))
                        && (MeetsCriteria(car, "4-Doors"))
                        && (MeetsCriteria(car, "5-Doors"))
                        && (MeetsCriteria(car, "Power")) );

            return include;
        }

        private bool MeetsCriteria(CarProfile car, string criteriaName)
        {
            try
            {
                return (_adaptiveScorer.Mean(criteriaName) - car.Characteristics[criteriaName]) <
                       _adaptiveScorer.StandardDeviation(criteriaName);
            }
            catch (KeyNotFoundException)
            {
                return true;
            }
        }
    }
}
