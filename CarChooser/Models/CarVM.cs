using System.Collections.Generic;
using CarChooser.Web.Mappers;

namespace CarChooser.Web.Models
{
    public class CarVM
    {
        public string Model { get; set; }
        public int ModelId { get; set; }

        public string Manufacturer { get; set; }

        public decimal Acceleration { get; set; }
        public int TopSpeed { get; set; }
        public int Power { get; set; }
        public string Derivative { get; set; }

        public int InsuranceGroup { get; set; }

        public long Id { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        public int YearFrom { get; set; }

        public int YearTo { get; set; }

        public IEnumerable<RatingVM> Ratings { get; set; }

        public string Image { get; set; }

        public int Mpg { get; set; }

        public int EngineSize { get; set; }

        public int LuggageCapacity { get; set; }

        public string Transmission { get; set; }

        public int Weight { get; set; }

        public int Cylinders { get; set; }

        public int Emissions { get; set; }

        public int Torque { get; set; }

        public int EuroEmissionsStandard { get; set; }

        public decimal Price { get; set; }

        public string FullName { get { return string.Format("{0} {1} {2} {3}", Manufacturer, Model, Derivative, GetYear()); }}

        private string GetYear()
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