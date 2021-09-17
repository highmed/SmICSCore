using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.Employees.ContactTracing
{
    public class ContactTracingFactory : IContactTracingFactory
    {
        public IRestDataAccess _restData;

        public ContactTracingFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<ContactTracingReceiveModel> Process(PatientListParameter parameter)
        {

            List<ContactTracingReceiveModel> ctList = _restData.AQLQuery<ContactTracingReceiveModel>(AQLCatalog.EmployeeContactTracing(parameter));

            if (ctList is null)
            {
                return new List<ContactTracingReceiveModel>();
            }

            return ctList;
        }

    }
}
