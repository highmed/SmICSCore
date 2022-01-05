using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;

namespace SmICSCoreLib.Factories.MiBi
{
    public class AntibiogramParameter : PathogenParameter
    { 
        public string IsolatNo { get; set; }
        public string Pathogen { get; set; }

        public AntibiogramParameter() {}
        public AntibiogramParameter(PathogenParameter parameter) :base(parameter){}
        public AntibiogramParameter(AntibiogramParameter parameter) :base(parameter)
        {
            IsolatNo = parameter.IsolatNo;
            Pathogen = parameter.Pathogen;
        }
    }
}