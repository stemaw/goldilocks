using System.Collections.Generic;
using CarChooser.Web.Mappers;

namespace CarChooser.Web.Models
{
    public class CarVM
    {
        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public IEnumerable<PerformanceVM> Performance { get; set; }

        public long Id { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }

        public int YearFrom { get; set; }

        public int YearTo { get; set; }

        public IEnumerable<RatingVM> Ratings { get; set; }

        public int Attractiveness { get; set; }
    }
}