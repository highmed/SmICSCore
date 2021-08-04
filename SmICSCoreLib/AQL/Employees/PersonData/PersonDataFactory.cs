using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Employees.PersonData
{
    public class PersonDataFactory : IPersonDataFactory
    {
        private IRestDataAccess _restData;
        public PersonDataFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }
        public List<PersonDataModel> Process(PatientListParameter parameter)
        {

            List<PersonDataModel> ctList = _restData.AQLQuery<PersonDataModel>(AQLCatalog.EmployeePersonData(parameter));

            if (ctList is null)
            {
                return new List<PersonDataModel>();
            }

            return ctList;
        }
    }
}
