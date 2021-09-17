using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.REST;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientStay.Stationary.ReceiveModel;

namespace SmICSCoreLib.Factories.PatientStay.Stationary
{
    public class StationaryFactory : IStationaryFactory
    {
        public IRestDataAccess _restData;

        public StationaryFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<StationaryDataModel> Process(string patientId, string fallkennung, DateTime datum)
        {
            List<StationaryDataReceiveModel> stationaryDataReceives = _restData.AQLQuery<StationaryDataReceiveModel>(AQLCatalog.Stationary(patientId, fallkennung, datum));

            if (stationaryDataReceives is null)
            {
                return new List<StationaryDataModel>();
            }

            return StationaryConstructor(stationaryDataReceives);
        }

        public List<StationaryDataModel> ProcessFromCase(string patientId, string fallId)
        {
            List<StationaryDataReceiveModel> stationaryDataReceives = _restData.AQLQuery<StationaryDataReceiveModel>(AQLCatalog.StayFromCase(patientId, fallId));

            if (stationaryDataReceives is null)
            {
                return new List<StationaryDataModel>();
            }

            return StationaryConstructor(stationaryDataReceives);
        }

        public List<StationaryDataModel> ProcessFromDate(DateTime datum)
        {
            List<StationaryDataReceiveModel> stationaryDataReceives = _restData.AQLQuery<StationaryDataReceiveModel>(AQLCatalog.StayFromDate(datum));

            if (stationaryDataReceives is null)
            {
                return new List<StationaryDataModel>();
            }

            return StationaryConstructor(stationaryDataReceives);
        }

        private List<StationaryDataModel> StationaryConstructor(List<StationaryDataReceiveModel> stationaryDataReceives)
        {
            List<StationaryDataModel> stationaryDataModels = new List<StationaryDataModel>();
            foreach (StationaryDataReceiveModel dataReceiveModel in stationaryDataReceives)
            {
                stationaryDataModels.Add(new StationaryDataModel(dataReceiveModel));
            }
            return stationaryDataModels;
        }

    }
}
