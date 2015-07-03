namespace CarChooser.Domain
{
    public interface IReportProblems
    {
        void Report(string reason, int carId);
    }
}