using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public class PatientWardLocation : Case
    {
        public string Ward { get; set; }
        public string Room { get; set; }
    }
}