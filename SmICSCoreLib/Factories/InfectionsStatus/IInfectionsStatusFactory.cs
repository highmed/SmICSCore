using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.InfectionsStatus
{
    public interface IInfectionsStatusFactory
    {
        Dictionary<string, SortedDictionary<DateTime, int>> Process(TimespanParameter parameter, string kindOfFinding);
    }
}
