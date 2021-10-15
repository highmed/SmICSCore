using System.Collections.Generic;
using System;
using SmICSCoreLib.Factories.PatientStay;
using SmICSCoreLib.Factories.PatientStay.Stationary;
using SmICSCoreLib.Factories.PatientStay.Count;
using SmICSCoreLib.Factories.Symptome;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.StatistikDataModels;
using Microsoft.Extensions.Logging;

namespace SmICSCoreLib.StatistikServices
{
    public class EhrDataService
    {
        private readonly IPatientStay _patientStay;
        private readonly IPatientMovementFactory _patientMoveFac;
        private readonly ILogger<EhrDataService> _logger;
        private readonly ISymptomFactory _symptomFac;

        public EhrDataService(IPatientStay patinet_Stay, IPatientMovementFactory patientMoveFac, ISymptomFactory symptomFac, ILogger<EhrDataService> logger)
        {
            _patientStay = patinet_Stay;
            _patientMoveFac = patientMoveFac;
            _logger = logger;
            _symptomFac = symptomFac;
        }

        //Load EhrData

        //Liste alle Stationaer behandelte Patienten mit: PatientID, FallID und Datum.
        public List<StationaryDataModel> StationaryPatForNosku(string patientID, string fallID, DateTime datum)
        {
            try
            {
                List<StationaryDataModel> stationaryDatas = _patientStay.Stationary_Stay(patientID, fallID, datum);
                _logger.LogInformation("StationaryPatForNosku");
                return stationaryDatas;
            }
            catch (Exception e)
            {
                _logger.LogWarning("StationaryPatForNosku " + e.Message);
                return null;
            }
        }

        //Liste Stationaer behandelte Patienten mit: PatientId und FallID.
        public List<StationaryDataModel> StationaryPatByCaseID(string patientID, string fallID)
        {
            try
            {
                List<StationaryDataModel> patStationary = _patientStay.StayFromCase(patientID, fallID);
                _logger.LogInformation("StationaryPatByCaseID");
                return patStationary;
            }
            catch (Exception e)
            {
                _logger.LogWarning("StationaryPatByCaseID " + e.Message);
                return null;
            }
        }

        //Liste alle Stationaer behandelte Patienten mit: Datum.
        public List<StationaryDataModel> StationaryPatByDate(DateTime datum)
        {
            try
            {
                List<StationaryDataModel> patStationary = _patientStay.StayFromDate(datum);
                _logger.LogInformation("StationaryPatByDate");
                return patStationary;
            }
            catch (Exception e)
            {
                _logger.LogWarning("StationaryPatByDate " + e.Message);
                return null;
            }
        }

        // Liste Patientenbewegungen mit: PatientIDs
        public List<PatientMovementModel> GetPatMovement(string patientId)
        {
            try
            {
                List<string> patientList = new();
                patientList.Add(patientId);
                PatientListParameter patListParameter = new();
                patListParameter.patientList = patientList;
                List<PatientMovementModel> patientMovement = _patientMoveFac.Process(patListParameter);

                _logger.LogInformation("GetPatMovement");
                return patientMovement;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetPatMovement " + e.Message);
                return null;
            }
        }

        // Liste Patientenbewegungen mit: PatientID, Station, Starttime and Endtime
        public List<PatientMovementModel> GetPatMovementFromStation(List<string> patientList, string station, DateTime starttime, DateTime endtime)
        {
            try
            {
                PatientListParameter patListParameter = new();
                patListParameter.patientList = patientList;
                List<PatientMovementModel> patientMovement = _patientMoveFac.ProcessFromStation(patListParameter, station, starttime, endtime);
                _logger.LogInformation("GetPatMovementFromStation");
                return patientMovement;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetPatMovementFromStation " + e.Message);
                return null;
            }
        }

        // Liste CovidPatienten mit: BefundNachweis
        public List<CountDataModel> GetCovidPat(string nachweis)
        {
            try
            {
                List<CountDataModel> covidPat = _patientStay.CovidPat(nachweis);
                _logger.LogInformation("GetCovidPat");
                return covidPat;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetCovidPat " + e.Message);
                return null;
            }
        }

        // Liste alle positive Tests mit: PositiveNachweis
        public List<CountDataModel> GetAllPositivTest(string positivCodeString)
        {
            try
            {
                List<CountDataModel> allPositivTest = GetCovidPat(positivCodeString);
                _logger.LogInformation("GetAllPositivTest");
                return allPositivTest;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetAllPositivTest " + e.Message);
                return null;
            }
        }

        // Liste alle negative Tests mit: NegativeNachweis
        public List<CountDataModel> GetAllNegativTest(string negativeCodeString)
        {
            try
            {
                List<CountDataModel> allNegativPat = GetCovidPat(negativeCodeString);
                _logger.LogInformation("GetAllNegativTest");
                return allNegativPat;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetAllNegativTest " + e.Message);
                return null;
            }
        }

