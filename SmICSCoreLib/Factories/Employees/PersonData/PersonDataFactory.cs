using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            List<PersonDataModel> ctList = _restData.AQLQuery<PersonDataModel>(AQLCatalog.EmployeePersonData(parameter));

            if (ctList is null)
            {
                return new List<PersonDataModel>();
            }

            return ctList;
        }
    }
}
