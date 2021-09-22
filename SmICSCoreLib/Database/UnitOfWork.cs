using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.Test;

namespace SmICSCoreLib.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public BaseRepository<BlAttribute> _blAttribute;
        public BaseRepository<TestKlasse> _testKlasse;

        public IRepository<BlAttribute> BlAttribute
        {
            get
            {
                return _blAttribute ??
                    (_blAttribute = new BaseRepository<BlAttribute>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }

        public IRepository<TestKlasse> testKlasse
        {
            get
            {
                return _testKlasse ??
                    (_testKlasse = new BaseRepository<TestKlasse>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }
    }
}
