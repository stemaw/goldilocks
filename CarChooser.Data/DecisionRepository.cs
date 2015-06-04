using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarChooser.Domain;
using CarChooser.Domain.Audit;
using Newtonsoft.Json;
using Npgsql;

namespace CarChooser.Data
{
    public class DecisionRepository : IRecordDecisions
    {
        public void RecordDecision(DecisionEntry decision)
        {
            // var ConnectionString = @"Server=goldilocks.clstkqqph5qy.eu-west-1.rds.amazonaws.com;Port=5432;User Id=postgres;Password=postgres;Database=goldilocks;Timeout=60";

            var ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var command = new NpgsqlCommand(@"insert into decisions(""CarId"", ""DislikeReason"") values ( @CarId, @DislikeReason )", conn);

                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@CarId", decision.CarId);
                command.Parameters.AddWithValue("@DislikeReason", decision.DislikeReason);

                command.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}
