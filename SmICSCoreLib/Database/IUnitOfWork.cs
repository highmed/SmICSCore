using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public interface IUnitOfWork
    {
        //BlAttribute as Test 
        IRepository<BlAttribute> BlAttribute { get; }

        IRepository<DailyReport> DailyReport { get; }
    }
}
