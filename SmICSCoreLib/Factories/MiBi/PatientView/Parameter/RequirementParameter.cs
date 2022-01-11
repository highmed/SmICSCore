namespace SmICSCoreLib.Factories.MiBi.PatientView.Parameter
{
    public  class RequirementParameter : SpecimenParameter
    {
        public RequirementParameter() { }
        public RequirementParameter(SpecimenParameter parameter) : base(parameter) { }
        public RequirementParameter(RequirementParameter parameter) : base(parameter){ }
    }
}