using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public interface IHospitalizationFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        Task<List<Hospitalization>> ProcessAsync(Patient patient);
        Task<Hospitalization> ProcessAsync(Case Case);
        Task<List<HospStay>> ProcessAsync(DateTime admission, DateTime? discharge);
    }
}