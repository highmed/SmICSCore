using SmICSCoreLib.Factories.General;
using System;

namespace SmICSWebApp.Data.MedicalFinding
{
    public class VisuLabResult : Case
    {
        public string LabID { get; internal set; }
        public DateTime SpecimenCollectionDateTime { get; internal set; }
        public DateTime? SpecimenReceiptDateTime { get; internal set; }
        public string MaterialID { get; internal set; }
        public bool Result { get; internal set; }
        public string Pathogen { get; internal set; }
        public DateTime ResultDate { get; internal set; }
        public object Comment { get; internal set; }
        public string Quantity { get; internal set; }
    }
}