using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Nosocomial
{
    public class InfectionStatusFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IMibiResultFactory _mibiResultFac;
        private readonly IHospitalizationFactory _hospitalizationFac;

        public InfectionStatusFactory(IRestDataAccess restDataAccess, IMibiResultFactory mibiResultFac, IHospitalizationFactory hospitalizationFac)
        {
            RestDataAccess = restDataAccess;
            _mibiResultFac = mibiResultFac;
            _hospitalizationFac = hospitalizationFac;
        }

        public SortedList<Hospitalization, Dictionary<string, InfectionStatus>> Process(Patient patient)
        {
            List<Case> cases = RestDataAccess.AQLQuery<Case>(AQLCatalog.Cases(patient));
            SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionInformationByCase = new SortedList<Hospitalization, Dictionary<string, InfectionStatus>>(new HospitalizationComparer());

            foreach (Case c in cases)
            {
                Hospitalization hospitalization = _hospitalizationFac.Process(c);
                List<MiBiResult> results = _mibiResultFac.Process(c);

                Dictionary<string, InfectionStatus> infectionInformation = new Dictionary<string, InfectionStatus>();

                DetermineInfectionInformation(ref infectionInformation, results, hospitalization, infectionInformationByCase);
                infectionInformationByCase.Add(hospitalization, infectionInformation);

            }
            return infectionInformationByCase;
        }
        private void DetermineInfectionInformation(ref Dictionary<string, InfectionStatus> infectionInformation, List<MiBiResult> results, Hospitalization hospitalization, SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionInformationByCase)
        {
            foreach (MiBiResult result in results)
            {
                foreach (Specimen specimen in result.Specimens)
                {
                    TimeSpan timespan = hospitalization.Admission.Date - specimen.SpecimenCollectionDateTime;
                    foreach (Pathogen pathogen in specimen.Pathogens)
                    {
                        int threshold = GetNosocomialThreshold(pathogen);
                        if (pathogen.Result && timespan.Days < threshold)
                        {
                            AddInfectionInformation(ref infectionInformation, pathogen);
                        }
                        else if (pathogen.Result && timespan.Days >= threshold)
                        {
                            bool hasFoundOldStatus = HasOldKnownCase(ref infectionInformation, pathogen, infectionInformationByCase);
                            if (!hasFoundOldStatus)
                            {
                                AddInfectionInformation(ref infectionInformation, pathogen);
                            }
                        }
                        else if (!pathogen.Result)
                        {
                            HasOldKnownCase(ref infectionInformation, pathogen, infectionInformationByCase);
                        }
                    }
                }
            }
        }
        private bool HasOldKnownCase(ref Dictionary<string, InfectionStatus> infectionInformation, Pathogen pathogen, SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionInformationByCase)
        {
            foreach (Hospitalization hospi in infectionInformationByCase.Keys)
            {
                if (infectionInformationByCase[hospi].ContainsKey(pathogen.Name))
                {
                    if (infectionInformationByCase[hospi][pathogen.Name].Nosocomial || infectionInformationByCase[hospi][pathogen.Name].Known)
                    {
                        AddInfectionInformation(ref infectionInformation, pathogen);
                        return true;
                    }
                }
            }
            return false;
        }
        private void AddInfectionInformation(ref Dictionary<string, InfectionStatus> infectionInformation, Pathogen pathogen)
        {
            InfectionStatus infection = new InfectionStatus() { Known = false, Nosocomial = true };
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
