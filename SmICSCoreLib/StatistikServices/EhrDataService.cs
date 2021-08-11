using System.Collections.Generic;
using System;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.StatistikServices
{
    public class EhrDataService
    {
        private readonly IPatinet_Stay _patinet_Stay;
        private readonly IPatientInformation _patientInformation;

        public EhrDataService(IPatinet_Stay patinet_Stay, IPatientInformation patientInformation)
        {
            _patinet_Stay = patinet_Stay;
            _patientInformation = patientInformation;
        }

        //Load EhrData

        //Liste alle Stationaer behandelte Patienten mit: PatientID, FallID und Datum.
        public List<StationaryDataModel> StationaryPatForNosku(string patientID, string fallID, DateTime datum)
        {
            try
            {
                List<StationaryDataModel> stationaryDatas = _patinet_Stay.Stationary_Stay(patientID, fallID, datum);
                return stationaryDatas;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Liste Stationaer behandelte Patienten mit: PatientId und FallID.
        public List<StationaryDataModel> StationaryPatByCaseID(string patientID, string fallID)
        {
            try
            {
                List<StationaryDataModel> patStationary = _patinet_Stay.StayFromCase(patientID, fallID);
                return patStationary;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Liste alle Stationaer behandelte Patienten mit: Datum.
        public List<StationaryDataModel> StationaryPatByDate(DateTime datum)
        {
            try
            {
                List<StationaryDataModel> patStationary = _patinet_Stay.StayFromDate(datum);
                return patStationary;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Liste Patientenbewegungen mit: PatientIDs
        public List<PatientMovementModel> GetPatMovement(string patientId)
        {
            List<string> patientList = new();
            patientList.Add(patientId);
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Ps(patListParameter);
            return patientMovement;
        }

        // Liste Patientenbewegungen mit: PatientID, Station, Starttime and Endtime
        public List<PatientMovementModel> GetPatMovementFromStation(List<string> patientList, string station, DateTime starttime, DateTime endtime)
        {
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Station(patListParameter, station, starttime, endtime);
            return patientMovement;
        }

        // Liste CovidPatienten mit: BefundNachweis
        public List<CountDataModel> GetCovidPat(string nachweis)
        {
            List<CountDataModel> covidPat = _patinet_Stay.CovidPat(nachweis);
            return covidPat;
        }

        // Liste alle positive Tests mit: PositiveNachweis
        public List<CountDataModel> GetAllPositivTest()
        {
            List<CountDataModel> allPositivTest = GetCovidPat("260373001");
            return allPositivTest;
        }

        // Liste alle negative Tests mit: NegativeNachweis
        public List<CountDataModel> GetAllNegativTest()
        {
            List<CountDataModel> allNegativPat = GetCovidPat("260415000");
            return allNegativPat;
        }

        // Liste alle Patienten die einmal positive oder negative waren: Liste positive/negative Patienten
        public List<CountDataModel> GetAllPatByTest(List<CountDataModel> allTest)
        {
            List<CountDataModel> testPat = new();
            foreach (CountDataModel countData in allTest)
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
            return testPat;
        }

        //Noskumale Regeln
        //1.Regel Stationaer Behandlung, keine Symptome bei Aufnahme und positive Test ab Tag 4.
        public List<Patient> GetAllNoskumalPat(List<CountDataModel> positivPatList)
        {
            SymptomService symptom = new(_patientInformation, this);

            List<Patient> patNoskumalList = new();
            List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
            "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
            "Pain in throat (finding)"});

            foreach (CountDataModel positivPat in positivPatList)
            {
                List<StationaryDataModel> statPatList = StationaryPatForNosku(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);

                if (statPatList != null || statPatList.Count != 0)
                {
                    foreach (StationaryDataModel statPatient in statPatList)
                    {
                        List<SymptomModel> symptoms = symptom.GetAllSymByPatID(statPatient.PatientID, statPatient.Datum_Uhrzeit_der_Aufnahme);
                        if (symptoms is null || symptoms.Count == 0)
                        {
                            patNoskumalList.Add(new Patient(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                        }
                        else
                        {
                            foreach (var symptomItem in symptoms)
                            {
                                if (!symptomList.Contains(symptomItem.NameDesSymptoms) &&
                                    !patNoskumalList.Contains(new Patient { PatientID = positivPat.PatientID }))
                                {
                                    patNoskumalList.Add(new Patient(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                                }
                            }
                        }

                    }
                }
            }
            return patNoskumalList;
        }

        //2.Regel Kontakt mit einem positiven Patient 
        public List<Patient> GetNoskumalByContact(List<Patient> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            List<Patient> patNoskumalList = new();

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
            return patNoskumalList;
        }

        public List<PatientMovementModel> FindContact(List<CountDataModel> allPositivPat, string patientID, string station, DateTime beginn, DateTime ende)
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
            return patientMovement;
        }

        //Anzahl Patiententage im Krankenhaus: Liste positive Patienten
        public int PatStay(List<CountDataModel> positivPat)
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
            return Convert.ToInt32(gesamt);
        }

    }
}
