using SmICSDataGenerator.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSFactory.Tests
{
    public class PatientInfos : PatientIDs
    {
        public string FallID { get; set; }
        public DateTime Datum_Uhrzeit_der_Aufnahme { get; set; }
    }
}
