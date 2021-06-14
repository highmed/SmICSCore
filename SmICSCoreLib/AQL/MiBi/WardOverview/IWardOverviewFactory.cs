using System.Collections.Generic;

namespace SmICSCoreLib.AQL.MiBi.WardOverview
{
    public interface IWardOverviewFactory
    {
        List<WardOverviewModel> Process(WardOverviewParameters parameters);
    }
}