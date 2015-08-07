using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CarChooser.Domain.ScoreStrategies
{
    public class AdaptiveScorer : ILearn, IFilter
    {
        private Dictionary<string, List<double>> LikeFactorScores { get; set; }
        private Dictionary<string, List<double>> DislikeFactorScores { get; set; }

        private List<Car> Rejections { get; set; }
        private List<Car> Likes { get; set; }

        public AdaptiveScorer()
        {
            Rejections = new List<Car>();
            Likes = new List<Car>();

            var scoringProperties = CriteriaBuilder.GetScoringProperties();

            LikeFactorScores = new Dictionary<string, List<double>>();
            DislikeFactorScores = new Dictionary<string, List<double>>();

            foreach (var property in scoringProperties)
            {
                LikeFactorScores.Add(property.Name, new List<double>(){0.0} );
                DislikeFactorScores.Add(property.Name, new List<double>(){0.0} );
            }
        }


        public double Mean(string criteriaName, bool forLikes = true)
        {
            var factorScore = forLikes ? LikeFactorScores[criteriaName] : DislikeFactorScores[criteriaName];

            if (factorScore.Count > 0 )
            {
                return factorScore.Average();
            }
            return 0;
        }

        public double StandardDeviation(string criteriaName, bool forLikes = true)
        {
            var factorScore = forLikes ? LikeFactorScores[criteriaName] : DislikeFactorScores[criteriaName];

            if (factorScore.Count > 0)
            {
                return factorScore.StandardDeviation();
            }

            return double.MaxValue;
        }

        public bool Learn(CarProfile profile, bool doILikeIt)
        {
            const int entry = 1;
            
            if (doILikeIt)
            {
                Likes.Add(profile.OriginalCar);

                foreach (var factor in LikeFactorScores.Keys)
                    LikeFactorScores[factor].Add(entry*profile.Characteristics[factor]);
            }
            else
            {
                Rejections.Add(profile.OriginalCar);

                foreach (var factor in DislikeFactorScores.Keys)
                    DislikeFactorScores[factor].Add(entry * profile.Characteristics[factor]);
            }
            return true;
        }
        
        
        public double ScoreTheCar(CarProfile car)
        {
            var factors = DislikeFactorScores;

            IList<double> residuals = factors.Keys.Select(key =>
                                                               {
                                                                   var denominator = Math.Abs(factors[key].StandardDeviation()) < 0.00000000000001 ? 1 : factors[key].StandardDeviation();
                                                                   return Math.Abs(car.Characteristics[key] - factors[key].StandardDeviation()) / denominator;
                                                               }).ToList();

            var coefficientMatrix =
                (Matrix) Matrix.Build.DenseOfRowArrays(new List<double[]> {residuals.ToArray()});

            var carMatrix = (Matrix)Matrix.Build.DenseOfColumnArrays(new List<double[]> { car.Characteristics.Values.ToArray() });
            var scoreMatrix = (Matrix) (coefficientMatrix * carMatrix);

            return scoreMatrix[0, 0];
        }

        
        private bool IsCandidate(string criteriaName, CarProfile profile)
        {
            //if (profile.Model.Contains("V40")) Debugger.Break();

            var mean = Mean(criteriaName);
            var standardDeviation = StandardDeviation(criteriaName);
            var value = profile.Characteristics[criteriaName];

            return value == 0 || (Likes.Any() && ((value >= mean - standardDeviation) && (value <= mean + standardDeviation)));
        }

        private bool IsNotCandidate(string criteriaName, CarProfile profile)
        {
            return Rejections.Any() && (profile.Characteristics[criteriaName] >= Mean(criteriaName, false) - StandardDeviation(criteriaName, false)) &&
                   (profile.Characteristics[criteriaName] <= Mean(criteriaName, false) + StandardDeviation(criteriaName, false));
        }

        public List<Car> Filter(List<Car> carOptions)
        {
            var properties = CriteriaBuilder.GetScoringProperties();

            Func<Car, bool> predicate =
                c =>
                    {                        
                        var result = true;

                        foreach (var propertyInfo in properties)
                        {
                            result &= IsCandidate(propertyInfo.Name, c.Profile);
                        }

                        return result;
                    };

            var rejectionIds = Rejections.Select(d => d.Id);

            var viableCars = carOptions.Where(c => !(rejectionIds.Contains(c.Id)))
                .Where(predicate)
                .OrderByDescending(c => ScoreTheCar(c.Profile)).ToList();

            return viableCars;
        }
    }
}
