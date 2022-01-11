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
                if (MedicalField == SmICSCoreLib.Factories.General.MedicalField.MICROBIOLOGY)
                {
                    return ResultString == NACHWEIS ? true : false;
                }
                else
                {
                    return ResultString == DETECTED ? true : false;
                }
            }
        }
        public string ResultString { private get; set; }
        public DateTime Timestamp { get; set; }
        private string _rate;
        public string Rate
        {
            get
            {
                if (SmICSCoreLib.Factories.General.MedicalField.VIROLOGY == MedicalField)
                {
                    return _rate + " " + Unit;
                }
                return _rate;
            }
            set
            {
                _rate = value;
            }
        }
        public string IsolatNr { get; set; }
        public List<Antibiogram> Antibiograms { get; set; }
        private string Unit { get; set; }
        public string MedicalField { get; private set; }

    }
}