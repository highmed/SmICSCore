using Microsoft.Extensions.Logging;
using SmICSCoreLib.DB;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.NUMNode
{
    public class NUMNodeFactory : INUMNodeFactory
    {
        private List<NUMNodeModel> NUMNodeList;
        private SortedDictionary<DateTime, Dictionary<string, NUMNodeModel>> dataAggregationStorage;

        private Dictionary<string, List<int>> AverageNumberOfStays;
        private Dictionary<string, List<int>> AverageNumberOfNosCases;
        private Dictionary<string, List<int>> AverageNumberOfContacts;

        private List<LabDataReceiveModel> receiveLabDataListnegativ;
        private List<NUMNodeCountModel> countStays;

        private static int countPatient;
        private static int numberOfStays;
        private static int numberOfNosCases;
        private static int numberOfContacts;
        private string pathogen = "94500-6";

        public IRestDataAccess RestDataAccess { get; set; }
        private ILogger<NUMNodeFactory> _logger;
        public NUMNodeFactory(IRestDataAccess restDataAccess, ILogger<NUMNodeFactory> logger)
        {
            RestDataAccess = restDataAccess;
            _logger = logger;
        }

        public List<NUMNodeModel> Process()
        {
            InitializeGlobalVariables();
            TimespanParameter timespan = new TimespanParameter { Starttime = DateTime.Now, Endtime = DateTime.Now };
            (countPatient, numberOfStays, numberOfNosCases, numberOfContacts) = getDataAggregation(timespan, pathogen);
            return null;
        }

        public void FirstDataEntry()
        {
            InitializeGlobalVariables();
            TimespanParameter timespan = new TimespanParameter { Starttime = DateTime.MinValue, Endtime = DateTime.Now };
            (countPatient, numberOfStays, numberOfNosCases, numberOfContacts) = getDataAggregation(timespan, pathogen);
        }

        private (int, int, int, int) getDataAggregation(TimespanParameter timespan, string pathogen)
        {
            List<LabDataReceiveModel> receiveLabDataListpositiv = RestDataAccess.AQLQuery<LabDataReceiveModel>(LaborPositivData(timespan, pathogen));
            if (receiveLabDataListpositiv is not null)
            {
                foreach (LabDataReceiveModel pat in receiveLabDataListpositiv.GroupBy(x => x.PatientID).Select(g => g.First()).ToList())
                {
                    countPatient++;
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
                }

                foreach (NUMNodeCountModel count in countStays)
                {
                    numberOfStays += count.Count;
                }
                return (countPatient, numberOfStays, 0, 0);
            }
            else
            {
                return (countPatient, numberOfStays, 0, 0);
            }

        }

        private void InitializeGlobalVariables()
        {
            dataAggregationStorage = new SortedDictionary<DateTime, Dictionary<string, NUMNodeModel>>();
            NUMNodeList = new List<NUMNodeModel>();
            AverageNumberOfStays = new Dictionary<string, List<int>>();
            AverageNumberOfNosCases = new Dictionary<string, List<int>>();
            AverageNumberOfContacts = new Dictionary<string, List<int>>();
            receiveLabDataListnegativ = new List<LabDataReceiveModel>();
            countStays = new List<NUMNodeCountModel>();
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
                        AND a/data[at0001]/items[at0004]/value >= '{labDataListpositiv.Befunddatum.ToString("yyyy-MM-dd")}' AND a/data[at0001]/items[at0005]/value <='{labDataListnegativ.Befunddatum.ToString("yyyy-MM-dd")}'"
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
