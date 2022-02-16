using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
//using Npgsql;
using System;

namespace SmICSCoreLib.DB
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DbType DatabaseType { get; }
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("default");
            switch (_configuration.GetConnectionString("type"))
            {
                case "MSSQL":
                    DatabaseType = DbType.MSSQL;
                    break;
                case "Postgres":
                    DatabaseType = DbType.POSTGRES;
                    break;
                case "SQLite":
                    DatabaseType = DbType.SQLITE;
                    break;
            }
        }
        public IDbConnection CreateConnection()
        {
            switch (DatabaseType)
            {
                //case DbType.MSSQL:
                //    return new SqlConnection(_connectionString);
                //case DbType.POSTGRES:
                //    return new NpgsqlConnection(_connectionString);
                case DbType.SQLITE:
                    return new SqliteConnection(_connectionString);
            }
            throw new Exception("No Valid ConnectionString or DbType found. Valid ConnectionString type parameters: [ SQLITE ]");
        }
    }
}
