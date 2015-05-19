namespace CarChooser.Domain.ScoreStrategies
{
    public interface IEducator
    {
        bool Learn(CarProfile car, bool doILikeIt);
    }
}