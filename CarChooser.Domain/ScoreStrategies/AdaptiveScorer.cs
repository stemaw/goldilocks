using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CarChooser.Domain.ScoreStrategies
{
    public class AdaptiveScorer : ILearn, IFilter
    {
        private Dictionary<string, List<double>> FactorScores { get; set; }

        private List<Car> Rejections { get; set; }

        public AdaptiveScorer()
        {
            Rejections = new List<Car>();

            var scoringProperties = CriteriaBuilder.GetScoringProperties();

            FactorScores = new Dictionary<string, List<double>>();

            foreach (var property in scoringProperties)
            {
                FactorScores.Add(property.Name, new List<double>(){0.0} );
            }
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

        public bool Learn(CarProfile profile, bool doILikeIt)
        {
            if (doILikeIt)
            {
                const int entry = 1;

                foreach( var factor in FactorScores.Keys)
                    FactorScores[factor].Add(entry * profile.Characteristics[factor]);
            }
            else
                Rejections.Add(profile.OriginalCar);


            return true;
        }
        
        
        public double ScoreTheCar(CarProfile car)
        {
            IList<double> residuals = FactorScores.Keys.Select(key =>
                                                               {
                                                                   var denominator = Math.Abs(FactorScores[key].StandardDeviation()) < 0.00000000000001 ? 1 : FactorScores[key].StandardDeviation();
                                                                   return Math.Abs(car.Characteristics[key] - FactorScores[key].StandardDeviation())/denominator;
                                                               }).ToList();

            var coefficientMatrix =
                (Matrix) Matrix.Build.DenseOfRowArrays(new List<double[]> {residuals.ToArray()});

            var carMatrix = (Matrix)Matrix.Build.DenseOfColumnArrays(new List<double[]> { car.Characteristics.Values.ToArray() });
            var scoreMatrix = (Matrix) (coefficientMatrix * carMatrix);

            return scoreMatrix[0, 0];
        }

        
        private bool IsCandidate(string criteriaName, CarProfile profile)
        {
            return ( profile.Characteristics[criteriaName] >= Mean(criteriaName) - StandardDeviation(criteriaName) ) &&
                   ( profile.Characteristics[criteriaName] <= Mean(criteriaName) + StandardDeviation(criteriaName) );
        }

        public List<Car> Filter(List<Car> carOptions)
        {
            Func<Car, bool> predicate =
                c =>
                    {
                        var carProfile = CarProfile.From(c);
                        var properties = CriteriaBuilder.GetScoringProperties();

                        var result = true;

                        foreach (var propertyInfo in properties)
                        {
                            result &= IsCandidate(propertyInfo.Name, carProfile);
                        }

                        return result;
                    };

            var rejectionIds = Rejections.Select(d => d.Id);

            var viableCars = carOptions.Where(c => !(rejectionIds.Contains(c.Id)))
                .Where(predicate)
                .OrderBy(c => ScoreTheCar(CarProfile.From(c))).ToList();

            return viableCars;
        }
    }
}
