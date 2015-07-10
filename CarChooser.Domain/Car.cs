using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.Domain
{
    public class Car
    {
        private IList<IList<UserRating>> _userRatings;
        private CarProfile _profile;
        public string VEDBand { get; set; }
        public int Id { get; set; }
        public int ModelId { get;set; }

        public Manufacturer Manufacturer { get; set; }
        public string Model { get; set; }

        public string Name { get; set; }

        [Score]
        public decimal Acceleration { get; set; }

        [Score]
        public int TopSpeed { get; set; }

        [Score]
        public int Power { get; set; }

        [Score]
        public int InsuranceGroup { get; set; }

        [Score]
        public decimal Price { get; set; }

        [Score]
        public int Length { get; set; }

        [Score]
        public int Width { get; set; }

        [Score]
        public int Height { get; set; }

        [Score]
        public int YearFrom { get; set; }

        [Score]
        public int YearTo { get; set; }

        public int Sales { get; set; }

        [Score]
        public int Mpg { get; set; }

        [Score]
        public int EngineSize { get; set; }

        [Score]
        public int LuggageCapacity { get; set; }

        [Score]
        public int Weight { get; set; }

        [Score]
        public int Torque { get; set; }

        [Score]
        public int Emissions { get; set; }

        [Score]
        public int DoorCount
        {
            get
            {
                var doors = 0;

                // Parse this shizzle!
                if (Name != null)
                {
                    foreach (Match match in Regex.Matches(Name.Trim(), @"(?<doors>[1-9](\-?d))"))
                        doors = int.Parse(match.Groups["doors"].ToString().Substring(0, 1));
                    return doors;
                }
                return -1;
            }
        }

        [Score]
        public double Reliability {
            get { return Manufacturer.ReliabilityIndex; } 
        }

        [Score]
        public double Prestige
        {
            get { return Manufacturer.PrestigeIndex; }
        }

        public string Transmission { get; set; }

        public int SizeScore { get; set; }

        public int PriceScore { get; set; }

        public Dictionary<string, decimal> Ratings { get; set; }

        public int PerformanceScore { get; set; }

        public int PrestigeScore { get; set; }

        public int AttractivenessScore { get; set; }

        public int Cylinders { get; set; }

        public int EuroEmissionsStandard { get; set; }

        public string ReviewPage { get; set; }

        public string FactsPage { get; set; }

        public override string ToString()
        {
            return string.Format("Manufactuer:{0} Model:{1} YearFrom:{2} YearTo:{3}", Manufacturer.Name, Model,
                                 YearFrom, YearTo);
        }

        [IgnoreDataMember]
        public string LessCrypticName
        {
            get { return Regex.Replace(Name, @"[1-9](\-?d)", DoorCount + " door"); }
        }

        public IList<IList<UserRating>> UserRatings
        {
            get { return _userRatings ?? (_userRatings = new List<IList<UserRating>>()); }
            set { _userRatings = value; }
        }

        [IgnoreDataMember]
        public CarProfile Profile
        {
            get { return _profile ?? (_profile = CarProfile.From(this)); }
            set { _profile = value; }
        }

        [IgnoreDataMember]
        public string DerivativeName
        {
            get
            {
                return string.Format("{0} ({1} to {2})", Name, YearFrom, YearTo == 0 ? "date" : YearTo.ToString());
            }
        }

        public string GetYear()
        {
            if (YearTo == 0) return string.Format("({0} on)", To2DigitYear(YearFrom));

            return string.Format("({0} - {1})", To2DigitYear(YearFrom), To2DigitYear(YearTo));
        }

        private static string To2DigitYear(int year)
        {
            return year.ToString().Substring(2, 2);
        }
    }
}
