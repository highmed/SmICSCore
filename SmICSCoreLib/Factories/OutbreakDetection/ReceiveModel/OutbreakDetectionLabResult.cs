using SmICSCoreLib.Factories.General;
using System;

namespace SmICSCoreLib.Factories.OutbreakDetection.ReceiveModel
{
    public class OutbreakDetectionLabResult : Case
    {
        public DateTime ResultDate { get; set; }
        public int Result { get; set; }
    }
}
