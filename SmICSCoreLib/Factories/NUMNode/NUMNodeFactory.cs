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

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeFactory : INUMNodeFactory
    {
        private NUMNodeModel NUMNodeList;
        private List<NUMNodeModel> dataAggregationStorage;
        private List<HospStay> caseList;
        private List<SymptomModel> patSymptomList;

        private int averageNumberOfStays;
        private int averageNumberOfNosCases;
        private int averageNumberOfMaybeNosCases;
        private int averageNumberOfContacts;

        private List<LabDataReceiveModel> receiveLabDataListnegativ;
        private List<NUMNodeCountModel> countStays;

        private static int countPatient;
        private static int numberOfStays;
        private static int numberOfNosCases;
        private static int numberOfMaybeNosCases;
        private static int numberOfContacts;
        private string pathogen = "94500-6";

        private (int, int, int, int) average;
        private readonly string path = @"../SmICSWebApp/Resources/NUMNode.csv";
        private List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
                "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
                "Pain in throat (finding)"});

        public IRestDataAccess RestDataAccess { get; set; }
        private ILogger<NUMNodeFactory> _logger;
        public NUMNodeFactory(IRestDataAccess restDataAccess, ILogger<NUMNodeFactory> logger)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
        }

        public void Process(TimespanParameter timespan)
        {
            
            (countPatient, numberOfStays, numberOfNosCases, numberOfMaybeNosCases, numberOfContacts) = getDataAggregation(timespan, pathogen);

            (averageNumberOfStays, averageNumberOfNosCases, averageNumberOfMaybeNosCases, averageNumberOfContacts) = GetAverage((numberOfStays, numberOfNosCases, numberOfMaybeNosCases, numberOfContacts), countPatient);

            NUMNodeList = new NUMNodeModel() { AverageNumberOfStays = averageNumberOfStays, AverageNumberOfNosCases = averageNumberOfNosCases,AverageNumberOfMaybeNosCases = numberOfMaybeNosCases, AverageNumberOfContacts = averageNumberOfContacts, DateTime = DateTime.Now };
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
            if (receiveLabDataListpositiv is not null)
            {
                foreach (LabDataReceiveModel pat in receiveLabDataListpositiv.GroupBy(x => x.PatientID).Select(g => g.First()).ToList())
                {
                    countPatient++;
                    numberOfStays = GetNumberOfStays(timespan, pathogen, pat);
                    (numberOfMaybeNosCases, numberOfNosCases) = GetNumberOfNosCases(pat);
                    numberOfContacts = GetNumberOfContacts(timespan, pathogen, pat);
                }
                return (countPatient, numberOfStays, numberOfNosCases, numberOfMaybeNosCases,  numberOfContacts);

            }
            else
            {
                return (countPatient, numberOfStays, numberOfNosCases, numberOfMaybeNosCases,  numberOfContacts);
            }

        }

        private int GetNumberOfStays(TimespanParameter timespan, string pathogen, LabDataReceiveModel pat)
        {
            receiveLabDataListnegativ = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborNegativData(timespan, pathogen, pat));
            if (receiveLabDataListnegativ is not null)
            {
                countStays = RestDataAccess.AQLQuery<NUMNodeCountModel>(getStaysCount(receiveLabDataListnegativ.First(), pat));
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

        private (int, int) GetNumberOfNosCases(LabDataReceiveModel pat)
        {

            caseList = RestDataAccess.AQLQuery<HospStay>(PatientAddmissionByNegativCase(pat));
            patSymptomList = RestDataAccess.AQLQuery<SymptomModel>(GetAllSymptomsByPatient(pat));
            int countNosCases = 0;
            int countMaybeNosCases = 0;

            if (patSymptomList is not null && caseList is not null)
            {
                bool symCase = false;
                foreach (var sym in patSymptomList)
                {
                    if (symptomList.Contains(sym.NameDesSymptoms))
                    {
                        symCase = true;
                    }

                }
                if (symCase == false)
                {
                    countMaybeNosCases++;
                    //to do: nos cases with contacts
                }
            }

            return (countMaybeNosCases, countNosCases);
        }

        private int GetNumberOfContacts(TimespanParameter timespan, string pathogen, LabDataReceiveModel pat)
        {
            return 0;
        }

        private void InitializeGlobalVariables()
        {
            dataAggregationStorage = new List<NUMNodeModel>();
            NUMNodeList = new NUMNodeModel();
            receiveLabDataListnegativ = new List<LabDataReceiveModel>();
            countStays = new List<NUMNodeCountModel>();
            caseList = new List<HospStay>();
            patSymptomList = new List<SymptomModel>();
        }

        public (int, int, int, int) GetAverage((int, int, int, int) parameter, int quanti)
        {
            
            if (parameter != (0, 0, 0, 0) & quanti != 0)
            {
                int average1 = parameter.Item1 / quanti;
                int average2 = parameter.Item2 / quanti;
                int average3 = parameter.Item3 / quanti;
                int average4 = parameter.Item4 / quanti;
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
                Query = $@"SELECT COUNT(g/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value) as Count
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

        private AQLQuery PatientAddmissionByNegativCase(LabDataReceiveModel lab)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "PatientAddmissionByNegativCase",
                Query = $@"SELECT i/data[at0001]/items[at0071]/value/value AS Beginn,
                                p/data[at0001]/items[at0011,'Datum/Uhrzeit der Entlassung']/value/value AS Ende,
                                c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value AS FallID
                                FROM EHR e
                                CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.fall.v1]
                                CONTAINS (ADMIN_ENTRY i[openEHR-EHR-ADMIN_ENTRY.admission.v0] and ADMIN_ENTRY p[openEHR-EHR-ADMIN_ENTRY.discharge_summary.v0]) 
                                WHERE c/name/value='Stationärer Versorgungsfall'
                                AND c/context/other_context[at0001]/items[at0003,'Fall-Kennung']/value/value = '{ lab.FallID }'
                                AND i/data[at0001]/items[at0071]/value/value>='{ lab.Befunddatum.AddDays(-4).ToString("yyyy-MM-dd") }'"
            };
            return aql;
        }
        //need to test
        private AQLQuery GetAllSymptomsByPatient(LabDataReceiveModel lab)
        {
            AQLQuery aql = new AQLQuery()
            {
                Name = "GetAllSymptomsByPatient",
                Query = $@"SELECT o/data[at0190]/events[at0191]/data[at0192]/items[at0001]/value/value AS NameDesSymptoms,
                                o/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value AS Beginn,
                                e/ehr_status/subject/external_ref/id/value AS PatientenID
                                FROM EHR e
                                CONTAINS COMPOSITION c
                                CONTAINS OBSERVATION o[openEHR-EHR-OBSERVATION.symptom_sign.v0]  
                                WHERE c/archetype_details/template_id='Symptom'
                                AND e/ehr_status/subject/external_ref/id/value = '{lab.PatientID}'
                                AND o/data[at0190]/events[at0191]/data[at0192]/items[at0152]/value/value>='{ lab.Befunddatum.AddDays(-4).ToString("yyyy-MM-dd") }'"
            };
            return aql;
        }
    }
}
