using System;
using CarChooser.Domain;
using CarChooser.Domain.ScoreStrategies;
using NUnit.Framework;
using Moq;

namespace CarChooser.UnitTests.CarScoring
{
    [TestFixture]
    public class GoldilocksScorerTest
    {
        [Test]
        public void WhenThereIsOneCarTheyLikeThenScoresContainJustThatScoringProfile()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);

            Assert.That(scorer.Mean("Top Speed", true), Is.EqualTo(112));
            Assert.That(scorer.Mean("3-Doors", true), Is.EqualTo(1));
            Assert.That(scorer.Mean("Power", true), Is.EqualTo(84));
        }

        [Test]
        public void WhenThereIsOneCarTheyDontLikeThenScoresContainNothing()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", false);

            Assert.That(scorer.Mean("Top Speed", true), Is.EqualTo(0));
            Assert.That(scorer.Mean("3-Doors", true), Is.EqualTo(0));
            Assert.That(scorer.Mean("5-Doors", true), Is.EqualTo(0));
            Assert.That(scorer.Mean("Power", true), Is.EqualTo(0));
        }


        [Test]
        public void WhenThereAreTwoCarsTheyLikeThenScoresContainsThatSumScore()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", true);

            Assert.That(scorer.Mean("Top Speed"), Is.EqualTo(119));
            Assert.That(scorer.Mean("3-Doors"), Is.EqualTo(0.5));
            Assert.That(scorer.Mean("5-Doors"), Is.EqualTo(0.5));
            Assert.That(scorer.Mean("Power"), Is.EqualTo(102));
        }

        [Test]
        public void WhenThereAreTwoCarsTheyLikeAndOneTheyDontThenScoresOnTwoCars()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 112, 5, 84, "Audi", "A1 Sportsback 1.2 TFSI Contract Edition Plus 5d", true);

            Assert.That(scorer.Mean("Top Speed"), Is.EqualTo(112));
            Assert.That(scorer.Mean("3-Doors"), Is.EqualTo(0.5));
            Assert.That(scorer.Mean("5-Doors"), Is.EqualTo(0.5));
            Assert.That(scorer.Mean("Power"), Is.EqualTo(84));
        }

        [Test]
        public void WhenThereAreTwoCarsTheyLikeAndOneTheyDontThenCarScoredInStandardDeviationIsSelected()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 147, 4, 170, "Audi", "A6 2005 SE", true);

            Assert.That(Math.Round(scorer.StandardDeviation("Top Speed"), 6), Is.EqualTo(24.748737));
            Assert.That(Math.Round(scorer.StandardDeviation("3-Doors"), 6), Is.EqualTo(0.707107));
            Assert.That(Math.Round(scorer.StandardDeviation("4-Doors"), 6), Is.EqualTo(0.707107));
            Assert.That(Math.Round(scorer.StandardDeviation("5-Doors"), 6), Is.EqualTo(0.0));
            Assert.That(Math.Round(scorer.StandardDeviation("Power"), 6), Is.EqualTo(60.811183)); 
        }

        [Test]
        public void WhenThereAreTwoCarsTheyLikeAndOneTheyDontThenCarScoredInStandardDeviationIsSelectedAndChangesOnNextScore()
        {
            var scorer = new AdaptiveScorer();

            TestCarFactory.PrepScorerWithCar(scorer, 112, 3, 84, "Audi", "A1 Sportsback 1.2 TFSI SE 3d", true);
            TestCarFactory.PrepScorerWithCar(scorer, 126, 5, 120, "Audi", "A1 Sportsback 1.4 TFSI Amplified Edition 5d", false);
            TestCarFactory.PrepScorerWithCar(scorer, 147, 4, 170, "Audi", "A6 2005 SE", true);
            TestCarFactory.PrepScorerWithCar(scorer, 147, 4, 170, "Audi", "A6 2005 SE", true);

            Assert.That(Math.Round(scorer.StandardDeviation("Top Speed"), 6), Is.EqualTo(20.207259));
            Assert.That(Math.Round(scorer.StandardDeviation("3-Doors"), 6), Is.EqualTo(0.577350));
            Assert.That(Math.Round(scorer.StandardDeviation("4-Doors"), 6), Is.EqualTo(0.577350));
            Assert.That(Math.Round(scorer.StandardDeviation("5-Doors"), 6), Is.EqualTo(0.0));
            Assert.That(Math.Round(scorer.StandardDeviation("Power"), 6), Is.EqualTo(49.652123)); 
        }
    }
}
