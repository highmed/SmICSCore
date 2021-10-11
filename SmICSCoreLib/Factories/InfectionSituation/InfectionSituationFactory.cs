using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientStay.Count;
using SmICSCoreLib.Factories.PatientStay.Stationary;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.Factories.Vaccination;
using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.Factories.InfectionsSituation.ReceiveModel;
using SmICSCoreLib.REST;
using System.Linq;

namespace SmICSCoreLib.Factories.InfectionSituation
{
    public class InfectionSituationFactory : IInfectionSituationFactory
    {
        private readonly string positiv = "260373001";
        private Dictionary<string, SortedDictionary<DateTime, int>> dataAggregationStorage;

        private readonly ICountFactory _countFactory;
        private readonly IStationaryFactory _stationaryFactory;
        private readonly ISymptomFactory _symptomFactory;
        private readonly IPatientMovementFactory _patMoveFac;
        private readonly IVaccinationFactory _vaccFac;
        private readonly ILogger<InfectionSituationFactory> _logger;
        private readonly IRestDataAccess _restData;
        private readonly ILogger<PatientMovementFactory> _loggerPatMov;

        public InfectionSituationFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public InfectionSituationFactory(ICountFactory countFactory, IStationaryFactory stationaryFactory,
                                         ISymptomFactory symptomFactory, IPatientMovementFactory patMoveFac,
                                         IVaccinationFactory vaccFac, ILogger<InfectionSituationFactory> logger, IRestDataAccess restData, ILogger<PatientMovementFactory> loggerPatMov)
        {
            _countFactory = countFactory;
            _symptomFactory = symptomFactory;            
            _stationaryFactory = stationaryFactory;
            _patMoveFac = patMoveFac;
            _vaccFac = vaccFac;
            _logger = logger;
            _restData = restData;
            _loggerPatMov = loggerPatMov;
        }

        public List<Patient> Process(PatientListParameter parameter)
        {
            List<Patient> patienten = new();
            //Liste positive Patienten
            List<CountDataModel> positivPatList = _countFactory.ProcessFromID(positiv, parameter);
            List<CountDataModel> sortPattList = new();
            if (positivPatList != null || positivPatList.Count != 0)
            {
                foreach (var patient in positivPatList)
                {
                    if (!sortPattList.Contains(patient))
                    {
                        sortPattList.Add(patient);
                    }
                }
            }

            List<CountDataModel> allPositivPatList = _countFactory.Process(positiv);

            //Liste moegliche Nosokomiale Infektionen
            List<Patient> posPatientenListe = PossibleNosocomialsList(sortPattList);
            if (posPatientenListe != null && posPatientenListe.Count != 0)
            {
                //Liste wahrscheinliche Nosokomiale Infektionen
                List<Patient> proPatientenListe = ProbableNosocomialsList(posPatientenListe, allPositivPatList);
                foreach (var patientID in parameter.patientList)
                {
                    if (posPatientenListe.Contains(new Patient { PatientID = patientID }))
                    {
                        Patient patient = posPatientenListe.Find(x => x.PatientID == patientID);
                        PatientListParameter patListParameter = new();
                        List<string> patientList = new();
                        patientList.Add(patientID);
                        patListParameter.patientList = patientList;
                        List<VaccinationModel> patVaccination;
                        List<VaccinationModel> patientVaccination = _vaccFac.ProcessSpecificVaccination(patListParameter, "Infectious disease (disorder)");
                        if (patientVaccination != null && patientVaccination.Count != 0)
                        {
                            patVaccination = patientVaccination;
                            if (proPatientenListe.Contains(patient))
                            {
                                patienten.Add(new Patient(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                         "Wahrscheinliche Nosokomiale Infektion", patVaccination));
                            }
                            else
                            {
                                patienten.Add(new Patient(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                          "Moegliche Nosokomiale Infektion", patVaccination));
                            }
                        }
                        else
                        {
                            if (proPatientenListe.Contains(patient))
                            {
                                patienten.Add(new Patient(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                          "Wahrscheinliche Nosokomiale Infektion", null));
                            }
                            else
                            {
                                patienten.Add(new Patient(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                          "Moegliche Nosokomiale Infektion", null));
                            }
                        }
                    }
                }
            }
            return patienten;
        }

        public List<Patient> PossibleNosocomialsList(List<CountDataModel> positivPatList)
        {
            List<Patient> patNoskumalList = new();
            List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
            "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
            "Pain in throat (finding)"});

