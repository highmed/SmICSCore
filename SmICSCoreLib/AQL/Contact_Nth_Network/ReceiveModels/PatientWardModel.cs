using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Contact_Nth_Network.ReceiveModels
{
    public class PatientWardModel
    {
        public DateTime Beginn { get; set; }
        public DateTime Ende { get; set; }
        public string Fachabteilung { get; set; }
        public string StationID { get; set; }
    }
}
