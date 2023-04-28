using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.PatientMovementNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.ContactNew
{
    public class IndexPatient : Patient
    {
        public string? CurrentCase { get; set; }
        public List<string>? ChoosenPathoghen { get; set; }
        public string? ChoosenMRE { get; set; }
        public List<Hospitalization> Hospitalizations { get; set; }
        public Dictionary<Case, ContactPatient> ContactHistory { get; set;}
        public Dictionary<Case, InfectionStatus> InfectionStatusHistory { get; set; }
    }
}
