using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface IPathogenFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<Pathogen> Process(PathogenParameter parameter);
    }
}