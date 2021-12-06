namespace SmICSCoreLib.Factories.MiBi.PatientView.Parameter
{
    public class PathogenParameter : SpecimenParameter
    {
        public string LabID { get; internal set; }

        public PathogenParameter() { }
        public PathogenParameter(SpecimenParameter parameter) : base(parameter) {}
        public PathogenParameter(PathogenParameter parameter) : base(parameter) 
        {
            LabID = parameter.LabID;
        }
    }
}