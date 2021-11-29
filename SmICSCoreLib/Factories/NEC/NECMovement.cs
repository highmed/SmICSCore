using SmICSCoreLib.Factories.Lab.MibiLabData;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECMovement
    {
        public DateTime Admission { get; internal set; }
        public DateTime Discharge { get; internal set; }
        public string Ward { get; internal set; }
        public int MovementType { get; internal set; }
        public List<NECPatientLabDataModel> LabData { get; internal set; }
    }
}