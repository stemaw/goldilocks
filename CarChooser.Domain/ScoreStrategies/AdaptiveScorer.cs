using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain.SearchStrategies;

namespace CarChooser.Domain.ScoreStrategies
{
    public class AdaptiveScorer : IEducator
    {
        private readonly IGetCars _carStore;
        private Dictionary<string, List<double>> FactorScores { get; set; }
        private AdjudicationFilter filter;


        public AdaptiveScorer()
        {
            FactorScores = new Dictionary<string, List<double>>
                           {
                               {"Top Speed", new List<double>()},
                               {"Power", new List<double>()},
                               {"2-Doors", new List<double>()},
                               {"3-Doors", new List<double>()},
                               {"4-Doors", new List<double>()},
                               {"5-Doors", new List<double>()}
                           };

            filter = new AdjudicationFilter(this);
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
            var entry = 0;

            if (doILikeIt)
            {
                entry = 1;

                FactorScores["Top Speed"].Add(entry*car.Characteristics["Top Speed"]);
                FactorScores["Power"].Add(entry*car.Characteristics["Power"]);

/*
                var desiredDoors = (int) car.Characteristics["Doors"];

                for (var doors = 2; doors <= 5; doors++)
                    FactorScores[string.Format("{0}-Doors", doors)].Add(0);

                var doorsCriteria = string.Format("{0}-Doors", desiredDoors);
                var length = FactorScores[doorsCriteria].Count - 1;

                if (doILikeIt)
                    FactorScores[doorsCriteria][length] = 1;
*/
            }
            return true;
        }
    }
}
