using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CarChooser.Domain
{
    public class Car
    {
        public long Id { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public string Model { get; set; }
        public int PriceScore { get; set; }
        public int PerformanceScore { get; set; }
        public int PrestigeScore { get; set; }
        public int AttractivenessScore { get; set; }
        public int ReliabilityScore { get; set; }
        public int SizeScore { get; set; }
        public decimal Price { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<string, decimal> Ratings { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public List<Performance> PerformanceFigures { get; set; }

        public int Attractiveness { get; set; }

        public int Sales { get; set; }

        public Car WithPriceScore(int priceScore)
        {
            PriceScore = priceScore;
            return this;
        }

        public Car WithPerformanceScore(int performanceScore)
        {
            PerformanceScore = performanceScore;
            return this;
        }

        public int GetDoorCount()
        {
            var doors = 0;
                
            // Parse this shizzle!
            if (Model != null)
            {
                foreach (Match match in Regex.Matches(Model.Trim(), @"(?<doors>[1-9](\-?d))"))
                    doors = int.Parse(match.Groups["doors"].ToString().Substring(0, 1));
                return doors;
            }
            return -1;
        }

        public Car WithPrestigeScore(int prestigeScore)
        {
            PrestigeScore = prestigeScore;
            return this;
        }

        public Car WithAttractivenessScore(int attractivenessScore)
        {
            AttractivenessScore = attractivenessScore;
            return this;
        }

        public Car WithReliabilityScore(int reliabilityScore)
        {
            ReliabilityScore = reliabilityScore;
            return this;
        }

        public Car WithSizeScore(int sizeScore)
        {
            SizeScore = sizeScore;
            return this;
        }

        public override string ToString()
        {
            return string.Format("Manufactuer:{0} Model:{1} YearFrom:{2} YearTo:{3}", Manufacturer.Name, Model,
                                 YearFrom, YearTo);
        }
    }
}
