using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public class PatientInfectionModel
    {
        public string PatientID { get; set; }
        public string PathogenName { get; set; }

        public string InfectionWard { get; set; }

        private bool _isInfected;
        public bool IsInfected
        {
            get
            {
                return _isInfected;
            }
            set
            {
                if (value)
                {
                    HasFirstNegativeTest = false;
                    _isInfected = value;
                }
            }
        }
        public bool HasFirstNegativeTest { get; set; } = false;
    }
}
