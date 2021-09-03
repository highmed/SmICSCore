using System;
using System.Collections.Generic;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;

namespace SmICSCoreLib.AQL.Patient_Stay
{
    public interface IPatinet_Stay
    {
        List<StationaryDataModel> Stationary_Stay(string patientId, string fallkennung, DateTime datum);
        List<CountDataModel> CovidPat(string nachweis);
        List<StationaryDataModel> StayFromDate(DateTime datum);
        List<StationaryDataModel> StayFromCase(string patientId, string fallId);

    }
}
