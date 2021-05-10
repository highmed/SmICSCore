using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement.ReceiveModels;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.AQL.MiBi.WardOverview
{
    public class WardOverviewFactory
    {
        private IRestDataAccess _rest;
        private IMibiPatientLaborDataFactory _mibiLab;
        private ILogger<WardOverviewFactory> _logger;
        public WardOverviewFactory(IRestDataAccess rest, ILogger<WardOverviewFactory> logger, IMibiPatientLaborDataFactory mibiLab)
        {
            _logger = logger;
            _rest = rest;
            _mibiLab = mibiLab;
    }

        public void Process(WardOverviewParameters parameters)
        {
            List<Patient> patients = GetAllPatientsOnWardInTimeSpan(parameters);
            Dictionary<Patient, List<MibiLabDataModel>> labDataForPatients = GetAllLabResults(patients);
            IsMibiLabDataNosokomial(labDataForPatients);
        }

        private void IsMibiLabDataNosokomial(WardOverviewParameters parameters, Dictionary<Patient, List<MibiLabDataModel>> labDataForPatients)
        {
            WardOverviewModel wardOverview = new WardOverviewModel();
            if(labDataForPatients == null)
            {
                return;
            }
            foreach(Patient patient in labDataForPatients.Keys)
            {
                //labDataForPatients[patient].OrderBy(x => x.ZeitpunktProbenentnahme).Reverse();
                MibiLabDataModel labDataWithinTime = labDataForPatients[patient].Where(x => x.ZeitpunktProbenentnahme >= parameters.Start && x.ZeitpunktProbenentnahme <= parameters.End && x.Befund).OrderBy(x => x.ZeitpunktProbenentnahme).FirstOrDefault() ?? null;
                if(labDataWithinTime == null)
                {
                    //Schaue im er eventuell vorher und/oder woanders einen psoitiven Befund hat
                }
                wardOverview.Nosokomial = IsNosokomial(labDataForPatients[patient], labDataWithinTime, patient);
                wardOverview.PositivFinding = true;
                
            }
        }

        private bool IsNosokomial(List<MibiLabDataModel> mibiLabDataModels, MibiLabDataModel labDataWithinTime, Patient patient)
        {
            List<MibiLabDataModel> positivFindings = mibiLabDataModels.Where(x => x.Befund).ToList();
            if(positivFindings == null)
            {
                return false;
            }
            MibiLabDataModel firstPositiv = positivFindings.OrderBy(x => x.ZeitpunktProbenentnahme).First();
            if(firstPositiv == labDataWithinTime)
            {
                EpsiodeOfCareParameter episodeOfCare = new EpsiodeOfCareParameter() { PatientID = patient.EHRID, CaseID = labDataWithinTime.FallID };
                EpisodeOfCareModel admission = _rest.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientAdmission(episodeOfCare))[0];
                TimeSpan diff = labDataWithinTime.ZeitpunktProbenentnahme.Subtract(admission.Beginn);
                if(diff.TotalDays >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<Patient, List<MibiLabDataModel>> GetAllLabResults(List<Patient> patients)
        {
            if (patients != null)
            {
                Dictionary<Patient, List<MibiLabDataModel>> labDataForPatients = new Dictionary<Patient, List<MibiLabDataModel>>();
                foreach (Patient patient in patients)
                {
                    PatientListParameter patList = new PatientListParameter() { patientList = new List<string> { patient.EHRID } };
                    List<MibiLabDataModel> labData = _mibiLab.Process(patList);
                    labDataForPatients.Add(patient, labData);
                }
            }
            return null;
        }

        private List<Patient> GetAllPatientsOnWardInTimeSpan(WardOverviewParameters parameters)
        {
            List<Patient> patients = _rest.AQLQuery<Patient>(AQLCatalog.PatientsOnWard(parameters));
            if (patients != null)
            {
                return patients;
            }
            return null;
        }
    }
}
