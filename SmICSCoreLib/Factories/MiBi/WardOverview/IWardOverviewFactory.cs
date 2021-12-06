using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public interface IWardOverviewFactory
    {
        List<WardOverviewModel> Process(WardOverviewParameters parameters);
        List<Case> Processm(WardOverviewParameters parameter);
    }
}