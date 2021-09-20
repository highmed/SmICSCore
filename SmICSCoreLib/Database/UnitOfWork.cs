using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public BaseRepository<BlAttribute> _blAttribute;

        public IRepository<BlAttribute> BlAttribute
        {
            get
            {
                return _blAttribute ??
                    (_blAttribute = new BaseRepository<BlAttribute>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }

    }
}
