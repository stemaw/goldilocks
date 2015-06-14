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
    }
}