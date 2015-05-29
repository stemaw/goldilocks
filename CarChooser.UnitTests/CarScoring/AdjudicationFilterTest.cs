using System.Collections.Generic;
using System.Linq;
using CarChooser.Domain;
using CarChooser.Domain.ScoreStrategies;
using CarChooser.Domain.SearchStrategies;
using Moq;
using NUnit.Framework;

namespace CarChooser.UnitTests.CarScoring
{
    [TestFixture]
    public class AdjudicationFilterTest
    {
        [Test]
        public void SelectingACarInTheLearnedListIncludeReturnsTrue()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            var car = TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            var filter = new AdjudicationFilter(scorer);

            Assert.That(filter.IsViable(car), Is.True);
        }


        [Test]
        public void SelectingACarSoFarOutFromTheLearnedListIncludeReturnsFalse()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            var filter = new AdjudicationFilter(scorer);

            var car = TestCarFactory.CreateTestCarOnly(40, 4, 30, "Horse", "And Cart");

            Assert.That(filter.IsViable(car), Is.False);
        }

        [Test]
        public void SelectingACarInitiallyOutOfLearnedListThenInsideTheLearnedListIncludeReturnsTrue()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            var filter = new AdjudicationFilter(scorer);

            var car = TestCarFactory.CreateTestCarOnly(40, 4, 30, "Horse", "And Cart");

            // This biases the results towards this car this time
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);

            Assert.That(filter.IsViable(car), Is.True);
        }

        [Test]
        public void SelectingACarInitiallyAcceptedEarlierAndThenItIsAcceptedAgain()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            var filter = new AdjudicationFilter(scorer);

            // This biases the results towards this car this time
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            
            var car = TestCarFactory.CreateTestCarOnly(112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d");

            Assert.That(filter.IsViable(car), Is.True);
        }


        [Test]
        public void TestingCarOutsideVariance()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            var filter = new AdjudicationFilter(scorer);

            // This biases the results towards this car this time
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);
            TestCarFactory.PrepScorerWithCar(scorer, 40, 4, 30, "Horse", "And Cart", true);

            var car = TestCarFactory.CreateTestCarOnly(37, 2, 23, "NautAudi", "A10432 Sportsback 1.2 TFSI Contract Edition Plus 5d");

            Assert.That(filter.IsViable(car), Is.False);
        }


        [Test]
        public void TestingViableListOfCarsInsideVariance()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            //var includedCar = TestCarFactory.CreateTestCarOnly(112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d");

            var filter = new AdjudicationFilter(scorer);

            var includedCar = new Car()
                {
                    Manufacturer = new Manufacturer() {Name = "Audi"},
                    Model = "A1 Sportsback 1.2 TFSI Contract Edition Plus 3d",
                    Power = 84,
                    TopSpeed = 112
                };


            var excludedCar1 = new Car()
                {
                    Manufacturer = new Manufacturer {Name = "Toyota"},
                    Model = "Hilux",
                    Power = 32,
                    TopSpeed = 23
                };

            var carList = new List<Car>()
                                {
                                    includedCar,
                                    excludedCar1
                                };


            var viableList = filter.GetViableCarsFrom(carList);

            Assert.That(viableList.Any(c => c.Manufacturer.Name == includedCar.Manufacturer.Name), Is.True);
            Assert.That(viableList.Any(c => c.Manufacturer.Name == excludedCar1.Manufacturer.Name), Is.False);
        }


        [Test]
        public void TestingViableListOfCarsOutsideVariance()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            var filter = new AdjudicationFilter(scorer);

            var carList = new List<Car>()
                {
                    new Car()
                        {
                            Manufacturer = new Manufacturer() {Name = "Toyota"},
                            Model = "Avensys 5d",
                            Power = 23,
                            TopSpeed = 37
                        }
                };


            var viableList = filter.GetViableCarsFrom(carList);

            Assert.That(viableList.Any(c => c.Manufacturer.Name == "Toyota"), Is.False);
        }

    }
}
