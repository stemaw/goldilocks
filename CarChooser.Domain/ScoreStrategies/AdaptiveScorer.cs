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

            FactorScores = new Dictionary<string, List<double>>
                           {
                               // Note, added zero entries so we get a deviation.
                               {"Performance Review", new List<double>(){0.0} },
                               {"Prestige Review", new List<double>(){0.0} },
                               {"Reliability Review", new List<double>(){0.0} },
                               {"Attractiveness Review", new List<double>(){0.0} },
                               {"Size Review", new List<double>(){0.0} },
                               {"Price Review", new List<double>(){0.0} },
                               {"Acceleration", new List<double>(){0.0} },
                               {"TopSpeed", new List<double>(){0.0} },
                               {"Power", new List<double>(){0.0} },
                               {"InsuranceGroup", new List<double>(){0.0} },
                               {"Price", new List<double>(){0.0} },
                               {"Length", new List<double>(){0.0} },
                               {"Width", new List<double>(){0.0} },
                               {"Height", new List<double>(){0.0} },
                               {"YearFrom", new List<double>(){0.0} },
                               {"YearTo", new List<double>(){0.0} }
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
              c => IsCandidate("Performance Review", CarProfile.From(c))
                   && IsCandidate("Prestige Review", CarProfile.From(c))
                   && IsCandidate("Attractiveness Review", CarProfile.From(c))
                   && IsCandidate("Size Review", CarProfile.From(c))
                   && IsCandidate("Price Review", CarProfile.From(c))
                   && IsCandidate("Acceleration", CarProfile.From(c))
                   && IsCandidate("TopSpeed", CarProfile.From(c))
                   && IsCandidate("Power", CarProfile.From(c))
                   && IsCandidate("InsuranceGroup", CarProfile.From(c))
                   && IsCandidate("Price", CarProfile.From(c))
                   && IsCandidate("Length", CarProfile.From(c))
                   && IsCandidate("Width", CarProfile.From(c))
                   && IsCandidate("Height", CarProfile.From(c))
                   && IsCandidate("YearFrom", CarProfile.From(c))
                   && IsCandidate("YearTo", CarProfile.From(c));

            var viableCars = carOptions.Where(predicate).Where( c => !(Rejections.Select( d => d.Id ).Contains(c.Id))).OrderBy(c => ScoreTheCar(CarProfile.From(c))).ToList();
            
            var carScores = viableCars.Select(c => ScoreTheCar(CarProfile.From(c)));

            return viableCars;
        }
    }
}
