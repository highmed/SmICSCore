using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public class PathogenInfectionStatuses : Case
    {
        public string Pathogen { get; set; }
        public SortedList<DateTime, InfectionStatus> InfectionStatuses { get; set; }
    }
}
