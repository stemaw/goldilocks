using System.Collections.Generic;

namespace CarChooser.Domain
{
    public interface IReportProblems
    {
        void Report(string reason, int carId);
        IEnumerable<int> GetReports(string reason);
    }
}