        // Liste alle Patienten die einmal positive oder negative waren: Liste positive/negative Patienten
        public List<CountDataModel> GetAllPatByTest(List<CountDataModel> allTests)
        {
            try
            {
                List<CountDataModel> testPat = new();
                foreach (CountDataModel countData in allTests)
                {
                    if (!testPat.Contains(countData))
                    {
                        testPat.Add(countData);
                    }
                    else
                    {
                        CountDataModel data = testPat.Find(i => i.PatientID == countData.PatientID);

                        if (data.Zeitpunkt_des_Probeneingangs > countData.Zeitpunkt_des_Probeneingangs)
                        {
                            testPat.Remove(data);
                            testPat.Add(countData);
                        }
                    }
                }
                _logger.LogInformation("GetAllPatByTest");
                return testPat;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetAllPatByTest " + e.Message);
                return null;
            }
        }

        //Noskumale Regeln
        //1.Regel Stationaer Behandlung, keine Symptome bei Aufnahme und positive Test ab Tag 4.
        public List<PatientModel> GetAllNoskumalPat(List<CountDataModel> positivPatList)
        {
            try
            {
                SymptomService symptom = new(_symptomFac, this);

                List<PatientModel> patNoskumalList = new();
                List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
                "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
                "Pain in throat (finding)"});

                foreach (CountDataModel positivPat in positivPatList)
                {
                    List<StationaryDataModel> statPatList = StationaryPatForNosku(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);

                    if (statPatList != null)
                    {
                        if (statPatList.Count != 0)
                        {
                            foreach (StationaryDataModel statPatient in statPatList)
                            {
                                List<SymptomModel> symptoms = symptom.GetAllSymByPatID(statPatient.PatientID, statPatient.Datum_Uhrzeit_der_Aufnahme);
                                if (symptoms is null || symptoms.Count == 0)
                                {
                                    patNoskumalList.Add(new PatientModel(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                                }
                                else
                                {
                                    foreach (var symptomItem in symptoms)
                                    {
                                        if (!symptomList.Contains(symptomItem.NameDesSymptoms) &&
                                            !patNoskumalList.Contains(new PatientModel { PatientID = positivPat.PatientID }))
                                        {
                                            patNoskumalList.Add(new PatientModel(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                _logger.LogInformation("GetAllNoskumalPat");
                return patNoskumalList;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetAllNoskumalPat " + e.Message);
                return null;
            }
        }

        //2.Regel Kontakt mit einem positiven Patient 
        public List<PatientModel> GetNoskumalByContact(List<PatientModel> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            try
            {
                List<PatientModel> patNoskumalList = new();

                foreach (var patient in allNoskumalPat)
                {
                    List<PatientMovementModel> patBewegungen = GetPatMovement(patient.PatientID);
                    if (patBewegungen.Count != 0)
                    {
                        foreach (var bewegung in patBewegungen)
                        {
                            if (bewegung.Beginn < bewegung.Ende.AddMinutes(-15))
                            {
                                List<PatientMovementModel> patientMovement = FindContact(allPositivPat, bewegung.PatientID,
                                    bewegung.Fachabteilung, bewegung.Beginn, bewegung.Ende);

                                if (patientMovement.Count != 0 && !patNoskumalList.Contains(patient))
                                {
                                    patNoskumalList.Add(patient);
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation("GetNoskumalByContact");
                return patNoskumalList;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetNoskumalByContact " + e.Message);
                return null;
            }
        }

        public List<PatientMovementModel> FindContact(List<CountDataModel> allPositivPat, string patientID, string station, DateTime beginn, DateTime ende)
        {
            try
            {
                List<PatientMovementModel> patientMovement = new();
                List<string> patientList = new();

                foreach (var positivPat in allPositivPat)
                {
                    patientList.Add(positivPat.PatientID);
                }

                List<PatientMovementModel> patBewegungen = GetPatMovementFromStation(patientList, station, beginn, ende);
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

                _logger.LogInformation("FindContact");
                return patientMovement;
            }
            catch (Exception e)
            {
                _logger.LogWarning("FindContact " + e.Message);
                return null;
            }
        }

        //Anzahl Patiententage im Krankenhaus: Liste positive Patienten
        public int PatStay(List<CountDataModel> positivPat)
        {
            try
            {
                double start, gesamt = 0;
                foreach (CountDataModel item in positivPat)
                {
                    List<StationaryDataModel> statPatList = StationaryPatByCaseID(item.PatientID, item.Fallkennung);
                    if (statPatList != null && statPatList.Count != 0)
                    {
                        foreach (StationaryDataModel statData in statPatList)
                        {
                            if (statData.Datum_Uhrzeit_der_Entlassung.GetHashCode() == 0)
                            {
                                statData.Datum_Uhrzeit_der_Entlassung = DateTime.Now;
                            }
                            start = (statData.Datum_Uhrzeit_der_Entlassung - statData.Datum_Uhrzeit_der_Aufnahme).TotalDays;
                            gesamt += start;
                        }
                    }
                }

                _logger.LogInformation("PatStay");
                return Convert.ToInt32(gesamt);
            }
            catch (Exception e)
            {
                _logger.LogWarning("PatStay " + e.Message);
                return 0;
            }
        }

    }
}
