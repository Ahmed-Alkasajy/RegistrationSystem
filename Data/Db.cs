
using System.Data;
using Microsoft.Data.SqlClient;

namespace student_online_system.Data
{
    public class Db
    {
        private readonly string _connectionString;

        public Db(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString")!;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public SqlCommand CreateStoredProcCommand(string procName, SqlConnection con)
        {
            var cmd = new SqlCommand(procName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }
    }
}

