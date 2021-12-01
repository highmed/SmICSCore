using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class MiBiResult : Case
    {
        public string Status { get; set; }
        public DateTime ResultDateTime { get; set; }
        public string OrderID { get; set; }
        public PatientLocation Sender { get; set; }
        public List<Requirement> Requirements { get; set; }
        public List<Specimen> Specimens { get; set; }
        public string UID { get; set; }
    }
}
