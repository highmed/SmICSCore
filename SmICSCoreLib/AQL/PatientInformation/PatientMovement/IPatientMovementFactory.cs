using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung
{
    public interface IPatientMovementFactory
    {
        List<PatientMovementModel> Process(PatientListParameter parameter);
    }
}