using System.Configuration;
using System.Data;
using CarChooser.Domain.Audit;
using Npgsql;

namespace CarChooser.Data
{
    public class DecisionRepository : IRecordDecisions
    {
        public void RecordDecision(DecisionEntry decision)
        {
            // var ConnectionString = @"Server=goldilocks.clstkqqph5qy.eu-west-1.rds.amazonaws.com;Port=5432;User Id=postgres;Password=postgres;Database=goldilocks;Timeout=60";

            var connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"insert into decisions(""CarId"", ""DislikeReason"", ""Session"") values ( @CarId, @DislikeReason, @SessionId )", conn);

                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@CarId", decision.CarId);
                command.Parameters.AddWithValue("@DislikeReason", decision.DislikeReason);
                command.Parameters.AddWithValue("@SessionId", decision.SessionId);

                command.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
