using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public class ContactParameter : Patient
    {
        public List<string> Cases { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Ward { get; set; }
    }
}