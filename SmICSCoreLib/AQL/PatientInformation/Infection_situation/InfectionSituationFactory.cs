using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.PatientInformation.Patient_Bewegung;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.StatistikDataModels;

namespace SmICSCoreLib.AQL.PatientInformation.Infection_situation
{
    public class InfectionSituationFactory : IInfectionSituationFactory
    {
        private readonly string positiv = "260373001";
        private readonly ILogger<InfectionSituationFactory> _logger;

        private readonly ICountFactory _countFactory;
        private readonly IStationaryFactory _stationaryFactory;
        private readonly ISymptomFactory _symptomFactory;
        private readonly IPatientMovementFactory _patMoveFac;


        public InfectionSituationFactory(ICountFactory countFactory, IStationaryFactory stationaryFactory, 
                                         ISymptomFactory symptomFactory, IPatientMovementFactory patMoveFac,
                                         ILogger<InfectionSituationFactory> logger)
        {
            _countFactory = countFactory;
            _stationaryFactory = stationaryFactory;
            _symptomFactory = symptomFactory;
            _patMoveFac = patMoveFac;
            _logger = logger;
        }

        public List<Patient> Process(PatientListParameter parameter) {
            
            List<Patient> patienten = new();
            //Liste alle positive Patienten
            List<CountDataModel> positivPatList = _countFactory.Process(positiv);          
            List<Patient> PosPatientenListe = PossibleNosocomialsList(positivPatList);
            List<Patient> ProPatientenListe = ProbableNosocomialsList(PosPatientenListe, positivPatList);
            foreach (var patientID in parameter.patientList)
            {
                foreach (var posPat in PosPatientenListe)
                {
                    if (posPat.PatientID == patientID)
                    {
                        if (ProPatientenListe.Contains(posPat))
                        {
                            patienten.Add(new Patient(posPat.PatientID, posPat.Probenentnahme, posPat.Aufnahme, posPat.Entlastung, "Wahrscheinliche Nosokomiale"));
                        }
                        else
                        {
                            patienten.Add(new Patient(posPat.PatientID, posPat.Probenentnahme, posPat.Aufnahme, posPat.Entlastung, "Mögliche Nosokomiale"));
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

            //Hole alle positive Patienten
            //List<CountDataModel> positivPatList = _countFactory.Process(positiv);


            //Ueberpruefen, ob der Patient stationär behandelt ist
            foreach (CountDataModel positivPat in positivPatList)
            {
                List<StationaryDataModel> statPatList = _stationaryFactory.Process(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);

                if (statPatList != null || statPatList.Count != 0)
                {
                    foreach (StationaryDataModel statPatient in statPatList)
                    {
                        //Ueberpruefen, ob der Patient am Aufnahmedatum Symptome hat
                        List<SymptomModel> symptoms = _symptomFactory.SymptomByPatient(statPatient.PatientID, statPatient.Datum_Uhrzeit_der_Aufnahme);
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

        public List<Patient> ProbableNosocomialsList(List<Patient> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            List<string> patientList = new();
            PatientListParameter patListParameter = new();
            List<Patient> patNoskumalList = new();

            foreach (var patient in allNoskumalPat)
            {               
                patientList.Add(patient.PatientID);
                patListParameter.patientList = patientList;

                List<PatientMovementModel> patBewegungen = _patMoveFac.Process(patListParameter);
                if (patBewegungen.Count != 0)
                {
                    foreach (var bewegung in patBewegungen)
                    {
                        if (bewegung.Beginn < bewegung.Ende.AddMinutes(-15))
                        {
                            List<PatientMovementModel> patientMovement = FindContact(allPositivPat, bewegung.PatientID,
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
    }
}
