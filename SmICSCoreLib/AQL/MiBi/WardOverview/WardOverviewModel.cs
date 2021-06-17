using System;

namespace SmICSCoreLib.AQL.MiBi.WardOverview
{
    public class WardOverviewModel
    {
        public WardOverviewModel()
        {
        }

        public bool PositivFinding { get; set; }
        public bool Nosokomial { get; set; }
        public bool OnWard { get; set; }
        public DateTime TestDate { get; set; }
        public bool NewCase { get; set; }
        public string PatientID { get; set; }
    }
}