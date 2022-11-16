using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl
{
    public class PersInfoInfecCtrlFactory : IPersInfoInfecCtrlFactory
    {
        public IRestDataAccess _restData;
        public PersInfoInfecCtrlFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PersInfoInfecCtrlModel> Process(PatientListParameter parameter)
        {

            List<PersInfoInfecCtrlModel> ctList = _restData.AQLQueryAsync<PersInfoInfecCtrlModel>(AQLCatalog.EmployeePersInfoInfecCtrl(parameter)).GetAwaiter().GetResult();

            if (ctList is null)
            {
                return new List<PersInfoInfecCtrlModel>();
            }

            return ctList;
        }
    }
}
