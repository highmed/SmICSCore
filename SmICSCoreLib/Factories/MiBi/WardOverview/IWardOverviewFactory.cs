using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public interface IWardOverviewFactory
    {
        List<PatientLocation> Process(WardOverviewParameter parameter);
    }
}