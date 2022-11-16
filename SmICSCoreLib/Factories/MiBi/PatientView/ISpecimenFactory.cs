using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface ISpecimenFactory
    {
        IRestDataAccess _restDataAccess { get; set; }

        Task<List<Specimen>> ProcessAsync(SpecimenParameter parameter, PathogenParameter pathogen = null);
    }
}