using SmICSCoreLib.AQL.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public class ContactParameter : TimespanParameter
    {
        public string PatientID { get; set; }
        public int Degree { get; set; }
    }
}
