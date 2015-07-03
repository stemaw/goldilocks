using System.Configuration;
using System.Data;
using CarChooser.Domain;
using Npgsql;

namespace CarChooser.Data
{
    public class ReportRepository : IReportProblems
    {
        public void Report(string reason, int carId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"insert into reports(""CarId"", ""Reason"") values ( @CarId, @Reason)", conn);

                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@CarId", carId);
                command.Parameters.AddWithValue("@Reason", reason);
                
                command.ExecuteNonQuery();
                conn.Close();
            }
        }
    }

}
