using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
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
      

        public InfectionSituationFactory(ICountFactory countFactory, IStationaryFactory stationaryFactory, 
                                         ISymptomFactory symptomFactory, ILogger<InfectionSituationFactory> logger)
        {
            _countFactory = countFactory;
            _stationaryFactory = stationaryFactory;
            _symptomFactory = symptomFactory;
            _logger= logger;
        }

        public List<Patient> Process()
        {
            List<Patient> patNoskumalList = new();
            List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
            "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
            "Pain in throat (finding)"});

            //Get all positive Patient
            List<CountDataModel> positivPatList = _countFactory.Process(positiv);


            //Check if the patient has been treated as an inpatient 
            foreach (CountDataModel positivPat in positivPatList)
            {
                List<StationaryDataModel> statPatList = _stationaryFactory.Process(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);

                if (statPatList != null || statPatList.Count != 0)
                {
                    foreach (StationaryDataModel statPatient in statPatList)
                    {
                        //Check if the patient has any Symptoms at Date of recording
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

    }
}
