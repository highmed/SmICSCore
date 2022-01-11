using System;

namespace SmICSCoreLib.Factories.MiBi.PatientView.Parameter
{
    public class PathogenParameter : SpecimenParameter
    {
        private string _name;
        public string Name { get
            {
                return _name;
            }
            set
            {
                _name = value;
                MedicalField = GetResultType(value);
            } 
        }
        public string LabID { get; internal set; }

        public PathogenParameter() { }
        public PathogenParameter(SpecimenParameter parameter) : base(parameter) {}
        public PathogenParameter(PathogenParameter parameter) : base(parameter) 
        {
            LabID = parameter.LabID;
        }
        public string MedicalField { get; private set; }

        private string GetResultType(string pathogenName)
        {
            if (!string.IsNullOrEmpty(pathogenName))
            {
                switch (pathogenName)
                {
                    case "Sars-Cov-2":
                        return SmICSCoreLib.Factories.General.MedicalField.VIROLOGY;
                    default:
                        return SmICSCoreLib.Factories.General.MedicalField.MICROBIOLOGY;
                }
            }
            throw new ArgumentNullException("Missing PathogenParameter");
        }
    }
}