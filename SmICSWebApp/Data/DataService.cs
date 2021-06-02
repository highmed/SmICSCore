using SmICSWebApp.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using System.IO;
using ExcelDataReader;
using System.Net;
using System.Data;
using SmICSCoreLib.JSONFileStream;
using System.Collections;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.General;

namespace SmICSWebApp.Data
{
    public class DataService
    {
        private readonly IPatinet_Stay _patinet_Stay;
        private readonly IPatientInformation _patientInformation;

        public DataService(IPatinet_Stay patinet_Stay, IPatientInformation patientInformation)
        {
            _patinet_Stay = patinet_Stay;
            _patientInformation = patientInformation;
        }

        //Load EhrData
        public List<StationaryDataModel> GetStationaryPat(string patientID, string fallkennung, DateTime datum)
        {
            List<StationaryDataModel> stationaryDatas = _patinet_Stay.Stationary_Stay(patientID, fallkennung, datum);
            return stationaryDatas;
        }

        public List<StationaryDataModel> StayFromCase(string patientId, string fallId)
        {
            List<StationaryDataModel> patStationary = _patinet_Stay.StayFromCase(patientId, fallId);
            return patStationary;
        }

        public List<PatientMovementModel> GetPatMovement(string patientId)
        {
            List<string> patientList = new();
            patientList.Add(patientId);
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Ps(patListParameter);
            return patientMovement;
        }

        public List<PatientMovementModel> GetPatMovementFromStation(List<string> patientList, string station, DateTime starttime, DateTime endtime)
        {
            //List<string> patientList = new();
            //patientList.Add(patientId);
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Station(patListParameter, station, starttime, endtime);
            return patientMovement;
        }

        public List<CountDataModel> GetCovidPat(string nachweis)
        {
            List<CountDataModel> covidPat = _patinet_Stay.CovidPat(nachweis);
            return covidPat;
        }
        
        public List<CountDataModel> GetAllPositivTest()
        {
            List<CountDataModel> allPositivTest = GetCovidPat("260373001");
            return allPositivTest;
        }
       
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
      
        public List<CountDataModel> GetAllNegativTest()
        {
            List<CountDataModel> allNegativPat = GetCovidPat("260415000");
            return allNegativPat;
        }
       
        public List<SymptomModel> GetAllSymByPat(string patientId, DateTime datum)
        {
            List<SymptomModel> symptomListe = _patientInformation.Symptoms_By_PatientId(patientId, datum);
            return symptomListe;
        }
       
        public List<Patient> GetAllNoskumalPat(List<CountDataModel> positivPatList)
        {
            List<Patient> patNoskumalList = new();
            List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
            "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
            "Pain in throat (finding)"});

            foreach (CountDataModel positivPat in positivPatList)
            {
                List<StationaryDataModel> statPatList = GetStationaryPat(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);

                if (statPatList != null || statPatList.Count != 0)
                {
                    foreach (StationaryDataModel statPatient in statPatList)
                    {
                        List<SymptomModel> symptoms = GetAllSymByPat(statPatient.PatientID, statPatient.Datum_Uhrzeit_der_Aufnahme);
                        if (symptoms is null || symptoms.Count == 0)
                        {
                            patNoskumalList.Add(new Patient(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                        }
                        else
                        {
                            foreach (var symptom in symptoms)
                            {
                                if (!symptomList.Contains(symptom.NameDesSymptoms))
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
        
        public List<Patient> GetNoskumalByContact(List<Patient> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            List<Patient> patNoskumalList = new ();

            foreach (var patient in allNoskumalPat)
            {
                List<PatientMovementModel> patBewegungen = GetPatMovement(patient.PatientID);
                if (patBewegungen.Count != 0)
                {
                    foreach (var bewegung in patBewegungen)
                    {
                        if (bewegung.Beginn < bewegung.Ende.AddMinutes(-15))
                        {
                            List <PatientMovementModel> patientMovement = FindContact(allPositivPat, bewegung.PatientID, 
                                bewegung.Fachabteilung, bewegung.Beginn, bewegung.Ende);

                            if (patientMovement.Count != 0)
                            {
                                patNoskumalList.Add(patient);
                            }
                        }
                    }
                }
            }
            return patNoskumalList;
        }
       
        public List<PatientMovementModel> FindContact(List<CountDataModel> allPositivPat, string patientID, string station, DateTime beginn, DateTime ende  ) {

            List <PatientMovementModel> patientMovement = new();
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

        public int PatStay(List<CountDataModel> positivPat)
        {
            double start;
            double gesamt = 0;
            foreach (CountDataModel item in positivPat)
            {
                List<StationaryDataModel> statPatList = _patinet_Stay.StayFromCase(item.PatientID, item.Fallkennung);

                foreach (StationaryDataModel statData in statPatList)
                {
                    start = (statData.Datum_Uhrzeit_der_Entlassung - statData.Datum_Uhrzeit_der_Aufnahme).TotalDays;
                    gesamt += start;
                }
            }
            return Convert.ToInt32(gesamt);
        }
  
    }
}
