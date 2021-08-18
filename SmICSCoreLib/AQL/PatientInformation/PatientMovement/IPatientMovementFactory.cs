using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung
{
    public interface IPatientMovementFactory
    {
        List<PatientMovementModel> Process(PatientListParameter parameter);
        List<PatientMovementModel> ProcessFromStation(PatientListParameter parameter, string station, DateTime starttime, DateTime endtime);
        List<PatientMovementModel> ProcessGetStations();
    }
}