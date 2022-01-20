using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class LabResult : Case
    {
        public string Status { get; set; }
        public DateTime ResultDateTime { get; set; }
        public string OrderID { get; set; }
        public PatientLocation Sender { get; set; }
        public string Requirement { get; set; }
        public List<Specimen> Specimens { get; set; }
        public string UID { get; set; }
        public object Comment { get; set; }
    }
}
