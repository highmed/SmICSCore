using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public interface IHospitalizationFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<Hospitalization> Process(Patient patient);
        Hospitalization Process(Case Case);
        List<HospStay> Process(DateTime admission, DateTime? discharge);
    }
}