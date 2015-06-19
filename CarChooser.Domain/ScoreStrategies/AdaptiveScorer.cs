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
                               {"Sales", new List<double>(){0.0} },
                               {"Acceleration", new List<double>(){0.0} },
                               {"TopSpeed", new List<double>(){0.0} },
                               {"Power", new List<double>(){0.0} },
                               {"InsuranceGroup", new List<double>(){0.0} },
                               {"Price", new List<double>(){0.0} },
                               {"Length", new List<double>(){0.0} },
                               {"Width", new List<double>(){0.0} },
                               {"Height", new List<double>(){0.0} },
                               {"YearFrom", new List<double>(){0.0} },
                               {"YearTo", new List<double>(){0.0} },
                               {"LuggageCapacity", new List<double>(){0.0} },
                               {"Mpg", new List<double>(){0.0} },
                               {"Torque", new List<double>(){0.0} },
                               {"Weight", new List<double>(){0.0} },
                               {"Emissions", new List<double>(){0.0} },
                               {"EngineSize", new List<double>(){0.0} },
                               {"Doors", new List<double>(){0.0} },
                               {"Prestige", new List<double>(){0.0} },
                               {"Reliability", new List<double>(){0.0} },
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
                c =>
                    {
                        var carProfile = CarProfile.From(c);
                        return IsCandidate("Sales", carProfile)
                                && IsCandidate("Acceleration", carProfile)
                                && IsCandidate("TopSpeed", carProfile)
                                && IsCandidate("Power", carProfile)
                                && IsCandidate("InsuranceGroup", carProfile)
                                && IsCandidate("Price", carProfile)
                                && IsCandidate("Length", carProfile)
                                && IsCandidate("Width", carProfile)
                                && IsCandidate("Height", carProfile)
                                && IsCandidate("YearFrom", carProfile)
                                && IsCandidate("YearTo", carProfile)
                                && IsCandidate("LuggageCapacity", carProfile)
                                && IsCandidate("Mpg", carProfile)
                                && IsCandidate("Torque", carProfile)
                                && IsCandidate("Weight", carProfile)
                                && IsCandidate("Emissions", carProfile)
                                && IsCandidate("EngineSize", carProfile)
                                && IsCandidate("Prestige", carProfile)
                                && IsCandidate("Reliability", carProfile)
                                && IsCandidate("Doors", carProfile);
                    };

            var viableCars = carOptions.Where(predicate).Where(c => !(Rejections.Select(d => d.Id).Contains(c.Id))).ToList();
                //.OrderBy(c => ScoreTheCar(CarProfile.From(c))).ToList();
            
            //var carScores = viableCars.Select(c => ScoreTheCar(CarProfile.From(c)));

            return viableCars;
        }
    }
}
