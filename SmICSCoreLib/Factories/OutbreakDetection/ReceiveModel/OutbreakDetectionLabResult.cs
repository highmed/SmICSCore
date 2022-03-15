using SmICSCoreLib.Factories.General;
using System;

namespace SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel
{
    public class OutbreakDetectionLabResult : Case
    {
        public int Result { get; set; }
        public string UID { get; set; }
        public DateTime SpecimenCollectionDateTime { get; set; }
    }
}