            foreach (CountDataModel positivPat in positivPatList)
            {
                //Check, ob der Patient Stationaer Behandlung hat
                List<StationaryDataModel> statPatList = _stationaryFactory.Process(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);
                if (statPatList != null || statPatList.Count != 0)
                {
                    foreach (StationaryDataModel statPatient in statPatList)
                    {
                        //Check, ob der Patient am Aufnahmedatum Symptome hat
                        List<SymptomModel> symptoms = _symptomFactory.SymptomByPatient(statPatient.PatientID, statPatient.Datum_Uhrzeit_der_Aufnahme);
                        if (symptoms is null || symptoms.Count == 0)
                        {
                            patNoskumalList.Add(new Patient(positivPat.PatientID,
                                                            positivPat.Zeitpunkt_des_Probeneingangs,
                                                            statPatient.Datum_Uhrzeit_der_Aufnahme,
                                                            statPatient.Datum_Uhrzeit_der_Entlassung));
                        }
                        else
                        {
                            foreach (var symptom in symptoms)
                            {
                                if (!symptomList.Contains(symptom.NameDesSymptoms) && 
                                    !patNoskumalList.Contains(new Patient { PatientID = positivPat.PatientID }))
                                {
                                    patNoskumalList.Add(new Patient(positivPat.PatientID,
                                                                  positivPat.Zeitpunkt_des_Probeneingangs,
                                                                  statPatient.Datum_Uhrzeit_der_Aufnahme,
                                                                  statPatient.Datum_Uhrzeit_der_Entlassung));
                                }
                            }
                        }
                    }
                }
            }

