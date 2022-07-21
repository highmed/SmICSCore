using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>> ProcessAsync(Patient patient, string MedicalField)
        {
            try
            { 
                return await ProcessAsync(patient, MedicalField, null);
            }
            catch { throw; }
        }

        public async Task<SortedList<Hospitalization, Dictionary<string, InfectionStatus>>> ProcessAsync(Patient patient, PathogenParameter pathogen)
        {
            try
            {
                SortedList<Hospitalization, Dictionary<string, InfectionStatus>> retVal = retVal = new SortedList<Hospitalization, Dictionary<string, InfectionStatus>>();
                SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> tmp = await ProcessAsync(patient, null, pathogen);
                for (int i = 0; i < tmp.Keys.Count; i++)
                {
                    foreach (string code in pathogen.PathogenCodes)
                    {
                        if (tmp[tmp.Keys[i]].Count > 0 && tmp[tmp.Keys[i]].ContainsKey(code))
                        {
                            retVal.Add(tmp.Keys[i], tmp[tmp.Keys[i]][code]);
                        }
                    }
                }
                return retVal;
            }
            catch
            {
                throw;
            }
        }

        public async Task<SortedList<Hospitalization, InfectionStatus>> ProcessAsync(Patient patient, PathogenParameter pathogen, string Resistence)
        {
            try
            {
                SortedList<Hospitalization, InfectionStatus> retVal = new SortedList<Hospitalization, InfectionStatus>();
                SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> tmp = await ProcessAsync(patient, null, pathogen);
                for (int i = 0; i < tmp.Keys.Count; i++)
                {
                    foreach (string code in pathogen.PathogenCodes)
                    {
                        if (tmp[tmp.Keys[i]].ContainsKey(code))
                        {
                            if (tmp[tmp.Keys[i]][code].ContainsKey(Resistence))
                            {
                                retVal.Add(tmp.Keys[i], tmp[tmp.Keys[i]][code][Resistence]);
                            }
                        }
                    }
                }
                return retVal;
            }
            catch
            {

                throw;
            }
            
        }

        //Hospitalization, Pathogen, Resitance, InfectionStatus
        private async Task<SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>> ProcessAsync(Patient patient, string MedicalField = null, PathogenParameter pathogen = null)
        {
            try
            {
                List<Case> cases = await RestDataAccess.AQLQueryAsync<Case>(AQLCatalog.Cases(patient));
                SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionInformationByCase = new SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>>();
                if (cases is not null)
                {
                    foreach (Case c in cases)
                    {
                        List<LabResult> results = null;
                        Hospitalization hospitalization = await _hospitalizationFac.ProcessAsync(c);
                        if (MedicalField == null)
                        {
                            results = await _mibiResultFac.ProcessAsync(c, pathogen);
                        }
                        else if (pathogen == null)
                        {
                            results = await _mibiResultFac.ProcessAsync(c, MedicalField);
                        }
                        if (results is not null)
                        {
                            results = results.OrderBy(l => l.Specimens.First().SpecimenCollectionDateTime).ToList();

                            Dictionary<string, Dictionary<string, InfectionStatus>> infectionInformation = new Dictionary<string, Dictionary<string, InfectionStatus>>();

                            DetermineInfectionInformation(ref infectionInformation, results, hospitalization, infectionInformationByCase);
                            infectionInformationByCase.Add(hospitalization, infectionInformation);
                        }
                    }
                }
                return infectionInformationByCase;
            }
            catch
            {
                throw;
            } 
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
                            if(pathogen.Name.ToLower().Contains("sar-cov-2"))
                            {
                                resistances = new List<string>() { "sars-cov-2" };
                            }
                            if (!infectionInformation.ContainsKey(pathogen.ID))
                            {
                                infectionInformation.Add(pathogen.ID, new Dictionary<string, InfectionStatus>());
                            }
                            if (resistances == null)
                            {
                                if (pathogen.Result && timespan.Days < (threshold - 1))
                                { 
                                    if (!infectionInformation[pathogen.ID].ContainsKey("keine"))
                                    {
                                        infectionInformation[pathogen.ID].Add("keine", new InfectionStatus { PathogenCode = pathogen.ID, Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = "keine", LabID = specimen.LabID });
                                    }
                                }
                                else if (pathogen.Result && timespan.Days >= (threshold - 1))
                                {
                                    bool hasFoundOldStatus = HasOldKnownCase(pathogen, "keine", infectionInformationByCase);
                                    if (!hasFoundOldStatus)
                                    {
                                        if (!infectionInformation[pathogen.ID].ContainsKey("keine"))
                                        {
                                            infectionInformation[pathogen.ID].Add("keine", new InfectionStatus { PathogenCode = pathogen.ID, Pathogen = pathogen.Name, Nosocomial = true, Known = false, NosocomialDate = specimen.SpecimenCollectionDateTime, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = "keine", LabID = specimen.LabID });
                                        }
                                    }
                                    else
                                    {
                                        if (!infectionInformation[pathogen.ID].ContainsKey("keine"))
                                        {
                                            infectionInformation[pathogen.ID].Add("keine", new InfectionStatus { PathogenCode = pathogen.ID, Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = "keine", LabID = specimen.LabID });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (string res in resistances)
                                {
                                    if (pathogen.Result && timespan.Days < (threshold - 1))
                                    {
                                        if (!infectionInformation[pathogen.ID].ContainsKey(res))
                                        {
                                            infectionInformation[pathogen.ID].Add(res, new InfectionStatus { PathogenCode = pathogen.ID, Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = res, LabID = specimen.LabID });
                                        }
                                    }
                                    else if (pathogen.Result && timespan.Days >= (threshold - 1))
                                    {
                                        bool hasFoundOldStatus = HasOldKnownCase(pathogen, res, infectionInformationByCase);
                                        if (!hasFoundOldStatus)
                                        {
                                            if (!infectionInformation[pathogen.ID].ContainsKey(res))
                                            {
                                                infectionInformation[pathogen.ID].Add(res, new InfectionStatus { PathogenCode = pathogen.ID, Pathogen = pathogen.Name, Nosocomial = true, Known = false, NosocomialDate = specimen.SpecimenCollectionDateTime, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = res, LabID = specimen.LabID });
                                            }
                                        }
                                        else
                                        {
                                            if (!infectionInformation[pathogen.ID].ContainsKey(res))
                                            {
                                                infectionInformation[pathogen.ID].Add(res, new InfectionStatus { PathogenCode = pathogen.ID, Pathogen = pathogen.Name, Nosocomial = false, Known = true, NosocomialDate = null, Infected = pathogen.Result, ConsecutiveNegativeCounter = 0, Resistance = res, LabID = specimen.LabID });
                                            }
                                        }
                                    }
                                }
                            }
                            SetHealingValue(infectionInformation[pathogen.ID], pathogen.Result, resistances, specimen.SpecimenCollectionDateTime);
                        }                            
                    }
                }
            }
        }

        private void SetHealingValue(Dictionary<string, InfectionStatus> infectionByResistance, bool result, List<string> resistances, DateTime specimenDate)
        {
            foreach (string mre in infectionByResistance.Keys)
            {
                if (resistances is not null)
                {
                    if ((result == true && !resistances.Contains(mre)) || result == false)
                    {
                        infectionByResistance[mre].ConsecutiveNegativeCounter++;
                        if (infectionByResistance[mre].ConsecutiveNegativeCounter == 3)
                        {
                            infectionByResistance[mre].Healed = true;
                            infectionByResistance[mre].HealedDate = specimenDate;
                            infectionByResistance[mre].ConsecutiveNegativeCounter = 0;
                        }
                    }
                    if ((result == true && resistances.Contains(mre)))
                    {
                        if (infectionByResistance[mre].Healed)
                        {
                            infectionByResistance[mre].Healed = false;
                            //What happens if he is positive again?
                        }
                        infectionByResistance[mre].ConsecutiveNegativeCounter = 0;
                    }
                }
            }
        }

        private bool HasOldKnownCase(Pathogen pathogen, string resistance, SortedList<Hospitalization, Dictionary<string, Dictionary<string, InfectionStatus>>> infectionInformationByCase)
        {
            foreach (Hospitalization hospi in infectionInformationByCase.Keys)
            {
                if (infectionInformationByCase[hospi].ContainsKey(pathogen.ID))
                {
                    if (infectionInformationByCase[hospi][pathogen.ID].ContainsKey(resistance))
                    {
                        if ((infectionInformationByCase[hospi][pathogen.ID][resistance].Nosocomial || infectionInformationByCase[hospi][pathogen.ID][resistance].Known) && !infectionInformationByCase[hospi][pathogen.ID][resistance].Healed)
                        {
                            return true;
                        }
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
