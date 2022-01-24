using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public class InfectionStatusFactory : IInfectionStatusFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly ILabResultFactory _mibiResultFac;
        private readonly IHospitalizationFactory _hospitalizationFac;

        public InfectionStatusFactory(IRestDataAccess restDataAccess, ILabResultFactory mibiResultFac, IHospitalizationFactory hospitalizationFac)
        {
            RestDataAccess = restDataAccess;
            _mibiResultFac = mibiResultFac;
            _hospitalizationFac = hospitalizationFac;
        }

        public SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> Process(Patient patient, string MedicalField)
        {
           return Process(patient, MedicalField, null);
        }

        public SortedList<Hospitalization, Dictionary<string, InfectionStatus>> Process(Patient patient, PathogenParameter pathogen)
        {
            SortedList<Hospitalization, Dictionary<string, InfectionStatus>> retVal = retVal = new SortedList<Hospitalization, Dictionary<string, InfectionStatus>>();
            SortedList <Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> tmp = Process(patient, null, pathogen);
            for (int i = 0; i < tmp.Keys.Count; i++)
            {
                if(tmp[tmp.Keys[i]].Count > 0 && tmp[tmp.Keys[i]].ContainsKey(pathogen.Name))
                {
                    retVal.Add(tmp.Keys[i], tmp[tmp.Keys[i]][pathogen.Name]);
                }
            }
            return retVal;
        }

        public SortedList<Hospitalization, InfectionStatus> Process(Patient patient, PathogenParameter pathogen, string Resistence)
        {
            SortedList<Hospitalization, InfectionStatus> retVal = new SortedList<Hospitalization, InfectionStatus>();
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> tmp = Process(patient, null, pathogen);
            for(int i = 0; i < tmp.Keys.Count; i++)
            {
                retVal.Add(tmp.Keys[i], tmp[tmp.Keys[i]][pathogen.Name][Resistence]);
            }
            return retVal;
        }

        private SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> Process(Patient patient, string MedicalField = null, PathogenParameter pathogen = null)
        {
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionInformationByCase = new SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>();

            foreach (Case c in cases)
            {
                List<LabResult> results = null;
                Hospitalization hospitalization = _hospitalizationFac.Process(c);
                if (MedicalField == null)
                {
                    results = _mibiResultFac.Process(c, pathogen);
                }
                else if (pathogen == null)
                {
                    results = _mibiResultFac.Process(c, MedicalField);
                }
                Dictionary<string, Dictionary<string, InfectionStatus>>infectionInformation = new Dictionary<string, Dictionary<string, InfectionStatus>>();

                DetermineInfectionInformation(ref infectionInformation, results, hospitalization, infectionInformationByCase);
                infectionInformationByCase.Add(hospitalization, infectionInformation);

            }
            return infectionInformationByCase;
        }

        private void DetermineInfectionInformation(ref Dictionary<string, Dictionary<string, InfectionStatus>> infectionInformation, List<LabResult> results, Hospitalization hospitalization, SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionInformationByCase)
        {
            if (results != null)
            {
                foreach (LabResult result in results)
                {
                    foreach (Specimen specimen in result.Specimens)
                    {
                        TimeSpan timespan = specimen.SpecimenCollectionDateTime - hospitalization.Admission.Date;
                        foreach (Pathogen pathogen in specimen.Pathogens)
                        {
                            int threshold = GetNosocomialThreshold(pathogen);
                            List<string> resistances = Rules.GetResistances(pathogen);
                            if (resistances == null)
                            {
                                infectionInformation[pathogen.Name].Add("normal", new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = false, Known = false, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = "normal", LabID = specimen.LabID });
                            }
                            else
                            {
                                infectionInformation[pathogen.Name] = new Dictionary<string, InfectionStatus>();

                                foreach (string res in resistances)
                                {
                                    if (pathogen.Result && timespan.Days < threshold)
                                    {
                                        infectionInformation[pathogen.Name].Add(res, new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = res, LabID = specimen.LabID });
                                    }
                                    else if (pathogen.Result && timespan.Days >= threshold)
                                    {
                                        bool hasFoundOldStatus = HasOldKnownCase(pathogen, res, infectionInformationByCase);
                                        if (!hasFoundOldStatus)
                                        {
                                            infectionInformation[pathogen.Name].Add(res, new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = true, Known = false, NosocomialDate = specimen.SpecimenCollectionDateTime, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = res, LabID = specimen.LabID });
                                        }
                                        else
                                        {
                                            infectionInformation[pathogen.Name].Add(res, new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = res, LabID = specimen.LabID });
                                        }
                                    }
                                }
                                SetHealingValue(infectionInformation[pathogen.Name], pathogen.Result, resistances);
                            }
                        }                            
                    }
                }
            }
            else
            {

            }
        }

        private void SetHealingValue(Dictionary<string, InfectionStatus> infectionByResistance, bool result, List<string> resistances)
        {
            foreach (string mre in infectionByResistance.Keys)
            {
                if (result == true && !resistances.Contains(mre))
                {
                    infectionByResistance[mre].ConsecutiveNegativeCounter++;
                }
                else if (result == false)
                {
                    infectionByResistance[mre].ConsecutiveNegativeCounter++;
                }
            }
        }

        private bool HasOldKnownCase(Pathogen pathogen, string resistance, SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionInformationByCase)
        {
            foreach (Hospitalization hospi in infectionInformationByCase.Keys)
            {
                if (infectionInformationByCase[hospi].ContainsKey(pathogen.Name))
                {
                    if ((infectionInformationByCase[hospi][pathogen.Name][resistance].Nosocomial || infectionInformationByCase[hospi][pathogen.Name][resistance].Known) && infectionInformationByCase[hospi][pathogen.Name][resistance].Healed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int GetNosocomialThreshold(Pathogen pathogen)
        {
            string pathogenName = pathogen.Name.ToLower().Replace(" ", "");

            //Needs a saving possibility for different nosocomial thresholds
            return 3;
        }

    }    
}