            return patNoskumalList;
        }

        public List<Patient> ProbableNosocomialsList(List<Patient> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            List<Patient> patNoskumalList = new();
            PatientListParameter patListParameter = new();

            foreach (var patient in allNoskumalPat)
            {
                List<string> patientList = new();
                patientList.Add(patient.PatientID);
                patListParameter.patientList = patientList;

                List<PatientMovementModel> nosPatBewegungen = _patMoveFac.Process(patListParameter);
                if (nosPatBewegungen.Count != 0 )
                {
                    foreach (var bewegung in nosPatBewegungen)
                    {
                        if (bewegung.Beginn < bewegung.Ende.AddMinutes(-15))
                        {
                            List<PatientMovementModel> covdPatBewegungen = FindContact(allPositivPat, bewegung.PatientID, bewegung.Fachabteilung, bewegung.Beginn, bewegung.Ende);
                            if (covdPatBewegungen.Count != 0 && !patNoskumalList.Contains(patient))
                            {
                                patNoskumalList.Add(patient);
                            }
                        }
                    }
                }
            }
            return patNoskumalList;
        }

        public List<PatientMovementModel> FindContact(List<CountDataModel> allPositivPat, string patientID, string station, DateTime beginn, DateTime ende)
        {
            List<PatientMovementModel> patientMovement = new();
            PatientListParameter patListParameter = new();
            List<string> patientList = new();

            foreach (var positivPat in allPositivPat)
            {
                patientList.Add(positivPat.PatientID);
            }
            patListParameter.patientList = patientList;

            List<PatientMovementModel> patBewegungen = _patMoveFac.ProcessFromStation(patListParameter, station, beginn, ende);
            if (patBewegungen.Count != 0)
            {
                foreach (var patBewegung in patBewegungen)
                {
                    if (patBewegung.PatientID != patientID &&
                        patBewegung.Beginn < patBewegung.Ende.AddMinutes(-15))
                    {
                        patientMovement.Add(patBewegung);
                    }
                }
            }
            return patientMovement;
        }

        public Dictionary<string, SortedDictionary<DateTime, int>> Process(TimespanParameter parameter, string kindOfFinding)
        {
            List<TimeDataPointModel> specimenIdentifierList = new List<TimeDataPointModel>();
            Dictionary<string, SortedDictionary<DateTime, int>> sortedStations = new();
            bool useAQLs = true;

            //BEGIN Different treatment for 'virological' and 'microbiological' finding
            //      = Change the test result description
            if (kindOfFinding == "virological")
            {
                specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatus(parameter));

                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    try
                    {
                        // Case for ''Not detected'' or ''Inconclusive'', i.e. ''negativ''
                        if (curTimeDataPoint.CodeForTestResult == 260415000
                            || curTimeDataPoint.CodeForTestResult == 419984006)
                        {
                            curTimeDataPoint.VirologicalTestResult = "negativ";
                        }
                        // Case for curTimeDataPoint.VirologicalTestResult different from positiv/negativ.
                        // Case for ''Not detected'' already treated above.
                        else if (curTimeDataPoint.CodeForTestResult != 260415000
                                 && curTimeDataPoint.CodeForTestResult != 419984006
                                 && curTimeDataPoint.CodeForTestResult != 260373001)
                        {
                            System.Diagnostics.Debug.WriteLine("Warning in InfectionsStatusDevelopmentCurveFactory");
                            System.Diagnostics.Debug.WriteLine(curTimeDataPoint.CodeForTestResult);
                        }
                        // Case for ''Detected'', i.e. ''positiv''
                        else if (curTimeDataPoint.CodeForTestResult == 260373001)
                        {
                            curTimeDataPoint.VirologicalTestResult = "positiv";
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(curTimeDataPoint.VirologicalTestResult);
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }

                // Faelle nach Stationen sortieren
                Dictionary<string, SortedDictionary<DateTime, string>> patientenAufenthalte = new();
                List<string> patientenIDs = new List<string>();

                // Liste mit PatientIDs erstellen. Gleichzeitig die PatientIDs zum Dictionary hinzufuegen
                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    if (!patientenIDs.Exists(t => t == curTimeDataPoint.PatientID))
                    {
                        patientenIDs.Add(curTimeDataPoint.PatientID);
                    }
                }

                PatientMovementFactory pmf_00001 = new PatientMovementFactory(_restData, _loggerPatMov);

                foreach (string curPatientID in patientenIDs)
                {
                    SortedDictionary<DateTime, string> curValueOfDictionary = new();
                    List<TimeDataPointModel> curPatientIDFound = specimenIdentifierList.FindAll(t => t.PatientID == curPatientID);
                    List<PatientLocation> patientLocations = new();

                    List<PatientMovementModel> movementsCurPatient
                    = pmf_00001.Process(new PatientListParameter { patientList = new() { curPatientID } });

                    foreach (TimeDataPointModel curTimeDataPoint in curPatientIDFound)
                    {
                        foreach (PatientMovementModel curMovementModel in movementsCurPatient)
                        {
                            if (curTimeDataPoint.VirologicalTestResult == "positiv"
                                && curTimeDataPoint.Zeitpunkt >= curMovementModel.Beginn
                                && curTimeDataPoint.Zeitpunkt < curMovementModel.Ende
                                && curMovementModel.BewegungstypID > 1)
                            {
                                curValueOfDictionary.Add(curTimeDataPoint.Zeitpunkt, curMovementModel.StationID);
                            }
                        }
                    }
                    patientenAufenthalte.Add(curPatientID, curValueOfDictionary);
                }

                // Nach Station sortieren
                List<string> stationsIDs = new List<string>();

                // Liste mit StationenIDs erstellen.
                foreach (var item0 in patientenAufenthalte)
                {
                    SortedDictionary<DateTime, string> curValueOfDictionary = item0.Value;
                    foreach (var item1 in curValueOfDictionary)
                    {
                        string curItem1Value = item1.Value.Trim();
                        if (!stationsIDs.Exists(t => t == curItem1Value))
                        {
                            stationsIDs.Add(curItem1Value);
                        }
                    }
                }

                foreach (string curStationID in stationsIDs)
                {
                    int[] curCounts = new int[(int)(parameter.Endtime - parameter.Starttime).TotalDays + 1];
                    SortedDictionary<DateTime, int> curSortedDictionary = new();
                    foreach (var item0 in patientenAufenthalte)
                    {
                        SortedDictionary<DateTime, string> curValueOfDictionary = new();
                        foreach (var item1 in item0.Value)
                        {
                            if ((item1.Value).Trim() == curStationID)
                            {
                                curValueOfDictionary.Add(item1.Key, item1.Value);
                            }
                        }

                        if (!(curValueOfDictionary.Count == 0))
                        {
                            DateTime curDate = new(curValueOfDictionary.Keys.Min().Year,
                                                   curValueOfDictionary.Keys.Min().Month,
                                                   curValueOfDictionary.Keys.Min().Day,
                                                   curValueOfDictionary.Keys.Min().Hour,
                                                   curValueOfDictionary.Keys.Min().Minute,
                                                   curValueOfDictionary.Keys.Min().Second);
                            int curIndex = (int)(curDate - parameter.Starttime).TotalDays;
                            curCounts[curIndex] += 1;
                        }
                    }

                    for (int o = 0; o < curCounts.Length; o++)
                    {
                        curSortedDictionary.Add(parameter.Starttime.AddDays(o), curCounts[o]);
                    }
                    sortedStations.Add(curStationID, curSortedDictionary);
                }

                foreach (string curPatientID in patientenIDs)
                {
                    SortedDictionary<DateTime, string> curValueOfDictionary = patientenAufenthalte[curPatientID];
                }
            }
            else if (kindOfFinding == "microbiological")
            {
                //PW20210510__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter).Query);
                //PW20210705__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter));
                //PW20210817__specimenIdentifierList = _restData.AQLQuery<TimeDataPointModel>(AQLCatalog.InfectionsStatusDevelopmentCurveMicrobiologicalFinding(parameter));
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("Microbiological switch");
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("The length of the list is in this case {0}", specimenIdentifierList.Count);
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("");

                /*foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    try
                    {
                        // Case for ''Kein Nachweis'', i.e. ''negativ''
                        if (curTimeDataPoint.VirologicalTestResult.Length > " Nachweis ".Length)
                        {
                            curTimeDataPoint.VirologicalTestResult = "negativ";
                        }
                        // Case for ''Nachweis'', i.e. ''positiv''
                        else if (curTimeDataPoint.VirologicalTestResult.Length < "Kein Nachweis".Length-1)
                        {
                            curTimeDataPoint.VirologicalTestResult = "positiv";
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }*/

                foreach (TimeDataPointModel curTimeDataPoint in specimenIdentifierList)
                {
                    System.Diagnostics.Debug.WriteLine("    {0}    {1}", curTimeDataPoint.Zeitpunkt.Date, curTimeDataPoint.VirologicalTestResult);

                    try
                    {
                        curTimeDataPoint.VirologicalTestResult = "positiv";
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }
            }
            //END Different treatment for 'virological' and 'microbiological' finding

            return sortedStations;
        }

    }
}
