using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;


namespace SmICSCoreLib.DB
{
    public class DataAccess : IDataAccess
    {
        public DapperContext DBContext { get; }
        public DataAccess(DapperContext context)
        {
            DBContext = context;
        }
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {

            using (IDbConnection connection = DBContext.CreateConnection())
            {
                var rows = connection.Query<T>(sql, parameters);

                return rows.ToList();
            }
              
        }
        
        public async Task SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = DBContext.CreateConnection())
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
