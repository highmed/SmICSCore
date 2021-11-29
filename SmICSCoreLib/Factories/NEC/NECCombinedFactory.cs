using SmICSCoreLib.Factories.NEC.ReceiveModel;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SmICSCoreLib.Factories.Lab.MibiLabData;

namespace SmICSCoreLib.Factories.NEC
{
    public class NECCombinedFactory : INECCombinedFactory
    {
        public IRestDataAccess _restData;
        private IMibiPatientLaborDataFactory _labFac;
        private IPatientMovementFactory _movFac;

        public NECCombinedFactory(IRestDataAccess restData, IMibiPatientLaborDataFactory labFac, IPatientMovementFactory movFac)
        {
            _restData = restData;
            _labFac = labFac;
            _movFac = movFac;
        }

        public List<NECPatientInformation> Process(DateTime date)
        {
            List<PatientModel> currentPatients = GetCurrentPatients(date); 
            List<NECPatientInformation> currentData = new List<NECPatientInformation>();

            foreach (PatientModel pat in currentPatients)
            {
                Console.WriteLine(pat.PatientID);
                PatientListParameter patientParameter = PatientModelToPatientListParameter(pat);
                List<MibiLabDataModel> patLabs = _labFac.Process(patientParameter);
                List<PatientMovementModel> patMovements = _movFac.Process(patientParameter);
                if (patMovements.Count != 0)
                {
                    patMovements = patMovements.Where(m => (m.Beginn.Date <= date.Date && m.Ende.Date >= date.Date)).ToList();
                    patLabs = patLabs.Where(l => l.ZeitpunktProbenentnahme.Date == date.Date).ToList();

                    currentData.Add(CombineMovementsWitLab(patMovements, patLabs));
                }
            }


            return currentData;
        }

        private NECPatientInformation CombineMovementsWitLab(List<PatientMovementModel> Movements, List<MibiLabDataModel> Labs)
        {
            NECPatientInformation patInfo = new NECPatientInformation();
            patInfo.PatientInformation = new List<NECMovement>();

            patInfo.PatientID = Movements[0].PatientID;

            foreach (PatientMovementModel movement in Movements)
            {
                List<MibiLabDataModel> linkedLabs = Labs.Where(l => movement.Beginn < l.ZeitpunktProbenentnahme && movement.Ende >= l.ZeitpunktProbenentnahme).ToList();
                patInfo.PatientInformation.Add(new NECMovement()
                {
                    Admission = movement.Beginn,
                    Discharge = movement.Ende,
                    Ward = movement.StationID,
                    MovementType = movement.BewegungstypID,
                    LabData = linkedLabs != null ? reduceToNECData(linkedLabs) : null
                }); 
            }
            return patInfo;
        }

        private List<PatientModel> GetCurrentPatients(DateTime date)
        {
            List<PatientModel> admittedPatient = _restData.AQLQuery<PatientModel>(AQLCatalog.GetAllPatients(date));
            List<PatientModel> dischargedPatient = _restData.AQLQuery<PatientModel>(AQLCatalog.GetAllPatientsDischarged(date));
            if(admittedPatient == null)
            {
                return dischargedPatient;
            }
            else if(dischargedPatient == null)
            {
                return admittedPatient;
            }
            return admittedPatient.Concat(dischargedPatient).ToList();

        }
        private PatientListParameter PatientModelToPatientListParameter(PatientModel currentPatient)
        {
            PatientListParameter patientList = new PatientListParameter();
            patientList.patientList = new List<string>();
            patientList.patientList.Add(currentPatient.PatientID);
            return patientList;
        }

        private List<NECPatientLabDataModel> reduceToNECData(List<MibiLabDataModel> patientLabDatas)
        {
            List<NECPatientLabDataModel> necPatientData = new List<NECPatientLabDataModel>();
            foreach (MibiLabDataModel patientLabData in patientLabDatas)
            {
                NECPatientLabDataModel necPatientLabData = new NECPatientLabDataModel();
                necPatientLabData.Result = patientLabData.Befund;
                necPatientLabData.ResultDate = patientLabData.Befunddatum;
                necPatientLabData.Pathogen = patientLabData.KeimID;
                necPatientLabData.MaterialID = patientLabData.MaterialID;
                necPatientLabData.SpecimenColletionDate = patientLabData.ZeitpunktProbenentnahme;

                necPatientData.Add(necPatientLabData);
            }
            return necPatientData;
        }
    }
}
