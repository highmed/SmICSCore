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
                    PathogenParameter pathogenParameter = new PathogenParameter() { Name = parameter.Pathogen };
                    InfectionStatus infectionStatus = _infectionStatusFac.Process(c, pathogenParameter).Last().Value.ContainsKey(parameter.Pathogen) ? _infectionStatusFac.Process(c, pathogenParameter).Last().Value[parameter.Pathogen] : null;
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

            for (DateTime date = parameter.Start.Date; date <= parameter.End.Date; date = date.AddDays(1.0))
            {
                chartEntries["Nosokomial"].Add(date, 0);
                chartEntries["Known"].Add(date, 0);
            }

            foreach (WardOverview overview in wardOverviews)
            {
                if(overview.InfectionStatus != null && overview.InfectionStatus.Nosocomial)
                {
                    chartEntries["Nosokomial"][overview.] += 1;
                } 
                else if(overview.InfectionStatus != null && overview.InfectionStatus.Known)
                {
                    chartEntries["Known"][overview.PatientStay.Admission] += 1;
                }
            }
            return chartEntries;
        }

        
    }
}
