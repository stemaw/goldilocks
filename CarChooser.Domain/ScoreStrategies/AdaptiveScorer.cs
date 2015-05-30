using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting.Messaging;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace CarChooser.Domain.ScoreStrategies
{
    public class AdaptiveScorer : ILearn, IFilter
    {
        private Dictionary<string, List<double>> FactorScores { get; set; }

        public AdaptiveScorer()
        {
            FactorScores = new Dictionary<string, List<double>>
                           {
                               // Note, added zero entries so we get a deviation.
                               {"Performance Review", new List<double>(){0.0}},
                               {"Prestige Review", new List<double>(){0.0}},
                               {"Reliability Review", new List<double>(){0.0}},
                               {"Attractiveness Review", new List<double>(){0.0}},
                               {"Size Review", new List<double>(){0.0}},
                               {"Price Review", new List<double>(){0.0}}
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
        
        
        public double ScoreTheCar(CarProfile car)
        {
            IList<double> residuals = new List<double>();

            // Build array or CarProfile characteristic differences for each car relative to sigma (signed not absolute)
            foreach (var key in car.Characteristics.Keys)
            {
                // Take a residual relative to standard deviation (note, this adaptively changes)
                residuals.Add( Math.Abs(car.Characteristics[key] - FactorScores[key].StandardDeviation())/ FactorScores[key].StandardDeviation());
            }

            var coefficients = (Vector) Vector.Build.DenseOfArray(residuals.ToArray());

            var carElements = (Vector)Vector.Build.DenseOfArray(car.Characteristics.Values.ToArray());

            var coefficientMatrix =
                (Matrix) Matrix.Build.DenseOfRowArrays(new List<double[]> {residuals.ToArray()});

            var carMatrix = (Matrix)Matrix.Build.DenseOfColumnArrays(new List<double[]> { car.Characteristics.Values.ToArray() });
            var scoreMatrix = (Matrix) (coefficientMatrix * carMatrix);

            return scoreMatrix[0, 0];
        }

        public List<Car> Filter(List<Car> carOptions)
        {
            var viableCars = carOptions.OrderBy(c => ScoreTheCar(CarProfile.From(c))).ToList();

            var carScores = viableCars.Select(c => ScoreTheCar(CarProfile.From(c)));

            return viableCars;
        }
    }
}
