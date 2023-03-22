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
using System.Threading.Tasks;

namespace SmICSWebApp.Data.WardView
{
    public class WardOverviewService
    {
        private readonly IPatientStayFactory _stayFac;
        private readonly IInfectionStatusFactory _infectionStatusFac;
        private readonly ILabResultFactory _labFac;
        private readonly IHospitalizationFactory _hospitalizationFac;
        private readonly IHelperFactory _helperFac;

        public event EventHandler Progress;

        private List<WardPatient> wardPatients;
        public WardOverviewService(IPatientStayFactory stayFac, IInfectionStatusFactory infectionStatusFac, ILabResultFactory labFac, IHospitalizationFactory hospitalizationFactory, IHelperFactory helperFac)
        {
            _stayFac = stayFac;
            _infectionStatusFac = infectionStatusFac;
            _labFac = labFac;
            _hospitalizationFac = hospitalizationFactory;
            _helperFac = helperFac;
        }

        public async Task<List<WardPatient>> GetData(WardParameter parameter)
        {
            try
            {
                //throw new Exception("Ooops Somethhin went wrong. \n Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.");
                wardPatients = new List<WardPatient>();
                PathogenParameter pathogenParameter = new PathogenParameter() { PathogenCodes = parameter.PathogenCode };

                List<HospStay> possibleContactHosp = await _hospitalizationFac.ProcessAsync(parameter.Start, parameter.End);
                if (possibleContactHosp is not null)
                {
                    //progress.Report("Determine Cases on Ward");
                    List<Case> casesOnWard = await _helperFac.GetPatientOnWardsFromFilteredAsync(possibleContactHosp, parameter.Ward);
                    if (casesOnWard is not null)
                    {
                        foreach (Case _case in casesOnWard)
                        {
                            WardParameter tmpParam = parameter;
                            tmpParam.PatientID = _case.PatientID;
                            tmpParam.CaseID = _case.CaseID;

                            //progress.Report(string.Format("Getting PatientStay Information for {0}", _case.PatientID));
                            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = await _stayFac.ProcessAsync(tmpParam);
                            Hospitalization caseHosp = await _hospitalizationFac.ProcessAsync(_case);

                            if (patientStays is not null)
                            {
                                patientStays = patientStays.OrderBy(stay => stay.Admission).ToList();

                                //progress.Report(string.Format("Calculation InfectionSituation for {0}", _case.PatientID));
                                SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionStatusByCase = await _infectionStatusFac.ProcessAsync(_case, pathogenParameter);
                                Dictionary<string, InfectionStatus> infectionStatus = null;
                                if (infectionStatusByCase.Count > 0 && infectionStatusByCase.Where(h => h.Key.CaseID == _case.CaseID).Count() > 0)
                                {
                                    infectionStatus = infectionStatusByCase.Where(h => h.Key.CaseID == _case.CaseID).First().Value;
                                }
                                List<LabResult> labResults = await _labFac.ProcessAsync(_case, pathogenParameter);

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
                                    if (caseHosp is not null)
                                    {
                                        patient.CaseAdmission = caseHosp.Admission.Date;
                                        patient.CaseDischarge = caseHosp.Discharge.Date;
                                    }
                                    wardPatients.Add(patient);
                                }
                            }
                        }
                    }
                    return wardPatients.OrderBy(v => v.Admission).ToList();
                }
                return null;
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<string, SortedDictionary<DateTime, int>> GetChartEntries(WardParameter parameter,
            List<WardPatient> patients)
        {
            Dictionary<string, SortedDictionary<DateTime, int>> chartEntries = InitializeChartEntryDictionary(parameter.Start, parameter.End);

            foreach (WardPatient patient in patients)
            {
                if (IsNosocomial(patient))
                {
                    DateTime infectionDate = NosocomialInfectionDate(patient);

                    if (NosocomialWithinOverviewParameter(patient, parameter, infectionDate))
                    {
                        chartEntries["Nosokomial"][infectionDate.Date] += 1;
                        IncrementStress(chartEntries, infectionDate.Date, patient.Discharge.HasValue && patient.Discharge.Value <= parameter.End ? patient.Discharge.Value : parameter.End);
                    }
                    else if (NosocomialBeforeOverwievParameter(patient, parameter, infectionDate))
                    {
                        IncrementStress(chartEntries, parameter.Start.Date, patient.Discharge.HasValue && patient.Discharge.Value <= parameter.End ? patient.Discharge.Value : parameter.End);
                    }
                }
                else if (IsKnown(patient))
                {
                    if (patient.Admission.Date >= parameter.Start.Date)
                    {
                        chartEntries["Known"][patient.Admission.Date] += 1;
                        IncrementStress(chartEntries, patient.Admission.Date, patient.Discharge.HasValue && patient.Discharge.Value <= parameter.End ? patient.Discharge.Value : parameter.End);
                    }
                    else
                    {
                        IncrementStress(chartEntries, parameter.Start.Date, patient.Discharge.HasValue && patient.Discharge.Value <= parameter.End ? patient.Discharge.Value : parameter.End);
                    }
                }
            }
            return chartEntries;
        }

        private bool IsNosocomial(WardPatient patient)
        {
            if (patient.InfectionStatus is not null)
            {
                if (patient.InfectionStatus.Values.Any(x =>
                x.Nosocomial
                && x.NosocomialDate.Value.Date > patient.Admission.Date.AddDays(2.0)
                && (patient.Discharge.HasValue ? x.NosocomialDate <= patient.Discharge : true)))
                {
                    return true;
                }
            }
            return false;
        }
        private bool NosocomialWithinOverviewParameter(WardPatient patient, WardParameter parameter, DateTime infectionDate)
        {
            return infectionDate.Date >= parameter.Start.Date
                    && infectionDate.Date <= parameter.End.Date
                    && (!patient.Discharge.HasValue || infectionDate.Date <= patient.Discharge.Value);
        }
        private DateTime NosocomialInfectionDate(WardPatient patient)
        {
            return patient.InfectionStatus.Values.Where(inf => inf.Nosocomial).OrderBy(inf => inf.NosocomialDate).Select(inf => inf.NosocomialDate).First().Value;
        }
        private bool NosocomialBeforeOverwievParameter(WardPatient patient, WardParameter parameter, DateTime infectionDate)
        {
            return infectionDate.Date < parameter.Start.Date
            && (!patient.Discharge.HasValue || infectionDate.Date <= patient.Discharge.Value);
        }
        private bool IsKnown(WardPatient patient)
        {
            if (patient.InfectionStatus is not null)
            {
                if ((patient.InfectionStatus.Values.Any(x => x.Known)
                    || patient.InfectionStatus.Values.Any(x => x.Nosocomial
                    && x.NosocomialDate.Value.Date <= patient.Admission.Date.AddDays(2.0))))
                {
                    return true;
                }
            }
            return false;
        }
        private void IncrementStress(Dictionary<string, SortedDictionary<DateTime, int>> ChartEntries, DateTime start, DateTime end)
        {
            for (DateTime date = start.Date; date <= end.Date; date = date.Date.AddDays(1.0))
            {
                ChartEntries["Stress"][date.Date] += 1;
            }
        }


        private Dictionary<string, SortedDictionary<DateTime, int>> InitializeChartEntryDictionary(DateTime start, DateTime end)
        {
            Dictionary<string, SortedDictionary<DateTime, int>> chartEntries = new Dictionary<string, SortedDictionary<DateTime, int>>();

            chartEntries.Add("Nosokomial", new SortedDictionary<DateTime, int>());
            chartEntries.Add("Known", new SortedDictionary<DateTime, int>());
            chartEntries.Add("Stress", new SortedDictionary<DateTime, int>());

            for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1.0))
            {
                chartEntries["Nosokomial"].Add(date, 0);
                chartEntries["Known"].Add(date, 0);
                chartEntries["Stress"].Add(date, 0);
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
                        DateTime? firstTmp = tmp.Count > 0 ? tmp.First() : null;
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
            if (last.Value == DateTime.MinValue)
            {
                last = null;
            }
            if (first.Value == DateTime.MaxValue)
            {
                first = null;
            }
            return (first, last);
        }

        private DateTime? GetFirstPositveLabResultDate(List<LabResult> labResults, PatientStay patStay)
        {
            DateTime? last = DateTime.MaxValue;
            if (labResults is not null)
            {
                foreach (LabResult labResult in labResults)
                {

                    IEnumerable<DateTime> dates = labResult.Specimens.
                        OrderBy(s => s.SpecimenCollectionDateTime).
                        Where(s => s.Pathogens.Any(p => p.Result)).
                        Select(s => s.SpecimenCollectionDateTime);

                    DateTime? tmp = dates.Count() > 0 ? dates.First() : null;
                    if (tmp.HasValue && last.Value > tmp.Value)
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
                    IEnumerable<Specimen> sorted = labResult.Specimens.Where(s => s.Pathogens.Any(p => p.Result))
                        .OrderBy(s => s.SpecimenCollectionDateTime);
                    if (sorted is not null)
                    {
                        DateTime tmp = sorted.Last().SpecimenCollectionDateTime;
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
