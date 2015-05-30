using System;
using System.Collections.Generic;
using System.Linq;

namespace CarChooser.Domain.ScoreStrategies
{
    public class AdaptiveScorer : ILearn, IFilter
    {
        private Dictionary<string, List<double>> FactorScores { get; set; }

        public AdaptiveScorer()
        {
            FactorScores = new Dictionary<string, List<double>>
                           {
                               {"Performance Review", new List<double>()},
                               {"Prestige Review", new List<double>()},
                               {"Reliability Review", new List<double>()},
                               {"Attractiveness Review", new List<double>()},
                               {"Size Review", new List<double>()},
                               {"Price Review", new List<double>()}
                           };
        }


        public double Mean(string criteriaName)
        {
            var factorScore = FactorScores[criteriaName];

            if ( factorScore.Count > 0 )
            {
                return factorScore.Average();
            }
            return 0;
        }

        public double StandardDeviation(string criteriaName)
        {
            var factorScore = FactorScores[criteriaName];

            if (factorScore.Count > 0)
            {
                return FactorScores[criteriaName].StandardDeviation();
            }

            return double.MaxValue;
        }

        public bool Learn(CarProfile car, bool doILikeIt)
        {
            if (doILikeIt)
            {
                var entry = 1;

                FactorScores["Performance Review"].Add(entry * car.Characteristics["Performance Review"]);
                FactorScores["Prestige Review"].Add(entry * car.Characteristics["Prestige Review"]);
                FactorScores["Reliability Review"].Add(entry * car.Characteristics["Reliability Review"]);
                FactorScores["Attractiveness Review"].Add(entry * car.Characteristics["Attractiveness Review"]);
                FactorScores["Size Review"].Add(entry * car.Characteristics["Size Review"]);
                FactorScores["Price Review"].Add(entry * car.Characteristics["Price Review"]);
            }
            return true;
        }
        public bool IsViable(CarProfile car)
        {
            var include = ((IsViable(car, "Performance Review"))
                        && (IsViable(car, "Prestige Review"))
                        && (IsViable(car, "Reliability Review"))
                        && (IsViable(car, "Attractiveness Review"))
                        && (IsViable(car, "Size Review"))
                        && (IsViable(car, "Price Review")));

            return include;
        }

        private bool IsViable(CarProfile carProfile, string criteriaName)
        {
            try
            { 
                if ( FactorScores[criteriaName].Count >= 12 )
                    return Math.Abs(Mean(criteriaName) -
                        carProfile.Characteristics[criteriaName]) <
                           StandardDeviation(criteriaName);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return true;
            }
        }

        public List<Car> Filter(List<Car> carOptions)
        {
            var viableCars = carOptions.Where(c => IsViable(CarProfile.From(c))).Select(c => c).ToList();

            return viableCars;
        }
    }
}
