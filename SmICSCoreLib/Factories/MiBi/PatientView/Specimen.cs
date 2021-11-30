using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class Specimen
    {
        public string Kind { get; set; }
        public string LabID { get; set; }
        public DateTime SpecimenCollectionDateTime { get; set; }
        public DateTime? SpecimenReceiptDate  { get; set; }
        public string Location { get; set; }
        public List<Pathogen> Pathogens { get; set; }
    }
}