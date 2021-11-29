using System.Collections.Generic;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECPatientInformation
    {
        public string PatientID { get; internal set; }
        public List<NECMovement> PatientInformation { get; internal set; }
    }
}