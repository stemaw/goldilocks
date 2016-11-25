using CarChooser.Domain.Audit;
using LiteDB;

namespace CarChooser.Data
{
    public class DecisionRepository : IRecordDecisions
    {
        private readonly string _db;

        public DecisionRepository(string db)
        {
            _db = db;
        }

        public void RecordDecision(DecisionEntry decision)
        {
            using (var db = new LiteDatabase(_db))
            {
                var col = db.GetCollection<DecisionEntry>("decisions");

                col.Insert(decision);
            }
        }
    }
}
