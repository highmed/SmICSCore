using System;
using System.Collections.Generic;
using SmICSCoreLib.Factories.PatientStay.Count;
using SmICSCoreLib.Factories.PatientStay.Stationary;

namespace SmICSCoreLib.Factories.PatientStay
{
    public interface IPatientStay
    {
        List<StationaryDataModel> Stationary_Stay(string patientId, string fallkennung, DateTime datum);
        List<CountDataModel> CovidPat(string nachweis);
        List<StationaryDataModel> StayFromDate(DateTime datum);
        List<StationaryDataModel> StayFromCase(string patientId, string fallId);

    }
}
