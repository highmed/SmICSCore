using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.AQL.General;

namespace SmICSCoreLib.AQL.InfectionsStatus
{
    public interface IInfectionsStatusFactory
    {
        Dictionary<string, SortedDictionary<DateTime, int>> Process(TimespanParameter parameter, string kindOfFinding);
    }
}
