using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public interface IMibiResultFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<MiBiResult> Process(Patient patient);
        List<MiBiResult> Process(Case Case);
    }
}