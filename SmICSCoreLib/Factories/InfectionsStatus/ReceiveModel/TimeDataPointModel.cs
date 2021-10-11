using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.InfectionsStatus.ReceiveModel
{
    public class TimeDataPointModel
    {
        public DateTime Zeitpunkt { get; set;}
        public string SpecimenIdentifier { get; set;}
        public string VirologicalTestResult { get; set;}
        public string DiseaseName { get; set;}
        public string StationID { get; set;}
        public int CodeForTestResult { get; set;}
        public string PatientID { get; set;}
    }
}
