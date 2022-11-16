using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.PersonData
{
    public class PersonDataFactory : IPersonDataFactory
    {
        public IRestDataAccess _restData;
        public PersonDataFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PersonDataModel> Process(PatientListParameter parameter)
        {

            List<PersonDataModel> ctList = _restData.AQLQueryAsync<PersonDataModel>(AQLCatalog.EmployeePersonData(parameter)).GetAwaiter().GetResult();

            if (ctList is null)
            {
                return new List<PersonDataModel>();
            }

            return ctList;
        }
    }
}
