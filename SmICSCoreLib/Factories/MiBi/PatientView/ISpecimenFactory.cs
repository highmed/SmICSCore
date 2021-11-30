using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface ISpecimenFactory
    {
        IRestDataAccess _restDataAccess { get; set; }

        List<Specimen> Process(SpecimenParameter parameter);
    }
}