using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientStay.Count.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Count
{
    public class CountFactory : ICountFactory
    {
        public IRestDataAccess _restData;

        public CountFactory(IRestDataAccess restData) 
        {
            _restData = restData;
        }

        public List<CountDataModel> Process(string nachweis)
        {
            List<CountDataReceiveModel> countDataReceiveModels = _restData.AQLQueryAsync<CountDataReceiveModel>(AQLCatalog.CovidPat(nachweis)).GetAwaiter().GetResult();

            if (countDataReceiveModels is null )
            {
                return new List<CountDataModel>();
            }
            return CountConstructor(countDataReceiveModels);
        }

        public List<CountDataModel> ProcessFromID(string nachweis, PatientListParameter parameter)
        {

            List<CountDataReceiveModel> countDataReceiveModels = _restData.AQLQueryAsync<CountDataReceiveModel>(AQLCatalog.CovidPatByID(nachweis, parameter)).GetAwaiter().GetResult();

            if (countDataReceiveModels is null)
            {
                return new List<CountDataModel>();
            }
            return CountConstructor(countDataReceiveModels);
        }
        private List<CountDataModel> CountConstructor(List<CountDataReceiveModel> countDataReceiveModels)
        {
            List<CountDataModel> countDataModels = new List<CountDataModel>();
            foreach (CountDataReceiveModel dataReceiveModel in countDataReceiveModels)
            {
                countDataModels.Add(new CountDataModel(dataReceiveModel));
            }
            return countDataModels;
        }

    }
}
