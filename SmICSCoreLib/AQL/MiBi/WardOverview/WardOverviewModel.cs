using System;

namespace SmICSCoreLib.AQL.MiBi.WardOverview
{
    public class WardOverviewModel
    {
        public WardOverviewModel()
        {
        }

        public bool PositivFinding { get; internal set; }
        public bool Nosokomial { get; internal set; }
        public bool OnWard { get; internal set; }
        public DateTime TestDate { get; internal set; }
        public bool NewCase { get; internal set; }
        public string PatientID { get; internal set; }
    }
}