using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Domain;

namespace DataImporter
{
    public class DescriptionGenerator
    {
        private Random _random;
        private bool _lastWasBad;

        public DescriptionGenerator()
        {
            _random = new Random();
        }


        public void Generate(Car car)
        {
            _lastWasBad = false;

            car.Description = string.Format("{0}'s {1} ({2}) {3} {4}",
                                            car.Manufacturer.Name,
                                            car.Model,
                                            car.LessCrypticName,
                                            GetProductionInfo(car),
                                            ChooseRandom(car));

        }

        private string ChooseRandom(Car car)
        {
            var random = _random.Next(1, 4);

            switch (random)
            {
                case 1:
                    return string.Join(" ", GetOverall(car), GetDrivabilityInfo(car), GetPerformanceInfo(car),
                                       GetGreenInfo(car), GetInsuranceInfo(car));
                case 2:
                    return string.Join(" ", GetPerformanceInfo(car), GetDrivabilityInfo(car), GetGreenInfo(car),
                                       GetOverall(car), GetInsuranceInfo(car));
                case 3:
                    return string.Join(" ", GetDrivabilityInfo(car), GetPerformanceInfo(car), GetOverall(car),
                                       GetGreenInfo(car), GetInsuranceInfo(car));
                case 4:
                    return string.Join(" ", GetGreenInfo(car), GetDrivabilityInfo(car), GetPerformanceInfo(car),
                                       GetOverall(car), GetInsuranceInfo(car));
            }

            return string.Empty;
        }

        private string GetInsuranceInfo(Car car)
        {
            if (car.InsuranceGroup == 0) return string.Empty;

            if (car.InsuranceGroup > 40)
            {
                return "Watch out for the cost of insurance as it's in a pretty high group.";
            }

            if (car.InsuranceGroup < 15)
            {
                return "Might be a good one for younger drivers as insurance is typically lower for this car.";
            }

            return string.Empty;
        }

        private string GetOverall(Car car)
        {
            _lastWasBad = false;

            if (car.Ratings.ContainsKey("Overall") && car.Ratings["Overall"] >= 4)
            {
                return string.Format("Overall this is car is {0}.", GetPositiveSuperlative());
            }

            if (car.Ratings.ContainsKey("Overall") && car.Ratings["Overall"] >= 3)
            {
                return string.Format("Overall this car is {0}.", GetAverageSuperlative());
            }
            if (car.Ratings.ContainsKey("Overall") && car.Ratings["Overall"] < 3)
            {
                _lastWasBad = true;
                return string.Format("Overall the {0} is {1}.", car.Model, GetNegativeSuperlative());
            }

            return string.Empty;
        }

        private string GetDrivabilityInfo(Car car)
        {
            _lastWasBad = false;

            if (car.Ratings.ContainsKey("Handling") && car.Ratings["Handling"] > 4)
            {
                return "It's sharp handling makes it a delight to drive.";
            }
            if (car.Ratings.ContainsKey("Handling") && car.Ratings["Handling"] < 3)
            {
                _lastWasBad = true;
                return string.Format("It's a pretty {0} drive unfortunately so not one for {1}.", GetNegativeSuperlative(), GetSuperlativeForDrivers());
            }

            return string.Format("Handling is at best {0}.", GetAverageSuperlative());
        }

        private string GetAverageSuperlative()
        {
            var list = new List<string>
                {
                    "ordinary",
                    "average",
                    "less than inspiring",
                    "somewhat humdrum",
                    "tolerable",
                    "adequate"
                };

            return list.ElementAt(_random.Next(0, list.Count));
        }

        private string GetPositiveSuperlative()
        {
            var list = new List<string>
                {
                    "great",
                    "brilliant",
                    "very capable",
                    "pretty good",
                    "better than most",
                    "bordering on greatness",
                    "fantastic",
                };

            return list.ElementAt(_random.Next(0, list.Count));
        }

        private string GetNegativeSuperlative()
        {
            var list = new List<string>
                {
                    "dreadful",
                    "woeful",
                    "awful",
                    "poor",
                    "dire",
                    "tragic",
                    "shameful",
                    "rubbish"
                };

            return list.ElementAt(_random.Next(0, list.Count));
        }

        private string GetSuperlativeForDrivers()
        {
            var list = new List<string>
                {
                    "keen drivers",
                    "enthusiastic drivers",
                    "the stig (or similar)",
                    "a fun drive around the more twisty bits"
                };

            return list.ElementAt(_random.Next(0, list.Count));
        }
        private string GetConjuction(bool forPositiveNextStatement)
        {
            if (_lastWasBad && forPositiveNextStatement)
                return "However";

            if (_lastWasBad && !forPositiveNextStatement)
            {
                return "Also";
            }

            return string.Empty;
        }
        private string GetGreenInfo(Car car)
        {
            _lastWasBad = false;
            string result = string.Empty;

            if (car.Ratings.ContainsKey("Green credentials") && car.Ratings["Green credentials"] >= 4)
            {
                result = result + "It's pretty good for the environment";
            }

            if (car.Ratings.ContainsKey("Green credentials") && car.Ratings["Green credentials"] < 3)
            {
                _lastWasBad = true;
                result = result + "It's not exactly environmently friendly";
            }

            if (car.Mpg > 0)
            {
                if (result != string.Empty)
                {
                    result = result +
                             string.Format(" and you should typically get around {0} miles to the gallon",
                                           car.Mpg);
                }
                else
                {
                    result = string.Format("You should typically get around {0} miles to the gallon",
                                           car.Mpg);
                }
            }

            return result + (result != string.Empty ? "." : "");

        }

        private string GetPerformanceInfo(Car car)
        {
            if (car.Acceleration == 0) return string.Empty;

            var superlative = "woeful";
            _lastWasBad = true;

            if (car.Acceleration < 10) superlative = "adequate";
            if (car.Acceleration < 8)
            {
                _lastWasBad = false;
                superlative = "good";
            }
            if (car.Acceleration < 6) superlative = "blistering";
            if (car.Acceleration < 4) superlative = "bonkers";

            return string.Format("Performance is {0} with a top speed of {1}mph and 0-60 in {2} seconds.",
                                 superlative, car.TopSpeed, car.Acceleration);
        }

        private string GetProductionInfo(Car car)
        {
            if (car.YearTo == 0 && car.Price > 0)
                return string.Format("starts at {0} before you've loaded any options.", string.Format("{0:C0}" ,car.Price));

            if (car.YearFrom < DateTime.Today.Year)
            {
                return "is available new or used.";
            }

            if (car.Ratings["Buying used"] > 5)
                return "is no longer in production but makes a good second hand purchase.";

            return "is no longer in production and not the easiest to find a good second hand example.";
        }
    }
}
