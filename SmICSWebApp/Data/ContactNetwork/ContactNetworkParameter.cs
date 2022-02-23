
using System;

namespace SmICSWebApp.Data.ContactNetwork
{
    public class ContactNetworkParameter : SmICSCoreLib.Factories.General.Patient
    {
        public int Degree { get; set; }
        public DateTime ContactStart { get; set; }
        public DateTime ContactEnd { get; set; }
        public string Pathogen { get; set; }

    }
}
