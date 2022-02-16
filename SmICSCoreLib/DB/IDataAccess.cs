using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.DB
{
    public interface IDataAccess
    {
        public DapperContext DBContext { get; }
        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
    }
}