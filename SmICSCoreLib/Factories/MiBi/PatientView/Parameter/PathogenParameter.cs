using SmICSCoreLib.Factories.General;
using System;

namespace SmICSCoreLib.Factories.MiBi.PatientView.Parameter
{
    public class PathogenParameter : SpecimenParameter
    {
        public string Name { get
            {
                return Name;
            }
            set
            {
                Name = value;
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
        public MedicalField MedicalField { get; private set; }

        private MedicalField GetResultType(string pathogenName)
        {
            if (!string.IsNullOrEmpty(pathogenName))
            {
                switch (pathogenName)
                {
                    case "Sars-Cov-2":
                        return MedicalField.VIROLOGY;
                    default:
                        return MedicalField.MICROBIOLOGY;
                }
            }
            throw new ArgumentNullException("Missing PathogenParameter");
        }
    }
}