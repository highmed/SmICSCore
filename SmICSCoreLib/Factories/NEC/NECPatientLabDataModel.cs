using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECPatientLabDataModel
    {
        public string PatientID { get; set; }
        public string MaterialID { get; set; }
        public DateTime ZeitpunktProbeentnahme { get; set; }
        public DateTime Befunddatum { get; set; }
        public bool Befund { get; set; }
        public string KeimID { get; set; }
    }
}
