using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using System.Collections.Generic;

namespace SmICSWebApp.Data
{
    public class PatientViewModel
    {
        public List<PatientMovementModel> Movements { get; internal set; }
        public List<MibiLabDataModel> LabData { get; internal set; }
    }
}