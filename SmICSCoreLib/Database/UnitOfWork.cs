using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        //Generic
        public BaseRepository<T> _repository;

        //Generic
        public IRepository<T> Repository
        {
            get
            {
                return _repository ??
                    (_repository = new BaseRepository<T>(DBConfig.DB_Url, DBConfig.DB_Keyspace, DBConfig.DB_User, DBConfig.DB_Password));
            }
        }

    }
}
