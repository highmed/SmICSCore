using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Database
{
    public interface IUnitOfWork<T> where T : class
    {
        //Generic
        IRepository<T> Repository { get; }
  
    }
}
