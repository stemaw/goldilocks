using System.Collections.Generic;
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

        public IEnumerable<int> GetReports(string reason)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"select ""CarId"" from reports where ""Reason"" = @Reason", conn);
                command.Parameters.AddWithValue("@Reason", reason);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    yield return (int)reader[0];
                }
            }
        }

        public void DeleteReport(string reason, int id)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"delete from reports where ""Reason"" = @Reason and ""CarId"" = @CarId", conn);
                command.Parameters.AddWithValue("@Reason", reason);
                command.Parameters.AddWithValue("@CarId", id);
                command.ExecuteNonQuery();
            }
        }
    }

}
