using System.Collections.Generic;

namespace SmICSWebApp.Data.Contact
{
    public class ContactParameter : SmICSCoreLib.Factories.General.Patient
    {
        public string Pathogen { get; set; }
        public string Resistence { get; set; }

        public List<string> PathogenCodes { get; set; }
    }
}
