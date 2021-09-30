using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public BaseRepository<BlAttribute> _blAttribute;
        public BaseRepository<DailyReport> _dailyReport;

        public BaseRepository<BerichtNew> _berichtNew;
        public BaseRepository<BundeslandNew> _bundeslandNew;

        //public BaseRepository<object> _repository;

        public IRepository<BlAttribute> BlAttribute
        {
            get
            {
                return _blAttribute ??
                    (_blAttribute = new BaseRepository<BlAttribute>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }
        public IRepository<DailyReport> DailyReport
        {
            get
            {
                return _dailyReport ??
                    (_dailyReport = new BaseRepository<DailyReport>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }

        public IRepository<BerichtNew> BerichtNew
        {
            get
            {
                return _berichtNew ??
                    (_berichtNew = new BaseRepository<BerichtNew>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }
        public IRepository<BundeslandNew> BundeslandNew
        {
            get
            {
                return _bundeslandNew ??
                    (_bundeslandNew = new BaseRepository<BundeslandNew>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
            }
        }

        //public IRepository<object> Repository
        //{
        //    get
        //    {
        //        return _repository ??
        //            (_repository = new BaseRepository<object>(DBConfig.DB_Url, DBConfig.DB_Keyspace));
        //    }
        //}

    }
}
