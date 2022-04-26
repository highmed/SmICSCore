using System;
using System.Collections.Generic;
using SmICSCoreLib.Util;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.EpiCurve.ReceiveModel;
using SmICSCoreLib.REST;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.DB.MenuItems;
using SmICSCoreLib.DB.Models;
using System.Linq;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public class EpiCurveFactory : IEpiCurveFactory
    {
        private readonly string COMPLETE_CLINIC = "klinik";
        private readonly int WEEK = 7;
        private readonly int MONTH = 28;

        private List<EpiCurveModel> epiCurveList;
        private SortedDictionary<DateTime, Dictionary<string, EpiCurveModel>> dataAggregationStorage;
        private Dictionary<string, PatientInfectionModel> infections;
        private Dictionary<string, EpiCurveModel> EpiCurveEntryByWard;

        private Dictionary<string, List<int>> mavg7;
        private Dictionary<string, List<int>> mavg28;
        
        public IRestDataAccess RestDataAccess { get; }
        private ILogger<EpiCurveFactory> _logger;
        private IMenuItemDataAccess _menuDataAccess;
        public EpiCurveFactory(IMenuItemDataAccess menuDataAccess, IRestDataAccess restData, ILogger<EpiCurveFactory> logger)
        {
            RestDataAccess = restData;
            _logger = logger;
            _menuDataAccess = menuDataAccess;
        }
        public List<EpiCurveModel> Process(EpiCurveParameter parameter)
        {
            InitializeGlobalVariables();
            ExtendedEpiCurveParameter extendedParams = new ExtendedEpiCurveParameter()
            {
                Pathogen = parameter.Pathogen,
                Starttime = parameter.Starttime,
                Endtime = parameter.Endtime,
            };
            if (extendedParams.Pathogen == "sars-cov-2")
            {
                List<Pathogen> pathos = _menuDataAccess.GetPathogens().Result;
                List<string> codes = new List<string>();
                foreach(Pathogen p in pathos)
                {
                    if(p.Name.ToLower().Contains("sars-cov-2"))
                    {
                        codes.Add(p.Code);
                    }
                }
                extendedParams.PathogenCodes = codes;
            }
            else
            {
                List<Pathogen> pathogens = _menuDataAccess.GetPathogendByName(parameter.Pathogen).Result;
                extendedParams.PathogenCodes = pathogens.Select(p => p.Code).ToList();
            }
            for (DateTime date = parameter.Starttime.Date; date <= parameter.Endtime.Date; date = date.AddDays(1.0))
            {
                CreateDailyEntries(date, extendedParams);
                CreateEmptyWardEntries(date);
            }

            AddMissingValues(extendedParams);
            DataAggregationStorageToList();
            
            return epiCurveList;
        }

        private void InitializeGlobalVariables()
        {
            dataAggregationStorage = new SortedDictionary<DateTime, Dictionary<string, EpiCurveModel>>();
            epiCurveList = new List<EpiCurveModel>();
            EpiCurveEntryByWard = new Dictionary<string, EpiCurveModel>();
            infections = new Dictionary<string, PatientInfectionModel>();
            mavg28 = new Dictionary<string, List<int>>();
            mavg7 = new Dictionary<string, List<int>>();
            mavg7.Add(COMPLETE_CLINIC, new List<int>());
            mavg28.Add(COMPLETE_CLINIC, new List<int>());
        }
        private void CreateDailyEntries(DateTime date, ExtendedEpiCurveParameter parameter)
        {
            _logger.LogDebug("Flag - Query Paramters: Datum: {Date} \r PathogenList: {pathogens}", date.ToString(), parameter.PathogenCodesToAqlMatchString());
            List<FlagTimeModel> flagTimes = RestDataAccess.AQLQuery<FlagTimeModel>(LaborEpiCurve(date, parameter));

            if (flagTimes == null)
            {
                AddToEpiCurveToSortedDict(date);
                return;
            }

            PopulateDailyEpicCurve(flagTimes, date);

            AddToEpiCurveToSortedDict(date);

            
        }
        private void AddToEpiCurveToSortedDict(DateTime date)
        {
            dataAggregationStorage.Add(date, EpiCurveEntryByWard);
        }
        private void PopulateDailyEpicCurve(List<FlagTimeModel> flagTimes, DateTime date)
        {
            foreach (FlagTimeModel flag in flagTimes)
            {
                _logger.LogDebug("PatientLocation - Query Paramters: PatientID: {PatientID} \r Datum: {Date}", flag.PatientID, flag.Datum.ToString());

                List<PatientLocation> patientLocations = RestDataAccess.AQLQuery<PatientLocation>(AQLCatalog.PatientLocation(flag.Datum, flag.PatientID));

                PatientLocation patientLocation = null;
                if (patientLocations == null)
                {
                    _logger.LogDebug("PatientLocation - Query Response Count: {LocationCount}", null);
                    patientLocation = new PatientLocation() { Ward = "ohne Stationsangabe", Departement = "0000" };
                }
                else
                {
                    patientLocation = patientLocations[0];
                }
                if(patientLocation.Ward == null)
                {
                    patientLocation.Ward = "Fachabteilung: " + patientLocation.Departement;
                }
                SetBasicDailyEpiCurveEntries(flag, patientLocation, date);
                AggregateFlagInformation(flag, patientLocation);

            }
            GetMovingAverages();
        }
        private void SetBasicDailyEpiCurveEntries(FlagTimeModel flag, PatientLocation patientLocation, DateTime date)
        { 
            if (!EpiCurveEntryByWard.ContainsKey(COMPLETE_CLINIC))
            {
                EpiCurveEntryByWard.Add(COMPLETE_CLINIC, InitializeNewEpiCurveModel(flag, COMPLETE_CLINIC, date));
            }
            if (!EpiCurveEntryByWard.ContainsKey(patientLocation.Ward) && flag.HasFlag())
            {
                EpiCurveEntryByWard.Add(patientLocation.Ward, InitializeNewEpiCurveModel(flag, patientLocation.Ward, date));
            }
            if(!mavg28.ContainsKey(patientLocation.Ward) && !mavg7.ContainsKey(patientLocation.Ward))
            {
                mavg7.Add(patientLocation.Ward, new List<int>());
                mavg28.Add(patientLocation.Ward, new List<int>());
            }
        }
        private void AggregateFlagInformation(FlagTimeModel flag, PatientLocation patientLocation)
        {
            if (infections.ContainsKey(flag.PatientID))
            {
                PatientInfectionModel patientInfections = infections[flag.PatientID];

                if (patientInfections.IsInfected && !patientInfections.HasFirstNegativeTest && !flag.HasFlag())
                {
                    patientInfections.HasFirstNegativeTest = true;
                }
                else if (patientInfections.IsInfected && patientInfections.HasFirstNegativeTest && !flag.HasFlag())
                {
                    DecrementOverallCount(patientInfections.InfectionWard);
                    DecrementOverallCount(COMPLETE_CLINIC);
                }
                else if(!patientInfections.IsInfected && flag.HasFlag())
                {
                    IncrementCounts(patientLocation.Ward);
                    IncrementCounts(COMPLETE_CLINIC);
                }
            }
            else
            {
                InitializeNewInfectiousPatient(flag, patientLocation);
                if (flag.HasFlag())
                {
                    IncrementCounts(patientLocation.Ward);
                    IncrementCounts(COMPLETE_CLINIC);

                }
            }
        }
        private void GetMovingAverages()
        {
            foreach(KeyValuePair<string, EpiCurveModel> keyValuePair in EpiCurveEntryByWard)
            {
                keyValuePair.Value.MAVG7 = MovingAverage.Calculate(mavg7[keyValuePair.Key], keyValuePair.Value.Anzahl, WEEK);
                keyValuePair.Value.MAVG28 = MovingAverage.Calculate(mavg28[keyValuePair.Key], keyValuePair.Value.Anzahl, MONTH);
            }
        }
        private void DecrementOverallCount(string ward)
        {
            EpiCurveEntryByWard[ward].anzahl_gesamt -= 1;
        }
        private void IncrementCounts(string ward)
        {
            EpiCurveEntryByWard[ward].Anzahl += 1;
            EpiCurveEntryByWard[ward].anzahl_gesamt += 1;
        }
        private void DataAggregationStorageToList()
        {
            foreach(KeyValuePair<DateTime, Dictionary<string, EpiCurveModel>> keyValuePair in dataAggregationStorage)
            {
                epiCurveList.AddRange(dataAggregationStorage[keyValuePair.Key].Values);
            }
        }
        private void AddMissingValues(TimespanParameter parameter)
        {
            ICollection<string> wards = dataAggregationStorage[parameter.Endtime.Date].Keys;
            DateTime date = parameter.Starttime.Date;
            while (dataAggregationStorage[date].Keys.Count != wards.Count)
            {
                foreach (string ward in wards)
                {
                    if (!dataAggregationStorage[date].ContainsKey(ward))
                    {
                        dataAggregationStorage[date].Add(ward, InitializeEmptyEpiCurveModel(dataAggregationStorage[parameter.Endtime.Date][ward], date, Purpose.FOR_PAST));
                    }
                }
                
                date = date.AddDays(1);
            }
        }
        private void CreateEmptyWardEntries(DateTime date)
        {
            Dictionary<string, EpiCurveModel> nextDayEntries = new Dictionary<string, EpiCurveModel>();
            
            foreach(KeyValuePair<string, EpiCurveModel> keyValuePair in dataAggregationStorage[date])
            {
                nextDayEntries.Add(keyValuePair.Key, InitializeEmptyEpiCurveModel(keyValuePair.Value, date.AddDays(1), Purpose.FOR_FUTURE));
            }

            EpiCurveEntryByWard = nextDayEntries;
        }
        private void InitializeDailyEpiCurve(FlagTimeModel firstFlag, DateTime date)
        {
            EpiCurveEntryByWard = new Dictionary<string, EpiCurveModel>();
            EpiCurveEntryByWard.Add(COMPLETE_CLINIC, InitializeNewEpiCurveModel(firstFlag, COMPLETE_CLINIC, date));
        }
        private void InitializeNewInfectiousPatient(FlagTimeModel flag, PatientLocation loc)
        {
            infections.Add(flag.PatientID, new PatientInfectionModel
            {
                PatientID = flag.PatientID, IsInfected = flag.HasFlag(), InfectionWard = loc.Ward
            });

        }
        private EpiCurveModel InitializeNewEpiCurveModel(FlagTimeModel flag, string ward, DateTime date)
        {
            return new EpiCurveModel()
            {
                ErregerID = flag.PathogenCode,
                ErregerBEZL = flag.Pathogen,
                Anzahl = 0,
                anzahl_gesamt = 0,
                Datum = date,
                StationID = ward
            };
        }
        private EpiCurveModel InitializeEmptyEpiCurveModel(EpiCurveModel oldModel, DateTime date, Purpose purpose)
        {
            return new EpiCurveModel()
            {
                Anzahl = 0,
                anzahl_gesamt = purpose == Purpose.FOR_FUTURE ? oldModel.anzahl_gesamt : 0,
                Anzahl_cs = 0,
                anzahl_gesamt_av28 = 0,
                anzahl_gesamt_av7 = 0,
                MAVG28 = 0,
                MAVG28_cs = 0,
                MAVG7 = 0,
                MAVG7_cs = 0,
                Datum = date,
                ErregerBEZK = oldModel.ErregerBEZK,
                ErregerBEZL = oldModel.ErregerBEZL,
                ErregerID = oldModel.ErregerID,
                StationID = oldModel.StationID
            };
        }

        private AQLQuery LaborEpiCurve(DateTime date, ExtendedEpiCurveParameter parameter)
        {
            if (parameter.MedicalField == MedicalField.VIROLOGY)
            {
                return new AQLQuery()
                {
                    Name = "LaborEpiCurve",
                    Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001]/value/value as FallID,
                        d/items[at0001]/value/defining_code/code_string as Flag,
                        d/items[at0024]/value/defining_code/code_string as PathogenCode,
                        d/items[at0024]/value/value as Pathogen,
                        m/items[at0015]/value/value as Datum
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                        CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                        CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
                        AND CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                        CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                        WHERE c/name/value='Virologischer Befund' 
                        AND d/items[at0001]/name/value='Nachweis' 
                        AND d/items[at0024]/value/defining_code/code_string MATCHES { parameter.PathogenCodesToAqlMatchString() }
                        AND m/items[at0015]/value/value>='{ date.ToString("yyyy-MM-dd") }' 
                        AND m/items[at0015]/value/value<'{ date.AddDays(1).ToString("yyyy-MM-dd") }'"
                };
            }
            else
            {
                return new AQLQuery()
                {
                    Name = "",
                    Query = @$"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                    n/items[at0001]/value/value as FallID,
                    i/items[at0024]/value/value as Flag,
                    i/items[at0001]/value/value as Pathogen,
                    i/items[at0001]/value/defining_code/code_string as PathogenCode,
                    u/items[at0015]/value/value as Datum
                    FROM EHR e
                    CONTAINS COMPOSITION c
                    CONTAINS (CLUSTER n[openEHR-EHR-CLUSTER.case_identification.v0] and OBSERVATION x[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                    CONTAINS (CLUSTER u[openEHR-EHR-CLUSTER.specimen.v1] and CLUSTER v[openEHR-EHR-CLUSTER.laboratory_test_panel.v0]
                    CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                    WHERE c/archetype_details/template_id/value='Mikrobiologischer Befund' 
                    and i/items[at0001]/value/defining_code/code_string MATCHES {parameter.PathogenCodesToAqlMatchString()}
                    and u/items[at0015]/value/value >= '{ date.ToString("yyyy-MM-dd") }'
                    and u/items[at0015]/value/value < '{ date.AddDays(1).ToString("yyyy-MM-dd") }'"
                };
            }
        }


        private AQLQuery ViroLaborEpiCurve(DateTime date, ExtendedEpiCurveParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "LaborEpiCurve",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001]/value/value as FallID,
                        d/items[at0001]/value/defining_code/code_string as Flag,
                        d/items[at0024]/value/defining_code/code_string as VirusCode,
                        d/items[at0024]/value/value as Virus,
                        m/items[at0015]/value/value as Datum
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                        CONTAINS (CLUSTER i[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND OBSERVATION v[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                        CONTAINS (CLUSTER m[openEHR-EHR-CLUSTER.specimen.v1] 
                        AND CLUSTER s[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                        CONTAINS (CLUSTER d[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1])))
                        WHERE c/name/value='Virologischer Befund' 
                        AND d/items[at0001]/name/value='Nachweis' 
                        AND d/items[at0024]/value/defining_code/code_string MATCHES { parameter.PathogenCodesToAqlMatchString() }
                        AND m/items[at0015]/value/value>='{ date.ToString("yyyy-MM-dd") }' 
                        AND m/items[at0015]/value/value<'{ date.AddDays(1).ToString("yyyy-MM-dd") }'"
            };
        }
    }
    internal enum Purpose
    {
        FOR_FUTURE,
        FOR_PAST
    }
}
