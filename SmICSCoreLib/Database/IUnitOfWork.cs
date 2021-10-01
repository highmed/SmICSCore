using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public interface IUnitOfWork<T> where T : class
    {
        //Generic
        IRepository<T> Repository { get; }

        //Types
        IRepository<BlAttribute> BlAttribute { get; }
        IRepository<DailyReport> DailyReport { get; }

        //Table
        IRepository<BerichtNew> BerichtNew { get; }
        IRepository<BundeslandNew> BundeslandNew { get; }
    }
}
