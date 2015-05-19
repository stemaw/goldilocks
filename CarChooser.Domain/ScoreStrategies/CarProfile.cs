using System.Collections.Generic;

namespace CarChooser.Domain.ScoreStrategies
{
    public class CarProfile
    {
        private readonly string _make;
        public Dictionary<string, double> Characteristics { get; set; }
        private string _model;

        public CarProfile(string make, string model, Dictionary<string, double> characteristics)
        {
            _make = make;
            _model = model;
            Characteristics = characteristics;
        }
    }
}