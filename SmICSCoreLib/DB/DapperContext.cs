using System.Data;
using Microsoft.Data.Sqlite;


namespace SmICSCoreLib.DB
{
    public class DapperContext
    {
        private readonly string _connectionString = "Data Source=./Resources/db/SmICS.db";

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
