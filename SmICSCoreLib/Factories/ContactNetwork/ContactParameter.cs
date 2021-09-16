using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.Contact_Nth_Network
{
    public class ContactParameter : TimespanParameter
    {
        public string PatientID { get; set; }
        public int Degree { get; set; } = 1;
    }
}
