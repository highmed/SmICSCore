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
    public class WardOverviewFactory : IWardOverviewFactory
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

        public List<WardOverviewModel> Process(WardOverviewParameters parameters)
        {
            List<Patient> patients = GetAllPatientsOnWardInTimeSpan(parameters);
            Dictionary<Patient, Dictionary<bool, List<MibiLabDataModel>>> labDataForPatients = GetAllLabResults(parameters, patients);
            List<WardOverviewModel> wardOverview = GetWardOverwievInformation(parameters, labDataForPatients);
            return wardOverview;
        }

        private List<WardOverviewModel> GetWardOverwievInformation(WardOverviewParameters parameters, Dictionary<Patient, Dictionary<bool, List<MibiLabDataModel>>> labDataForPatients)
        {
            List<WardOverviewModel> wardOverviews = new List<WardOverviewModel>();
            if (labDataForPatients == null)
            {
                return null;
            }
            foreach (Patient patient in labDataForPatients.Keys)
            {
                MibiLabDataModel labDataWithinTime = null;
                WardOverviewModel wardOverview = new WardOverviewModel();
                if (labDataForPatients[patient][true].Count > 0)
                {
                    labDataWithinTime = labDataForPatients[patient][true].Where(x => x.Befund & x.ZeitpunktProbeneingang >= parameters.Start).OrderBy(x => x.ZeitpunktProbenentnahme).FirstOrDefault() ?? null;
                    EpisodeOfCareModel admission = getCurrentAdmission(labDataWithinTime, patient);
                    if (labDataWithinTime == null)
                    {
                        continue;
                    }
                    Dictionary<string, bool> isNosokomailOrNewCase = IsNosokomialOrNewCase(labDataForPatients[patient][true], labDataWithinTime, patient, admission);
                    wardOverview.Nosokomial = isNosokomailOrNewCase["IsNosokomial"];
                    wardOverview.NewCase = isNosokomailOrNewCase["IsNewCase"];
                    wardOverview.PositivFinding = true;
                    wardOverview.OnWard = IsResultFromParameterWard(labDataWithinTime, parameters, patient);

                }
                else
                {
                    labDataWithinTime = labDataForPatients[patient][false].Where(x => x.Befund & x.ZeitpunktProbeneingang >= parameters.Start).OrderBy(x => x.ZeitpunktProbenentnahme).LastOrDefault() ?? null;
                    wardOverview.Nosokomial = false;
                    wardOverview.NewCase = false;
                    wardOverview.PositivFinding = false;
                    wardOverview.OnWard = false;
                }
                wardOverview.PatientID = patient.PatientID;
                wardOverviews.Add(wardOverview);
                wardOverview.TestDate = labDataWithinTime.ZeitpunktProbenentnahme;

            }
            return wardOverviews;
        }

        private bool IsResultFromParameterWard(MibiLabDataModel labDataWithinTime, WardOverviewParameters parameters, Patient patient)
        {
            PatientLocation location = _rest.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(labDataWithinTime.ZeitpunktProbenentnahme, patient.EHRID)).FirstOrDefault() ?? null;
            if (location != null && parameters.Ward == location.Ward)
            {
                return true;
            }
            return false;
        }

        private EpisodeOfCareModel getCurrentAdmission(MibiLabDataModel labDataWithinTime, Patient patient)
        {
            EpsiodeOfCareParameter episodeOfCare = new EpsiodeOfCareParameter() { PatientID = patient.EHRID, CaseID = labDataWithinTime.CaseID };
            EpisodeOfCareModel admission = _rest.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientAdmission(episodeOfCare))[0];
            return admission;
        }

        private Dictionary<string, bool> IsNosokomialOrNewCase(List<MibiLabDataModel> mibiLabDataModels, MibiLabDataModel labDataWithinTime, Patient patient, EpisodeOfCareModel admission)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool> { { "IsNosokomial", false }, { "IsNewCase", false } };
            List<MibiLabDataModel> positivFindings = mibiLabDataModels.Where(x => x.Befund).ToList();
            MibiLabDataModel firstEverPositiv = positivFindings.OrderBy(x => x.ZeitpunktProbenentnahme).First();
            MibiLabDataModel firstPositivThisStay = positivFindings.OrderBy(x => x.ZeitpunktProbenentnahme).Where(x => x.ZeitpunktProbenentnahme.Date >= admission.Beginn.Date).First();
            if (firstEverPositiv == firstPositivThisStay)
            {
                dict["IsNewCase"] = true;
                TimeSpan diff = labDataWithinTime.ZeitpunktProbenentnahme.Subtract(admission.Beginn);
                if (diff.TotalDays >= 3)
                {
                    dict["IsNosokomial"] = true;
                }
            }
            return dict;
        }

        private Dictionary<Patient, Dictionary<bool, List<MibiLabDataModel>>> GetAllLabResults(WardOverviewParameters parameters, List<Patient> patients)
        {
            if (patients != null)
            {
                Dictionary<Patient, Dictionary<bool, List<MibiLabDataModel>>> labDataForPatients = new Dictionary<Patient, Dictionary<bool, List<MibiLabDataModel>>>();
                foreach (Patient patient in patients)
                {
                    PatientListParameter patList = new PatientListParameter() { patientList = new List<string> { patient.EHRID } };
                    List<MibiLabDataModel> labData = _mibiLab.Process(patList);
                    foreach (MibiLabDataModel data in labData)
                    {
                        if (parameters.MRE == "MRSA")
                        {
                            List<Antibiogram> mrsa = data.Antibiogram.Where(x => x.Antibiotic == "Oxacillin" && x.Resistance == "R").ToList();
                            if (mrsa != null && mrsa.Count > 0)
                            {
                                if (labDataForPatients.ContainsKey(patient))
                                {
                                    labDataForPatients[patient][true] = labDataForPatients[patient][true].Concat(labData).ToList();
                                }
                                else
                                {
                                    labDataForPatients.Add(patient, new Dictionary<bool, List<MibiLabDataModel>> { { true, new List<MibiLabDataModel>() }, { false, new List<MibiLabDataModel>() } });
                                    labDataForPatients[patient][true] = labDataForPatients[patient][true].Concat(labData).ToList();
                                }
                            }
                            else
                            {
                                if (labDataForPatients.ContainsKey(patient))
                                {
                                    labDataForPatients[patient][false] = labDataForPatients[patient][false].Concat(labData).ToList();
                                }
                                else
                                {
                                    labDataForPatients.Add(patient, new Dictionary<bool, List<MibiLabDataModel>> { { true, new List<MibiLabDataModel>() }, { false, new List<MibiLabDataModel>() } });
                                    labDataForPatients[patient][false] = labDataForPatients[patient][false].Concat(labData).ToList();
                                }
                            }
                        }

                    }
                }
                return labDataForPatients;
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
