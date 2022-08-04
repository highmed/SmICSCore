using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.InfectionSituation;
using SmICSCoreLib.StatistikDataModels;
using System.Threading.Tasks;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.PatientMovement.ReceiveModels;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeFactory : INUMNodeFactory
    {
        private NUMNodeModel NUMNodeList;
        private List<NUMNodeModel> dataAggregationStorage;
        private LabPatientModel labPatient;
        private List<LabPatientModel> labPatientList;
        private List<LabDataReceiveModel> receiveLabDataListpositiv;
        private List<LabDataReceiveModel> receiveLabDataListnegativ;
        private List<NUMNodeCountModel> countStays;
        private List<NUMNodeCountModel> countContacts;
        private EpsiodeOfCareParameter episodeOfCareParameter;

        private double averageNumberOfStays;
        private double averageNumberOfNosCases;
        private double averageNumberOfMaybeNosCases;
        private double averageNumberOfContacts;

        private double medianNumberOfStays;
        private double medianNumberOfNosCases;
        private double medianNumberOfMaybeNosCases;
        private double medianNumberOfContacts;

        private double underQuartilNumberOfStays;
        private double underQuartilNumberOfNosCases;
        private double underQuartilNumberOfMaybeNosCases;
        private double underQuartilNumberOfContacts;

        private double upperQuartilNumberOfStays;
        private double upperQuartilNumberOfNosCases;
        private double upperQuartilNumberOfMaybeNosCases;
        private double upperQuartilNumberOfContacts;

        private static int countPatient;
        private static int numberOfStays;
        private static int numberOfNosCases;
        private static int numberOfMaybeNosCases;
        private static int numberOfContacts;

        private static int currentcountPatient;
        private static int currentnumberOfStays;
        private static int currentnumberOfNosCases;
        private static int currentnumberOfMaybeNosCases;
        private static int currentnumberOfContacts;

        private readonly string pathogen = "94500-6";
        private readonly string path = @"../SmICSWebApp/Resources/";

        public IRestDataAccess RestDataAccess { get; set; }
        private readonly ILogger<NUMNodeFactory> _logger;
        private readonly IInfectionSituationFactory _infecFac;

        public NUMNodeFactory(IRestDataAccess restDataAccess, ILogger<NUMNodeFactory> logger, IInfectionSituationFactory infecFac)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
            _infecFac = infecFac;
        }
        public void FirstDataEntry()
        {
            InitializeGlobalVariables();
            TimespanParameter timespan = new() { Starttime = DateTime.MinValue, Endtime = DateTime.Now };
            Process(timespan);
        }

        public void RegularDataEntry()
        {
            InitializeGlobalVariables();
            TimespanParameter timespan = new() { Starttime = DateTime.Now.AddDays(-7), Endtime = DateTime.Now };
            Process(timespan);
        }

        private void InitializeGlobalVariables()
        {
            NUMNodeList = new NUMNodeModel();
            dataAggregationStorage = new List<NUMNodeModel>();
            receiveLabDataListnegativ = new List<LabDataReceiveModel>();
            countStays = new List<NUMNodeCountModel>();
            labPatient = new LabPatientModel();
            labPatientList = new List<LabPatientModel>();
            receiveLabDataListpositiv = new List<LabDataReceiveModel>();
        }

        private async void Process(TimespanParameter timespan)
        {
            try
            {
                Task getPatList = Task.Run(() =>GetLabPatientList(timespan));
                await getPatList;

                var tasks = new Task[]
                {
                    GetNumberOfStays(),
                    GetNumberOfNosCases(),
                    GetNumberOfContacts()
                };

                foreach(var task in tasks)
                {
                    await task;
                }

                NUMNodeList = new NUMNodeModel() {
                    Provider = "MHH",
                    CDDV = "0.3.0",
                    Timestamp = DateTime.Now,
                    Author = "SmICS",
                    Dataitems = new List<NUMNodeDataItems>(){
                        new NUMNodeDataItems(){
                            Itemname = "current.patientstays",
                            Itemtype = "aggregated",
                            Data = new NUMNodeData(){
                                average = averageNumberOfStays,
                                median = medianNumberOfStays,
                                underquartil = underQuartilNumberOfStays,
                                upperquartil = upperQuartilNumberOfStays,
                                max = labPatientList.Max(a => a.CountStays),
                                min = labPatientList.Min(a => a.CountStays)
                            }
                        },
                        new NUMNodeDataItems()
                        {
                            Itemname = "current.patientmaybenoscases",
                            Itemtype = "aggregated",
                            Data = new NUMNodeData(){
                                average = averageNumberOfMaybeNosCases,
                                median = medianNumberOfMaybeNosCases,
                                underquartil = underQuartilNumberOfMaybeNosCases,
                                upperquartil = upperQuartilNumberOfMaybeNosCases,
                                max = labPatientList.Max(a => a.CountMaybeNosCases),
                                min = labPatientList.Min(a => a.CountMaybeNosCases)
                            }
                        },
                        new NUMNodeDataItems()
                        {
                            Itemname = "current.patientnoscases",
                            Itemtype = "aggregated",
                            Data = new NUMNodeData(){
                                average = averageNumberOfNosCases,
                                median = medianNumberOfNosCases,
                                underquartil = underQuartilNumberOfNosCases,
                                upperquartil = upperQuartilNumberOfNosCases,
                                max = labPatientList.Max(a => a.CountNosCases),
                                min = labPatientList.Min(a => a.CountNosCases)
                            }
                        },
                        new NUMNodeDataItems()
                        {
                            Itemname = "current.patientcontacts",
                            Itemtype = "aggregated",
                            Data = new NUMNodeData(){
                                average = averageNumberOfContacts,
                                median = medianNumberOfContacts,
                                underquartil = underQuartilNumberOfContacts,
                                upperquartil = upperQuartilNumberOfContacts,
                                max = labPatientList.Max(a => a.CountContacts),
                                min = labPatientList.Min(a => a.CountContacts)
                            }
                        }
                    }
                };
                dataAggregationStorage.Add(NUMNodeList);

                JSONFileStream.JSONWriter.Write(dataAggregationStorage, path, "NUMNode");
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can not get aggregated Data " + e.Message);
            }
        }

        private void GetLabPatientList(TimespanParameter timespan)
        {
            receiveLabDataListpositiv = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborPositivData(timespan, pathogen));
            if(receiveLabDataListpositiv is not null)
            {
                foreach(var pat in receiveLabDataListpositiv)
                {
                    receiveLabDataListnegativ = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborNegativData(timespan, pathogen, pat));
                    if (receiveLabDataListnegativ is null || receiveLabDataListnegativ.Count < 2)
                    {
                        episodeOfCareParameter = new EpsiodeOfCareParameter { PatientID = pat.PatientID, CaseID = pat.FallID };
                        List<EpisodeOfCareModel> discharge = RestDataAccess.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientDischarge(episodeOfCareParameter));
                        if (discharge is not null)
                        {
                            labPatient = new LabPatientModel { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = discharge.First().Ende };
                            countPatient++;
                        }
                        else
                        {
                            labPatient = new LabPatientModel { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = null };
                            currentcountPatient++;
                        }
                    }
                    else if (receiveLabDataListnegativ is not null && receiveLabDataListnegativ.Count > 1)
                    {
                        labPatient = new LabPatientModel { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = receiveLabDataListnegativ.OrderBy(a => a.Befunddatum).ElementAt(1).Befunddatum };
                        countPatient++;
                    }
                    labPatientList.Add(labPatient);
                }  
            }
            else
            {
                _logger.LogWarning("No positiv patient data has been found.");
            }
        }

        private async Task GetNumberOfStays()
        {
            foreach(var labPatient in labPatientList)
            {
                if(labPatient.Endtime is null)
                {
                    labPatient.Endtime = DateTime.Today;
                }
                countStays = RestDataAccess.AQLQuery<NUMNodeCountModel>(GetStaysCount(labPatient));
                foreach (NUMNodeCountModel count in countStays)
                {
                    if(labPatient.Endtime == DateTime.Today)
                    {
                        currentnumberOfStays += count.Count;
                    }else
                    {
                        numberOfStays += count.Count;
                    }
                    labPatient.CountStays += count.Count;  
                }
            }

            averageNumberOfStays = NUMNodeStatistics.GetAverage(numberOfStays + currentnumberOfStays, countPatient + currentcountPatient);
            (medianNumberOfStays, underQuartilNumberOfStays, upperQuartilNumberOfStays) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "stay");

            await Task.CompletedTask;
        }

        private async Task GetNumberOfNosCases()
        {
            List<string> patlist = new();
            List<string> currentpatlist = new();

            if (labPatient.Endtime is null)
            {
                labPatient.Endtime = DateTime.Today;
            }

            foreach (var labPatient in labPatientList)
            {
                if(labPatient.Endtime == DateTime.Today)
                {
                    currentpatlist.Add(labPatient.PatientID);
                }
                else
                {
                    patlist.Add(labPatient.PatientID);
                }
            }

            PatientListParameter patList = new() { patientList = patlist };
            PatientListParameter currentpatList = new() { patientList = currentpatlist };
            List<PatientModel> list = _infecFac.Process(patList);
            List<PatientModel> currentlist = _infecFac.Process(currentpatList);

            if (list is not null & currentlist is not null)
            {
                numberOfMaybeNosCases = list.Where(x => x.Infektion == "Moegliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count();
                numberOfNosCases = list.Where(x => x.Infektion == "Wahrscheinliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count();
                currentnumberOfMaybeNosCases = currentlist.Where(x => x.Infektion == "Moegliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count();
                currentnumberOfNosCases = currentlist.Where(x => x.Infektion == "Wahrscheinliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count();

                foreach(LabPatientModel labPatient in labPatientList)
                {
                    labPatient.CountMaybeNosCases = (list.Where(x => x.PatientID == labPatient.PatientID && x.Infektion == "Moegliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count())
                        + (currentlist.Where(x => x.PatientID == labPatient.PatientID && x.Infektion == "Moegliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count());
                    labPatient.CountNosCases = (list.Where(x => x.PatientID == labPatient.PatientID && x.Infektion == "Wahrscheinliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count())
                        + (currentlist.Where(x => x.PatientID == labPatient.PatientID && x.Infektion == "Wahrscheinliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count());
                }
            }
            else
            {
                numberOfMaybeNosCases = 0;
                numberOfNosCases = 0;
                currentnumberOfMaybeNosCases = 0;
                currentnumberOfNosCases = 0;
            }

            averageNumberOfMaybeNosCases = NUMNodeStatistics.GetAverage(numberOfMaybeNosCases + currentnumberOfMaybeNosCases, countPatient + currentcountPatient);
            averageNumberOfNosCases = NUMNodeStatistics.GetAverage(numberOfNosCases + currentnumberOfNosCases, countPatient + currentcountPatient);
            (medianNumberOfMaybeNosCases, underQuartilNumberOfMaybeNosCases, upperQuartilNumberOfMaybeNosCases) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "maybeNosCase");
            (medianNumberOfNosCases, underQuartilNumberOfNosCases, upperQuartilNumberOfNosCases) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "nosCase");

            await Task.CompletedTask;
        }
        //not sure if it works right
        private async Task GetNumberOfContacts()
        {
            foreach (var labPatient in labPatientList)
            {
                if (labPatient.Endtime is null)
                {
                    labPatient.Endtime = DateTime.Today;
                }
                List<ContactParameter> patStay = RestDataAccess.AQLQuery<ContactParameter>(GetStays(labPatient));
                if(patStay is not null)
                {
                    foreach (var stay in patStay)
                    {
                        countContacts = RestDataAccess.AQLQuery<NUMNodeCountModel>(GetContactsCount(labPatient, stay));
                        foreach (NUMNodeCountModel count in countContacts)
                        {
                            if(labPatient.Endtime == DateTime.Today)
                            {
                                currentnumberOfContacts += count.Count;
                            }
                            else
                            {
                                numberOfContacts += count.Count;
                            }
                            labPatient.CountContacts += count.Count;
                        }
                    }
                }else
                {
                    _logger.LogWarning("No patient movements has been found.");
                }
            }

            averageNumberOfContacts = NUMNodeStatistics.GetAverage(numberOfContacts + currentnumberOfContacts, countPatient + currentcountPatient);
            (medianNumberOfContacts, underQuartilNumberOfContacts, upperQuartilNumberOfContacts) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "contact");

            await Task.CompletedTask;
        }

        private AQLQuery GetContactsCount(LabPatientModel labpatient, ContactParameter patStay)
        {
            AQLQuery aql = new()
            {
                Name = "getStaysCount",
                Query = $@"SELECT COUNT(Distinct g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value) AS Count
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                        CONTAINS (CLUSTER g[openEHR-EHR-CLUSTER.case_identification.v0] and ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]
                        CONTAINS (CLUSTER z[openEHR-EHR-CLUSTER.location.v1])) 
                        WHERE c/name/value='Patientenaufenthalt'
                        AND (a/data[at0001]/items[at0004]/value/value <= '{patStay.End?.ToString("yyyy-MM-dd")}' 
                        AND a/data[at0001]/items[at0005]/value/value >='{patStay.Start?.ToString("yyyy-MM-dd")}') 
                        AND NOT e/ehr_status/subject/external_ref/id/value = '{labpatient.PatientID}'
                        AND z/items[at0027]/value = '{patStay.Ward}'"
            };
            return aql;
        }

        private AQLQuery GetStays(LabPatientModel labpatient)
        {
            AQLQuery aql = new()
            {
                Name = "getStays",
                Query = $@"SELECT p/data[at0001]/items[at0004]/value/value AS Start,
                                p/data[at0001]/items[at0005]/value/value AS End,
                                y/items[at0027]/value/value AS Ward
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS (CLUSTER h[openEHR-EHR-CLUSTER.case_identification.v0] AND ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER y[openEHR-EHR-CLUSTER.location.v1] and CLUSTER a[openEHR-EHR-CLUSTER.organization.v0])) 
                                WHERE c/name/value = 'Patientenaufenthalt'
                                AND h/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value = '{labpatient.CaseID}'
                                AND p/data[at0001]/items[at0004]/value/value <= '{labpatient.Endtime?.ToString("yyyy-MM-dd")}' 
                                AND (p/data[at0001]/items[at0005]/value/value >='{labpatient.Starttime.ToString("yyyy-MM-dd")}'
                                OR NOT EXISTS p/data[at0001]/items[at0005]/value/value)"
            };
            return aql;
        }

        private AQLQuery GetStaysCount(LabPatientModel labpatient)
        {
            AQLQuery aql = new()
            {
                Name = "getStaysCount",
                Query = $@"SELECT COUNT(g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value) AS Count
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                        CONTAINS (CLUSTER g[openEHR-EHR-CLUSTER.case_identification.v0] and ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]) 
                        WHERE c/name/value='Patientenaufenthalt'
                        AND g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value = '{labpatient.CaseID}'
                        AND a/data[at0001]/items[at0004]/value/value <= '{labpatient.Endtime?.ToString("yyyy-MM-dd")}' 
                        AND (a/data[at0001]/items[at0005]/value/value >='{labpatient.Starttime.ToString("yyyy-MM-dd")}' 
                        OR NOT EXISTS a/data[at0001]/items[at0005]/value/value)"
            };
            return aql;
        }

        private AQLQuery LaborPositivData(TimespanParameter timespan, string pathogen)
        {
            AQLQuery aql = new()
            {
                Name = "LaborPositivData",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                i/items[at0001]/value/value as FallID,
                                d/items[at0001]/value/defining_code/code_string as BefundCode,
                                d/items[at0024]/value/defining_code/code_string as ProbeID,
                                m/items[at0015]/value/value as Befunddatum
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                                CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
                                AND CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                                WHERE c/name/value='Virologischer Befund' 
                                AND d/items[at0001]/name/value='Nachweis'
                                AND d/items[at0001,'Nachweis']/value/defining_code/code_string = '260373001'
                                AND d/items[at0024]/value/defining_code/code_string = '{pathogen}'
                                AND m/items[at0015]/value/value>='{timespan.Starttime.ToString("yyyy-MM-dd")}' 
                                AND m/items[at0015]/value/value<'{timespan.Endtime.ToString("yyyy-MM-dd")}'"
            };
            return aql;
        }

        private AQLQuery LaborNegativData(TimespanParameter timespan, string pathogen, LabDataReceiveModel lab)
        {
            AQLQuery aql = new()
            {
                Name = "LaborNegativData",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                                i/items[at0001]/value/value as FallID,
                                d/items[at0001]/value/defining_code/code_string as BefundCode,
                                d/items[at0024]/value/defining_code/code_string as ProbeID,
                                m/items[at0015]/value/value as Befunddatum
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                                CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
                                AND CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                                WHERE c/name/value='Virologischer Befund' 
                                AND d/items[at0001]/name/value='Nachweis'
                                AND d/items[at0001,'Nachweis']/value/defining_code/code_string = '260415000'
                                AND d/items[at0024]/value/defining_code/code_string = '{pathogen}'
                                AND m/items[at0015]/value/value>='{timespan.Starttime.ToString("yyyy-MM-dd")}' 
                                AND m/items[at0015]/value/value<'{timespan.Endtime.ToString("yyyy-MM-dd")}'
                                AND  i/items[at0001]/value/value = '{lab.FallID}'"
            };
            return aql;
        }

    }
}
