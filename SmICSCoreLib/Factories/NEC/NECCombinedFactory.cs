using SmICSCoreLib.Factories.NEC.ReceiveModel;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECCombinedFactory : INECCombinedFactory
    {
        public IRestDataAccess _restData;
        private IViroLabDataFactory _labFac;
        private IPatientMovementFactory _movFac;

        public NECCombinedFactory(IRestDataAccess restData, IViroLabDataFactory labFac, IPatientMovementFactory movFac)
        {
            _restData = restData;
            _labFac = labFac;
            _movFac = movFac;
        }

        public NECCombinedDataModel Process(DateTime date)
        {
            List<PatientModel> currentPatients = _restData.AQLQuery<PatientModel>(AQLCatalog.GetAllPatients(date));
            List<NECRequestModel> currentData = new List<NECRequestModel>();

            foreach (PatientModel pat in currentPatients)
            {
                PatientListParameter patientParameter = PatientModelToPatientListParameter(pat);
                List<LabDataModel> patLabs = _labFac.Process(patientParameter);
                List<PatientMovementModel> patMovemenst = _movFac.Process(patientParameter);
                patLabs.Where(l => l.ZeitpunktProbenentnahme =)
                currentData = new NECRequestModel()
                {
                    labdat = reduceToNECData(patLabs),
                    movementData = reduceToNECData(patMovemenst)
                };
            }


            return currentData;
        }

        private PatientListParameter PatientModelToPatientListParameter(PatientModel currentPatient)
        {
            PatientListParameter patientList = new PatientListParameter();
            patientList.patientList = new List<string>();
            patientList.patientList.Add(currentPatient.PatientID);
            return patientList;
        }

        private List<NECPatientMovementDataModel> reduceToNECData(List<PatientMovementModel> patientMovements)
        {
            List<NECPatientMovementDataModel> necPatientData = new List<NECPatientMovementDataModel>();
            foreach (PatientMovementModel patientMovement in patientMovements)
            {
                NECPatientMovementDataModel necPatientMovementData = new NECPatientMovementDataModel();
                necPatientMovementData.PatientID = patientMovement.PatientID;
                necPatientMovementData.Beginn = patientMovement.Beginn;
                necPatientMovementData.Ende = patientMovement.Ende;
                necPatientMovementData.StationID = patientMovement.StationID;
                necPatientMovementData.BewegungstypID = patientMovement.BewegungstypID;

                necPatientData.Add(necPatientMovementData);
            }
            return necPatientData;
        }

        private List<NECPatientLabDataModel> reduceToNECData(List<LabDataModel> patientLabDatas)
        {
            List<NECPatientLabDataModel> necPatientData = new List<NECPatientLabDataModel>();
            foreach (LabDataModel patientLabData in patientLabDatas)
            {
                NECPatientLabDataModel necPatientLabData = new NECPatientLabDataModel();
                necPatientLabData.PatientID = patientLabData.PatientID;
                necPatientLabData.Befund = patientLabData.Befund;
                necPatientLabData.Befunddatum = patientLabData.Befunddatum.Value;
                necPatientLabData.KeimID = patientLabData.KeimID;
                necPatientLabData.MaterialID = patientLabData.MaterialID;
                necPatientLabData.ZeitpunktProbeentnahme = patientLabData.ZeitpunktProbenentnahme;

                necPatientData.Add(necPatientLabData);
            }
            return necPatientData;
        }
    }
}
