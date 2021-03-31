using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmICSCoreLib.AQL.Employees.ContactTracing
{
    public class ContactTracingFactory : IContactTracingFactory
    {
        private IRestDataAccess _restData;
        public ContactTracingFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<ContactTracingModel> Process(PatientListParameter parameter)
        {

            List<ContactTracingModel> ctList = _restData.AQLQuery<ContactTracingModel>(AQLCatalog.EmployeeContactTracing(parameter).Query);

            if (ctList is null)
            {
                return new List<ContactTracingModel>();
            }

            return ctList;
        }
    }
}
