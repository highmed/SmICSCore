using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.Factories.ContactNetwork;
using SmICSCoreLib.Factories.ContactNetwork.ReceiveModels;
using SmICSCoreLib.Factories.InfectionSituation;
using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeFactory : INUMNodeFactory
    {
        private NUMNodeModel NUMNodeList;
        private List<NUMNodeModel> dataAggregationStorage;
        private ContactModel contacts;

        private double averageNumberOfStays;
        private double averageNumberOfNosCases;
        private double averageNumberOfMaybeNosCases;
        private double averageNumberOfContacts;

        private List<LabDataReceiveModel> receiveLabDataListnegativ;
        private List<NUMNodeCountModel> countStays;

        private static int countPatient;
        private static int numberOfStays;
        private static int numberOfNosCases;
        private static int numberOfMaybeNosCases;
        private static int numberOfContacts;
        private string pathogen = "94500-6";

        private (double, double, double, double) average;
        private readonly string path = @"../SmICSWebApp/Resources/NUMNode.csv";
        public List<string> patientList;

        public IRestDataAccess RestDataAccess { get; set; }
        private ILogger<NUMNodeFactory> _logger;
        private readonly IInfectionSituationFactory _infecFac;
        private readonly IContactNetworkFactory _contactFac;
        public NUMNodeFactory(IRestDataAccess restDataAccess, ILogger<NUMNodeFactory> logger, IInfectionSituationFactory infecFac, IContactNetworkFactory contactFac)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
            _infecFac = infecFac;
            _contactFac = contactFac;
        }

        public void Process(TimespanParameter timespan)
        {
            
            (countPatient, numberOfStays, numberOfNosCases, numberOfMaybeNosCases, numberOfContacts) = getDataAggregation(timespan, pathogen);
            (averageNumberOfStays, averageNumberOfNosCases, averageNumberOfMaybeNosCases, averageNumberOfContacts) = GetAverage((numberOfStays, numberOfNosCases, numberOfMaybeNosCases, numberOfContacts), countPatient);

            NUMNodeList = new NUMNodeModel() { AverageNumberOfStays = averageNumberOfStays, AverageNumberOfNosCases = averageNumberOfNosCases, AverageNumberOfMaybeNosCases = averageNumberOfMaybeNosCases, AverageNumberOfContacts = averageNumberOfContacts, DateTime = DateTime.Now };
            dataAggregationStorage.Add(NUMNodeList);

            SaveCSV.SaveToCsv(dataAggregationStorage, path);
        }

        public void RegularDataEntry()
        {
            InitializeGlobalVariables();
            TimespanParameter timespan = new TimespanParameter { Starttime = DateTime.Now, Endtime = DateTime.Now };
            Process(timespan);
        }

        public void FirstDataEntry()
        {
            InitializeGlobalVariables();
            TimespanParameter timespan = new TimespanParameter { Starttime = DateTime.MinValue, Endtime = DateTime.Now };
            Process(timespan);
        }

        private (int, int, int, int, int) getDataAggregation(TimespanParameter timespan, string pathogen)
        {
            List<LabDataReceiveModel> receiveLabDataListpositiv = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborPositivData(timespan, pathogen));

            patientList = new List<string>();
            
            if (receiveLabDataListpositiv is not null)
            {
                foreach (var pat in receiveLabDataListpositiv)
                {
                    countPatient++;
                    patientList.Add(pat.PatientID);
                    numberOfStays = GetNumberOfStays(timespan, pathogen, pat, receiveLabDataListpositiv);
                    numberOfContacts = GetNumberOfContacts(timespan, pathogen, pat);
                }
                PatientListParameter patList = new PatientListParameter { patientList = patientList };
                List<PatientModel> list = _infecFac.Process(patList);
                numberOfMaybeNosCases = list.Where(x => x.Infektion == "Moegliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count();
                numberOfNosCases = list.Where(x => x.Infektion == "Wahrscheinliche Nosokomiale Infektion").Select(x => x.PatientID).Distinct().Count();
                return (countPatient, numberOfStays, numberOfNosCases, numberOfMaybeNosCases,  numberOfContacts);

            }
            else
            {
                return (countPatient, numberOfStays, numberOfNosCases, numberOfMaybeNosCases,  numberOfContacts);
            }

        }

        private int GetNumberOfStays(TimespanParameter timespan, string pathogen, LabDataReceiveModel pat, List<LabDataReceiveModel> receiveLabDataListpositiv)
        {
            receiveLabDataListnegativ = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborNegativData(timespan, pathogen, pat));
            if (receiveLabDataListnegativ is not null)
            {
                countStays = RestDataAccess.AQLQuery<NUMNodeCountModel>(getStaysCount(receiveLabDataListnegativ.OrderBy(x => x.Befunddatum).First(), pat));
 
            }
            else
            {
                LabDataReceiveModel labDataReceiveModel = new LabDataReceiveModel { Befunddatum = DateTime.Now };
                countStays = RestDataAccess.AQLQuery<NUMNodeCountModel>(getStaysCount(labDataReceiveModel, pat));
            }

            foreach (NUMNodeCountModel count in countStays)
            {
                numberOfStays = numberOfStays + count.Count;
            }
            return numberOfStays;
        }

        private int GetNumberOfContacts(TimespanParameter timespan, string pathogen, LabDataReceiveModel pat)
        {
            ContactParameter contact = new ContactParameter { PatientID = pat.PatientID, Starttime = timespan.Starttime, Endtime = timespan.Endtime };
            contacts = _contactFac.Process(contact);
            int numberofCon = contacts.PatientMovements.Select(x => x.PatientID).Distinct().Count();
            numberOfContacts = numberOfContacts + numberofCon;
            return numberOfContacts;
        }

        private void InitializeGlobalVariables()
        {
            dataAggregationStorage = new List<NUMNodeModel>();
            NUMNodeList = new NUMNodeModel();
            receiveLabDataListnegativ = new List<LabDataReceiveModel>();
            countStays = new List<NUMNodeCountModel>();
            contacts = new ContactModel();
        }

        public (double, double, double, double) GetAverage((int, int, int, int) parameter, int quanti)
        {
            
            if (quanti != 0)
            {
                double average1 = (double)parameter.Item1 / (double)quanti;
                double average2 = (double)parameter.Item2 / (double)quanti;
                double average3 = (double)parameter.Item3 / (double)quanti;
                double average4 = (double)parameter.Item4 / (double)quanti;
                average = (average1, average2, average3, average4);
                return average;
            }
            else
            {
                return average = (0, 0, 0, 0);
            }

        }

        private AQLQuery getStaysCount(LabDataReceiveModel labDataListnegativ, LabDataReceiveModel labDataListpositiv)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "LaborNegativData",
                Query = $@"SELECT COUNT(g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value) AS Count
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                        CONTAINS (CLUSTER g[openEHR-EHR-CLUSTER.case_identification.v0] and ADMIN_ENTRY a[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0]) 
                        WHERE c/name/value='Patientenaufenthalt'
                        AND g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value = '{labDataListpositiv.FallID}'
                        AND a/data[at0001]/items[at0004]/value/value <= '{labDataListnegativ.Befunddatum.ToString("yyyy-MM-dd")}' 
                        AND (a/data[at0001]/items[at0005]/value/value >='{labDataListpositiv.Befunddatum.ToString("yyyy-MM-dd")}' 
                        OR NOT EXISTS a/data[at0001]/items[at0005]/value/value)"
            };
            return aql;
        }

        private AQLQuery LaborPositivData(TimespanParameter timespan, string pathogen)
        {
            AQLQuery aql = new AQLQuery()
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
                                AND d/items[at0024]/value/defining_code/code_string = '{ pathogen }'
                                AND m/items[at0015]/value/value>='{ timespan.Starttime.ToString("yyyy-MM-dd") }' 
                                AND m/items[at0015]/value/value<'{ timespan.Endtime.ToString("yyyy-MM-dd") }'"
            };
            return aql;
        }

        private AQLQuery LaborNegativData(TimespanParameter timespan, string pathogen, LabDataReceiveModel lab)
        {
            AQLQuery aql = new AQLQuery()
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
                                AND d/items[at0024]/value/defining_code/code_string = '{ pathogen }'
                                AND m/items[at0015]/value/value>='{ timespan.Starttime.ToString("yyyy-MM-dd") }' 
                                AND m/items[at0015]/value/value<'{ timespan.Endtime.ToString("yyyy-MM-dd") }'
                                AND  i/items[at0001]/value/value = '{ lab.FallID }'"
            };
            return aql;
        }

    }
}
