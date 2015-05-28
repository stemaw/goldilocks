using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain.SearchStrategies;

namespace CarChooser.Domain.ScoreStrategies
{
    public class AdaptiveScorer : IEducator
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
    }
}
