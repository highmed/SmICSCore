using System;

namespace SmICSCoreLib.Factories.LabData.MibiLabdata.ReceiveModel
{
    public class SampleReceiveModel
    {
        public string LabordatenID { get; set; }
        public DateTime ZeitpunktProbeentnahme { get; set; }
        public DateTime ZeitpunktProbeneingang { get; set; }
        public string MaterialID { get; set; }
        public string Material_l { get; set; }
    }
}