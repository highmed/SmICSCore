using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public interface IWardOverviewFactory
    {
        List<WardOverviewModel> Process(WardOverviewParameters parameters);
    }
}