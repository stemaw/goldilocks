namespace CarChooser.Domain.ScoreStrategies
{
    public interface ILearn
    {
        bool Learn(CarProfile car, bool doILikeIt);
    }
}