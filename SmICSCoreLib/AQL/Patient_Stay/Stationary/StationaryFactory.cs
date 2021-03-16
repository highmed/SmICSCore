using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Patient_Stay.Stationary.ReceiveModel;

namespace SmICSCoreLib.AQL.Patient_Stay.Stationary
{
    public class StationaryFactory : IStationaryFactory
    {
        private IRestDataAccess _restData;

        public StationaryFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<StationaryDataModel> Process(string patientId, DateTime datum)
        {
            List<StationaryDataReceiveModel> stationaryDataReceives = _restData.AQLQuery<StationaryDataReceiveModel>(AQLCatalog.Stationary(patientId, datum).Query);

            if (stationaryDataReceives is null)
            {
                return new List<StationaryDataModel>();
            }

            return StationaryConstructor(stationaryDataReceives);
        }
        //Wenn die Fallkennung vorhanden ist

        //public List<StationaryDataModel> Process(string patientId, DateTime datum, string fallkennung)
        //{
        //    List<StationaryDataReceiveModel> stationaryDataReceives = _restData.AQLQuery<StationaryDataReceiveModel>(AQLCatalog.Stationary( patientId, datum, fallkennung).Query);

        //    if (stationaryDataReceives is null)
        //    {
        //        return new List<StationaryDataModel>();
        //    }

        //    return StationaryConstructor(stationaryDataReceives);
        //}


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
