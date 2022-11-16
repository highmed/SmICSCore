using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi
{
    public interface IAntibiogramFactory
    {
        Task<List<Antibiogram>> ProcessAsync(AntibiogramParameter parameters);
    }
}