using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CarChooser.Domain
{
    public class Car
    {
        public int Id { get; set; }
        public int ModelId { get;set; }

        public Manufacturer Manufacturer { get; set; }
        public string Model { get; set; }

        public string Name { get; set; }
        
        public decimal Acceleration { get; set; }
        public int TopSpeed { get; set; }
        public int Power { get; set; }

        public int InsuranceGroup { get; set; }
        
        public decimal Price { get; set; }
        
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public Dictionary<string, decimal> Ratings { get; set; }
        
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        
        public int Sales { get; set; }

        public int PriceScore { get; set; }
        public int PerformanceScore { get; set; }
        public int PrestigeScore { get; set; }
        public int AttractivenessScore { get; set; }
        public int ReliabilityScore { get; set; }
        public int SizeScore { get; set; }

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

        public override string ToString()
        {
            return string.Format("Manufactuer:{0} Model:{1} YearFrom:{2} YearTo:{3}", Manufacturer.Name, Model,
                                 YearFrom, YearTo);
        }
    }
}
