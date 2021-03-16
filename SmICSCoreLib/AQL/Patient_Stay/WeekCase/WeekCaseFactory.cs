using SmICSCoreLib.AQL.Patient_Stay.WeekCase.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.WeekCase
{
    class WeekCaseFactory : IWeekCaseFactory
    {
        private IRestDataAccess _restData;

        public WeekCaseFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<WeekCaseDataModel> Process(DateTime startDate, DateTime endDate)
        {
            List<WeekCaseReceiveModel> weekCaseReceiveModels = _restData.AQLQuery<WeekCaseReceiveModel>(AQLCatalog.WeekCase(startDate, endDate).Query);

            if (weekCaseReceiveModels is  null)
            {
                return new List<WeekCaseDataModel>();
            }

            return WeekCaseConstructor(weekCaseReceiveModels);
        }

        private List<WeekCaseDataModel> WeekCaseConstructor(List<WeekCaseReceiveModel> weekCaseReceiveModels)
        {

            List<WeekCaseDataModel> weekCaseMoldes = new List<WeekCaseDataModel>();

            foreach (WeekCaseReceiveModel weekCase in weekCaseReceiveModels)
            {
                weekCaseMoldes.Add(new WeekCaseDataModel(weekCase));
            }

            return weekCaseMoldes;
        }
    }
}
