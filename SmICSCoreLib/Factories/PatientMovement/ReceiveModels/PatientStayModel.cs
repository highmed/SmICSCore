using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientMovement.ReceiveModels
{
    public class PatientStayModel
    {
        public string PatientID { get; set; }
        public DateTime Beginn { get; set; }
        public DateTime Ende { get; set; }
        
        public string Fachabteilung { get; set; }
        public string StationID { get; set; }
        public string FallID { get; set; }
        public string FachabteilungsID { get; set; }
        public string Raum { get; set; }
        public string Bewegungsart_l { get; set; }
    }
}
