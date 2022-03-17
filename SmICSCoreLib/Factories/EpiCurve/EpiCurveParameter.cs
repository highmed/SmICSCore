using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public class EpiCurveParameter : TimespanParameter
    {
        
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

        public EpiCurveParameter(TimespanParameter timespanParameter, string pathogen)
        {
            Starttime = timespanParameter.Starttime;
            Endtime = timespanParameter.Endtime;
            Pathogen = pathogen;
        }

        
    }
}