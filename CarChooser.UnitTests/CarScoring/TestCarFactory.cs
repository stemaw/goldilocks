using System.Collections.Generic;
using CarChooser.Domain.ScoreStrategies;

namespace CarChooser.UnitTests.CarScoring
{
    public class TestCarFactory
    {
        public static CarProfile PrepScorerWithCar(IEducator scorer, int topSpeed, int numberOfDoors, int power, string make, string model, bool doILikeIt)
        {
            var car = CreateTestCarOnly(topSpeed, numberOfDoors, power, make, model);

            scorer.Learn(car, doILikeIt);

            return car;
        }

        public static CarProfile CreateTestCarOnly(int topSpeed, int numberOfDoors, int power, string make, string model)
        {
            var characteristics = new Dictionary<string, double>();

            characteristics.Add("Top Speed", topSpeed);
            characteristics.Add("Doors", numberOfDoors);
            characteristics.Add("Power", power);

            var car = new CarProfile(make, model, characteristics);
            return car;
        }
    }
}