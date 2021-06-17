using System.Collections.Generic;

namespace SmICSCoreLib.AQL.MiBi
{
    public interface IAntibiogramFactory
    {
        List<Antibiogram> Process(AntibiogramParameter parameters);
    }
}