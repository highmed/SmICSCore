using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.WardView
{
    public class WardOverviewService
    {
        private readonly IPatientStayFactory _stayFac;
        private readonly InfectionStatusFactory _infectionStatusFac;
        private readonly ILabResultFactory _labFac;

        private List<WardPatient> wardPatients;
        public WardOverviewService(IPatientStayFactory stayFac, InfectionStatusFactory infectionStatusFac, ILabResultFactory labFac)
        {
            _stayFac = stayFac;
            _infectionStatusFac = infectionStatusFac;
            _labFac = labFac;
        }

        public List<WardPatient> GetData(WardParameter parameter)
        {
            wardPatients = new List<WardPatient>();
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = _stayFac.Process(parameter);
            if (patientStays != null)
            {
                List<Case> cases = new List<Case>();
                patientStays.ForEach(c => cases.Add(c));
                cases = cases.Where(c => patientStays.Select(s => s.CaseID).Contains(c.CaseID)).ToList();
                foreach (Case c in cases)
                {
                    PathogenParameter pathogenParameter = new PathogenParameter() { Name = parameter.Pathogen };
                    SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionStatusByCase = _infectionStatusFac.Process(c, pathogenParameter);
                    Dictionary<string, InfectionStatus> infectionStatus = infectionStatusByCase.Count > 0 ? infectionStatusByCase.Where(h => h.Key.CaseID == c.CaseID).First().Value : null; 
                    WardPatient patient = new WardPatient();
                    patient.Pathogen = parameter.Pathogen;
                    patient.InfectionStatus = infectionStatus;
                    int entryCount = wardPatients.Where(w => w.PatientID == c.PatientID).Count();
                    PatientStay stay = patientStays.Where(stay => stay.PatientID == c.PatientID).ElementAt(entryCount) ?? null;
                    patient.PatientID = stay.PatientID;
                    patient.Admission = stay.Admission;
                    patient.Discharge = stay.Discharge;
                    List<LabResult> labResults = _labFac.Process(c, pathogenParameter);
                    patient.FirstPositiveResult = GetFirstPositveLabResultDate(labResults, stay);
                    (patient.FirstWardPositiveResult, patient.LastWardResult) = GetFirstAndLastWardLabResultDate(labResults, stay);
                    patient.CurrentResult = GetLastLabResultDate(labResults);
                    wardPatients.Add(patient);
                }
            }
            return wardPatients.OrderBy(v => v.Admission).ToList();
        }


        public Dictionary<string, SortedDictionary<DateTime, int>> GetChartEntries(WardParameter parameter, string filterMRE = null, string filterNosokomial = null)
        {
            Dictionary<string, SortedDictionary<DateTime, int>> chartEntries = new Dictionary<string, SortedDictionary<DateTime, int>>();

            chartEntries.Add("Nosokomial", new SortedDictionary<DateTime, int>());
            chartEntries.Add("Known", new SortedDictionary<DateTime, int>());
            chartEntries.Add("Stress", new SortedDictionary<DateTime, int>());

            for (DateTime date = parameter.Start.Date; date <= parameter.End.Date; date = date.AddDays(1.0))
            {
                chartEntries["Nosokomial"].Add(date, 0);
                chartEntries["Known"].Add(date, 0);
            }

            foreach (WardPatient patient in wardPatients)
            {
                if (string.IsNullOrEmpty(filterMRE) || (patient.InfectionStatus != null && patient.InfectionStatus.ContainsKey(filterMRE)))
                {
                    if (patient.InfectionStatus != null && 
                        patient.InfectionStatus.Values.Any(x => x.Nosocomial && x.NosocomialDate > patient.Admission && (patient.Discharge.HasValue ? x.NosocomialDate <= patient.Discharge : true)) &&
                        (string.IsNullOrEmpty(filterNosokomial) ||
                        filterNosokomial == "Nosokomial"))
                    {
                        DateTime infectionDate = patient.InfectionStatus.Values.Where(inf => inf.Nosocomial).OrderBy(inf => inf.NosocomialDate).Select(inf => inf.NosocomialDate).First().Value;
                        chartEntries["Nosokomial"][infectionDate.Date] += 1;
                    }
                    else if (patient.InfectionStatus != null &&
                        (patient.InfectionStatus.Values.Any(x => x.Known) || patient.InfectionStatus.Values.Any(x => x.Nosocomial && x.NosocomialDate < patient.Admission)) &&
                        (string.IsNullOrEmpty(filterNosokomial) ||
                        filterNosokomial == "Bekannt"))
                    {
                        chartEntries["Known"][patient.Admission.Date] += 1;
                    }
                }
            }

            for (DateTime date = parameter.Start.Date; date <= parameter.End.Date; date = date.AddDays(1.0))
            {
                int stress = chartEntries["Nosokomial"][date] + chartEntries["Known"][date];
                chartEntries["Stress"].Add(date, stress);
                if(date.Date > parameter.Start.Date)
                {
                    chartEntries["Stress"][date.Date] += chartEntries["Stress"][date.Date.AddDays(-1.0)];
                }
            }

            foreach (WardPatient patient in wardPatients)
            {
                if (string.IsNullOrEmpty(filterMRE) || (patient.InfectionStatus != null && patient.InfectionStatus.ContainsKey(filterMRE)))
                {
                    if (string.IsNullOrEmpty(filterMRE))
                    {
                        if (patient.InfectionStatus != null)
                        {
                            var dict = patient.InfectionStatus.Where(infection => infection.Value.Infected).ToList();
                            if (dict.Count > 0 && dict.All(infection => infection.Value.Healed))
                            {
                                DateTime latestHealedDate = patient.InfectionStatus.Max(infection => infection.Value.HealedDate.Value);
                                if (chartEntries["Stress"].ContainsKey(latestHealedDate.Date))
                                {
                                    DecrementSince(latestHealedDate, chartEntries["Stress"]);
                                    continue;
                                }
                            }
                        }
                    }
                    else if (patient.InfectionStatus != null && patient.InfectionStatus.ContainsKey(filterMRE))
                    {
                        if (patient.InfectionStatus[filterMRE].Healed && chartEntries["Stress"].ContainsKey(patient.InfectionStatus[filterMRE].HealedDate.Value.Date))
                        {
                            DecrementSince(patient.InfectionStatus[filterMRE].HealedDate.Value.Date, chartEntries["Stress"]);
                            continue;
                        }
                    }
                }
                if (patient.Discharge.HasValue && chartEntries["Stress"].ContainsKey(patient.Discharge.Value.Date.AddDays(1.0)))
                {
                    DecrementSince(patient.Discharge.Value.Date.AddDays(1.0), chartEntries["Stress"]);
                }
            }      
            return chartEntries;
        }

        private void DecrementSince(DateTime dt, SortedDictionary<DateTime, int> chartEntry)
        {
            for (DateTime date = dt.Date; date <= chartEntry.Keys.Last().Date; date = date.AddDays(1.0))
            {
                chartEntry[date.Date] -= 1;
            }
        }
        public List<string> GetFilter(WardParameter parameter)
        {
            List<string> filter = Rules.GetPossibleMREClasses(parameter.Pathogen);
            return filter;
        }

        private (DateTime?, DateTime?) GetFirstAndLastWardLabResultDate(List<LabResult> labResults, PatientStay patStay)
        {
            DateTime last = DateTime.MinValue;
            DateTime first = DateTime.MaxValue;
            foreach (LabResult labResult in labResults)
            {
                if (labResult.Sender.Ward == patStay.Ward)
                {
                    List<DateTime> tmp = labResult.Specimens.
                        OrderBy(s => s.SpecimenCollectionDateTime).
                        Where(s => s.SpecimenCollectionDateTime >= patStay.Admission && (patStay.Discharge.HasValue ? s.SpecimenCollectionDateTime <= patStay.Discharge.Value : true)).
                        Select(s => s.SpecimenCollectionDateTime).
                        ToList();
                    DateTime? firstTmp = tmp.FirstOrDefault();
                    DateTime? lastTmp = tmp.LastOrDefault();
                    if (lastTmp.HasValue && last < lastTmp.Value)
                    {
                        last = lastTmp.Value;
                    }
                    if (firstTmp.HasValue && first > firstTmp.Value)
                    {
                        first = firstTmp.Value;
                    }
                }

            }
            return ((last == DateTime.MinValue ? null : last), (first == DateTime.MaxValue ? null : first));
        }

        private DateTime? GetFirstPositveLabResultDate(List<LabResult> labResults, PatientStay patStay)
        {
            DateTime last = DateTime.MaxValue;
            foreach (LabResult labResult in labResults)
            {
                if (labResult.Sender.Ward == patStay.Ward)
                {
                    IEnumerable<DateTime> dates = labResult.Specimens.
                        OrderBy(s => s.SpecimenCollectionDateTime).
                        Where(s => s.Pathogens.Any(p => p.Result)).
                        Select(s => s.SpecimenCollectionDateTime);

                    DateTime? tmp = dates.Count() > 0 ? dates.First() : null;
                    if (tmp.HasValue && last > tmp.Value)
                    {
                        last = tmp.Value;
                    }
                }

            }
            return last == DateTime.MaxValue ? null : last;
        }

        private DateTime? GetLastLabResultDate(List<LabResult> labResults)
        {
            DateTime last = DateTime.MinValue;
            foreach (LabResult labResult in labResults)
            {
                DateTime tmp = labResult.Specimens.OrderBy(s => s.SpecimenCollectionDateTime).Last().SpecimenCollectionDateTime;
                if (last < tmp)
                {
                    last = tmp;
                }
            }
            return last == DateTime.MinValue ? null : last;
        }
    }
}
