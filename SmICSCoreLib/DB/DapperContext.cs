using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.Sqlite;


namespace SmICSCoreLib.DB
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = @"Data Source=./Resources/db/SmICS.db";

        public DapperContext()
        {
        }
        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
            
        }
    }
}
