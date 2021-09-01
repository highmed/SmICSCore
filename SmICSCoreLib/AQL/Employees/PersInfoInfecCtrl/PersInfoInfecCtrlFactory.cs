using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmICSCoreLib.AQL.Employees.PersInfoInfecCtrl
{
    public class PersInfoInfecCtrlFactory : IPersInfoInfecCtrlFactory
    {
        private IRestDataAccess _restData;
        public PersInfoInfecCtrlFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PersInfoInfecCtrlModel> Process(PatientListParameter parameter)
        {

            List<PersInfoInfecCtrlModel> ctList = _restData.AQLQuery<PersInfoInfecCtrlModel>(AQLCatalog.EmployeePersInfoInfecCtrl(parameter));

            if (ctList is null)
            {
                return new List<PersInfoInfecCtrlModel>();
            }

            return ctList;
        }
    }
}
