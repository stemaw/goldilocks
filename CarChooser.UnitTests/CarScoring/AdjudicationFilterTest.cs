using CarChooser.Domain.ScoreStrategies;
using CarChooser.Domain.SearchStrategies;
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

            Assert.That(filter.IsIncluded(car), Is.True);
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

            Assert.That(filter.IsIncluded(car), Is.False);
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

            Assert.That(filter.IsIncluded(car), Is.True);
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

            Assert.That(filter.IsIncluded(car), Is.True);
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

            Assert.That(filter.IsIncluded(car), Is.False);
        }
    }
}
