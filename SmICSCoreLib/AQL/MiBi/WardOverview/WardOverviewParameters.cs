using System;

namespace SmICSCoreLib.AQL.MiBi.WardOverview
{
    public class WardOverviewParameters
    {
        public string Ward { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string MRE { get; set; }
    }
}