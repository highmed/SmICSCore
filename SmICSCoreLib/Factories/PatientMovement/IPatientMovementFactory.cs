using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovement
{
    public interface IPatientMovementFactory
    {
        IRestDataAccess RestDataAccess { get; }
        List<PatientMovementModel> Process(PatientListParameter parameter);
        List<PatientMovementModel> ProcessFromStation(PatientListParameter parameter, string station, DateTime starttime, DateTime endtime);
        List<PatientMovementModel> ProcessGetStations();
    }
}