using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
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

        private List<WardOverview> wardOverviews;
        public WardOverviewService(IPatientStayFactory stayFac, InfectionStatusFactory infectionStatusFac, ILabResultFactory labFac)
        {
            _stayFac = stayFac;
            _infectionStatusFac = infectionStatusFac;
            _labFac = labFac;
        }

        public List<WardOverview> GetData(WardParameter parameter)
        {
            wardOverviews = new List<WardOverview>();
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = _stayFac.Process(parameter);
            if (patientStays != null)
            {
                List<Case> cases = new List<Case>();
                patientStays.ForEach(c => cases.Add(c));

                foreach (Case c in cases)
                {
                    if(c.PatientID == "Patient103")
                    {
                        Console.WriteLine("");
                    }
                    PathogenParameter pathogenParameter = new PathogenParameter() { Name = parameter.Pathogen };
                    Dictionary<string, InfectionStatus> infectionStatus = _infectionStatusFac.Process(c, pathogenParameter).Last().Value.ContainsKey(parameter.Pathogen) ? _infectionStatusFac.Process(c, pathogenParameter).Last().Value[parameter.Pathogen] : null;
                    WardOverview overview = new WardOverview();
                    overview.InfectionStatus = infectionStatus;
                    overview.PatientStay = patientStays.Where(stay => stay.PatientID == c.PatientID).FirstOrDefault();
                    overview.LabData = _labFac.Process(c, pathogenParameter);

                    wardOverviews.Add(overview);
                }
            }
            return wardOverviews;
        }

        public Dictionary<string, SortedDictionary<DateTime, int>> GetChartEntries(WardParameter parameter)
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

            foreach (WardOverview overview in wardOverviews)
            {
                if(overview.InfectionStatus != null && overview.InfectionStatus.Values.Any(x => x.Nosocomial && x.NosocomialDate > overview.PatientStay.Admission && (overview.PatientStay.Discharge.HasValue ? x.NosocomialDate <= overview.PatientStay.Discharge : true)))
                {
                    DateTime infectionDate = overview.InfectionStatus.Values.Where(inf => inf.Nosocomial).OrderBy(inf => inf.NosocomialDate).Select(inf => inf.NosocomialDate).First().Value;
                    chartEntries["Nosokomial"][infectionDate.Date] += 1;
                } 
                else if(overview.InfectionStatus != null && (overview.InfectionStatus.Values.Any(x => x.Known) || overview.InfectionStatus.Values.Any(x => x.Nosocomial && x.NosocomialDate < overview.PatientStay.Admission)))
                {
                    chartEntries["Known"][overview.PatientStay.Admission.Date] += 1;
                }
            }

            for (DateTime date = parameter.Start.Date; date <= parameter.End.Date; date = date.AddDays(1.0))
            {
                int stress = chartEntries["Nosokomial"][date] + chartEntries["Known"][date];
                if (date.Date > parameter.Start.Date)
                {
                    stress += chartEntries["Stress"][date.Date.AddDays(-1.0)];
                    int stressRelease = wardOverviews.Count(ward => ward.PatientStay.Discharge.HasValue && ward.PatientStay.Discharge.Value.Date.AddDays(-1.0) == date.Date.AddDays(-1.0));
                    stress -= stressRelease;
                }
                chartEntries["Stress"].Add(date, stress);
            }
            return chartEntries;
        }

        
    }
}
