using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.MiBi.Nosocomial;

namespace SmICSWebApp.Data.Contact
{
    public class ContactService
    {
        private readonly IContactFactory _contactFac;
        private readonly InfectionStatusFactory _infectionStatusFac;
        public ContactService(IContactFactory contactFac, InfectionStatusFactory infectionStatusFac)
        {
            _contactFac = contactFac;
            _infectionStatusFac = infectionStatusFac;
        }

        public void LoadContactData(Patient patient)
        {
            _contactFac.Process();
        }
        

        
    }
}
