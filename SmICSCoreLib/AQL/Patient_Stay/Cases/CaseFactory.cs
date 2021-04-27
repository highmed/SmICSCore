using SmICSCoreLib.AQL.Patient_Stay.Cases.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Cases
{
    class CaseFactory : ICaseFactory
    {
        private IRestDataAccess _restData;

        public CaseFactory(IRestDataAccess restData) 
        {
            _restData = restData ;
        }
        public List<CaseDataModel> Process( DateTime datum)
        {
            List<CaseDataReceiveModel> caseDataReceiveModels = _restData.AQLQuery<CaseDataReceiveModel>(AQLCatalog.Case(datum));

            if (caseDataReceiveModels is null)
            {
                return new List<CaseDataModel>();
            }
            return CaseConstructor(caseDataReceiveModels);

        }

        private List<CaseDataModel> CaseConstructor(List<CaseDataReceiveModel> caseDataReceiveModels)
        {
            List<CaseDataModel> caseDataModels = new List<CaseDataModel>();
            foreach (CaseDataReceiveModel dataReceiveModel in caseDataReceiveModels)
            {
                caseDataModels.Add(new CaseDataModel(dataReceiveModel));
            }
            return caseDataModels;
        }
    }
}
