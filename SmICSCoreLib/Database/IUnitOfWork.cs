using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.Test;

namespace SmICSCoreLib.Database
{
    public interface IUnitOfWork
    {
        //BlAttribute as Test 
        IRepository<BlAttribute> BlAttribute { get; }
        IRepository<TestKlasse> testKlasse { get; }
    }
}
