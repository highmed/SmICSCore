using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi
{
    public interface IAntibiogramFactory
    {
        List<Antibiogram> Process(AntibiogramParameter parameters);
    }
}