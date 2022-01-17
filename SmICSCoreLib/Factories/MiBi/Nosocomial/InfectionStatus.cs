using System;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public class InfectionStatus
    {
        public bool Nosocomial { get; set; }
        public DateTime? NosocomialDate { get; set; }
        public bool Known { get; set; }
        public bool Infected { get; set; }
        public int ConsecutiveNegativeCounter { get; set; }
        public bool Healed { get; set; }
        public string Pathogen { get; set; }
        public string Resistance { get; set; }
        public string LabID { get; set; }
    }
}
