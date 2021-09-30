using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public interface IUnitOfWork
    {
        //Types
        //IRepository<object> Repository { get; }

        //Types
        IRepository<BlAttribute> BlAttribute { get; }
        IRepository<DailyReport> DailyReport { get; }

        //Table
        IRepository<BerichtNew> BerichtNew { get; }
        IRepository<BundeslandNew> BundeslandNew { get; }
    }
}
