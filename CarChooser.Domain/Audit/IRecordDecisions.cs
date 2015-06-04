namespace CarChooser.Domain.Audit
{
    public interface IRecordDecisions
    {
        void RecordDecision(DecisionEntry decision);
    }
}