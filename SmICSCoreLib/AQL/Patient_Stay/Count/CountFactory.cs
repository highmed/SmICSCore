using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Patient_Stay.Count.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Count
{
    public class CountFactory : ICountFactory
    {
        private IRestDataAccess _restData;

        public CountFactory(IRestDataAccess restData) 
        {
            _restData = restData;
        }

        public List<CountDataModel> Process(string nachweis)
        {
            List<CountDataReceiveModel> countDataReceiveModels = _restData.AQLQuery<CountDataReceiveModel>(AQLCatalog.CovidPat(nachweis));

            if (countDataReceiveModels is null )
            {
                return new List<CountDataModel>();
            }
            return CountConstructor(countDataReceiveModels);
        }

        public List<CountDataModel> ProcessFromID(string nachweis, PatientListParameter parameter)
        {

            List<CountDataReceiveModel> countDataReceiveModels = _restData.AQLQuery<CountDataReceiveModel>(AQLCatalog.CovidPatByID(nachweis, parameter));

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
