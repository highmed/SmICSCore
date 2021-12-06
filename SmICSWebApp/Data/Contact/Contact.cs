using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;

namespace SmICSWebApp.Data.Contact
{
    public class Contact : Case
    {
        public InfectionStatus InfectionStatus { get; set; }
        public PatientLocation PatientLocation { get; internal set; }
    }
}
