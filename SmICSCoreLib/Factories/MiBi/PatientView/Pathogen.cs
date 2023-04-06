using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class Pathogen
    {
        private const string NACHWEIS = "Nachweis";
        private const string DETECTED = "260373001";
        public string Name { get; set; }
        public string ID { get; set; }
        public bool Result {
            get
            {
                if (MedicalField == SmICSCoreLib.Factories.General.MedicalField.VIROLOGY)
                {
                    return ResultString == DETECTED ? true : false;
                }
                else
                {
                    return ResultString == NACHWEIS ? true : false;
                }
            }
        }
        public string ResultString { private get; set; }
        public string ResultText { get; set; }
        public DateTime? Timestamp { get; set; }

        public string Rate { get; set; }
        public string IsolatNr { get; set; }
        public List<Antibiogram> Antibiograms { get; set; }
        public string Unit { get; set; }
        public string MedicalField { get; private set; }

    }
}