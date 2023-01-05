using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private LabPatientModel labPatient;
        private List<LabPatientModel> labPatientList;
        private List<LabDataReceiveModel> receiveLabDataListpositiv;
        private List<LabDataReceiveModel> receiveLabDataListnegativ;
        private List<NUMNodeCountModel> countStays;
        private EpsiodeOfCareParameter episodeOfCareParameter;
        private List<NUMNodeDataItems> nodeDataItems;

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

        private double standardDeviationNumberOfStays;
        private double standardDeviationNumberOfNosCases;
        private double standardDeviationNumberOfMaybeNosCases;
        private double standardDeviationNumberOfContacts;

        private int countPatient;
        private int numberOfStays;
        private int numberOfNosCases;
        private int numberOfMaybeNosCases;
        private int numberOfContacts;

        private int currentcountPatient;
        private int currentnumberOfStays;
        private int currentnumberOfNosCases;
        private int currentnumberOfMaybeNosCases;
        private int currentnumberOfContacts;
        private NUMNodeSaveModel saveData;

        private readonly string pathogen = "94500-6";
        private readonly string path = @"../SmICSWebApp/Resources/NUMNode/";

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
            TimespanParameter timespan = new() { Starttime = DateTime.Now.AddYears(-10), Endtime = DateTime.Now };
            _ = Process(timespan);
        }

        public void RegularDataEntry()
        {
            InitializeGlobalVariables();
            try
            {
                saveData = JSONFileStream.JSONReader<NUMNodeSaveModel>.ReadObject(path + "NumNodeSave.json");

                countPatient = saveData.CountPatient;
                numberOfStays = saveData.NumberOfStays;
                numberOfNosCases = saveData.NumberOfNosCases;
                numberOfMaybeNosCases = saveData.NumberOfMaybeNosCases;
                numberOfContacts = saveData.NumberOfContacts;
            }catch (Exception e)
            {
                _logger.LogWarning("Cannot read saved data :" + e);
            }
            DateTime date = DateTime.Now;
            TimespanParameter timespan;
            if (date.Day == 1)
            {
                timespan = new() { Starttime = DateTime.Now.AddMonths(-1).AddDays(14), Endtime = DateTime.Now };
            }
            else
            {
                timespan = new() { Starttime = DateTime.Now.AddDays(-14), Endtime = DateTime.Now };
            }

            _ = Process(timespan);
        }

        private void InitializeGlobalVariables()
        {
            NUMNodeList = new NUMNodeModel();
            receiveLabDataListnegativ = new List<LabDataReceiveModel>();
            countStays = new List<NUMNodeCountModel>();
            labPatient = new LabPatientModel();
            labPatientList = new List<LabPatientModel>();
            receiveLabDataListpositiv = new List<LabDataReceiveModel>();
            saveData = new NUMNodeSaveModel();
            nodeDataItems = new List<NUMNodeDataItems>();
        }

        private async Task Process(TimespanParameter timespan)
        {
            try
            {
                await GetLabPatientList(timespan);

                var tasks = new Task[]
                {
                    GetNumberOfStays(),
                    GetNumberOfNosCases(),
                    //GetNumberOfContacts()
                };

                foreach(var task in tasks)
                {
                    await task;
                }

                SaveStaticData();
                List<string> itemNames = new() {
                    "current.wardsvisited",
                    "current.nosocomialcases.possible",
                    "current.nosocomialcases.probable" 
                    /*"current.patientcontacts"*/ };
                List<double> averageList = new() { averageNumberOfStays, averageNumberOfMaybeNosCases, averageNumberOfNosCases, averageNumberOfContacts };
                List<double> medianList = new() { medianNumberOfStays, medianNumberOfMaybeNosCases, medianNumberOfNosCases, medianNumberOfContacts };
                List<double> underquartilList = new() { underQuartilNumberOfStays, underQuartilNumberOfMaybeNosCases, underQuartilNumberOfNosCases, underQuartilNumberOfContacts };
                List<double> upperquartilList = new() { upperQuartilNumberOfStays, upperQuartilNumberOfMaybeNosCases, upperQuartilNumberOfNosCases, upperQuartilNumberOfContacts };
                List<double> maxList;
                List<double> minList;
                List<double> standardDeviationList = new() { standardDeviationNumberOfStays, standardDeviationNumberOfMaybeNosCases, standardDeviationNumberOfNosCases, standardDeviationNumberOfContacts };
                if (labPatientList.Count != 0)
                {
                    maxList = new() { labPatientList.Max(a => a.CountStays), labPatientList.Max(a => a.CountMaybeNosCases), labPatientList.Max(a => a.CountNosCases), labPatientList.Max(a => a.CountContacts) };
                    minList = new() { labPatientList.Min(a => a.CountStays), labPatientList.Min(a => a.CountMaybeNosCases), labPatientList.Min(a => a.CountNosCases), labPatientList.Min(a => a.CountContacts) };
                }
                else
                {
                    maxList = new() { 0, 0, 0, 0 };
                    minList = new() { 0, 0, 0, 0 };
                }               

                for (int i = 0; i < itemNames.Count-1; i++)
                {
                    nodeDataItems = new List<NUMNodeDataItems>(){
                            new NUMNodeDataItems(){
                                itemname = itemNames[i],
                                itemtype = "statsmean",
                                data = new NUMNodeData(){
                                    average = averageList[i],
                                    //median = medianList[i],
                                    //underquartil = underquartilList[i],
                                    //upperquartil = upperquartilList[i],
                                    //max = maxList[i],
                                    //min = minList[i],
                                    standard_dev = standardDeviationList[i],
                                    sample_size = countPatient + currentcountPatient
                                }
                            }
                        };
                }

                NUMNodeList = new NUMNodeModel()
                {
                    provider = "MHH-SmICS",
                    corona_dashboard_dataset_version = "0.3.0",
                    exporttimestamp = DateTime.Now,
                    author = "SmICS",
                    dataitems = nodeDataItems
                };
                JSONFileStream.JSONWriter.Write(NUMNodeList, path, "NUMNode_R" + DateTime.Today.ToString("yyyy_MM_dd"));
            }
            catch (Exception e)
            {
                _logger.LogWarning("Can not get aggregated Data " + e.Message);
            }
        }

        private async Task GetLabPatientList(TimespanParameter timespan)
        {
            try
            {
                receiveLabDataListpositiv = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborPositivData(timespan, pathogen));
                if (receiveLabDataListpositiv is not null)
                {
                    foreach (var pat in receiveLabDataListpositiv)
                    {
                        if (!labPatientList.Select(x => x.CaseID).Contains(pat.FallID))
                        {
                            receiveLabDataListnegativ = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborNegativData(timespan, pathogen, pat));

                            if (receiveLabDataListnegativ is null || receiveLabDataListnegativ.Count < 2)
                            {
                                episodeOfCareParameter = new EpsiodeOfCareParameter { PatientID = pat.PatientID, CaseID = pat.FallID };
                                List<EpisodeOfCareModel> discharge = RestDataAccess.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientDischarge(episodeOfCareParameter));
                                List<EpisodeOfCareModel> admission = RestDataAccess.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientAdmission(episodeOfCareParameter));
                                if (discharge is not null && admission is not null)
                                {
                                    LabPatientModel labPatient = new() { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = discharge.First().Ende };
                                    countPatient++;
                                    labPatientList.Add(labPatient);
                                }
                                else if (admission is not null)
                                {
                                    LabPatientModel labPatient = new() { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = null };
                                    currentcountPatient++;
                                    labPatientList.Add(labPatient);
                                }
                            }
                            else if (receiveLabDataListnegativ is not null && receiveLabDataListnegativ.Count > 1)
                            {
                                List<DateTime> posdates = receiveLabDataListpositiv.Where(x => x.FallID == pat.FallID).Select(a => a.Befunddatum).OrderBy(b => b.Date).ToList();
                                List<DateTime> negdates = receiveLabDataListnegativ.Select(x => x.Befunddatum).OrderBy(a => a.Date).ToList();
                                SortedList<DateTime, bool> alldates = new();

                                foreach (var item in posdates)
                                {
                                    if (!alldates.ContainsKey(item))
                                        alldates.Add(item, true);
                                }
                                foreach (var item in negdates)
                                {
                                    if (!alldates.ContainsKey(item))
                                        alldates.Add(item, false);
                                }
                                int dateTimeNegCount = 0;
                                bool newPosTimeframe = false;

                                foreach (var item in alldates)
                                {
                                    if (item.Value == false)
                                    {
                                        if (dateTimeNegCount == 1)
                                        {
                                            LabPatientModel labPatient = new() { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = item.Key };
                                            countPatient++;
                                            labPatientList.Add(labPatient);
                                            newPosTimeframe = true;
                                        }
                                        dateTimeNegCount++;
                                    }
                                    else
                                    {
                                        dateTimeNegCount = 0;
                                        if (newPosTimeframe == true)
                                        {
                                            episodeOfCareParameter = new EpsiodeOfCareParameter { PatientID = pat.PatientID, CaseID = pat.FallID };
                                            List<EpisodeOfCareModel> discharge = RestDataAccess.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientDischarge(episodeOfCareParameter));
                                            List<EpisodeOfCareModel> admission = RestDataAccess.AQLQuery<EpisodeOfCareModel>(AQLCatalog.PatientAdmission(episodeOfCareParameter));
                                            if (discharge is not null && admission is not null)
                                            {
                                                LabPatientModel labPatient = new() { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = item.Key, Endtime = discharge.First().Ende };
                                                countPatient++;
                                                labPatientList.Add(labPatient);
                                            }
                                            else if (admission is not null)
                                            {
                                                LabPatientModel labPatient = new() { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = item.Key, Endtime = null };
                                                currentcountPatient++;
                                                labPatientList.Add(labPatient);
                                            }
                                            newPosTimeframe = false;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                LabPatientModel labPatient = new() { PatientID = pat.PatientID, CaseID = pat.FallID, Starttime = pat.Befunddatum, Endtime = null };
                                currentcountPatient++;
                                labPatientList.Add(labPatient);
                            }

                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No positiv patient data has been found.");
                }
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot get positiv patient list :" + e);
            }          
        }

        private async Task GetNumberOfStays()
        {
            try
            {
                foreach (var labPatient in labPatientList)
                {
                    if (labPatient.Endtime is null)
                    {
                        labPatient.Endtime = DateTime.Today;
                    }
                    countStays = RestDataAccess.AQLQuery<NUMNodeCountModel>(GetStaysCount(labPatient));
                    foreach (NUMNodeCountModel count in countStays)
                    {
                        if (labPatient.Endtime == DateTime.Today)
                        {
                            currentnumberOfStays += count.Count;
                        }
                        else
                        {
                            numberOfStays += count.Count;
                        }
                        labPatient.CountStays += count.Count;
                    }
                }

                averageNumberOfStays = NUMNodeStatistics.GetAverage(numberOfStays + currentnumberOfStays, countPatient + currentcountPatient);
                (medianNumberOfStays, underQuartilNumberOfStays, upperQuartilNumberOfStays) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "stay");
                standardDeviationNumberOfStays = NUMNodeStatistics.GetStandardDeviation(labPatientList, countPatient + currentcountPatient, averageNumberOfStays, "stay");
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot get aggregated dataset for patient stays :" + e);
                await Task.CompletedTask;
            }         
        }

        private async Task GetNumberOfNosCases()
        {
            try
            {
                List<string> patlist = new();
                List<string> currentpatlist = new();
                if(labPatientList.Count != 0)
                {
                    if (labPatient.Endtime is null)
                    {
                        labPatient.Endtime = DateTime.Today;
                    }

                    foreach (var labPatient in labPatientList)
                    {
                        if (labPatient.Endtime == DateTime.Today)
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

                        foreach (LabPatientModel labPatient in labPatientList)
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
                }

                averageNumberOfMaybeNosCases = NUMNodeStatistics.GetAverage(numberOfMaybeNosCases + currentnumberOfMaybeNosCases, countPatient + currentcountPatient);
                averageNumberOfNosCases = NUMNodeStatistics.GetAverage(numberOfNosCases + currentnumberOfNosCases, countPatient + currentcountPatient);
                (medianNumberOfMaybeNosCases, underQuartilNumberOfMaybeNosCases, upperQuartilNumberOfMaybeNosCases) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "maybeNosCase");
                (medianNumberOfNosCases, underQuartilNumberOfNosCases, upperQuartilNumberOfNosCases) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "nosCase");
                standardDeviationNumberOfNosCases = NUMNodeStatistics.GetStandardDeviation(labPatientList, countPatient + currentcountPatient, averageNumberOfNosCases, "nosCase");
                standardDeviationNumberOfMaybeNosCases = NUMNodeStatistics.GetStandardDeviation(labPatientList, countPatient + currentcountPatient, averageNumberOfMaybeNosCases, "maybeNosCase");
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot get aggregated dataset for nosokomial infections :" + e);
                await Task.CompletedTask;
            }
            
        }
        //to much server load
        private async Task GetNumberOfContacts()
        {
            try
            {
                TimespanParameter currenttimespan = new() { Starttime = DateTime.Now.AddDays(-7), Endtime = DateTime.Now };
                foreach (var labPatient in labPatientList)
                {
                    if(labPatient.Starttime >= currenttimespan.Starttime && (labPatient.Endtime < currenttimespan.Endtime || labPatient.Endtime is null))
                    {
                        if (labPatient.Endtime is null)
                        {
                            labPatient.Endtime = DateTime.Today;
                        }
                        List<WardParameter> patStay = RestDataAccess.AQLQuery<WardParameter>(GetStays(labPatient));
                        if (patStay is not null)
                        {
                            List<WardParameter> distinctList = new();
                            foreach (var pat in patStay)
                            {
                                if (!distinctList.Contains(pat))
                                {
                                    distinctList.Add(pat);
                                }
                            }
                            List<WardParameter> sortedList = distinctList.OrderBy(p => p.Start).ToList();

                            List<LabPatientModel> patListStationary = RestDataAccess.AQLQuery<LabPatientModel>(GetStationaryPatientList(sortedList.First(), sortedList.Last()));
                            List<LabPatientModel> distinctListStationary = new();

                            if (patListStationary is not null && sortedList is not null)
                            {
                                foreach (var pat in patListStationary)
                                {
                                    if (!distinctListStationary.Contains(pat))
                                    {
                                        distinctListStationary.Add(pat);
                                    }
                                    if (pat.PatientID != labPatient.PatientID && distinctListStationary.Contains(pat))
                                    {
                                        foreach (var labPat in sortedList)
                                        {
                                            pat.Starttime = labPat.Start;
                                            pat.Endtime = labPat.End;
                                            if (pat.Endtime is null)
                                            {
                                                pat.Endtime = DateTime.Today;
                                            }
                                            List<WardParameter> contactStay = RestDataAccess.AQLQuery<WardParameter>(GetStays(pat));
                                            List<WardParameter> distinctContact = new();

                                            if (contactStay is not null)
                                            {
                                                foreach (var contact in contactStay)
                                                {
                                                    if (!distinctContact.Contains(contact) && contact.PatientID != labPatient.PatientID)
                                                    {
                                                        distinctContact.Add(contact);
                                                    }
                                                    if (contact.Ward == labPat.Ward && distinctContact.Contains(contact))
                                                    {
                                                        if (pat.Endtime == DateTime.Today)
                                                        {
                                                            currentnumberOfContacts++;
                                                        }
                                                        else
                                                        {
                                                            numberOfContacts++;
                                                        }
                                                        labPatient.CountContacts++;
                                                    }
                                                    else if (contact.DepartementID is not null
                                                        && contact.DepartementID == labPat.DepartementID
                                                        && distinctContact.Contains(contact)
                                                        && contact.Ward is null)
                                                    {
                                                        if (pat.Endtime == DateTime.Today)
                                                        {
                                                            currentnumberOfContacts++;
                                                        }
                                                        else
                                                        {
                                                            numberOfContacts++;
                                                        }
                                                        labPatient.CountContacts++;
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }

                averageNumberOfContacts = NUMNodeStatistics.GetAverage(numberOfContacts + currentnumberOfContacts, countPatient + currentcountPatient);
                (medianNumberOfContacts, underQuartilNumberOfContacts, upperQuartilNumberOfContacts) = NUMNodeStatistics.GetMedianAndInterquartil(labPatientList, countPatient + currentcountPatient, "contact");
                standardDeviationNumberOfContacts = NUMNodeStatistics.GetStandardDeviation(labPatientList, countPatient + currentcountPatient, averageNumberOfContacts, "contact");
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot get aggregated dataset for patient contacts :" + e);
                await Task.CompletedTask;
            }
            
        }

        private void SaveStaticData()
        {
            try
            {
                saveData = new NUMNodeSaveModel
                {
                    CountPatient = countPatient,
                    NumberOfMaybeNosCases = numberOfMaybeNosCases,
                    NumberOfNosCases = numberOfNosCases,
                    NumberOfContacts = numberOfContacts,
                    NumberOfStays = numberOfStays
                };

                JSONFileStream.JSONWriter.Write(saveData, path, "NUMNodeSave");
            }
            catch (Exception e)
            {
                _logger.LogWarning("Data cannot be saved :" + e);
            }           
        }

        private AQLQuery GetStays(LabPatientModel labpatient)
        {
            return new AQLQuery("GetStays", $@"SELECT s/data[at0001]/items[at0004]/value/value AS Start,
                                s/data[at0001]/items[at0005]/value/value AS End,
                                y/items[at0027]/value/value AS Ward,
                                l/items[at0024,'Fachabteilungsschlüssel']/value/defining_code/code_string AS DepartementID
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS (CLUSTER k[openEHR-EHR-CLUSTER.case_identification.v0] 
                                AND ADMIN_ENTRY s[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                                CONTAINS (CLUSTER y[openEHR-EHR-CLUSTER.location.v1] 
                                AND CLUSTER l[openEHR-EHR-CLUSTER.organization.v0])) 
                                WHERE c/name/value = 'Patientenaufenthalt'
                                AND k/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value = '{labpatient.CaseID}'
                                AND e/ehr_status/subject/external_ref/id/value = '{labpatient.PatientID}'
                                AND s/data[at0001]/items[at0004]/value/value <= '{labpatient.Endtime?.ToString("yyyy-MM-dd")}' 
                                AND (s/data[at0001]/items[at0005]/value/value >='{labpatient.Starttime:yyyy-MM-dd}'
                                OR NOT EXISTS s/data[at0001]/items[at0005]/value/value)");
        }

        private AQLQuery GetStaysCount(LabPatientModel labpatient)
        {
            return new AQLQuery("GetStaysCount", $@"SELECT COUNT(g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value) AS Count
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                        CONTAINS (CLUSTER g[openEHR-EHR-CLUSTER.case_identification.v0] and ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]) 
                        WHERE c/name/value='Patientenaufenthalt'
                        AND g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value = '{labpatient.CaseID}'
                        AND e/ehr_status/subject/external_ref/id/value = '{labpatient.PatientID}'
                        AND a/data[at0001]/items[at0004]/value/value <= '{labpatient.Endtime?.ToString("yyyy-MM-dd")}' 
                        AND (a/data[at0001]/items[at0005]/value/value >='{labpatient.Starttime:yyyy-MM-dd}' 
                        OR NOT EXISTS a/data[at0001]/items[at0005]/value/value)");
        }

        private AQLQuery LaborPositivData(TimespanParameter timespan, string pathogen)
        {
            return new AQLQuery("LaborPositivData", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND m/items[at0015]/value/value>='{timespan.Starttime:yyyy-MM-dd}' 
                                AND m/items[at0015]/value/value<'{timespan.Endtime:yyyy-MM-dd}'");
        }

        private AQLQuery LaborNegativData(TimespanParameter timespan, string pathogen, LabDataReceiveModel lab)
        {
            return new AQLQuery("LaborNegativData", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
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
                                AND m/items[at0015]/value/value>='{timespan.Starttime:yyyy-MM-dd}' 
                                AND m/items[at0015]/value/value<'{timespan.Endtime:yyyy-MM-dd}'
                                AND  i/items[at0001]/value/value = '{lab.FallID}'");
        }

        private AQLQuery GetStationaryPatientList(WardParameter stay_first, WardParameter stay_last)
        {
            return new AQLQuery("GetStationaryPatientList", $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value AS CaseID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1]
                        CONTAINS (ADMIN_ENTRY f[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY h[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
                        WHERE c/name/value='Stationärer Versorgungsfall'
                        AND f/data[at0001]/items[at0071]/value/value >= '{stay_first.Start:yyyy-MM-dd}' 
                        AND (h/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value <= '{stay_last.End:yyyy-MM-dd}'
                        OR NOT EXISTS h/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value)");
        }
    }
}
