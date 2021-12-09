using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class Pathogen
    {
        public string Name { get; set; }
        public bool Result { get; set; }
        public DateTime Timestamp { get; set; }
        public string Rate
        {
            get
            {
                if (MedicalField.VIROLOGY == MedicalField)
                {
                    return Rate + " " + Unit;
                }
                return Rate;
            }
            set
            {
                Rate = value;
            }
        }
        public string IsolatNr { get; set; }
        public List<Antibiogram> Antibiograms { get; set; }
        private string Unit { get; set; }
        public MedicalField MedicalField { get; private set; }
    }
}