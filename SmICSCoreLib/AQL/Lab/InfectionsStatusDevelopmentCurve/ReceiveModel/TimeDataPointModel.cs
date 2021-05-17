using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Lab.InfectionsStatusDevelopmentCurve.ReceiveModel
{
    public class TimeDataPointModel
    {
        public DateTime Zeitpunkt { get; set;}
        public string SpecimenIdentifier { get; set;}
        public string VirologicalTestResult { get; set;}
        public string DiseaseName { get; set;}
    }

    public class CountOfSpecimenIDAppearanceModel
    {
        public string SpecimenIdentifier { get; set;}
        public int CountOfSpecimenIDAppearance { get; set;}
    }
}
