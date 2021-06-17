using SmICSCoreLib.AQL.MiBi.WardOverview;
using SmICSCoreLib.AQL.PatientInformation.PatientData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSWebApp.Data
{
    public class MibiViewService
    {
        private IWardOverviewFactory _mibi;
        private IPatientDataFactory _patient;

        public MibiViewService(IPatientDataFactory patient, IWardOverviewFactory mibi)
        {
            _patient = patient;
            _mibi = mibi;
        }

        public List<MibiViewModel> GetData(WardOverviewParameters parameter)
        {
            List<MibiViewModel> mibiViews = new List<MibiViewModel>();
            List<WardOverviewModel> overviewData = _mibi.Process(parameter);
            foreach (WardOverviewModel overview in overviewData)
            {
                PatientData patData = _patient.Process(overview.PatientID);
                MibiViewModel mibiView = new MibiViewModel(patData, overview);
                mibiViews.Add(mibiView);
            }
            return mibiViews;
        }

    }
}
