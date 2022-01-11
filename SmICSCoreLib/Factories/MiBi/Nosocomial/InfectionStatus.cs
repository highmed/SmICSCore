using System;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public class InfectionStatus
    {
        public bool Nosocomial { get; set; }
        public DateTime? NosocomialDate { get; set; }
        public bool Known { get; set; }
    }
}
