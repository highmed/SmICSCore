using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Helpers;
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
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IHelperFactory _helperFac;

        public event EventHandler Progress;

        private List<WardPatient> wardPatients;
        public WardOverviewService(IPatientStayFactory stayFac, InfectionStatusFactory infectionStatusFac, ILabResultFactory labFac, IHospitalizationFactory hospitalizationFactory, IHelperFactory helperFac)
        {
            _stayFac = stayFac;
            _infectionStatusFac = infectionStatusFac;
            _labFac = labFac;
            _hospitalizationFac = hospitalizationFactory;
            _helperFac = helperFac;
        }

        public List<WardPatient> GetData(WardParameter parameter)
        {
            wardPatients = new List<WardPatient>();
            PathogenParameter pathogenParameter = new PathogenParameter() { PathogenCodes = parameter.PathogenCode };
            Progress?.Invoke("Searching for fitting Cases", EventArgs.Empty);
            List<HospStay> possibleContactHosp = _hospitalizationFac.Process(parameter.Start, parameter.End);
            if (possibleContactHosp is not null)
            {
                Progress?.Invoke("Determine Cases on Ward", EventArgs.Empty);
                List<Case> casesOnWard = _helperFac.GetPatientOnWardsFromFiltered(possibleContactHosp, parameter.Ward);
                if(casesOnWard is not null)
                {
                    foreach (Case _case in casesOnWard)
                    {
                        WardParameter tmpParam = parameter;
                        tmpParam.PatientID = _case.PatientID;
                        tmpParam.CaseID = _case.CaseID;

                        Progress?.Invoke(string.Format("Getting PatientStay Information for {0}", _case.PatientID), EventArgs.Empty);
                        List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = _stayFac.Process(tmpParam);
                        if (patientStays is not null)
                        {
                            patientStays = patientStays.OrderBy(stay => stay.Admission).ToList();

                            Progress?.Invoke(string.Format("Calculation InfectionSituation for {0}", _case.PatientID), EventArgs.Empty);
                            SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionStatusByCase = _infectionStatusFac.Process(_case, pathogenParameter);
                            Dictionary<string, InfectionStatus> infectionStatus = null;
                            if (infectionStatusByCase.Count > 0 && infectionStatusByCase.Where(h => h.Key.CaseID == _case.CaseID).Count() > 0)
                            {
                                infectionStatus = infectionStatusByCase.Where(h => h.Key.CaseID == _case.CaseID).First().Value;
                            }
                            List<LabResult> labResults = _labFac.Process(_case, pathogenParameter);

                            foreach (PatientStay stay in patientStays)
                            {
                                WardPatient patient = new WardPatient();
                                patient.PathogenCodes = parameter.PathogenCode;
                                patient.InfectionStatus = infectionStatus;
                                patient.PatientID = stay.PatientID;
                                patient.Admission = stay.Admission;
                                patient.Discharge = stay.Discharge;
                                patient.CaseID = stay.CaseID;
                                patient.FirstPositiveResult = GetFirstPositveLabResultDate(labResults, stay);
                                (patient.FirstWardPositiveResult, patient.LastWardResult) = GetFirstAndLastWardLabResultDate(labResults, stay);
                                patient.CurrentResult = GetLastLabResultDate(labResults);
                                wardPatients.Add(patient);
                            }
                        }
                    }
                } 
                return wardPatients.OrderBy(v => v.Admission).ToList();
            }
            return null;
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
                chartEntries["Stress"].Add(date, 0);
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
                        if (infectionDate.Date >= parameter.Start.Date && (!patient.Discharge.HasValue || infectionDate.Date <= patient.Discharge.Value))
                        {
                            chartEntries["Nosokomial"][infectionDate.Date] += 1;
                        }
                        else if(infectionDate.Date < parameter.Start.Date && (!patient.Discharge.HasValue || infectionDate.Date <= patient.Discharge.Value))
                        {
                            chartEntries["Stress"][parameter.Start.Date] += 1;
                        }
                    }
                    else if (patient.InfectionStatus != null &&
                        (patient.InfectionStatus.Values.Any(x => x.Known) || patient.InfectionStatus.Values.Any(x => x.Nosocomial && x.NosocomialDate < patient.Admission)) &&
                        (string.IsNullOrEmpty(filterNosokomial) ||
                        filterNosokomial == "Bekannt"))
                    {
                        if (patient.Admission.Date >= parameter.Start.Date)
                        {
                            chartEntries["Known"][patient.Admission.Date] += 1;
                        }
                        else
                        {
                            chartEntries["Stress"][parameter.Start.Date] += 1;
                        }
                    }
                }
            }

            for (DateTime date = parameter.Start.Date; date <= parameter.End.Date; date = date.AddDays(1.0))
            {
                int stress = chartEntries["Nosokomial"][date] + chartEntries["Known"][date];
                chartEntries["Stress"][date] += stress;
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

                if ((string.IsNullOrEmpty(filterNosokomial) || filterNosokomial == "Nosokomial") 
                    && patient.Discharge.HasValue && patient.InfectionStatus is not null
                    && (patient.InfectionStatus.Values.Any(i => (i.Nosocomial && i.NosocomialDate >= patient.Admission && i.NosocomialDate <= patient.Discharge.Value)))
                    && chartEntries["Stress"].ContainsKey(patient.Discharge.Value.Date.AddDays(1.0)))
                {
                    DecrementSince(patient.Discharge.Value.Date.AddDays(1.0), chartEntries["Stress"]);
                }
                else if ((string.IsNullOrEmpty(filterNosokomial) || filterNosokomial == "Bekannt") 
                    && patient.Discharge.HasValue && patient.InfectionStatus is not null
                    && (patient.InfectionStatus.Values.Any(i => i.Known || (i.Nosocomial && i.NosocomialDate < patient.Admission)))
                    && chartEntries["Stress"].ContainsKey(patient.Discharge.Value.Date.AddDays(1.0)))
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
            List<string> filter = Rules.GetPossibleMREClasses(parameter.PathogenCode);
            return filter;
        }

        private (DateTime?, DateTime?) GetFirstAndLastWardLabResultDate(List<LabResult> labResults, PatientStay patStay)
        {
            DateTime? last = DateTime.MinValue;
            DateTime? first = DateTime.MaxValue;
            if (labResults is not null)
            {
                foreach (LabResult labResult in labResults)
                {
                    if (labResult.Sender.Ward == patStay.Ward)
                    {
                        List<DateTime> tmp = labResult.Specimens.
                            OrderBy(s => s.SpecimenCollectionDateTime).
                            Where(s => s.SpecimenCollectionDateTime >= patStay.Admission && (patStay.Discharge.HasValue ? s.SpecimenCollectionDateTime <= patStay.Discharge.Value : true) && s.Pathogens.Any(p => p.Result)).
                            Select(s => s.SpecimenCollectionDateTime).
                            ToList();
                        DateTime? firstTmp = tmp.FirstOrDefault();
                        DateTime? lastTmp = tmp.LastOrDefault();
                        if (lastTmp.HasValue && last.Value < lastTmp.Value)
                        {
                            last = lastTmp.Value;
                        }
                        if (firstTmp.HasValue && first.Value > firstTmp.Value)
                        {
                            first = firstTmp.Value;
                        }
                    }

                }
            }
            if(last.Value == DateTime.MinValue)
            {
                last = null;
            }
            if (first.Value == DateTime.MaxValue)
            {
                first = null;
            }
            return (last, first);
        }

        private DateTime? GetFirstPositveLabResultDate(List<LabResult> labResults, PatientStay patStay)
        {
            DateTime last = DateTime.MaxValue;
            if (labResults is not null)
            {
                foreach (LabResult labResult in labResults)
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
            if (labResults is not null)
            {
                foreach (LabResult labResult in labResults)
                {
                    DateTime tmp = labResult.Specimens.Where(s => s.Pathogens.Any(p => p.Result))
                        .OrderBy(s => s.SpecimenCollectionDateTime)
                        .Last().SpecimenCollectionDateTime;
                    if (tmp != null)
                    {
                        if (last < tmp)
                        {
                            last = tmp;
                        }
                    }
                }
            }
            return last == DateTime.MinValue ? null : last;
        }

    }
}
