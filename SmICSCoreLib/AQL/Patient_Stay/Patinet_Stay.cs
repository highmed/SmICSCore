using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.AQL.Patient_Stay.Cases;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.WeekCase;

namespace SmICSCoreLib.AQL.Patient_Stay
{
    public class Patinet_Stay : IPatinet_Stay
    {
        private IStationaryFactory _stationaryFactory;
        private ICountFactory _countFactory;
        private ICaseFactory _caseFactory;
        private IWeekCaseFactory _weekCaseFactory;

        public Patinet_Stay(IStationaryFactory stationaryFactory, ICountFactory countFactory, 
            ICaseFactory caseFactory, IWeekCaseFactory weekCaseFactory)
        {
            _stationaryFactory = stationaryFactory;
            _countFactory = countFactory;
            _caseFactory = caseFactory;
            _weekCaseFactory = weekCaseFactory;
        }

        public List<CaseDataModel> Case(DateTime date)
        {
            return _caseFactory.Process(date);
        }

        public List<CountDataModel> CovidPat(string nachweis)
        {
            return _countFactory.Process(nachweis);
        }

        public List<StationaryDataModel> Stationary_Stay(string patientId, string fallkennung, DateTime datum)
        {
            return _stationaryFactory.Process(patientId, fallkennung, datum);
        }

        public List<WeekCaseDataModel> WeekCase(DateTime startDate, DateTime endDate)
        {
            return _weekCaseFactory.Process(startDate, endDate);
        }

        public List<StationaryDataModel> StayFromCase(string patientId, string fallId)
        {
            return _stationaryFactory.ProcessFromCase(patientId, fallId);
        }
        public List<StationaryDataModel> StayFromDate(DateTime datum)
        {
            return _stationaryFactory.ProcessFromDate(datum);
        }

    }
}
