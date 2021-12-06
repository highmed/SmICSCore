using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovementNew.PatientStays
{
    public interface IPatientStayFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<PatientStay> Process(Case Case);
    }
}