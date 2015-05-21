using System.Collections.Generic;
using MathNet.Numerics.Statistics;

namespace CarChooser.Domain.ScoreStrategies
{
    public static class VarianceExtension
    {
        public static double StandardDeviation(this List<double> scores)
        {
            var factorScores = scores;
            var standardDeviation = Statistics.StandardDeviation(factorScores);

            return standardDeviation;
        }
    }
}
