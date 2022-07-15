using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface IPathogenFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<Pathogen>> ProcessAsync(PathogenParameter parameter);
    }
}