using System;
using System.Collections.Generic;
using SmICSCoreLib.Factories.PatientStay.Count;
using SmICSCoreLib.Factories.PatientStay.Stationary;

namespace SmICSCoreLib.Factories.PatientStay
{
    public class PatientStay : IPatientStay
    {
        private IStationaryFactory _stationaryFactory;
        private ICountFactory _countFactory;
        public PatientStay(IStationaryFactory stationaryFactory, ICountFactory countFactory)
        {
            _stationaryFactory = stationaryFactory;
            _countFactory = countFactory;
        }
        public List<CountDataModel> CovidPat(string nachweis)
        {
            return _countFactory.Process(nachweis);
        }
        public List<StationaryDataModel> Stationary_Stay(string patientId, string fallkennung, DateTime datum)
        {
            return _stationaryFactory.Process(patientId, fallkennung, datum);
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
