using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class Pathogen
    {
        private const string NACHWEIS = "Nachweis";
        public string Name { get; set; }
        public bool Result {
            get
            {
                return ResultString == NACHWEIS ? true : false;
            }
        }
        public string ResultString { private get; set; }
        public DateTime Timestamp { get; set; }
        public string Rate { get; set; }
        public string IsolatNr { get; set; }
        public List<Antibiogram> Antibiograms { get; set; }
    }
}