using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.EpiCurve.ReceiveModel
{
    public class FlagTimeModel
    {
        public string PatientID { get; set; }
        public string FallID { get; set; }
        public string Virus { get; set; }
        public string VirusCode { get; set; }
        public string Flag { get; set; }
        public DateTime Datum { get; set; }

        public bool HasFlag() 
        {
            return Flag == "260373001" ? true : false;
        }

    }
}
