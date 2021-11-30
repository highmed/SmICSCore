using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;

namespace SmICSCoreLib.Factories.MiBi
{
    public class AntibiogramParameter : PathogenParameter
    { 
        public string IsolatNo { get; set; }
        public string Pathogen { get; set; }
    }
}