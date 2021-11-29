using System;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECPatientLabDataModel
    {
        public string MaterialID { get; set; }
        public DateTime SpecimenColletionDate { get; set; }
        public DateTime? ResultDate { get; set; }
        public bool Result { get; set; }
        public string Pathogen { get; set; }
    }
}
