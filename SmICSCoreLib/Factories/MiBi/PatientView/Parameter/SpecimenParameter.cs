using SmICSCoreLib.Factories.General;

namespace SmICSCoreLib.Factories.MiBi.PatientView.Parameter
{
    public class SpecimenParameter
    {
        public string UID { get; internal set; }

        public SpecimenParameter() {}
        public SpecimenParameter(SpecimenParameter parameter)
        {
            UID = parameter.UID;
        }
    }
}
