using Newtonsoft.Json;
using RulesEngine.Models;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.IO;

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

        public SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> Process(Patient patient, PathogenParameter pathogen = null)
        {
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionInformationByCase = new SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>(new HospitalizationComparer());

            foreach (Case c in cases)
            {
                Hospitalization hospitalization = _hospitalizationFac.Process(c);
                List<LabResult> results = _mibiResultFac.Process(c, pathogen);

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
                    if (result.Specimens != null)
                    {
                        foreach (Specimen specimen in result.Specimens)
                        {
                            if (specimen.Pathogens != null)
                            {
                                TimeSpan timespan = hospitalization.Admission.Date - specimen.SpecimenCollectionDateTime;
                                foreach (Pathogen pathogen in specimen.Pathogens)
                                {
                                    int threshold = GetNosocomialThreshold(pathogen);
                                    List<string> resistances = Rules.GetResistances(pathogen);
                                    
                                    foreach (string res in resistances)
                                    {
                                        if (pathogen.Result && timespan.Days < threshold)
                                        {
                                            new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = true, ConsecutiveNegativeCounter = 0, Resistance = res };
                                        }
                                        else if (pathogen.Result && timespan.Days >= threshold)
                                        {
                                            bool hasFoundOldStatus = HasOldKnownCase(pathogen, res, infectionInformationByCase);
                                            if (!hasFoundOldStatus)
                                            {
                                                new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = true, Known = false, NosocomialDate = specimen.SpecimenCollectionDateTime, Infected = true, ConsecutiveNegativeCounter = 0, Resistance = res };
                                            }
                                            else
                                            {
                                                new InfectionStatus { Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = true, ConsecutiveNegativeCounter = 0, Resistance = res };
                                            }
                                        }                                       
                                    }
                                    SetHealingValue(infectionInformation[pathogen.Name], pathogen.Result, resistances);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetHealingValue(Dictionary<string, InfectionStatus> infectionByResistance, bool result, List<string> resistances)
        {
            if (result == true && infectionByResistance.ContainsKey("I") && !resistances.Contains("I"))
            {
                infectionByResistance["I"].ConsecutiveNegativeCounter++;
            }
            if (result == true && infectionByResistance.ContainsKey("R") && !resistances.Contains("R"))
            {
                infectionByResistance["R"].ConsecutiveNegativeCounter++;
            }
            if (result == true && infectionByResistance.ContainsKey("S") && !resistances.Contains("S"))
            {
                infectionByResistance["S"].ConsecutiveNegativeCounter++;
            }
            if (result == false)
            {
                infectionByResistance["I"].ConsecutiveNegativeCounter++;
                infectionByResistance["R"].ConsecutiveNegativeCounter++;
                infectionByResistance["S"].ConsecutiveNegativeCounter++;
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
        private void AddInfectionInformation(ref Dictionary<string, InfectionStatus> infectionInformation, Pathogen pathogen, bool known, bool nosocomial, DateTime? nosocomialData = null)
        {
            InfectionStatus infection = new InfectionStatus() { Known = false, Nosocomial = nosocomial, NosocomialDate = nosocomialData };
            if (!infectionInformation.ContainsKey(pathogen.Name))
            {
                infectionInformation.Add(pathogen.Name, infection);
            }
        }
        private int GetNosocomialThreshold(Pathogen pathogen)
        {

            string pathogenName = pathogen.Name.ToLower().Replace(" ", "");

            //Needs a saving possibility for different nosocomial thresholds
            return 3;
        }
    }

    internal class HospitalizationComparer : IComparer<Hospitalization>
    {
        public int Compare(Hospitalization x, Hospitalization y)
        {
            return y.Admission.Date.CompareTo(x.Admission.Date);
        }
    }
}
