using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovement
{
    public interface IPatientMovementFactory
    {
        List<PatientMovementModel> Process(PatientListParameter parameter);
        List<PatientMovementModel> ProcessFromStation(PatientListParameter parameter, string station, DateTime starttime, DateTime endtime);
    }
}