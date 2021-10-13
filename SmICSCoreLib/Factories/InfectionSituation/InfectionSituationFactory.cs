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

namespace SmICSCoreLib.Factories.InfectionSituation
{
    public class InfectionSituationFactory : IInfectionSituationFactory
    {
        private readonly string positiv = "260373001";

        private readonly ICountFactory _countFactory;
        private readonly IStationaryFactory _stationaryFactory;
        private readonly ISymptomFactory _symptomFactory;
        private readonly IPatientMovementFactory _patMoveFac;
        private readonly IVaccinationFactory _vaccFac;
        private readonly ILogger<InfectionSituationFactory> _logger;


        public InfectionSituationFactory(ICountFactory countFactory, IStationaryFactory stationaryFactory,
                                         ISymptomFactory symptomFactory, IPatientMovementFactory patMoveFac,
                                         IVaccinationFactory vaccFac, ILogger<InfectionSituationFactory> logger)
        {
            _countFactory = countFactory;
            _symptomFactory = symptomFactory;
            _stationaryFactory = stationaryFactory;
            _patMoveFac = patMoveFac;
            _vaccFac = vaccFac;
            _logger = logger;
        }

        public List<PatientModel> Process(PatientListParameter parameter)
        {
            List<PatientModel> patienten = new();
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
            List<PatientModel> posPatientenListe = PossibleNosocomialsList(sortPattList);
            if (posPatientenListe != null && posPatientenListe.Count != 0)
            {
                //Liste wahrscheinliche Nosokomiale Infektionen
                List<PatientModel> proPatientenListe = ProbableNosocomialsList(posPatientenListe, allPositivPatList);
                foreach (var patientID in parameter.patientList)
                {
                    if (posPatientenListe.Contains(new PatientModel { PatientID = patientID }))
                    {
                        PatientModel patient = posPatientenListe.Find(x => x.PatientID == patientID);
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
                                patienten.Add(new PatientModel(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                         "Wahrscheinliche Nosokomiale Infektion", patVaccination));
                            }
                            else
                            {
                                patienten.Add(new PatientModel(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                          "Moegliche Nosokomiale Infektion", patVaccination));
                            }
                        }
                        else
                        {
                            if (proPatientenListe.Contains(patient))
                            {
                                patienten.Add(new PatientModel(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                          "Wahrscheinliche Nosokomiale Infektion", null));
                            }
                            else
                            {
                                patienten.Add(new PatientModel(patient.PatientID, patient.Probenentnahme, patient.Aufnahme, patient.Entlastung,
                                                          "Moegliche Nosokomiale Infektion", null));
                            }
                        }
                    }
                }
            }
            return patienten;
        }

        public List<PatientModel> PossibleNosocomialsList(List<CountDataModel> positivPatList)
        {
            List<PatientModel> patNoskumalList = new();
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
                            patNoskumalList.Add(new PatientModel(positivPat.PatientID,
                                                            positivPat.Zeitpunkt_des_Probeneingangs,
                                                            statPatient.Datum_Uhrzeit_der_Aufnahme,
                                                            statPatient.Datum_Uhrzeit_der_Entlassung));
                        }
                        else
                        {
                            foreach (var symptom in symptoms)
                            {
                                if (!symptomList.Contains(symptom.NameDesSymptoms) &&
                                    !patNoskumalList.Contains(new PatientModel { PatientID = positivPat.PatientID }))
                                {
                                    patNoskumalList.Add(new PatientModel(positivPat.PatientID,
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

        public List<PatientModel> ProbableNosocomialsList(List<PatientModel> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            List<PatientModel> patNoskumalList = new();
            PatientListParameter patListParameter = new();

            foreach (var patient in allNoskumalPat)
            {
                List<string> patientList = new();
                patientList.Add(patient.PatientID);
                patListParameter.patientList = patientList;

                List<PatientMovementModel> nosPatBewegungen = _patMoveFac.Process(patListParameter);
                if (nosPatBewegungen.Count != 0)
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

    }
}