using System;

namespace SmICSCoreLib.Factories.EpiCurve.ReceiveModel
{
    public class FlagTimeModel
    {
        public string PatientID { get; set; }
        public string FallID { get; set; }
        public string Pathogen { get; set; }
        public string PathogenCode { get; set; }
        public string Flag { get; set; }
        public DateTime Datum { get; set; }

        public bool HasFlag() 
        {
            return (Flag == "260373001" || Flag=="Nachweis") ? true : false;
        }

    }
}
