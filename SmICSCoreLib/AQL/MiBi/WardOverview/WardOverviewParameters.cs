using System;

namespace SmICSCoreLib.AQL.MiBi.WardOverview
{
    public class WardOverviewParameters
    {
        public string Ward { get; set; }
        public DateTime Start { get; set; } = new DateTime(2021,5,26);
        public DateTime End { get; set; } = new DateTime(2021, 6, 5);
        public string MRE { get; set; } = "MRSA";
    }
}