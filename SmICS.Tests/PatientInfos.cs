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
        public string StationID { get; set; }
        public string NameDesSymptoms { get; set; }
       
        public DateTime Beginn { get; set; }
        public DateTime Ende { get; set; }

        public DateTime Datum_Uhrzeit_der_Aufnahme { get; set; }
    }
}
