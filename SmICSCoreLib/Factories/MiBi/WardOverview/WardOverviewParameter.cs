using System;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public class WardOverviewParameter
    {
        public string Ward { get; set; }
        public DateTime Start { get; set; } = new DateTime(2021, 5, 26);
        public DateTime End { get; set; } = new DateTime(2021, 6, 5);
        public string Pathogen { get; set; }
    }
}