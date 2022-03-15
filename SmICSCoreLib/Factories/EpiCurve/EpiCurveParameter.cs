using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public class EpiCurveParameter : TimespanParameter
    {
        public List<string> PathogenCodes { get; set; }
        public string Pathogen { get; set; }

        public string MedicalField
        { 
            get
            {
                if(Pathogen == "sars-cov-2")
                {
                    return General.MedicalField.VIROLOGY;
                }
                else
                {
                    return General.MedicalField.MICROBIOLOGY;
                }
            }
        }
        public EpiCurveParameter() { }

        public EpiCurveParameter(TimespanParameter timespanParameter, List<string> pathogenCodes, string pathogen)
        {
            Starttime = timespanParameter.Starttime;
            Endtime = timespanParameter.Endtime;
            PathogenCodes = pathogenCodes;
            Pathogen = pathogen;
        }

        public string PathogenCodesToAqlMatchString()
        {
            string convertedList = String.Join("','", PathogenCodes);
            return "{'" + convertedList + "'}";
        }
    }
}