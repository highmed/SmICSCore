using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.MibiLabData;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.PatientMovement.ReceiveModels;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public class WardOverviewFactory : IWardOverviewFactory
    {
        public IRestDataAccess RestDataAccess { get; private set; }
        private IMibiPatientLaborDataFactory _mibiLab;
        private ILogger<WardOverviewFactory> _logger;
        public WardOverviewFactory(IRestDataAccess rest, ILogger<WardOverviewFactory> logger, IMibiPatientLaborDataFactory mibiLab)
        {
            _logger = logger;
            RestDataAccess = rest;
            _mibiLab = mibiLab;
        }

        public List<Case> Processm(WardOverviewParameters parameter)
        {
            List<Case> cases = RestDataAccess.AQLQuery<Case>(PatientsOnWardQuery(parameter));
            return cases;
        }

        public List<WardOverviewModel> Process(WardOverviewParameters parameters)
        {
            List<Case> patients = GetAllPatientsOnWardInTimeSpan(parameters);
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
            foreach (Case patient in labDataForPatients.Keys)
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

        private bool IsResultFromParameterWard(MibiLabDataModel labDataWithinTime, WardOverviewParameters parameters, Case patient)
        {
            PatientLocation location = RestDataAccess.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(labDataWithinTime.ZeitpunktProbenentnahme, patient.PatientID)).FirstOrDefault() ?? null;
            if (location != null && parameters.Ward == location.Ward)
            {
                return true;
            }
            return false;
        }

        private EpisodeOfCareModel getCurrentAdmission(MibiLabDataModel labDataWithinTime, Patient patient)
        {
            EpsiodeOfCareParameter episodeOfCare = new EpsiodeOfCareParameter() { PatientID = patient.PatientID, CaseID = labDataWithinTime.CaseID };
            EpisodeOfCareModel admission = RestDataAccess.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientAdmission(episodeOfCare))[0];
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
                    PatientListParameter patList = new PatientListParameter() { patientList = new List<string> { patient.PatientID } };
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

        private List<Case> GetAllPatientsOnWardInTimeSpan(WardOverviewParameters parameters)
        {
            List<Case> patients = RestDataAccess.AQLQuery<Case>(PatientsOnWardQuery(parameters));
            if (patients != null)
            {
                return patients;
            }
            return null;
        }

        /// <summary>
        /// Returns all patients on given ward where the admission is greater equal than the given start and less equal than the given end. 
        /// </summary>
        /// <param name="parameter"></param>
        private AQLQuery PatientsOnWardQuery(WardOverviewParameters parameter)
        {
            return new AQLQuery()
            {
                Name = "PatientsOnWard",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value/value as CaseID
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                        CONTAINS (CLUSTER u[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                        AND CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]))
                        WHERE c/name/value = 'Patientenaufenthalt' 
                        AND a/items[at0027]/value/value = '{parameter.Ward}' 
                        AND u/data[at0001]/items[at0004]/value/value >= '{ parameter.Start.ToString("yyyy-MM-dd") }'
                        AND u/data[at0001]/items[at0004]/value/value <= '{ parameter.End.ToString("yyyy-MM-dd") }'"
            };
        }
    }
}
