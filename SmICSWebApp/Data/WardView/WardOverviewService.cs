using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmICSWebApp.Data.WardView
{
    public class WardOverviewService
    {
        private readonly IPatientStayFactory _stayFac;
        private readonly InfectionStatusFactory _infectionStatusFac;
        private readonly ILabResultFactory _labFac;
        public WardOverviewService(IPatientStayFactory stayFac, InfectionStatusFactory infectionStatusFac, ILabResultFactory labFac)
        {
            _stayFac = stayFac;
            _infectionStatusFac = infectionStatusFac;
            _labFac = labFac;
        }

        public List<WardOverview> GetData(WardParameter parameter)
        {
            List<WardOverview> wardOverviews = new List<WardOverview>();
            List<SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay> patientStays = _stayFac.Process(parameter);
            if (patientStays != null)
            {
                List<Case> cases = new List<Case>();
                patientStays.ForEach(c => cases.Add(c));

                foreach (Case c in cases)
                {
                    PathogenParameter pathogenParameter = new PathogenParameter() { Name = parameter.Pathogen };
                    InfectionStatus infectionStatus = _infectionStatusFac.Process(c, pathogenParameter).Last().Value[parameter.Pathogen];
                    WardOverview overview = new WardOverview();
                    overview.InfectionStatus = infectionStatus;
                    overview.PatientStay = patientStays.Where(stay => stay.PatientID == c.PatientID).FirstOrDefault();
                    overview.LabData = _labFac.Process(c, pathogenParameter);

                    wardOverviews.Add(overview);
                }
            }
            return wardOverviews;
        }
    }
}
